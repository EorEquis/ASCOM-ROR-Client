Public Class frmMain

    Dim ShutterState() As String = {"Open", "Closed", "Opening", "Closing", "Error"}
    Private driver As ASCOM.DriverAccess.Dome

    ''' <summary>
    ''' This event is where the driver is choosen. The device ID will be saved in the settings.
    ''' </summary>
    ''' <param name="sender">The source of the event.</param>
    ''' <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
    Private Sub buttonChoose_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles buttonChoose.Click
        My.Settings.DriverId = ASCOM.DriverAccess.Dome.Choose(My.Settings.DriverId)
        SetUIState()
    End Sub

    ''' <summary>
    ''' Connects to the device to be tested.
    ''' </summary>
    ''' <param name="sender">The source of the event.</param>
    ''' <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
    Private Sub buttonConnect_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles buttonConnect.Click
        If (IsConnected) Then
            Timer1.Enabled = False
            driver.Connected = False
        Else
            driver = New ASCOM.DriverAccess.Dome(My.Settings.DriverId)
            driver.Connected = True
            Timer1.Enabled = True
        End If
        SetUIState()
    End Sub

    Private Sub Form1_FormClosing(ByVal sender As System.Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles MyBase.FormClosing
        If IsConnected Then
            driver.Connected = False
        End If
        ' the settings are saved automatically when this application is closed.
    End Sub

    ''' <summary>
    ''' Sets the state of the UI depending on the device state
    ''' </summary>
    Private Sub SetUIState()
        buttonConnect.Enabled = Not String.IsNullOrEmpty(My.Settings.DriverId)
        buttonChoose.Enabled = Not IsConnected
        btnClose.Enabled = IsConnected
        btnOpen.Enabled = IsConnected
        buttonConnect.Text = IIf(IsConnected, "Disconnect", "Connect")
        If IsConnected Then ' Why the hell IIf doesn't work here, I don't know...but it doesn't.  It breaks.
            lblStatus.Text = ShutterState(driver.ShutterStatus)
        Else
            lblStatus.Text = ""
        End If

    End Sub

    ''' <summary>
    ''' Gets a value indicating whether this instance is connected.
    ''' </summary>
    ''' <value>
    ''' 
    ''' <c>true</c> if this instance is connected; otherwise, <c>false</c>.
    ''' 
    ''' </value>
    Private ReadOnly Property IsConnected() As Boolean
        Get
            If Me.driver Is Nothing Then Return False
            Return driver.Connected
        End Get
    End Property

    ' TODO: Add additional UI and controls to test more of the driver being tested.


    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        lblStatus.Text = ShutterState(driver.ShutterStatus)
    End Sub

    Private Sub btnOpen_Click(sender As Object, e As EventArgs) Handles btnOpen.Click
        If IsConnected Then
            driver.OpenShutter()
            btnOpen.Enabled = False
            btnClose.Enabled = True
        End If
    End Sub

    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        If IsConnected Then
            driver.CloseShutter()
            btnOpen.Enabled = True
            btnClose.Enabled = False
        End If
    End Sub

    Private Sub btnHalt_Click(sender As Object, e As EventArgs) Handles btnHalt.Click
        If IsConnected Then
            driver.AbortSlew()
            btnOpen.Enabled = True
            btnClose.Enabled = True
        End If
    End Sub
End Class
