'Rudy Earnest
'RCET 3371
'Fall 2025
'Smart Home HVAC Controller Application
'Git link: https://github.com/rc-earnest/HVACSmartHomeController.git
Option Strict On
Option Explicit On
Option Compare Text
Imports System.IO.Ports
Imports System.Runtime.InteropServices
Imports System.Windows.Forms

Public Class HVACForm
    Public ports As String()
    Public buttonsPressed As String
    Public bitArray() As String
    Public disable As Boolean = False
    Public outputStatus As Byte
    Public heatEnable As Boolean = False
    Public coolEnable As Boolean = False
    Public fanEnabled As Boolean = False
    Public fanOnlyMode As Boolean = False
    Public override As Boolean = False
    ' ISU Color Palette
    Public GrowlGreyLight As Color = Color.FromArgb(230, 231, 232)
    Public GrowlGreyMed As Color = Color.FromArgb(167, 167, 167)
    Public GrowlGrey As Color = Color.FromArgb(130, 130, 130)
    Public Roarange As Color = Color.FromArgb(244, 121, 32)
    Public RoarangeL As Color = Color.FromArgb(246, 146, 64)
    Public BengalBlack As Color = Color.FromArgb(0, 0, 0)

    ' Reconnect helpers
    Private reconnectTimer As System.Windows.Forms.Timer
    Private isReconnecting As Boolean = False

    ' Remember last successful port so reconnect attempts target the same port
    Private lastPortName As String = String.Empty

    Private lowSetpoint As Decimal = 50D
    Private highSetpoint As Decimal = 90D
    Private Const MinSetpoint As Decimal = 50D
    Private Const MaxSetpoint As Decimal = 90D
    Private Const SetpointStep As Decimal = 0.5D

    ' Fault logging lock
    Private ReadOnly faultLogLock As New Object()
    Private fanOff As Boolean = False

    Private lastFanToggle As DateTime = DateTime.MinValue
    Private Const FanToggleLockMs As Integer = 2000 ' 2 seconds debounce for fan toggle

    Sub Connect()
        ' Get available ports
        ports = ComPort.GetPortNames()

        If ports Is Nothing OrElse ports.Length = 0 Then
            MessageBox.Show("No Com Ports Detected. Please connect the HVAC Controller and restart the application.", "No Com Ports", MessageBoxButtons.OK, MessageBoxIcon.Error)
            ComsStatusToolStripStatusLabel.Text = "No Com Ports Detected"
            Return
        End If

        ' If we already have a remembered port, try that first
        If Not String.IsNullOrEmpty(lastPortName) Then
            If TryOpenPort(lastPortName) Then
                Return
            End If
            ' If trying remembered port failed, fall back to scanning all ports below
        End If

        Dim triedAny As Boolean = False
        For Each p As String In ports
            triedAny = True
            If TryOpenPort(p) Then
                Return
            End If
        Next

        ' If we reached here, no port matched
        If Not triedAny Then
            ComsStatusToolStripStatusLabel.Text = "Not Connected"
        Else
            ComsStatusToolStripStatusLabel.Text = "Not Connected"
        End If
    End Sub

    ' Attempts to open and verify the device at the specified port.
    ' Returns True on success and sets lastPortName.
    Private Function TryOpenPort(p As String) As Boolean
        Dim expectedByte As Byte = CByte(AscW("Q"c)) ' ASCII 'Q' = 81 (&H51)
        Dim probe() As Byte = {&B11110000} ' one byte to write
        Dim buffer(63) As Byte ' space for up to 64 bytes

        Try
            ' Ensure closed before changing PortName
            If ComPort.IsOpen Then
                Try
                    ComPort.Close()
                Catch
                End Try
            End If

            ComPort.PortName = p
            ComPort.BaudRate = 19200
            ComPort.Parity = Parity.None
            ComPort.StopBits = StopBits.One
            ComPort.DataBits = 8
            ComPort.ReadTimeout = 500
            ComPort.WriteTimeout = 500

            ComPort.Open()
            ComPort.DiscardInBuffer()
            ComPort.DiscardOutBuffer()

            ' send probe
            ComPort.Write(probe, 0, 1)

            ' Wait up to ~500ms for response, reading into buffer as bytes arrive.
            Dim start = DateTime.Now
            Dim totalRead As Integer = 0
            Dim requiredIndex As Integer = 58 ' 58th byte -> index 58 (adjust if you meant index 57)

            While (DateTime.Now - start).TotalMilliseconds < 500
                Dim available = ComPort.BytesToRead
                If available > 0 Then
                    Dim toRead = Math.Min(available, buffer.Length - totalRead)
                    Dim actuallyRead = ComPort.Read(buffer, totalRead, toRead)
                    totalRead += actuallyRead
                    If totalRead > requiredIndex Then
                        Exit While
                    End If
                End If
                System.Threading.Thread.Sleep(20)
            End While

            ' If we have the required byte and it matches 'Q', we're connected
            If totalRead > requiredIndex AndAlso buffer(requiredIndex) = expectedByte Then
                lastPortName = ComPort.PortName
                If Me.InvokeRequired Then
                    Me.Invoke(Sub() ComsStatusToolStripStatusLabel.Text = $"Connected to {ComPort.PortName}")
                Else
                    ComsStatusToolStripStatusLabel.Text = $"Connected to {ComPort.PortName}"
                End If
                SendTimer.Enabled = True
                TempTimer.Enabled = True
                Return True
            End If

            ' Not matched - close port and return false
            If ComPort.IsOpen Then
                ComPort.Close()
            End If
        Catch ex As Exception
            ' swallow exceptions per original behavior, but ensure port closed
            Try
                If ComPort IsNot Nothing AndAlso ComPort.IsOpen Then
                    ComPort.Close()
                End If
            Catch
            End Try
        End Try

        Return False
    End Function

    ' Called when a disconnection is detected; updates UI and starts reconnect attempts.
    Private Sub OnPortDisconnected(reason As String)
        If isReconnecting Then
            ' already handling reconnect; just update status
            If Me.InvokeRequired Then
                Me.Invoke(Sub() ComsStatusToolStripStatusLabel.Text = "Disconnected")
            Else
                ComsStatusToolStripStatusLabel.Text = "Disconnected"
            End If
            Return
        End If

        ' Log the communication loss event
        LogFault("Communication lost with QY@ board - attempting to reconnect")

        isReconnecting = True

        ' Ensure UI updates happen on UI thread
        If Me.InvokeRequired Then
            Me.Invoke(Sub()
                          ComsStatusToolStripStatusLabel.Text = "Disconnected"
                          MessageBox.Show($"COM port disconnected: {reason}{Environment.NewLine}The app will attempt to reconnect every 10 seconds.", "COM Disconnected", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                      End Sub)
        Else
            ComsStatusToolStripStatusLabel.Text = "Disconnected"
            MessageBox.Show($"COM port disconnected: {reason}{Environment.NewLine}The app will attempt to reconnect every 10 seconds.", "COM Disconnected", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End If

        ' Close the port if still open
        Try
            If ComPort IsNot Nothing AndAlso ComPort.IsOpen Then
                ComPort.Close()
            End If
        Catch
        End Try

        ' Start (or ensure started) the reconnect timer
        If reconnectTimer Is Nothing Then
            reconnectTimer = New System.Windows.Forms.Timer()
            reconnectTimer.Interval = 10000 ' 10 seconds
            AddHandler reconnectTimer.Tick, AddressOf ReconnectTimer_Tick
        End If

        If Not reconnectTimer.Enabled Then
            reconnectTimer.Start()
        End If
    End Sub

    ' Timer tick: attempt to reconnect by trying the last-known port only.
    Private Sub ReconnectTimer_Tick(sender As Object, e As EventArgs)
        ' Update status immediately
        If Me.InvokeRequired Then
            Me.Invoke(Sub() ComsStatusToolStripStatusLabel.Text = "Attempting reconnection...")
        Else
            ComsStatusToolStripStatusLabel.Text = "Attempting reconnection..."
        End If

        Try
            ' If we have a remembered port, attempt only that port repeatedly.
            If Not String.IsNullOrEmpty(lastPortName) Then
                If Me.InvokeRequired Then
                    ' update status to include port name on UI thread
                    Me.Invoke(Sub() ComsStatusToolStripStatusLabel.Text = $"Attempting reconnection to {lastPortName}...")
                Else
                    ComsStatusToolStripStatusLabel.Text = $"Attempting reconnection to {lastPortName}..."
                End If

                If TryOpenPort(lastPortName) Then
                    ' successfully reconnected
                    isReconnecting = False
                    If reconnectTimer IsNot Nothing AndAlso reconnectTimer.Enabled Then
                        reconnectTimer.Stop()
                    End If
                    If Me.InvokeRequired Then
                        Me.Invoke(Sub()
                                      ComsStatusToolStripStatusLabel.Text = $"Reconnected to {ComPort.PortName}"
                                      MessageBox.Show($"Reconnected to {ComPort.PortName}.", "COM Reconnected", MessageBoxButtons.OK, MessageBoxIcon.Information)
                                  End Sub)
                    Else
                        ComsStatusToolStripStatusLabel.Text = $"Reconnected to {ComPort.PortName}"
                        MessageBox.Show($"Reconnected to {ComPort.PortName}.", "COM Reconnected", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    End If
                    Return
                End If

                ' failed to reconnect to remembered port; we'll try again next tick (do not scan other ports)
                Return
            Else
                ' No remembered port - fall back to scanning all ports once
                Connect()
            End If
        Catch
            ' ignore exceptions - Connect handles its own exceptions
        End Try
    End Sub

    ' Serial port event handlers to detect errors / disconnection
    Private Sub ComPort_ErrorReceived(sender As Object, e As SerialErrorReceivedEventArgs)
        Try
            OnPortDisconnected($"Serial error: {e.EventType}")
        Catch
        End Try
    End Sub

    Private Sub ComPort_PinChanged(sender As Object, e As SerialPinChangedEventArgs)
        Try
            ' If a pin state changed and the port is no longer available, treat as disconnected
            If Not ComPort.IsOpen Then
                OnPortDisconnected($"Pin change: {e.EventType}")
            End If
        Catch
        End Try
    End Sub

    Private Sub HVACForm_Load(sender As Object, e As EventArgs) Handles Me.Load
        Me.BackColor = GrowlGrey
        StatusStrip.BackColor = GrowlGrey
        LowSetpointDownButton.BackColor = Roarange
        LowSetpointUpButton.BackColor = Roarange
        HighSetpointDownButton.BackColor = Roarange
        HighSetpointUpButton.BackColor = Roarange
        ExitButton.BackColor = Roarange

        ' Initialize setpoint displays and make them read-only in this form
        UpdateLowSetpointDisplay()
        UpdateHighSetpointDisplay()
        LowSetpointRichTextBox.ReadOnly = True
        LowSetpointRichTextBox.ShortcutsEnabled = False
        LowSetpointRichTextBox.TabStop = False
        HighSetpointRichTextBox.ReadOnly = True
        HighSetpointRichTextBox.ShortcutsEnabled = False
        HighSetpointRichTextBox.TabStop = False

        ' Make temperature displays read-only as well
        RoomTempRichTextBox.ReadOnly = True
        RoomTempRichTextBox.ShortcutsEnabled = False
        RoomTempRichTextBox.TabStop = False
        MachineTempRichTextBox.ReadOnly = True
        MachineTempRichTextBox.ShortcutsEnabled = False
        MachineTempRichTextBox.TabStop = False

        AutoRadioButton.Checked = True
        OffRadioButton.Checked = True
        ComsStatusToolStripStatusLabel.Text = "Not Connected"

        ' Hook serial port events and prepare reconnect timer
        Try
            AddHandler ComPort.ErrorReceived, AddressOf ComPort_ErrorReceived
            AddHandler ComPort.PinChanged, AddressOf ComPort_PinChanged
        Catch
        End Try

        reconnectTimer = New System.Windows.Forms.Timer()
        reconnectTimer.Interval = 10000
        AddHandler reconnectTimer.Tick, AddressOf ReconnectTimer_Tick

        For port = 0 To ComPort.GetPortNames.Length - 1
            ports = ComPort.GetPortNames()
        Next
        Connect()
    End Sub

    Private Sub ExitButton_Click(sender As Object, e As EventArgs) Handles ExitButton.Click, ExitToolStripMenuItem.Click
        ' Stop timer and close port cleanly on exit
        Try
            SendTimer.Enabled = False
            FiveSecondTimer.Enabled = False
            If reconnectTimer IsNot Nothing AndAlso reconnectTimer.Enabled Then
                reconnectTimer.Stop()
            End If
            If ComPort IsNot Nothing AndAlso ComPort.IsOpen Then
                ComPort.Close()
            End If
        Catch
        End Try

        Me.Close()
    End Sub
    Private Sub SendTimer_Tick(sender As Object, e As EventArgs) Handles SendTimer.Tick
        Try
            If bitArray IsNot Nothing Then
                Dim anyone As Boolean = False
                For i As Integer = 0 To bitArray.Length - 1
                    If bitArray(i) = "1" Then
                        anyone = True
                        Exit For
                    End If
                Next
                If anyone = True Then
                    If bitArray(7) = "0" Then
                        DigitalInput1()
                    End If
                    If bitArray(7) = "1" Then
                        DigitalInput1T()
                    End If
                    If bitArray(6) = "0" Then
                        DigitalInput2()
                    End If
                    If bitArray(5) = "0" Then
                        DigitalInput3()
                    End If
                    If bitArray(3) = "0" Then
                        DigitalInput5()
                    End If
                End If
            End If
        Catch ex As Exception
            ' Ignore exceptions from input handling
        End Try

        ' Auto control: enable when outside thresholds; when inside setpoint range turn everything off
        Try
            Dim roomText As String = RoomTempRichTextBox.Text.Replace("°F", "").Trim()
            Dim machineText As String = MachineTempRichTextBox.Text.Replace("°F", "").Trim()
            Dim roomVal As Decimal
            Dim machineVal As Decimal

            If Decimal.TryParse(roomText, System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, roomVal) _
               AndAlso Decimal.TryParse(machineText, System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, machineVal) Then

                If Not disable Then
                    ' Use room temperature for decisions (room is what needs heating/cooling).
                    ' If room is well below low setpoint -> heat
                    If roomVal <= (lowSetpoint - 2D) Then
                        If Not heatEnable Then
                            heatEnable = True
                            coolEnable = False
                            fanEnabled = True
                            outputStatus = CByte(outputStatus Or CType(&H8, Byte))
                            outputStatus = CByte(outputStatus And CType(&HFD, Byte))
                            Dim dataOn(1) As Byte
                            dataOn(0) = &H20
                            dataOn(1) = outputStatus
                            Try
                                ComPort.Write(dataOn, 0, 2)
                            Catch ex As Exception
                                SendTimer.Enabled = False
                                OnPortDisconnected("Error sending data: " & ex.Message)
                            End Try
                            Dim msgOn As String = $"Auto: Heating enabled (room {roomVal:F1}°F <= low setpoint -2 ({lowSetpoint - 2D:F1}°F))"
                            FaultToolStripStatusLabel.Text = msgOn
                            LogFault(msgOn)
                            FiveSecondTimer.Enabled = True
                            override = False
                            ' update UI
                            HeatRadioButton.Checked = True
                        End If
                        ' If room is well above high setpoint -> cool
                    ElseIf roomVal >= (highSetpoint + 2D) Then
                        If Not coolEnable Then
                            coolEnable = True
                            heatEnable = False
                            fanEnabled = True
                            outputStatus = CByte(outputStatus Or CType(&H8, Byte))
                            outputStatus = CByte(outputStatus And CType(&HFB, Byte))
                            Dim dataOn(1) As Byte
                            dataOn(0) = &H20
                            dataOn(1) = outputStatus
                            Try
                                ComPort.Write(dataOn, 0, 2)
                            Catch ex As Exception
                                SendTimer.Enabled = False
                                OnPortDisconnected("Error sending data: " & ex.Message)
                            End Try
                            Dim msgOn As String = $"Auto: Cooling enabled (room {roomVal:F1}°F >= high setpoint +2 ({highSetpoint + 2D:F1}°F))"
                            FaultToolStripStatusLabel.Text = msgOn
                            LogFault(msgOn)
                            FiveSecondTimer.Enabled = True
                            override = False
                            ' update UI
                            CoolRadioButton.Checked = True
                        End If
                    Else
                        ' Room is within the user-set bounds [lowSetpoint .. highSetpoint]
                        ' If either heating or cooling is running, turn them off and leave fan on for 5 seconds
                        If override = False Then
                            If heatEnable Or coolEnable Then

                                heatEnable = False
                                coolEnable = False
                                fanEnabled = True
                                outputStatus = CByte(outputStatus Or CType(&H8, Byte)) ' set fan
                                outputStatus = CByte(outputStatus And CType(&HFB, Byte)) ' clear heat
                                outputStatus = CByte(outputStatus And CType(&HFD, Byte)) ' clear cool
                                Dim dataOff(1) As Byte
                                dataOff(0) = &H20
                                dataOff(1) = outputStatus
                                Try
                                    ComPort.Write(dataOff, 0, 2)
                                Catch ex As Exception
                                    SendTimer.Enabled = False
                                    OnPortDisconnected("Error sending data: " & ex.Message)
                                End Try
                                Dim msgOff As String = $"Auto: Heating/Cooling turned off (room {roomVal:F1}°F within setpoints {lowSetpoint:F1}-{highSetpoint:F1}); fan on 5s"
                                FaultToolStripStatusLabel.Text = msgOff
                                LogFault(msgOff)
                                ' trigger fan-off sequence
                                FiveSecondTimer.Enabled = True
                                fanOff = True
                                ' update UI to Off while fan runs
                                OffRadioButton.Checked = True
                            End If
                        End If
                    End If
                End If
            End If
        Catch
            ' keep SendTimer stable
        End Try

        Dim dataSend(1) As Byte
        dataSend(0) = &H53
        dataSend(1) = &H30
        If ComPort.IsOpen Then
            Try
                'ComPort.DiscardInBuffer()
                ComPort.Write(dataSend, 0, 1)
                ComPort.Write(dataSend, 1, 1)
            Catch ex As Exception
                SendTimer.Enabled = False
                OnPortDisconnected("Error sending data: " & ex.Message)
            End Try
        End If
    End Sub

    Private Sub ComPort_DataReceived(sender As Object, e As SerialDataReceivedEventArgs) Handles ComPort.DataReceived
        If SendTimer.Enabled = False Then
            Return
        End If
        CheckForIllegalCrossThreadCalls = False
        Dim incomingData As Integer = ComPort.BytesToRead
        Dim buffer(incomingData - 1) As Byte
        Dim recieved% = ComPort.Read(buffer, 0, incomingData)
        Static roomTemp#
        Static machineTemp#
        Static buttons%
        Dim Temp1%
        Dim Temp2%
        If recieved > 0 Then
            If buffer.Length >= 5 Then
                Temp1 = CInt(buffer(0)) * 4
                Temp2 = CInt((buffer(1)) / 64)
                machineTemp = Temp1 + Temp2
                Temp1 = CInt(buffer(2)) * 4
                Temp2 = CInt((buffer(3)) / 64)
                roomTemp = Temp1 + Temp2
                buttons = CInt(buffer(4))
                buttonsPressed = $"{ByteToBinary(buffer(4))}"
                RoomTempRichTextBox.Text = CDec(($"{((60 / 1023) * (roomTemp)) + 40}")).ToString("000.00°F")
                MachineTempRichTextBox.Text = CDec(($"{((60 / 1023) * (machineTemp)) + 40}")).ToString("000.00°F")
            End If
        End If
    End Sub

    Sub DigitalInput1()
        disable = True
        OffRadioButton.Checked = True
        FaultToolStripStatusLabel.Text = "Fault: Safety Interlock Triggered"
        LogFault("Fault: Safety Interlock Triggered")
        Dim data(1) As Byte
        outputStatus = &H1
        data(0) = &H20
        data(1) = outputStatus
        Try
            ComPort.Write(data, 0, 2)
        Catch ex As Exception
            SendTimer.Enabled = False
            OnPortDisconnected("Error sending data: " & ex.Message)
        End Try
    End Sub
    Sub DigitalInput1T()
        disable = False
        FaultToolStripStatusLabel.Text = "Fault:"
        Dim data(1) As Byte
        outputStatus = (outputStatus And CType(&HFE, Byte))
        data(0) = &H20
        data(1) = outputStatus
        Try
            ComPort.Write(data, 0, 2)
        Catch ex As Exception
            SendTimer.Enabled = False
            OnPortDisconnected("Error sending data: " & ex.Message)
        End Try
    End Sub

    Sub DigitalInput2()
        If disable = True Then
            Return
        End If
        coolEnable = False
        heatEnable = True
        fanEnabled = True
        override = True
        Dim data(1) As Byte
        outputStatus = (outputStatus Or CType(&H8, Byte))
        data(0) = &H20
        data(1) = outputStatus
        Try
            ComPort.Write(data, 0, 2)
        Catch ex As Exception
            SendTimer.Enabled = False
            OnPortDisconnected("Error sending data: " & ex.Message)
        End Try
        FiveSecondTimer.Enabled = True
        ' update UI
    End Sub

    Sub DigitalInput3()
        If disable = True Then
            Return
        End If

        Dim now As DateTime = DateTime.Now
        ' Ignore toggles that happen within the debounce window
        If (now - lastFanToggle).TotalMilliseconds < FanToggleLockMs Then
            Return
        End If

        ' Record the toggle time immediately to prevent re-entrant toggles
        lastFanToggle = now

        Dim data(1) As Byte

        ' Toggle fan-only mode: second call turns fan off
        If fanOnlyMode Then
            ' Turn fan-only mode off
            fanOnlyMode = False
            fanEnabled = False
            override = False
            ' clear fan bit (bit 3 / 0x08)
            outputStatus = CByte(outputStatus And CType(&HF7, Byte))
            data(0) = &H20
            data(1) = outputStatus
            Try
                ComPort.Write(data, 0, 2)
            Catch ex As Exception
                SendTimer.Enabled = False
                OnPortDisconnected("Error sending data: " & ex.Message)
            End Try

            ' Update UI: if heat or cool still active show them, otherwise auto
            If heatEnable Then
                HeatRadioButton.Checked = True
            ElseIf coolEnable Then
                CoolRadioButton.Checked = True
            Else
                AutoRadioButton.Checked = True
            End If
            LogFault("Fan-only mode turned off by digital input")
        Else
            ' Turn fan-only mode on
            fanOnlyMode = True
            fanEnabled = True
            override = True
            outputStatus = (outputStatus Or CType(&H8, Byte))
            data(0) = &H20
            data(1) = outputStatus
            Try
                ComPort.Write(data, 0, 2)
            Catch ex As Exception
                SendTimer.Enabled = False
                OnPortDisconnected("Error sending data: " & ex.Message)
            End Try

            ' Update UI to fan mode (use your fan radio control name)
            OnRadioButton.Checked = True
            LogFault("Fan-only mode enabled by digital input")
            FiveSecondTimer.Enabled = True
        End If
    End Sub

    Sub DigitalInput4()
        disable = True
        FaultToolStripStatusLabel.Text = "Fault: Pressure Sensor"
        LogFault("Fault: Pressure Sensor")
        If fanEnabled = True Or fanOnlyMode = True Then
            TwoMinuteTimer.Enabled = True
        End If
        If fanEnabled = False And fanOnlyMode = False Then
            TwoMinuteTimer.Enabled = False
        End If
    End Sub
    Sub DigitalInput4T()
        disable = False
        FaultToolStripStatusLabel.Text = "Fault:"
        If fanEnabled = True Or fanOnlyMode = True Then
            TwoMinuteTimer.Enabled = True
        End If
        If fanEnabled = False And fanOnlyMode = False Then
            TwoMinuteTimer.Enabled = False
        End If
    End Sub

    Sub DigitalInput5()
        If disable = True Then
            Return
        End If
        coolEnable = True
        fanEnabled = True
        heatEnable = False
        override = True
        Dim data(1) As Byte
        outputStatus = (outputStatus Or CType(&H8, Byte))
        data(0) = &H20
        data(1) = outputStatus
        Try
            ComPort.Write(data, 0, 2)
        Catch ex As Exception
            SendTimer.Enabled = False
            OnPortDisconnected("Error sending data: " & ex.Message)
        End Try
        FiveSecondTimer.Enabled = True
    End Sub

    Public Function ByteToBinary(ByVal b As Byte) As String()
        ' Create an 8-character binary string, MSB..LSB
        Dim bin As String = Convert.ToString(b, 2).PadLeft(8, "0"c)

        ' Prepare an array where index 0 = LSB (bit0) ... index 7 = MSB (bit7)
        Dim bits(7) As String
        For i As Integer = 0 To 7
            ' bin(0) is MSB, bin(7) is LSB; reverse so bits(0) == LSB
            bits(i) = bin(7 - i).ToString()
        Next

        ' Store into the class-level Array field for existing code that reads Array(0) etc.
        Me.bitArray = bits

        Return bits
    End Function

    Private Sub FiveSecondTimer_Tick(sender As Object, e As EventArgs) Handles FiveSecondTimer.Tick
        Try
            ' Existing behavior: process inputs if present
            If bitArray IsNot Nothing Then
                If bitArray(4) = "1" Then
                    DigitalInput4T()
                End If
                If bitArray(4) = "0" Then
                    DigitalInput4()
                End If
            End If

            ' If heating still on, send heat output
            If heatEnable = True Then
                Dim data(1) As Byte
                outputStatus = &HC
                data(0) = &H20
                data(1) = outputStatus
                Try
                    ComPort.Write(data, 0, 2)
                Catch ex As Exception
                    SendTimer.Enabled = False
                    OnPortDisconnected("Error sending data: " & ex.Message)
                End Try
                ' ensure UI reflects heating
                HeatRadioButton.Checked = True
            End If

            ' If cooling still on, send cool output
            If coolEnable = True Then
                heatEnable = False
                Dim data(1) As Byte
                outputStatus = &HA
                data(0) = &H20
                data(1) = outputStatus
                Try
                    ComPort.Write(data, 0, 2)
                Catch ex As Exception
                    SendTimer.Enabled = False
                    OnPortDisconnected("Error sending data: " & ex.Message)
                End Try
                ' ensure UI reflects cooling
                CoolRadioButton.Checked = True
            End If

            ' If we're in the "fan on for 5 seconds" phase, clear the fan bit now
            If fanOff Then
                fanOff = False
                fanEnabled = False
                ' clear fan bit (bit 3 / 0x08)
                outputStatus = CByte(outputStatus And CType(&HF7, Byte)) ' 0xF7 clears bit3
                Dim data(1) As Byte
                data(0) = &H20
                data(1) = outputStatus
                Try
                    ComPort.Write(data, 0, 2)
                Catch ex As Exception
                    SendTimer.Enabled = False
                    OnPortDisconnected("Error sending data: " & ex.Message)
                End Try
                LogFault("Auto: Fan turned off after 5s")

                ' Update UI: if heat/cool still active show them, otherwise auto
                If heatEnable Then
                    HeatRadioButton.Checked = True
                ElseIf coolEnable Then
                    CoolRadioButton.Checked = True
                Else
                    AutoRadioButton.Checked = True
                End If
            End If
        Catch
            ' Keep timer stable
        Finally
            FiveSecondTimer.Enabled = False
        End Try
    End Sub

    Private Sub TwoMinuteTimer_Tick(sender As Object, e As EventArgs) Handles TwoMinuteTimer.Tick
        If bitArray(4) = "1" Then
            DigitalInput4T()
        End If
        If bitArray(4) = "0" Then
            DigitalInput4()
        End If
    End Sub

    Private Sub TempTimer_Tick(sender As Object, e As EventArgs) Handles TempTimer.Tick
        Try
            ' Parse numeric values from the UI (format: "000.00°F")
            Dim roomText As String = RoomTempRichTextBox.Text.Replace("°F", "").Trim()
            Dim machineText As String = MachineTempRichTextBox.Text.Replace("°F", "").Trim()
            Dim roomVal As Decimal
            Dim machineVal As Decimal

            If Decimal.TryParse(roomText, System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, roomVal) _
               AndAlso Decimal.TryParse(machineText, System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, machineVal) Then

                ' When heating is active, machine should be warmer than room
                If heatEnable AndAlso Not disable Then
                    If machineVal <= roomVal Then
                        Dim msg As String = "Fault: Machine not warmer than room while heating"
                        FaultToolStripStatusLabel.Text = msg
                        LogFault(msg)
                    Else
                        ' clear this specific fault if condition corrected
                        If FaultToolStripStatusLabel.Text.StartsWith("Fault: Machine not warmer") Then
                            FaultToolStripStatusLabel.Text = "Fault:"
                        End If
                    End If
                End If

                ' When cooling is active, machine should be cooler than room
                If coolEnable AndAlso Not disable Then
                    If machineVal >= roomVal Then
                        Dim msg As String = "Fault: Machine not cooler than room while cooling"
                        FaultToolStripStatusLabel.Text = msg
                        LogFault(msg)
                    Else
                        If FaultToolStripStatusLabel.Text.StartsWith("Fault: Machine not cooler") Then
                            FaultToolStripStatusLabel.Text = "Fault:"
                        End If
                    End If
                End If
            End If
        Catch
            ' Swallow any parse/runtime errors to keep timer stable
        End Try
    End Sub

    Private Sub UpdateLowSetpointDisplay()
        LowSetpointRichTextBox.Text = lowSetpoint.ToString("F1") & "°F"
    End Sub

    Private Sub UpdateHighSetpointDisplay()
        HighSetpointRichTextBox.Text = highSetpoint.ToString("F1") & "°F"
    End Sub

    ' Low setpoint increase / decrease handlers
    Private Sub LowSetpointUpButton_Click(sender As Object, e As EventArgs) Handles LowSetpointUpButton.Click
        If lowSetpoint + SetpointStep <= MaxSetpoint Then
            lowSetpoint += SetpointStep
            UpdateLowSetpointDisplay()
        End If
    End Sub

    Private Sub LowSetpointDownButton_Click(sender As Object, e As EventArgs) Handles LowSetpointDownButton.Click
        If lowSetpoint - SetpointStep >= MinSetpoint Then
            lowSetpoint -= SetpointStep
            UpdateLowSetpointDisplay()
        End If
    End Sub

    ' High setpoint increase / decrease handlers
    Private Sub HighSetpointUpButton_Click(sender As Object, e As EventArgs) Handles HighSetpointUpButton.Click
        If highSetpoint + SetpointStep <= MaxSetpoint Then
            highSetpoint += SetpointStep
            UpdateHighSetpointDisplay()
        End If
    End Sub

    Private Sub HighSetpointDownButton_Click(sender As Object, e As EventArgs) Handles HighSetpointDownButton.Click
        If highSetpoint - SetpointStep >= MinSetpoint Then
            highSetpoint -= SetpointStep
            UpdateHighSetpointDisplay()
        End If
    End Sub

    ' Allow the user to type a setpoint by double-clicking the setpoint display.
    ' Prompts, validates range (50.0-90.0) and 0.5°F increments, updates the setpoint and display.

    Private Sub LowSetpointRichTextBox_DoubleClick(sender As Object, e As EventArgs) Handles LowSetpointRichTextBox.DoubleClick
        Dim input As String = Microsoft.VisualBasic.Interaction.InputBox($"Enter low setpoint ({MinSetpoint:F1} - {MaxSetpoint:F1}) in °F (0.5° increments):", "Low Setpoint", lowSetpoint.ToString("F1"))
        If String.IsNullOrWhiteSpace(input) Then
            Return ' user cancelled
        End If

        Dim value As Decimal
        If Not Decimal.TryParse(input.Trim(), value) Then
            MessageBox.Show("Invalid number format. Use a decimal value like 65 or 65.5.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        If value < MinSetpoint OrElse value > MaxSetpoint Then
            MessageBox.Show($"Setpoint must be between {MinSetpoint:F1}°F and {MaxSetpoint:F1}°F.", "Out of Range", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        ' Ensure value is a multiple of 0.5
        Dim doubled As Decimal = value * 2D
        Dim nearest As Integer = CInt(Math.Round(CDbl(doubled)))
        If Math.Abs(CDbl(doubled - CDec(nearest))) > 0.0001 Then
            MessageBox.Show("Setpoint must be in 0.5°F increments (e.g. 72.0, 72.5).", "Invalid Increment", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        ' Normalize to nearest 0.5 and assign
        lowSetpoint = CDec(nearest) / 2D
        UpdateLowSetpointDisplay()
    End Sub

    Private Sub HighSetpointRichTextBox_DoubleClick(sender As Object, e As EventArgs) Handles HighSetpointRichTextBox.DoubleClick
        Dim input As String = Microsoft.VisualBasic.Interaction.InputBox($"Enter high setpoint ({MinSetpoint:F1} - {MaxSetpoint:F1}) in °F (0.5° increments):", "High Setpoint", highSetpoint.ToString("F1"))
        If String.IsNullOrWhiteSpace(input) Then
            Return ' user cancelled
        End If

        Dim value As Decimal
        If Not Decimal.TryParse(input.Trim(), value) Then
            MessageBox.Show("Invalid number format. Use a decimal value like 75 or 75.5.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        If value < MinSetpoint OrElse value > MaxSetpoint Then
            MessageBox.Show($"Setpoint must be between {MinSetpoint:F1}°F and {MaxSetpoint:F1}°F.", "Out of Range", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        ' Ensure value is a multiple of 0.5
        Dim doubled As Decimal = value * 2D
        Dim nearest As Integer = CInt(Math.Round(CDbl(doubled)))
        If Math.Abs(CDbl(doubled - CDec(nearest))) > 0.0001 Then
            MessageBox.Show("Setpoint must be in 0.5°F increments (e.g. 72.0, 72.5).", "Invalid Increment", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        ' Normalize to nearest 0.5 and assign
        highSetpoint = CDec(nearest) / 2D
        UpdateHighSetpointDisplay()
    End Sub

    ' Logs fault messages to a file named HVACSystem-YYmmDD.log two directory levels up from the application's folder.
    Private Sub LogFault(detail As String)
        Try
            Dim fileName As String = $"HVACSystem-{DateTime.Now:yyMMdd}.log"
            Dim baseDir As String = IO.Path.GetFullPath(IO.Path.Combine(Application.StartupPath, "..", "..", ".."))
            If Not IO.Directory.Exists(baseDir) Then
                IO.Directory.CreateDirectory(baseDir)
            End If
            Dim path As String = IO.Path.Combine(baseDir, fileName)
            Dim entry As String = $"{DateTime.Now:yyMMdd-HHmmss}: {detail}"
            SyncLock faultLogLock
                Using sw As New IO.StreamWriter(path, True)
                    sw.WriteLine(entry)
                End Using
            End SyncLock
        Catch
            ' avoid throwing from logging
        End Try
    End Sub
End Class