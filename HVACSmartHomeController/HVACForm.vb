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
            ComPort.ReadTimeout = 2000
            ComPort.WriteTimeout = 2000

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

    '' Serial port event handlers to detect errors / disconnection
    'Private Sub ComPort_ErrorReceived(sender As Object, e As SerialErrorReceivedEventArgs)
    '    Try
    '        OnPortDisconnected($"Serial error: {e.EventType}")
    '    Catch
    '    End Try
    'End Sub

    'Private Sub ComPort_PinChanged(sender As Object, e As SerialPinChangedEventArgs)
    '    Try
    '        ' If a pin state changed and the port is no longer available, treat as disconnected
    '        If Not ComPort.IsOpen Then
    '            OnPortDisconnected($"Pin change: {e.EventType}")
    '        End If
    '    Catch
    '    End Try
    'End Sub

    Private Sub HVACForm_Load(sender As Object, e As EventArgs) Handles Me.Load
        Me.BackColor = GrowlGrey
        StatusStrip.BackColor = GrowlGrey
        LowSetpointDownButton.BackColor = Roarange
        LowSetpointUpButton.BackColor = Roarange
        HighSetpointDownButton.BackColor = Roarange
        HighSetpointUpButton.BackColor = Roarange
        ExitButton.BackColor = Roarange
        LowSetpointRichTextBox.Text = $"{50}°F"
        HighSetpointRichTextBox.Text = $"{90}°F"
        AutoRadioButton.Checked = True
        OffRadioButton.Checked = True
        ComsStatusToolStripStatusLabel.Text = "Not Connected"

        ' Hook serial port events and prepare reconnect timer
        'Try
        '    AddHandler ComPort.ErrorReceived, AddressOf ComPort_ErrorReceived
        '    AddHandler ComPort.PinChanged, AddressOf ComPort_PinChanged
        'Catch
        'End Try

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
        Dim data(1) As Byte
        data(0) = &H53
        data(1) = &H30
        Try
            ComPort.DiscardInBuffer()
            ComPort.Write(data, 0, 1)
            ComPort.Write(data, 1, 1)
        Catch ex As Exception
            SendTimer.Enabled = False
            OnPortDisconnected("Error sending data: " & ex.Message)
        End Try
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
            Try
                If bitArray IsNot Nothing Then

                    If bitArray(7) = "0" Then
                        DigitalInput1()
                    End If
                    If bitArray(7) = "1" Then
                        DigitalInput1T()
                    End If
                    If bitArray(6) = "1" Then
                        'DigitalInput2()
                    End If
                    If bitArray(5) = "1" Then
                        ' DigitalInput3()
                    End If
                    If bitArray(3) = "1" Then
                        ' DigitalInput5()
                    End If
                End If
            Catch ex As Exception
                ' Ignore exceptions from input handling
            End Try
        End If
    End Sub

    Sub DigitalInput1()
        disable = True
        OffRadioButton.Checked = True
        FaultToolStripStatusLabel.Text = "Fault: Safety Interlock Triggered"
        Dim data(1) As Byte
        outputStatus = (outputStatus Or CType(&H80, Byte))
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
        outputStatus = (outputStatus And CType(&H7F, Byte))
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
        heatEnable = True
        If disable = True Then
            Return
        End If
        Dim data(1) As Byte
        outputStatus = (outputStatus Or CType(&H10, Byte))
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

    Sub DigitalInput3()
        If disable = True Then
            Return
        End If
        Dim data(1) As Byte
        outputStatus = (outputStatus Or CType(&H10, Byte))
        data(0) = &H20
        data(1) = outputStatus
        Try
            ComPort.Write(data, 0, 2)
        Catch ex As Exception
            SendTimer.Enabled = False
            OnPortDisconnected("Error sending data: " & ex.Message)
        End Try
    End Sub

    Sub DigitalInput4()
        disable = True
        FaultToolStripStatusLabel.Text = "Fault: Pressure Sensor"
    End Sub
    Sub DigitalInput4T()
        disable = False
        FaultToolStripStatusLabel.Text = "Fault:"
    End Sub

    Sub DigitalInput5()
        coolEnable = True
        If disable = True Then
            Return
        End If
        Dim data(1) As Byte
        outputStatus = (outputStatus Or CType(&H10, Byte))
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
        If disable = True Then
            Return
        End If
        If bitArray(4) = "1" Then
            DigitalInput4T()
        End If
        If bitArray(4) = "0" Then
            DigitalInput4()
        End If
        If heatEnable = True Then
            coolEnable = False
            Dim data(1) As Byte
            outputStatus = (outputStatus Or CType(&H40, Byte))
            data(0) = &H20
            data(1) = outputStatus
            Try
                ComPort.Write(data, 0, 2)
            Catch ex As Exception
                SendTimer.Enabled = False
                OnPortDisconnected("Error sending data: " & ex.Message)
            End Try
        End If
        If coolEnable = True Then
            heatEnable = False
            Dim data(1) As Byte
            outputStatus = (outputStatus Or CType(&H20, Byte))
            data(0) = &H20
            data(1) = outputStatus
            Try
                ComPort.Write(data, 0, 2)
            Catch ex As Exception
                SendTimer.Enabled = False
                OnPortDisconnected("Error sending data: " & ex.Message)
            End Try
        End If
    End Sub
End Class