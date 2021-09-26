Public Class formBlueScreen

    Private Sub formBlueScreen_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        Me.Width = My.Computer.Screen.Bounds.Width
        Me.Height = My.Computer.Screen.Bounds.Height
    End Sub

    Dim AnimationProcess As Integer = 0
    Dim AnimationWaiting As Integer = 0
    Private Sub timerAnimation_Tick() Handles timerAnimation.Tick
        AnimationProcess = AnimationProcess + 1
        Select Case AnimationProcess
            Case 0, 1, 2, 3, 4, 13, 14, 16, 17, 18
                Me.Visibility = Visibility.Visible
            Case 5, 6, 7, 8, 9, 10, 11, 12, 15
                Me.Visibility = Visibility.Collapsed
            Case 19, 20, 21, 22
                Me.Visibility = Visibility.Visible
                panMain.Visibility = Visibility.Visible
            Case Else
                If Val(labProcess.Content) < 100 Then
                    If RandomInteger(0, 4) = 4 Then
                        labProcess.Content = Int(MathRange(Val(labProcess.Content) + RandomInteger(2, 30), 0, 100))
                    End If
                Else
                    AnimationWaiting = AnimationWaiting + 1
                    If AnimationWaiting = 40 Then
                        Me.Close()
                        IsShowingDeathBlue = False
                        If ReadReg("ThemeDeathBlue", "False") = "False" Then
                            WriteReg("ThemeDeathBlue", "True")
                            MyMsgbox("隐藏主题 死机蓝 已解锁，请到设置页面查看！", "主题已解锁", "我知道了")
                            SendStat("彩蛋", "主题", "死机蓝")
                        Else
                            ShowHint("你是不是闲得无聊……")
                        End If
                    End If
                End If
        End Select
    End Sub

End Class
