Public Class MetroMsgbox

    Private MyConverter As MyMsgboxConverter

    Public Sub New(ByVal Converter As MyMsgboxConverter)
        InitializeComponent()
        btn1.Name = btn1.Name & GetUUID()
        btn2.Name = btn2.Name & GetUUID()
        btn3.Name = btn3.Name & GetUUID()
        MyConverter = Converter
        labTitle.Text = Converter.Title
        labCaption.Text = Converter.Caption
        btn1.Text = Converter.Button1
        If Converter.IsWarn Then btn1.ColorType = Button.State.RED
        btn2.Text = Converter.Button2
        btn3.Text = Converter.Button3
        btn2.Visibility = If(Converter.Button2 = "", Visibility.Collapsed, Visibility.Visible)
        btn3.Visibility = If(Converter.Button3 = "", Visibility.Collapsed, Visibility.Visible)
    End Sub

    Private Sub Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Loaded
        Try
            log("[MsgBox] 显示弹窗：" & labTitle.Text)
            log("[MsgBox] 弹窗内容：" & Mid(labCaption.Text, 1, 400) & If(Len(labCaption.Text) > 400, "...", ""))
            If panCaption.ActualWidth = panMain.ActualWidth Then panMain.Width = panMain.ActualWidth + 75
            panMain.UpdateLayout()
            '配色初始化
            If btn2.IsVisible And Not btn1.ColorType = Button.State.RED Then btn1.ColorType = Button.State.HIGHLIGHT
            '延时执行的动画
            Me.Name = "frm" & GetUUID()
            AniStart({
                AaBackGround(Me, New MyColor(20, 0, 0, 0), 500, , New AniEaseStart),
                AaBackGround(panMain, New MyColor(255, 0, 0, 0), 300, , New AniEaseStart),
                AaOpacity(shadow, 0.75, 600, 150),
                AaWidth(line, panMain.ActualWidth - 40, 350, 150, New AniEaseEnd),
                AaOpacity(line, 1, 250, 150),
                AaWidth(labTitle, labTitle.ActualWidth, 250, 200, New AniEaseEnd),
                AaOpacity(labTitle, 1, 200, 200),
                AaOpacity(panCaption, 1, 250, 250),
                AaHeight(btn1, btn1.ActualHeight, 150, 300),
                AaHeight(btn2, btn2.ActualHeight, 150, 350),
                AaHeight(btn3, btn3.ActualHeight, 150, 400)
            }, "MsgboxStart" & Me.Name)
            '动画初始化
            panBtn.Height = panBtn.ActualHeight
            btn1.Height = 0
            btn2.Height = 0
            btn3.Height = 0
            labTitle.Width = 0
            labTitle.Opacity = 0
            labTitle.TextWrapping = TextWrapping.NoWrap
            panCaption.Opacity = 0
        Catch ex As Exception
            ExShow(ex, "弹窗显示失败", ErrorLevel.AllUsers)
        End Try
    End Sub
    Private Sub CloseWindow()
        log("[MsgBox] 关闭弹窗，返回值：" & MyConverter.ReturnCode)
        AniStart({
            AaBackGround(Me, New MyColor(-20, 0, 0, 0), 300, , New AniEaseStart),
            AaBackGround(panMain, New MyColor(-255, 0, 0, 0), 300, 100, New AniEaseStart),
            AaOpacity(shadow, -0.75, 150),
            AaWidth(line, -line.ActualWidth, 250, , New AniEaseStart),
            AaOpacity(line, -1, 150, 100),
            AaWidth(labTitle, -labTitle.ActualWidth, 250),
            AaOpacity(labTitle, -1, 200),
            AaOpacity(panCaption, -1, 200),
            AaHeight(btn1, -btn1.ActualHeight, 150, 60),
            AaHeight(btn2, -btn2.ActualHeight, 150, 30),
            AaHeight(btn3, -btn3.ActualHeight, 150),
            AaCode({"Close", Me}, , True)
        }, "MsgboxClose" & Me.Name)
    End Sub

    Private Sub MetroMsgbox_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Input.KeyEventArgs) Handles Me.KeyDown
        If e.Key = Key.Enter Then btn1_Click()
    End Sub
    Private Sub btn1_Click() Handles btn1.Click
        If MyConverter.IsExited Then Exit Sub
        MyConverter.IsExited = True
        MyConverter.ReturnCode = 1
        CloseWindow()
    End Sub
    Private Sub btn2_Click() Handles btn2.Click
        If MyConverter.IsExited Then Exit Sub
        MyConverter.IsExited = True
        MyConverter.ReturnCode = 2
        CloseWindow()
    End Sub
    Private Sub btn3_Click() Handles btn3.Click
        If MyConverter.IsExited Then Exit Sub
        MyConverter.IsExited = True
        MyConverter.ReturnCode = 3
        CloseWindow()
    End Sub

End Class
