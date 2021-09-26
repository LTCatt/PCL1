Public Class formGuild

    '切换页面
    Private Sub ShowPage(Page As Grid)
        frmMain.Dispatcher.Invoke(Sub()

                                      log("[Guild] 显示页面：" & Page.Name)
                                      CType(Page.RenderTransform, ScaleTransform).ScaleX = 0.96
                                      CType(Page.RenderTransform, ScaleTransform).ScaleY = 0.96
                                      Page.Opacity = 0
                                      Page.Visibility = Visibility.Visible
                                      AniStart({
                                                    AaScaleTransform(Page, 1 - CType(Page.RenderTransform, ScaleTransform).ScaleX, 200, 130, New AniEaseJumpEnd(0.8)),
                                                    AaOpacity(Page, 1 - Page.Opacity, 150, 130)
                                               }, "GuildChangePage" & GetUUID())

                                  End Sub)
    End Sub
    Private Sub HidePage(Page As Grid)
        frmMain.Dispatcher.Invoke(Sub()

                                      log("[Guild] 隐藏页面：" & Page.Name)
                                      AniStart({
                                                    AaScaleTransform(Page, 0.96 - CType(Page.RenderTransform, ScaleTransform).ScaleX, 200,, New AniEaseEnd),
                                                    AaOpacity(Page, -Page.Opacity, 150),
                                                    AaCode({"Visible", Page, False}, 200)
                                               }, "GuildChangePage" & GetUUID())

                                  End Sub)
    End Sub

    '检测启动器更新与 Java
    Private Sub formGuild_Loaded(sender As Object, e As RoutedEventArgs) Handles panMain.Loaded
        panJava.Visibility = Visibility.Collapsed
        panPlayer.Visibility = Visibility.Collapsed
        panTheme.Visibility = Visibility.Collapsed
        panMinecraft.Visibility = Visibility.Collapsed
        panVersion.Visibility = Visibility.Collapsed
        ShowPage(panLoad)
        Dim th As New Thread(Sub()

                                 '启动器更新

                                 frmMain.Dispatcher.Invoke(Sub() labLoad.Content = "检查启动器更新中")
                                 Do While NeedUpdate = LoadState.Loading Or NeedUpdate = LoadState.Waiting
                                     Thread.Sleep(10)
                                 Loop
                                 If NeedUpdate = LoadState.Loaded Then
                                     '需要更新
                                     frmMain.Dispatcher.Invoke(Sub() labLoad.Content = "启动器更新中")
                                     WriteIni("setup", "LastVersionCode", "0")
                                     Dim th2 As New Thread(AddressOf DownloadUpdate)
                                     th2.Start()
                                     Do While UpdateState = LoadState.Loading Or UpdateState = LoadState.Waiting
                                         Thread.Sleep(20)
                                     Loop
                                 End If

                                 'Java 检测

                                 frmMain.Dispatcher.Invoke(Sub() labLoad.Content = "检测 Java 中")
                                 PoolJavaFolder(True)
                                 If PATH_JAVA = "" Then
                                     '未找到 Java
                                     HidePage(panLoad)
                                     ShowPage(panJava)
                                     Exit Sub
                                 End If

                                 frmMain.Dispatcher.Invoke(Sub() labLoad.Content = "准备中")
                                 HidePage(panLoad)
                                 ShowPage(panTheme)

                             End Sub)
        th.Start()
    End Sub

    'Java 页面
    Private Sub btnJavaClick(sender As Object, e As RoutedEventArgs) Handles btnJava.Click
        Process.Start("https://www.java.com/winoffline_installer/")
        MyMsgbox("请在下载、安装结束后重启电脑。", "提示")
    End Sub
    Private Sub btnJavaSelectClick(sender As Object, e As RoutedEventArgs) Handles btnJavaSelect.Click
        Using fileDialog As New Forms.OpenFileDialog
            fileDialog.AddExtension = True
            fileDialog.AutoUpgradeEnabled = True
            fileDialog.CheckFileExists = True
            fileDialog.Filter = "JavaW|javaw.exe"
            fileDialog.Multiselect = False
            fileDialog.Title = "选择 javaw.exe"
            fileDialog.ShowDialog()
            '选择文件结束
            If Not fileDialog.FileName = "" Then
                PATH_JAVA = Mid(fileDialog.FileName, 1, fileDialog.FileName.LastIndexOf("\"))
                WriteReg("SetupJavaPath", PATH_JAVA)
                log("[Guild] 人工指定的新的Java路径：" & PATH_JAVA)
                Dim th As New Thread(AddressOf SetJavaEnvironment)
                th.Priority = ThreadPriority.AboveNormal
                th.Start()
                HidePage(panJava)
                ShowPage(panTheme)
            End If
        End Using
    End Sub

    '主题配色
    Private Sub ThemeChange(sender As Object, e As EventArgs) Handles rectTheme0.MouseUp, rectTheme1.MouseUp, rectTheme2.MouseUp, rectTheme3.MouseUp
        WriteIni("setup", "UiTheme", sender.Tag)
        RefreshTheme()
    End Sub
    Private Sub btnTheme_Loaded(sender As Object, e As RoutedEventArgs) Handles btnTheme.Click
        HidePage(panTheme)
        ShowPage(panPlayer)
    End Sub

    '玩家种类
    Private IsNewPlayer As Boolean
    Private Sub btnPlayerClickNew() Handles btnPlayerNew.Click
        IsNewPlayer = True
        WriteIni("setup", "DownForgeAction", "0")
        WriteIni("setup", "HomeUpdateRelease", "True")
        HidePage(panPlayer)
        MinecraftFolder()
    End Sub
    Private Sub btnPlayerClickOld() Handles btnPlayerOld.Click
        IsNewPlayer = False
        HidePage(panPlayer)
        MinecraftFolder()
    End Sub

    'Minecraft 文件夹
    Private Sub MinecraftFolder()
        Try

            '优先尝试设置为当前文件夹
            If CheckDirectoryPermission(PATH & ".minecraft\versions") Then MinecraftThere() : Exit Sub

            '获取官启目录下的版本数量
            Dim MojangPathCount As Integer = 0
            Dim MojangPath As String = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) & "\.minecraft\"
            If CheckDirectoryPermission(MojangPath & "versions\") Then MojangPathCount = Directory.GetDirectories(MojangPath & "versions\").Length

            '如果官启目录下有版本的话就进行询问
            If IsNewPlayer Then MinecraftThere() : Exit Sub
            If MojangPathCount > 0 Then
                HidePage(panPlayer)
                ShowPage(panMinecraft)
            Else
                MinecraftThere()
            End If

        Catch ex As Exception
            MinecraftThere()
        End Try
    End Sub
    Private Sub MinecraftMojang() Handles btnMinecraftMojang.Click
        WriteIni("setup", "LaunchFolderSelect", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) & "\.minecraft\")
        HidePage(panMinecraft)
        PoolMinecraftFolder()
        VersionCheck()
    End Sub
    Private Sub MinecraftThere() Handles btnMinecraftThere.Click
        WriteIni("setup", "LaunchFolderSelect", PATH & ".minecraft\")
        HidePage(panMinecraft)
        PoolMinecraftFolder()
        VersionCheck()
    End Sub

    '版本隔离
    Private Sub VersionCheck()
        '检查根目录与版本文件夹内是否有 mod
        Dim IsRoot As Boolean = Directory.Exists(PATH_MC & "mods")
        Dim IsVersion As Boolean = False
        Try
            If Directory.Exists(PATH_MC & "versions/") Then
                For Each Dir As DirectoryInfo In New DirectoryInfo(PATH_MC & "versions/").GetDirectories
                    If Directory.Exists(Dir.FullName & "/mods/") Then
                        IsVersion = True
                        GoTo FindEnd
                    End If
                Next
            End If
        Catch
        End Try
FindEnd:
        '判断情况
        If IsRoot Then
            If IsVersion Then
                '根目录和版本文件夹都有
                GoTo RequireCheck
            Else
                '根目录有，版本文件夹没有
                GoTo DirectNo
            End If
        Else
            If IsVersion Then
                '根目录没有，版本文件夹有
                GoTo DirectSplit
            Else
                '两边都没有
                If IsNewPlayer Then
                    GoTo DirectNo
                Else
                    GoTo RequireCheck
                End If
            End If
        End If
        Exit Sub
        '各种处理
RequireCheck:
        ShowPage(panVersion)
        Exit Sub
DirectSplit:
        WriteIni("setup", "LaunchSplit", "1")
        GuildExit()
        Exit Sub
DirectNo:
        WriteIni("setup", "LaunchSplit", "0")
        GuildExit()
        Exit Sub
    End Sub
    Private Sub labVersionAll_Click(sender As Object, e As EventArgs) Handles labVersionAll.Click
        WriteIni("setup", "LaunchSplit", "2")
        GuildExit()
    End Sub
    Private Sub labVersionForge_Click(sender As Object, e As EventArgs) Handles labVersionForge.Click
        WriteIni("setup", "LaunchSplit", "1")
        GuildExit()
    End Sub
    Private Sub labVersionNo_Click(sender As Object, e As EventArgs) Handles labVersionNo.Click
        WriteIni("setup", "LaunchSplit", "0")
        GuildExit()
    End Sub

    '退出设置向导
    Private Sub GuildExit()

        ShowHint(New HintConverter("配置完成，欢迎使用 PCL！", HintState.Finish))
        frmMain.panTopSelect.Visibility = Visibility.Visible
        frmMain.panTopSelect.Opacity = 0
        frmMain.panGuild.IsHitTestVisible = False
        frmMain.labTopVer.Visibility = Visibility.Visible
        frmMain.labTopVer.Opacity = 0
        AniStart({
                AaHeight(frmMain.panTop, 52, 200),
                AaOpacity(frmMain.labFirst, -1, 200),
                AaOpacity(frmMain.panTopSelect, 1, 150, 150),
                AaOpacity(frmMain.labTopVer, 0.23, 150, 150),
                AaOpacity(frmMain.panGuild, -1, 300)
            }, "ExitGuild")

    End Sub

End Class
