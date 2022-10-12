Imports System.IO

Public Class Credits
    Private Sub Credits_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.label1.Text = "This application was written by" + System.Environment.NewLine +
                "Alessandro Albini (AxeR)" + System.Environment.NewLine +
                "(ID 2032013)" + System.Environment.NewLine +
                "Github: https://github.com/AxeR44/statistics_2223" + System.Environment.NewLine +
                "Website: https://axer44.github.io/statistics_2223/"
        Me.label1.TextAlign = ContentAlignment.MiddleCenter
        Dim pwd As String
        pwd = Directory.GetCurrentDirectory()
        Dim bmp = New Bitmap(".\\assets\\sapienza.png")
        Me.pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage
        pictureBox1.Image = bmp
    End Sub
End Class