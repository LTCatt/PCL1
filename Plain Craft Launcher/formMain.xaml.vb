Public Class formMain

#Region "全窗体事件"

    ''' <summary>
    ''' 从Win10任务栏关闭。
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub formMain_Closing(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles Me.Closing
        EndForce()
    End Sub
    ''' <summary>
    ''' 初始化加载。
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub Window_Loaded(ByVal sender As System.Object, ByVal e As EventArgs) Handles MyBase.ContentRendered
        Try

            log("[Main] 初始化开始")
            '数据统计
            'If Not MODE_OFFLINE Then
            '    webData.Navigate(New Uri(QN_SERVER & "stats.html"))
            '    SetWebBrowserSilent(webData, True)
            'End If
            '开始动画
            UseControlAnimation = True
            AniStartRun()
            AniLastTick = 0
            '检查最后的版本号
            Dim th As New Thread(Sub()
                                     frmMain.Dispatcher.Invoke(Sub()
                                                                   Select Case Val(ReadIni("setup", "LastVersionCode", "0"))
                                                                       Case VERSION_CODE
                                                                           '已经启动过当前版本
                                                                       Case 0
                                                                           '从未启动过
                                                                           WriteIni("setup", "LastVersionCode", VERSION_CODE)
                                                                       Case Is < VERSION_CODE
                                                                           '已经启动过老版本
                                                                           WriteIni("setup", "LastVersionCode", VERSION_CODE)
                                                                           OutputFileInResource("UpdateLog", PATH & "PCL\UpdateLog.txt", True)
                                                                           If File.Exists(PATH & "PCL\UpdateLog.txt") Then Shell("notepad", """" & PATH & "PCL\UpdateLog.txt""")
                                                                   End Select
                                                               End Sub)
                                 End Sub)
            th.Start()
            '开始遍历
            timerRuntimeCheck.IsEnabled = True
            '播放 BGM
            If File.Exists(PATH & "PCL\bgm.wav") Or File.Exists(PATH & "PCL\bgm.mp3") Then
                If ReadIni("setup", "UiBgmAuto", "True") Then ChangeBgm()
                imgTopMusic.Visibility = Windows.Visibility.Visible
            Else
                imgTopMusic.Visibility = Windows.Visibility.Collapsed
            End If
            '首次启动
            If frmStart.IsFirstUsing Then
                log("[Main] 首次启动 PCL，进入设置向导")
                panTop.Height = 40
                labFirst.Visibility = Visibility.Visible
                panTopSelect.Visibility = Visibility.Collapsed
                labTopVer.Visibility = Visibility.Collapsed
                panGuild.Visibility = Visibility.Visible
                '控件置入
                Dim frmGuild As New formGuild
                If Not IsNothing(frmGuild.panMain.Parent) Then frmGuild.panMain.Parent.SetValue(ContentPresenter.ContentProperty, Nothing)
                panGuild.Children.Add(frmGuild.panMain)
                Exit Sub
            Else
                panTop.Height = 92
                labFirst.Visibility = Visibility.Collapsed
                panTopSelect.Visibility = Visibility.Visible
                labTopVer.Visibility = Visibility.Visible
                panGuild.Visibility = Visibility.Collapsed
            End If
            '打开 4 次或 99 次后请求反馈
            If ReadReg("RequiredFeedback", "False") = "False" And Not MODE_OFFLINE Then
                If Val(ReadIni("setup", "Count", "0")) >= 4 Then
                    If MyMsgbox("你对 PCL 有没有什么建议？在刚开始使用时，你有没有什么不清楚的地方？" & vbCrLf & "有了你的反馈，PCL 才能够做得更好！", "用户反馈", "反馈", "取消") = 1 Then Process.Start("https://www.wjx.cn/jq/14677608.aspx")
                    WriteReg("RequiredFeedback", "True")
                    GoTo EndFeedback
                End If
            End If
            If ReadReg("RequiredFeedback2", "False") = "False" And Not MODE_OFFLINE Then
                If Val(ReadIni("setup", "Count", "0")) >= 99 Then
                    Select Case MyMsgbox("你对 PCL 有没有什么建议？你觉得 PCL 怎样才能做得更好？我们期待你的反馈！" & vbCrLf & "你随时都可以通过设置进入反馈页面。", "用户反馈", "捐助", "反馈", "取消")
                        Case 1
                            '捐助
                            Process.Start("https://afdian.net/@LTCat")
                        Case 2
                            '反馈
                            Process.Start("https://www.wjx.cn/jq/14677608.aspx")
                    End Select
                    WriteReg("RequiredFeedback2", "True")
                End If
            End If
EndFeedback:
            '打开 25 的倍数次的时候发送反馈信息
            If Val(ReadIni("setup", "Count", "0")) Mod 25 = 5 And Not MODE_OFFLINE Then
                Dim FeedbackThread As New Thread(Sub() FeedbackSetup())
                FeedbackThread.Priority = ThreadPriority.Lowest
                FeedbackThread.Start()
            End If
            '打开 10 的倍数次的时候发送次数反馈信息
            If Val(ReadIni("setup", "Count", "0")) Mod 10 = 1 And Not MODE_OFFLINE Then
                SendStat("反馈", "启动器打开次数", ReadIni("setup", "Count", "0"), ReadIni("setup", "Count", "0"))
            End If
        Catch ex As Exception
            ExShow(ex, "主窗体初始化失败", ErrorLevel.MsgboxAndFeedback)
        End Try
    End Sub
    ''' <summary>
    ''' 窗体拖动。
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub FormDragDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles labTopVer.MouseLeftButtonDown
        On Error Resume Next
        Me.DragMove()
    End Sub
    Private Sub FormDragDown(ByVal sender As Grid, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles panTop.MouseLeftButtonDown
        On Error Resume Next
        If sender.IsMouseDirectlyOver Then Me.DragMove()
    End Sub
    ''' <summary>
    ''' 在设置按钮隐藏时，双击版本号进入设置页。
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub labTopVer_MouseDoubleClick(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles labTopVer.MouseDoubleClick
        If btnTopSetup.Visibility = Visibility.Collapsed Then PageChange("设置")
    End Sub
    ''' <summary>
    ''' 按回车直接启动游戏。
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub formMain_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Input.KeyEventArgs) Handles Me.KeyDown
        If Math.Abs(ReadIni("setup", "UiLauncherOpacity", "100") / 100 - Me.Opacity) > 0.01 Then Exit Sub
        If PageSelect = "首页" And e.Key = Key.Enter Then frmHomeRight.ClickStartButton()
    End Sub

#End Region

#Region "动画"

    '指向动画
    Private Sub AniMouseEnter(ByVal sender As Object, ByVal e As System.Windows.Input.MouseEventArgs) Handles imgTopMin.MouseEnter, imgTopClose.MouseEnter, imgTopMusic.MouseEnter
        AniStart({AaOpacity(sender, 0.75 - sender.Opacity, 80)}, "MouseEnter" & sender.Name)
    End Sub
    Private Sub AniMouseLeave(ByVal sender As Object, ByVal e As System.Windows.Input.MouseEventArgs) Handles imgTopMin.MouseLeave, imgTopClose.MouseLeave, imgTopMusic.MouseLeave
        AniStart({AaOpacity(sender, 0.35 - sender.Opacity, 80)}, "MouseEnter" & sender.Name)
        btnTopMin_Down = False
        btnTopClose_Down = False
    End Sub
    Private Sub AniMouseEnter2(ByVal sender As Object, ByVal e As System.Windows.Input.MouseEventArgs) Handles btnTopMain.MouseEnter, btnTopManage.MouseEnter, btnTopDown.MouseEnter, btnTopHelp.MouseEnter, btnTopSetup.MouseEnter
        If PageSelect = sender.Tag Or panTopSelect.Tag = "Changing" Then Exit Sub '已经选中的项目、切换中不响应事件
        AniStart({AaOpacity(sender, 0.75 - sender.Opacity, 80)}, "MouseEnter" & sender.Name)
    End Sub
    Private Sub AniMouseLeave2(ByVal sender As Object, ByVal e As System.Windows.Input.MouseEventArgs) Handles btnTopMain.MouseLeave, btnTopManage.MouseLeave, btnTopDown.MouseLeave, btnTopHelp.MouseLeave, btnTopSetup.MouseLeave
        If PageSelect = sender.Tag Or panTopSelect.Tag = "Changing" Then Exit Sub '已经选中的项目、切换中不响应事件
        AniStart({AaOpacity(sender, 0.35 - sender.Opacity, 80)}, "MouseEnter" & sender.Name)
    End Sub

#End Region

#Region "最小化、关闭、音乐"

    Private btnTopMin_Down As Boolean = False
    Private Sub btnTopMin_MouseLeftButtonDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles imgTopMin.MouseLeftButtonDown
        btnTopMin_Down = True
    End Sub
    Private Sub imgTopMin_MouseLeftButtonUp(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles imgTopMin.MouseLeftButtonUp
        If Not btnTopMin_Down Then Exit Sub
        btnTopMin_Down = False
        log("[Main] 点击最小化按钮")
        Me.WindowState = WindowState.Minimized
    End Sub

    Private btnTopClose_Down As Boolean = False
    Private Sub imgTopClose_MouseLeftButtonDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles imgTopClose.MouseLeftButtonDown
        btnTopClose_Down = True
    End Sub
    Private Sub btnTopClose_MouseLeftButtonUp(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles imgTopClose.MouseLeftButtonUp
        If Not btnTopClose_Down Then Exit Sub
        btnTopClose_Down = False
        log("[Main] 点击关闭按钮")
        EndNormal()
    End Sub

    Private btnTopMusic_Down As Boolean = False
    Private Sub imgTopMusic_MouseLeftButtonDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles imgTopMusic.MouseLeftButtonDown
        btnTopMusic_Down = True
    End Sub
    Private Sub btnTopMusic_MouseLeftButtonUp() Handles imgTopMusic.MouseLeftButtonUp
        If Not btnTopMusic_Down Then Exit Sub
        btnTopMusic_Down = False
        ChangeBgm()
    End Sub
    ''' <summary>
    ''' 切换 BGM 播放状态。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub ChangeBgm()
        Try
            Dim FilePath As String = If(File.Exists(PATH & "PCL\bgm.wav"), PATH & "PCL\bgm.wav", PATH & "PCL\bgm.mp3")
            If IsPlayingMusic Then
                PlayBgm(FilePath, True)
            Else
                PlayBgm(FilePath, False)
            End If
            IsPlayingMusic = Not IsPlayingMusic
        Catch ex As Exception
            ExShow(ex, "切换音乐播放出错", ErrorLevel.MsgboxAndFeedback)
        End Try
    End Sub
#End Region

#Region "顶部选择条"

    ''' <summary>
    ''' 当前选择的页面名称，如“首页”。
    ''' </summary>
    ''' <remarks></remarks>
    Public PageSelect As String = "首页"
    ''' <summary>
    ''' 当前选择的页面编号，首页为0。
    ''' </summary>
    ''' <remarks></remarks>
    Public PageSelectId As Integer = 0

    Public ENTER_PAGE_TIME As Integer = 200
    Public LEAVE_PAGE_TIME As Integer = 100
    Public Const CHANGE_PAGE_DISTANCE As Integer = 65
    ''' <summary>
    ''' 切换页面
    ''' </summary>
    ''' <param name="PageNew">页面的名称，如“首页”</param>
    ''' <remarks></remarks>
    Public Sub PageChange(ByVal PageNew As String)
        Try

            '已经选中的项目不响应事件
            If PageNew = PageSelect Then Exit Sub

            '初始化
            frmHomeLeft.timerPage.Reset()

            '改变页面
            AniStop("HomeLeftChange")
            Select Case PageNew
                Case "管理"
                    SendPage("manage")
                    '计算动画朝向
                    Dim Towards As Integer = If(PageSelectId < 1, -1, 1)
                    '切换页面的动画，用对象保存再单独丢出去会莫名找不到对象
                    If Not IsNothing(frmManageLeft.panMain.Parent) Then frmManageLeft.panMain.Parent.SetValue(ContentPresenter.ContentProperty, Nothing)
                    If Not IsNothing(frmManageRight.panMain.Parent) Then frmManageRight.panMain.Parent.SetValue(ContentPresenter.ContentProperty, Nothing)
                    frmHomeRight.HideVersion()
                    AniStart({
                             AaOpacity(panMain, -panMain.Opacity, LEAVE_PAGE_TIME),
                             AaX(panMain, CHANGE_PAGE_DISTANCE * Towards - panMain.Margin.Left, LEAVE_PAGE_TIME, , New AniEaseStart),
                             AaCode({"Clear", panHostLeft.Children}, , True),
                             AaCode({"Clear", panHostRight.Children}),
                             AaCode({"Add", panHostLeft.Children, frmManageLeft.panMain}),
                             AaCode({"Add", panHostRight.Children, frmManageRight.panMain}),
                             AaX(panMain, -CHANGE_PAGE_DISTANCE * 2 * Towards, 1),
                             AaX(panMain, CHANGE_PAGE_DISTANCE * Towards, ENTER_PAGE_TIME, , New AniEaseJumpEnd(0.6)),
                             AaOpacity(panMain, 1, ENTER_PAGE_TIME)
                         }, "HomeLeftChange")
                    '顶部条的动画
                    AniStart({
                            AaOpacity(btnTopMain, 0.35 - btnTopMain.Opacity, 80),
                            AaOpacity(btnTopManage, 1 - btnTopManage.Opacity, 80),
                            AaOpacity(btnTopDown, 0.35 - btnTopDown.Opacity, 80),
                            AaOpacity(btnTopHelp, 0.35 - btnTopHelp.Opacity, 80),
                            AaOpacity(btnTopSetup, 0.35 - btnTopSetup.Opacity, 80)
                        }, "HomeLeftChange2")
                    PageSelectId = 1
                Case "下载"
                    SendPage("download")
                    If IsNothing(frmDownloadRight) Then frmDownloadRight = New formDownloadRight
                    Dim Towards As Integer = If(PageSelectId < 2, -1, 1)
                    If Not IsNothing(frmDownloadLeft.panMain.Parent) Then frmDownloadLeft.panMain.Parent.SetValue(ContentPresenter.ContentProperty, Nothing)
                    If Not IsNothing(frmDownloadRight.panMain.Parent) Then frmDownloadRight.panMain.Parent.SetValue(ContentPresenter.ContentProperty, Nothing)
                    frmHomeRight.HideVersion()
                    AniStart({
                             AaOpacity(panMain, -panMain.Opacity, LEAVE_PAGE_TIME),
                             AaX(panMain, CHANGE_PAGE_DISTANCE * Towards - panMain.Margin.Left, LEAVE_PAGE_TIME, , New AniEaseStart),
                             AaCode({"Clear", panHostLeft.Children}, , True),
                             AaCode({"Clear", panHostRight.Children}),
                             AaCode({"Add", panHostLeft.Children, frmDownloadLeft.panMain}),
                             AaCode({"Add", panHostRight.Children, frmDownloadRight.panMain}),
                             AaX(panMain, -CHANGE_PAGE_DISTANCE * 2 * Towards, 1),
                             AaX(panMain, CHANGE_PAGE_DISTANCE * Towards, ENTER_PAGE_TIME, , New AniEaseJumpEnd(0.6)),
                             AaOpacity(panMain, 1, ENTER_PAGE_TIME)
                         }, "HomeLeftChange")
                    AniStart({
                            AaOpacity(btnTopMain, 0.35 - btnTopMain.Opacity, 80),
                            AaOpacity(btnTopManage, 0.35 - btnTopManage.Opacity, 80),
                            AaOpacity(btnTopDown, 1 - btnTopDown.Opacity, 80),
                            AaOpacity(btnTopHelp, 0.35 - btnTopHelp.Opacity, 80),
                            AaOpacity(btnTopSetup, 0.35 - btnTopSetup.Opacity, 80)
                        }, "HomeLeftChange2")
                    PageSelectId = 2
                Case "帮助"
                    SendPage("help")
                    Dim Towards As Integer = If(PageSelectId < 3, -1, 1)
                    If Not IsNothing(frmHelpLeft.panMain.Parent) Then frmHelpLeft.panMain.Parent.SetValue(ContentPresenter.ContentProperty, Nothing)
                    If Not IsNothing(frmHelpRight.panMain.Parent) Then frmHelpRight.panMain.Parent.SetValue(ContentPresenter.ContentProperty, Nothing)
                    frmHomeRight.HideVersion()
                    AniStart({
                             AaOpacity(panMain, -panMain.Opacity, LEAVE_PAGE_TIME),
                             AaX(panMain, CHANGE_PAGE_DISTANCE * Towards - panMain.Margin.Left, LEAVE_PAGE_TIME, , New AniEaseStart),
                             AaCode({"Clear", panHostLeft.Children}, , True),
                             AaCode({"Clear", panHostRight.Children}),
                             AaCode({"Add", panHostLeft.Children, frmHelpLeft.panMain}),
                             AaCode({"Add", panHostRight.Children, frmHelpRight.panMain}),
                             AaX(panMain, -CHANGE_PAGE_DISTANCE * 2 * Towards, 1),
                             AaX(panMain, CHANGE_PAGE_DISTANCE * Towards, ENTER_PAGE_TIME, , New AniEaseJumpEnd(0.6)),
                             AaOpacity(panMain, 1, ENTER_PAGE_TIME)
                         }, "HomeLeftChange")
                    AniStart({
                            AaOpacity(btnTopMain, 0.35 - btnTopMain.Opacity, 80),
                            AaOpacity(btnTopManage, 0.35 - btnTopManage.Opacity, 80),
                            AaOpacity(btnTopDown, 0.35 - btnTopDown.Opacity, 80),
                            AaOpacity(btnTopHelp, 1 - btnTopHelp.Opacity, 80),
                            AaOpacity(btnTopSetup, 0.35 - btnTopSetup.Opacity, 80)
                        }, "HomeLeftChange2")
                    PageSelectId = 3
                Case "设置"
                    SendPage("setup")
                    If IsNothing(frmSetup) Then frmSetup = New formSetup
                    Dim Towards As Integer = If(PageSelectId < 4, -1, 1)
                    If Not IsNothing(frmSetup.panMain.Parent) Then frmSetup.panMain.Parent.SetValue(ContentPresenter.ContentProperty, Nothing)
                    frmHomeRight.HideVersion()
                    AniStart({
                             AaOpacity(panMain, -panMain.Opacity, LEAVE_PAGE_TIME),
                             AaX(panMain, CHANGE_PAGE_DISTANCE * Towards - panMain.Margin.Left, LEAVE_PAGE_TIME, , New AniEaseStart),
                             AaCode({"Clear", panHostLeft.Children}, , True),
                             AaCode({"Clear", panHostRight.Children}),
                             AaCode({"Add", panHostLeft.Children, frmSetup.panMain}),
                             AaX(panMain, -CHANGE_PAGE_DISTANCE * 2 * Towards, 1),
                             AaX(panMain, CHANGE_PAGE_DISTANCE * Towards, ENTER_PAGE_TIME, , New AniEaseJumpEnd(0.6)),
                             AaOpacity(panMain, 1, ENTER_PAGE_TIME)
                         }, "HomeLeftChange")
                    AniStart({
                            AaOpacity(btnTopMain, 0.35 - btnTopMain.Opacity, 80),
                            AaOpacity(btnTopManage, 0.35 - btnTopManage.Opacity, 80),
                            AaOpacity(btnTopDown, 0.35 - btnTopDown.Opacity, 80),
                            AaOpacity(btnTopHelp, 0.35 - btnTopHelp.Opacity, 80),
                            AaOpacity(btnTopSetup, 1 - btnTopSetup.Opacity, 80)
                        }, "HomeLeftChange2")
                    PageSelectId = 4
                Case Else '首页作为默认选项
                    SendPage("stats")
                    Dim Towards As Integer = If(PageSelectId < 0, -1, 1)
                    If Not IsNothing(frmHomeLeft.panMain.Parent) Then frmHomeLeft.panMain.Parent.SetValue(ContentPresenter.ContentProperty, Nothing)
                    If Not IsNothing(frmHomeRight.panMain.Parent) Then frmHomeRight.panMain.Parent.SetValue(ContentPresenter.ContentProperty, Nothing)
                    AniStart({
                             AaOpacity(panMain, -panMain.Opacity, LEAVE_PAGE_TIME),
                             AaX(panMain, CHANGE_PAGE_DISTANCE * Towards - panMain.Margin.Left, LEAVE_PAGE_TIME, , New AniEaseStart),
                             AaCode({"Clear", panHostLeft.Children}, , True),
                             AaCode({"Clear", panHostRight.Children}),
                             AaCode({"Add", panHostLeft.Children, frmHomeLeft.panMain}),
                             AaCode({"Add", panHostRight.Children, frmHomeRight.panMain}),
                             AaX(panMain, -CHANGE_PAGE_DISTANCE * 2 * Towards, 1),
                             AaX(panMain, CHANGE_PAGE_DISTANCE * Towards, ENTER_PAGE_TIME, , New AniEaseJumpEnd(0.6)),
                             AaOpacity(panMain, 1, ENTER_PAGE_TIME)
                         }, "HomeLeftChange")
                    AniStart({
                            AaOpacity(btnTopMain, 1 - btnTopMain.Opacity, 80),
                            AaOpacity(btnTopManage, 0.35 - btnTopManage.Opacity, 80),
                            AaOpacity(btnTopDown, 0.35 - btnTopDown.Opacity, 80),
                            AaOpacity(btnTopHelp, 0.35 - btnTopHelp.Opacity, 80),
                            AaOpacity(btnTopSetup, 0.35 - btnTopSetup.Opacity, 80)
                        }, "HomeLeftChange2")
                    PageSelectId = 0
            End Select

            '回设
            PageSelect = PageNew

        Catch ex As Exception
            ExShow(ex, "切换页面失败", ErrorLevel.MsgboxAndFeedback)
        End Try
    End Sub

    ''' <summary>
    ''' 点击Grid触发切换页面
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub PageChangeClick(ByVal sender As Grid, ByVal e As EventArgs) Handles btnTopMain.MouseLeftButtonUp, btnTopManage.MouseLeftButtonUp, btnTopDown.MouseLeftButtonUp, btnTopHelp.MouseLeftButtonUp, btnTopSetup.MouseLeftButtonUp
        PageChange(sender.Tag)
    End Sub

#End Region

#Region "文件拖放"

    '进入的动画
    Private Sub AniDragEnter() Handles panBack.DragEnter
        AniStart({
                 AaOpacity(panDrag, 1 - panDrag.Opacity, 150)
             }, "frmMainDrag", False)
    End Sub
    '离开的动画
    Private Sub AniDragLeave() Handles panBack.DragLeave, panBack.Drop
        AniStart({
                 AaOpacity(panDrag, -panDrag.Opacity, 150)
             }, "frmMainDrag", False)
    End Sub

    '接受文件拖放
    Private Sub panBack_DragEnter(ByVal sender As Object, ByVal e As System.Windows.DragEventArgs) Handles panBack.DragEnter
        '接受拖放
        e.Effects = DragDropEffects.Link
    End Sub

    '文件放入
    Private Sub panBack_Drop(ByVal sender As Object, ByVal e As System.Windows.DragEventArgs) Handles panBack.Drop
        '获取文件列表
        Dim FilePathList As New ArrayList
        Try
            FilePathList.AddRange(CType(e.Data.GetData(DataFormats.FileDrop), Array))
            If FilePathList.Count = 0 Then Exit Sub
        Catch
            Exit Sub
        End Try
        '处理文件
        For Each File As String In FilePathList
            AutoSetup(File)
        Next
    End Sub

#End Region

#Region "任务栏"

    Private Sub CommandBinding_Executed(ByVal sender As System.Object, ByVal e As System.Windows.Input.ExecutedRoutedEventArgs)
        frmHomeRight.ClickStartButton()
        e.Handled = True
    End Sub
    Private Sub CommandBinding_CanExecute(ByVal sender As System.Object, ByVal e As System.Windows.Input.CanExecuteRoutedEventArgs)
        e.CanExecute = True
        e.Handled = True

    End Sub

#End Region

    Private IsRamWarned As Boolean = False
    Private IsSendStat As Boolean = False
    Private Sub timerRuntimeCheck_Tick() Handles timerRuntimeCheck.Tick
        Try

            '显示提示
            If WaitingHint.Count > 1 Then WaitingHint = ArrayNoDouble(WaitingHint.ToArray)
            Do While WaitingHint.Count > 0
                WaitingHint(0).Text = WaitingHint(0).Text.Replace(vbCrLf, " ").Replace(vbCr, " ").Replace(vbLf, " ") '去回车
                log("[Main] 显示弹出提示：" & CType(WaitingHint(0), HintConverter).toString)
                '准备控件
                Dim NewGrid As New Grid With {.Name = "panHintChildren" & GetUUID(), .Opacity = 0, .Background = New MyColor(CType(WaitingHint(0), HintConverter)), .Height = 0, .HorizontalAlignment = HorizontalAlignment.Left, .Margin = New Thickness(-15, 0, 0, 0)}
                NewGrid.Children.Add(New Label With {.Content = WaitingHint(0).Text, .FontSize = 13, .Foreground = New MyColor(255, 255, 255), .Height = 25, .HorizontalAlignment = HorizontalAlignment.Left, .Padding = New Thickness(23, 0, 7, 0), .VerticalAlignment = VerticalAlignment.Center, .VerticalContentAlignment = VerticalAlignment.Center})
                NewGrid.Children.Add(New Image With {.Height = 16, .HorizontalAlignment = HorizontalAlignment.Left, .Margin = New Thickness(4, 5, 0, 4), .VerticalAlignment = VerticalAlignment.Center, .Width = 16, .Source = CType((New ImageSourceConverter).ConvertFromString(PATH_IMAGE & "Hint-" & WaitingHint(0).GetTypeName & ".png"), ImageSource)})
                frmMain.panHint.Children.Add(NewGrid)
                '控件动画
                'NewGrid 高度增加25px，向右移动15px，透明度增加1
                '等待大约3秒后，NewGrid 平滑收回并且删除
                AniStart({
                         AaX(NewGrid, 15, , , New AniEaseEnd),
                         AaOpacity(NewGrid, 1, 200),
                         AaHeight(NewGrid, 25, , , New AniEaseEnd),
                         AaOpacity(NewGrid, -1, 200, 850 + MathRange(Len(WaitingHint(0).Text), 5, 23) * 180),
                         AaHeight(NewGrid, -25, 300, 850 + MathRange(Len(WaitingHint(0).Text), 5, 23) * 180, New AniEaseEnd),
                         AaCode({"Remove", frmMain.panHint.Children, NewGrid}, , True)
                     }, "ShowHint" & GetUUID())
                WaitingHint.RemoveAt(0)
            Loop

            '显示弹窗
            Do While WaitingHintWindow.Count > 0
                WaitingHintWindow(0)(1) = WaitingHintWindow(0)(1).Replace(vbCrLf, " ").Replace(vbCr, " ").Replace(vbLf, " ") '去回车
                log("[Main] 显示弹出提示窗口：" & WaitingHintWindow(0)(0) & "，内容：" & WaitingHintWindow(0)(1))
                '准备窗口
                Dim Window As New formHint
                Window.labTitle.Content = WaitingHintWindow(0)(0)
                Window.labText.Text = WaitingHintWindow(0)(1)
                Window.Show()
                '控件动画
                'AniStart({
                '         AaX(NewGrid, 15, , , AniAdditionType.FadeOut),
                '         AaOpacity(NewGrid, 1, 200),
                '         AaHeight(NewGrid, 25, , , AniAdditionType.FadeOut),
                '         AaOpacity(NewGrid, -1, 200, 850 + MathRange(Len(WaitingHint(0).Text), 5, 20) * 160, 120),
                '         AaHeight(NewGrid, -25, 300, 850 + MathRange(Len(WaitingHint(0).Text), 5, 20) * 160, AniAdditionType.FadeOut),
                '         AaCode({"Remove", frmMain.panHint.Children, NewGrid}, , True)
                '     }, "ShowHint" & GetUUID())
                WaitingHintWindow.RemoveAt(0)
            Loop

            '彩蛋检查
            If panHint.ActualHeight > MAINFORM_HEIGHT - 20 And Not IsShowingDeathBlue Then
                If ReadReg("ThemeDeathBlue", "False") = "False" Then
                    IsShowingDeathBlue = True
                    Dim BlueScreen As New formBlueScreen
                    BlueScreen.Show()
                End If
            End If

            '显示弹窗
            Do While WaitingMyMsgbox.Count > 0
                MyMsgbox(WaitingMyMsgbox(0))
                WaitingMyMsgbox.RemoveAt(0)
            Loop

            '显示内存警告
            Dim RamLeft As Integer = (New Devices.ComputerInfo).AvailablePhysicalMemory / 1024 / 1024
            If RamLeft < 500 And Not IsRamWarned Then
                '放在里面是为了防止多次读取导致卡顿
                If RamLeft < ReadReg("SystemWarn", "100") Then
                    ShowHintWindow("PCL - 内存警告", "电脑内存目前仅剩 " & RamLeft & " M！")
                    IsRamWarned = True
                End If
            End If
            If IsRamWarned Then
                If RamLeft > Val(ReadReg("SystemWarn", "100")) + 400 Then
                    ShowHintWindow("PCL - 内存提示", "电脑内存已回升至 " & RamLeft & " M。")
                    IsRamWarned = False
                End If
            End If

            '发送统计
            If IsSendStat Then
                Try
                    Do While SendInvokes.Count > 0
                        webData.InvokeScript("eval", New Object() {SendInvokes(0)})
                        SendInvokes.RemoveAt(0)
                    Loop
                Catch ex As Exception
                    log("[Main] 发送统计数据失败：" & GetStringFromException(ex))
                End Try
            End If

            '刷新 UI 显示的文件组的进度条
            For i = 0 To WebGroups.Count - 1
                frmDownloadRight.RefreshGroup(WebGroups.Values(i))
            Next

        Catch ex As Exception
            ExShow(ex, "主时钟执行异常")
        End Try
    End Sub

    Private Sub webData_LoadCompleted() Handles webData.LoadCompleted
        log("[Main] 统计页面加载完成")
        IsSendStat = True
    End Sub

End Class