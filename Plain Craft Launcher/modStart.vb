Imports Ionic.Zip

Module modStart

#Region "开始"

    Public Class GameStartInfo
        ''' <summary>
        ''' 登录方式。
        ''' </summary>
        Public LoginMethod As LoginMethods
        ''' <summary>
        ''' 玩家名。正版登录时给出返回的名称，离线登录时给出输入的名称。
        ''' </summary>
        Public UserName As String
        ''' <summary>
        ''' 启动的 Minecraft 版本。
        ''' </summary>
        Public SelectVersion As MCVersion
    End Class

    Public GameWindowState As LoadState = LoadState.Waiting
    ''' <summary>
    ''' 开始游戏。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub GameStart(Info As GameStartInfo)
        Try

            '初始化进度条（设置为 0 即为初始化）
            frmHomeRight.StartProcess = 0

            '检查输入信息、Java、内存。如果有问题会直接以 Exception 抛出
            frmHomeRight.StartProcess = 0.03
            log("[Launch] 正在进行启动前检查")
            frmHomeRight.StartButtomSet("检查基础信息中")
            GameCheck(Info)

            '检查 Lib 文件，有可能转入下载
            frmHomeRight.StartProcess = 0.06
            log("[Launch] 正在检查支持库")
            frmHomeRight.StartButtomSet("检查支持库中")
            Try
                GameLibStartCheck(Info)
            Catch ex As Exception
                If ex.Message.Contains("版本文件缺失") Then Throw ex
                Select Case MyMsgbox("支持库下载失败，是否要继续启动游戏？游戏很可能会启动失败。" & vbCrLf & "这通常是因为网络不稳定，或国内下载源尚未更新刚发布的 Minecraft 的文件。你可以选择取消启动，并且稍后重新尝试下载。", "支持库下载失败", "确定", "取消", , True)
                    Case 1 '确定
                        log("[Launch] 支持库下载失败，但是被强行启动")
                    Case 2 '取消
                        Throw New Exception("")
                End Select
            End Try

            '检查 Assets 文件，有可能转入下载
            frmHomeRight.StartProcess = 0.36
            If ReadIni("setup", "DownMinecraftAssetsHint", "True") = "True" Then
                log("[Launch] 正在检查资源文件")
                frmHomeRight.StartButtomSet("检查资源文件中")
                Try
                    GameAssetsStartCheck(Info)
                Catch ex As Exception
                    Select Case MyMsgbox("资源文件下载失败，是否要继续启动游戏？游戏可能没有中文，或是缺失部分声音。" & vbCrLf & "这通常是因为网络不稳定，或国内下载源尚未更新刚发布的 Minecraft 的文件。你可以选择取消启动，并且稍后重新尝试下载。", "资源文件下载失败", "确定", "取消", , True)
                        Case 1 '确定
                            log("[Launch] 资源文件下载失败，但是被强行启动")
                        Case 2 '取消
                            Throw New Exception("")
                    End Select
                End Try
            End If

            '加载启动信息
            frmHomeRight.StartProcess = 0.72
            frmHomeRight.StartButtomSet("加载启动信息中")
            Dim LaunchCMD As String = ""
            Select Case Info.LoginMethod
                Case LoginMethods.Mojang
                    '正版登录
                    log("[Launch] 获取正版登录启动参数")
                    LaunchCMD = GameArgumentsMojang(Info.SelectVersion)
                Case LoginMethods.Legacy
                    '离线登录
                    log("[Launch] 获取离线登录启动参数")
                    LaunchCMD = GameArgumentsLegacy(Info.SelectVersion, Info.UserName)
            End Select

            '启动游戏进程与日志监视窗口
            frmHomeRight.StartProcess = 0.82
            log("[Launch] 正在启动游戏进程")
            frmHomeRight.StartButtomSet("启动游戏进程中")
            GameThreadStart(LaunchCMD, Info)

            '等待游戏窗口
            frmHomeRight.StartProcess = 0.92
            log("[Launch] 正在等待游戏窗口")
            frmHomeRight.StartButtomSet("等待游戏窗口中")
            Do While GameWindowState = LoadState.Loading
                Thread.Sleep(15)
            Loop
            If GameWindowState = LoadState.Failed Then Throw New Exception("Minecraft 已崩溃")
            GameWindowState = LoadState.Waiting

            '启动后处理
            frmHomeRight.StartProcess = 0.96
            log("[Launch] 正在进行启动后处理")
            frmHomeRight.StartButtomSet("进行启动后处理中")
            GameThreadEnding()

            '启动结束，由动画触发结束事件
            frmHomeRight.StartProcess = 1
            log("[Launch] 启动结束")

        Catch ex As Exception

            If ex.Message = "" Then
                '空异常代表已知的取消
                frmHomeRight.StartProcess = -1
            Else
                '非空异常代表未知的启动失败
                frmHomeRight.StartProcess = -2
                If ex.Message.Contains("！") Or ex.Message.Contains("Minecraft 已崩溃") Then
                    '带有感叹号的是我写的错误信息
                    ShowHint(New HintConverter("游戏启动失败：" & ex.Message, HintState.Critical))
                Else
                    '不带有感叹号的是系统错误信息，直接利用 ExShow 返回
                    ExShow(ex, "游戏启动失败", ErrorLevel.MsgboxWithoutFeedback)
                End If
            End If

        End Try
    End Sub

#End Region

#Region "0.03-0.06 检查"

    Private Sub GameCheck(Info As GameStartInfo)

        '游戏名检测
        If Info.UserName = "" Or Info.UserName = "游戏名" Then
            Throw New FormatException("游戏名不能为空！")
        End If
        If Info.UserName.Contains("""") Then
            Throw New FormatException("游戏名不能包含引号！")
        End If

        'Java 检测
        If Not File.Exists(PATH_JAVA & "\javaw.exe") Then
            Select Case MyMsgbox("未找到 Java，请下载或人工查找！", "提示", "下载 Java", "人工查找", "取消")
                Case 1
                    If MODE_OFFLINE Then
                        '无网络连接
                        ShowHint(New HintConverter("没有网络连接，无法下载", HintState.Warn))
                    Else
                        '有网络连接
                        Process.Start("https://www.java.com/winoffline_installer/")
                        MyMsgbox("请在下载安装结束后重启电脑，再开始游戏。", "下载提示")
                    End If
                Case 2
                    '选择文件
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
                            GoTo JavaOK
                        End If
                    End Using
                    WriteReg("SetupJavaPath", PATH_JAVA)
                    log("[Launch] 人工指定的新的Java路径：" & PATH_JAVA)
                    Dim th As New Thread(AddressOf SetJavaEnvironment)
                    th.Priority = ThreadPriority.AboveNormal
                    th.Start()
                    Thread.Sleep(2000)
            End Select
            Throw New Exception("")
        Else
JavaOK:
            log("[Launch] Java路径：" & PATH_JAVA)
        End If

        'Java 环境变量检测
        If Not PathEnv.Contains(PATH_JAVA) Then
            frmHomeRight.StartButtomSet("设置环境变量中")
            SetJavaEnvironment()
            Thread.Sleep(2000)
            frmHomeRight.StartButtomSet("检查基础信息中")
        End If

        'Java 版本检测
        If Val(Mid(Info.SelectVersion.ReleaseTime, 1, 4)) >= 2017 Then
            Dim JavaVersion As String = GetFileVersion(PATH_JAVA & "\javaw.exe")
            log("[Launch] Java 版本：" & JavaVersion)
            '版本不足
            If Val(Mid(JavaVersion, 1, 1)) < 8 Then
                If MyMsgbox("自 Minecraft 1.12 起，Minecraft 必须要 Java 8 或者更高版本才能启动，请在官网下载安装最新版的 Java！", "提示", "下载Java", "取消") = 1 Then
                    If MODE_OFFLINE Then
                        '无网络连接
                        ShowHint(New HintConverter("没有网络连接，无法下载", HintState.Warn))
                    Else
                        '有网络连接
                        Process.Start("https://www.java.com/winoffline_installer/")
                        MyMsgbox("请在下载安装结束后重启电脑，然后在 启动设置 的 Java 路径 中填写新版 Java 的路径，再开始游戏。", "下载提示")
                    End If
                End If
                Throw New Exception("") '抛出空异常时不会弹窗
            End If
        Else
            log("[Launch] 不需要强制使用 Java 8 启动，跳过版本检查")
        End If

    End Sub

#End Region

#Region "0.06-0.36 支持库检查"

    Private Structure GameLibFile
        ''' <summary>
        ''' 文件的本地完整路径。
        ''' </summary>
        ''' <remarks></remarks>
        Public LocalPath As String
        ''' <summary>
        ''' 文件大小。
        ''' </summary>
        ''' <remarks></remarks>
        Public Size As FileSize
        ''' <summary>
        ''' 是否为Natives文件。
        ''' </summary>
        ''' <remarks></remarks>
        Public IsNatives As Boolean

        Public Overrides Function ToString() As String
            Return If(IsNatives, "[Native] ", "") & Size.ToString & " | " & LocalPath
        End Function
    End Structure

    Private Sub GameLibStartCheck(Info As GameStartInfo)
        Dim FileList As New ArrayList
        If Not GameLibCheck(Info.SelectVersion, FileList) Then
            'FileList 在调用 GameLibCheck 时用 ByRef 获取
            frmHomeRight.StartButtomSet("下载支持库中")
            GameLibDownload(Info.SelectVersion.Name, FileList)
        End If
    End Sub

    ''' <summary>
    ''' 获取 Minecraft 某一版本的完整支持库列表。（包含引用版本等）
    ''' </summary>
    Private Function GameLibListGet(Version As MCVersion) As ArrayList

        '初步获取引用表
        log("[Launch] 开始获取引用表")
        GameLibListGet = GameLibListGetByJson(Version.Json("libraries"))

        '处理继承版本
        If Version.InheritVersion = "" Then
            '没有继承，添加当前版本jar作为引用
            log("[Launch] 没有继承版本")
            GameLibListGet.Add(New GameLibFile With {.LocalPath = Version.Path & Version.Name & ".jar", .Size = GetFileSize(Version.Path & Version.Name & ".jar"), .IsNatives = False})
        Else
            '读取Json
            log("[Launch] 读取继承版本Json：" & PATH_MC & "versions\" & Version.InheritVersion & "\" & Version.InheritVersion & ".json")
            Dim InheritVersionJson As JObject = Nothing
            For Each Encode As Encoding In {Encoding.Default, New UTF8Encoding(False), Encoding.Unicode, Encoding.ASCII}
                Try
                    InheritVersionJson = CType(Newtonsoft.Json.JsonConvert.DeserializeObject(ReadFileToEnd(PATH_MC & "versions\" & Version.InheritVersion & "\" & Version.InheritVersion & ".json", Encode)), JObject)
                    GoTo InheritJsonReadFinish
                Catch
                    '编码错误
                    InheritVersionJson = Nothing
                End Try
            Next
            '各种编码都没戏了
            Throw New FileFormatException("Json文件读取失败：" & PATH_MC & "versions\" & Version.InheritVersion & "\" & Version.InheritVersion & ".json")
InheritJsonReadFinish:
            log("[Launch] 获取继承版本引用表")
            '获取继承版本中的Json文件
            GameLibListGet.AddRange(GameLibListGetByJson(InheritVersionJson("libraries")))
            '添加继承的Jar
            GameLibListGet.Add(New GameLibFile With {.LocalPath = PATH_MC & "versions\" & Version.Json("inheritsFrom").ToString & "\" & Version.Json("inheritsFrom").ToString & ".jar",
                                          .Size = GetFileSize(PATH_MC & "versions\" & Version.Json("inheritsFrom").ToString & "\" & Version.Json("inheritsFrom").ToString & ".jar"), .IsNatives = False})
        End If

    End Function

    ''' <summary>
    ''' 获取 Minecraft 某一版本的支持库列表。
    ''' </summary>
    ''' <param name="LibJson">版本 json 中的 libraries 项。</param>
    ''' <returns></returns>
    Private Function GameLibListGetByJson(ByVal LibJson As JToken) As ArrayList
        GameLibListGetByJson = New ArrayList
        For Each Library As JToken In LibJson

            '检查是否需要（Rules）
            If Not GameRuleCheck(Library("rules")) Then GoTo NextFile

            '获取名称（Name）
            Dim SourceArray() As String
            SourceArray = Library("name").ToString.Split(":")

            '根据是否本地化处理（Natives）
            If IsNothing(Library("natives")) Then
                '没有Natives
                Dim Size As Integer = 0
                Try
                    If IsNothing(Library("downloads")) Then
                        Size = 0
                    Else
                        Size = Val(Library("downloads")("artifact")("size").ToString)
                    End If
                Catch
                    Size = 0
                End Try
                '添加
                GameLibListGetByJson.Add(New GameLibFile With {.LocalPath =
                    PATH_MC & "libraries\" &
                                  SourceArray(0).Replace(".", "\") & "\" &
                                  SourceArray(1) & "\" &
                                  SourceArray(2) & "\" &
                                  SourceArray(1) & "-" & SourceArray(2) & ".jar", .Size = Size, .IsNatives = False})
            Else
                '有natives
                If Not IsNothing(Library("natives")("windows")) Then
                    Dim Size As Integer = 0
                    Try
                        If IsNothing(Library("downloads")) Then
                            Size = 0
                        Else
                            Size = Val(Library("downloads")("classifiers")("natives-windows")("size").ToString)
                        End If
                    Catch
                        Size = 0
                    End Try
                    GameLibListGetByJson.Add(New GameLibFile With {.LocalPath =
                                    (PATH_MC & "libraries\" &
                                      SourceArray(0).Replace(".", "\") & "\" &
                                      SourceArray(1) & "\" &
                                      SourceArray(2) & "\" &
                                      SourceArray(1) & "-" & SourceArray(2) & "-" & Library("natives")("windows").ToString & ".jar").Replace("${arch}", If(Environment.Is64BitOperatingSystem, "64", "32")), .Size = Size, .IsNatives = True})
                End If
            End If

NextFile:
        Next
    End Function

    ''' <summary>
    ''' 检查 Minecraft 的某一版本的支持库是否存在。
    ''' </summary>
    Private Function GameLibCheck(ByVal Version As MCVersion, ByRef FileList As ArrayList)
        Try

            '检查每个文件的大小是否合乎要求
            FileList = GameLibListGet(Version)
            For i As Integer = FileList.Count - 1 To 0 Step -1
                If FileList(i).Size = 0 Then
                    '不包含大小信息
                    If Not File.Exists(FileList(i).LocalPath) Then
                        log("[Launch] 支持库有误：" & FileList(i).LocalPath)
                        If FileList(i).LocalPath.Contains(PATH_MC & "versions\") Then
                            Throw New Exception("版本文件缺失：" & FileList(i).LocalPath & "，请尝试重新下载安装该版本。")
                        End If
                        Return False
                    End If
                Else
                    '包含大小信息
                    If Not GetFileSize(FileList(i).LocalPath) = FileList(i).Size Then
                        log("[Launch] 支持库有误：" & FileList(i).LocalPath)
                        If FileList(i).LocalPath.Contains(PATH_MC & "versions\") Then
                            Throw New Exception("版本文件缺失：" & FileList(i).LocalPath & "，请尝试重新下载安装该版本。")
                        End If
                        Return False
                    End If
                End If
            Next

        Catch ex As Exception
            If ex.Message.Contains("版本文件缺失") Then Throw ex
            ExShow(ex, "检查支持库完整性出错", ErrorLevel.Slient)
            Return False
        End Try

        '没有出错就返回 True
        Return True
    End Function

    Private GameLibIndexDownloading As LoadState = LoadState.Waiting
    ''' <summary>
    ''' 下载 Minecraft 的支持库。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub GameLibDownload(ByVal VersionName As String, FileList As ArrayList)
        log("[Launch] 下载支持库：" & VersionName)

        GameLibIndexDownloading = LoadState.Loading
        Dim Source As New ArrayList
        For Each File As GameLibFile In FileList
            If File.LocalPath.Contains("minecraftforge") Then
                If ReadIni("setup", "DownForge", "1") = "0" Then '官方源优先
                    Source.Add(New WebRequireFile With {
                                .WebURLs = New ArrayList From {
                                    "http://files.minecraftforge.net/maven" & File.LocalPath.Replace(PATH_MC & "libraries", "").Replace("\", "/").Replace(".jar", "-universal.jar"),
                                    "http://bmclapi2.bangbang93.com/maven" & File.LocalPath.Replace(PATH_MC & "libraries", "").Replace("\", "/").Replace(".jar", "-universal.jar"),
                                    "http://bmclapi2.bangbang93.com/libraries" & File.LocalPath.Replace(PATH_MC & "libraries", "").Replace("\", "/")
                                },
                                .LocalFolder = GetPathFromFullPath(File.LocalPath), .LocalName = GetFileNameFromPath(File.LocalPath),
                                .KnownFileSize = File.Size
                            })
                Else
                    'BMCLAPI 优先
                    Source.Add(New WebRequireFile With {
                                .WebURLs = New ArrayList From {
                                    "http://bmclapi2.bangbang93.com/maven" & File.LocalPath.Replace(PATH_MC & "libraries", "").Replace("\", "/").Replace(".jar", "-universal.jar"),
                                    "http://bmclapi2.bangbang93.com/libraries" & File.LocalPath.Replace(PATH_MC & "libraries", "").Replace("\", "/"),
                                    "http://files.minecraftforge.net/maven" & File.LocalPath.Replace(PATH_MC & "libraries", "").Replace("\", "/").Replace(".jar", "-universal.jar")
                                },
                                .LocalFolder = GetPathFromFullPath(File.LocalPath), .LocalName = GetFileNameFromPath(File.LocalPath),
                                .KnownFileSize = File.Size
                            })
                End If
            ElseIf File.LocalPath.Contains("optifine\OptiFine") Then
                Source.Add(New WebRequireFile With {
                            .WebURLs = New ArrayList From {
                                "http://bmclapi2.bangbang93.com/maven/com/optifine/" & File.LocalPath.Replace(PATH_MC & "libraries\optifine\OptiFine\", "").Split("_")(0) & "/" & GetFileNameFromPath(File.LocalPath).Replace("-", "_")
                            },
                            .LocalFolder = GetPathFromFullPath(File.LocalPath), .LocalName = GetFileNameFromPath(File.LocalPath),
                            .KnownFileSize = File.Size
                        })
            Else
                If ReadIni("setup", "DownMinecraft", "1") = "0" Then
                    '官方源优先
                    Source.Add(New WebRequireFile With {
                                .WebURLs = New ArrayList From {
                                    "https://libraries.minecraft.net" & File.LocalPath.Replace(PATH_MC & "libraries", "").Replace("\", "/"),
                                    "http://bmclapi2.bangbang93.com/libraries" & File.LocalPath.Replace(PATH_MC & "libraries", "").Replace("\", "/"),
                                    "https://libraries.minecraft.net" & File.LocalPath.Replace(PATH_MC & "libraries", "").Replace("\", "/"),
                                    "http://bmclapi2.bangbang93.com/libraries" & File.LocalPath.Replace(PATH_MC & "libraries", "").Replace("\", "/"),
                                    "http://bmclapi2.bangbang93.com/maven" & File.LocalPath.Replace(PATH_MC & "libraries", "").Replace("\", "/")
                                },
                                .LocalFolder = GetPathFromFullPath(File.LocalPath), .LocalName = GetFileNameFromPath(File.LocalPath),
                                .KnownFileSize = File.Size
                            })
                Else
                    'BMCLAPI 优先
                    Source.Add(New WebRequireFile With {
                                .WebURLs = New ArrayList From {
                                    "http://bmclapi2.bangbang93.com/libraries" & File.LocalPath.Replace(PATH_MC & "libraries", "").Replace("\", "/"),
                                    "https://libraries.minecraft.net" & File.LocalPath.Replace(PATH_MC & "libraries", "").Replace("\", "/"),
                                    "http://bmclapi2.bangbang93.com/libraries" & File.LocalPath.Replace(PATH_MC & "libraries", "").Replace("\", "/"),
                                    "https://libraries.minecraft.net" & File.LocalPath.Replace(PATH_MC & "libraries", "").Replace("\", "/"),
                                    "http://bmclapi2.bangbang93.com/maven" & File.LocalPath.Replace(PATH_MC & "libraries", "").Replace("\", "/")
                                },
                                .LocalFolder = GetPathFromFullPath(File.LocalPath), .LocalName = GetFileNameFromPath(File.LocalPath),
                                .KnownFileSize = File.Size
                            })
                End If
            End If
        Next
        WebStart(ArrayConventer(Of WebRequireFile, ArrayList)(Source),
                 Name:=VersionName & " 支持库",
                 OnSuccess:=Sub() GameLibIndexDownloading = LoadState.Loaded,
                 OnFail:=Sub() GameLibIndexDownloading = LoadState.Failed,
                 RequireSize:=WebRequireSize.AtLeast)
        '等待下载结束
        Do While GameLibIndexDownloading = LoadState.Loading
            Try
                Thread.Sleep(250)
                If WebGroups.Keys.Contains(VersionName & " 支持库") Then
                    Dim Percent As Double = WebGroups(VersionName & " 支持库").Percent
                    frmHomeRight.StartButtomSet("下载支持库中 " & Math.Round(Percent * 100, 1) & "%")
                    frmHomeRight.StartProcess = 0.09 + MathRange(Percent, 0, 1) * 0.27
                End If
            Catch
            End Try
        Loop
        '如果下载失败则返回
        If GameLibIndexDownloading = LoadState.Failed Then Throw New Exception("下载支持库失败！")

    End Sub

#End Region

#Region "0.36-0.72 资源文件检查"

    Private Structure GameAssetsFile
        Public IsVirtual As Boolean
        ''' <summary>
        ''' 本地文件的完整路径，包含文件名。
        ''' </summary>
        Public LocalPath As String
        Public Hash As String
        ''' <summary>
        ''' Json 中书写的源路径。例如 minecraft/sounds/mob/stray/death2.ogg 。
        ''' </summary>
        Public SourcePath As String
        Public Size As Integer
    End Structure

    Public Sub GameAssetsStartCheck(Info As GameStartInfo)
        If Info.SelectVersion.Assets = "" Then
            log("[Launch] 不存在资源文件项，可能是由于 Json 不标准")
        Else
            log("[Launch] 资源文件：" & Info.SelectVersion.Assets)
            If Not GameAssetsCheck(Info.SelectVersion.Assets) Then
                If ReadIni("setup", "DownMinecraftAssetsAuto", "True") = "True" Then
                    GoTo StartDownload
                ElseIf MyMsgbox("Minecraft 资源文件（" & Info.SelectVersion.Assets & "）未下载。" & vbCrLf & "若不下载，游戏可能没有中文与声音，是否立即下载？" & vbCrLf & "你可以在设置中关闭此提示。", "缺少资源文件", "下载", "取消") = 1 Then
StartDownload:
                    frmHomeRight.StartProcess = 0.39
                    frmHomeRight.StartButtomSet("下载资源文件中")
                    GameAssetsDownload(Info.SelectVersion)
                End If
            End If
        End If
    End Sub

    ''' <summary>
    ''' 获取 Minecraft 的资源文件列表。
    ''' </summary>
    ''' <param name="Name">版本的资源名称。</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GameAssetsListGet(ByVal Name As String) As ArrayList
        Try

            '初始化
            If Not File.Exists(PATH_MC & "assets\indexes\" & Name & ".json") Then Throw New FileNotFoundException("索引文件未找到", PATH_MC & "assets\indexes\" & Name & ".json")
            GameAssetsListGet = New ArrayList
            Dim Json = ReadJson(ReadFileToEnd(PATH_MC & "assets\indexes\" & Name & ".json"))

            '确认Virtual
            Dim IsVirtual As Boolean = False
            If Not IsNothing(Json("virtual")) Then IsVirtual = Json("virtual").ToString

            '加载列表
            If IsVirtual Then
                For Each File As JProperty In Json("objects").Children
                    GameAssetsListGet.Add(New GameAssetsFile With {
                                        .IsVirtual = True,
                                        .LocalPath = PATH_MC & "assets\virtual\" & Name & "\" & File.Name.Replace("/", "\"),
                                        .SourcePath = File.Name,
                                        .Hash = File.Value("hash").ToString,
                                        .Size = File.Value("size").ToString
                                    })
                Next
            Else
                For Each File As JProperty In Json("objects").Children
                    GameAssetsListGet.Add(New GameAssetsFile With {
                                        .IsVirtual = False,
                                        .LocalPath = PATH_MC & "assets\objects\" & Left(File.Value("hash").ToString, 2) & "\" & File.Value("hash").ToString,
                                        .SourcePath = File.Name,
                                        .Hash = File.Value("hash").ToString,
                                        .Size = File.Value("size").ToString
                                    })
                Next
            End If

        Catch ex As Exception
            ExShow(ex, "获取资源文件列表失败：" & Name)
            Throw
        End Try
    End Function

    ''' <summary>
    ''' 检查 Minecraft 的某一版本的资源文件是否存在。
    ''' </summary>
    ''' <param name="Name">版本的资源名称。</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GameAssetsCheck(ByVal Name As String)
        Try

            '检查 Json 是否存在
            If Not File.Exists(PATH_MC & "assets\indexes\" & Name & ".json") Then Return False

            '检查每个文件的大小是否合乎要求
            Dim FileList As ArrayList = GameAssetsListGet(Name)
            For i As Integer = FileList.Count - 1 To 0 Step -1
                If Not GetFileSize(FileList(i).LocalPath) = FileList(i).Size Then
                    log("[System] 资源文件有误：" & FileList(i).LocalPath)
                    Return False
                End If
            Next

        Catch ex As Exception
            ExShow(ex, "检查资源文件完整性出错：" & Name, ErrorLevel.Slient)
            Return False
        End Try

        '没有出错就返回 True
        Return True
    End Function

    Private GameAssetsIndexDownloading As LoadState = LoadState.Waiting
    ''' <summary>
    ''' 下载 Minecraft 的资源文件。
    ''' </summary>
    ''' <param name="Version">版本信息。</param>
    ''' <remarks></remarks>
    Public Sub GameAssetsDownload(ByVal Version As MCVersion)
        log("[Launch] 下载资源文件：" & Version.Assets)
        Directory.CreateDirectory(PATH_MC & "assets\indexes\")

        '获取资源文件索引下载地址及文件大小，输出为 AssetsAddress 与 AssetsSize

        Dim AssetsAddress As String = ""
        Dim AssetsSize As Integer = 0
        Try
            '直接获取
            Dim URLKey As JToken = Version.Json("assetIndex")
            If Not IsNothing(URLKey) Then
                AssetsAddress = URLKey("url").ToString
                AssetsSize = URLKey("size").ToString
                GoTo FinishJson
            End If
            '从继承版本获取
            If Not Version.InheritVersion = "" Then
                Dim InheritVersion = New MCVersion(Version.InheritVersion)
                URLKey = InheritVersion.Json("assetIndex")
                If Not IsNothing(URLKey) Then
                    AssetsAddress = URLKey("url").ToString
                    AssetsSize = URLKey("size").ToString
                    GoTo FinishJson
                End If
            End If
        Catch ex As Exception
            log("[Launch] 获取资源文件索引下载地址失败：" & GetStringFromException(ex))
        End Try
FinishJson:
        If AssetsAddress = "" Then
            AssetsAddress = "http://s3.anazonaws.com/Minecraft.Download/indexes/" & Version.Assets & ".json"
            log("[Launch] 无法获取资源文件索引下载地址，使用旧版本下载地址：" & AssetsAddress)
        Else
            log("[Launch] 资源文件索引下载地址：" & AssetsAddress)
        End If

        '下载资源文件索引

        GameAssetsIndexDownloading = LoadState.Loading
        WebStart({New WebRequireFile With {
                         .WebURLs = New ArrayList From {
                             "http://bmclapi2.bangbang93.com/indexes/" & Version.Assets & ".json",
                             AssetsAddress.Replace("https://launchermeta.mojang.com", "http://bmclapi2.bangbang93.com"),
                             AssetsAddress,
                             "http://s3.anazonaws.com/Minecraft.Download/indexes/" & Version.Assets & ".json"},
                         .LocalFolder = PATH_MC & "assets\indexes\", .LocalName = Version.Assets & ".json",
                         .KnownFileSize = AssetsSize
                 }},
                 Name:=Version.Assets & " 资源文件信息",
                 OnSuccess:=Sub() GameAssetsIndexDownloading = LoadState.Loaded,
                 OnFail:=Sub() GameAssetsIndexDownloading = LoadState.Failed,
                 RequireSize:=If(AssetsSize = 0, WebRequireSize.Require, WebRequireSize.Known))
        '等待下载结束
        Do While GameAssetsIndexDownloading = LoadState.Loading
            Thread.Sleep(50)
        Loop
        '如果下载失败则返回
        If GameAssetsIndexDownloading = LoadState.Failed Then Throw New Exception("下载资源文件索引失败！")

        '下载资源文件

        GameAssetsIndexDownloading = LoadState.Loading
        Dim FileList As ArrayList = GameAssetsListGet(Version.Assets)
        Dim KVList As New ArrayList
        For Each AssetFile As GameAssetsFile In FileList
            If ReadIni("setup", "DownAssets", "0") = "0" Then
                KVList.Add(New WebRequireFile With {
                           .LocalFolder = GetPathFromFullPath(AssetFile.LocalPath), .LocalName = GetFileNameFromPath(AssetFile.LocalPath),
                           .WebURLs = New ArrayList From {
                               "http://resources.download.minecraft.net/" & Left(AssetFile.Hash, 2) & "/" & AssetFile.Hash,
                               "http://bmclapi2.bangbang93.com/assets/" & Left(AssetFile.Hash, 2) & "/" & AssetFile.Hash,
                               "http://resources.download.minecraft.net/" & Left(AssetFile.Hash, 2) & "/" & AssetFile.Hash,
                               "http://bmclapi2.bangbang93.com/assets/" & Left(AssetFile.Hash, 2) & "/" & AssetFile.Hash,
                               "http://resources.download.minecraft.net/" & Left(AssetFile.Hash, 2) & "/" & AssetFile.Hash,
                               "http://bmclapi2.bangbang93.com/assets/" & Left(AssetFile.Hash, 2) & "/" & AssetFile.Hash,
                               "http://resources.download.minecraft.net/" & Left(AssetFile.Hash, 2) & "/" & AssetFile.Hash,
                               "http://bmclapi2.bangbang93.com/assets/" & Left(AssetFile.Hash, 2) & "/" & AssetFile.Hash,
                               "http://resources.download.minecraft.net/" & Left(AssetFile.Hash, 2) & "/" & AssetFile.Hash,
                               "http://bmclapi2.bangbang93.com/resources/" & AssetFile.SourcePath},
                            .KnownFileSize = AssetFile.Size
                       })
            Else
                KVList.Add(New WebRequireFile With {
                           .LocalFolder = GetPathFromFullPath(AssetFile.LocalPath), .LocalName = GetFileNameFromPath(AssetFile.LocalPath),
                           .WebURLs = New ArrayList From {
                               "http://bmclapi2.bangbang93.com/assets/" & Left(AssetFile.Hash, 2) & "/" & AssetFile.Hash,
                               "http://resources.download.minecraft.net/" & Left(AssetFile.Hash, 2) & "/" & AssetFile.Hash,
                               "http://bmclapi2.bangbang93.com/assets/" & Left(AssetFile.Hash, 2) & "/" & AssetFile.Hash,
                               "http://resources.download.minecraft.net/" & Left(AssetFile.Hash, 2) & "/" & AssetFile.Hash,
                               "http://bmclapi2.bangbang93.com/assets/" & Left(AssetFile.Hash, 2) & "/" & AssetFile.Hash,
                               "http://resources.download.minecraft.net/" & Left(AssetFile.Hash, 2) & "/" & AssetFile.Hash,
                               "http://bmclapi2.bangbang93.com/assets/" & Left(AssetFile.Hash, 2) & "/" & AssetFile.Hash,
                               "http://resources.download.minecraft.net/" & Left(AssetFile.Hash, 2) & "/" & AssetFile.Hash,
                               "http://bmclapi2.bangbang93.com/resources/" & AssetFile.SourcePath},
                            .KnownFileSize = AssetFile.Size
                       })
            End If
        Next
        WebStart(ArrayConventer(Of WebRequireFile, ArrayList)(KVList),
                 Name:=Version.Assets & " 资源文件",
                 OnSuccess:=Sub() GameAssetsIndexDownloading = LoadState.Loaded,
                 OnFail:=Sub() GameAssetsIndexDownloading = LoadState.Failed,
                 RequireSize:=WebRequireSize.Known)
        '等待下载结束
        Do While GameAssetsIndexDownloading = LoadState.Loading
            Try
                Thread.Sleep(250)
                If WebGroups.Keys.Contains(Version.Assets & " 资源文件") Then
                    Dim Percent As Double = WebGroups(Version.Assets & " 资源文件").Percent
                    frmHomeRight.StartButtomSet("下载资源文件中 " & Math.Round(Percent * 100, 1) & "%")
                    frmHomeRight.StartProcess = 0.42 + MathRange(Percent, 0, 1) * 0.3
                End If
            Catch
            End Try
        Loop
        '如果下载失败则返回
        If GameAssetsIndexDownloading = LoadState.Failed Then Throw New Exception("下载资源文件失败！")

    End Sub

#End Region

#Region "0.72-0.82 基础参数"

    Dim GameArguments As New Dictionary(Of String, String)

    ''' <summary>
    ''' 获取 Minecraft 启动所需的 JVM 字符串（第一段）。这个函数会抛出 Exception。
    ''' </summary>
    Private Function GameArgumentsJVM(ByRef Version As MCVersion) As String
        If IsNothing(Version.Json("arguments")) Then
            '老版
            Return GameArgumentsJVMOld(Version)
        Else
            '新版
            Return GameArgumentsJVMNew(Version)
        End If
    End Function
    ''' <summary>
    ''' 获取老版 Minecraft JVM 启动参数。
    ''' </summary>
    Private Function GameArgumentsJVMOld(ByRef Version As MCVersion) As String

        '存储以空格为间隔的启动参数列表
        Dim DataList As New ArrayList

        '输出固定参数
        Dim JVMArguments As String = ReadIni("setup", "LaunchJVM", "-XX:+UseG1GC -XX:-UseAdaptiveSizePolicy -XX:-OmitStackTraceInFastThrow -Dfml.ignoreInvalidMinecraftCertificates=True -Dfml.ignorePatchDiscrepancies=True")
        DataList.Add("-XX:HeapDumpPath=MojangTricksIntelDriversForPerformance_javaw.exe_minecraft.exe.heapdump")
        DataList.Add(JVMArguments)
        DataList.Add("-Xmn128m")
        DataList.Add("-Xmx" & ReadReg("LaunchMaxRam", "1024") & "m")
        DataList.Add("""-Djava.library.path=" & Version.Path & Version.Name & "-natives""")

        '获取支持库列表
        Dim LibList As ArrayList = GameLibListGet(Version)
        Dim CPString As String = "-cp """
        For Each Library As GameLibFile In LibList
            If Not Library.IsNatives Then CPString = CPString & Library.LocalPath & ";"
        Next

        '把支持库添加进启动参数表
        DataList.Add(Mid(CPString, 1, Len(CPString) - 1) & """")

        '解压Natives
        frmHomeRight.StartButtomSet("解压文件中")
        frmHomeRight.StartProcess = 0.74

        '删除原文件
        If ReadIni("setup", "LaunchNatives", "True") = "False" Then
            log("[Launch] Natives解压根据设置跳过")
            GoTo NoUnrar
        End If
        log("[Launch] 删除原Natives")
        Directory.CreateDirectory(Version.Path & Version.Name & "-natives")
        For Each FileName As String In IO.Directory.GetFiles(Version.Path & Version.Name & "-natives")
            Try
                File.Delete(FileName)
            Catch ex As Exception
                log("[System] 删除原dll访问被拒绝，这通常代表有一个MC正在运行，跳过解压步骤")
                log("[System] 实际的错误信息：" & GetStringFromException(ex))
                GoTo NoUnrar
            End Try
        Next
        frmHomeRight.StartProcess = 0.77

        '解压文件
        log("[Launch] 解压新Natives")
        For Each Native As GameLibFile In LibList
            If Not Native.IsNatives Then GoTo NextFile
            Try
                Using zip As New ZipFile(Native.LocalPath)
                    For Each File As ZipEntry In zip
                        If File.FileName.Contains("dll") Then
                            IO.File.Delete(Version.Path & Version.Name & "-natives\" & File.FileName)
                            zip.ExtractSelectedEntries(File.FileName, "", Version.Path & Version.Name & "-natives")
                        End If
                    Next
                    log("[Launch] 解压的Native文件：" & Native.LocalPath)
                End Using
            Catch ex As Exception
                ExShow(ex, "解压文件失败")
                If MyMsgbox("解压文件失败，是否要继续启动游戏？" & vbCrLf & vbCrLf & "详细的错误信息：" & GetStringFromException(ex, True), "启动游戏异常", "确定", "取消", , True) = 2 Then Throw
            End Try
NextFile:
        Next
NoUnrar:

        '添加 MainClass
        DataList.Add(Version.Json("mainClass").ToString)

        Return Join(DataList.ToArray)
    End Function
    ''' <summary>
    ''' 获取新版 Minecraft JVM 启动参数。
    ''' </summary>
    Private Function GameArgumentsJVMNew(ByRef Version As MCVersion) As String

        '加载可用参数
        GameArguments.Add("${natives_directory}", Version.Path & Version.Name & "-natives")
        GameArguments.Add("${launcher_name}", APPLICATION_FULL_NAME)
        GameArguments.Add("${launcher_version}", VERSION_CODE)

        '获取支持库列表
        Dim LibList As ArrayList = GameLibListGet(Version)
        Dim CPString As String = ""
        For Each Library As GameLibFile In LibList
            If Not Library.IsNatives Then CPString = CPString & Library.LocalPath & ";"
        Next
        GameArguments.Add("${classpath}", Mid(CPString, 1, Len(CPString) - 1))

        '解压Natives
        frmHomeRight.StartButtomSet("解压文件中")
        frmHomeRight.StartProcess = 0.74

        '删除原文件
        If ReadIni("setup", "LaunchNatives", "True") = "False" Then
            log("[Launch] Natives解压根据设置跳过")
            GoTo NoUnrar
        End If
        log("[Launch] 删除原Natives")
        Directory.CreateDirectory(Version.Path & Version.Name & "-natives")
        For Each FileName As String In IO.Directory.GetFiles(Version.Path & Version.Name & "-natives")
            Try
                File.Delete(FileName)
            Catch ex As Exception
                log("[System] 删除原dll访问被拒绝，这通常代表有一个MC正在运行，跳过解压步骤")
                log("[System] 实际的错误信息：" & GetStringFromException(ex))
                GoTo NoUnrar
            End Try
        Next
        frmHomeRight.StartProcess = 0.77

        '解压文件
        log("[Launch] 解压新Natives")
        For Each Native As GameLibFile In LibList
            If Not Native.IsNatives Then GoTo NextFile
            Try
                Using zip As New ZipFile(Native.LocalPath)
                    For Each File As ZipEntry In zip
                        If File.FileName.Contains("dll") Then
                            IO.File.Delete(Version.Path & Version.Name & "-natives\" & File.FileName)
                            zip.ExtractSelectedEntries(File.FileName, "", Version.Path & Version.Name & "-natives")
                        End If
                    Next
                    log("[Launch] 解压的Native文件：" & Native.LocalPath)
                End Using
            Catch ex As Exception
                ExShow(ex, "解压文件失败")
                If MyMsgbox("解压文件失败，是否要继续启动游戏？" & vbCrLf & vbCrLf & "详细的错误信息：" & GetStringFromException(ex, True), "启动游戏异常", "确定", "取消", , True) = 2 Then Throw
            End Try
NextFile:
        Next
NoUnrar:

        '处理 Json
        Dim DataList As New ArrayList
        If Version.Json("arguments")("jvm") IsNot Nothing Then
            For Each SubJson As JToken In Version.Json("arguments")("jvm")
                If SubJson.Type = JTokenType.String Then
                    '字符串类型
                    DataList.Add(SubJson.ToString)
                Else
                    '非字符串类型
                    If GameRuleCheck(SubJson("rules")) Then
                        '满足准则
                        If SubJson("value").Type = JTokenType.String Then
                            DataList.Add(SubJson("value").ToString)
                        Else
                            For Each value As JToken In SubJson("value")
                                DataList.Add(value.ToString)
                            Next
                        End If
                    End If
                End If
            Next
        End If
        If Version.InheritVersion <> "" Then
            '读取Json
            Log("[Launch] 读取继承版本Json：" & PATH_MC & "versions\" & Version.InheritVersion & "\" & Version.InheritVersion & ".json")
            Dim InheritVersionJson As JObject = Nothing
            For Each Encode As Encoding In {Encoding.Default, New UTF8Encoding(False), Encoding.Unicode, Encoding.ASCII}
                Try
                    InheritVersionJson = CType(Newtonsoft.Json.JsonConvert.DeserializeObject(ReadFileToEnd(PATH_MC & "versions\" & Version.InheritVersion & "\" & Version.InheritVersion & ".json", Encode)), JObject)
                    GoTo InheritJsonReadFinish
                Catch
                    '编码错误
                    InheritVersionJson = Nothing
                End Try
            Next
            '各种编码都没戏了
            Throw New FileFormatException("Json文件读取失败：" & PATH_MC & "versions\" & Version.InheritVersion & "\" & Version.InheritVersion & ".json")
InheritJsonReadFinish:
            If InheritVersionJson("arguments")("jvm") IsNot Nothing Then
                For Each SubJson As JToken In InheritVersionJson("arguments")("jvm")
                    If SubJson.Type = JTokenType.String Then
                        '字符串类型
                        DataList.Add(SubJson.ToString)
                    Else
                        '非字符串类型
                        If GameRuleCheck(SubJson("rules")) Then
                            '满足准则
                            If SubJson("value").Type = JTokenType.String Then
                                DataList.Add(SubJson("value").ToString)
                            Else
                                For Each value As JToken In SubJson("value")
                                    DataList.Add(value.ToString)
                                Next
                            End If
                        End If
                    End If
                Next
            End If
        End If

        '输出固定参数
        DataList.Add(ReadIni("setup", "LaunchJVM", "-XX:+UseG1GC -XX:-UseAdaptiveSizePolicy -XX:-OmitStackTraceInFastThrow -Dfml.ignoreInvalidMinecraftCertificates=True -Dfml.ignorePatchDiscrepancies=True"))
        DataList.Add("-Xmn128m")
        DataList.Add("-Xmx" & ReadReg("LaunchMaxRam", "1024") & "m")

        '添加 MainClass
        DataList.Add(Version.Json("mainClass").ToString)

        Return Join(DataList.ToArray)
    End Function

    ''' <summary>
    ''' 获取 Minecraft 启动所需的 Game 字符串（第二段）。若是盗版，才传入 UserName。
    ''' </summary>
    ''' <param name="Version"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GameArgumentsGame(ByRef Version As MCVersion) As String
        If IsNothing(Version.Json("arguments")) Then
            '老版
            Return GameArgumentsGameOld(Version)
        Else
            '新版
            Return GameArgumentsGameNew(Version)
        End If
    End Function
    ''' <summary>
    ''' 获取老版 Minecraft Game 启动参数。
    ''' </summary>
    ''' <param name="Version"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GameArgumentsGameOld(ByRef Version As MCVersion) As String
        Dim DataList = New ArrayList

        '本地化 Minecraft 启动信息
        log("[Launch] 处理基准参数字符串")
        Dim BasicString As String = Version.Json("minecraftArguments").ToString & " --height ${resolution_height} --width ${resolution_width}"
        DataList.Add(BasicString)

        '添加服务器信息
        Dim Server As String = ReadIni("setup", "LaunchServer", "")
        If Server.Length > 0 Then
            If Server.Contains(":") Then
                '包含端口号
                DataList.Add("--server " & Server.Split(":")(0))
                DataList.Add("--port " & Server.Split(":")(1))
            Else
                '不包含端口号
                DataList.Add("--server " & Server)
                DataList.Add("--port 25565")
            End If
        End If

        '添加全屏
        If ReadIni("setup", "LaunchMode", "0") = "1" Then DataList.Add("--fullscreen")

        Return Join(DataList.ToArray)
    End Function
    ''' <summary>
    ''' 获取新版 Minecraft Game 启动参数。
    ''' </summary>
    ''' <param name="Version"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GameArgumentsGameNew(ByRef Version As MCVersion) As String
        Dim DataList = New ArrayList

        '处理 Json
        For Each SubJson As JToken In Version.Json("arguments")("game")
            If SubJson.Type = JTokenType.String Then
                '字符串类型
                DataList.Add(SubJson.ToString)
            Else
                '非字符串类型
                If GameRuleCheck(SubJson("rules")) Then
                    '满足准则
                    If SubJson("value").Type = JTokenType.String Then
                        DataList.Add(SubJson("value").ToString)
                    Else
                        For Each value As JToken In SubJson("value")
                            DataList.Add(value.ToString)
                        Next
                    End If
                End If
            End If
        Next
        If Version.InheritVersion <> "" Then
            '读取Json
            Log("[Launch] 读取继承版本Json：" & PATH_MC & "versions\" & Version.InheritVersion & "\" & Version.InheritVersion & ".json")
            Dim InheritVersionJson As JObject = Nothing
            For Each Encode As Encoding In {Encoding.Default, New UTF8Encoding(False), Encoding.Unicode, Encoding.ASCII}
                Try
                    InheritVersionJson = CType(Newtonsoft.Json.JsonConvert.DeserializeObject(ReadFileToEnd(PATH_MC & "versions\" & Version.InheritVersion & "\" & Version.InheritVersion & ".json", Encode)), JObject)
                    GoTo InheritJsonReadFinish
                Catch
                    '编码错误
                    InheritVersionJson = Nothing
                End Try
            Next
            '各种编码都没戏了
            Throw New FileFormatException("Json文件读取失败：" & PATH_MC & "versions\" & Version.InheritVersion & "\" & Version.InheritVersion & ".json")
InheritJsonReadFinish:
            If InheritVersionJson("arguments")("game") IsNot Nothing Then
                For Each SubJson As JToken In InheritVersionJson("arguments")("game")
                    If SubJson.Type = JTokenType.String Then
                        '字符串类型
                        DataList.Add(SubJson.ToString)
                    Else
                        '非字符串类型
                        If GameRuleCheck(SubJson("rules")) Then
                            '满足准则
                            If SubJson("value").Type = JTokenType.String Then
                                DataList.Add(SubJson("value").ToString)
                            Else
                                For Each value As JToken In SubJson("value")
                                    DataList.Add(value.ToString)
                                Next
                            End If
                        End If
                    End If
                Next
            End If
        End If

        '添加服务器信息
        Dim Server As String = ReadIni("setup", "LaunchServer", "")
        If Server.Length > 0 Then
            If Server.Contains(":") Then
                '包含端口号
                DataList.Add("--server " & Server.Split(":")(0))
                DataList.Add("--port " & Server.Split(":")(1))
            Else
                '不包含端口号
                DataList.Add("--server " & Server)
                DataList.Add("--port 25565")
            End If
        End If

        '添加全屏
        If ReadIni("setup", "LaunchMode", "0") = "1" Then DataList.Add("--fullscreen")

        Return Join(DataList.ToArray)
    End Function

    ''' <summary>
    ''' 获取正版登录参数。
    ''' </summary>
    ''' <param name="Version">要启动的Minecraft版本。</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GameArgumentsMojang(ByVal Version As MCVersion) As String
        '初始化参数
        GameArguments = New Dictionary(Of String, String)
        Dim LoginJson = CType(Newtonsoft.Json.JsonConvert.DeserializeObject(LoginResult), JObject)
        GameArguments.Add("${version_name}", Version.Name)
        GameArguments.Add("${version_type}", ReadIni("setup", "LaunchShow", APPLICATION_FULL_NAME))
        GameArguments.Add("${game_directory}", Mid(GetVersionFolder(Version), 1, Len(GetVersionFolder(Version)) - 1))
        GameArguments.Add("${assets_root}", PATH_MC & "assets")
        GameArguments.Add("${game_assets}", PATH_MC & "assets\virtual\" & If(Version.Assets = "", "legacy", Version.Assets))
        GameArguments.Add("${assets_index_name}", Version.Assets)
        GameArguments.Add("${user_properties}", "{}")
        GameArguments.Add("${auth_player_name}", LoginJson("selectedProfile")("name").ToString)
        GameArguments.Add("${auth_uuid}", LoginJson("selectedProfile")("id").ToString)
        GameArguments.Add("${auth_access_token}", LoginJson("accessToken").ToString)
        GameArguments.Add("${access_token}", LoginJson("accessToken").ToString)
        GameArguments.Add("${auth_session}", LoginJson("accessToken").ToString)
        GameArguments.Add("${user_type}", "mojang")
        GameArguments.Add("${resolution_width}", ReadIni("setup", "LaunchModeWindowWidth", "854"))
        GameArguments.Add("${resolution_height}", ReadIni("setup", "LaunchModeWindowHeight", "480"))
        '获取字符串
        Dim Arguments As String = GameArgumentsJVM(Version) & " " & GameArgumentsGame(Version)
        '替换参数
        For Each entry As KeyValuePair(Of String, String) In GameArguments
            Arguments = Arguments.Replace(entry.Key, If(entry.Value.Contains(" ") Or entry.Value.Contains("-"), """" & entry.Value & """", entry.Value))
        Next
        Return Arguments
    End Function
    ''' <summary>
    ''' 获取离线登录参数。
    ''' </summary>
    ''' <param name="Version">要启动的Minecraft版本</param>
    ''' <param name="UserName">用户名</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GameArgumentsLegacy(ByVal Version As MCVersion, ByVal UserName As String) As String
        '初始化参数
        GameArguments = New Dictionary(Of String, String)
        Dim UUID As String = GetLegacyUUID(UserName, True)
        Dim AccessToken As String = SerRemove(ReadReg("AccessToken", ""))
        If Not Len(AccessToken) = 32 Then AccessToken = UUID
        log("[Launch] UUID：" & UUID)
        GameArguments.Add("${version_name}", Version.Name)
        GameArguments.Add("${version_type}", ReadIni("setup", "LaunchShow", APPLICATION_FULL_NAME))
        GameArguments.Add("${game_directory}", Mid(GetVersionFolder(Version), 1, Len(GetVersionFolder(Version)) - 1))
        GameArguments.Add("${assets_root}", PATH_MC & "assets")
        GameArguments.Add("${game_assets}", PATH_MC & "assets\virtual\" & If(Version.Assets = "", "legacy", Version.Assets))
        GameArguments.Add("${assets_index_name}", Version.Assets)
        GameArguments.Add("${user_properties}", "{}")
        GameArguments.Add("${auth_player_name}", UserName.Replace("""", ""))
        GameArguments.Add("${auth_uuid}", UUID)
        GameArguments.Add("${auth_access_token}", AccessToken)
        GameArguments.Add("${access_token}", AccessToken)
        GameArguments.Add("${auth_session}", AccessToken)
        GameArguments.Add("${user_type}", "Legacy")
        GameArguments.Add("${resolution_width}", ReadIni("setup", "LaunchModeWindowWidth", "854"))
        GameArguments.Add("${resolution_height}", ReadIni("setup", "LaunchModeWindowHeight", "480"))
        '获取字符串
        Dim Arguments As String = GameArgumentsJVM(Version) & " " & GameArgumentsGame(Version)
        '替换参数
        For Each entry As KeyValuePair(Of String, String) In GameArguments
            Arguments = Arguments.Replace(entry.Key, If(entry.Value.Contains(" ") Or entry.Value.Contains("-"), """" & entry.Value & """", entry.Value))
        Next
        Return Arguments
    End Function

    ''' <summary>
    ''' 由皮肤设置与用户名获取离线UUID。
    ''' </summary>
    Public Function GetLegacyUUID(ByVal UserName As String, ByVal TryMojangSource As Boolean) As String
        Select Case Val(ReadReg("LaunchSkin", "3"))
            Case 0
                '默认
                Return FillLength(SerAdd(UserName), "0", 32) '从用户名生成
            Case 1
                'Steve
                Dim UUID As String = FillLength(SerAdd(UserName), "0", 32) '从用户名生成
                Do Until GetSkinTypeFromUUID(UUID) = "Steve"
                    If UUID.EndsWith("FFFFF") Then UUID = Mid(UUID, 1, 32 - 5) & "00000"
                    UUID = Mid(UUID, 1, 32 - 5) & (Long.Parse(Right(UUID, 5), Globalization.NumberStyles.AllowHexSpecifier) + 1).ToString("X")
                Loop
                Return UUID
            Case 2
                'Alex
                Dim UUID As String = FillLength(SerAdd(UserName), "0", 32) '从用户名生成
                Do Until GetSkinTypeFromUUID(UUID) = "Alex"
                    If UUID.EndsWith("FFFFF") Then UUID = Mid(UUID, 1, 32 - 5) & "00000"
                    UUID = Mid(UUID, 1, 32 - 5) & (Long.Parse(Right(UUID, 5), Globalization.NumberStyles.AllowHexSpecifier) + 1).ToString("X")
                Loop
                Return UUID
            Case Else
                '使用正版用户名
                UserName = If(ReadReg("LaunchSkinName") = "", UserName, ReadReg("LaunchSkinName"))
                Dim UUID As String = GetUUIDFromUserName(UserName, TryMojangSource)
                If UUID = "" Then
                    Return FillLength(SerAdd(UserName), "0", 32)
                Else
                    Return UUID
                End If
        End Select
    End Function
    ''' <summary>
    ''' 从官方玩家用户名获取官方 UUID。获取失败会返回空字符串。
    ''' </summary>
    Public Function GetUUIDFromUserName(ByVal UserName As String, ByVal TryOnline As Boolean) As String
        Try
            '尝试从列表获取
            GetUUIDFromUserName = ReadIni("cache\skin\UUID", UserName, "")
            If Len(GetUUIDFromUserName) = 32 Then Exit Function
            '尝试从 Mojang 官方获取
            If Not TryOnline Then Return ""
            If UserName = "" Or UserName = "游戏名" Then Return ""
            Dim GotJson = CType(Newtonsoft.Json.JsonConvert.DeserializeObject(GetWebsiteCode("https://api.mojang.com/users/profiles/minecraft/" & UserName, Encoding.Default)), JObject)
            If IsNothing(GotJson) Then Return ""
            GetUUIDFromUserName = If(GotJson("id"), "")
            If Len(GetUUIDFromUserName) = 32 Then
                WriteIni("cache\skin\UUID", UserName, GetUUIDFromUserName)
            Else
                Return ""
            End If
        Catch ex As Exception
            ExShow(ex, "获取 UUID 失败（" & UserName & "）", ErrorLevel.Slient)
            Return ""
        End Try
    End Function

    ''' <summary>
    ''' 是否符合 Rule。输入为 "rule" 的 Json 项目，返回是否符合。
    ''' </summary>
    ''' <param name="RuleObj"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GameRuleCheck(ByVal RuleObj As JToken) As Boolean
        If IsNothing(RuleObj) Then Return True

        '初始化
        Dim Required As Boolean = False
        For Each Rule As JToken In RuleObj

            '单条条件验证
            Dim IsRightRule As Boolean? = Nothing '是否为正确的规则
            'Rule: 操作系统
            If Not IsNothing(Rule("os")) Then
                If Rule("os")("name") = "windows" Then
                    '是 Windows
                    If IsNothing(Rule("os")("version")) Then
                        IsRightRule = True
                    Else
                        '要求操作系统版本检查
                        Dim Version As String = My.Computer.Info.OSVersion
                        Dim Cr As String = Rule("os")("version").ToString
                        IsRightRule = RegexSearch(Version, Cr).Count > 0
                    End If
                Else
                    IsRightRule = False
                End If
            End If
            'Rule: 标签
            If Not IsNothing(Rule("features")) Then
                '只反选“is_demo_user”
                IsRightRule = IsNothing(Rule("features")("is_demo_user"))
            End If

            '反选确认
            If Rule("action").ToString = "allow" Then
                'allow
                If If(IsRightRule, True) Then
                    Required = True
                End If
            Else
                'disallow
                If If(IsRightRule, True) Then
                    Required = False
                End If
            End If

        Next
        Return Required
    End Function

#End Region

#Region "0.82-1.00 启动游戏"

    Private Sub GameThreadStart(ByVal CMD As String, ByVal Info As GameStartInfo)

        'CMD = "set appdata=" & PATH_MC & "&&cd /D %appdata%&&""" & PATH_JAVA & "\javaw.exe"" " & CMD
        'Dim myProcess As New Process()
        'Dim myProcessStartInfo As New ProcessStartInfo("cmd.exe")
        'myProcessStartInfo.UseShellExecute = False
        'myProcessStartInfo.RedirectStandardOutput = True
        'myProcessStartInfo.RedirectStandardError = True
        'myProcessStartInfo.RedirectStandardInput = True
        'myProcessStartInfo.CreateNoWindow = True
        'myProcessStartInfo.Arguments = "/c " & CMD
        'myProcess.StartInfo = myProcessStartInfo

        '启动信息
        Dim GameProcess = New Process()
        Dim StartInfo As New ProcessStartInfo(PATH_JAVA & "\javaw.exe")
        log("[Launch] Java 路径：" & PATH_JAVA & "\javaw.exe（" & GetFileVersion(PATH_JAVA & "\javaw.exe") & "）")
        StartInfo.EnvironmentVariables("appdata") = PATH_MC
        StartInfo.WorkingDirectory = GetVersionFolder(Info.SelectVersion)
        log("[Launch] 工作目录：" & GetVersionFolder(Info.SelectVersion))
        StartInfo.UseShellExecute = False
        StartInfo.RedirectStandardOutput = True
        StartInfo.RedirectStandardError = True
        StartInfo.CreateNoWindow = False
        StartInfo.Arguments = CMD
        GameProcess.StartInfo = StartInfo

        '开始进程
        log("[Launch] 启动 Minecraft，版本：" & Info.SelectVersion.Name)
        SendStat("启动", GetStringFromEnum(Info.LoginMethod), Info.SelectVersion.Name, Info.SelectVersion.MainVersionCode)
        GameProcess.Start()
        GameProcess.BeginOutputReadLine()
        GameProcess.BeginErrorReadLine()

        '设置进程优先级
        Select Case Val(ReadReg("LaunchLevel", "1"))
            Case 0
                GameProcess.PriorityClass = ProcessPriorityClass.AboveNormal
                Process.GetCurrentProcess.PriorityClass = ProcessPriorityClass.High
            Case 1
                '设置 MC 的进程优先级，默认为 Normal 所以不设置
                Process.GetCurrentProcess.PriorityClass = ProcessPriorityClass.AboveNormal
            Case 2
                GameProcess.PriorityClass = ProcessPriorityClass.BelowNormal
                '将 PCL 的进程优先级设置得比 MC 高一级，这样就不会造成互锁卡顿。由于这里正好是 Normal 所以不设置
        End Select

        '输出脚本
        CMD = "title Minecraft " & Info.SelectVersion.Name.Replace("""", "").Replace("&", "") & "&&set appdata=" & PATH_MC & "&&cd /D %appdata%&&""" & PATH_JAVA & "\javaw.exe"" " & CMD
        log("[Launch] 启动脚本（保存为 .bat 文件即可使用）：" & vbCrLf & CMD)

        '显示日志窗口
        GameWindowState = LoadState.Loading
        frmMain.Dispatcher.Invoke(Sub()
                                      Dim wind As New formMinecraft(GameProcess, Info.SelectVersion, ReadIni("setup", "LaunchLog", "False") = "False")
                                  End Sub)

    End Sub

    ''' <summary>
    ''' 启动后处理。
    ''' </summary>
    Private Sub GameThreadEnding()

        '剩余的内存（MB）
        Dim RamLeft As Integer = (New Microsoft.VisualBasic.Devices.ComputerInfo).AvailablePhysicalMemory / 1024 / 1024
        log("[Launch] Minecraft 启动成功，剩余内存：" & RamLeft & "MB")
        frmMain.Dispatcher.Invoke(Sub()

                                      Select Case Val(ReadIni("setup", "LaunchVisibility", "3"))
                                          Case 0 '关闭
                                              EndNormal()
                                          Case 3 '两分钟后关闭
                                              AniStart({
                                                  AaOpacity(frmMain, -1, 300),
                                                  AaCode({"Visible", frmMain, False}, 400),
                                                  AaCode({"ShowInTaskbar", frmMain, False}, 400),
                                                  AaCode({"End"}, 120000)
                                              }, "EndAll")
                                              If IsPlayingMusic Then frmMain.ChangeBgm()
                                              log("[Launch] 启动器已隐藏，将在两分钟后关闭")
                                          Case 1 '隐藏
                                              AniStart({
                                                  AaOpacity(frmMain, -1, 300),
                                                  AaCode({"Visible", frmMain, False}, 400),
                                                  AaCode({"ShowInTaskbar", frmMain, False}, 400)
                                              }, "EndAll")
                                              If IsPlayingMusic Then frmMain.ChangeBgm()
                                              log("[Launch] 启动器已隐藏")
                                          Case 2 '保留
                                              If RamLeft < 600 Then ShowHint(New HintConverter("内存仅剩 " & RamLeft & " MB，Minecraft 有可能崩溃", HintState.Warn))
                                          Case 4 '最小化
                                              frmMain.WindowState = WindowState.Minimized
                                      End Select

                                  End Sub)

    End Sub

#End Region

End Module