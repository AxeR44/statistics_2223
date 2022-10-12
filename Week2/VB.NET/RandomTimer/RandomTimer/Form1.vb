Imports System.Timers

Public Class Form1
    Private WithEvents Timer As System.Timers.Timer
    Private rnd As Random
    Private isDouble As Boolean = False
    Private timerStarted As DateTime

    Private Sub button1_Click(sender As Object, e As EventArgs) Handles button1.Click
        If Not Me.Timer.Enabled Then
            Me.Timer.Enabled = True
            Me.timerStarted = DateTime.Now
            Me.button1.Text = "Stop Timer"
        Else
            Me.Timer.Enabled = False
            Me.button1.Text = "Start Timer"
        End If
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Timer = New Timers.Timer(1000)
        Me.Timer.AutoReset = True
        Me.rnd = New Random()
    End Sub

    Private Sub OnTimedEvent(sender As Object, e As ElapsedEventArgs) Handles Timer.Elapsed
        Dim elapsedTime As TimeSpan = e.SignalTime - timerStarted
        Invoke(Sub()
                   UpdateUI(elapsedTime)
               End Sub)
    End Sub

    Private Sub UpdateUI(elapsedTime As TimeSpan)
        If elapsedTime.Seconds Mod 2 = 0 Then
            If Me.isDouble Then
                Me.label1.Text = "The random number is: " + Me.rnd.NextDouble().ToString()
            Else
                Me.label1.Text = "The random number is: " + Me.rnd.Next().ToString()
            End If
        End If
        Me.label2.Text = "Time Elapsed: " + elapsedTime.ToString("hh\:mm\:ss")
    End Sub

    Private Sub checkBox1_CheckedChanged(sender As Object, e As EventArgs) Handles checkBox1.CheckedChanged
        Me.isDouble = True
    End Sub

    Private Sub button2_Click(sender As Object, e As EventArgs) Handles button2.Click
        Dim cred As Credits = New Credits()
        cred.Show()
    End Sub
End Class
