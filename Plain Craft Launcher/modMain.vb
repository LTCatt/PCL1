Imports Ionic.Zip

Public Module modMain

#Region "声明"

    '常量
    Public Const VERSION_NAME As String = "1.0.9"
    Public Const VERSION_CODE As Integer = 52
    Public Const MC_VERSION_CODE As Integer = 7
    Public Const PUSH_VERSION_CODE As Integer = 2
    Public Const MAINFORM_HEIGHT As Integer = 435
    Public Const MAINFORM_WIDTH As Integer = 760
    Public Const MAINFORM_NAME As String = "Plain Craft Launcher"
    Public Const APPLICATION_SHORT_NAME As String = "PCL"
    Public Const APPLICATION_FULL_NAME As String = "Plain Craft Launcher"
    Public Const DOWNLOADING_END As String = ".PCLdownloading"
    Public Const TX_SREVER_1 As String = "http://pcl-1253424809.cosgz.myqcloud.com/"
    Public Const TX_SREVER_2 As String = "http://pcl-1254159202.costj.myqcloud.com/"
    Public Const QN_SERVER As String = "http://op7rrgq1p.bkt.clouddn.com/"
    Public MODE_DEBUG As Boolean = False
    Public MODE_OFFLINE As Boolean = False
    Public MODE_DEVELOPER As Boolean = False

    '各种路径
    ''' <summary>
    ''' 程序内嵌图片文件夹路径。
    ''' </summary>
    ''' <remarks></remarks>
    Public PATH_IMAGE As String = "pack://application:,,,/images/"
    ''' <summary>
    ''' 用户设置的下载文件夹路径。
    ''' </summary>
    ''' <remarks></remarks>
    Public PATH_DOWNLOAD As String
    ''' <summary>
    ''' Java路径。不包含“javaw.exe”，不以“\”结尾。
    ''' </summary>
    ''' <remarks></remarks>
    Public PATH_JAVA As String
    ''' <summary>
    ''' .minecraft 文件夹路径。以“\”结尾。
    ''' </summary>
    ''' <remarks></remarks>
    Public PATH_MC As String

    '窗体
    Public frmStart As formStart
    Public frmMain As New formMain
    Public frmHomeLeft As New formHomeLeft
    Public frmHomeRight As New formHomeRight
    Public frmDownloadLeft As New formDownloadLeft
    Public frmDownloadRight As New formDownloadRight
    Public frmSetup As formSetup
    '尚未使用的窗体
    Public frmManageLeft As formManageLeft
    Public frmManageRight As formManageLeft
    Public frmHelpLeft As formHelpLeft
    Public frmHelpRight As formHelpLeft

    '通用信息
    ''' <summary>
    ''' 是否正在显示蓝屏。
    ''' </summary>
    ''' <remarks></remarks>
    Public IsShowingDeathBlue As Boolean = False
    ''' <summary>
    ''' 是否启用基础控件动画。
    ''' </summary>
    ''' <remarks></remarks>
    Public UseControlAnimation As Boolean = False
    ''' <summary>
    ''' 程序加载耗时的计时器。
    ''' </summary>
    ''' <remarks></remarks>
    Public LoadTimeCost As Integer
    ''' <summary>
    ''' 正版登录结果。可以通过判断它是否为空来确定正版登录是否成功。
    ''' </summary>
    ''' <remarks></remarks>
    Public LoginResult As String = ""
    ''' <summary>
    ''' 所有Minecraft版本的列表。
    ''' </summary>
    ''' <remarks></remarks>
    Public VersionsList As New Dictionary(Of VersionSwapState, ArrayList) From {{VersionSwapState.UNKNOWN, New ArrayList}, {VersionSwapState.NORMAL, New ArrayList}, {VersionSwapState.SWAP, New ArrayList}, {VersionSwapState.OLD, New ArrayList}, {VersionSwapState.WRONG, New ArrayList}}
    Private _PathEnv As String = ""
    ''' <summary>
    ''' Path环境变量。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property PathEnv As String
        Get
            If _PathEnv = "" Then _PathEnv = Environment.GetEnvironmentVariable("Path")
            Return _PathEnv
        End Get
        Set(ByVal value As String)
            _PathEnv = PathEnv
        End Set
    End Property
    ''' <summary>
    ''' 是否正在播放音乐。
    ''' </summary>
    ''' <remarks></remarks>
    Public IsPlayingMusic As Boolean = False
    ''' <summary>
    ''' 是否允许收集操作信息。
    ''' </summary>
    ''' <remarks></remarks>
    Public AllowFeedback As Boolean = True

#End Region

#Region "枚举"

    ''' <summary>
    ''' 加载状态，如加载中、成功、失败等。
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum LoadState As Byte
        ''' <summary>
        ''' 尚未开始加载，正在等待
        ''' </summary>
        ''' <remarks></remarks>
        Waiting = 0
        ''' <summary>
        ''' 正在加载中
        ''' </summary>
        ''' <remarks></remarks>
        Loading = 1
        ''' <summary>
        ''' 加载成功
        ''' </summary>
        ''' <remarks></remarks>
        Loaded = 2
        ''' <summary>
        ''' 加载失败
        ''' </summary>
        ''' <remarks></remarks>
        Failed = 3
    End Enum
    ''' <summary>
    ''' 提示信息种类，如警告、完成、错误等。
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum HintState As Integer
        ''' <summary>
        ''' 信息，通常是蓝色的“i”。
        ''' </summary>
        ''' <remarks></remarks>
        Info = 0
        ''' <summary>
        ''' 已完成，通常是绿色的“√”。
        ''' </summary>
        ''' <remarks></remarks>
        Finish = 1
        ''' <summary>
        ''' 警告，通常是黄色的“！”。
        ''' </summary>
        ''' <remarks></remarks>
        Warn = 2
        ''' <summary>
        ''' 错误，通常是红色的“×”。
        ''' </summary>
        ''' <remarks></remarks>
        Critical = 3
    End Enum
    ''' <summary>
    ''' Minecraft登陆方式。
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum LoginMethods As Integer
        ''' <summary>
        ''' 离线。
        ''' </summary>
        ''' <remarks></remarks>
        Legacy = 0
        ''' <summary>
        ''' 正版登录。
        ''' </summary>
        ''' <remarks></remarks>
        Mojang = 1
        ''' <summary>
        ''' 未知。这用来标记未加载时的状态。
        ''' </summary>
        ''' <remarks></remarks>
        Unknown = 2
    End Enum
    ''' <summary>
    ''' Minecraft版本检查结果。
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum VersionCheckState As Byte
        ''' <summary>
        ''' 尚未检查。
        ''' </summary>
        ''' <remarks></remarks>
        NOT_LOAD = 0
        ''' <summary>
        ''' 已经检查，没有问题。
        ''' </summary>
        ''' <remarks></remarks>
        NO_PROBLEM = 1
        ''' <summary>
        ''' 文件夹 或 Json与Jar 均不存在。
        ''' </summary>
        ''' <remarks></remarks>
        EMPTY_FOLDER = 2
        ''' <summary>
        ''' Json不存在，但是Jar存在。
        ''' </summary>
        ''' <remarks></remarks>
        JSON_NOT_EXIST = 3
        ''' <summary>
        ''' Json存在但是无法读取。
        ''' </summary>
        ''' <remarks></remarks>
        JSON_CANT_READ = 4
        ''' <summary>
        ''' 依赖版本文件夹不存在。
        ''' </summary>
        ''' <remarks></remarks>
        INHERITS_NOT_EXIST = 5
        ''' <summary>
        ''' 依赖版本检查出错。
        ''' </summary>
        ''' <remarks></remarks>
        INHERITS_EXCEPTION = 6
        ''' <summary>
        ''' Json存在，但是主要Jar不存在。
        ''' </summary>
        ''' <remarks></remarks>
        JAR_NOT_EXIST = 7
    End Enum
    ''' <summary>
    ''' Minecraft版本类型。
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum MCVersionType As Byte
        ''' <summary>
        ''' 未获取版本或版本获取失败。
        ''' </summary>
        ''' <remarks></remarks>
        UNKNOWN = 0
        ''' <summary>
        ''' 快照和预览版。
        ''' </summary>
        ''' <remarks></remarks>
        SNAPSHOT = 1
        ''' <summary>
        ''' 正式版。
        ''' </summary>
        ''' <remarks></remarks>
        RELEASE = 2
        ''' <summary>
        ''' 愚人节版。
        ''' </summary>
        ''' <remarks></remarks>
        FOOL = 6
        ''' <summary>
        ''' OptiFine版。
        ''' </summary>
        ''' <remarks></remarks>
        OPTIFINE = 3
        ''' <summary>
        ''' Forge版。
        ''' </summary>
        ''' <remarks></remarks>
        FORGE = 4
        ''' <summary>
        ''' 发布时间在2012年及更早的版本。
        ''' </summary>
        ''' <remarks></remarks>
        OLD = 5
    End Enum
    ''' <summary>
    ''' 版本折叠分类。
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum VersionSwapState As Byte
        ''' <summary>
        ''' 尚未确认。
        ''' </summary>
        ''' <remarks></remarks>
        UNKNOWN = 255
        ''' <summary>
        ''' 正常，无需折叠。
        ''' </summary>
        ''' <remarks></remarks>
        NORMAL = 0
        ''' <summary>
        ''' 折叠。
        ''' </summary>
        ''' <remarks></remarks>
        SWAP = 1
        ''' <summary>
        ''' 老版本。
        ''' </summary>
        ''' <remarks></remarks>
        OLD = 2
        ''' <summary>
        ''' 错误的版本。
        ''' </summary>
        ''' <remarks></remarks>
        WRONG = 3
    End Enum

#End Region

#Region "类"

    ''' <summary>
    ''' 提示信息种类转换。
    ''' </summary>
    ''' <remarks></remarks>
    Public Class HintConverter

        ''' <summary>
        ''' 提示信息文本。
        ''' </summary>
        ''' <remarks></remarks>
        Public Text As String

        ''' <summary>
        ''' 提示信息种类。
        ''' </summary>
        ''' <remarks></remarks>
        Public Type As HintState = 0

        Public Sub New(ByVal Text As String)
            Me.Text = Text
        End Sub
        Public Sub New(ByVal Text As String, ByVal Type As HintState)
            Me.Text = Text
            Me.Type = Type
        End Sub

        Public Shared Widening Operator CType(ByVal Text As String) As HintConverter
            Return New HintConverter(Text)
        End Operator
        Public Shared Widening Operator CType(ByVal Conv As HintConverter) As Color
            Select Case Conv.Type
                Case HintState.Info
                    Return Color.FromRgb(26, 148, 252)
                Case HintState.Finish
                    Return Color.FromRgb(29, 160, 29)
                Case HintState.Warn
                    Return Color.FromRgb(216, 137, 8)
                Case Else
                    Return Color.FromRgb(255, 46, 0)
            End Select
        End Operator
        Public Shadows Function ToString() As String
            Return "[" & {"Hint", "Finish", "Warn", "Critical"}(Type) & "] " & Text
        End Function

        ''' <summary>
        ''' 获取目前提示种类的英文名。
        ''' </summary>
        ''' <returns>提示种类的英文名，如“Critical”。</returns>
        ''' <remarks></remarks>
        Public Function GetTypeName() As String
            Return HintState.GetName(Type.GetType, Type)
        End Function

    End Class

    ''' <summary>
    ''' 一个不包含UI支持库的Minecraft版本类。
    ''' </summary>
    ''' <remarks></remarks>
    Public Class MCVersion
        Implements IComparable(Of MCVersion)
        ''' <summary>
        ''' 这些版本信息是否都是从已经配置好的文件中读取的。
        ''' </summary>
        ''' <remarks></remarks>
        Public LoadedByFile As Boolean = True
        ''' <summary>
        ''' 版本折叠分类。
        ''' </summary>
        ''' <remarks></remarks>
        Public SwapType As VersionSwapState = VersionSwapState.UNKNOWN
        ''' <summary>
        ''' 对应的Minecraft版本。这与Forge等无关。
        ''' </summary>
        ''' <remarks></remarks>
        Public Version As String = "未知"
        ''' <summary>
        ''' 版本的发布时间。用于鉴别版本号。
        ''' </summary>
        ''' <remarks></remarks>
        Public ReleaseTime As String = "1970-01-01T00:00:00"
        ''' <summary>
        ''' 主版本号（如1.9.2的9）。快照版为0。
        ''' </summary>
        ''' <remarks></remarks>
        Public MainVersionCode As Integer = 0
        ''' <summary>
        ''' Minecraft版本种类。
        ''' </summary>
        ''' <remarks></remarks>
        Public Type As MCVersionType = MCVersionType.UNKNOWN

        ''' <summary>
        ''' Logo的内容。用于ListItem显示。
        ''' </summary>
        ''' <remarks></remarks>
        Public Logo As String = PATH_IMAGE & "Block-Grass.png"
        ''' <summary>
        ''' 描述内容。用于ListItem显示。
        ''' </summary>
        ''' <remarks></remarks>
        Public Description As String = ""

        ''' <summary>
        ''' 版本检查结果。在检查版本时完成。
        ''' </summary>
        ''' <remarks></remarks>
        Public VersionCheckResult As VersionCheckState = VersionCheckState.NOT_LOAD
        ''' <summary>
        ''' 该版本的依赖版本。在检查版本时完成。
        ''' </summary>
        ''' <remarks></remarks>
        Public InheritVersion As String = ""
        ''' <summary>
        ''' 该版本的资源文件号。在检查版本时完成。
        ''' </summary>
        ''' <remarks></remarks>
        Public Assets As String

        '自处理属性

        ''' <summary>
        ''' 文件夹名称，或者说是这个版本的名称。
        ''' </summary>
        ''' <remarks></remarks>
        Public Name As String

        Private _Json As JObject
        ''' <summary>
        ''' Json对象。
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property Json As JObject
            Get
                If IsNothing(_Json) Then ReadJson()
                Return If(_JsonText = "ERROR", "", _Json)
            End Get
        End Property

        Private _JsonText As String = ""
        ''' <summary>
        ''' Json文件的内容。
        ''' </summary>
        ''' <remarks></remarks>
        Public ReadOnly Property JsonText As String
            Get
                If _JsonText = "" Then ReadJson()
                Return If(_JsonText = "ERROR", "", _JsonText)
            End Get
        End Property

        ''' <summary>
        ''' 完整的文件夹地址。以“\”结尾。
        ''' </summary>
        ''' <remarks></remarks>
        Public ReadOnly Property Path As String
            Get
                If Name = "" Then Throw New Exception("没有指定本版本的文件夹。")
                Return PATH_MC & "versions\" & Name & "\"
            End Get
        End Property

        '事件

        Public Sub New(ByVal FolderName As String)
            Name = FolderName
        End Sub
        Private Sub ReadJson()
            '读取Json
            For Each Encode As Encoding In {Encoding.Default, Encoding.Unicode, New UTF8Encoding(False), Encoding.ASCII}
                Try
                    _JsonText = ReadFileToEnd(Path & Name & ".json", Encode)
                    If _JsonText = "" Then GoTo Fail
                    _Json = CType(Newtonsoft.Json.JsonConvert.DeserializeObject(_JsonText), JObject)
                    Exit Sub
                Catch
                    '编码错误
                    _JsonText = "ERROR"
                End Try
NextFormat:
            Next
Fail:
            '各种编码都没戏了
            log("[System] Json文件读取失败：" & Path & Name & ".json", True)
            Throw New FileFormatException("Json文件读取失败：" & Path & Name & ".json")
        End Sub

        Public Overrides Function ToString() As String
            Return Name & " / " & GetStringFromEnum(SwapType) & If(Version = Name, "", " / " & Version) & If(VersionCheckResult = VersionCheckState.NO_PROBLEM, "", " / " & GetStringFromEnum(VersionCheckResult))
        End Function
        Public Overloads Function CompareTo(ByVal Other As MCVersion) As Integer Implements IComparable(Of MCVersion).CompareTo
            If IsNothing(Other) Then Return 1
            Return String.Compare(Me.Name, Other.Name)
        End Function

    End Class

#End Region

#Region "登录"

    ''' <summary>
    ''' 进行Login方式的正版登录。
    ''' </summary>
    ''' <param name="Email">邮箱地址。</param>
    ''' <param name="Pass">明文密码。</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function OnlineLogin(ByVal Email As String, ByVal Pass As String) As String
        '没联网登录个毛哦
        If MODE_OFFLINE Then Throw New WebException("没有网络连接，无法登录！")
        '尝试登录
        Try
            log("[System] 正版登录（Login）：" & Email)
            Dim request As WebRequest = WebRequest.Create("https://authserver.mojang.com/authenticate")
            request.Method = "POST"
            Dim postData As String = "{""agent"": {""name"": ""Minecraft"",""version"": 1},""username"":""" & Email & """,""password"":""" & Pass & """,""requestUser"":true}"
            Dim byteArray As Byte() = New UTF8Encoding(False).GetBytes(postData)
            request.ContentType = "application/json; charset=utf-8"
            request.ContentLength = byteArray.Length
            request.Timeout = 20000
            Dim dataStream As Stream = request.GetRequestStream()
            dataStream.Write(byteArray, 0, byteArray.Length)
            dataStream.Close()
            Dim response As WebResponse = request.GetResponse()
            dataStream = response.GetResponseStream()
            Dim reader As New StreamReader(dataStream)
            Dim responseFromServer As String = reader.ReadToEnd()
            OnlineLogin = responseFromServer

            '输出信息
            Dim LoginJson = CType(Newtonsoft.Json.JsonConvert.DeserializeObject(OnlineLogin), JObject)
            WriteReg("AccessToken", SerAdd(LoginJson("accessToken").ToString))
            WriteReg("ClientToken", SerAdd(LoginJson("clientToken").ToString))
            WriteReg("MojangPlayerName", LoginJson("selectedProfile")("name").ToString)
            WriteReg("MojangPlayerUUID", LoginJson("selectedProfile")("id").ToString)

            log("[System] 正版登录成功")
            reader.Close()
            dataStream.Dispose()
            response.Close()
        Catch ex As Exception
            ExShow(ex, "正版登录失败", ErrorLevel.Slient)
            If ex.Message.Contains("403") Then
                Throw New WebException("正版登录失败，请检查你的邮箱和密码是否正确或等待5分钟再试！")
            Else
                Throw New WebException("正版登录失败！" & vbCrLf & "详细的错误信息：" & ex.Message)
            End If
        End Try
    End Function
    ''' <summary>
    ''' 进行Refresh方式的正版登录。
    ''' </summary>
    ''' <param name="AccessToken">在Login登录中存储的AccessToken。</param>
    ''' <param name="ClientToken">在Login登录中存储的ClientToken。</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function RefreshLogin(ByVal AccessToken As String, ByVal ClientToken As String) As String
        '没联网登录个毛哦
        If MODE_OFFLINE Then Throw New WebException("没有网络连接，无法登录！")
        '尝试登录
        Try
            log("[System] 刷新正版登录")
            Dim request As WebRequest = WebRequest.Create("https://authserver.mojang.com/refresh")
            request.Method = "POST"
            Dim postData As String = "{""accessToken"":""" & AccessToken & """,""clientToken"":""" & ClientToken & """}"
            Dim byteArray As Byte() = New UTF8Encoding(False).GetBytes(postData)
            request.ContentType = "application/json; charset=utf-8"
            request.ContentLength = byteArray.Length
            request.Timeout = 20000
            Dim dataStream As Stream = request.GetRequestStream()
            dataStream.Write(byteArray, 0, byteArray.Length)
            dataStream.Close()
            Dim response As WebResponse = request.GetResponse()
            dataStream = response.GetResponseStream()
            Dim reader As New StreamReader(dataStream)
            Dim responseFromServer As String = reader.ReadToEnd()
            RefreshLogin = responseFromServer

            '输出信息
            Dim LoginJson = CType(Newtonsoft.Json.JsonConvert.DeserializeObject(RefreshLogin), JObject)
            WriteReg("AccessToken", SerAdd(LoginJson("accessToken").ToString))
            WriteReg("ClientToken", SerAdd(LoginJson("clientToken").ToString))
            WriteReg("MojangPlayerName", LoginJson("selectedProfile")("name").ToString)
            WriteReg("MojangPlayerUUID", LoginJson("selectedProfile")("id").ToString)

            log("[System] 刷新正版登录成功")
            reader.Close()
            dataStream.Dispose()
            response.Close()
        Catch ex As Exception
            ExShow(ex, "刷新正版登录失败", ErrorLevel.Slient)
            Throw New WebException("刷新正版登录失败！" & vbCrLf & "详细的错误信息：" & ex.Message)
        End Try
    End Function

#End Region

#Region "线程池"
    Public PoolCount As Integer = 0 '目前运行中的线程计数
    Public Const POOL_MAXCOUNT As Integer = 15 '最大线程数

    Public Pool As New ArrayList '线程池
    Public Sub PoolLoader()
        On Error Resume Next
        Do While True
            If Pool.Count > 0 Then
ReSearch:
                For i = 0 To Pool.Count - 1
                    If IsNothing(Pool(i)) Then
                        Pool.RemoveAt(i)
                        Exit For
                    End If
                    Select Case Pool(i).ThreadState
                        Case System.Threading.ThreadState.Unstarted
                            If PoolCount < POOL_MAXCOUNT Then
                                '线程尚未启动
                                Pool(i).Start()
                                PoolCount = PoolCount + 1
                            End If
                        Case System.Threading.ThreadState.Stopped, System.Threading.ThreadState.Aborted
                            '线程已经停止
                            PoolCount = PoolCount - 1
                            Pool.RemoveAt(i)
                            i = i - 1
                    End Select
                    If i >= Pool.Count - 1 Then Exit For
                Next i
            End If
            Thread.Sleep(20)
        Loop
    End Sub

    ''' <summary>
    ''' 加载线程：Java文件夹检测。它会检测PATH_JAVA的值，如果无Java则初始化。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub PoolJavaFolder(IsJustFind As Boolean)

        '初始化
        PATH_JAVA = If(IsJustFind, "", ReadReg("SetupJavaPath"))

        Try

            '如果注册表已经存在 Java 的正确路径就结束
            Dim Env As String = PathEnv
            If File.Exists(PATH_JAVA & "\javaw.exe") And (File.Exists(PATH_JAVA & "\jli.dll") Or PATH_JAVA.Contains("javapath")) Then
                log("[Pool] Java 路径：" & PATH_JAVA)
                Exit Sub
            End If

            '检查环境变量中的 Java
            Dim EachPath As String() = Split(Env, ";")
            For Each PathFind As String In EachPath
                '格式化字符串，保证不以“\”结尾
                If PathFind.EndsWith("\") Then PathFind = Left(PathFind, Len(PathFind) - 1)
                '检查有效性
                If File.Exists(PathFind & "\javaw.exe") And (File.Exists(PathFind & "\jli.dll") Or PathFind.Contains("javapath")) Then
                    PATH_JAVA = PathFind
                    log("[Pool] 环境变量中找到 Java：" & PATH_JAVA)
                    WriteReg("SetupJavaPath", PATH_JAVA)
                    Exit Sub
                End If
            Next

            '人工查找
            log("[Pool] 遍历寻找 Java 开始")
            Dim Result As String
            '循环每个盘
            For Each Disk As DriveInfo In DriveInfo.GetDrives()
                Result = SearchJava(Disk.Name)
                '如果找到了的话就跳出，并且设置环境变量
                If Not Result = "" Then
                    PATH_JAVA = Mid(Result, 1, Result.LastIndexOf("\"))
                    GoTo FinishSearch
                End If
            Next
            '查找当前启动器目录
            Result = SearchJava(PATH, True)
            If Not Result = "" Then
                PATH_JAVA = Mid(Result, 1, Result.LastIndexOf("\"))
                GoTo FinishSearch
            End If

            '人工查找失败
            log("[Pool] 遍历寻找 Java 失败")
            PATH_JAVA = ""
            ShowHint(New HintConverter("未找到可用的 Java", HintState.Warn))
            Exit Sub

        Catch ex As Exception
            ExShow(ex, "查找 Java 时出错", ErrorLevel.MsgboxAndFeedback)
        End Try

FinishSearch:
        If Not IsJustFind Then SetJavaEnvironment()

    End Sub
    ''' <summary>
    ''' 检查指定路径下的文件夹并且模糊搜索Java。这不会搜索全部路径。
    ''' </summary>
    ''' <param name="path">开始搜索的起始路径</param>
    ''' <param name="fullSearch">搜索当前文件夹下的全部文件夹（这不会传递到子级文件夹）</param>
    ''' <returns>搜索到的Java路径，如果失败则为空</returns>
    ''' <remarks></remarks>
    Private Function SearchJava(ByVal path As String, Optional ByVal fullSearch As Boolean = False) As String
        Try
            SearchJava = ""
            Dim AllPath() As String
            Dim LastEntry As String '文件夹或文件名
            If Directory.Exists(path) Then
                '该目录存在
                AllPath = Directory.GetFileSystemEntries(path)
                For Each Entry As String In AllPath
                    LastEntry = Entry.Split("\")(Entry.Split("\").Length - 1) '获取文件夹或文件名
                    Dim SearchEntry = LastEntry.ToLower.Replace(" ", "") '用于搜索的字符串
                    If fullSearch Or SearchEntry.Contains("java") Or SearchEntry.Contains("jdk") Or SearchEntry.Contains("jre") Or SearchEntry.Contains("bin") Or SearchEntry.Contains("mc") Or SearchEntry.Contains("minecraft") Or SearchEntry.Contains("program") Or SearchEntry.Contains("我的世界") Or SearchEntry.Contains("net") Or SearchEntry.Contains("runtime") Or SearchEntry.Contains("oracle") Or SearchEntry.Contains("1.") Or SearchEntry.Contains("启") Then
                        If File.Exists(Entry) Then
                            '如果是文件
                            If LastEntry = "javaw.exe" And File.Exists(Mid(Entry, 1, Entry.LastIndexOf("\")) & "\jli.dll") Then
                                '找到Java
                                SearchJava = Entry
                                Exit For
                            End If
                        ElseIf Directory.Exists(Entry) Then
                            '如果是文件夹
                            Dim Result As String = SearchJava(Entry)
                            If Not Result = "" Then Return Result
                        End If
                    End If
                Next
            End If
        Catch ex As Exception
            ExShow(ex, "遍历查找Java时出错", ErrorLevel.Slient)
            '防止无文件夹权限而导致崩溃
            SearchJava = ""
        End Try
    End Function

    ''' <summary>
    ''' 加载线程：Minecraft文件夹检测。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub PoolMinecraftFolder()
        Try
            '等待版本列表加载结束
            Do While IsPoolVersionListRunning
                Thread.Sleep(25)
            Loop

            Dim MojangPath As String = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) & "\.minecraft\"
            Dim AllFoldersList As New ArrayList

            '加载可用文件夹地址
            Dim AllFolders As String = ReadIni("setup", "LaunchFolders", "")
            If Not AllFolders = "" Then
                '如果存在可用文件夹地址，则再次检查它们
                Dim NewFolderList As New ArrayList
                For Each Dir As String In AllFolders.Split("|")
                    If CheckDirectoryPermission(Dir) Then
                        NewFolderList.Add(Dir)
                        AllFoldersList.Add(Dir)
                    End If
                Next
                AllFolders = Join(NewFolderList.ToArray, "|")
                WriteIni("setup", "LaunchFolders", AllFolders)
            End If

            '加载设置中的Minecraft文件夹地址
            PATH_MC = ReadIni("setup", "LaunchFolderSelect", "")
            If CheckDirectoryPermission(MojangPath & "versions\") Then
                If Not Directory.GetDirectories(MojangPath & "versions\").Length = 0 Then AllFoldersList.Add(MojangPath)
            End If

            '如果为当前目录则自动创建
            If PATH_MC = PATH & ".minecraft\" Then
                If Not CheckDirectoryPermission(PATH_MC) Then
                    Directory.CreateDirectory(PATH_MC)
                    Directory.CreateDirectory(PATH_MC & "versions\")
                End If
                GoTo FinishMinecraftFolderCheck
            End If

            '如果在可用文件夹列表且有效则结束
            If AllFoldersList.Contains(PATH_MC) And CheckDirectoryPermission(PATH_MC) Then GoTo FinishMinecraftFolderCheck

            '如果是用户设置的路径，则显示一个通知
            If Not PATH_MC = "" Then ShowHint("原先的 Minecraft 文件夹路径已失效：" & PATH_MC)

            '优先尝试设置为当前文件夹
            If CheckDirectoryPermission(PATH & ".minecraft\versions") Then
                PATH_MC = PATH & ".minecraft\"
                GoTo FinishMinecraftFolderCheck
            End If

            '优先尝试设置为官启文件夹
            If AllFoldersList.Contains(MojangPath) Then
                If MyMsgbox("官方启动器的 Minecraft 文件夹下存在 " & Directory.GetDirectories(MojangPath & "versions").Length & " 个版本，是否使用官方启动器的文件夹？" & vbCrLf & "你可以在设置页面更改这个设置。", "Minecraft 文件夹确认", "确定", "取消") = 1 Then
                    '使用官启文件夹
                    PATH_MC = MojangPath
                    GoTo FinishMinecraftFolderCheck
                End If
            End If
            AllFoldersList.Remove(MojangPath)

            '从列表中查找新的目录
            For Each Dir As String In AllFolders.Split("|")
                If CheckDirectoryPermission(Dir) Then
                    PATH_MC = Dir
                    GoTo FinishMinecraftFolderCheck
                End If
            Next

            '执行到这里就需要创建一个新的.minecraft文件夹了
            PATH_MC = PATH & ".minecraft\"
            Directory.CreateDirectory(PATH_MC)
            Directory.CreateDirectory(PATH_MC & "versions\")

FinishMinecraftFolderCheck:

            log("[Pool] .minecraft文件夹：" & PATH_MC)
            WriteIni("setup", "LaunchFolderSelect", PATH_MC)

            '输出启动器信息
            If Not File.Exists(PATH_MC & "launcher_profiles.json") Then
                WriteFile(PATH_MC & "launcher_profiles.json",
                            "{" & vbCrLf &
                            "  ""profiles"": {" & vbCrLf &
                            "    """ & APPLICATION_SHORT_NAME & """: {" & vbCrLf &
                            "      ""name"": """ & APPLICATION_SHORT_NAME & """," & vbCrLf &
                            "    }," & vbCrLf &
                            "    ""(Default)"": {" & vbCrLf &
                            "      ""name"": ""(" & APPLICATION_SHORT_NAME & ")""" & vbCrLf &
                            "    }" & vbCrLf &
                            "  }," & vbCrLf &
                            "  ""selectedProfile"": ""(Default)""," & vbCrLf &
                            "  ""clientToken"": ""88888888-8888-8888-8888-888888888888""" & vbCrLf &
                            "}")
                log("[Pool] 已创建 launcher_profiles.json")
            End If
            If Not File.Exists(PATH_MC & "options.txt") Then
                WriteFile(PATH_MC & "options.txt", "lang:zh_cn" & vbCrLf)
                log("[Pool] 已创建 options.txt")
            End If

            '继续加载版本列表
            Pool.Add(New Thread(AddressOf PoolVersionList))

        Catch ex As Exception
            ExShow(ex, "检测MC文件夹失败", ErrorLevel.MsgboxAndFeedback)
        End Try
    End Sub

    Private PoolLoginLock As New Object
    ''' <summary>
    ''' 加载线程：自动登录。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub PoolLogin(IsAutoLogin As Boolean)
        Try
            If MODE_OFFLINE Then Exit Sub
            SyncLock PoolLoginLock
                If (Not ReadReg("Email").Contains("@")) Or (Len(ReadReg("Password")) < 4) Or (IsAutoLogin And ReadReg("HomeSave", "True") = "False") Then GoTo ExitSub

                frmHomeRight.StartButtonIsLogining = True

                '尝试登录
                If ReadReg("ClientToken") = "" Then
                    PoolLoginRun(New LoginInfo With {.IsRefresh = False, .Email = ReadReg("Email"), .Password = SerRemove(ReadReg("Password"))}, True)
                Else
                    If Not PoolLoginRun(New LoginInfo With {.IsRefresh = True, .AccessToken = SerRemove(ReadReg("AccessToken")), .ClientToken = SerRemove(ReadReg("ClientToken"))}, False) Then
                        PoolLoginRun(New LoginInfo With {.IsRefresh = False, .Email = ReadReg("Email"), .Password = SerRemove(ReadReg("Password"))}, True)
                    End If
                End If

ExitSub:
                frmHomeRight.StartButtonIsLogining = False
            End SyncLock

            '加载皮肤
            If LoginResult.Contains("selectedProfile") Then
                Dim UUID As String = CType(Newtonsoft.Json.JsonConvert.DeserializeObject(LoginResult), JObject)("selectedProfile")("id").ToString
                WriteIni("cache\skin\UUID", CType(Newtonsoft.Json.JsonConvert.DeserializeObject(LoginResult), JObject)("selectedProfile")("name").ToString, UUID)
                Dim SkinAddress As String = DownloadSkin(UUID)
                If Not SkinAddress = "" Then frmMain.Dispatcher.Invoke(Sub() frmHomeRight.LoadMojangSkin(SkinAddress))
            End If

        Catch ex As Exception
            frmHomeRight.StartButtonIsLogining = False
            ExShow(ex, "自动登录出错", ErrorLevel.MsgboxAndFeedback)
        End Try
    End Sub
    ''' <summary>
    ''' 进行正版登录所需的信息包。
    ''' </summary>
    ''' <remarks></remarks>
    Public Structure LoginInfo
        Public IsRefresh As Boolean
        Public Email As String
        Public Password As String
        Public AccessToken As String
        Public ClientToken As String
    End Structure
    ''' <summary>
    ''' 按照给定的方式进行登录尝试。
    ''' </summary>
    ''' <remarks></remarks>
    Private Function PoolLoginRun(ByVal Info As LoginInfo, ByVal IsShowHint As Boolean) As Boolean

        '正版登录
        Try
            log("[Poll] 登录开始")
            If Info.IsRefresh Then
                LoginResult = RefreshLogin(Info.AccessToken, Info.ClientToken)
            Else
                LoginResult = OnlineLogin(Info.Email, Info.Password)
            End If
            log("[Poll] 登录成功")
            Return True
        Catch ex As Exception
            ExShow(ex, "登录失败")
            If IsShowHint Then ShowHint(New HintConverter(GetStringFromException(ex), HintState.Warn))
            LoginResult = ""
            Return False
        End Try

    End Function

    Public NeedUpdate As LoadState = LoadState.Waiting
    ''' <summary>
    ''' 加载线程：自动更新。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub PoolUpdate()
        If MODE_OFFLINE Then NeedUpdate = LoadState.Failed : Exit Sub

        '更新

        Try
            NeedUpdate = LoadState.Loading
            Dim data As String = GetWebsiteCode(TX_SREVER_1 & "update" & If(ReadIni("setup", "SysUpdateTest", "False"), "_snapshot", "_release") & ".ini", Encoding.Default)
            If Not data.Contains("VersionCode") Then
                Throw New Exception("获取自动更新网页代码失败。")
                Exit Sub
            End If

            WriteFile("PCL\update.ini", data, , , False)

            '检查更新
            Dim newVersionCode As Integer = ReadIni(PATH & "PCL\update.ini", "VersionCode", "0")
            '为了避免在首次打开时自动更新到 PCL2，这一段暂时注释了
            'If newVersionCode > VERSION_CODE Then
            '    NeedUpdate = LoadState.Loaded
            '    If ReadIni("setup", "SysUpdate", "True") = "False" Then GoTo NoHint '设置为不检查更新
            '    log("[Pool] 发现新版本（" & newVersionCode & "）")
            '    '更新可用
            '    If ReadIni("setup", "SysUpdateHint", "True") Then
            '        '提示更新
            '        frmHomeRight.Dispatcher.Invoke(Sub() frmHomeRight.ShowUpdate("发现启动器更新：" & ReadIni("update", "Version", "未知版本"), formHomeRight.UpdateType.PCL))
            '    Else
            '        '自动开始更新
            '        ShowHint("正在自动更新，更新结束后 PCL 将会自动重启")
            '        Dim th As New Thread(AddressOf DownloadUpdate)
            '        th.Start()
            '    End If
            'Else
            NeedUpdate = LoadState.Failed
            Log("[Pool] 已经是最新版本（" & newVersionCode & "）")
            'End If
NoHint:

        Catch ex As Exception
            NeedUpdate = LoadState.Failed
            ExShow(ex, "检查更新失败")
            '更新检测失败
            Dim th As New Thread(Sub()
                                     log("[Pool] 开始测试服务器 Ping")
                                     Try
                                         log("[Pool] Ping 测试成功：" & Ping(TX_SREVER_1, 10000) & "ms")
                                     Catch exx As Exception
                                         ExShow(exx, "无法连接至 PCL 服务器")
                                     End Try
                                 End Sub)
            th.Start()
        End Try

        '通知

        Dim newNotice As Integer = ReadIni(PATH & "PCL\update.ini", "Notice", "0")
        Dim currentNotice As Integer = ReadIni("setup", "Notice", "0")
        If newNotice > currentNotice Then
            Try
                log("[Pool] 发现公告（" & newNotice & "）")
                Dim Notices As JObject = ReadJson(GetWebsiteCode(TX_SREVER_1 & "notice.json", Encoding.Default))
                For Each Notice As JToken In Notices("all")
                    If Val(Notice("id")) > currentNotice Then

                        '确认要求
                        For Each Require As JToken In If(Notice("require"), {})
                            Select Case Require("type").ToString
                                Case "open_count_min"
                                    '{"type":"open_count_min","value":50}
                                    If Val(ReadIni("setup", "Count", "0")) < Val(Require("value")) Then SendStat("公告", "跳过", Notice("id")) : GoTo NextNotice
                                Case "open_count_max"
                                    '{"type":"open_count_max","value":55}
                                    If Val(ReadIni("setup", "Count", "0")) > Val(Require("value")) Then SendStat("公告", "跳过", Notice("id")) : GoTo NextNotice
                                Case "version_code_min"
                                    '{"type":"version_code_min","value":5}
                                    If VERSION_CODE < Val(Require("value")) Then SendStat("公告", "跳过", Notice("id")) : GoTo NextNotice
                                Case "version_code_max"
                                    '{"type":"version_code_max","value":10}
                                    If VERSION_CODE > Val(Require("value")) Then SendStat("公告", "跳过", Notice("id")) : GoTo NextNotice
                            End Select
                        Next

                        '初始化
                        SendStat("公告", "显示", Notice("id"))
                        Dim Arguments() As String = Notice("text").ToString.Split("|")

                        '确认类型
                        Select Case Notice("type").ToString
                            Case "msgbox"
                                '弹窗
                                '"text":"弹窗测试标题|弹窗测试内容\n第二行|知道啦"
                                MyMsgbox(Arguments(1), Arguments(0).Replace("\n", vbCrLf), Arguments(2))
                            Case "url"
                                '可选调用URL
                                '"text":"网页测试标题|网页测试\n第二行|知道啦|不打开|http://www.mcbbs.net/"
                                If MyMsgbox(Arguments(1), Arguments(0).Replace("\n", vbCrLf), Arguments(2), Arguments(3)) = 1 Then
                                    Process.Start(Arguments(4))
                                End If
                            Case "update"
                                '要求更新
                                '"text":"更新测试标题|更新测试\n第二行|知道啦|打死不更新"
                                If NeedUpdate Then
                                    If MyMsgbox(Arguments(1), Arguments(0).Replace("\n", vbCrLf), Arguments(2), Arguments(3)) = 1 Then
                                        frmMain.Dispatcher.Invoke(Sub()
                                                                      ShowHint("正在更新，更新结束后 PCL 将会自动重启")
                                                                      Dim th As New Thread(AddressOf DownloadUpdate)
                                                                      th.Start()
                                                                      AniStart({
                                                                                  AaHeight(frmHomeRight.panLaunch, 277 - frmHomeRight.panLaunch.Height, , , New AniEaseEnd),
                                                                                  AaOpacity(frmHomeRight.btnUpdate, -frmHomeRight.btnUpdate.Opacity, 200)
                                                                              }, "HomeRightHideUpdate")
                                                                  End Sub)
                                    End If
                                End If
                        End Select

                    End If
NextNotice:
                Next
                WriteIni("setup", "Notice", newNotice)
            Catch ex As Exception
                ExShow(ex, "处理公告失败")
            End Try
        End If

    End Sub

    Public IsPoolVersionListRunning As Boolean = False
    ''' <summary>
    ''' 加载线程：加载版本列表。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub PoolVersionList()
        '保证不同时加载版本列表
        Do While IsPoolVersionListRunning
            Thread.Sleep(RandomInteger(15, 65))
        Loop
        IsPoolVersionListRunning = True
        frmHomeRight.StartButtonIsLoading = True

        Try

            log("[Pool] 预加载版本列表开始")

            Dim ReloadAll As Boolean = False '是否重载全部版本

            '检查Versions文件夹
            If Not Directory.Exists(PATH_MC & "versions\") Then
                VersionsList = New Dictionary(Of VersionSwapState, ArrayList) From {{VersionSwapState.UNKNOWN, New ArrayList}, {VersionSwapState.NORMAL, New ArrayList}, {VersionSwapState.SWAP, New ArrayList}, {VersionSwapState.OLD, New ArrayList}, {VersionSwapState.WRONG, New ArrayList}}
                GoTo LoadEnd
            End If

            '遍历文件夹
            Dim FolderList As New ArrayList
            For Each Folder As DirectoryInfo In (New DirectoryInfo(PATH_MC & "versions")).GetDirectories
                FolderList.Add(Folder.Name)
            Next

            '对比文件夹列表
            Dim FolderListCheck As String = (MC_VERSION_CODE & "#" & Join(FolderList.ToArray, "#")).Replace(vbCrLf, "\n").Replace(":", "..").Replace("LastCheckedFolder", "LCF")
            If Not ReadIni(PATH_MC & "PCL.ini", "LastCheckedFolder") = FolderListCheck Then
                '文件夹列表不符
                log("[Pool] 文件夹列表变更，重载所有版本")
                ReloadAll = True
                WriteIni(PATH_MC & "PCL.ini", "LastCheckedFolder", FolderListCheck)
            End If

Reload:
            VersionsList = New Dictionary(Of VersionSwapState, ArrayList) From {{VersionSwapState.UNKNOWN, New ArrayList}, {VersionSwapState.NORMAL, New ArrayList}, {VersionSwapState.SWAP, New ArrayList}, {VersionSwapState.OLD, New ArrayList}, {VersionSwapState.WRONG, New ArrayList}}

            '遍历每个版本
            For Each VersionName As String In FolderList
                If Not CheckDirectoryPermission(PATH_MC & "versions\" & VersionName & "\") Then GoTo NextFolder
                Dim Version As New MCVersion(VersionName)
                Directory.CreateDirectory(Version.Path & "\PCL")

                If ReadIni(Version.Path & "\PCL\Version.ini", "VersionListCode", "0") = MC_VERSION_CODE And Not ReloadAll Then

                    '存在有效的PCL配置文件，读取配置文件
                    Version.LoadedByFile = True
                    With Version
                        .SwapType = ReadIni(Version.Path & "\PCL\Version.ini", "SwapType")
                        .Version = ReadIni(Version.Path & "\PCL\Version.ini", "Version")
                        .ReleaseTime = ReadIni(Version.Path & "\PCL\Version.ini", "ReleaseTime")
                        .Assets = ReadIni(Version.Path & "\PCL\Version.ini", "Assets")
                        .MainVersionCode = ReadIni(Version.Path & "\PCL\Version.ini", "MainVersionCode")
                        .Type = ReadIni(Version.Path & "\PCL\Version.ini", "Type")
                        .Logo = ReadIni(Version.Path & "\PCL\Version.ini", "Logo")
                        .Description = ReadIni(Version.Path & "\PCL\Version.ini", "Description")
                        .VersionCheckResult = ReadIni(Version.Path & "\PCL\Version.ini", "VersionCheckResult")
                        .InheritVersion = ReadIni(Version.Path & "\PCL\Version.ini", "InheritVersion")
                    End With

                    '重新检测
                    Dim OldResult As VersionCheckState = Version.VersionCheckResult
                    CheckMCVersion(Version, Version.VersionCheckResult = VersionCheckState.INHERITS_EXCEPTION)
                    If Not Version.VersionCheckResult = OldResult Then
                        ReloadAll = True
                        GoTo Reload
                    End If

                    '图片的重新检测
                    If Not Version.Logo.StartsWith(PATH_IMAGE) Then
                        If Not File.Exists(Version.Logo) Then GoTo NotLoadFile
                    End If

                Else
NotLoadFile:
                    '不存在PCL配置文件，读取版本信息
                    Version.LoadedByFile = False
                    CheckMCVersion(Version, True)

                    '版本存在错误
                    If Not Version.VersionCheckResult = VersionCheckState.NO_PROBLEM Then
                        '错误的版本
                        Version.SwapType = VersionSwapState.WRONG
                        GoTo NextVersion
                    End If

                    '尝试从继承版本处获取版本号
                    If Not Version.InheritVersion = "" Then
                        Version.Version = Version.InheritVersion
                        GoTo VersionSearchEnd
                    End If

                    '从json获取版本失败，试图从文件夹名称获取版本号，如果失败则标记“未知”
                    Dim CharCheck As ArrayList = RegexSearch(Version.Name, "1\.1?[0-9]{1}(\.[1-9]{1}([0-9]{1})?)?(-pre[1-9]?)?|1[2-9]{1}w[0-9]{1,2}[a-z]{1}")
                    If CharCheck.Count > 0 Then
                        Version.Version = CharCheck(0)
                    Else
                        Version.Version = "未知"
                    End If
VersionSearchEnd:

                    '获取发布时间
                    If Version.JsonText.Contains("releaseTime") Then
                        Version.ReleaseTime = RegexSearch(Version.JsonText.Replace(" ", ""), "(?<=releaseTime"":"")([^T]+)([^\+\-]+)")(0)
                    Else
                        Version.ReleaseTime = ""
                    End If

                    '设置主版本号
                    Dim MainVersion As ArrayList = RegexSearch(Version.Version, "1.[0-9]+")
                    Version.MainVersionCode = Math.Min(20, If(MainVersion.Count = 1, Int(MainVersion(0).Replace("1.", "")), 0))

                    '获取版本种类
                    Version.Type = MCVersionType.RELEASE
                    If Version.InheritVersion = "" Then
                        '无继承版本的
                        If Version.Version = "未知" Then Version.Type = MCVersionType.UNKNOWN
                        If Version.Version.ToLower.Contains("w") Or Version.Version.ToLower.Contains("pre") Or Version.JsonText.Replace(" ", "").Contains("""type"":""snapshot""") Then Version.Type = MCVersionType.SNAPSHOT
                        If Len(Version.ReleaseTime) > 6 Then If Val(Mid(Version.ReleaseTime, 3, 2)) < 14 Then Version.Type = MCVersionType.OLD '未知版本可能显示为1970年，故取后两位数
                        If If(IsNothing(Version.Json("type")), False, Version.Json("type") = "fool") Then Version.Type = MCVersionType.FOOL
                        If Version.JsonText.Contains("minecraftforge:minecraftforge") Then Version.Type = MCVersionType.FORGE
                    Else
                        '有继承版本的
                        Version.Type = If(Version.JsonText.Contains("Forge"), MCVersionType.FORGE, MCVersionType.OPTIFINE)
                    End If

                    '检测自定义种类信息
                    Version.SwapType = Val(ReadIni(Version.Path & "PCL\Setup.ini", "CustomType", "255"))
                    If Version.SwapType = VersionSwapState.UNKNOWN Then 'UNKNOWN为255
                        Select Case Version.Type
                            Case MCVersionType.SNAPSHOT, MCVersionType.RELEASE, MCVersionType.OPTIFINE
                                '需要再次确认是否应该折叠，不加处理
                            Case MCVersionType.OLD
                                '老版本，直接归类
                                Version.SwapType = VersionSwapState.OLD
                            Case MCVersionType.FOOL
                                '愚人节版本，直接折叠
                                Version.SwapType = VersionSwapState.SWAP
                            Case Else
                                '未知版本、Forge、不折叠版本，直接列入列表
                                Version.SwapType = VersionSwapState.NORMAL
                        End Select
                    End If

NextVersion:

                    '设置图标
                    If File.Exists(Version.Path & "PCL\Icon.png") Then
                        Try
                            Dim LogoTryLoad = New MyBitmap(Version.Path & "PCL\Icon.png")
                            Version.Logo = Version.Path & "PCL\Icon.png"
                        Catch ex As Exception
                            File.Delete(Version.Path & "PCL\Icon.png") '加载失败就删了图片，因为下一次还是会失败的……
                            GoTo CommonPicture
                        End Try
                    Else
CommonPicture:
                        Select Case Version.Type
                            Case MCVersionType.FORGE
                                Version.Logo = PATH_IMAGE & "Block-Anvil.png"
                            Case MCVersionType.OLD
                                Version.Logo = PATH_IMAGE & "Block-CobbleStone.png"
                            Case MCVersionType.SNAPSHOT
                                Version.Logo = PATH_IMAGE & "Block-CommandBlock.png"
                            Case MCVersionType.FOOL
                                Version.Logo = PATH_IMAGE & "Block-Dirt.png"
                            Case Else 'OptiFine、原版、未知
                                If Version.SwapType = VersionSwapState.WRONG Then
                                    Version.Logo = PATH_IMAGE & "Block-RedstoneBlock.png"
                                Else
                                    Version.Logo = PATH_IMAGE & "Block-Grass.png"
                                End If
                        End Select
                    End If

                    '设置描述文本
                    If Version.SwapType = VersionSwapState.WRONG Then
                        '错误描述文本
                        Select Case Version.VersionCheckResult
                            Case VersionCheckState.EMPTY_FOLDER
                                Version.Description = "空文件夹"
                            Case VersionCheckState.INHERITS_EXCEPTION
                                Version.Description = "依赖版本出错：" & Version.InheritVersion
                            Case VersionCheckState.INHERITS_NOT_EXIST
                                Version.Description = "依赖版本丢失：" & Version.InheritVersion
                            Case VersionCheckState.JAR_NOT_EXIST
                                Version.Description = "Jar 文件缺失"
                            Case VersionCheckState.JSON_CANT_READ
                                Version.Description = "Json 读取失败"
                            Case VersionCheckState.JSON_NOT_EXIST
                                Version.Description = "Json 文件缺失"
                            Case Else
                                Version.Description = "未知问题"
                        End Select
                    Else
                        '加载自定义文本
                        Version.Description = ReadIni(Version.Path & "PCL\Setup.ini", "Description")
                        If Version.Description = "" Then
                            '程序输出文本
                            Dim Description As String = If(Version.Type = MCVersionType.OPTIFINE, "OptiFine ", If(Version.Type = MCVersionType.FORGE, "Forge ", "")) & Version.Version
                            If Not Version.Name = Description And Not Description = "未知" Then Version.Description = Description
                        End If
                    End If

                End If

                VersionsList(Version.SwapType).Add(Version)
NextFolder:
            Next

            '判断是否需要重载列表次序

            If VersionsList(VersionSwapState.UNKNOWN).Count = 0 Then GoTo LoadEnd

            '计算最新的版本
            Dim LargestTimeVer As New MCVersion("") '最新的版本
            Dim LargestTime As ULong = 0 '最新的版本的时间
            Dim CurrentTime As ULong '当前的版本的时间
            Dim NewVersion As New Dictionary(Of String, MCVersion) '存储版本列表，键为“MainVersionCode-Type”
            For Each ver As MCVersion In VersionsList(VersionSwapState.UNKNOWN)
                CurrentTime = Val(ver.ReleaseTime.Replace("-", "").Replace(":", "").Replace("T", ""))
                '取最大时间
                If CurrentTime > LargestTime Then
                    LargestTime = CurrentTime
                    LargestTimeVer = ver
                End If
                'OptiFine与正式版判断
                If Not ver.Type = MCVersionType.SNAPSHOT Then
                    Dim CurrentVersion As New MCVersion("")
                    If NewVersion.TryGetValue(ver.MainVersionCode & "-" & ver.Type, CurrentVersion) Then
                        If CurrentTime > Val(CurrentVersion.ReleaseTime.Replace("-", "").Replace(":", "").Replace("T", "")) Then
                            NewVersion(ver.MainVersionCode & "-" & ver.Type) = ver
                        End If
                    Else
                        NewVersion.Add(ver.MainVersionCode & "-" & ver.Type, ver)
                    End If
                End If
            Next

            '如果最新的版本为快照，则添加入列表
            If LargestTimeVer.Type = MCVersionType.SNAPSHOT Then LargestTimeVer.SwapType = VersionSwapState.NORMAL

            '准备检查后的主版本列表
            Dim MainCodeList As New ArrayList '主版本列表
            For Each Key As String In NewVersion.Keys
                If Not MainCodeList.Contains(Integer.Parse(Key.Split("-")(0))) Then MainCodeList.Add(Integer.Parse(Key.Split("-")(0)))
            Next

            '把各个OptiFine和原版加入列表
            Dim WillAdd As New ArrayList
            For Each MainCode As Integer In MainCodeList
                If NewVersion.ContainsKey(MainCode & "-" & MCVersionType.OPTIFINE) Then
                    '存在OptiFine版本
                    WillAdd.Add(NewVersion(MainCode & "-" & MCVersionType.OPTIFINE))
                    If NewVersion.ContainsKey(MainCode & "-" & MCVersionType.RELEASE) Then
                        '如果原版比OptiFine版更新也显示原版
                        If Val(NewVersion(MainCode & "-" & MCVersionType.RELEASE).ReleaseTime.Replace("-", "").Replace(":", "").Replace("T", "")) > Val(NewVersion(MainCode & "-" & MCVersionType.OPTIFINE).ReleaseTime.Replace("-", "").Replace(":", "").Replace("T", "")) Then
                            WillAdd.Add(NewVersion(MainCode & "-" & MCVersionType.RELEASE))
                        End If
                    End If
                Else
                    '不存在OptiFine版本，检查原版版本
                    If NewVersion.ContainsKey(MainCode & "-" & MCVersionType.RELEASE) Then WillAdd.Add(NewVersion(MainCode & "-" & MCVersionType.RELEASE))
                End If
            Next

            '列表处理
            For Each Ver As MCVersion In WillAdd
                Ver.SwapType = VersionSwapState.NORMAL
            Next
            For Each Ver As MCVersion In VersionsList(VersionSwapState.UNKNOWN)
                If Ver.SwapType = VersionSwapState.UNKNOWN Then Ver.SwapType = VersionSwapState.SWAP
                VersionsList(Ver.SwapType).Add(Ver)
            Next

LoadEnd:
            VersionsList.Remove(VersionSwapState.UNKNOWN)

            '根据设置合并折叠与老版本
            If ReadIni("setup", "HomeVersionSwap", "True") = "False" Then
                VersionsList(VersionSwapState.NORMAL).AddRange(VersionsList(VersionSwapState.SWAP))
                VersionsList(VersionSwapState.SWAP).Clear()
            End If
            If ReadIni("setup", "HomeVersionOld", "True") = "False" Then
                VersionsList(VersionSwapState.NORMAL).AddRange(VersionsList(VersionSwapState.OLD))
                VersionsList(VersionSwapState.OLD).Clear()
            End If

            '保存文件 & 列表排序
            For Each List As ArrayList In VersionsList.Values
                '排序
                Dim Sorter As MCVersion() = ArrayConventer(Of MCVersion, ArrayList)(List)
                Array.Sort(Sorter)
                List.Clear()
                For Each Item In Sorter
                    If Not IsNothing(Item) Then List.Add(Item)
                Next
                '写入文件
                For Each Version As MCVersion In List
                    If Version.LoadedByFile = False Then
                        '写入PCL配置文件
                        Using iniWriter As New StreamWriter(Version.Path & "PCL\Version.ini", False, New UTF8Encoding(False))
                            iniWriter.WriteLine("VersionListCode:" & MC_VERSION_CODE)
                            iniWriter.WriteLine("SwapType:" & Version.SwapType)
                            iniWriter.WriteLine("Version:" & Version.Version)
                            iniWriter.WriteLine("ReleaseTime:" & Version.ReleaseTime)
                            iniWriter.WriteLine("MainVersionCode:" & Version.MainVersionCode)
                            iniWriter.WriteLine("Type:" & Version.Type)
                            iniWriter.WriteLine("Logo:" & Version.Logo)
                            iniWriter.WriteLine("Description:" & Version.Description)
                            iniWriter.WriteLine("VersionCheckResult:" & Version.VersionCheckResult)
                            iniWriter.WriteLine("InheritVersion:" & Version.InheritVersion)
                            iniWriter.Flush()
                        End Using
                    End If
                Next
            Next

            log("[Pool] 预加载版本列表结束")

        Catch ex As Exception

            ExShow(ex, "加载版本列表失败", ErrorLevel.MsgboxAndFeedback)
            '清空列表
            VersionsList = New Dictionary(Of VersionSwapState, ArrayList)

        Finally
            frmHomeRight.Dispatcher.Invoke(Sub() frmHomeRight.LoadVersionList())
        End Try
    End Sub

    ''' <summary>
    ''' 加载线程：刷新背景图片和顶部图片。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub PoolLoadPictures()
        Try

            If Not ReadIni("setup", "UiBackgroundURL", "") = "" Then
                log("[Pool] 刷新背景图片开始")
                DownloadFile(ReadIni("setup", "UiBackgroundURL", ""), PATH & "PCL\back.png" & DOWNLOADING_END)
                File.Delete(PATH & "PCL\back.png")
                FileSystem.Rename(PATH & "PCL\back.png" & DOWNLOADING_END, PATH & "PCL\back.png")
                log("[Pool] 刷新背景图片结束")
            End If

        Catch ex As Exception
            ExShow(ex, "刷新背景图片失败", ErrorLevel.AllUsers)
        End Try
        Try

            If Not ReadIni("setup", "UiBarURL", "") = "" Then
                log("[Pool] 刷新顶栏图片开始")
                DownloadFile(ReadIni("setup", "UiBarURL", ""), PATH & "PCL\top.png" & DOWNLOADING_END)
                File.Delete(PATH & "PCL\top.png")
                FileSystem.Rename(PATH & "PCL\top.png" & DOWNLOADING_END, PATH & "PCL\top.png")
                log("[Pool] 刷新顶栏图片结束")
            End If

        Catch ex As Exception
            ExShow(ex, "刷新顶栏图片失败", ErrorLevel.AllUsers)
        End Try
        frmMain.Dispatcher.Invoke(New RefreshThemeInvoke(AddressOf RefreshTheme))
    End Sub

    Private PoolPushCount As Integer = 0
    ''' <summary>
    ''' 加载线程：首页。
    ''' </summary>
    ''' <remarks></remarks>
    Public Class PushSource
        Public Name As String
        Public URL As String
        Public Introduce As String
        Public IsEnabled As String
        Public LocalPath As String
    End Class
    Public Sub PoolPush()
        If MODE_OFFLINE Then frmStart.IsPushLoading = False : Exit Sub

        '加载特殊处理的第三方推荐
        If ReadIni("setup", "HomeMCBBSPush", "True") = "True" Then Dim th As New Thread(AddressOf PoolMainMCBBS) : th.Priority = ThreadPriority.BelowNormal : th.Start() : PoolPushCount = PoolPushCount + 1
        If ReadIni("setup", "HomeTbPush", "True") = "True" Then Dim th As New Thread(AddressOf PoolMainTb) : th.Priority = ThreadPriority.BelowNormal : th.Start() : PoolPushCount = PoolPushCount + 1
        If ReadIni("setup", "HomeMojangPush", "True") = "True" Then Dim th As New Thread(AddressOf PoolMainMojang) : th.Priority = ThreadPriority.BelowNormal : th.Start() : PoolPushCount = PoolPushCount + 1
        If ReadIni("setup", "HomeForumPush", "False") = "True" Then Dim th As New Thread(AddressOf PoolMainForum) : th.Priority = ThreadPriority.BelowNormal : th.Start() : PoolPushCount = PoolPushCount + 1

        '加载默认推荐
        Dim AllLocals As New ArrayList
        If ReadIni("setup", "HomePCLPush", "True") = "True" Then AllLocals.Add(New PushSource With {.Name = "PCL 推荐", .URL = TX_SREVER_1 & "push.ini", .IsEnabled = True, .Introduce = "PCL 官方的优质内容推荐（不含广告请放心食用）。"})

        '加载每个推荐
        Dim Count As Integer = 0
        For Each Push As PushSource In AllLocals
            If Push.IsEnabled Then
                PoolPushCount = PoolPushCount + 1
                Dim Source As PushSource = Push
                Dim thurl As New Thread(Sub() PoolMainPushURL(Source))
                thurl.Start()
                Count = Count + 1
            End If
        Next

        If PoolPushCount = 0 Then frmStart.IsPushLoading = False
        log("[Pool-HomeLeft] 订阅信息加载完成")
    End Sub

#Region "推荐源"

    ''' <summary>
    ''' 贴吧源。
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub PoolMainTb()
        log("[Pool-HomeLeft] 加载推荐源：Minecraft 吧精品")
        If File.Exists(PATH & "PCL\cache\source\CA28E736032912D2801529958AB8937EBB351E2338212FA49529\cache.ini") Then
            '如果缓存存在，读取->(下载->处理)
            PoolMainTbLoad()
            PoolPushCount = PoolPushCount - 1
            If PoolPushCount = 0 Then frmStart.IsPushLoading = False
            Thread.Sleep(5000)
            PoolMainTbDownload()
        Else
            '如果缓存不存在，(下载->处理)->读取
            If PoolMainTbDownload() Then PoolMainTbLoad()
            PoolPushCount = PoolPushCount - 1
            If PoolPushCount = 0 Then frmStart.IsPushLoading = False
        End If
    End Sub
    Private Function PoolMainTbDownload() As Boolean

        '————————————
        '下载
        '————————————

        Dim SourceCode As String
        Try
            SourceCode = GetWebsiteCode("https://tieba.baidu.com/f?kw=minecraft&ie=utf-8&tab=good", New UTF8Encoding(False))
            If SourceCode.Length < 100000 Then Throw New Exception("获取的代码长度不足：" & SourceCode)
        Catch ex As Exception
            ExShow(ex, "下载推荐源失败（Minecraft 吧精品）")
            Return False
        End Try

        '————————————
        '处理推荐
        '————————————

        Dim AllImages As New ArrayList
        Dim AllLinks As New ArrayList
        Dim AllTexts As New ArrayList

        Try
            '预处理范围
            SourceCode = Mid(SourceCode, SourceCode.IndexOf("""thread_list"""))
            SourceCode = Mid(SourceCode, 1, SourceCode.IndexOf("thread_list_bottom"))
            Dim Threads As ArrayList = RegexSearch(SourceCode, "col2_right[\w\W]+?author_name")
            Dim i As Integer = 0
            For Count As Integer = 0 To 5

                '每个帖子
                AllLinks.Add("https://tieba.baidu.com" & RegexSearch(Threads(i), "/p/[^""]+")(0))
                Threads(i) = Threads(i).ToString.Substring(Threads(i).ToString.IndexOf("a href="))
                AllTexts.Add(RegexSearch(Threads(i), "(?<=title="")[^""]+")(0))
                Dim ImageTry = RegexSearch(Threads(i), "(?<=data-original="")[^""]+")
                AllImages.Add(If(ImageTry.Count > 0, ImageTry(0), ""))

                i = i + RandomInteger(1, 3)
                If i > Threads.Count - 1 Then Exit For
            Next
        Catch ex As Exception
            ExShow(ex, "预处理推荐源失败（Minecraft 吧精品）")
            Return False
        End Try

        '————————————
        '项目过滤
        '————————————

        Dim Sources As New ArrayList
        Try
            For i = 0 To AllTexts.Count - 1
                '检测是否为其它启动器广告
                If Not (AllTexts(i).Contains("启动器") Or AllTexts(i).Contains("吧") Or AllTexts(i).Contains("水楼") Or AllTexts(i).Contains("赛") Or AllTexts(i).Contains("招聘") Or AllTexts(i).Contains("联机") Or AllTexts(i).Contains("服务器") Or AllTexts(i).Contains("HMCL") Or AllTexts(i).Contains("Launcher") Or AllTexts(i).Contains("Baka")) Then
                    Sources.Add(New InfoBoxSource With {
                                .OnClickURL = AllLinks(i),
                                .Title = AllTexts(i),
                                .Source = "Minecraft 吧精品",
                                .PictureName = AllImages(i),
                                .Type = ""})
                End If
            Next

            '————————————
            '图片种类、纯文本处理
            '————————————

            For Each Source As InfoBoxSource In Sources
                GetSourceType(Source)
                Dim regex As New RegularExpressions.Regex("\[[0-9]{2,4}-[0-9]{1,2}-[0-9]{1,2}\]")
                Source.Title = regex.Replace(Source.Title, "")
                If RegexSearch(Source.Title, "【[^0]{2,4}】").Count > 0 Then
                    If Source.Title.StartsWith(RegexSearch(Source.Title, "【[^0]{2,4}】")(0)) Then Source.Title = Source.Title.Replace(RegexSearch(Source.Title, "【[^0]{2,4}】")(0), "")
                ElseIf RegexSearch(Source.Title, "\[[^0]{2,4}\]").Count > 0 Then
                    If Source.Title.StartsWith(RegexSearch(Source.Title, "\[[^0]{2,4}\]")(0)) Then Source.Title = Source.Title.Replace(RegexSearch(Source.Title, "\[[^0]{2,4}\]")(0), "")
                End If
            Next
        Catch ex As Exception
            ExShow(ex, "处理推荐源失败（Minecraft 吧精品）")
            Return False
        End Try

        '————————————
        '输出
        '————————————

        Dim OutputIni As String = "Version:" & PUSH_VERSION_CODE & vbCrLf
        For i = 1 To Sources.Count
            Dim Source As InfoBoxSource = Sources(i - 1)
            OutputIni = OutputIni & vbCrLf
            OutputIni = OutputIni & "Picture" & i & ":" & Source.PictureName & vbCrLf
            OutputIni = OutputIni & "Title" & i & ":" & Source.Title & vbCrLf
            OutputIni = OutputIni & "Link" & i & ":" & Source.OnClickURL & vbCrLf
            OutputIni = OutputIni & "Type" & i & ":" & Source.Type & vbCrLf
        Next
        WriteFile("PCL\cache\source\CA28E736032912D2801529958AB8937EBB351E2338212FA49529\cache.ini", OutputIni, isFullPath:=False)

        Return True
    End Function
    Private Sub PoolMainTbLoad()
        PoolMainPushURLLoad(New PushSource With {
                            .LocalPath = PATH & "PCL\cache\source\CA28E736032912D2801529958AB8937EBB351E2338212FA49529\cache.ini",
                            .Name = "Minecraft 吧精品"})
    End Sub

    ''' <summary>
    ''' Mojang 源。
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub PoolMainMojang()
        log("[Pool-HomeLeft] 加载推荐源：Mojang 官方")
        If File.Exists(PATH & "PCL\cache\source\A407DF2F142912192C15239087B89C1BBB82\cache.ini") Then
            PoolMainMojangLoad()
            PoolPushCount = PoolPushCount - 1 : If PoolPushCount = 0 Then frmStart.IsPushLoading = False
            Thread.Sleep(5000)
            PoolMainMojangDownload()
        Else
            '如果缓存不存在，(下载->处理)->读取
            If PoolMainMojangDownload() Then PoolMainMojangLoad()
            PoolPushCount = PoolPushCount - 1 : If PoolPushCount = 0 Then frmStart.IsPushLoading = False
        End If
    End Sub
    Private Function PoolMainMojangDownload() As Boolean

        '————————————
        '下载
        '————————————

        Dim SourceCode As String
        Try
            SourceCode = GetWebsiteCode("http://www.mcbbs.net/portal.php", New UTF8Encoding(False))
            If SourceCode.Length < 10000 Then Throw New Exception("获取的代码长度不足：" & SourceCode)
        Catch ex As Exception
            ExShow(ex, "下载推荐源失败（Mojang）")
            Return False
        End Try

        '————————————
        '处理推荐
        '————————————

        Dim AllImages As New ArrayList
        Dim AllLinks As New ArrayList
        Dim AllTexts As New ArrayList
        Dim AllTypes As New ArrayList

        '————————————
        '加载
        '————————————

        Try
            '去头尾
            SourceCode = Mid(SourceCode, SourceCode.IndexOf("portal_zb"))
            SourceCode = Mid(SourceCode, 1, SourceCode.IndexOf("titletext"))
            '获取文本
            AllImages = RegexSearch(SourceCode, "(?<=<img src="")[^""]+")
            AllLinks = RegexSearch(SourceCode, "(?<=a href="")[^""]+")
            AllTexts = RegexSearch(SourceCode, "(?<=;"">)[^<]+")
            For i = 0 To AllTexts.Count - 1
                AllTypes.Add("")
            Next
        Catch ex As Exception
            ExShow(ex, "处理推荐源失败（Mojang）")
            AllImages.Clear()
            AllLinks.Clear()
            AllTexts.Clear()
            AllTypes.Clear()
        End Try

        '————————————
        'URL 二次处理及项目过滤
        '————————————

        Dim Sources As New ArrayList
        Try
            For i = 0 To AllLinks.Count - 1
                If RegexSearch(AllLinks(i), "[0-9]{5,7}").Count = 0 Then
                    AllLinks(i) = ""
                Else
                    AllLinks(i) = RegexSearch(AllLinks(i), "[0-9]{5,7}")(0)
                End If
            Next
            Dim NoDoubledArray As ArrayList = ArrayNoDouble(AllLinks.ToArray)
            For Each NoDoubledURL As String In NoDoubledArray
                Dim Index As Integer = AllLinks.IndexOf(NoDoubledURL)
                If Not Index >= AllTexts.Count Then
                    '检测是否为其它启动器广告
                    If Not (AllTexts(Index).Contains("启动器") Or AllTexts(Index).Contains("赛") Or AllTexts(Index).Contains("招聘") Or AllTexts(Index).Contains("联机") Or AllTexts(Index).Contains("服务器") Or AllTexts(Index).Contains("HMCL") Or AllTexts(Index).Contains("Launcher") Or AllTexts(Index).Contains("Baka") Or NoDoubledURL = "") Then
                        Sources.Add(New InfoBoxSource With {.OnClickURL = "http://www.mcbbs.net/thread-" & NoDoubledURL & "-1-1.html", .Title = AllTexts(Index), .Source = "Mojang 官方", .PictureName = AllImages(Index), .Type = AllTypes(Index)})
                    End If
                End If
            Next

            '————————————
            '图片种类、纯文本处理
            '————————————

            For Each Source As InfoBoxSource In Sources
                Dim Title2 = Source.Title
                GetSourceType(Source)
                Source.Title = Title2
            Next
        Catch ex As Exception
            ExShow(ex, "处理推荐源失败（Mojang）")
            Return False
        End Try

        '————————————
        '输出
        '————————————

        Dim OutputIni As String = "Version:" & PUSH_VERSION_CODE & vbCrLf
        Dim IsWeekly As Boolean = False '每周的只显示一个
        Dim OutI As Integer = 1 '用于输出的 i
        For i = 1 To Sources.Count
            If Sources(i - 1).Title.Contains("周") Then
                If IsWeekly Then
                    GoTo NextSource
                Else
                    IsWeekly = True
                End If
            End If
            Dim Source As InfoBoxSource = Sources(i - 1)
            OutputIni = OutputIni & vbCrLf
            OutputIni = OutputIni & "Picture" & OutI & ":" & Source.PictureName & vbCrLf
            OutputIni = OutputIni & "Title" & OutI & ":" & Source.Title & vbCrLf
            OutputIni = OutputIni & "Link" & OutI & ":" & Source.OnClickURL & vbCrLf
            OutputIni = OutputIni & "Type" & OutI & ":" & Source.Type & vbCrLf
            OutI = OutI + 1
NextSource:
        Next
        WriteFile("PCL\cache\source\A407DF2F142912192C15239087B89C1BBB82\cache.ini", OutputIni, isFullPath:=False)

        Return True
    End Function
    Private Sub PoolMainMojangLoad()
        PoolMainPushURLLoad(New PushSource With {
                            .LocalPath = PATH & "PCL\cache\source\A407DF2F142912192C15239087B89C1BBB82\cache.ini",
                            .Name = "Mojang 官方"})
    End Sub

    ''' <summary>
    ''' Forum 源。
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub PoolMainForum()
        log("[Pool-HomeLeft] 加载推荐源：Minecraft Forum")
        If File.Exists(PATH & "PCL\cache\source\" & Left(SerAdd("Minecraft Forum"), 100) & "\cache.ini") Then
            PoolMainForumLoad()
            PoolPushCount = PoolPushCount - 1 : If PoolPushCount = 0 Then frmStart.IsPushLoading = False
            Thread.Sleep(5000)
            PoolMainForumDownload()
        Else
            '如果缓存不存在，(下载->处理)->读取
            If PoolMainForumDownload() Then PoolMainForumLoad()
            PoolPushCount = PoolPushCount - 1 : If PoolPushCount = 0 Then frmStart.IsPushLoading = False
        End If
    End Sub
    Private Function PoolMainForumDownload() As Boolean

        '————————————
        '下载
        '————————————

        Dim SourceCode As String
        Try
            SourceCode = GetWebsiteCode("http://www.minecraftforum.net/", New UTF8Encoding(False))
            If SourceCode.Length < 500 Then Throw New Exception("获取的代码长度不足：" & SourceCode)
        Catch ex As Exception
            ExShow(ex, "下载推荐源失败（Minecraft Forum）")
            Return False
        End Try

        '————————————
        '处理推荐
        '————————————

        Dim AllLinks As New ArrayList
        Dim AllTexts As New ArrayList
        Dim AllImages As New ArrayList

        Try
            Dim Threads As ArrayList = RegexSearch(SourceCode, "<article>[\w\W]+?</article>")
            Dim i As Integer = 0
            For Count As Integer = 0 To 2

                '每条Article
                AllLinks.Add(RegexSearch(Threads(i), "http://www.minecraftforum.net/[^""]+")(0))
                AllTexts.Add(RegexSearch(Threads(i), "(?<=<a href[^>]+>)[^<]{5,100}")(0))
                AllImages.Add(RegexSearch(Threads(i), "(?<=<img src="")[^""]+")(0))

                i = i + RandomInteger(1, 2)
                If i > Threads.Count - 1 Then Exit For
            Next
        Catch ex As Exception
            ExShow(ex, "预处理推荐源失败（Minecraft Forum）")
            Return False
        End Try

        '————————————
        '项目过滤
        '————————————

        Dim Sources As New ArrayList
        Try
            For i = 0 To AllTexts.Count - 1
                If AllTexts(i).ToString.StartsWith(" ") Then AllTexts(i) = Right(AllTexts(i), Len(AllTexts(i)) - 1)
                Sources.Add(New InfoBoxSource With {
                                .OnClickURL = AllLinks(i),
                                .Title = AllTexts(i),
                                .Source = "Minecraft Forum",
                                .PictureName = AllImages(i),
                                .Type = ""})
                GetSourceType(Sources(Sources.Count - 1))
            Next
        Catch ex As Exception
            ExShow(ex, "处理推荐源失败（Minecraft Forum）")
            Return False
        End Try

        '————————————
        '输出
        '————————————

        Dim OutputIni As String = "Version:" & PUSH_VERSION_CODE & vbCrLf
        For i = 1 To Sources.Count
            Dim Source As InfoBoxSource = Sources(i - 1)
            OutputIni = OutputIni & vbCrLf
            OutputIni = OutputIni & "Picture" & i & ":" & Source.PictureName & vbCrLf
            OutputIni = OutputIni & "Title" & i & ":" & Source.Title & vbCrLf
            OutputIni = OutputIni & "Link" & i & ":" & Source.OnClickURL & vbCrLf
            OutputIni = OutputIni & "Type" & i & ":" & Source.Type & vbCrLf
        Next
        WriteFile("PCL\cache\source\" & Left(SerAdd("Minecraft Forum"), 100) & "\cache.ini", OutputIni, isFullPath:=False)

        Return True
    End Function
    Private Sub PoolMainForumLoad()
        PoolMainPushURLLoad(New PushSource With {
                            .LocalPath = PATH & "PCL\cache\source\" & Left(SerAdd("Minecraft Forum"), 100) & "\cache.ini",
                            .Name = "Minecraft Forum"})
    End Sub

    ''' <summary>
    ''' MCBBS 源。
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub PoolMainMCBBS()
        log("[Pool-HomeLeft] 加载推荐源：我的世界中文论坛")
        If File.Exists(PATH & "PCL\cache\source\C6378196067760DF918A0145291AB651\cache.ini") Then
            PoolMainMCBBSLoad()
            PoolPushCount = PoolPushCount - 1 : If PoolPushCount = 0 Then frmStart.IsPushLoading = False
            Thread.Sleep(5000)
            PoolMainMCBBSDownload()
        Else
            '如果缓存不存在，(下载->处理)->读取
            If PoolMainMCBBSDownload() Then PoolMainMCBBSLoad()
            PoolPushCount = PoolPushCount - 1 : If PoolPushCount = 0 Then frmStart.IsPushLoading = False
        End If
    End Sub
    Private Function PoolMainMCBBSDownload() As Boolean

        '————————————
        '下载
        '————————————

        Dim SourceCode As String
        Try
            SourceCode = GetWebsiteCode("http://www.mcbbs.net/forum.php", New UTF8Encoding(False))
            If SourceCode.Length < 10000 Then Throw New Exception("获取的代码长度不足：" & SourceCode)
        Catch ex As Exception
            ExShow(ex, "下载推荐源失败（我的世界中文论坛）")
            Return False
        End Try

        '————————————
        '处理推荐
        '————————————

        Dim AllImages As New ArrayList
        Dim AllLinks As New ArrayList
        Dim AllTexts As New ArrayList
        Dim AllTypes As New ArrayList

        '————————————
        '首页图片推荐
        '————————————

        Try
            SourceCode = Mid(SourceCode, SourceCode.IndexOf("var files = '")).Replace(" var files = '", "")
            AllImages.AddRange(Mid(SourceCode, 1, SourceCode.IndexOf("';")).Split("|"))
            SourceCode = Mid(SourceCode, SourceCode.IndexOf("var links = '")).Replace(" var links = '", "")
            AllLinks.AddRange(Mid(SourceCode, 1, SourceCode.IndexOf("';")).Split("|"))
            SourceCode = Mid(SourceCode, SourceCode.IndexOf("var texts = '")).Replace(" var texts = '", "")
            AllTexts.AddRange(Mid(SourceCode, 1, SourceCode.IndexOf("';")).Split("|"))
            For i = 0 To AllTexts.Count - 1
                AllTypes.Add("")
            Next
        Catch ex As Exception
            ExShow(ex, "处理推荐源失败（我的世界中文论坛/图片推荐）")
            AllImages.Clear()
            AllLinks.Clear()
            AllTexts.Clear()
            AllTypes.Clear()
        End Try

        '————————————
        '新闻资讯与最新精华
        '————————————

        Try
            '最新精华
            SourceCode = Mid(SourceCode, SourceCode.IndexOf("div id=""kk_xshow_cont_3"""))
            Dim TextGood As String = Mid(SourceCode, 1, SourceCode.IndexOf("div id=""kk_xshow_cont_4"""))
            Dim GoodLinks As New ArrayList(RegexSearch(TextGood, "(?<=a href="")[^""]+[0-9]{4,7}[^""]+(?="")"))
            Dim GoodTexts As New ArrayList(RegexSearch(TextGood, "(?<=标题: )[^\n]+"))
            For i = 0 To Math.Min(GoodLinks.Count - 1, 6) Step 0
                AllLinks.Add(GoodLinks(i))
                AllTexts.Add(GoodTexts(i))
                AllImages.Add("")
                AllTypes.Add("文本推荐")
                i = i + RandomInteger(1, 3)
            Next

            '新闻资讯
            SourceCode = Mid(SourceCode, SourceCode.IndexOf("div id=""diy_right"))
            Dim SourceString = RegexSearch(SourceCode, "(?<=a href="")[^""]*[0-9]{5,7}[^""]*"" title=""[^""""]+(?="")")(0)
            '处理源字符串
            AllLinks.Add(Mid(SourceString, 1, SourceString.IndexOf(""" title=""")))
            AllTexts.Add(Mid(SourceString, 10 + SourceString.IndexOf(""" title=""")))
            AllImages.Add("")
            AllTypes.Add("新闻")
        Catch ex As Exception
            ExShow(ex, "处理推荐源失败（我的世界中文论坛/文本推荐）")
        End Try

        '————————————
        'URL 二次处理及项目过滤
        '————————————

        Dim Sources As New ArrayList
        Try
            For i = 0 To AllLinks.Count - 1
                If RegexSearch(AllLinks(i), "[0-9]{5,7}").Count = 0 Then
                    AllLinks(i) = ""
                Else
                    AllLinks(i) = RegexSearch(AllLinks(i), "[0-9]{5,7}")(0)
                End If
            Next
            Dim NoDoubledArray As ArrayList = ArrayNoDouble(AllLinks.ToArray)
            For Each NoDoubledURL As String In NoDoubledArray
                Dim Index As Integer = AllLinks.IndexOf(NoDoubledURL)
                If Not Index >= AllTexts.Count Then
                    '检测是否为其它启动器广告
                    If Not (AllTexts(Index).Contains("启动器") Or AllTexts(Index).Contains("赛") Or AllTexts(Index).Contains("招聘") Or AllTexts(Index).Contains("联机") Or AllTexts(Index).Contains("服务器") Or AllTexts(Index).Contains("HMCL") Or AllTexts(Index).Contains("Launcher") Or AllTexts(Index).Contains("Baka") Or NoDoubledURL = "") Then
                        Sources.Add(New InfoBoxSource With {.OnClickURL = "http://www.mcbbs.net/thread-" & NoDoubledURL & "-1-1.html", .Title = AllTexts(Index), .Source = "我的世界中文论坛", .PictureName = AllImages(Index), .Type = AllTypes(Index)})
                    End If
                End If
            Next

            '————————————
            '图片种类、纯文本处理
            '————————————

            For Each Source As InfoBoxSource In Sources
                Dim Title2 = Source.Title
                GetSourceType(Source)
                Source.Title = Title2
            Next
        Catch ex As Exception
            ExShow(ex, "处理推荐源失败（我的世界中文论坛）")
            Return False
        End Try

        '————————————
        '输出
        '————————————

        Dim OutputIni As String = "Version:" & PUSH_VERSION_CODE & vbCrLf
        For i = 1 To Sources.Count
            Dim Source As InfoBoxSource = Sources(i - 1)
            OutputIni = OutputIni & vbCrLf
            OutputIni = OutputIni & "Picture" & i & ":" & Source.PictureName & vbCrLf
            OutputIni = OutputIni & "Title" & i & ":" & Source.Title & vbCrLf
            OutputIni = OutputIni & "Link" & i & ":" & Source.OnClickURL & vbCrLf
            OutputIni = OutputIni & "Type" & i & ":" & Source.Type & vbCrLf
        Next
        WriteFile("PCL\cache\source\C6378196067760DF918A0145291AB651\cache.ini", OutputIni, isFullPath:=False)

        Return True
    End Function
    Private Sub PoolMainMCBBSLoad()
        PoolMainPushURLLoad(New PushSource With {
                            .LocalPath = PATH & "PCL\cache\source\C6378196067760DF918A0145291AB651\cache.ini",
                            .Name = "我的世界中文论坛"})
        Thread.Sleep(2000)
    End Sub

    Public IsHaveAd As Boolean = False
    ''' <summary>
    ''' ini 源。
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub PoolMainPushURL(ByVal Source As PushSource)
        log("[Pool-HomeLeft] 加载推荐源：" & Source.Name)
        Source.LocalPath = PATH & "PCL\cache\source\" & Left(SerAdd(Source.Name), 100) & "\cache.ini"
        If File.Exists(Source.LocalPath) Then
            '如果缓存存在，读取->下载
            PoolMainPushURLLoad(Source)
            PoolPushCount = PoolPushCount - 1 : If PoolPushCount = 0 Then frmStart.IsPushLoading = False
            Thread.Sleep(5000)
            PoolMainPushURLDownload(Source)
        Else
            '如果缓存不存在，(下载->处理)->读取
            If PoolMainPushURLDownload(Source) Then PoolMainPushURLLoad(Source)
            PoolPushCount = PoolPushCount - 1 : If PoolPushCount = 0 Then frmStart.IsPushLoading = False
        End If
    End Sub
    Private Function PoolMainPushURLDownload(ByVal Source As PushSource) As Boolean

        If DownloadFile(Source.URL, Source.LocalPath) Then
            If Val(ReadIni(Source.LocalPath, "Version", "0")) < PUSH_VERSION_CODE Then
                log("[Pool-HomeLeft] 推荐源版本过老（" & Source.Name & "）：目前为 " & ReadIni(Source.LocalPath, "Version", "0") & "，要求为 " & PUSH_VERSION_CODE, True)
                Return False
            End If
            Return True
        Else
            log("[Pool-HomeLeft] 下载推荐源失败：" & Source.Name, True)
            Return False
        End If

    End Function
    Private Sub PoolMainPushURLLoad(ByVal Source As PushSource)

        '版本检查
        If Val(ReadIni(Source.LocalPath, "Version", "0")) < PUSH_VERSION_CODE Then
            log("[Pool-HomeLeft] 推荐源版本过老（" & Source.Name & "）：目前为 " & ReadIni(Source.LocalPath, "Version", "0") & "，要求为 " & PUSH_VERSION_CODE, True)
            Exit Sub
        End If

        '处理代码
        Try
            Dim CanLoadItem As New ArrayList
            Dim ii As Integer = 1
            Do Until ReadIni(Source.LocalPath, "Title" & ii) = ""
                '过滤
                Dim Context As String = ReadIni(Source.LocalPath, "Title" & ii)
                If Context.Contains("Plain Craft Launcher") Or Context.Contains("PCL") Or Not (Context.Contains("启动器") Or Context.Contains("HMCL") Or Context.Contains("Launcher") Or Context.Contains("Baka") Or ReadIni(Source.LocalPath, "Link" & ii).Contains("twitter")) Then
                    Dim InfoSource As New InfoBoxSource With {
                        .OnClickURL = ReadIni(Source.LocalPath, "Link" & ii),
                        .PictureName = ReadIni(Source.LocalPath, "Picture" & ii),
                        .Title = Context,
                        .Source = Source.Name,
                        .Type = ReadIni(Source.LocalPath, "Type" & ii)}
                    '纯文本订阅只保留一半
                    If Not InfoSource.PictureName.Contains("://") Then
                        If ReadIni("setup", "HomeHide", "True") = "True" And Not RandomInteger(0, 1) = 1 Then GoTo NextItem
                    End If
                    frmHomeLeft.LoadingItem.Add(InfoSource)
                    InfoSource.Load()
                End If
NextItem:
                ii = ii + 1
            Loop

        Catch ex As Exception
            log("[Pool-HomeLeft] 处理推荐源失败（" & Source.Name & "）：" & GetStringFromException(ex, True), True)
        End Try

    End Sub

    Private Sub GetSourceType(ByRef Source As InfoBoxSource)
        If Source.Type = "新闻" Then
        ElseIf Source.Title.Contains("教程") Or Source.Title.Contains("新手") Or Source.Title.Contains("技巧") Or Source.Title.ToLower.Contains("wiki") Or Source.Title.Contains("翻译") Then
            Source.Type = "教程"
        ElseIf Source.Title.Contains("编辑器") Or Source.Title.Contains("软件") Or Source.Title.Contains("工具") Or Source.Title.Contains("生成器") Then
            Source.Type = "软件"
        ElseIf Source.Title.Contains("联机") Or Source.Title.Contains("插件") Or Source.Title.Contains("服务端") Or Source.Title.Contains("开服") Or Source.Title.Contains("服务器") Then
            Source.Type = "多人"
        ElseIf Source.Title.Contains("皮肤") Then
            Source.Type = "皮肤"
        ElseIf Source.Title.ToLower.Contains("mod") Then
            Source.Type = "Mod"
        ElseIf Source.Title.Contains("解密") Or Source.Title.Contains("逃") Or Source.Title.Contains("跑酷") Or Source.Title.Contains("空岛") Or Source.Title.Contains("RPG") Or Source.Title.Contains("生存") Or Source.Title.Contains("小游戏") Or Source.Title.Contains("CB") Or Source.Title.Contains("红石") Or Source.Title.Contains("闯关") Or Source.Title.Contains("PVP") Or Source.Title.Contains("PVE") Or Source.Title.Contains("命令") Or Source.Title.Contains("战") Or Source.Title.Contains("冒险") Then
            Source.Type = "地图"
        ElseIf Source.Title.Contains("创作") Or Source.Title.Contains("小说") Or Source.Title.Contains("漫画") Or Source.Title.Contains("物语") Or Source.Title.Contains("插画") Or Source.Title.Contains("短篇") Or Source.Title.Contains("长篇") Then
            Source.Type = "创作"
        ElseIf Source.Title.Contains("材质") Or Source.Title.Contains("资源包") Then
            Source.Type = "资源包"
        ElseIf Source.Title.Contains("整合") Or Source.Title.Contains("懒人包") Then
            Source.Type = "懒人包"
        ElseIf Source.Title.Contains("阁") Or Source.Title.Contains("航母") Or Source.Title.Contains("港") Or Source.Title.Contains("宫") Or Source.Title.Contains("上古之石") Or Source.Title.Contains("楼") Or Source.Title.Contains("城") Or Source.Title.Contains("镇") Or Source.Title.Contains("殿") Or Source.Title.Contains("厅") Or Source.Title.Contains("桥") Or Source.Title.Contains("陵") Or Source.Title.Contains("园") Or Source.Title.Contains("宅") Or Source.Title.Contains("街") Or Source.Title.Contains("筑") Or Source.Title.Contains("山") Or Source.Title.Contains("景") Or Source.Title.ToLower.Contains("city") Then
            Source.Type = "建筑"
        ElseIf Source.Title.Contains("地图") Or Source.Title.Contains("作品") Then
            Source.Type = "地图"
        ElseIf Source.Source = "Minecraft Forum" Or Source.Title.Contains("发布") Or Source.Title.Contains("eleas") Then
            Source.Type = "新闻"
        Else
            Source.Type = ""
        End If
    End Sub

#End Region

#End Region

#Region "自动安装"

    ''' <summary>
    ''' 自动安装文件或文件夹。
    ''' </summary>
    ''' <param name="Path">文件或文件夹完整路径。文件夹不以“\”结尾。</param>
    ''' <remarks></remarks>
    Public Sub AutoSetup(ByVal Path As String)
        log("[System] 安装文件请求：" & Path)
        Try

            '基础检测
            If Directory.Exists(Path) Then
                '文件夹
                Path = Path & "\"
                Dim a = Path & GetFileNameFromPath(Mid(Path, 1, Len(Path) - 1)) & ".jar"
ReCheckDir:
                If File.Exists(Path & "level.dat") And Directory.Exists(Path & "region") Then
                    '存档文件夹
                    Dim th As New Thread(AddressOf AutoSetupSaveFolder)
                    th.Start(Path)
                ElseIf File.Exists(Path & "pack.mcmeta") And Directory.Exists(Path & "assets") Then
                    '资源包文件夹
                    Dim th As New Thread(AddressOf AutoSetupResFolder)
                    th.Start(Path)
                ElseIf File.Exists(Path & GetFileNameFromPath(Mid(Path, 1, Len(Path) - 1)) & ".jar") And File.Exists(Path & GetFileNameFromPath(Mid(Path, 1, Len(Path) - 1)) & ".json") Then
                    '版本文件夹
                    Dim th As New Thread(AddressOf AutoSetupVersionFolder)
                    th.Start(Path)
                Else
                    '进一层鉴别
                    Dim AllDir As String() = Directory.GetDirectories(Path)
                    If AllDir.Length = 1 Then
                        '发现进一层文件夹
                        Path = AllDir(0) & "\"
                        log("[System] 进一层的文件夹：" & Path)
                        GoTo ReCheckDir
                    Else
                        '没有进一层文件夹
                        ShowHint(New HintConverter("自动安装失败：不支持的文件夹", HintState.Warn))
                        Exit Sub
                    End If
                End If
            ElseIf File.Exists(Path) Then
                '文件
                '获取后缀
                Dim SplitResult As String() = Path.Split(".")
                Dim EndDot As String = If(SplitResult.Length > 1, SplitResult(SplitResult.Length - 1), "")
                '判断后缀
                If EndDot = "zip" Then
                    'zip处理线程
                    Dim th As New Thread(Sub()
                                             Try
                                                 Using zip As New ZipFile(Path, Encoding.Default)
                                                     Dim BasePath As String = "" '基础路径，用于一层嵌套的文件
ReCheckZip:
                                                     '检查是否为版本文件夹
                                                     Dim LastPath As String
                                                     If BasePath = "" Then
                                                         LastPath = GetFileNameFromPath(Path).Replace(".zip", "")
                                                     Else
                                                         LastPath = Mid(BasePath, 1, Len(BasePath) - 1).Split("/")(Mid(BasePath, 1, Len(BasePath) - 1).Split("/").Length - 1)
                                                     End If
                                                     log("[System] 末端文件夹名：" & LastPath)

                                                     '文件检查
                                                     If zip.ContainsEntry(BasePath & "level.dat") And zip.ContainsEntry(BasePath & "region/") Then
                                                         '存档zip
                                                         AutoSetupSaveZip(zip, BasePath, Path)
                                                     ElseIf zip.ContainsEntry(BasePath & "pack.mcmeta") And zip.ContainsEntry(BasePath & "assets/") Then
                                                         '资源包zip
                                                         If BasePath = "" Then
                                                             AutoSetupResZip(Path)
                                                         Else
                                                             ShowHint(New HintConverter("自动安装失败：资源包压缩包不支持内置文件夹", HintState.Warn))
                                                             Exit Sub
                                                         End If
                                                     ElseIf zip.ContainsEntry(BasePath & LastPath & ".jar") And zip.ContainsEntry(BasePath & LastPath & ".json") Then
                                                         '版本zip
                                                         AutoSetupVersionZip(zip, BasePath, Path)
                                                     Else
                                                         '进一层鉴别
                                                         Dim AllDir As New ArrayList
                                                         For Each Entry As String In zip.EntryFileNames
                                                             If Entry.IndexOf("/") = Len(Entry) - 1 Then AllDir.Add(Entry)
                                                         Next
                                                         If AllDir.Count = 1 Then
                                                             BasePath = AllDir(0)
                                                             log("[System] 进一层的压缩文件路径：" & BasePath)
                                                             GoTo ReCheckZip
                                                         Else
                                                             ShowHint(New HintConverter("自动安装失败：不支持的文件", HintState.Warn))
                                                             Exit Sub
                                                         End If
                                                     End If
                                                 End Using
                                             Catch ex As Exception
                                                 ShowHint(New HintConverter("自动安装失败：" & GetStringFromException(ex), HintState.Warn))
                                             End Try
                                         End Sub)
                    th.Start()
                ElseIf EndDot = "json" Or EndDot = "jar" Then
                    '版本文件
                    AutoSetupVersionFile(Path, EndDot)
                ElseIf EndDot = "rar" Or EndDot = "7z" Then
                    ShowHint(New HintConverter("自动安装失败：PCL暂时只支持.zip文件", HintState.Warn))
                    Exit Sub
                Else
                    ShowHint(New HintConverter("自动安装失败：未知的文件后缀", HintState.Warn))
                    Exit Sub
                End If
            Else
                ShowHint(New HintConverter("自动安装失败：它既不是文件也不是文件夹", HintState.Warn))
                Exit Sub
            End If

        Catch ex As Exception
            ExShow(ex, "自动安装失败", ErrorLevel.AllUsers)
        End Try
    End Sub

    ''' <summary>
    ''' 自动安装存档文件夹。需要异步调用。
    ''' </summary>
    ''' <param name="DirPath">文件夹路径，以“\”结尾。</param>
    ''' <remarks></remarks>
    Public Sub AutoSetupSaveFolder(ByVal DirPath As String)
        Try

            log("[System] 存档文件夹：" & DirPath)
            Dim SaveName As String = GetFileNameFromPath(Left(DirPath, Len(DirPath) - 1))
            Dim TargetPath As String = PATH_MC & "saves\" & SaveName & "\"
            Directory.CreateDirectory(PATH_MC & "saves\")
            If TargetPath = DirPath Then Exit Sub '路径相同

            '已存在检测
Recheck:
            If Directory.Exists(TargetPath) Then
                Select Case AutoSetupCheck("存档 " & SaveName & " 已经存在。" & vbCrLf & "位置：" & TargetPath, True)
                    Case 1
                        '替换
                        ShowHint("正在删除原存档：" & SaveName)
                        Directory.Delete(TargetPath, True)
                    Case 2
                        '重命名
                        TargetPath = Left(TargetPath, Len(TargetPath) - 1) & "-\"
                        SaveName = SaveName & "-"
                        GoTo Recheck
                    Case 3
                        '取消
                        Exit Sub
                End Select
            End If

            ShowHint("正在安装存档：" & SaveName)
            Directory.CreateDirectory(TargetPath)
            My.Computer.FileSystem.CopyDirectory(DirPath, TargetPath)
            ShowHint(New HintConverter("安装存档成功：" & SaveName, HintState.Finish))
        Catch ex As Exception
            ExShow(ex, "自动安装失败", ErrorLevel.AllUsers)
        End Try
    End Sub

    ''' <summary>
    ''' 自动安装资源包文件夹。需要异步调用。
    ''' </summary>
    ''' <param name="DirPath">文件夹路径，以“\”结尾。</param>
    ''' <remarks></remarks>
    Public Sub AutoSetupResFolder(ByVal DirPath As String)
        Try

            log("[System] 资源包文件夹：" & DirPath)
            Dim SaveName As String = GetFileNameFromPath(Left(DirPath, Len(DirPath) - 1))
            Dim TargetPath As String = PATH_MC & "resourcepacks\" & SaveName & "\"
            Directory.CreateDirectory(PATH_MC & "resourcepacks\")
            If TargetPath = DirPath Then Exit Sub '路径相同

            '已存在检测
Recheck:
            If Directory.Exists(TargetPath) Then
                Select Case AutoSetupCheck("资源包 " & SaveName & " 已经存在。" & vbCrLf & "位置：" & TargetPath)
                    Case 1
                        '替换
                        ShowHint("正在删除原资源包：" & SaveName)
                        Directory.Delete(TargetPath, True)
                    Case 2
                        '取消
                        Exit Sub
                End Select
            End If

            ShowHint("正在安装资源包：" & SaveName)
            Directory.CreateDirectory(TargetPath)
            My.Computer.FileSystem.CopyDirectory(DirPath, TargetPath)
            ShowHint(New HintConverter("安装资源包成功：" & SaveName, HintState.Finish))
        Catch ex As Exception
            ExShow(ex, "自动安装失败", ErrorLevel.AllUsers)
        End Try
    End Sub

    ''' <summary>
    ''' 自动安装版本文件夹。需要异步调用。
    ''' </summary>
    ''' <param name="DirPath">文件夹路径，以“\”结尾。</param>
    ''' <remarks></remarks>
    Public Sub AutoSetupVersionFolder(ByVal DirPath As String)
        Try

            log("[System] 版本文件夹：" & DirPath)
            Dim SaveName As String = GetFileNameFromPath(Left(DirPath, Len(DirPath) - 1))
            Dim TargetPath As String = PATH_MC & "versions\" & SaveName & "\"
            Directory.CreateDirectory(PATH_MC & "versions\")
            If TargetPath = DirPath Then Exit Sub '路径相同

            '已存在检测
Recheck:
            If Directory.Exists(TargetPath) Then
                Select Case AutoSetupCheck("版本 " & SaveName & " 已经存在。" & vbCrLf & "位置：" & TargetPath)
                    Case 1
                        '替换
                        ShowHint("正在删除原版本：" & SaveName)
                        Directory.Delete(TargetPath, True)
                    Case 2
                        '取消
                        Exit Sub
                End Select
            End If

            ShowHint("正在安装版本：" & SaveName)
            Directory.CreateDirectory(TargetPath)
            My.Computer.FileSystem.CopyDirectory(DirPath, TargetPath)
            ShowHint(New HintConverter("安装版本成功：" & SaveName, HintState.Finish))
        Catch ex As Exception
            ExShow(ex, "自动安装失败", ErrorLevel.AllUsers)
        End Try
        '强制重载版本列表
        WriteIni(PATH_MC & "PCL.ini", "LastCheckedFolder", "")
        Pool.Add(New Thread(AddressOf PoolVersionList))
    End Sub

    ''' <summary>
    ''' 自动安装版本文件。需要异步调用。
    ''' </summary>
    ''' <param name="FilePath">文件路径。</param>
    ''' <param name="EndDot">文件后缀。不包含小数点。</param>
    ''' <remarks></remarks>
    Public Sub AutoSetupVersionFile(ByVal FilePath As String, ByVal EndDot As String)
        Try

            log("[System] 版本文件：" & FilePath)
            Dim SaveName As String = GetFileNameFromPath(FilePath).Replace("." & EndDot, "")
            Dim TargetPath As String = PATH_MC & "versions\" & SaveName & "\" & SaveName & "." & EndDot
            Directory.CreateDirectory(PATH_MC & "versions\")
            If TargetPath = FilePath Then Exit Sub '路径相同

            '已存在检测
Recheck:
            If Directory.Exists(PATH_MC & "versions\" & SaveName & "\") Then
                If File.Exists(TargetPath) Then
                    Select Case AutoSetupCheck("版本文件 " & SaveName & " 已经存在。" & vbCrLf & "位置：" & TargetPath, True)
                        Case 1
                            '替换
                            ShowHint("正在删除原版本文件：" & SaveName)
                            File.Delete(TargetPath)
                        Case 2
                            '重命名
                            SaveName = SaveName & "-"
                            TargetPath = PATH_MC & "versions\" & SaveName & "\" & SaveName & "." & EndDot
                            GoTo Recheck
                        Case 3
                            '取消
                            Exit Sub
                    End Select
                End If
            End If

            ShowHint("正在安装版本文件：" & SaveName)
            Directory.CreateDirectory(PATH_MC & "versions\" & SaveName & "\")
            File.Copy(FilePath, TargetPath)
            ShowHint(New HintConverter("安装版本文件成功：" & SaveName, HintState.Finish))
        Catch ex As Exception
            ExShow(ex, "自动安装失败", ErrorLevel.AllUsers)
        End Try
        '强制重载版本列表
        WriteIni(PATH_MC & "PCL.ini", "LastCheckedFolder", "")
        Pool.Add(New Thread(AddressOf PoolVersionList))
    End Sub

    ''' <summary>
    ''' 自动安装存档zip压缩包。需要异步调用。
    ''' </summary>
    ''' <param name="Zip">ZipFile类型的已经打开的压缩文件对象。</param>
    ''' <param name="BasePath">在压缩文件中的基础路径。</param>
    ''' <remarks></remarks>
    Public Sub AutoSetupSaveZip(ByVal Zip As ZipFile, ByVal BasePath As String, ByVal FilePath As String)
        Try

            log("[System] 存档zip" & If(BasePath = "", "", "：" & BasePath))
            Dim SaveName As String = If(BasePath = "", GetFileNameFromPath(FilePath).Replace(".zip", ""), Left(BasePath, Len(BasePath) - 1))
            Dim TargetPath As String = PATH_MC & "saves\" & SaveName & "\"
            Directory.CreateDirectory(PATH_MC & "saves\")

            '已存在检测
            Dim Renamed As Boolean = False
Recheck:
            If Directory.Exists(TargetPath) Then
                Select Case AutoSetupCheck("存档 " & SaveName & " 已经存在。" & vbCrLf & "位置：" & TargetPath, True)
                    Case 1
                        '替换
                        ShowHint("正在删除原存档：" & SaveName)
                        Directory.Delete(TargetPath, True)
                    Case 2
                        '重命名
                        TargetPath = Left(TargetPath, Len(TargetPath) - 1) & "-\"
                        SaveName = SaveName & "-"
                        Renamed = True
                        GoTo Recheck
                    Case 3
                        '取消
                        Exit Sub
                End Select
            End If

            ShowHint("正在安装存档：" & SaveName)
            Directory.CreateDirectory(TargetPath)
            If BasePath = "" Then
                Zip.ExtractAll(TargetPath, ExtractExistingFileAction.OverwriteSilently)
            Else
                Zip.ExtractAll(PATH, ExtractExistingFileAction.OverwriteSilently)
                My.Computer.FileSystem.CopyDirectory(PATH & Left(BasePath, Len(BasePath) - 1), TargetPath)
                Directory.Delete(PATH & Left(BasePath, Len(BasePath) - 1), True)
            End If
            ShowHint(New HintConverter("安装存档成功：" & SaveName, HintState.Finish))
        Catch ex As Exception
            ExShow(ex, "自动安装失败", ErrorLevel.AllUsers)
        End Try
    End Sub

    ''' <summary>
    ''' 自动安装资源包zip压缩包。需要异步调用。
    ''' </summary>
    ''' <param name="FilePath">Zip文件路径。</param>
    ''' <remarks></remarks>
    Public Sub AutoSetupResZip(ByVal FilePath As String)
        Try

            log("[System] 资源包zip：" & FilePath)
            Dim SaveName As String = GetFileNameFromPath(FilePath)
            Dim TargetPath As String = PATH_MC & "resourcepacks\" & SaveName
            Directory.CreateDirectory(PATH_MC & "resourcepacks\")
            If TargetPath = FilePath Then Exit Sub '路径相同

            '已存在检测
Recheck:
            If File.Exists(TargetPath) Then
                Select Case AutoSetupCheck("资源包 " & SaveName & " 已经存在。" & vbCrLf & "位置：" & TargetPath)
                    Case 1
                        '替换
                        ShowHint("正在删除原资源包：" & SaveName)
                        File.Delete(TargetPath)
                    Case 2
                        '取消
                        Exit Sub
                End Select
            End If

            ShowHint("正在安装资源包：" & SaveName)
            File.Copy(FilePath, TargetPath)
            ShowHint(New HintConverter("安装资源包成功：" & SaveName, HintState.Finish))
        Catch ex As Exception
            ExShow(ex, "自动安装失败", ErrorLevel.AllUsers)
        End Try
    End Sub

    ''' <summary>
    ''' 自动安装版本zip压缩包。需要异步调用。
    ''' </summary>
    ''' <param name="Zip">ZipFile类型的已经打开的压缩文件对象。</param>
    ''' <param name="BasePath">在压缩文件中的基础路径。</param>
    ''' <remarks></remarks>
    Public Sub AutoSetupVersionZip(ByVal Zip As ZipFile, ByVal BasePath As String, ByVal FilePath As String)
        Try

            log("[System] 版本zip" & If(BasePath = "", "", "：" & BasePath))
            Dim VersionName As String = If(BasePath = "", GetFileNameFromPath(FilePath).Replace(".zip", ""), Left(BasePath, Len(BasePath) - 1))
            Dim TargetPath As String = PATH_MC & "versions\" & VersionName & "\"
            Directory.CreateDirectory(PATH_MC & "versions\")

            '已存在检测
            Dim Renamed As Boolean = False
Recheck:
            If Directory.Exists(TargetPath) Then
                Select Case AutoSetupCheck("版本 " & VersionName & " 已经存在。" & vbCrLf & "位置：" & TargetPath)
                    Case 1
                        '替换
                        ShowHint("正在删除原版本：" & VersionName)
                        Directory.Delete(TargetPath, True)
                    Case 2
                        '取消
                        Exit Sub
                End Select
            End If

            ShowHint("正在安装版本：" & VersionName)
            Directory.CreateDirectory(TargetPath)
            If BasePath = "" Then
                Zip.ExtractAll(TargetPath, ExtractExistingFileAction.OverwriteSilently)
            Else
                Zip.ExtractAll(PATH, ExtractExistingFileAction.OverwriteSilently)
                My.Computer.FileSystem.CopyDirectory(PATH & Left(BasePath, Len(BasePath) - 1), TargetPath)
                Directory.Delete(PATH & Left(BasePath, Len(BasePath) - 1), True)
            End If
            ShowHint(New HintConverter("安装版本成功：" & VersionName, HintState.Finish))
        Catch ex As Exception
            ExShow(ex, "自动安装失败", ErrorLevel.AllUsers)
        End Try
        '强制重载版本列表
        WriteIni(PATH_MC & "PCL.ini", "LastCheckedFolder", "")
        Pool.Add(New Thread(AddressOf PoolVersionList))
    End Sub

    ''' <summary>
    ''' 弹出自动安装的确认消息。需要异步调用。
    ''' </summary>
    ''' <param name="Text">显示的文本。</param>
    ''' <param name="CanRename">是否显示重命名选项。</param>
    ''' <returns>用户做出的是否安装的决定。</returns>
    ''' <remarks></remarks>
    Public Function AutoSetupCheck(ByVal Text As String, Optional ByVal CanRename As Boolean = False) As Integer
        Return MyMsgbox(Text, "自动安装确认", "覆盖", If(CanRename, "重命名", "取消"), If(CanRename, "取消", ""), True)
    End Function

#End Region

#Region "设置"

    ''' <summary>
    ''' 切换页面隐藏。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub SetupRefreshHidden()
        frmMain.btnTopDown.Visibility = If(ReadIni("setup", "UiHiddenDownload", "False") = "True", Visibility.Collapsed, Visibility.Visible)
        frmMain.btnTopSetup.Visibility = If(ReadIni("setup", "UiHiddenSetup", "False") = "True", Visibility.Collapsed, Visibility.Visible)
    End Sub

    ''' <summary>
    ''' 切换背景图片的展示方式。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub SetupRefreshBackground()
        Select Case Val(ReadIni("setup", "UiBackgroundShow", "0"))
            Case 0
                '右下
                frmMain.imgMainBg.HorizontalAlignment = HorizontalAlignment.Right
                frmMain.imgMainBg.VerticalAlignment = VerticalAlignment.Bottom
                frmMain.imgMainBg.Stretch = Stretch.None
            Case 1
                '左上
                frmMain.imgMainBg.HorizontalAlignment = HorizontalAlignment.Left
                frmMain.imgMainBg.VerticalAlignment = VerticalAlignment.Top
                frmMain.imgMainBg.Stretch = Stretch.None
            Case 2
                '右上
                frmMain.imgMainBg.HorizontalAlignment = HorizontalAlignment.Right
                frmMain.imgMainBg.VerticalAlignment = VerticalAlignment.Top
                frmMain.imgMainBg.Stretch = Stretch.None
            Case 3
                '居中
                frmMain.imgMainBg.HorizontalAlignment = HorizontalAlignment.Center
                frmMain.imgMainBg.VerticalAlignment = VerticalAlignment.Center
                frmMain.imgMainBg.Stretch = Stretch.None
            Case 4
                '拉伸
                frmMain.imgMainBg.HorizontalAlignment = HorizontalAlignment.Stretch
                frmMain.imgMainBg.VerticalAlignment = VerticalAlignment.Stretch
                frmMain.imgMainBg.Stretch = Stretch.Fill
            Case 5
                '适应
                frmMain.imgMainBg.HorizontalAlignment = HorizontalAlignment.Stretch
                frmMain.imgMainBg.VerticalAlignment = VerticalAlignment.Stretch
                frmMain.imgMainBg.Stretch = Stretch.UniformToFill
        End Select
    End Sub

    ''' <summary>
    ''' 根据进行版本隔离返回某版本所需的游戏目录，以“\”结尾。
    ''' </summary>
    ''' <param name="Version"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetVersionFolder(ByVal Version As MCVersion) As String
        Select Case Val(ReadIni("setup", "LaunchSplit", "0"))
            Case 0
                Return PATH_MC
            Case 1
                Return If(Version.Type = MCVersionType.FORGE, Version.Path, PATH_MC)
            Case Else
                Return Version.Path
        End Select
    End Function

#End Region

#Region "提示"

    ''' <summary>
    ''' 等待弹出的提示文本。
    ''' </summary>
    ''' <remarks></remarks>
    Public WaitingHint As ArrayList = If(IsNothing(WaitingHint), New ArrayList, WaitingHint)
    ''' <summary>
    ''' 在窗口左下角弹出提示文本。
    ''' </summary>
    ''' <param name="Hint">要显示的提示。</param>
    ''' <remarks></remarks>
    Public Sub ShowHint(ByVal Hint As HintConverter)
        If IsNothing(WaitingHint) Then WaitingHint = New ArrayList 'MDZZ这初始化也太坑了
        For Each NowHint As HintConverter In WaitingHint
            If Hint.Text = NowHint.Text Then Exit Sub
        Next
        WaitingHint.Add(Hint)
    End Sub
    ''' <summary>
    ''' 在窗口左下角弹出提示文本。
    ''' </summary>
    ''' <param name="Text">要显示的提示。</param>
    ''' <remarks></remarks>
    Public Sub ShowHint(ByVal Text As String)
        ShowHint(New HintConverter(Text))
    End Sub

    ''' <summary>
    ''' 等待弹出的提示窗口。
    ''' </summary>
    ''' <remarks></remarks>
    Public WaitingHintWindow As ArrayList = If(IsNothing(WaitingHintWindow), New ArrayList, WaitingHintWindow)
    ''' <summary>
    ''' 弹出提示窗口。
    ''' </summary>
    ''' <param name="Hint">要显示的提示文本。</param>
    ''' <remarks></remarks>
    Public Sub ShowHintWindow(ByVal Title As String, ByVal Hint As String)
        frmMain.Dispatcher.Invoke(Sub()

                                      If IsNothing(WaitingHintWindow) Then WaitingHintWindow = New ArrayList '初始化
                                      If Not WaitingHintWindow.Contains({Title, Hint}) Then
                                          WaitingHintWindow.Add({Title, Hint})
                                      End If

                                  End Sub)
    End Sub

#End Region

#Region "统计与反馈"

    Public SendInvokes As New ArrayList

    Private LastPage As String = QN_SERVER & "stats.html"
    ''' <summary>
    ''' 发送页面数据。
    ''' </summary>
    ''' <param name="Name">统计的页面。</param>
    ''' <remarks></remarks>
    Public Sub SendPage(ByVal Name As String)
        If MODE_DEVELOPER Then log("[System] 发送页面统计：" & Name)
        If MODE_OFFLINE Or MODE_DEVELOPER Then Exit Sub
        SendInvokes.Add("try{_czc.push([""_trackPageview"",""/" & Name & ".html"",""" & LastPage & """]);}catch (e){}")
        LastPage = QN_SERVER & Name & ".html"
    End Sub

    ''' <summary>
    ''' 发送事件统计。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub SendStat(ByVal MainType As String, ByVal Action As String, Optional ByVal Lab As String = "", Optional ByVal Number As Integer = 0)
        Try
            MainType = MainType.Replace(vbCrLf, "").Replace("""", " ").Replace("\", "/")
            Action = Action.Replace(vbCrLf, "").Replace("""", " ").Replace("\", "/")
            Lab = Lab.Replace(vbCrLf, "").Replace("""", " ").Replace("\", "/")
            If (Not AllowFeedback) And (Not MainType = "推荐源") Then Exit Sub
            If MODE_DEVELOPER Then log("[System] 发送事件统计：" & MainType & " > " & Action & If(Lab = "", "", " > " & Lab) & "（" & Number & "）")
            If MODE_OFFLINE Or MODE_DEVELOPER Then Exit Sub
            SendInvokes.Add("try{_czc.push([""_trackEvent"",""" & MainType & """,""" & Action & """,""" & Lab & """," & Number & ",]);}catch (e){}")
        Catch
        End Try
    End Sub

    ''' <summary>
    ''' 处理错误信息。
    ''' </summary>
    ''' <param name="ex"></param>
    ''' <param name="description">描述文本，如“下载文件时出错”。</param>
    ''' <param name="errorLevel">错误的严重程度。</param>
    ''' <remarks></remarks>
    Public Sub ExShow(ByVal ex As Exception, ByVal description As String, Optional ByVal errorLevel As ErrorLevel = ErrorLevel.DebugOnly, Optional ByVal msgboxText As String = "")
        On Error Resume Next
        '发送统计
        If Not description.Contains("下载文件失败") Then SendStat("错误", GetStringFromEnum(errorLevel), description & "：" & GetStringFromException(ex), errorLevel)
        '处理错误
        Select Case errorLevel
            Case modMain.ErrorLevel.Slient
                '不提示用户
                log("[Error] " & description & "：" & GetStringFromException(ex, MODE_DEVELOPER),, True)
            Case modMain.ErrorLevel.DebugOnly
                '提示调试模式用户
                log("[Error] " & description & "：" & GetStringFromException(ex, True), True)
            Case modMain.ErrorLevel.AllUsers
                '提示所有用户
                log("[Error] " & description & "：" & GetStringFromException(ex, True))
                ShowHint(New HintConverter(description & "：" & GetStringFromException(ex), HintState.Critical))
            Case modMain.ErrorLevel.MsgboxWithoutFeedback
                '弹窗，不要求反馈
                MyMsgbox(If(Not msgboxText = "", msgboxText & vbCrLf & vbCrLf, "") & "详细的错误信息：" & GetStringFromException(ex, True), description, IsWaitExit:=False)
            Case modMain.ErrorLevel.MsgboxAndFeedback
                '弹窗，要求反馈
                If MyMsgbox(If(Not msgboxText = "", msgboxText & vbCrLf & vbCrLf, "") & "详细的错误信息：" & GetStringFromException(ex, True) & vbCrLf & vbCrLf & "你是否愿意反馈这个问题？", description, "反馈", "不反馈") = 1 Then
                    Feedback()
                End If
            Case modMain.ErrorLevel.Barrier
                '尽可能显示错误信息，然后关闭程序
                If MsgBox(If(Not msgboxText = "", msgboxText & vbCrLf & vbCrLf, "") & "详细的错误信息：" & GetStringFromException(ex, True) & vbCrLf & vbCrLf & "你是否愿意反馈这个问题？", MsgBoxStyle.YesNo + MsgBoxStyle.Critical, description) = MsgBoxResult.Yes Then
                    Feedback()
                End If
                End
        End Select
    End Sub
    Public Enum ErrorLevel As Integer
        ''' <summary>
        ''' 正常情况下的常发错误。只记录 Log，不提示用户。
        ''' </summary>
        ''' <remarks></remarks>
        Slient = 0
        ''' <summary>
        ''' 只需要提示调试模式下的用户。
        ''' </summary>
        ''' <remarks></remarks>
        DebugOnly = 1
        ''' <summary>
        ''' 需要以弹出提示的方式提示所有用户。
        ''' </summary>
        ''' <remarks></remarks>
        AllUsers = 2
        ''' <summary>
        ''' 需要以弹窗的方式提示所有用户，但是不要求反馈。
        ''' </summary>
        ''' <remarks></remarks>
        MsgboxWithoutFeedback = 3
        ''' <summary>
        ''' 需要以弹窗的方式提示所有用户，并且要求反馈。
        ''' </summary>
        ''' <remarks></remarks>
        MsgboxAndFeedback = 4
        ''' <summary>
        ''' 尽可能弹出提示，然后结束程序。
        ''' </summary>
        ''' <remarks></remarks>
        Barrier = 5
    End Enum

    ''' <summary>
    ''' 开始用户反馈。本函数会直接打开log文件与反馈网页。
    ''' </summary>
    Public Sub Feedback()
        If MyMsgbox("PCL2 正在制作中，因此 PCL1 已经停止更新与维护，反馈通道已经关闭。" & vbCrLf & "你可在 http://afdian.net/@LTCat 查看 PCL2 当前制作进度。（制作进度日更，制作完成后依然免费下载）", "反馈提示", "查看 PCL2 制作进度", "返回") = 1 Then
            Process.Start("https://afdian.net/@LTCat")
        End If
        'Process.Start("https://www.wjx.cn/jq/14677608.aspx")
        'Shell("notepad", PATH & "PCL\log.txt")
    End Sub

    Public Sub FeedbackSetup()
        Try
            SendStat("反馈", "设置等级", ReadReg("SysLevel", "Default (0)"), ReadReg("SysLevel", "0"))
            SendStat("反馈", "调试模式", ReadReg("SysTest", "Default (False)"), ReadReg("SysTest", "False") = "True")
            SendStat("反馈", "检查更新", ReadIni("setup", "SysUpdate", "Default (True)"), ReadIni("setup", "SysUpdate", "True") = "True")
            SendStat("反馈", "有更新时是否只提示", ReadIni("setup", "SysUpdateHint", "Default (True)"), ReadIni("setup", "SysUpdateHint", "True") = "True")
            SendStat("反馈", "低内存警告", ReadIni("setup", "raSystemWarn", "Default (100)"), ReadIni("setup", "raSystemWarn", "100"))
            SendStat("反馈", "接收测试版更新", ReadIni("setup", "SysUpdateTest", "Default (False)"), ReadIni("setup", "SysUpdateTest", "False") = "True")
            SendStat("反馈", "启动器主题", ReadIni("setup", "UiTheme", "Default (0)"), ReadIni("setup", "UiTheme", "0"))
            SendStat("反馈", "启用自定义背景", File.Exists(PATH & "PCL\back.png"), File.Exists(PATH & "PCL\back.png"))
            SendStat("反馈", "启用自定义顶部栏", File.Exists(PATH & "PCL\top.png"), File.Exists(PATH & "PCL\top.png"))
            SendStat("反馈", "启用 BGM", File.Exists(PATH & "PCL\bgm.wav") Or File.Exists(PATH & "PCL\bgm.mp3"), File.Exists(PATH & "PCL\bgm.wav") Or File.Exists(PATH & "PCL\bgm.mp3"))
            SendStat("反馈", "背景透明度", ReadIni("setup", "UiBackgroundOpacity", "Default (50)"), ReadIni("setup", "UiBackgroundOpacity", "50"))
            SendStat("反馈", "启动器透明度", ReadIni("setup", "UiLauncherOpacity", "Default (100)"), ReadIni("setup", "UiLauncherOpacity", "100"))
            SendStat("反馈", "游戏窗口置顶", ReadReg("LaunchTopmost", "Default (False)"), ReadReg("LaunchTopmost", "False") = "True")
            SendStat("反馈", "启动器可见性", ReadIni("setup", "LaunchVisibility", "Default (3)"), ReadIni("setup", "LaunchVisibility", "3"))
            SendStat("反馈", "游戏进程优先级", ReadReg("LaunchLevel", "Default (1)"), ReadReg("LaunchLevel", "1"))
            SendStat("反馈", "显示游戏日志", ReadIni("setup", "LaunchLog", "Default (False)"), ReadIni("setup", "LaunchLog", "False") = "True")
            SendStat("反馈", "离线皮肤", ReadReg("LaunchSkin", "Default (3)"), ReadReg("LaunchSkin", "3"))
            SendStat("反馈", "离线皮肤正版名", ReadReg("LaunchSkinName", "Default (None)"))
            SendStat("反馈", "版本隔离", ReadIni("setup", "LaunchSplit", "Default (0)"), ReadIni("setup", "LaunchSplit", "0"))
            SendStat("反馈", "游戏窗口模式", ReadIni("setup", "LaunchMode", "Default (0)"), ReadIni("setup", "LaunchMode", "0"))
            SendStat("反馈", "Java 路径", ReadReg("SetupJavaPath", "Default (None)"))
            SendStat("反馈", "最大内存", ReadReg("LaunchMaxRam", "Default (1024)"), ReadReg("LaunchMaxRam", "1024"))
            SendStat("反馈", "电脑最大内存", Int(My.Computer.Info.TotalPhysicalMemory / 256 / 1024 / 1024) * 256 & "M", Int(My.Computer.Info.TotalPhysicalMemory / 256 / 1024 / 1024) * 256)
            SendStat("反馈", "Java 版本", GetFileVersion(PATH_JAVA & "\javaw.exe"))
            SendStat("反馈", "游戏版本列表源", ReadIni("setup", "DownVersion", "Default (0)"), ReadIni("setup", "DownVersion", "0"))
            SendStat("反馈", "游戏数据文件源", ReadIni("setup", "DownAssets", "Default (0)"), ReadIni("setup", "DownAssets", "0"))
            SendStat("反馈", "游戏组件源", ReadIni("setup", "DownMinecraft", "Default (1)"), ReadIni("setup", "DownMinecraft", "1"))
            SendStat("反馈", "OptiFine 源", ReadIni("setup", "DownOptiFine", "Default (1)"), ReadIni("setup", "DownOptiFine", "1"))
            SendStat("反馈", "Forge 源", ReadIni("setup", "DownForge", "Default (1)"), ReadIni("setup", "DownForge", "1"))
            SendStat("反馈", "Forge 自动安装", ReadIni("setup", "DownForgeAction", "Default (1)"), ReadIni("setup", "DownForgeAction", "1"))
            SendStat("反馈", "数据缺失提示", ReadIni("setup", "DownMinecraftAssetsHint", "Default (True)"), ReadIni("setup", "DownMinecraftAssetsHint", "True") = "True")
            SendStat("反馈", "数据缺失自动下载", ReadIni("setup", "DownMinecraftAssetsAuto", "Default (True)"), ReadIni("setup", "DownMinecraftAssetsAuto", "True") = "True")
            SendStat("反馈", "同时下载文件数", ReadIni("setup", "DownMaxinum", "Default (15)"), ReadIni("setup", "DownMaxinum", "15"))
            SendStat("反馈", "游戏更新提示", ReadIni("setup", "HomeUpdate", "Default (True)"), ReadIni("setup", "HomeUpdate", "True") = "True")
            SendStat("反馈", "游戏更新提示仅正式版", ReadIni("setup", "HomeUpdateRelease", "Default (False)"), ReadIni("setup", "HomeUpdateRelease", "False") = "True")
            SendStat("反馈", "保存密码", ReadReg("HomeSave", "Default (False)"), ReadReg("HomeSave", "False") = "True")
            SendStat("反馈", "自动登录", ReadIni("setup", "HomeAutologin", "Default (False)"), ReadIni("setup", "HomeAutologin", "False") = "True")
            SendStat("反馈", "自动播放速度", ReadIni("setup", "raHomeAutoplaySpeed", "Default (1)"), ReadIni("setup", "raHomeAutoplaySpeed", "1"))
            SendStat("反馈", "推荐源：PCL", ReadIni("setup", "HomePCLPush", "Default (True)"), ReadIni("setup", "HomePCLPush", "True") = "True")
            SendStat("反馈", "推荐源：MCBBS", ReadIni("setup", "HomeMCBBSPush", "Default (True)"), ReadIni("setup", "HomeMCBBSPush", "True") = "True")
            SendStat("反馈", "推荐源：贴吧", ReadIni("setup", "HomeTbPush", "Default (True)"), ReadIni("setup", "HomeTbPush", "True") = "True")
            SendStat("反馈", "推荐源：Forum", ReadIni("setup", "HomeForumPush", "Default (False)"), ReadIni("setup", "HomeForumPush", "True") = "True")
            SendStat("反馈", "推荐源：Mojang", ReadIni("setup", "HomeMojangPush", "Default (True)"), ReadIni("setup", "HomeMojangPush", "True") = "True")
            SendStat("反馈", "推荐源：广告", ReadIni("setup", "HomeAdPush", "Default (True)"), ReadIni("setup", "HomeAdPush", "True") = "True")
            SendStat("反馈", "启动器版本", VERSION_NAME, VERSION_CODE)
        Catch ex As Exception
            ExShow(ex, "发送用户反馈失败", ErrorLevel.Slient)
        End Try
    End Sub

#End Region

#Region "皮肤"

    ''' <summary>
    ''' 从官方 UUID 获取本地缓存的皮肤路径。如果失败返回空字符串。
    ''' </summary>
    ''' <returns></returns>
    Public Function GetCacheSkinAddressByMojangUUID(UUID As String) As String
        Dim SkinName = ReadIni("cache\skin\SkinName", UUID)
        If SkinName = "" Then
            Return ""
        ElseIf SkinName = "Steve" Or SkinName = "Alex" Then
            Return SkinName
        Else
            GetCacheSkinAddressByMojangUUID = PATH & "PCL\cache\skin\" & SkinName & ".png"
            If Not File.Exists(GetCacheSkinAddressByMojangUUID) Then Return ""
        End If
    End Function

    ''' <summary>
    ''' 下载对应官方 UUID 的皮肤并返回文件路径。如果失败返回空字符串。
    ''' </summary>
    ''' <param name="UUID"></param>
    ''' <returns></returns>
    Public Function DownloadSkin(UUID As String) As String
        Try

            '尝试读取缓存的皮肤

            Dim CacheSkinAddress As String = GetCacheSkinAddressByMojangUUID(UUID)
            If Not CacheSkinAddress = "" Then Return CacheSkinAddress

            '获取皮肤下载地址

            '获取皮肤地址
            Dim StringConv = GetWebsiteCode("https://sessionserver.mojang.com/session/minecraft/profile/" & UUID, Encoding.Default)

            '如果返回为空，则为离线皮肤
            If StringConv = "" Then Return GetSkinTypeFromUUID(UUID)

            '处理皮肤地址
            StringConv = StringConv.Replace(" ", "")
            StringConv = Mid(StringConv, StringConv.IndexOf("textures"",""value"":""") + 1).Replace("textures"",""value"":""", "")
            StringConv = Mid(StringConv, 1, StringConv.IndexOf("""")).Replace(" ", "")
            StringConv = System.Text.Encoding.GetEncoding("utf-8").GetString(Convert.FromBase64String(StringConv))
            StringConv = StringConv.Replace(" ", "")
            If Not StringConv.Contains("""url"":""") Then Return GetSkinTypeFromUUID(UUID)
            StringConv = Mid(StringConv, StringConv.IndexOf("""url"":""") + 1).Replace("""url"":""", "")
            StringConv = Mid(StringConv, 1, StringConv.IndexOf("""")).Replace(" ", "")

            '下载皮肤

            Dim FileName As String = Mid(StringConv, StringConv.LastIndexOf("/") + 2)
            If Not File.Exists(PATH & "PCL\cache\skin\" & FileName & ".png") Then
                DownloadFile(StringConv, PATH & "PCL\cache\skin\" & FileName & ".png" & DOWNLOADING_END)
                File.Delete(PATH & "PCL\cache\skin\" & FileName & ".png")
                FileSystem.Rename(PATH & "PCL\cache\skin\" & FileName & ".png" & DOWNLOADING_END, PATH & "PCL\cache\skin\" & FileName & ".png")
                log("[System] 皮肤下载成功：" & FileName)
            End If
            WriteIni("cache\skin\SkinName", UUID, FileName)
            Return PATH & "PCL\cache\skin\" & FileName & ".png"

        Catch ex As Exception
            ExShow(ex, "下载皮肤失败（" & UUID & "）", ErrorLevel.Slient)
            Return ""
        End Try
    End Function

    ''' <summary>
    ''' 获取UUID对应的离线皮肤，返回“Steve”或“Alex”。
    ''' </summary>
    ''' <param name="UUID"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetSkinTypeFromUUID(ByVal UUID As String) As String
        If Not UUID.Length = 32 Then Return "Steve"
        Dim a = Integer.Parse(UUID(7), Globalization.NumberStyles.AllowHexSpecifier)
        Dim b = Integer.Parse(UUID(15), Globalization.NumberStyles.AllowHexSpecifier)
        Dim c = Integer.Parse(UUID(23), Globalization.NumberStyles.AllowHexSpecifier)
        Dim d = Integer.Parse(UUID(31), Globalization.NumberStyles.AllowHexSpecifier)
        Return If(((a Xor b) Xor (c Xor d)) Mod 2, "Alex", "Steve")
    End Function

#End Region

    Public Delegate Sub RefreshThemeInvoke()
    ''' <summary>刷新主题。</summary>
    ''' <remarks></remarks>
    Public Sub RefreshTheme()
        Try
            '读取数据
            Dim LeftbarUri As String = PATH_IMAGE & "LeftBar-BG_Blue.png"
            frmMain.imgMainBg.Source = Nothing
            If File.Exists(PATH & "PCL\top.png") Then LeftbarUri = PATH & "PCL\top.png"
            If File.Exists(PATH & "PCL\back.png") Then
                frmMain.imgMainBg.Source = New MyBitmap(PATH & "PCL\back.png")
                SetupRefreshBackground()
            End If
            Select Case Val(ReadIni("setup", "UiTheme", "0"))
                Case 0
                    Application.Current.Resources("Color1") = New SolidColorBrush(Color.FromRgb(213, 233, 255))
                    Application.Current.Resources("Color2") = New SolidColorBrush(Color.FromRgb(106, 177, 255))
                    Application.Current.Resources("Color3") = New SolidColorBrush(Color.FromRgb(0, 121, 255))
                    Application.Current.Resources("Color4") = New SolidColorBrush(Color.FromRgb(0, 81, 170))
                    Application.Current.Resources("Color5") = New SolidColorBrush(Color.FromRgb(0, 40, 85))
                    log("[System] 主题配色：蓝色")
                Case 1
                    LeftbarUri = PATH_IMAGE & "LeftBar-BG_Black.png"
                    Application.Current.Resources("Color1") = New SolidColorBrush(Color.FromRgb(212, 212, 212))
                    Application.Current.Resources("Color2") = New SolidColorBrush(Color.FromRgb(148, 148, 148))
                    Application.Current.Resources("Color3") = New SolidColorBrush(Color.FromRgb(127, 127, 127))
                    Application.Current.Resources("Color4") = New SolidColorBrush(Color.FromRgb(90, 90, 90))
                    Application.Current.Resources("Color5") = New SolidColorBrush(Color.FromRgb(42, 42, 42))
                    log("[System] 主题配色：黑色")
                Case 2
                    LeftbarUri = PATH_IMAGE & "LeftBar-BG_Orange.png"
                    Application.Current.Resources("Color1") = New SolidColorBrush(Color.FromRgb(255, 232, 213))
                    Application.Current.Resources("Color2") = New SolidColorBrush(Color.FromRgb(255, 168, 96))
                    Application.Current.Resources("Color3") = New SolidColorBrush(Color.FromRgb(255, 126, 21))
                    Application.Current.Resources("Color4") = New SolidColorBrush(Color.FromRgb(191, 86, 0))
                    Application.Current.Resources("Color5") = New SolidColorBrush(Color.FromRgb(106, 48, 0))
                    log("[System] 主题配色：橙色")
                Case 3
                    LeftbarUri = PATH_IMAGE & "LeftBar-BG_Green.png"
                    Application.Current.Resources("Color1") = New SolidColorBrush(Color.FromRgb(213, 255, 217))
                    Application.Current.Resources("Color2") = New SolidColorBrush(Color.FromRgb(97, 233, 111))
                    Application.Current.Resources("Color3") = New SolidColorBrush(Color.FromRgb(43, 213, 60))
                    Application.Current.Resources("Color4") = New SolidColorBrush(Color.FromRgb(29, 141, 40))
                    Application.Current.Resources("Color5") = New SolidColorBrush(Color.FromRgb(15, 70, 20))
                    log("[System] 主题配色：绿色")
                Case 4
                    Application.Current.Resources("Color1") = New SolidColorBrush(Color.FromRgb(ReadIni("setup", "UiThemeColorR1", Application.Current.Resources("ColorE1").R), ReadIni("setup", "UiThemeColorG1", Application.Current.Resources("ColorE1").G), ReadIni("setup", "UiThemeColorB1", Application.Current.Resources("ColorE1").B)))
                    Application.Current.Resources("Color2") = New SolidColorBrush(Color.FromRgb(ReadIni("setup", "UiThemeColorR2", Application.Current.Resources("ColorE2").R), ReadIni("setup", "UiThemeColorG2", Application.Current.Resources("ColorE2").G), ReadIni("setup", "UiThemeColorB2", Application.Current.Resources("ColorE2").B)))
                    Application.Current.Resources("Color3") = New SolidColorBrush(Color.FromRgb(ReadIni("setup", "UiThemeColorR3", Application.Current.Resources("ColorE3").R), ReadIni("setup", "UiThemeColorG3", Application.Current.Resources("ColorE3").G), ReadIni("setup", "UiThemeColorB3", Application.Current.Resources("ColorE3").B)))
                    Application.Current.Resources("Color4") = New SolidColorBrush(Color.FromRgb(ReadIni("setup", "UiThemeColorR4", Application.Current.Resources("ColorE4").R), ReadIni("setup", "UiThemeColorG4", Application.Current.Resources("ColorE4").G), ReadIni("setup", "UiThemeColorB4", Application.Current.Resources("ColorE4").B)))
                    Application.Current.Resources("Color5") = New SolidColorBrush(Color.FromRgb(ReadIni("setup", "UiThemeColorR5", Application.Current.Resources("ColorE5").R), ReadIni("setup", "UiThemeColorG5", Application.Current.Resources("ColorE5").G), ReadIni("setup", "UiThemeColorB5", Application.Current.Resources("ColorE5").B)))
                    log("[System] 主题配色：自定义")
                Case 100
                    LeftbarUri = PATH_IMAGE & "LeftBar-BG_Hunluan.png"
                    Application.Current.Resources("Color1") = New SolidColorBrush(Color.FromRgb(248, 231, 191))
                    Application.Current.Resources("Color2") = New SolidColorBrush(Color.FromRgb(244, 208, 125))
                    Application.Current.Resources("Color3") = New SolidColorBrush(Color.FromRgb(228, 165, 16))
                    Application.Current.Resources("Color4") = New SolidColorBrush(Color.FromRgb(182, 130, 20))
                    Application.Current.Resources("Color5") = New SolidColorBrush(Color.FromRgb(102, 71, 0))
                    log("[System] 主题配色：混乱")
                Case 101
                    LeftbarUri = PATH_IMAGE & "LeftBar-BG_DeathBlue.png"
                    Application.Current.Resources("Color1") = New SolidColorBrush(Color.FromRgb(209, 234, 254))
                    Application.Current.Resources("Color2") = New SolidColorBrush(Color.FromRgb(71, 171, 252))
                    Application.Current.Resources("Color3") = New SolidColorBrush(Color.FromRgb(3, 134, 241))
                    Application.Current.Resources("Color4") = New SolidColorBrush(Color.FromRgb(0, 108, 194))
                    Application.Current.Resources("Color5") = New SolidColorBrush(Color.FromRgb(0, 73, 45))
                    log("[System] 主题配色：死机蓝")
            End Select
            Application.Current.Resources("ColorE1") = Application.Current.Resources("Color1").Color
            Application.Current.Resources("ColorE2") = Application.Current.Resources("Color2").Color
            Application.Current.Resources("ColorE3") = Application.Current.Resources("Color3").Color
            Application.Current.Resources("ColorE4") = Application.Current.Resources("Color4").Color
            Application.Current.Resources("ColorE5") = Application.Current.Resources("Color5").Color
            '改变控件图片
            If Not LeftbarUri = "" Then CType(frmMain.panTop.Background, Object).ImageSource = New MyBitmap(LeftbarUri)
            CType(frmMain.panTop.Background, Object).Stretch = Stretch.None
            CType(frmMain.panTop.Background, Object).TileMode = TileMode.Tile
            CType(CType(frmMain.panTop.Background, Object), ImageBrush).Viewport = (New RectConverter()).ConvertFromString("0,0," & CType(CType(frmMain.panTop.Background, Object).ImageSource, ImageSource).Width & "," & CType(CType(frmMain.panTop.Background, Object).ImageSource, ImageSource).Height)
            '改变已有控件
            'Dim E2NoAlpha As Color = Application.Current.Resources("ColorE2")
            'E2NoAlpha.A = 1
            'If Not IsNothing(frmHomeRight.SwapOldLab) Then CType(frmHomeRight.SwapOldLab.Background, SolidColorBrush).SetCurrentValue(SolidColorBrush.ColorProperty, E2NoAlpha)
            'If Not IsNothing(frmHomeRight.SwapVersionLab) Then CType(frmHomeRight.SwapVersionLab.Background, SolidColorBrush).SetCurrentValue(SolidColorBrush.ColorProperty, E2NoAlpha)
        Catch ex As Exception
            ExShow(ex, "刷新主题失败", ErrorLevel.Barrier)
        End Try
    End Sub

    ''' <summary>
    ''' 以正常方式结束程序运行，这会输出log与执行动画。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub EndNormal()
        AniStart({
            AaOpacity(frmMain, -1, 150),
            AaScaleTransform(frmMain.panAllBack, -0.05, 150,, New AniEaseJumpStart(0.4)),
            AaCode({"End"}, 250)
        }, "EndAll")
        log("[System] 收到正常关闭指令")
    End Sub
    ''' <summary>
    ''' 强制暴力结束程序执行。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub EndForce()
        On Error Resume Next
        Process.GetCurrentProcess.Kill()
        End
        My.Application.Shutdown()
        frmMain.Close()
        frmStart.Close()
    End Sub

    '动画事件
    Public Sub AaCodeCall(ByVal type As String, ByVal data As Array)
        Select Case type
            Case "HideInfoBox"
                data(1).Hide()
            Case "PageChange"
                frmMain.PageChange(data(1))
        End Select
    End Sub

    Private LogLock As New Object
    ''' <summary>
    ''' 输出Log。
    ''' </summary>
    ''' <param name="LogText">Log文本。</param>
    ''' <param name="Notice">是否为重要记录，如果为是则会给调试模式用户发送提示。</param>
    ''' <remarks></remarks>
    Public Sub Log(ByVal LogText As String, Optional ByVal Notice As Boolean = False, Optional DeveloperNotice As Boolean = False)
        Try
            Dim OutputText = "[" & GetTime() & "] " & LogText
            Debug.WriteLine(OutputText)
            If Not File.Exists(PATH & "PCL\log.txt") Then
                File.Create(PATH & "PCL\log.txt").Dispose()
            End If
            SyncLock LogLock
                Using Writter As New StreamWriter(PATH & "PCL\log.txt", True)
                    Writter.WriteLine(OutputText)
                    Writter.Close()
                End Using
            End SyncLock
            If Notice And MODE_DEBUG Then ShowHint(New HintConverter("[调试模式] " & LogText.Replace("[Error] ", ""), HintState.Warn))
            If DeveloperNotice And MODE_DEVELOPER Then ShowHint(New HintConverter("[开发者模式] " & LogText.Replace("[Error] ", ""), HintState.Info))
        Catch
        End Try
    End Sub

    Public UpdateState As LoadState = LoadState.Waiting
    ''' <summary>
    ''' 下载PCL版本更新。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub DownloadUpdate()
        UpdateState = LoadState.Loading
        log("[System] 下载更新开始")
        SendStat("反馈", "升级", VERSION_NAME, VERSION_CODE)
        WebStart({New WebRequireFile With {
                         .WebURLs = New ArrayList From {
                                 TX_SREVER_2 & If(ReadIni("setup", "SysUpdateTest", "False") = "True", "pcl_snapshot", "pcl_release") & ".zip",
                                 TX_SREVER_1 & If(ReadIni("setup", "SysUpdateTest", "False") = "True", "pcl_snapshot", "pcl_release") & ".zip"
                         },
                         .LocalFolder = PATH & "PCL\", .LocalName = "pcl.zip", .KnownFileSize = 256 * 1024}
                }, "PCL 更新", Sub()

                                 '下载成功
                                 Try
                                     If Directory.Exists(PATH & "PCL\Update") Then Directory.Delete(PATH & "PCL\Update", True)
                                     Directory.CreateDirectory(PATH & "PCL\Update")
                                     Using zip As New ZipFile(PATH & "PCL\pcl.zip", Encoding.Default)
                                         zip.ExtractAll(PATH & "PCL\Update", ExtractExistingFileAction.OverwriteSilently)
                                     End Using
                                     File.Delete(PATH & "PCL\pcl.zip")
                                     OutputFileInResource("PCLAM", PATH & "PCL\PCL Admin Manager.exe", False)
                                     Dim Arguments As String = "Auto Update:" & GetPathFromFullPath(Forms.Application.ExecutablePath) & "|" & Process.GetCurrentProcess.Id
                                     Shell(PATH & "PCL\PCL Admin Manager.exe", Arguments)
                                     UpdateState = LoadState.Loaded
                                     EndForce()
                                 Catch ex As Exception
                                     ShowHint(New HintConverter("更新失败：" & GetStringFromException(ex), HintState.Critical))
                                     log("[System] 更新失败：" & GetStringFromException(ex, True), True)
                                     UpdateState = LoadState.Failed
                                 End Try

                             End Sub, Sub()
                                          '下载失败
                                          ShowHint(New HintConverter("下载更新失败", HintState.Critical))
                                          UpdateState = LoadState.Failed
                                      End Sub, WebRequireSize.AtLeast)
    End Sub

    Private LastRefreshOffline As Boolean = False
    ''' <summary>
    ''' 刷新离线模式状态。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub RefreshOffline()
        '刷新状态
        If My.Computer.Network.IsAvailable Then
            MODE_OFFLINE = ReadIni("setup", "SysOffline", "False")
        Else
            MODE_OFFLINE = True
        End If
        '检测是否变化
        If LastRefreshOffline = MODE_OFFLINE Then Exit Sub
        '各种UI的变化
        On Error Resume Next
        If Not IsNothing(frmMain.labTopVer) Then frmMain.labTopVer.Content = VERSION_NAME & If(MODE_DEVELOPER, " | 开发者模式", If(MODE_DEBUG, " | 调试模式", "")) & If(MODE_OFFLINE, " | 离线模式", "")
        If Not IsNothing(frmHomeLeft) Then frmHomeLeft.ChangeWidth()
        If MODE_OFFLINE And Not IsNothing(frmHomeRight) Then frmHomeRight.ChangeLoginMethod(LoginMethods.Legacy)
        '更新状态
        LastRefreshOffline = MODE_OFFLINE
    End Sub

    ''' <summary>
    ''' 检查MC版本。
    ''' </summary>
    ''' <param name="Version">至少包含完整路径的MCVersion类。</param>
    ''' <param name="CheckInherit">是否对依赖版本进行递归检查。</param>
    ''' <remarks></remarks>
    Public Sub CheckMCVersion(ByRef Version As MCVersion, Optional ByVal CheckInherit As Boolean = False)

        '检查Json文件
        If Not File.Exists(Version.Path & Version.Name & ".json") Then
            If File.Exists(Version.Path & Version.Name & ".jar") Then
                '存在Jar，但是不存在Json
                Version.VersionCheckResult = VersionCheckState.JSON_NOT_EXIST
            Else
                'Jar与Json均不存在
                Version.VersionCheckResult = VersionCheckState.EMPTY_FOLDER
            End If
            Exit Sub
        End If

        '尝试读取Json
        Try
            If Version.JsonText.Length = 0 Then
            End If
        Catch
            Version.VersionCheckResult = VersionCheckState.JSON_CANT_READ
            Exit Sub
        End Try

        '检查Assets与依赖版本
        Try
            Version.InheritVersion = If(Version.Json("inheritsFrom"), "").ToString
            If IsNothing(Version.Json("assets")) Then
                If Version.InheritVersion = "" Then
                    Version.Assets = ""
                Else
                    Version.Assets = If(New MCVersion(Version.InheritVersion).Json("assets"), "").ToString
                End If
            Else
                Version.Assets = Version.Json("assets")
            End If
        Catch
            Version.Assets = ""
        End Try

        If Not Version.InheritVersion = "" Then
            '存在依赖版本
            If Not Directory.Exists(PATH_MC & "versions\" & Version.InheritVersion) Then
                '依赖版本不存在
                Version.VersionCheckResult = VersionCheckState.INHERITS_NOT_EXIST
                Exit Sub
            End If
            '检查依赖版本问题
            If CheckInherit Then
                Dim InheritVersion As New MCVersion(Version.InheritVersion)
                CheckMCVersion(InheritVersion)
                If Not InheritVersion.VersionCheckResult = VersionCheckState.NO_PROBLEM Then
                    '依赖版本存在问题
                    Version.VersionCheckResult = VersionCheckState.INHERITS_EXCEPTION
                    Exit Sub
                End If
            End If
        End If

        '获取全部引用
        Dim LibrariesName As New ArrayList
        Try
            For Each File As JToken In Version.Json("libraries")
                LibrariesName.Add(File("name").ToString)
            Next
        Catch
            Version.VersionCheckResult = VersionCheckState.JSON_CANT_READ
            Exit Sub
        End Try

        '获取主Jar路径
        For Each Library As String In LibrariesName

            If ReadIni("setup", "LaunchMending", "True") Then
                '允许自动修复
                If Library.Contains("net.minecraftforge") Or Library.Contains("optifine:OptiFine") Then
                    Version.VersionCheckResult = VersionCheckState.NO_PROBLEM
                    Exit Sub
                End If
            Else
                '不允许自动修复
                If Library.Contains("net.minecraftforge") Or Library.Contains("optifine:OptiFine") Then
                    '找到主Jar
                    If File.Exists(GetPathFromLibrary(Library)) Then
                        Version.VersionCheckResult = VersionCheckState.NO_PROBLEM
                    Else
                        Version.VersionCheckResult = VersionCheckState.JAR_NOT_EXIST
                    End If
                    Exit Sub
                End If
            End If

        Next

        '主Jar为原版

        If File.Exists(Version.Path & Version.Name & ".jar") Then
            Version.VersionCheckResult = VersionCheckState.NO_PROBLEM
        Else
            Version.VersionCheckResult = VersionCheckState.JAR_NOT_EXIST
        End If

    End Sub

    ''' <summary>
    ''' 从Json文件中的Library名获取文件所在的完整路径。
    ''' </summary>
    ''' <param name="Library"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetPathFromLibrary(ByVal Library As String) As String
        Dim SourceArray = Library.Split(":")
        Return PATH_MC & "libraries\" &
                                      SourceArray(0).Replace(".", "\") & "\" &
                                      SourceArray(1) & "\" &
                                      SourceArray(2) & "\" &
                                      SourceArray(1) & "-" & SourceArray(2) & ".jar"
    End Function

    Private IsSettingJavaEnvironment As Boolean = False
    ''' <summary>
    ''' 配置Java运行环境。必须在非主线程调用。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub SetJavaEnvironment()
        If IsSettingJavaEnvironment Then Exit Sub
        IsSettingJavaEnvironment = True

        '处理环境变量字符串，包括去重、去空白
        Dim Env As String = PathEnv
        Dim EnvironArray As Object() = ArrayNoDouble(Split(Env.Replace(";;", ";") & ";" & PATH_JAVA, ";")).ToArray
        Dim NewEnv As String = Join(EnvironArray, ";").Replace(";;", ";")
        Try
            log("[System] 设置环境变量：" & NewEnv)
            OutputFileInResource("PCLAM", PATH & "PCL\PCL Admin Manager.exe", False)
            Process.Start(PATH & "PCL\PCL Admin Manager.exe", "Set Environment Variable:" & NewEnv)
            PathEnv = NewEnv
            ShowHint(New HintConverter("正在配置游戏环境，如果有安全提示请允许！（如果反复出现本提示，重启电脑即可）", HintState.Warn))
        Catch ex As Exception
            PATH_JAVA = ""
            ExShow(ex, "设置环境变量时出现异常")
            ShowHint(New HintConverter("配置游戏环境失败：" & GetStringFromException(ex) & "（尝试重启电脑或关闭杀毒软件）", HintState.Critical))
            IsSettingJavaEnvironment = False
            Exit Sub
        End Try

        ''等待并检查
        'For i As Integer = 1 To 10
        '    Dim a = PathEnv
        '    Dim b = Mid(PATH_JAVA, 1, Len(PATH_JAVA) - 1)
        '    If PathEnv.Contains(Mid(PATH_JAVA, 1, Len(PATH_JAVA) - 1)) Then
        '        ShowHint(New HintConverter("配置游戏环境成功！", HintState.FINISH))
        '        IsSettingJavaEnvironment = False
        '        Exit Sub
        '    Else
        '        Thread.Sleep(2000)
        '    End If
        'Next i
        'ShowHint(New HintConverter("配置游戏环境失败！请尝试重启PCL或关闭杀毒软件。", HintState.CRITICAL))
        IsSettingJavaEnvironment = False
    End Sub

End Module
