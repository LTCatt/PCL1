Public Class formStart

    Public IsPushLoading As Boolean = ReadIni("setup", "HomeEnabled", "True") = "True"
    Public IsVersionLoading As Boolean = True
    Public IsFirstUsing As Boolean = False
    Private Sub Window_Loaded(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.ContentRendered
        Try

            log("[Start] 开始执行用户代码")
            frmStart = Me
            If ReadIni("setup", "UiShowLogo", "True") = "True" Then
                Me.Opacity = 1
                Me.IsHitTestVisible = True
            Else
                Me.Opacity = 0
                Me.IsHitTestVisible = False
            End If

            '旧版本处理
            If ReadIni("setup", "UiHiddenSource", "") = "True" And ReadIni("setup", "HomeEnabled", "Nothing") = "Nothing" Then WriteIni("setup", "HomeEnabled", "False")
            If ReadIni("setup", "LaunchJVM", "").Length > 0 Then WriteIni("setup", "LaunchJVM", ReadIni("setup", "LaunchJVM", "").Replace("-XX:HeapDumpPath=MojangTricksIntelDriversForPerformance_javaw.exe_minecraft.exe.heapdump ", ""))

            '初始化设置
            AllowFeedback = ReadReg("SysFeed", "True")
            RefreshOffline()
            IsFirstUsing = ReadIni("setup", "LastVersionCode", "0") = "0" And ReadIni("setup", "Count", "0") = "0"
            WriteIni("setup", "Count", Val(ReadIni("setup", "Count", "0")) + 1)
            PATH_DOWNLOAD = ReadIni("setup", "DownFolder", "")
            If Not PATH_DOWNLOAD.Contains("\") Then PATH_DOWNLOAD = PATH & "PCL\download\"
            '线程池初始化
            Dim PoolThread As New Thread(AddressOf PoolLoader)
            PoolThread.Start()
            ''静默模式检查
            'If (Not GetProgramArgument("Version") = "") And (Not GetProgramArgument("Method") = "") Then
            '    labProcess.Content = "正在检查信息"
            '    labProcess.Visibility = Visibility.Visible
            '    Me.Opacity = 1
            '    GoTo SlientMode
            'End If
            '线程池添加
            If IsFirstUsing Then
                IsVersionLoading = False
            Else
                If ReadIni("setup", "HomeAutologin", "True") = "True" Then Pool.Add(New Thread(Sub() PoolLogin(True)))
                Pool.Add(New Thread(Sub() PoolJavaFolder(False)))
                Pool.Add(New Thread(Sub() PoolMinecraftFolder()))
            End If
            If IsPushLoading Then Pool.Add(New Thread(AddressOf PoolPush))
            Pool.Add(New Thread(AddressOf PoolUpdate) With {.Priority = ThreadPriority.BelowNormal})
            Pool.Add(New Thread(AddressOf PoolLoadPictures) With {.Priority = ThreadPriority.BelowNormal})
            If ReadIni("setup", "HomeUpdate", "True") = "True" Then Pool.Add(New Thread(Sub() frmDownloadLeft.GetMinecraftBasic(False)) With {.Priority = ThreadPriority.BelowNormal})
            log("[Start] 初始化线程池结束")
            '初始化Timer
            ControlStartRun()
            WebStartRun()
            '主题及窗体加载
            RefreshTheme()
            log("[Start] 主题加载结束")
            'Debug 模式警告
#If CONFIG = "Debug" Then
            ShowHint("当前为 Debug 模式，不要用于正式版发布！")
#End If
            '初始化窗体
            SetupRefreshHidden()
            frmMain.Opacity = 0
            frmMain.Width = MAINFORM_WIDTH
            frmMain.Height = MAINFORM_HEIGHT
            frmMain.imgMainBg.Opacity = ReadIni("setup", "UiBackgroundOpacity", "50") / 100
            frmMain.labTopVer.Content = VERSION_NAME & If(MODE_DEVELOPER, " | 开发者模式", If(MODE_DEBUG, " | 调试模式", "")) & If(MODE_OFFLINE, " | 离线模式", "")
            '切换到首页
            If Not IsNothing(frmHomeLeft.panMain.Parent) Then frmHomeLeft.panMain.Parent.SetValue(ContentPresenter.ContentProperty, Nothing)
            If Not IsNothing(frmHomeRight.panMain.Parent) Then frmHomeRight.panMain.Parent.SetValue(ContentPresenter.ContentProperty, Nothing)
            frmMain.panHostLeft.Children.Add(frmHomeLeft.panMain)
            frmMain.panHostRight.Children.Add(frmHomeRight.panMain)
            frmMain.btnTopMain.Opacity = 1
            '显示
            If MODE_DEVELOPER Then
                log("[Start] PCL 以开发者模式运行")
                MODE_DEBUG = True
            End If
            If MODE_OFFLINE Then log("[Start] PCL 以离线模式运行")
            If MODE_DEBUG Then log("[Start] PCL 以调试模式运行")
            frmMain.Show()
            '检查加载结束
            Dim th As New Thread(Sub()
                                     Do While IsVersionLoading Or IsPushLoading
                                         Thread.Sleep(5)
                                     Loop
                                     LoadTimeCost = My.Computer.Clock.TickCount - LoadTimeCost
                                     log("[Start] 初始化结束，总耗时：" & LoadTimeCost / 1000 & "s")
                                     If LoadTimeCost < 20000 And RandomInteger(1, 10) = 4 Then SendStat("反馈", "加载用时", LoadTimeCost, LoadTimeCost) '发送打开统计
                                     frmMain.Dispatcher.Invoke(Sub()
                                                                   AniStart({
                                                                            AaScaleTransform(frmMain.panAllBack, -0.05, 1),
                                                                            AaScaleTransform(frmMain.panAllBack, 0.05, 250, 80, New AniEaseJumpEnd(0.6)),
                                                                            AaOpacity(frmMain, ReadIni("setup", "UiLauncherOpacity", "100") / 100, 250, 80),
                                                                            AaCode({"Close", frmStart}, 370),
                                                                            AaCode(Sub() Process.GetCurrentProcess.PriorityClass = ProcessPriorityClass.Normal, 400)
                                                                        }, "MainStart")
                                                                   frmHomeLeft.timerPage.Reset()
                                                               End Sub)
                                 End Sub)
            th.Start()

            'Exit Sub
            'SlientMode:
            '            Dim Version As String = GetProgramArgument("Version")
            '            Dim Method As String = GetProgramArgument("Method")
            '            log("[Start] PCL 以静默模式运行，目标版本：" & Version & "，登录方式：" & Method)
            '            'Java 校验
            '            PATH_JAVA = ReadReg("SetupJavaPath")
            '            Dim Env As String = PathEnv
            '            If File.Exists(PATH_JAVA & "\javaw.exe") And (File.Exists(PATH_JAVA & "\jli.dll") Or PATH_JAVA.Contains("javapath")) And Env.Contains(PATH_JAVA) Then
            '                log("[Start] Java路径：" & PATH_JAVA)
            '            Else
            '                labProcess.Content = "Java 路径不正确"
            '                GoTo EndSlient
            '            End If
            '            'Minecraft 校验
            '            PATH_MC = ReadIni("setup", "LaunchFolderSelect", "")
            '            If CheckDirectoryPermission(PATH_MC & "versions\" & Version & "\PCL") Then
            '                log("[Start] .minecraft文件夹：" & PATH_MC)
            '            Else
            '                labProcess.Content = "Minecraft " & Version & " 未找到"
            '                GoTo EndSlient
            '            End If
            '            '结束
            '            labProcess.Content = "已启动"
            '            GoTo EndSlient

            'EndSlient:
            '            Dim th As New Thread(Sub()
            '                                     Thread.Sleep(2000)
            '                                     frmStart.Dispatcher.Invoke(Sub() EndForce())
            '                                 End Sub)
            '            th.Start()

        Catch ex As Exception
            ExShow(ex, "全局初始化失败", ErrorLevel.Barrier)
        End Try
    End Sub

End Class
