Public Class formMinecraft

    Public Minecraft As Process = Nothing
    Public MinecraftVersion As MCVersion
    Public IsHidden As Boolean = False
    Public Logs As New ArrayList(10000)
    Public WaitToAppend As New ArrayList()
    Public MaxLength As Integer = 50 '最多显示的日志条数
    Public LogLastCount As Integer = 0 '上次显示时的日志条数，如果一样就不刷新
    Public DontExit As Boolean = False
    Public SendWarn As Boolean = ReadIni("setup", "LaunchLogWarn", "False") = "True"
    Public Sub New(ByVal MinecraftProcess As Process, ByVal Version As MCVersion, ByVal Hidden As Boolean)
        Try

            UseControlAnimation = False
            InitializeComponent()
            UseControlAnimation = True
            log("[Minecraft] 开始 Minecraft 日志监控")
            Minecraft = MinecraftProcess
            MinecraftVersion = Version
            IsHidden = Hidden
            Me.Title = "Minecraft 日志输出 - " & MinecraftVersion.Name
            AddHandler Minecraft.OutputDataReceived, AddressOf MinecraftDataReceived
            AddHandler Minecraft.ErrorDataReceived, AddressOf MinecraftDataReceived

            If IsHidden Then
                Me.Left = 100000
                Me.Visibility = Visibility.Collapsed
                Me.Show()
            Else
                Me.Show()
                log("[Minecraft] 显示 Minecraft 日志窗体")
                Try
                    Me.Icon = BitmapFrame.Create(New Uri(PATH_IMAGE & "icon.ico"))
                Catch ex As Exception
                    ExShow(ex, "创建 MC 监控窗口图标失败")
                End Try
                Me.ShowInTaskbar = True
                Me.WindowStyle = Windows.WindowStyle.SingleBorderWindow
                Me.Visibility = Visibility.Visible
                textOutput.Focus()
                '加载默认进程优先级
                Select Case Val(ReadReg("LaunchLevel", "1"))
                    Case 0
                        raHigh.Checked = True
                    Case 1
                        raMiddle.Checked = True
                    Case 2
                        raLow.Checked = True
                End Select
            End If

        Catch ex As Exception
            ExShow(ex, "创建 MC 监控窗口失败", ErrorLevel.MsgboxAndFeedback)
        End Try
    End Sub
    Private Sub MinecraftDataReceived(ByVal sender As Object, ByVal e As DataReceivedEventArgs)
        If Minecraft.HasExited Then Exit Sub
        WaitToAppend.Add(e.Data)
    End Sub
    Private Function AppendText(ByVal Text As String) As Boolean
        If IsNothing(Text) Or IsNothing(textOutput) Then Return True
        Text = Text.Replace(vbCrLf, vbCr).Replace(vbLf, vbCr).Replace(vbCr, vbCrLf)
        Logs.Add(Text)
        If Text.StartsWith("[PCL]") Then log(Text.Replace("[PCL]", "[Minecraft]"))
        '转发日志
        If Text.ToLower.Contains("error") Or Text.ToLower.Contains("exception") Or Text.Contains("错误") Then
            If Not (Text.Contains("THIS IS NOT A ERROR") Or Text.Contains("[CHAT]")) Then log("[Minecraft] 错误信息：" & Text)
        End If
        If Text.ToLower.Contains("warn") Or Text.Contains("警告") Then
            If SendWarn And Not Text.Contains("[CHAT]") Then log("[Minecraft] 警告信息：" & Text)
        End If
        If (Text.Contains("Game crashed! Crash report saved to:") Or Text.Contains("This crash report has been saved to:")) And Not Text.Contains("[CHAT]") Then
            On Error Resume Next
            Dim ReportAddress As String = RegexSearch(Text, "[A-N]{1}:\\.*")(0)
            log("[Minecraft] Minecraft 已崩溃，崩溃日志：" & ReportAddress)
            DontExit = True
            If File.Exists(ReportAddress) Then
                On Error Resume Next
                WriteFile(ReportAddress, ReadFileToEnd(ReportAddress).Replace(vbCrLf, vbCr).Replace(vbLf, vbCr).Replace(vbCr, vbCrLf))
                Shell("notepad", ReportAddress)
                ShowHintWindow("Minecraft " & If(MinecraftVersion.Version = "未知", "", MinecraftVersion.Version & " ") & "已崩溃", "PCL 将会为你打开崩溃报告。")
            Else
                ShowHintWindow("Minecraft " & If(MinecraftVersion.Version = "未知", "", MinecraftVersion.Version & " ") & "已崩溃", "崩溃报告未找到。")
            End If
            MinecraftCrash()
            Return False
        End If
        Return True
    End Function

    Private Sub timerRefresh_Tick() Handles timerRefresh.Tick
        Try
            '输出文本
            Dim Copyed As New ArrayList(WaitToAppend)
            WaitToAppend.Clear()
            For Each Str As String In Copyed
                If Not AppendText(Str) Then Exit Sub '如果是崩溃文本结束刷新
            Next
            '游戏退出检查
            If Minecraft.HasExited Then
                AppendText("[PCL] Minecraft 已退出，返回值：" & Minecraft.ExitCode)
                btnClose.IsEnabled = False
                raHigh.IsEnabled = False
                raMiddle.IsEnabled = False
                raLow.IsEnabled = False
                timerRefresh.IsEnabled = False
                If timerTitle.IsEnabled Then
                    '尚未捕获到 Minecraft 窗口
                    AppendText("[PCL] Minecraft 窗口未出现")
                    DontExit = True
                    MinecraftCrash()
                End If
                timerTitle.IsEnabled = False
                If (Not (Val(ReadIni("setup", "LaunchVisibility", "3")) = 2 Or Val(ReadIni("setup", "LaunchVisibility", "3")) = 4)) And Not DontExit Then
                    EndNormal()
                End If
            End If
            '显示日志
            If Not (Logs.Count = LogLastCount Or IsHidden) Then
                LogLastCount = Logs.Count
                Dim resText As String = ""
                For i = Logs.Count - 1 To Math.Max(0, Logs.Count - MaxLength) Step -1
                    resText = If(resText = "", "", resText & vbCrLf) & Logs(i)
                Next
                textOutput.Text = resText
                textOutput.Focus()
            End If
        Catch ex As Exception
            ExShow(ex, "MC 监控窗口主时钟出错")
        End Try
    End Sub

    Private Sub formMinecraft_Closed(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Closed
        If Not ReadIni("setup", "LaunchVisibility", "3") = "2" Then EndNormal()
    End Sub

    Private Sub btnClose_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnClose.Click
        'RunCMD("taskkill /PID " & Minecraft.Id & " /F /T", False)
        btnClose.IsEnabled = False
        raHigh.IsEnabled = False
        raMiddle.IsEnabled = False
        raLow.IsEnabled = False
        timerTitle.IsEnabled = False
        On Error Resume Next
        Minecraft.Kill()
        AppendText("[PCL] 已发出强制关闭命令")
        If GameWindowState = LoadState.Loading Then GameWindowState = LoadState.Loaded
    End Sub

    Private Sub LevelChange(ByVal sender As Object, ByVal e As EventArgs) Handles raHigh.MouseUp, raMiddle.MouseUp, raLow.MouseUp
        Try
            If sender.Equals(raHigh) Then
                If Not Minecraft.PriorityClass = ProcessPriorityClass.AboveNormal Then
                    Minecraft.PriorityClass = ProcessPriorityClass.AboveNormal
                    AppendText("[PCL] Minecraft 进程优先级被设置为 高于正常")
                End If
            ElseIf sender.Equals(raMiddle) Then
                If Not Minecraft.PriorityClass = ProcessPriorityClass.Normal Then
                    Minecraft.PriorityClass = ProcessPriorityClass.Normal
                    AppendText("[PCL] Minecraft 进程优先级被设置为 正常")
                End If
            Else
                If Not Minecraft.PriorityClass = ProcessPriorityClass.BelowNormal Then
                    Minecraft.PriorityClass = ProcessPriorityClass.BelowNormal
                    AppendText("[PCL] Minecraft 进程优先级被设置为 低于正常")
                End If
            End If
        Catch ex As Exception
            AppendText("[PCL] Minecraft 进程优先级设置失败：" & GetStringFromException(ex))
        End Try
    End Sub
    Private Sub LengthChange(ByVal sender As Object, ByVal e As EventArgs) Handles raUnlimited.MouseUp, ra150.MouseUp, ra50.MouseUp
        If sender.Equals(raUnlimited) Then
            MaxLength = Integer.MaxValue
        ElseIf sender.Equals(ra150) Then
            MaxLength = 150
        Else
            MaxLength = 50
        End If
        LogLastCount = 0 '强制刷新
        timerRefresh_Tick()
    End Sub

    Private Sub timerTitle_Tick() Handles timerTitle.Tick
        If Minecraft.HasExited Then Exit Sub
        If Minecraft.MainWindowHandle = 0 Then Exit Sub
        If Minecraft.MainWindowTitle.Contains("Machine") Or Minecraft.MainWindowTitle.Contains("JVM") Then Exit Sub
        '找到窗口
        timerTitle.IsEnabled = False
        AppendText("[PCL] Minecraft 窗口已加载")
        GameWindowState = LoadState.Loaded
        '改变窗口标题
        Dim th As New Thread(Sub()
                                 Try
                                     Thread.Sleep(2000)
                                     If ReadReg("LaunchTopmost", "False") = "True" Then
                                         AppendText("[PCL] 已设置窗口置顶")
                                         SetWindowPos(Minecraft.MainWindowHandle, -1, 0, 0, 0, 0, SWP_NOMOVE Or SWP_NOSIZE)
                                     End If
                                     Dim NewTitle As String = ReadIni("setup", "LaunchTitle", "")
                                     If NewTitle = "" Then Exit Sub
                                     If NewTitle.Contains("\\r") And File.Exists(PATH & "PCL\RandomTitle.txt") Then
                                         Dim AllText As String() = ReadFileToEnd(PATH & "PCL\RandomTitle.txt", Encoding.Default).Replace(vbCrLf, vbCr).Replace(vbLf, vbCr).Split(vbCr)
                                         NewTitle = NewTitle.Replace("\\r",
                                                                      If(AllText.Length = 0, "", AllText(RandomInteger(0, AllText.Length - 1))))
                                     End If
                                     NewTitle = NewTitle.Replace("\\v", If(MinecraftVersion.Version = "未知", "", MinecraftVersion.Version))
                                     AppendText("[PCL] 已将窗口标题修改为：" & NewTitle)
                                     SetWindowText(Minecraft.MainWindowHandle, NewTitle)
                                 Catch ex As Exception
                                     AppendText("[PCL] 设置窗口标题失败：" & GetStringFromException(ex, MODE_DEBUG))
                                 End Try
                             End Sub)
        th.Start()
    End Sub

    Private Sub MinecraftCrash()
        SendStat("启动", "Crash", MinecraftVersion.Name)
        If GameWindowState = LoadState.Loading Then GameWindowState = LoadState.Failed
        timerRefresh.IsEnabled = False
        '强制显示日志内容
        LogLastCount = Logs.Count
        Dim resText As String = ""
        For i = 0 To Logs.Count - 1
            resText = If(resText = "", "", resText & vbCrLf) & Logs(i)
        Next
        textOutput.Text = resText
        '显示日志窗口
        textOutput.Margin = New Thickness(0) '隐藏下边栏
        If Len(textOutput.Text) > 60 Then
            textOutput.Text = textOutput.Text & vbCrLf & "[PCL] 如果你需要其它人的帮助，请将本窗口内的所有内容都发送给对方"
            If Not Me.ShowInTaskbar Then
                Me.Left = 200
                Me.ShowInTaskbar = True
                Me.WindowStyle = Windows.WindowStyle.SingleBorderWindow
                Me.Visibility = Visibility.Visible
            End If
        End If
        '输出 PCL 调试信息
        Dim CurrentRam As Integer = Int((New Devices.ComputerInfo).AvailablePhysicalMemory / 1024 / 1024)
        textOutput.Text = textOutput.Text &
            vbCrLf &
            vbCrLf & "Plain Craft Launcher 诊断信息" &
            vbCrLf & "——————————————————" &
            vbCrLf & "启动器版本：" & VERSION_NAME & "（" & VERSION_CODE & "）" &
            vbCrLf & "Java 路径：" & PATH_JAVA &
            vbCrLf & "Java 版本：" & GetFileVersion(PATH_JAVA & "\javaw.exe") &
            vbCrLf & "关闭 MC 后的内存剩余：" & CurrentRam & "M / " & Int((New Devices.ComputerInfo).TotalPhysicalMemory / 1024 / 1024) & "M" &
            vbCrLf & "分配的内存：" & ReadReg("LaunchMaxRam", "1024") & "M" &
            vbCrLf & "Minecraft 文件夹：" & PATH_MC &
            vbCrLf & "Minecraft 版本文件夹名：" & MinecraftVersion.Name & "（" & MinecraftVersion.Version & "）" &
            vbCrLf
        '输出错误处理建议
        Dim Advices As New ArrayList
        If textOutput.Text.Contains("Could not reserve enough space") Then Advices.Add("分配的内存过多，尝试在启动设置中减少分配的内存空间。")
        If CurrentRam < ReadReg("LaunchMaxRam", "1024") Then Advices.Add("电脑内存不足，尝试关闭一些程序再试。（剩余 " & CurrentRam & " M，需要至少 " & ReadReg("LaunchMaxRam", "1024") & " M）")
        '显示错误处理建议
        If Advices.Count > 0 Then
            textOutput.Text = textOutput.Text &
                vbCrLf & "错误处理建议" &
                vbCrLf & "——————————————————"
            For i As Integer = 1 To Advices.Count
                textOutput.Text = textOutput.Text & vbCrLf & If(Advices.Count = 1, "", i & ". ") & Advices(i - 1)
            Next
            textOutput.Text = textOutput.Text & vbCrLf
        End If
        '末处理
        Me.Title = "Minecraft 日志 - " & MinecraftVersion.Name & "（已崩溃）"
        'textOutput.ScrollToEnd()
        textOutput.SelectionStart = textOutput.Text.Length
        textOutput.SelectionLength = 0
        textOutput.Focus()
    End Sub

End Class
