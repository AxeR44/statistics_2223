Imports System.ComponentModel
Imports System.IO

Public Class Form1

    Private WithEvents asyncWorker As BackgroundWorker

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Dim newForm As Credits
        newForm = New Credits()
        newForm.Show()
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Me.asyncWorker.RunWorkerAsync()
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.asyncWorker = New BackgroundWorker
    End Sub

    Private Function recursiveFactorial(value As Long) As Long
        If value < 0 Then
            Return -1
        ElseIf value <= 1 Then
            Return 1
        End If
        Return value * recursiveFactorial(value - 1)
    End Function

    Private Sub Work(sender As Object, e As DoWorkEventArgs) Handles asyncWorker.DoWork
        Dim disabler As MethodInvoker = AddressOf disableBox
        Try
            If InvokeRequired Then
                Invoke(disabler)
            End If
            Dim number = Integer.Parse(TextBox1.Text)
            e.Result = recursiveFactorial(number)
        Catch ex As Exception
            e.Result = -1
        End Try
    End Sub

    Private Sub WorkEnded(sender As Object, e As RunWorkerCompletedEventArgs) Handles asyncWorker.RunWorkerCompleted
        Dim res As String
        Dim enabler As MethodInvoker = AddressOf enableBox
        Dim r As Long = Convert.ToInt64(e.Result)
        If r = -1 Then
            res = "Number cannot be less than 0"
        ElseIf r <= 0 Then
            res = "Overflow"
        Else
            res = r.ToString()
        End If
        Label1.Text = "The factorial is: " + res
        Invoke(enabler)
    End Sub

    Private Sub enableBox()
        Me.TextBox1.Enabled = True
    End Sub

    Private Sub disableBox()
        Me.TextBox1.Enabled = False
    End Sub

End Class
