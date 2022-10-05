Imports System.IO

Public Class Credits
    Private Sub Credits_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Label1.Text = "This application was written by" + System.Environment.NewLine +
                "Alessandro Albini (AxeR)" + System.Environment.NewLine +
                "(ID 2032013)" + System.Environment.NewLine +
                "Github: https://github.com/AxeR44/statistics_2223" + System.Environment.NewLine +
                "Website: https://axer44.github.io/statistics_2223/"
        Me.Label1.TextAlign = ContentAlignment.MiddleCenter
        Dim pwd As String
        pwd = Directory.GetCurrentDirectory()
        Dim bmp = New Bitmap(".\\assets\\sapienza.png")
        Me.PictureBox1.SizeMode = PictureBoxSizeMode.StretchImage
        PictureBox1.Image = bmp
    End Sub
End Class