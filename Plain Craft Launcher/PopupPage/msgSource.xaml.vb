Public Class msgSource

    Dim Source As ListItem = Nothing

    Public Sub New(ByVal Source As ListItem)
        InitializeComponent()
        Me.Source = Source
        Me.textName.Text = Source.MainText
        Me.textURL.Text = Source.SubText
    End Sub

    Private Sub textName_TextChanged(ByVal sender As Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles textName.TextChanged
        If Not IsNothing(Source) Then Source.MainText = textName.Text
    End Sub

    Private Sub textURL_TextChanged(ByVal sender As Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles textURL.TextChanged
        If Not IsNothing(Source) Then Source.SubText = textURL.Text
    End Sub
End Class
