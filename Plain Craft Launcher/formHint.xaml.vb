Public Class formHint

    Private Sub formHint_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        Me.Height = panStack.ActualHeight
        Me.Width = panStack.ActualWidth
        Me.Top = My.Computer.Screen.WorkingArea.Height - Me.Height - 50
        Me.Left = My.Computer.Screen.WorkingArea.Width - Me.Width
        panBack.Width = Me.Width
        SetLeft(panBack, Me.Width)
        Me.Name = "frmHint" & GetUUID()
        AniStart({
                 AaOpacity(Me, 1, 100),
                 AaX(panBack, -Me.Width, 300, Ease:=New AniEaseEnd),
                 AaX(panBack, Me.Width, 300, 3000, New AniEaseStart),
                 AaCode({"Close", Me}, , True)
             }, Me.Name)
    End Sub
    Private Sub formHint_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles Me.MouseUp
        Me.IsEnabled = False
        AniStart({
                AaOpacity(Me, -1, 150),
                AaCode({"Close", Me}, , True)
             }, Me.Name & "Close")
    End Sub

End Class
