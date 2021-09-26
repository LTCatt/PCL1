Public Class formSetup

    ''' <summary>
    ''' 是否可以写入设置。
    ''' </summary>
    ''' <remarks></remarks>
    Private CanWrite As Boolean = False

#Region "窗体初始化"

    ''' <summary>
    ''' 是否已经初始化过了。
    ''' </summary>
    ''' <remarks></remarks>
    Private IsFirstLoaded As Boolean = False
    '初始化
    Private Sub FormSystemSetup_ContentRendered(ByVal sender As Object, ByVal e As System.EventArgs) Handles panMain.Loaded
        Try

            RefreshHidden()
            If IsFirstLoaded Then Exit Sub

            '初始化
            Log("[Setup] 读取设置开始")
            UseControlAnimation = False
            CanWrite = False
            panMain.UpdateLayout()
            panBack.Margin = New Thickness(0)
            selecter.ShowList = {"启动设置", "界面设置", "下载设置", "主页设置", "系统设置", "关于"}
            selecter.SelectIndex = 0
            selecterHide.HintList = {"仅显示少量最常用的设置。", "显示所有普通设置。", "显示所有普通设置与高级设置。高级设置均以 * 标注，且只建议服主、整合作者和熟悉游戏的玩家打开。"}
            selecterHide.ShowList = {"简洁模式", "普通模式", "高级模式"}
            selecterHide.SelectIndex = ReadReg("SysLevel", "0")

            '读取设置：系统设置
            chSysTest.Checked = ReadReg("SysTest", "False")
            chSysFeed.Checked = ReadReg("SysFeed", "True")
            chSysOffline.Checked = ReadIni("setup", "SysOffline", "False")
            chSysUpdate.Checked = ReadIni("setup", "SysUpdate", "True")
            raSysUpdateHint.Checked = ReadIni("setup", "SysUpdateHint", "True")
            raSysUpdateSetup.Checked = Not raSysUpdateHint.Checked
            CType(FindName("raSystemWarn" & ReadReg("SystemWarn", "100")), Radiobox).Checked = True
            chSysUpdateTest.Checked = ReadIni("setup", "SysUpdateTest", "False")

            '读取设置：界面设置
            chLaunchMending.Checked = ReadIni("setup", "LaunchMending", "True")
            chUiShowLogo.Checked = ReadIni("setup", "UiShowLogo", "True")
            textUiLauncherOpacity.Text = ReadIni("setup", "UiLauncherOpacity", "100")
            CType(FindName("raUiTheme" & ReadIni("setup", "UiTheme", "0")), Radiobox).Checked = True
            chUiBackground.Checked = File.Exists(PATH & "PCL\back.png")
            btnUiBarReset.Visibility = If(File.Exists(PATH & "PCL\top.png"), Visibility.Visible, Visibility.Collapsed)
            Dim IsBgm As Boolean = File.Exists(PATH & "PCL\bgm.mp3") Or File.Exists(PATH & "PCL\bgm.wav")
            btnUiBgmReset.Visibility = If(IsBgm, Visibility.Visible, Visibility.Collapsed)
            chUiBgm.Checked = IsBgm
            chUiBgmAuto.Checked = ReadIni("setup", "UiBgmAuto", "True")
            CType(FindName("raUiBackgroundShow" & ReadIni("setup", "UiBackgroundShow", "0")), Radiobox).Checked = True
            textUiBackgroundOpacity.Text = ReadIni("setup", "UiBackgroundOpacity", "50")
            textUiBackgroundURL.Text = ReadIni("setup", "UiBackgroundURL", "")
            textUiBarURL.Text = ReadIni("setup", "UiBarURL", "")
            chUiHiddenDownload.Checked = ReadIni("setup", "UiHiddenDownload", "False")
            chUiHiddenForge.Checked = ReadIni("setup", "UiHiddenForge", "False")
            chUiHiddenMinecraft.Checked = ReadIni("setup", "UiHiddenMinecraft", "False")
            chUiHiddenOptiFine.Checked = ReadIni("setup", "UiHiddenOptiFine", "False")
            chUiHiddenSetup.Checked = ReadIni("setup", "UiHiddenSetup", "False")

            '读取设置：启动设置
            chLaunchTopmost.Checked = ReadReg("LaunchTopmost", "False")
            chLaunchNatives.Checked = ReadIni("setup", "LaunchNatives", "True")
            CType(FindName("raLaunchVisibility" & ReadIni("setup", "LaunchVisibility", "3")), Radiobox).Checked = True
            CType(FindName("raLaunchLevel" & ReadReg("LaunchLevel", "1")), Radiobox).Checked = True
            chLaunchLog.Checked = ReadIni("setup", "LaunchLog", "False")
            chLaunchLogWarn.Checked = ReadIni("setup", "LaunchLogWarn", "False")
            CType(FindName("raLaunchSkin" & ReadReg("LaunchSkin", "3")), Radiobox).Checked = True
            textLaunchSkinName.Text = ReadReg("LaunchSkinName", "")
            textLaunchServer.Text = ReadIni("setup", "LaunchServer", "")
            CType(FindName("raLaunchSplit" & ReadIni("setup", "LaunchSplit", "0")), Radiobox).Checked = True
            CType(FindName("raLaunchMode" & ReadIni("setup", "LaunchMode", "0")), Radiobox).Checked = True
            textLaunchModeWindowHeight.Text = ReadIni("setup", "LaunchModeWindowHeight", "480")
            textLaunchModeWindowWidth.Text = ReadIni("setup", "LaunchModeWindowWidth", "854")
            textLaunchJava.Text = ReadReg("SetupJavaPath", "")
            textLaunchMaxRam.Text = ReadReg("LaunchMaxRam", "1024")
            textLaunchShow.Text = ReadIni("setup", "LaunchShow", APPLICATION_FULL_NAME)
            textLaunchTitle.Text = ReadIni("setup", "LaunchTitle", "")
            textLaunchTitle.ToolTip = "Minecraft 游戏窗口的标题。留空为不更改。" & vbCrLf & "你可以使用以下通配符：" & vbCrLf & "　\\v　对应版本的版本号（如 1.9.4）" & vbCrLf & "　\\r　从 PCL 文件夹中的 RandomTitle.txt 中随机读取一行"
            textLaunchJVM.Text = ReadIni("setup", "LaunchJVM", "-XX:+UseG1GC -XX:-UseAdaptiveSizePolicy -XX:-OmitStackTraceInFastThrow -Dfml.ignoreInvalidMinecraftCertificates=True -Dfml.ignorePatchDiscrepancies=True")
            LoadMinecraftFolderList()

            '读取设置：下载设置
            chDownProcess.Checked = ReadIni("setup", "DownProcess", "True")
            CType(FindName("raDownVersion" & ReadIni("setup", "DownVersion", "0")), Radiobox).Checked = True
            CType(FindName("raDownAssets" & ReadIni("setup", "DownAssets", "0")), Radiobox).Checked = True
            CType(FindName("raDownMinecraft" & ReadIni("setup", "DownMinecraft", "1")), Radiobox).Checked = True
            CType(FindName("raDownOptiFine" & ReadIni("setup", "DownOptiFine", "1")), Radiobox).Checked = True
            CType(FindName("raDownForge" & ReadIni("setup", "DownForge", "1")), Radiobox).Checked = True
            CType(FindName("raDownForgeAction" & ReadIni("setup", "DownForgeAction", "1")), Radiobox).Checked = True
            chDownMinecraftAssetsHint.Checked = ReadIni("setup", "DownMinecraftAssetsHint", "True")
            chDownMinecraftAssetsAuto.Checked = ReadIni("setup", "DownMinecraftAssetsAuto", "True")
            chDownOptiFineOpen.Checked = ReadIni("setup", "DownOptiFineOpen", "False")
            chDownOptiFineFolder.Checked = ReadIni("setup", "DownOptiFineFolder", "True")
            textDownFolder.Text = ReadIni("setup", "DownFolder", "")
            textDownMaxinum.Text = ReadIni("setup", "DownMaxinum", "20")

            '读取设置：主页设置
            chHomeUpdate.Checked = ReadIni("setup", "HomeUpdate", "True")
            chHomeUpdateRelease.Checked = ReadIni("setup", "HomeUpdateRelease", "False")
            chHomeSave.Checked = ReadReg("HomeSave", "True")
            chHomeEnabled.Checked = ReadIni("setup", "HomeEnabled", "True")
            chHomeAutologin.Checked = ReadIni("setup", "HomeAutologin", "True")
            chHomeAutoplay.Checked = ReadIni("setup", "HomeAutoplay", "True")
            chHomeHide.Checked = ReadIni("setup", "HomeHide", "True")
            chHomeFull.Checked = ReadIni("setup", "HomeFull", "True")
            CType(FindName("raHomeAutoplaySpeed" & ReadIni("setup", "raHomeAutoplaySpeed", "1")), Radiobox).Checked = True
            chHomePCLPush.Checked = ReadIni("setup", "HomePCLPush", "True")
            chHomeMCBBSPush.Checked = ReadIni("setup", "HomeMCBBSPush", "True")
            chHomeTbPush.Checked = ReadIni("setup", "HomeTbPush", "True")
            chHomeForumPush.Checked = ReadIni("setup", "HomeForumPush", "False")
            chHomeMojangPush.Checked = ReadIni("setup", "HomeMojangPush", "True")
            For i As Integer = 0 To 100
                Dim SourceString As String = ReadIni("setup", "HomeSource" & i)
                If SourceString = "" Then Exit For
                '分解字符串
                Dim URL As String = SourceString.Split("|")(0)
                Dim IsUsing As String = SourceString.Split("|")(1)
                Dim Source As String = SourceString.Split("|")(2)
                Dim Desc As String = SourceString.Split("|")(3)
                Dim item = New ListItem With {.CanCheck = False, .Width = 500, .ButtonLogo = New BitmapImage(New Uri("/Images/appbar.settings.png", UriKind.Relative)), .UseLayoutRounding = True, .Name = "itemLaunchMain" & Rnd.NextDouble.ToString.Replace(".", ""), .MainText = Source, .SubText = If(Desc = "", URL, Desc), .Logo = New BitmapImage(New Uri("/Images/Block-" & IsUsing & ".png", UriKind.Relative))}
                item.Version.LoadedByFile = IsUsing
                'AddHandler item.ButtonClick, AddressOf SourceClick
                'AddHandler item.IconClick, AddressOf IconClick
                panHomeSourceList.Children.Insert(panHomeSourceList.Children.Count - 1, item)
            Next i
            chHomeVersionSwap.Checked = ReadIni("setup", "HomeVersionSwap", "True")
            chHomeVersionOld.Checked = ReadIni("setup", "HomeVersionOld", "True")

            Log("[Setup] 读取设置成功")
        Catch ex As Exception
            ExShow(ex, "读取设置失败", ErrorLevel.MsgboxAndFeedback)
        Finally
            '初始化结束
            CanWrite = True
            IsFirstLoaded = True
            UseControlAnimation = True
        End Try
    End Sub
    '绑定滚动条
    Private Sub panSetupHost_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles panSetupHost.Loaded
        If scroll.SetControl(panBack, False) Then AddHandler panMain.MouseWheel, AddressOf scroll.RunMouseWheel
    End Sub
    '刷新隐藏主题
    Private Sub RefreshHidden()
        If ReadReg("ThemeHunluan", "False") = "True" Then
            raUiTheme100.IsEnabled = True
            raUiTheme100.Context = "混乱黄"
        End If
        If ReadReg("ThemeDeathBlue", "False") = "True" Then
            raUiTheme101.IsEnabled = True
            raUiTheme101.Context = "死机蓝"
        End If
    End Sub

#End Region

#Region "通用设置更改"

    Private Sub CheckBox_Change(ByVal sender As Checkbox, ByVal raiseByMouse As Boolean) Handles chSysTest.Change, chSysOffline.Change, chSysUpdate.Change, chSysUpdateTest.Change, chUiShowLogo.Change, chUiHiddenDownload.Change, chUiHiddenForge.Change, chUiHiddenMinecraft.Change, chUiHiddenOptiFine.Change, chUiHiddenSetup.Change, chHomeEnabled.Change, chLaunchNatives.Change, chDownMinecraftAssetsAuto.Change, chDownMinecraftAssetsHint.Change, chDownOptiFineFolder.Change, chDownOptiFineOpen.Change, chDownProcess.Change, chHomeAutologin.Change, chHomeAutoplay.Change, chHomePCLPush.Change, chHomeSave.Change, chHomeUpdate.Change, chHomeUpdateRelease.Change, chHomeVersionOld.Change, chHomeVersionSwap.Change, chLaunchLog.Change, chLaunchMending.Change, chLaunchLogWarn.Change, chLaunchTopmost.Change, chHomeMCBBSPush.Change, chHomeMojangPush.Change, chHomeTbPush.Change, chHomeForumPush.Change, chHomeHide.Change, chHomeFull.Change, chUiBgmAuto.Change, chSysFeed.Change
        If Not CanWrite Then Exit Sub

        '读取数据
        Dim Target As String = sender.Tag
        Dim IsReg As Boolean = False
        If Target.StartsWith("*") Then
            IsReg = True
            Target = Mid(Target, 2)
        End If

        '写入设置
        Try
            If IsReg Then
                WriteReg(Target, sender.Checked, True)
            Else
                WriteIni("setup", Target, sender.Checked)
            End If
        Catch ex As Exception
            ExShow(ex, "写入设置失败：" & Target, ErrorLevel.AllUsers)
        End Try

        '引发各自事件
        Select Case sender.Name
            Case "chSysOffline"
                If CanWrite Then RefreshOffline()
            Case "chUiHiddenDownload", "chUiHiddenMinecraft", "chUiHiddenOptiFine", "chUiHiddenForge"
                If CanWrite Then SetupRefreshHidden()
            Case "chUiHiddenSetup"
                If CanWrite Then
                    SetupRefreshHidden()
                    If ReadIni("setup", "UiHiddenSetup", "False") = "True" Then ShowHint("双击版本号可以进入被隐藏的设置页")
                End If
            Case "chLaunchMending"
                File.Delete(PATH_MC & "PCL.ini") '强制重载版本列表
                Pool.Add(New Thread(AddressOf PoolVersionList))
            Case "chHomeVersionSwap", "chHomeVersionOld"
                Pool.Add(New Thread(AddressOf PoolVersionList))
            Case "chDownProcess"
                WebTaskbarShow = sender.Checked
            Case "chHomePCLPush"
                If CanWrite Then
                    If Not sender.Checked Then
                        If MyMsgbox("PCL 推荐包含关于启动器的重要内容，并且不会含有任何商业广告，仅仅作为启动器通知和传播优秀作品的平台（如果你有任何优秀作品，都可以免费在 PCL 推荐上推广），确认要关闭吗？", "关闭确认", "算了不关闭了", "我还是要关闭") = 1 Then
                            sender.Checked = True
                            Exit Sub
                        End If
                    End If
                    ShowHint(New HintConverter("该设置将在重启后生效", HintState.Finish))
                End If
            Case "chHomeMCBBSPush", "chHomeTbPush", "chHomeMojangPush", "chHomeAdPush", "chHomeForumPush", "chHomeHide", "chSysUpdateTest", "chHomeEnabled"
                If CanWrite Then ShowHint(New HintConverter("该设置将在重启后生效", HintState.Finish))
            Case "chHomeAutoplay"
                If CanWrite Then frmHomeLeft.ChangeSetup()
            Case "chHomeFull"
                If CanWrite Then
                    frmHomeLeft.ChangeSetup()
                    ShowHint(New HintConverter("该设置将在重启后生效", HintState.Finish))
                End If
            Case "chSysFeed"
                AllowFeedback = chSysFeed.Checked
        End Select

    End Sub
    Private Sub RadioBox_Change(ByVal sender As Radiobox, ByVal raiseByMouse As Boolean) Handles raSysUpdateHint.Change, raSysUpdateSetup.Change, raUiTheme0.Change, raUiTheme1.Change, raUiTheme100.Change, raUiTheme101.Change, raUiTheme2.Change, raUiTheme3.Change, raUiTheme4.Change, raUiBackgroundShow0.Change, raUiBackgroundShow1.Change, raUiBackgroundShow2.Change, raUiBackgroundShow3.Change, raUiBackgroundShow4.Change, raUiBackgroundShow5.Change, raLaunchMode0.Change, raLaunchMode1.Change, raLaunchVisibility0.Change, raLaunchVisibility1.Change, raLaunchVisibility4.Change, raLaunchVisibility2.Change, raLaunchVisibility3.Change, raDownAssets0.Change, raDownAssets1.Change, raDownMinecraft0.Change, raDownMinecraft1.Change, raDownOptiFine0.Change, raDownOptiFine1.Change, raDownVersion0.Change, raDownVersion1.Change, raHomeAutoplaySpeed0.Change, raHomeAutoplaySpeed1.Change, raHomeAutoplaySpeed2.Change, raDownForge0.Change, raDownForge1.Change, raLaunchLevel0.Change, raLaunchLevel1.Change, raLaunchLevel2.Change, raSystemWarn0.Change, raSystemWarn100.Change, raSystemWarn300.Change, raLaunchSplit0.Change, raLaunchSplit1.Change, raLaunchSplit2.Change, raLaunchSkin0.Change, raLaunchSkin1.Change, raLaunchSkin2.Change, raLaunchSkin3.Change, raDownForgeAction0.Change, raDownForgeAction1.Change
        If Not CanWrite Then Exit Sub
        If Not sender.Checked Then Exit Sub

        '引发各自前置事件
        Select Case sender.Name
            Case "raLaunchVisibility0"
                If MyMsgbox("如果设置为打开游戏后关闭启动器，更改游戏窗口标题、游戏崩溃诊断、日志分析、低内存警告等功能额可能会失效。如果你担心 PCL 后台运行影响游戏性能，可以设置为隐藏启动器 2 分钟后自动关闭。" & vbCrLf & "确定要继续设置为直接关闭吗？", "设置确认", "确定", "取消", , True) = 2 Then
                    raLaunchVisibility3.Checked = True
                    raLaunchVisibility0.Checked = False
                    Exit Sub
                End If
        End Select

        '读取数据
        Dim Data As String = sender.Tag
        Dim IsReg As Boolean = False
        If Data.StartsWith("*") Then
            IsReg = True
            Data = Mid(Data, 2)
        End If
        Dim Target As String = Data.Split("/")(0)
        Dim NewValue As String = Data.Replace(Target & "/", "")

        '写入设置
        Try
            If IsReg Then
                WriteReg(Target, NewValue, True)
            Else
                WriteIni("setup", Target, NewValue)
            End If
        Catch ex As Exception
            ExShow(ex, "写入设置失败：" & Target, ErrorLevel.AllUsers)
        End Try

        '引发各自事件
        Select Case Target
            Case "UiTheme"
                raUiThemes_Change(sender)
            Case "UiBackgroundShow"
                SetupRefreshBackground()
            Case "LaunchVisibility"
                If NewValue = 0 Then chLaunchLog.Checked = False
            Case "LaunchFolderSelect"
                Dim th As New Thread(Sub()
                                         PoolMinecraftFolder()
                                         frmMain.Dispatcher.Invoke(Sub() LoadMinecraftFolderList())
                                     End Sub)
                th.Start()
            Case "LaunchSkin"
                Dim UserName As String = frmHomeRight.textLegacyUsername.Text
                Dim th As New Thread(Sub() frmHomeRight.RefreshLegacySkin(UserName))
                th.Start()
            Case "HomeAutoplaySpeed"
                If CanWrite Then frmHomeLeft.ChangeSetup()
        End Select

    End Sub
    Private Sub TextBox_Change(ByVal sender As TextBox, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles textUiBackgroundURL.TextChanged, textUiBackgroundOpacity.TextChanged, textUiBarURL.TextChanged, textUiLauncherOpacity.TextChanged, textLaunchJava.TextChanged, textLaunchJVM.TextChanged, textLaunchMaxRam.TextChanged, textLaunchMaxRam.TextChanged, textLaunchModeWindowHeight.TextChanged, textLaunchModeWindowWidth.TextChanged, textLaunchShow.TextChanged, textDownFolder.TextChanged, textDownMaxinum.TextChanged, textLaunchTitle.TextChanged, textLaunchSkinName.TextChanged, textLaunchServer.TextChanged
        If Not CanWrite Then Exit Sub

        '读取数据
        Dim Data As String = sender.Tag
        Dim IsReg As Boolean = False
        Dim IsNotReturn As Boolean = False
        If Data.StartsWith("*") Then
            IsReg = True
            Data = Mid(Data, 2)
        End If
        If Data.StartsWith("=") Then
            IsNotReturn = True
            Data = Mid(Data, 2)
        End If
        Dim Target As String = Data.Split("/")(0)
        Dim CheckMethod As String = Data.Split("/")(1)
        Dim CheckResult As String = If(IsNotReturn, "", sender.Name.Replace("text", "lab"))
        Dim CheckData() As String = {}
        If Len(Data) > Len(Target) + Len(CheckMethod) + 1 Then
            CheckData = (Mid(Data, Len(Target) + Len(CheckMethod) + 3)).Split("/")
        End If
        '至此提取出的有用信息：
        '   IsReg：是否写入注册表，为 False 时写入 setup.ini
        '   Target：设置项的 Key
        '   CheckMethod：输入检查的方式
        '   CheckResult：输入检查返回的目标 Label 名称
        '   CheckData()：输入检查的额外数据

        '输入检查
        Try
            Select Case CheckMethod
                Case "Integer"
                    '类型为整数
                    Dim UserInput As Integer = GetDoubleFromString(sender.Text)
                    If Not UserInput.ToString = sender.Text Then
                        SetCheckResult("必须为一个整数", CheckResult, sender)
                        Exit Sub
                    ElseIf UserInput < Int(Val(CheckData(0))) Then
                        SetCheckResult("最小为 " & Int(Val(CheckData(0))), CheckResult, sender)
                        Exit Sub
                    ElseIf UserInput > Int(Val(CheckData(1))) Then
                        SetCheckResult("最大为 " & Int(Val(CheckData(1))), CheckResult, sender)
                        Exit Sub
                    End If
                Case "URL"
                    '类型为网址
                    If sender.Text = "" Then
                        '为空不检查
                    ElseIf sender.Text.StartsWith("://") Or Not sender.Text.Contains("://") Then
                        SetCheckResult("必须为一个网址", CheckResult, sender)
                        Exit Sub
                    End If
                Case "UUID"
                    '类型为 UUID
                    Dim UserInput = sender.Text.Replace("-", "").Replace(" ", "").ToUpper
                    If UserInput = "" Then GoTo NoError
                    Dim reg = RegexSearch(UserInput, "[0-9A-F]{32}")
                    If Not Len(UserInput) = 32 Then
                        SetCheckResult("长度应为 32 位", CheckResult, sender)
                        Exit Sub
                    ElseIf Not reg.Count = 1 Then
                        SetCheckResult("格式不正确", CheckResult, sender)
                        Exit Sub
                    End If
NoError:
                Case "Ram"
                    '类型为 Ram（MB）
                    Dim UserInput As Integer = GetDoubleFromString(sender.Text)
                    If Not UserInput.ToString = sender.Text Then
                        SetCheckResult("必须为一个整数", CheckResult, sender)
                        Exit Sub
                    ElseIf UserInput < Int(Val(CheckData(0))) Then
                        SetCheckResult("最小为 256 M", CheckResult, sender)
                        Exit Sub
                    ElseIf UserInput > 1024 And Not Environment.Is64BitOperatingSystem Then
                        SetCheckResult("32 位系统最大为 1024 M", CheckResult, sender)
                        Exit Sub
                    ElseIf UserInput > Int((New Devices.ComputerInfo).TotalPhysicalMemory / 1024 / 1024) - 512 Then
                        SetCheckResult("最大为 " & Int((New Devices.ComputerInfo).TotalPhysicalMemory / 1024 / 1024) - 512 & " M", CheckResult, sender)
                        Exit Sub
                    End If
                Case "Java"
                    sender.Text = sender.Text.Replace("javaw.exe", "")
                    '类型为 Java 路径
                    If Not Directory.Exists(sender.Text) Then
                        SetCheckResult("文件夹不存在", CheckResult, sender)
                        Exit Sub
                    End If
                    If Not CheckDirectoryPermission(sender.Text) Then
                        SetCheckResult("文件夹无法访问", CheckResult, sender)
                        Exit Sub
                    ElseIf Not (File.Exists(sender.Text & "\javaw.exe") And (File.Exists(sender.Text & "\jli.dll") Or sender.Text.Contains("javapath"))) Then
                        SetCheckResult("未找到 Java", CheckResult, sender)
                        Exit Sub
                    End If
                    '去“\”
                    Dim NewText As String = (New RegularExpressions.Regex("[\\]{2,10000}")).Replace(sender.Text, "\")
                    If NewText.EndsWith("\") Then NewText = Mid(NewText, 1, Len(NewText) - 1)
                    If Not NewText = sender.Text Then
                        sender.Text = NewText
                        sender.SelectionStart = sender.Text.Length
                    End If
                Case "Folder"
                    '类型为文件夹
                    If sender.Text = "" Then
                        '为空不检查
                    Else
                        '检查
                        If sender.Text = "\" Or Not Directory.Exists(sender.Text) Then '莫名其妙的问题：Directory.Exists("\") = True
                            SetCheckResult("文件夹不存在", CheckResult, sender)
                            Exit Sub
                        End If
                        If Not CheckDirectoryPermission(sender.Text) Then
                            SetCheckResult("文件夹无法访问", CheckResult, sender)
                            Exit Sub
                        End If
                        '去“\”
                        Dim NewText As String = (New RegularExpressions.Regex("[\\]{2,10000}")).Replace(sender.Text, "\")
                        If Not NewText = sender.Text Then
                            sender.Text = NewText
                            sender.SelectionStart = sender.Text.Length
                        End If
                    End If
                Case "UserName"
                    '类型为用户名
                    Dim SafeCheck = RegexSearch(sender.Text, "[^0-9A-Za-z_]+")
                    If SafeCheck.Count > 0 Then
                        SetCheckResult("用户名不合法", CheckResult, sender)
                        Exit Sub
                    End If
                Case "NoPush"
                    '禁用英文单引号
                    If sender.Text.Contains("""") Then
                        SetCheckResult("不能包含英文单引号", CheckResult, sender)
                        Exit Sub
                    End If
                Case "Any"
                    '不管了
            End Select
            SetCheckResult("", CheckResult, sender)
        Catch ex As Exception
            ExShow(ex, "进行输入检查失败：" & sender.Text)
        End Try

        '写入设置
        Try
            If IsReg Then
                WriteReg(Target, sender.Text, True)
            Else
                WriteIni("setup", Target, sender.Text)
            End If
        Catch ex As Exception
            ExShow(ex, "写入设置失败：" & Target, ErrorLevel.AllUsers)
        End Try

        '引发各自事件
        Select Case sender.Name
            Case "textUiLauncherOpacity"
                If CanWrite Then frmMain.Opacity = ReadIni("setup", "UiLauncherOpacity", "100") / 100
            Case "textUiBackgroundOpacity"
                If CanWrite Then frmMain.imgMainBg.Opacity = ReadIni("setup", "UiBackgroundOpacity", "100") / 100
            Case "textLaunchShow"
                If CanWrite Then
                    If textLaunchShow.Text = "Searge" Then
                        textLaunchShow.Text = "#itzlipofutzli"
                        textLaunchShow.SelectionStart = textLaunchShow.Text.Length
                    End If
                End If
            Case "textLaunchSkinName"
                Dim UserName As String = frmHomeRight.textLegacyUsername.Text
                Dim th As New Thread(Sub() frmHomeRight.RefreshLegacySkin(UserName))
                th.Start()
            Case "textLaunchJava"
                Dim th As New Thread(Sub() PoolJavaFolder(False))
                th.Start()
            Case "textDownMaxinum"
                WebDownloadCountMax = sender.Text
            Case "textDownFolder"
                If Not (ReadIni("setup", "DownFolder", "\").EndsWith("\") Or ReadIni("setup", "DownFolder", "\") = "") Then
                    WriteIni("setup", "DownFolder", ReadIni("setup", "DownFolder", "") & "\")
                End If
                PATH_DOWNLOAD = ReadIni("setup", "DownFolder", "")
                If Not PATH_DOWNLOAD.Contains("\") Then
                    PATH_DOWNLOAD = PATH & "PCL\download\"
                    WriteIni("setup", "DownFolder", "")
                End If
                Log("[Setup] 新的下载文件夹目录：" & PATH_DOWNLOAD)
        End Select

    End Sub
    Private Sub ShowHide_Change(ByVal sender As Object, ByVal raiseByMouse As Boolean) Handles chSysUpdate.Change, raUiTheme4.Change, chUiBackground.Change, raLaunchMode1.Change, chDownMinecraftAssetsHint.Change, chHomeUpdate.Change, chHomeSave.Change, chHomeAutoplay.Change, raLaunchVisibility0.Change, raLaunchSkin3.Change, chUiBgm.Change
        '判定是否显示
        Dim IsShow As Boolean = sender.Checked
        If sender.Name = "raLaunchVisibility0" Or sender.Name = "raLaunchMode1" Then IsShow = Not IsShow
        '获取控件
        Dim ChangeGrid As Grid = FindName("pan" & Mid(sender.Name, 3))
        '改变高度
        If IsShow Then
            If UseControlAnimation Then
                AniStart({AaHeight(ChangeGrid, CType(ChangeGrid.Children(0), Object).Height - ChangeGrid.Height, 200, , New AniEaseJumpEnd(0.7)), AaOpacity(ChangeGrid, 1 - ChangeGrid.Opacity, 200)}, ChangeGrid.Name)
            Else
                ChangeGrid.Height = CType(ChangeGrid.Children(0), Object).Height
                ChangeGrid.Opacity = 1
            End If
        Else
            If UseControlAnimation Then
                AniStart({AaHeight(ChangeGrid, -ChangeGrid.Height, 150, , New AniEaseEnd), AaOpacity(ChangeGrid, -ChangeGrid.Opacity, 150)}, ChangeGrid.Name)
            Else
                ChangeGrid.Height = 0
                ChangeGrid.Opacity = 0
            End If
        End If
    End Sub
    Private Sub levelChange(ByVal Selection As String) Handles selecterHide.SelectionChange
        log("[Setup] 更改设置隐藏等级：" & Selection)
        If CanWrite Then WriteReg("SysLevel", selecterHide.SelectIndex)

        '变色
        Select Case Selection
            Case "简洁模式"
                AniStart({
                         AaForeGround(selecterHide, New MyColor(29, 141, 40) - selecterHide.Foreground)
                     }, "SelecterHideChangeColor")
            Case "普通模式"
                AniStart({
                         AaForeGround(selecterHide, New MyColor(182, 182, 20) - selecterHide.Foreground)
                     }, "SelecterHideChangeColor")
            Case "高级模式"
                AniStart({
                         AaForeGround(selecterHide, New MyColor(191, 86, 0) - selecterHide.Foreground)
                     }, "SelecterHideChangeColor")
        End Select

        '在 N 与 A 下显示的部分
        If Selection = "普通模式" Or Selection = "高级模式" Then
            chSysFeed.Visibility = Visibility.Visible
            chSysUpdateTest.Visibility = Visibility.Visible
            panSysUpdate2.Height = 56
            If panSysUpdate.Height > 1 And panSysUpdate.Height < 55 Then panSysUpdate.Height = panSysUpdate.Height + 28
            labSysLowMem.Visibility = Visibility.Visible
            panSysLowMem.Visibility = Visibility.Visible
            chUiShowLogo.Visibility = Visibility.Visible
            labUiLauncherOpacity.Visibility = Visibility.Visible
            textUiLauncherOpacity.Visibility = Visibility.Visible
            labDownSource.Visibility = Visibility.Visible
            panDownAssets.Visibility = Visibility.Visible
            panDownForge.Visibility = Visibility.Visible
            panDownMinecraft.Visibility = Visibility.Visible
            panDownOptiFine.Visibility = Visibility.Visible
            panDownVersion.Visibility = Visibility.Visible
            labDownMinecraft.Visibility = Visibility.Visible
            chDownMinecraftAssetsHint.Visibility = Visibility.Visible
            panDownMinecraftAssetsHint.Visibility = Visibility.Visible
            labDownMaxinum.Visibility = Visibility.Visible
            panDownMaxinum.Visibility = Visibility.Visible
            chHomeAutoplay.Visibility = Visibility.Visible
            panHomeAutoplay.Visibility = Visibility.Visible
        Else
            chSysFeed.Visibility = Visibility.Collapsed
            chSysUpdateTest.Visibility = Visibility.Collapsed
            panSysUpdate2.Height = 28
            If panSysUpdate.Height > 28 Then panSysUpdate.Height = panSysUpdate.Height - 28
            labSysLowMem.Visibility = Visibility.Collapsed
            panSysLowMem.Visibility = Visibility.Collapsed
            chUiShowLogo.Visibility = Visibility.Collapsed
            labUiLauncherOpacity.Visibility = Visibility.Collapsed
            textUiLauncherOpacity.Visibility = Visibility.Collapsed
            labDownSource.Visibility = Visibility.Collapsed
            panDownAssets.Visibility = Visibility.Collapsed
            panDownForge.Visibility = Visibility.Collapsed
            panDownMinecraft.Visibility = Visibility.Collapsed
            panDownOptiFine.Visibility = Visibility.Collapsed
            panDownVersion.Visibility = Visibility.Collapsed
            labDownMinecraft.Visibility = Visibility.Collapsed
            chDownMinecraftAssetsHint.Visibility = Visibility.Collapsed
            panDownMinecraftAssetsHint.Visibility = Visibility.Collapsed
            labDownMaxinum.Visibility = Visibility.Collapsed
            panDownMaxinum.Visibility = Visibility.Collapsed
            chHomeAutoplay.Visibility = Visibility.Collapsed
            panHomeAutoplay.Visibility = Visibility.Collapsed
        End If

        '只在 A 下显示的部分
        If Selection = "高级模式" Then
            chSysOffline.Visibility = Visibility.Visible
            labUiBackgroundURL.Visibility = Visibility.Visible
            textUiBackgroundURL.Visibility = Visibility.Visible
            labUiBarURL.Visibility = Visibility.Visible
            textUiBarURL.Visibility = Visibility.Visible
            labUiHide.Visibility = Visibility.Visible
            panUiHide1.Visibility = Visibility.Visible
            panUiHide2.Visibility = Visibility.Visible
            chLaunchNatives.Visibility = Visibility.Visible
            labLaunchJVM.Visibility = Visibility.Visible
            panLaunchJVM.Visibility = Visibility.Visible
            chLaunchLogWarn.Visibility = Visibility.Visible
            panLaunchVisibility02.Height = 56
            labLaunchServer.Visibility = Visibility.Visible
            textLaunchServer.Visibility = Visibility.Visible
            If panLaunchVisibility0.Height > 1 And panLaunchVisibility0.Height < 55 Then panLaunchVisibility0.Height = panLaunchVisibility0.Height + 28
            chLaunchMending.Visibility = Visibility.Visible
            chDownProcess.Visibility = Visibility.Visible
            labHomeVersion.Visibility = Visibility.Visible
            chHomeVersionOld.Visibility = Visibility.Visible
            chHomeVersionSwap.Visibility = Visibility.Visible
            chHomeHide.Visibility = Visibility.Visible
            chHomeFull.Visibility = Visibility.Visible
            chSysTest.Visibility = Visibility.Visible
        Else
            chSysOffline.Visibility = Visibility.Collapsed
            labUiBackgroundURL.Visibility = Visibility.Collapsed
            textUiBackgroundURL.Visibility = Visibility.Collapsed
            labUiBarURL.Visibility = Visibility.Collapsed
            textUiBarURL.Visibility = Visibility.Collapsed
            labUiHide.Visibility = Visibility.Collapsed
            panUiHide1.Visibility = Visibility.Collapsed
            panUiHide2.Visibility = Visibility.Collapsed
            chLaunchNatives.Visibility = Visibility.Collapsed
            labLaunchJVM.Visibility = Visibility.Collapsed
            panLaunchJVM.Visibility = Visibility.Collapsed
            panLaunchVisibility02.Height = 28
            chLaunchLogWarn.Visibility = Visibility.Collapsed
            labLaunchServer.Visibility = Visibility.Collapsed
            textLaunchServer.Visibility = Visibility.Collapsed
            If panLaunchVisibility0.Height > 28 Then panLaunchVisibility0.Height = panLaunchVisibility0.Height - 28
            chLaunchMending.Visibility = Visibility.Collapsed
            chDownProcess.Visibility = Visibility.Collapsed
            labHomeVersion.Visibility = Visibility.Collapsed
            chHomeVersionOld.Visibility = Visibility.Collapsed
            chHomeVersionSwap.Visibility = Visibility.Collapsed
            chHomeHide.Visibility = Visibility.Collapsed
            chHomeFull.Visibility = Visibility.Collapsed
            chSysTest.Visibility = Visibility.Collapsed
        End If

    End Sub
    Private Sub Reset_Change(ByVal sender As TextBox, ByVal e As Controls.TextChangedEventArgs) Handles textLaunchJVM.TextChanged, textDownMaxinum.TextChanged
        Dim ShouldReset As Boolean = sender.Text = FindName(sender.Name.Replace("text", "btn") & "Reset").Tag

        AniStart({
                 AaWidth(sender, 455 + If(ShouldReset, 45, 0) - sender.Width, 150, , New AniEaseEnd)
             }, sender.Name & "Reset")
    End Sub
    Private Sub Reset_Click(ByVal sender As Label, ByVal e As EventArgs) Handles btnLaunchJVMReset.MouseLeftButtonUp, btnDownMaxinumReset.MouseLeftButtonUp
        FindName(sender.Name.Replace("btn", "text").Replace("Reset", "")).Text = sender.Tag
    End Sub

    ''' <summary>
    ''' 设置输入检查结果。
    ''' </summary>
    ''' <param name="Reason">错误原因。如果没有错误则设置为空。</param>
    ''' <param name="TargetLabelName">作为显示目标的 Label 的名称。</param>
    ''' <param name="TargetTextBox">作为显示目标的 TextBox。</param>
    ''' <remarks></remarks>
    Private Sub SetCheckResult(ByVal Reason As String, ByVal TargetLabelName As String, ByVal TargetTextBox As TextBox)
        Dim TargetLabel As Label = Nothing
        If Not TargetLabelName = "" Then TargetLabel = FindName(TargetLabelName)
        If Reason = "" Then
            If Not IsNothing(TargetLabel) Then TargetLabel.Content = TargetLabel.Tag
            AniStart({
                     AaBorderBrush(TargetTextBox, New MyColor(Application.Current.Resources("ColorE4")) - New MyColor(CType(TargetTextBox.BorderBrush, SolidColorBrush).Color), 150),
                     AaForeGround(TargetTextBox, New MyColor(0, 0, 0) - New MyColor(CType(TargetTextBox.Foreground, SolidColorBrush).Color), 150)
                 }, TargetTextBox.Name)
        Else
            If Not IsNothing(TargetLabel) Then TargetLabel.Content = TargetLabel.Tag & " - " & Reason
            AniStart({
                     AaBorderBrush(TargetTextBox, New MyColor(255, 0, 0) - New MyColor(CType(TargetTextBox.BorderBrush, SolidColorBrush).Color), 150),
                     AaForeGround(TargetTextBox, New MyColor(255, 0, 0) - New MyColor(CType(TargetTextBox.Foreground, SolidColorBrush).Color), 150)
                 }, TargetTextBox.Name)
        End If
    End Sub

#End Region

#Region "左边栏"

    Private Sub selecter_Click(ByVal Selection As String, ByVal Index As Integer) Handles Selecter.Click
        AniStart({
                 AaValue(scroll, FindName("line" & Selection).TranslatePoint(New Point(0, 0), panSetupHost).Y - 33, 150, , New AniEaseEnd)
             }, "SelecterChange" & GetUUID())
    End Sub
    Private Sub timerLeft_Tick() Handles timerLeft.Tick
        Dim FindIndex As Byte = 0
        For i = selecter.ShowList.Length - 1 To 0 Step -1
            Dim lab As Rectangle = FindName("line" & selecter.ShowList(i))
            If lab.TranslatePoint(New Point(0, 0), panSetupHost).Y <= 150 Then
                FindIndex = i
                Exit For
            End If
        Next
        If Not FindIndex = selecter.SelectIndex Then selecter.SelectIndex = FindIndex
    End Sub

#End Region

#Region "系统设置"

    '调试模式
    Private Sub chSysTest_Change() Handles chSysTest.Change
        MODE_DEBUG = chSysTest.Checked Or MODE_DEVELOPER
        frmMain.labTopVer.Content = VERSION_NAME & If(MODE_DEVELOPER, " | 开发者模式", If(MODE_DEBUG, " | 调试模式", "")) & If(MODE_OFFLINE, " | 离线模式", "")
    End Sub
    '清理缓存
    Private Sub btnSysCache_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSysCache.Click
        On Error Resume Next
        Dim TotalSize As FileSize = 0
        Dim Files As ArrayList = GetFilesFromPath(PATH & "PCL\cache")
        For Each File As String In Files
            TotalSize = TotalSize + GetFileSize(File)
            IO.File.Delete(File)
        Next
        If TotalSize = 0 Then
            ShowHint(New HintConverter("没有可清除的缓存", HintState.Finish))
        Else
            ShowHint(New HintConverter("已清除 " & TotalSize.ToString & " 缓存", HintState.Finish))
        End If
    End Sub
    '清理支持库
    Private Sub btnSysLib_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSysLib.Click
        Try
            Dim TotalSize As FileSize = 0
            '删除libraries
            Dim Files As ArrayList = GetFilesFromPath(PATH_MC & "libraries")
            For Each File As String In Files
                Try
                    If Not (File.ToLower.Contains("forge") Or File.ToLower.Contains("optifine")) Then
                        TotalSize = TotalSize + GetFileSize(File)
                        IO.File.Delete(File)
                    End If
                Catch
                End Try
            Next
            '返回
            If TotalSize = 0 Then
                ShowHint(New HintConverter("没有可清除的支持库", HintState.Finish))
            Else
                ShowHint(New HintConverter("已清除 " & TotalSize.ToString & " 支持库", HintState.Finish))
            End If
        Catch ex As Exception
            ExShow(ex, "清除支持库失败", ErrorLevel.MsgboxAndFeedback)
        End Try
    End Sub
    '清理版本信息
    Private Sub btnSysVersion_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSysVersion.Click
        Try
            '删除各版本信息
            If Directory.Exists(PATH_MC & "versions\") Then
                'versions文件夹存在
                For Each Folder As DirectoryInfo In (New DirectoryInfo(PATH_MC & "versions")).GetDirectories
                    If Directory.Exists(Folder.FullName & "\PCL") Then Directory.Delete(Folder.FullName & "\PCL", True)
                Next
                File.Delete(PATH_MC & "PCL.ini")
            End If
            '返回
            ShowHint(New HintConverter("已清除版本信息", HintState.Finish))
            Pool.Add(New Thread(AddressOf PoolVersionList))
        Catch ex As Exception
            ExShow(ex, "清除版本信息失败", ErrorLevel.MsgboxAndFeedback)
        End Try
    End Sub
    '打开日志
    Private Sub btnSysLog_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSysLog.Click
        Shell("notepad", """" & PATH & "PCL\log.txt""")
    End Sub
    '打开文件夹
    Private Sub btnSysFolder_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSysFolder.Click
        Process.Start(PATH & "PCL\")
    End Sub
    '意见反馈
    Private Sub btnSysBack_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSysBack.Click, btnAboutFolder.Click
        Feedback()
    End Sub
    '重置启动器
    Private Sub ReloadPCL(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSysAll.Click
        If Not CanWrite Then Exit Sub
        If MyMsgbox("重置将会删除 PCL 的所有文件（主程序与 Minecraft 的文件不受影响），你确定要重置吗？重置后 PCL 将会自动关闭。", "重置警告", "确定", "取消", IsWarn:=True) = 2 Then Exit Sub

StartReload:
        On Error Resume Next
        frmMain.Visibility = Visibility.Collapsed
        frmMain.UpdateLayout()
        '清除注册表信息
        'My.Computer.Registry.CurrentUser.DeleteSubKey("Software\" & APPLICATION_SHORT_NAME, False)
        '读取各版本信息
        If Directory.Exists(PATH_MC & "versions\") Then
            'versions文件夹存在
            For Each Folder As DirectoryInfo In (New DirectoryInfo(PATH_MC & "versions")).GetDirectories
                If Directory.Exists(Folder.FullName & "\PCL") Then Directory.Delete(Folder.FullName & "\PCL", True)
            Next
            File.Delete(PATH_MC & "PCL.ini")
        End If
        '删除PCL存档目录
        Directory.Delete(PATH & "PCL", True)
        File.Delete(PATH_MC & "PCL.ini")
        '结束程序
        EndForce()
    End Sub

#End Region

#Region "界面设置"

    '主题
    Private Sub raUiThemes_Change(ByVal sender As Radiobox)
        If CanWrite And sender.Checked Then RefreshTheme()
    End Sub
    '自定义
    Private Sub ThemeCustomChange(ByVal sender As StackPanel, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles btnUiCustom1.MouseDown, btnUiCustom2.MouseDown, btnUiCustom3.MouseDown, btnUiCustom4.MouseDown, btnUiCustom5.MouseDown
        If Not CanWrite Then Exit Sub
        On Error Resume Next
        Dim selector As New Forms.ColorDialog
        selector.Color = System.Drawing.Color.FromArgb(ReadIni("setup", sender.Name.Replace("btnUiCustom", "UiThemeColorR"), CType(FindResource("ColorE" & Right(sender.Name, 1)), Color).R), ReadIni("setup", sender.Name.Replace("btnUiCustom", "UiThemeColorG"), CType(FindResource("ColorE" & Right(sender.Name, 1)), Color).G), ReadIni("setup", sender.Name.Replace("btnUiCustom", "UiThemeColorB"), CType(FindResource("ColorE" & Right(sender.Name, 1)), Color).B))
        selector.ShowDialog()
        If Not selector.Color = CType(FindResource("ColorE" & Right(sender.Name, 1)), MyColor) Then
            WriteIni("setup", sender.Name.Replace("btnUiCustom", "UiThemeColorR"), selector.Color.R)
            WriteIni("setup", sender.Name.Replace("btnUiCustom", "UiThemeColorG"), selector.Color.G)
            WriteIni("setup", sender.Name.Replace("btnUiCustom", "UiThemeColorB"), selector.Color.B)
            RefreshTheme()
        End If
        selector.Dispose()
    End Sub
    '设置背景图片
    Private Sub btnUiCustomBackground_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnUiBackgroundSet.Click
        If CanWrite Then
            '试图启用
            Dim fileName As String = SelectFile("常用图片文件(*.png;*.jpg;*.gif;*.jpeg)|*.png;*.jpg;*.gif;*.jpeg", "选择作为背景的图片（最适宜大小为 740×323）")
            '选择文件结束
            If fileName = "" Then Exit Sub
            Try
                '设置当前显示
                frmMain.imgMainBg.Source = New MyBitmap(fileName)
                '拷贝文件
                File.Delete(PATH & "PCL\back.png")
                File.Copy(fileName, PATH & "PCL\back.png")
            Catch ex As Exception
                ExShow(ex, "切换背景图片失败", ErrorLevel.MsgboxAndFeedback)
            Finally
                chUiBackground.Checked = File.Exists(PATH & "PCL\back.png")
            End Try
        End If
    End Sub
    '清除背景图片
    Private Sub btnUiBackgroundReset_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnUiBackgroundReset.Click
        File.Delete(PATH & "PCL\back.png")
        chUiBackground.Checked = File.Exists(PATH & "PCL\back.png")
        RefreshTheme()
    End Sub
    '背景图片框显示
    Private Sub chUiBackground_Change() Handles chUiBackground.Change
        btnUiBackgroundReset.Visibility = If(chUiBackground.Checked, Visibility.Visible, Visibility.Collapsed)
    End Sub
    '设置顶栏图片
    Private Sub btnUiBarSet_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnUiBarSet.Click
        If CanWrite Then
            '试图启用
            Dim fileName As String = SelectFile("常用图片文件(*.png;*.jpg;*.gif;*.jpeg)|*.png;*.jpg;*.gif;*.jpeg", "选择作为顶栏的图片（最适宜大小为 740×92）")
            '选择文件结束
            If fileName = "" Then Exit Sub
            Try
                '拷贝文件
                File.Delete(PATH & "PCL\top.png")
                File.Copy(fileName, PATH & "PCL\top.png")
                '设置当前显示
                CType(CType(frmMain.panTop.Background, Object), ImageBrush).ImageSource = New MyBitmap(fileName)
                CType(frmMain.panTop.Background, Object).Stretch = Stretch.None
                CType(frmMain.panTop.Background, Object).TileMode = TileMode.Tile
                CType(CType(frmMain.panTop.Background, Object), ImageBrush).Viewport = (New RectConverter()).ConvertFromString("0,0," & CType(CType(frmMain.panTop.Background, Object).ImageSource, ImageSource).Width & "," & CType(CType(frmMain.panTop.Background, Object).ImageSource, ImageSource).Height)
            Catch ex As Exception
                ExShow(ex, "切换顶栏图片失败", ErrorLevel.MsgboxAndFeedback)
            Finally
                btnUiBarReset.Visibility = If(File.Exists(PATH & "PCL\top.png"), Visibility.Visible, Visibility.Collapsed)
            End Try
        End If
    End Sub
    '清除顶栏图片
    Private Sub btnUiBarReset_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnUiBarReset.Click
        File.Delete(PATH & "PCL\top.png")
        btnUiBarReset.Visibility = If(File.Exists(PATH & "PCL\top.png"), Visibility.Visible, Visibility.Collapsed)
        RefreshTheme()
    End Sub
    '设置BGM
    Private Sub btnUiBgmSet_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnUiBgmSet.Click
        If CanWrite Then
            '试图启用
            Dim fileName As String = SelectFile("音乐(*.wav; .mp3)|*.wav;*.mp3", "选择背景音乐文件")
            '选择文件结束
            If fileName = "" Then Exit Sub
            Try
                btnUiBgmReset_Click(Nothing, Nothing)
                '拷贝文件
                Dim currentPath As String = PATH & "PCL\bgm" & If(fileName.EndsWith(".wav"), ".wav", ".mp3")
                File.Copy(fileName, currentPath)
                '设置当前音乐
                PlayBgm(currentPath, False, True)
                IsPlayingMusic = True
            Catch ex As Exception
                ExShow(ex, "设置背景音乐失败", ErrorLevel.AllUsers)
            Finally
                Dim IsRight As Boolean = File.Exists(PATH & "PCL\bgm.mp3") Or File.Exists(PATH & "PCL\bgm.wav")
                chUiBgm.Checked = IsRight
                btnUiBgmReset.Visibility = If(IsRight, Visibility.Visible, Visibility.Collapsed)
                frmMain.imgTopMusic.Visibility = If(IsRight, Visibility.Visible, Visibility.Collapsed)
            End Try
        End If
    End Sub
    '清除背景音乐
    Private Sub btnUiBgmReset_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnUiBgmReset.Click
        If File.Exists(PATH & "PCL\bgm.wav") Then
            PlayBgm(PATH & "PCL\bgm.wav", True)
            File.Delete(PATH & "PCL\bgm.wav")
        End If
        If File.Exists(PATH & "PCL\bgm.mp3") Then
            PlayBgm(PATH & "PCL\bgm.mp3", True)
            File.Delete(PATH & "PCL\bgm.mp3")
        End If
        btnUiBgmReset.Visibility = Visibility.Collapsed
        frmMain.imgTopMusic.Visibility = Windows.Visibility.Collapsed
        chUiBgm.Checked = False
        IsPlayingMusic = False
    End Sub

#End Region

#Region "启动设置"

    '重新加载 MC 文件夹列表显示
    Private Sub LoadMinecraftFolderList()
        Try
            panLaunchFolderList.Children.Clear()
            Dim SelectingFolder = ReadIni("setup", "LaunchFolderSelect", "")
            Dim AllFoldersList As New ArrayList From {PATH & ".minecraft\"}
            Dim MojangPath As String = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) & "\.minecraft\"
            If CheckDirectoryPermission(MojangPath & "versions\") Then AllFoldersList.Add(MojangPath)
            AllFoldersList.AddRange(ReadIni("setup", "LaunchFolders", "").Split("|"))
            AllFoldersList = ArrayNoDouble(AllFoldersList.ToArray)
            For Each Folder As String In AllFoldersList
                If Not Folder.Replace(" ", "") = "" Then
                    Dim Disc As String
                    Select Case Folder
                        Case PATH & ".minecraft\"
                            Disc = "当前文件夹（" & Folder & "）"
                        Case MojangPath
                            Disc = "官方启动器文件夹（" & Folder & "）"
                        Case Else
                            Disc = Folder
                    End Select
                    Dim Radio As New Radiobox With {.Context = Disc, .Checked = Folder = SelectingFolder, .Tag = "LaunchFolderSelect/" & Folder, .Name = "LaunchFolderSelectUUID" & GetUUID()}
                    AddHandler Radio.Change, AddressOf RadioBox_Change
                    If Disc = Folder Then AddHandler Radio.MouseRightButtonUp, AddressOf LaunchFolderRemove
                    panLaunchFolderList.Children.Add(Radio)
                End If
            Next
        Catch ex As Exception
            ExShow(ex, "重载文件夹列表失败", ErrorLevel.MsgboxAndFeedback)
        End Try
    End Sub
    '重置窗口尺寸可见性
    Private Sub LaunchSizeResetVisible() Handles textLaunchModeWindowHeight.TextChanged, textLaunchModeWindowWidth.TextChanged
        Dim ShouldReset As Boolean = Not (textLaunchModeWindowHeight.Text = "480" And textLaunchModeWindowWidth.Text = "854")
        AniStart({
                 AaWidth(btnLaunchModeWindowReset, If(ShouldReset, 45, 0) - btnLaunchModeWindowReset.Width, 300, , New AniEaseEnd)
             }, "btnLaunchModeWindowResetReset")
    End Sub
    '重置窗口尺寸
    Private Sub btnLaunchModeWindowReset_MouseLeftButtonUp(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles btnLaunchModeWindowReset.MouseLeftButtonUp
        textLaunchModeWindowHeight.Text = "480"
        textLaunchModeWindowWidth.Text = "854"
    End Sub
    '添加 MC 文件夹
    Private Sub btnLaunchFolderAdd_MouseLeftButtonUp(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles btnLaunchFolderAdd.MouseLeftButtonUp
        Try
            Dim NewFolder As String = SelectFolder("选择 Minecraft 文件夹。" & vbCrLf & "（这个文件夹可以不叫 .minecraft）")
            If NewFolder = "" Then Exit Sub
            '检测文件夹
            If CheckDirectoryPermission(NewFolder) Then
                NewFolder = NewFolder & "\" '加上斜杠……
                '检测通过，判断是否重复
                If NewFolder = PATH & ".minecraft\" Or NewFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) & "\.minecraft\" Then
                    ShowHint(New HintConverter("选择的文件夹有误", HintState.Warn))
                    Exit Sub
                End If
                '判断是否已经添加过
                Dim AllFoldersList As New ArrayList
                Dim ResentFolder As String = ReadIni("setup", "LaunchFolders", "")
                If Not ResentFolder = "" Then AllFoldersList.AddRange(ResentFolder.Split("|"))
                If AllFoldersList.Contains(NewFolder) Then
                    ShowHint(New HintConverter("列表中已经存在本文件夹", HintState.Warn))
                    Exit Sub
                End If
                '判断是否含有Versions文件夹
                If Not Directory.Exists(NewFolder & "versions") Then
                    If MyMsgbox("该文件夹中不包含任何 Minecraft 版本，是否继续添加？", "添加确认", "确定", "取消") = 2 Then Exit Sub
                End If
                '添加到文件夹列表
                AllFoldersList.Add(NewFolder)
                WriteIni("setup", "LaunchFolders", Join(AllFoldersList.ToArray, "|"))
                ShowHint("右键即可从列表中移除该文件夹")
                LoadMinecraftFolderList()
            Else
                '没有检测通过
                ShowHint(New HintConverter("选择的文件夹有误", HintState.Warn))
            End If
        Catch ex As Exception
            ExShow(ex, "添加文件夹失败", ErrorLevel.MsgboxAndFeedback)
        End Try
    End Sub
    '移除 MC 文件夹
    Private Sub LaunchFolderRemove(ByVal sender As Radiobox, ByVal e As System.Windows.Input.MouseButtonEventArgs)
        If MyMsgbox("是否要将该文件夹从 Minecraft 文件夹列表移除？" & vbCrLf & "　" & sender.Context, "移除确认", , "取消") = 1 Then
            Dim CurrentList As New ArrayList(ReadIni("setup", "LaunchFolders", "").Split("|"))
            CurrentList.Remove(sender.Context)
            WriteIni("setup", "LaunchFolders", Join(CurrentList.ToArray, "|"))
            If ReadIni("setup", "LaunchFolderSelect", "") = sender.Context Then
                WriteIni("setup", "LaunchFolderSelect", PATH & ".minecraft\")
                PoolMinecraftFolder()
            End If
            LoadMinecraftFolderList()
        End If
    End Sub
    '显示最大内存
    Private Sub textLaunchMaxRam_MouseEnter(ByVal sender As Object, ByVal e As MouseEventArgs) Handles textLaunchMaxRam.MouseEnter
        textLaunchMaxRam.ToolTip =
            "当前内存剩余：" & Int((New Devices.ComputerInfo).AvailablePhysicalMemory / 1024 / 1024) & "M / " & Int((New Devices.ComputerInfo).TotalPhysicalMemory / 1024 / 1024) & "M" & vbCrLf &
            "可设置范围：256M ~ " & Math.Min(If(Environment.Is64BitOperatingSystem, Double.PositiveInfinity, 1024), Int((New Devices.ComputerInfo).TotalPhysicalMemory / 1024 / 1024) - 512) & "M"
    End Sub
    '手动选择
    Private Sub btnLaunchJavaHand_MouseUp(ByVal sender As Object, ByVal e As MouseButtonEventArgs) Handles btnLaunchJavaHand.MouseLeftButtonDown
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
                textLaunchJava.Text = Mid(fileDialog.FileName, 1, fileDialog.FileName.LastIndexOf("\"))
            End If
        End Using
    End Sub
    '自动查找
    Private Sub btnLaunchJavaAuto_MouseUp(ByVal sender As Object, ByVal e As MouseButtonEventArgs) Handles btnLaunchJavaAuto.MouseLeftButtonDown
        PoolJavaFolder(True)
        textLaunchJava.Text = PATH_JAVA
    End Sub

#End Region

#Region "关于"

    'Logo 彩蛋
    Private AboutClickCount As Double = 0
    Private Sub imgAboutLogo_MouseUp(ByVal sender As Object, ByVal e As MouseButtonEventArgs) Handles imgAboutLogo.MouseLeftButtonDown

        If imgAboutLogo.Opacity < 0.5 Then Exit Sub
        Dim x As Double
        Dim y As Double
        Do
            x = RandomInteger(-5, 5)
            y = RandomInteger(-5, 5)
        Loop Until Math.Abs(x) + Math.Abs(y) > 3
        AniStart({
                 AaX(imgAboutLogo, x, 40, , New AniEaseEnd),
                 AaY(imgAboutLogo, y, 40, , New AniEaseEnd),
                 AaX(imgAboutLogo, -x, 40, 40, New AniEaseStart),
                 AaY(imgAboutLogo, -y, 40, 40, New AniEaseStart)
             }, "EasterEgg" & GetUUID())

        AboutClickCount = AboutClickCount + 1
        Select Case AboutClickCount
            Case 5
                If ReadReg("ThemeHunluan", "False") = "True" Then
                    ShowHint("真的真的什么也没了，就一个隐藏主题而已啦，点啊点有意思么……")
                    AboutClickCount = 170
                Else
                    ShowHint("点这个很好玩么……")
                End If
            Case 25
                If MyMsgbox("你现在是不是超无聊的？", "确认一下", "是的", "并不是") = 2 Then
                    ShowHint("那你还点啥……真是搞不懂。")
                End If
            Case 50
                ShowHint("嗯，加油吧，嗯……")
            Case 75
                MyMsgbox("隐藏主题 混乱黄 已解锁，请到设置页面查看！", "主题已解锁", "我知道了")
                SendStat("彩蛋", "主题", "混乱黄")
                WriteReg("ThemeHunluan", "True")
                RefreshHidden()
            Case 95
                ShowHint("你咋还这么无聊啊？")
            Case 130
                ShowHint("后面什么都没有了哦！")
            Case 155
                Select Case MyMsgbox("你真的不累么？", "温馨提示", "累死了", "真的不累")
                    Case 1
                        ShowHint("那你就别点了喂……后面真的真的真的什么都没有了！")
                    Case 2
                        If MyMsgbox("你真的真的不累么？", "超温馨的温馨提示", "累死了", "真的真的不累") = 2 Then
                            If MyMsgbox("你真的真的真的不累么？", "超超超温馨的温馨提示", "累死了", "真的真的真的不累") = 2 Then
                                ShowHint("好吧……不过后面是真的啥也没了，不用点了真的。")
                            Else
                                ShowHint("那你就别点了喂……后面真的真的真的什么都没有了！")
                            End If
                        Else
                            ShowHint("那你就别点了喂……后面真的真的真的什么都没有了！")
                        End If
                End Select
            Case 200
                ShowHint("还点，还点，我不让你点了……")
                imgAboutLogo.IsHitTestVisible = False
        End Select


    End Sub
    '加群
    Private Sub btnSysQQ_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAboutQQ.Click
        Process.Start("http://shang.qq.com/wpa/qunwpa?idkey=f003df7898678e4bdeb53061d8f9b04445c9ff206b0515fdab9bfbd5cb3eaec0")
    End Sub
    '捐助
    Private Sub btnAboutDonate_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnAboutDonate.Click
        Try
            Process.Start("https://afdian.net/@LTCat")
        Catch ex As Exception
            ExShow(ex, "捐助失败", ErrorLevel.DebugOnly)
        End Try
    End Sub

#End Region

    '    Private Sub SourceClick(ByVal sender As ListItem, ByVal e As System.Windows.Input.MouseButtonEventArgs)
    '        Dim setup As New msgSource(sender)
    '        If Not IsNothing(setup.panMain.Parent) Then setup.panMain.Parent.SetValue(ContentPresenter.ContentProperty, Nothing)
    '        If MyMsgbox("推荐源设置", setup.panMain, , "保存", "删除") = 2 Or sender.MainText = "" Or sender.SubText = "" Then
    '            '删除
    '            panHomeSourceList.Children.Remove(sender)
    '        End If
    '        Dim sources As New ArrayList
    '        Dim urls As New ArrayList
    '        Dim uses As New ArrayList
    '        If panHomeSourceList.Children.Count > 1 Then
    '            For i = 0 To panHomeSourceList.Children.Count - 1
    '                If panHomeSourceList.Children(i).GetType.Name = "ListItem" Then
    '                    Dim item As ListItem = panHomeSourceList.Children(i)
    '                    sources.Add(item.MainText)
    '                    urls.Add(item.SubText)
    '                    uses.Add(item.Version.LoadedByFile)
    '                End If
    '            Next
    '        End If
    '        WriteIni("setup", "LaunchMainSource", Join(sources.ToArray, "|"))
    '        WriteIni("setup", "LaunchMainURL", Join(urls.ToArray, "|"))
    '        WriteIni("setup", "LaunchMainUse", Join(uses.ToArray, "|"))
    '    End Sub
    '    Private Sub IconClick(ByVal sender As ListItem, ByVal e As System.Windows.Input.MouseButtonEventArgs)
    '        sender.Version.LoadedByFile = Not sender.Version.LoadedByFile
    '        sender.Logo = New BitmapImage(New Uri("/Images/Block-" & sender.Version.LoadedByFile & ".png", UriKind.Relative))
    '        Dim uses As New ArrayList
    '        If panHomeSourceList.Children.Count > 1 Then
    '            For i = 0 To panHomeSourceList.Children.Count - 2
    '                If panHomeSourceList.Children(i).GetType.Name = "ListItem" Then
    '                    Dim item As ListItem = panHomeSourceList.Children(i)
    '                    uses.Add(item.Version.LoadedByFile)
    '                End If
    '            Next
    '        End If
    '        WriteIni("setup", "LaunchMainUse", Join(uses.ToArray, "|"))
    '    End Sub
    '    Private Sub labPageMainAdd_MouseUp(ByVal sender As Object, ByVal e As EventArgs) Handles itemHomeSourceAdd.MouseLeftButtonUp
    '        Dim item = New ListItem With {.MainText = "", .SubText = "", .CanCheck = False, .Width = 363, .ButtonLogo = New BitmapImage(New Uri("/Images/appbar.settings.png", UriKind.Relative)), .UseLayoutRounding = True, .Name = "itemLaunchMain" & Rnd.NextDouble.ToString.Replace(".", ""), .Logo = New BitmapImage(New Uri("/Images/Block-True.png", UriKind.Relative))}
    '        item.Version.LoadedByFile = True
    '        panHomeSourceList.Children.Insert(panHomeSourceList.Children.Count - 1, item)
    '        AddHandler item.ButtonClick, AddressOf SourceClick
    '        AddHandler item.IconClick, AddressOf IconClick
    '        SourceClick(item, e)
    '    End Sub

End Class
