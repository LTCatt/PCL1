Public Class formHomeLeft

    Public LoadingItem As New ArrayList '正在加载的首页项目
    Public LoadedItem As New ArrayList '加载结束但没有编入随机位的首页项目
    Public RandomedItem As New ArrayList '已随机处理的首页项目

    Private ShowingCount As Integer = 0 '正在显示的首页项目计数，用于排布位置
    Private ShowingPage As Integer = 0 '正在显示的页码

    Private LoadedRefreshCount As Integer = 0 '刷新计数

    Public HideNotfull As Boolean = True

    Private Sub timerLoad_Tick() Handles timerLoad.Tick
        If MODE_OFFLINE Then Exit Sub

        '检查是否存在加载结束的项目（成功或是失败）
        If LoadingItem.Count > 0 Then
            For i = 0 To LoadingItem.Count - 1
                Dim item As InfoBoxSource = LoadingItem(i) '当前的InfoBox
                If IsNothing(item) Then Exit Sub
                frmStart.IsPushLoading = False
                Select Case item.State
                    Case LoadState.Loaded
                        '移动到加载结束的数组
                        LoadingItem.Remove(item)
                        LoadedItem.Add(item)
                        i = i - 1
                    Case LoadState.Failed
                        '输出信息并移除
                        log("[HomeLeft] 信息框加载失败：" & item.Source & " - " & item.Title, True)
                        LoadingItem.Remove(item)
                        i = i - 1
                End Select
                If i >= LoadingItem.Count - 1 Then Exit For
            Next i
        End If

        '检查是否存在可显示的项目
        Dim Count As Integer = 0
        Do While LoadedItem.Count > 0 And Count < 4
            Count = Count + 1
            '处理源
            Dim itemSource As InfoBoxSource = LoadedItem(RandomInteger(0, LoadedItem.Count - 1))
            LoadedItem.Remove(itemSource)
            RandomedItem.Add(itemSource)
            '处理新的控件
            Dim item As New InfoBox With {.Margin = New Thickness(0, 0, 15, 0), .UseLayoutRounding = False}
            item.Show(itemSource)
            '添加到 UI
            If ReadIni("setup", "HomeEnabled", "True") = "True" Then
                Select Case ShowingCount
                    Case 0, 1
                        If panTop.Children.Count < 2 Then SendStat("推荐源", "显示", item.Source & "（" & item.Title & "）") : item.IsSendStat = True
                        panTop.Children.Add(item)
                        ShowingCount = ShowingCount + 1
                    Case 2
                        If panBottom.Children.Count < 2 Then SendStat("推荐源", "显示", item.Source & "（" & item.Title & "）") : item.IsSendStat = True
                        panBottom.Children.Add(item)
                        ShowingCount = ShowingCount + 1
                    Case 3
                        If panBottom.Children.Count < 2 Then SendStat("推荐源", "显示", item.Source & "（" & item.Title & "）") : item.IsSendStat = True
                        panBottom.Children.Add(item)
                        ShowingCount = 0
                End Select
            End If
        Loop

        '检查是否需要刷新计数
        If Not RandomedItem.Count = LoadedRefreshCount Then
            LoadedRefreshCount = RandomedItem.Count
            '刷新页面计数器
            Dim NewPageAnimation As New ArrayList
            Do While panPage.Children.Count < If(HideNotfull, Math.Floor(LoadedRefreshCount / 4), Math.Floor((LoadedRefreshCount - 1) / 4) + 1)
                '添加新页面按钮
                Dim NewPage As Integer = panPage.Children.Count + 1 '新添加的页面编号
                Dim NewBorder As New Border With {.Name = "btnPage" & NewPage, .VerticalAlignment = VerticalAlignment.Center, .Height = 0, .Width = 0, .BorderThickness = New Thickness(2), .Margin = New Thickness(20 * (NewPage - 1) + 6, 0, 0, 0), .IsHitTestVisible = False, .HorizontalAlignment = HorizontalAlignment.Left, .RenderTransform = New RotateTransform(0), .RenderTransformOrigin = New Point(0.5, 0.5), .Background = New MyColor(128, 255, 255, 255), .UseLayoutRounding = True}
                NewBorder.SetResourceReference(BorderBrushProperty, "Color4")
                panPage.Children.Add(NewBorder)
                '按钮动画（每一页都按次序执行动画）
                NewPageAnimation.AddRange({
                         AaWidth(panPage, 20, 300, , , True),
                         AaWidth(NewBorder, 8, 250, 50),
                         AaHeight(NewBorder, 8, 250, 50)
                     })
            Loop
            If NewPageAnimation.Count > 0 Then AniStart(NewPageAnimation, "HomeLeftNewPage" & GetUUID()) '点点的动画
            ChangeWidth()
        End If

    End Sub

    Public Sub ChangeWidth() Handles panMain.Loaded
        '如果是离线模式或者没有推荐则直接隐藏，并且还顺便改变登录页大小
        Dim TotalCount As Integer = LoadingItem.Count + LoadedItem.Count + RandomedItem.Count
        If MODE_OFFLINE Or TotalCount = 0 Or ReadIni("setup", "HomeEnabled", "True") = "False" Then
            panMain.Visibility = Visibility.Collapsed
        Else
            panMain.Visibility = Visibility.Visible
        End If
    End Sub

    Private Sub StatShow() Handles panMain.Loaded
        Dim IsSendStat As Boolean = RandomInteger(1, 100) = 66 '1%的情况下反馈显示推荐源信息
        Dim Info As InfoBox
        If ShowingPage * 2 < panTop.Children.Count Then
            Info = CType(panTop.Children(ShowingPage * 2), InfoBox)
            If Info.IsSendStat = False Then
                Info.IsSendStat = True
                If IsSendStat Then SendStat("推荐源", "显示", Info.Source & "（" & Info.Title & "）")
            End If
        End If
        If ShowingPage * 2 + 1 < panTop.Children.Count Then
            Info = CType(panTop.Children(ShowingPage * 2 + 1), InfoBox)
            If Info.IsSendStat = False Then
                Info.IsSendStat = True
                If IsSendStat Then SendStat("推荐源", "显示", Info.Source & "（" & Info.Title & "）")
            End If
        End If
        If ShowingPage * 2 < panBottom.Children.Count Then
            Info = CType(panBottom.Children(ShowingPage * 2), InfoBox)
            If Info.IsSendStat = False Then
                Info.IsSendStat = True
                If IsSendStat Then SendStat("推荐源", "显示", Info.Source & "（" & Info.Title & "）")
            End If
        End If
        If ShowingPage * 2 + 1 < panBottom.Children.Count Then
            Info = CType(panBottom.Children(ShowingPage * 2 + 1), InfoBox)
            If Info.IsSendStat = False Then
                Info.IsSendStat = True
                If IsSendStat Then SendStat("推荐源", "显示", Info.Source & "（" & Info.Title & "）")
            End If
        End If
    End Sub

    ''' <summary>
    ''' 切换页面的点击事件。
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub panPage_MouseLeftButtonDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles panPage.MouseLeftButtonDown

        '初始化
        Dim ClickedPage As Integer = Math.Floor(e.GetPosition(panPage).X / 20)
        If ClickedPage = ShowingPage Then Exit Sub

        ChangePage(ClickedPage)

    End Sub

    Private Sub ChangePage(ByVal ClickedPage As Integer)
        If Not frmMain.PageSelect = "首页" Then Exit Sub

        '小点的动画
        Dim OldDot As Border = panPage.Children(ShowingPage)
        Dim NewDot As Border = panPage.Children(ClickedPage)
        AniStart({
            AaWidth(OldDot, 8 - OldDot.Width, 100),
            AaHeight(OldDot, 8 - OldDot.Height, 100),
            AaX(OldDot, 20 * ShowingPage + 6 - OldDot.Margin.Left, 100),
            AaBorderThickness(OldDot, 2 - OldDot.BorderThickness.Bottom, 60),
            AaRotateTransform(OldDot, -CType(OldDot.RenderTransform, RotateTransform).Angle, 100,, New AniEaseEnd),
            AaWidth(NewDot, 12 - NewDot.Width, 100),
            AaHeight(NewDot, 12 - NewDot.Height, 100),
            AaX(NewDot, 20 * ClickedPage + 4 - NewDot.Margin.Left, 100),
            AaBorderThickness(NewDot, 6 - NewDot.BorderThickness.Bottom, 60),
            AaRotateTransform(NewDot, 45 - CType(NewDot.RenderTransform, RotateTransform).Angle, 100,, New AniEaseEnd),
            AaX(panTop, -ClickedPage * 2 * (230 + 15) - panTop.Margin.Left, 250,, New AniEaseEnd()),
            AaX(panBottom, -ClickedPage * 2 * (230 + 15) - panBottom.Margin.Left, 250,, New AniEaseEnd())
        }, "HomeLeftPushChange")

        '结束
        ShowingPage = ClickedPage
        timerPage.Reset()
        StatShow()
    End Sub

    Public Sub ChangeSetup() Handles panMain.Loaded
        timerPage.IsEnabled = ReadIni("setup", "HomeAutoplay", "True") = "True"
        HideNotfull = ReadIni("setup", "HomeFull", "True")
        If timerPage.IsEnabled Then
            Select Case ReadIni("setup", "raHomeAutoplaySpeed", "1")
                Case "0"
                    timerPage.Interval = 5000
                Case "1"
                    timerPage.Interval = 10000
                Case "2"
                    timerPage.Interval = 20000
            End Select
        End If
    End Sub

    Private Sub timerPage_Tick() Handles timerPage.Tick
        If panBottom.Children.Count >= 4 Then
            Dim NewPageId As Integer = ShowingPage + 1
            If NewPageId >= panPage.Children.Count Then NewPageId = 0
            ChangePage(NewPageId)
        End If
    End Sub

End Class
