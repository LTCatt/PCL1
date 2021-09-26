Imports Ionic.Zip

Public Class formDownloadLeft

    '用于加载的Timer
    Private Sub timerLoad_Tick() Handles timerLoad.Tick
        Try

            '获取基本信息
            Dim NowState As LoadState
            Select Case selecter.SelectIndexName
                Case "Minecraft"
                    NowState = MinecraftState
                Case "OptiFine"
                    NowState = OptiFineState
                Case "Forge"
                    NowState = If(ForgeVersionState = LoadState.Loaded, ForgeState, ForgeVersionState)
                Case Else
                    Exit Sub
            End Select

            If selecter.SelectIndexName = "Forge" And ForgeVersionState = LoadState.Loaded Then
                'Forge状态处理
                Select Case NowState
                    Case LoadState.Loaded
                        Exit Sub
                    Case LoadState.Loading
                        labRight.Content = "加载中"
                        Exit Sub
                    Case LoadState.Failed
                        labRight.Content = "加载失败"
                        Exit Sub
                    Case LoadState.NoConnection
                        labRight.Content = "没有网络连接"
                        Exit Sub
                    Case LoadState.Success
                        labRight.Content = ""
                End Select
            Else
                '基本事态处理，这里自动控制了两个主UI的切换
                Select Case NowState
                    Case LoadState.Loaded
                        Exit Sub
                    Case LoadState.Loading
                        labLoading.Content = "加载中"
                        labLoading.Visibility = Visibility.Visible
                        panFinish.Visibility = Visibility.Hidden
                        Exit Sub
                    Case LoadState.Failed
                        labLoading.Content = "加载失败，点击以重试"
                        labLoading.Visibility = Visibility.Visible
                        panFinish.Visibility = Visibility.Hidden
                        Exit Sub
                    Case LoadState.NoConnection
                        labLoading.Content = "没有网络连接"
                        labLoading.Visibility = Visibility.Visible
                        panFinish.Visibility = Visibility.Hidden
                        Exit Sub
                    Case LoadState.Success
                        '开始加载，把控件显示出来
                        labLoading.Visibility = Visibility.Collapsed
                        panFinish.Visibility = Visibility.Visible
                End Select
            End If

            '加载
            Select Case selecter.SelectIndexName
                Case "Minecraft"
                    '结束显示
                    MinecraftState = LoadState.Loaded
                    '加载预览版，预览版总是列表第一个
                    item2.MainText = MinecraftArray(0).id
                    item2.Tag = MinecraftArray(0)
                    '是否加载正式版的标记
                    Dim item1Loaded As Boolean = False
                    '循环添加
                    For Each ver As MinecraftVersion In MinecraftArray
                        '判断正式版
                        If ver.type = "release" And Not item1Loaded Then
                            item1Loaded = True
                            item1.MainText = ver.id
                            item1.Tag = ver
                        End If
                        '判断Logo样式
                        Dim Photo As String = "Grass"
                        If ver.type = "snapshot" Then Photo = "CommandBlock"
                        If ver.type = "fool" Then Photo = "Dirt"
                        If ver.type.Contains("old") Then Photo = "CobbleStone"
                        '添加控件
                        Dim item As ListItem = New ListItem With {.UseLayoutRounding = True, .Tag = ver, .CanCheck = False, .ButtonLogo = New BitmapImage(New Uri("/Images/appbar.inbox.in.png", UriKind.Relative)), .MainText = ver.id, .SubText = ver.time, .Name = "list" & GetUUID(), .Logo = New BitmapImage(New Uri(PATH_IMAGE & "Block-" & Photo & ".png", UriKind.Absolute)), .ToolTip = If(Photo = "Dirt", "该版本由 PCL 特别提供", Nothing)}
                        AddHandler item.ButtonClick, AddressOf MinecraftDownloadClick
                        panVersion.Children.Add(item)
                    Next
                    '最新正式版与最新预览版一样则隐藏一个
                    If item1.MainText = item2.MainText Then
                        item2.Visibility = Visibility.Collapsed
                    Else
                        item2.Visibility = Visibility.Visible
                    End If
                Case "OptiFine"
                    '结束显示
                    OptiFineState = LoadState.Loaded
                    '加载预览版，预览版总是列表第一个
                    item2.MainText = OptiFineArray(0).id.ToString
                    item2.Tag = OptiFineArray(0)
                    '是否加载正式版的标记
                    Dim item1Loaded = False
                    For Each ver As OptiFineVersion In OptiFineArray
                        '判断正式版
                        If Not (ver.id.Contains("pre") Or item1Loaded) Then
                            item1Loaded = True
                            item1.MainText = ver.id
                            item1.Tag = ver
                        End If
                        '判断Logo样式
                        Dim Photo = If(ver.id.Contains("pre"), "CommandBlock", "Grass")
                        '添加控件
                        Dim item As ListItem = New ListItem With {.UseLayoutRounding = True, .Tag = ver, .CanCheck = False, .ButtonLogo = New BitmapImage(New Uri("/Images/appbar.inbox.in.png", UriKind.Relative)), .MainText = ver.id, .SubText = ver.time, .Name = "list" & GetUUID(), .Logo = New BitmapImage(New Uri("/Images/Block-" & Photo & ".png", UriKind.Relative))}
                        AddHandler item.ButtonClick, AddressOf OptiFineDownloadStart
                        panVersion.Children.Add(item)
                    Next
                    '最新正式版与最新预览版一样则隐藏一个
                    If item1.MainText = item2.MainText Then
                        item2.Visibility = Visibility.Collapsed
                    Else
                        item2.Visibility = Visibility.Visible
                    End If
                    ''使用 BMCLAPI 获取 OptiFine 源时进行隐藏
                    'If selecter.SelectIndexName = "OptiFine" Then
                    '    If OptiFineArray.Count > 0 Then
                    '        item1.Visibility = If(OptiFineArray(0).time = "", Visibility.Collapsed, Visibility.Visible)
                    '        item2.Visibility = If(OptiFineArray(0).time = "", Visibility.Collapsed, Visibility.Visible)
                    '    End If
                    'End If
                Case "Forge"
                    If ForgeVersionState = LoadState.Success Then 'ForgeVersion
                        '结束显示
                        ForgeVersionState = LoadState.Loaded
                        '加载列表
                        panForge.Children.Clear()
                        For Each Version As String In ForgeVersionArray
                            '添加控件
                            Dim item As ListItem = New ListItem With {.SubText = "", .ShowButton = False, .UseLayoutRounding = True, .CanCheck = True, .MainText = Version, .Name = "list" & GetUUID(), .Logo = New BitmapImage(New Uri("/Images/Block-Grass.png", UriKind.Relative))}
                            AddHandler item.Change, AddressOf ChangeForgeVersionSelection
                            panForge.Children.Add(item)
                        Next
                        '引发第一个的改变事件
                        If panForge.Children.Count > 0 Then
                            CType(panForge.Children(0), ListItem).Checked = True
                            ChangeForgeVersionSelection(CType(panForge.Children(0), ListItem), Nothing)
                        End If
                        AniStart(AaStack(panForge), "DownloadLeftShowForge")
                        scrollForge.Value = 0
                        scrollVersion.Visibility = Visibility.Hidden
                    Else 'Forge
                        panVersionHost.Visibility = Visibility.Collapsed
                        scrollVersion.Visibility = Visibility.Visible
                        Try
                            '结束显示
                            ForgeState = LoadState.Loaded
                            labRight.Content = ""
                            Dim array = ForgeArray
                            panVersion.Children.Clear()
                            For Each ver As ForgeVersion In array
                                Dim item As ListItem = New ListItem With {.UseLayoutRounding = True, .Tag = ver, .CanCheck = False, .ButtonLogo = New BitmapImage(New Uri("/Images/appbar.inbox.in.png", UriKind.Relative)), .MainText = "Forge " & ver.version, .SubText = ver.time, .Name = "list" & GetUUID(), .Logo = New BitmapImage(New Uri("/Images/Block-Anvil.png", UriKind.Relative))}
                                AddHandler item.ButtonClick, AddressOf ForgeDownloadStart
                                panVersion.Children.Add(item)
                            Next
                        Catch
                            '时不时会出现集合已修改的Exception，事实证明重新加载一次就成了，管它啥原因嘞
                            ForgeState = LoadState.Success
                        End Try
                    End If
                    panVersionHost.Visibility = Visibility.Visible
            End Select

        Catch ex As Exception
            ExShow(ex, "下载列表检测时钟出错", ErrorLevel.Slient)
        End Try
    End Sub

    '改变左边选择条
    Private Sub ChangeSelection(ByVal Selection As String) Handles selecter.SelectionChange
        '初始化滚动条
        panVersion.Children.Clear()
        scrollVersion.ValueMe = 0
        '初始化页面
        labLoading.Content = "加载中"
        labLoading.Visibility = Visibility.Visible
        panFinish.Visibility = Visibility.Hidden
        scrollVersion.Visibility = Visibility.Visible
        '分情况处理
        Select Case Selection
            Case "Minecraft"
                '状态改变
                Select Case MinecraftState
                    Case LoadState.Loaded
                        '已经加载过了，需要刷新显示
                        MinecraftState = LoadState.Success
                    Case LoadState.Failed
                        '加载失败，自动刷新
                        MinecraftState = LoadState.Loading
                        Pool.Add(New Thread(Sub() GetMinecraftBasic(True)))
                End Select
                '对应页面
                panForgeHost.Visibility = Visibility.Collapsed
                scrollForge.Visibility = Visibility.Collapsed
                item1.Visibility = Visibility.Visible
                item2.Visibility = Visibility.Visible
                labRight.Content = ""
            Case "OptiFine"
                '状态改变
                Select Case OptiFineState
                    Case LoadState.Loaded
                        '已经加载过了，需要刷新显示
                        OptiFineState = LoadState.Success
                    Case LoadState.Failed
                        '加载失败，自动刷新
                        OptiFineState = LoadState.Loading
                        Pool.Add(New Thread(AddressOf GetOptiFineBasic))
                End Select
                '对应页面
                panForgeHost.Visibility = Visibility.Collapsed
                scrollForge.Visibility = Visibility.Collapsed
                item1.Visibility = Visibility.Visible
                item2.Visibility = Visibility.Visible
                labRight.Content = ""
            Case "Forge"
                '状态改变
                Select Case ForgeVersionState
                    Case LoadState.Loaded
                        '已经加载过了，需要刷新显示
                        ForgeVersionState = LoadState.Success
                    Case LoadState.Failed
                        '加载失败，自动刷新
                        ForgeVersionState = LoadState.Loading
                        Pool.Add(New Thread(AddressOf GetForgeVersionBasic))
                End Select
                '对应页面
                panForgeHost.Visibility = Visibility.Visible
                scrollForge.Visibility = Visibility.Visible
                item1.Visibility = Visibility.Collapsed
                item2.Visibility = Visibility.Collapsed
                labRight.Content = "加载中"
        End Select
    End Sub

    '点击重试
    Private Sub labLoading_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles labLoading.MouseDown
        Select Case selecter.SelectIndexName
            Case "Minecraft"
                If MinecraftState = LoadState.Failed Then
                    Dim th As New Thread(Sub() GetMinecraftBasic(True))
                    th.Start()
                End If
            Case "OptiFine"
                If OptiFineState = LoadState.Failed Then
                    Dim th As New Thread(AddressOf GetOptiFineBasic)
                    th.Start()
                End If
            Case "Forge"
                If ForgeVersionState = LoadState.Failed Then
                    Dim th As New Thread(AddressOf GetForgeVersionBasic)
                    th.Start()
                End If
        End Select
    End Sub

    '加载的状态
    Public Enum LoadState As Byte
        ''' <summary>
        ''' 正在加载
        ''' </summary>
        ''' <remarks></remarks>
        Loading = 0
        ''' <summary>
        ''' 已经加载结束，但是没有显示
        ''' </summary>
        ''' <remarks></remarks>
        Success = 1
        ''' <summary>
        ''' 加载失败
        ''' </summary>
        ''' <remarks></remarks>
        Failed = 2
        ''' <summary>
        ''' 加载失败（没有联网）
        ''' </summary>
        ''' <remarks></remarks>
        NoConnection = 3
        ''' <summary>
        ''' 已经加载结束，并且已显示，此时不执行Timer代码
        ''' </summary>
        ''' <remarks></remarks>
        Loaded = 4
    End Enum

#Region "窗体基础"

    Dim PageLoaded As Boolean = False
    Private Sub panVersion_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles panVersion.Loaded
        Dim EnabledList As New ArrayList
        If ReadIni("setup", "UiHiddenMinecraft", "False") = "False" Then EnabledList.Add("Minecraft")
        If ReadIni("setup", "UiHiddenOptiFine", "False") = "False" Then EnabledList.Add("OptiFine")
        If ReadIni("setup", "UiHiddenForge", "False") = "False" Then EnabledList.Add("Forge")
        Select Case EnabledList.Count
            Case 0
                selecter.ShowList = {"Nothing"}
                panMain.Visibility = Visibility.Collapsed
            Case 1
                selecter.ShowList = EnabledList.ToArray
                selecter.Visibility = Visibility.Collapsed
                panMain.Visibility = Visibility.Visible
            Case 2, 3
                selecter.ShowList = EnabledList.ToArray
                selecter.Visibility = Visibility.Visible
                panMain.Visibility = Visibility.Visible
        End Select
        If Not PageLoaded Then
            PageLoaded = True
            panMain.UpdateLayout()
        End If
        If scrollVersion.SetControl(panVersion, True) Then AddHandler panVersionHost.MouseWheel, AddressOf scrollVersion.RunMouseWheel
    End Sub
    Private Sub panForge_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles panForge.Loaded
        If scrollForge.SetControl(panForge, True) Then AddHandler panForgeHost.MouseWheel, AddressOf scrollForge.RunMouseWheel
    End Sub

    Private Sub panNew_SizeChanged(ByVal sender As Object, ByVal e As System.Windows.SizeChangedEventArgs) Handles panNew.SizeChanged
        item2.Width = Int((panNew.ActualWidth - 9) / 2)
    End Sub

#End Region

#Region "Minecraft"

    Public MinecraftState As LoadState = LoadState.Failed '加载状态
    Public MinecraftArray As New ArrayList '代码分析的数组
    Public MinecraftInfo As String = "" '获取的代码
    Public Structure MinecraftVersion
        Dim id As String
        Dim type As String
        Dim url As String
        Dim time As String
    End Structure
    Public Sub GetMinecraftBasic(ByVal IsShowHint As Boolean)

        '初始化
        MinecraftState = LoadState.Loading
        MinecraftArray = New ArrayList
        If MODE_OFFLINE Then
            MinecraftState = LoadState.NoConnection
            Exit Sub
        End If

        '获取版本列表
        Try
            If ReadIni("setup", "DownVersion", "0") = "0" Then
                MinecraftInfo = GetWebsiteCode("https://launchermeta.mojang.com/mc/game/version_manifest.json", Encoding.Default)
                If MinecraftInfo = "" Then
                    MinecraftInfo = GetWebsiteCode("http://bmclapi2.bangbang93.com/mc/game/version_manifest.json", Encoding.Default)
                    If IsShowHint And Not MinecraftInfo = "" Then ShowHint("获取官方 Minecraft 版本列表失败，自动切换为 BMCLAPI")
                End If
            Else
                MinecraftInfo = GetWebsiteCode("http://bmclapi2.bangbang93.com/mc/game/version_manifest.json", Encoding.Default)
                If MinecraftInfo = "" Then
                    MinecraftInfo = GetWebsiteCode("https://launchermeta.mojang.com/mc/game/version_manifest.json", Encoding.Default)
                End If
            End If
            If Len(MinecraftInfo) < 300 Then Throw New WebException("获取到的列表长度不足：" & MinecraftInfo)
        Catch ex As Exception
            ExShow(ex, "获取 Minecraft 版本列表失败")
            MinecraftInfo = ""
            MinecraftState = LoadState.Failed
            Exit Sub
        End Try
        log("[DownloadLeft] 获取 Minecraft 版本列表成功")

        '加载版本列表信息
        Try
            log("[DownloadLeft] 加载 Minecraft 版本列表信息开始")
            '预处理
            Dim ids As ArrayList = RegexSearch(MinecraftInfo, "(?<=id"": "")[^""]+")
            ids.AddRange(RegexSearch(MinecraftInfo, "(?<=id"":"")[^""]+"))
            Dim urls As ArrayList = RegexSearch(MinecraftInfo, "(?<=url"": "")[^""]+")
            urls.AddRange(RegexSearch(MinecraftInfo, "(?<=url"":"")[^""]+"))
            Dim times As ArrayList = RegexSearch(MinecraftInfo, "(?<=releaseTime"": "")[^T]+")
            times.AddRange(RegexSearch(MinecraftInfo, "(?<=releaseTime"":"")[^T]+"))
            Dim types As ArrayList = RegexSearch(MinecraftInfo, "(?<=type"": "")[^""]+")
            types.AddRange(RegexSearch(MinecraftInfo, "(?<=type"":"")[^""]+"))
            '长度检查
            If Not (ids.Count = urls.Count And urls.Count = times.Count And times.Count = types.Count) Then Throw New WebException("获取到的列表长度不等")
            '添加
            For i = 0 To ids.Count - 1
                MinecraftArray.Add(New MinecraftVersion With {.id = ids(i), .url = urls(i), .time = times(i), .type = types(i)})
            Next
            '确认最新版本
            If MinecraftArray.Count > 0 Then
                Select Case ReadIni("setup", "LastMinecraftVersion")
                    Case ""
                        '没有执行过，不提醒
                        WriteIni("setup", "LastMinecraftVersion", CType(MinecraftArray(0), MinecraftVersion).id)
                    Case CType(MinecraftArray(0), MinecraftVersion).id
                        '相同，不提醒
                    Case Else
                        '不相同，提醒
                        WriteIni("setup", "LastMinecraftVersion", CType(MinecraftArray(0), MinecraftVersion).id)
                        If ReadIni("setup", "HomeUpdate", "True") = "True" Then
                            If Not (ReadIni("setup", "HomeUpdateRelease", "False") = "True" And (CType(MinecraftArray(0), MinecraftVersion).id.Contains("w") Or CType(MinecraftArray(0), MinecraftVersion).id.Contains("pre"))) Then
                                frmHomeRight.Dispatcher.Invoke(Sub() frmHomeRight.ShowUpdate("发现游戏更新：" & CType(MinecraftArray(0), MinecraftVersion).id, formHomeRight.UpdateType.MINECRAFT))
                            End If
                        End If
                End Select
            End If
        Catch ex As Exception
            ExShow(ex, "加载 Minecraft 版本列表信息失败")
            MinecraftInfo = ""
            MinecraftState = LoadState.Failed
            Exit Sub
        End Try

        log("[DownloadLeft] 加载 Minecraft 版本列表信息成功")
        MinecraftState = LoadState.Success

    End Sub

    Private Sub MinecraftDownloadClick(ByVal sender As ListItem, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles item1.ButtonClick, item2.ButtonClick
        If IsNothing(sender.Tag) Or Not selecter.SelectIndexName = "Minecraft" Then Exit Sub
        MinecraftDownloadStart(sender.Tag)
    End Sub
    Private Sub MinecraftDownloadStart(Ver As MinecraftVersion)
        Dim realID As String = Ver.id
        Dim local As String = PATH_MC & "versions\" & realID & "\"
        If WebGroups.ContainsKey("Minecraft " & realID) Or WebGroups.ContainsKey("Minecraft " & realID & " 信息") Then
            ShowHint(New HintConverter("正在下载中，请勿重复操作", HintState.Warn))
            Exit Sub
        End If
        If Not Directory.Exists(local) Then Directory.CreateDirectory(local)
        '检测已存在版本
        If GetFileSize(local & realID & ".jar") > 1024 Then
            If MyMsgbox("该版本已存在，是否要删除当前版本并且重新下载？", "提示", "确定", "取消") = 1 Then
                Try
                    File.Delete(local & realID & ".json")
                    File.Delete(local & realID & ".jar")
                Catch ex As Exception
                    ExShow(ex, "删除版本失败", ErrorLevel.MsgboxWithoutFeedback)
                End Try
            Else
                Exit Sub
            End If
        End If
        '下载
        log("[DownloadLeft] 下载版本：" & realID)
        ShowHint("Minecraft " & realID & " 开始下载")
        SendStat("下载", "Minecraft", realID)
        Dim th As New Thread(Sub()
                                 Try
                                     Directory.CreateDirectory(local)
                                     Dim DownloadList As ArrayList
                                     If ReadIni("setup", "DownMinecraft", "1") = "0" Then
                                         '官方源优先
                                         DownloadList = New ArrayList({
                                                     Ver.url.Replace("https://launcher.mojang.com", "http://bmclapi2.bangbang93.com").Replace("https://launchermeta.mojang.com", "http://bmclapi2.bangbang93.com"),
                                                     Ver.url
                                                 })
                                     Else
                                         'BMCLAPI 优先
                                         DownloadList = New ArrayList({
                                                     Ver.url,
                                                     Ver.url.Replace("https://launcher.mojang.com", "http://bmclapi2.bangbang93.com").Replace("https://launchermeta.mojang.com", "http://bmclapi2.bangbang93.com")
                                                 })
                                     End If
                                     WebStart({
                                              New WebRequireFile With {.WebURLs = DownloadList, .LocalFolder = local, .LocalName = realID & ".json", .KnownFileSize = 1024 * 2}}, "Minecraft " & realID & " 信息",
                                              Sub()
                                                  Try

                                                      Dim json As JObject = Newtonsoft.Json.JsonConvert.DeserializeObject(ReadFileToEnd(local & realID & ".json"))
                                                      Dim size As Integer
                                                      Try
                                                          size = json("downloads")("client")("size").ToString
                                                      Catch
                                                          size = 1024 * 50
                                                      End Try
                                                      Dim url As New ArrayList
                                                      If ReadIni("setup", "DownMinecraft", "1") = "0" Then
                                                          'Mojang 优先
                                                          url.Add(json("downloads")("client")("url").ToString)
                                                          url.Add(json("downloads")("client")("url").ToString.Replace("https://launcher.mojang.com", "http://bmclapi2.bangbang93.com").Replace("https://launchermeta.mojang.com", "http://bmclapi2.bangbang93.com"))
                                                          url.Add(json("downloads")("client")("url").ToString)
                                                          url.Add(json("downloads")("client")("url").ToString.Replace("https://launcher.mojang.com", "http://bmclapi2.bangbang93.com").Replace("https://launchermeta.mojang.com", "http://bmclapi2.bangbang93.com"))
                                                      Else
                                                          'BMCLAPI 优先
                                                          url.Add(json("downloads")("client")("url").ToString.Replace("https://launcher.mojang.com", "http://bmclapi2.bangbang93.com").Replace("https://launchermeta.mojang.com", "http://bmclapi2.bangbang93.com"))
                                                          url.Add(json("downloads")("client")("url").ToString)
                                                          url.Add(json("downloads")("client")("url").ToString.Replace("https://launcher.mojang.com", "http://bmclapi2.bangbang93.com").Replace("https://launchermeta.mojang.com", "http://bmclapi2.bangbang93.com"))
                                                          url.Add(json("downloads")("client")("url").ToString)
                                                      End If
                                                      WebStart({New WebRequireFile With {.WebURLs = url, .LocalFolder = local, .LocalName = realID & ".jar", .KnownFileSize = size}}, "Minecraft " & realID, AddressOf MinecraftDownloadSuccess, AddressOf MinecraftDownloadFail, If(size = 1024 * 50, WebRequireSize.AtLeast, WebRequireSize.Known))

                                                  Catch ex As Exception
                                                      ExShow(ex, "下载版本失败：" & realID, ErrorLevel.AllUsers)
                                                      Try
                                                          File.Delete(local & realID & ".json")
                                                          File.Delete(local & realID & ".jar")
                                                      Catch : End Try
                                                      Application.Current.Dispatcher.Invoke(CType(AddressOf MinecraftDownloadFail, ParameterizedThreadStart), realID)
                                                  End Try
                                              End Sub,
                                              AddressOf MinecraftDownloadFail,
                                              WebRequireSize.AtLeast)
                                 Catch ex As Exception
                                     ExShow(ex, "下载版本失败：" & realID, ErrorLevel.AllUsers)
                                     Try
                                         File.Delete(local & realID & ".json")
                                         File.Delete(local & realID & ".jar")
                                     Catch : End Try
                                     Application.Current.Dispatcher.Invoke(CType(AddressOf MinecraftDownloadFail, ParameterizedThreadStart), realID)
                                 End Try
                             End Sub)
        th.Start()
    End Sub
    Private Sub MinecraftDownloadSuccess(ByVal Name As String)
        Try
            Pool.Add(New Thread(AddressOf PoolVersionList))
            ShowHint(New HintConverter(Name & " 下载成功", HintState.Finish))
        Catch ex As Exception
            ExShow(ex, "Minecraft 下载结束后的处理异常：" & Name)
            ShowHint(New HintConverter(Name & " 下载失败：" & GetStringFromException(ex) & "！", HintState.Critical))
        End Try
    End Sub
    Private Sub MinecraftDownloadFail(ByVal Name As String)
        ShowHint(New HintConverter(Name & " 下载失败", HintState.Critical))
    End Sub

    Private Function MinecraftGet(VersionName As String) As MinecraftVersion
        If MinecraftArray.Count = 0 Then Return Nothing
        If VersionName.EndsWith(".0") And VersionName.StartsWith("1.") Then VersionName = Mid(VersionName, 1, Len(VersionName) - 2)

        '获取对应版本
        Dim SelectVersion As MinecraftVersion = Nothing
        For Each Ver As MinecraftVersion In MinecraftArray
            If Ver.id = VersionName Then
                SelectVersion = Ver
                Exit For
            End If
        Next
        Return SelectVersion
    End Function

#End Region

#Region "OptiFine"

    Public OptiFineState As LoadState = LoadState.Failed '加载状态
    Public OptiFineArray As New ArrayList '代码分析的数组
    Public OptiFineInfo As String = "" '获取的代码
    Public Structure OptiFineVersion
        Dim id As String
        Dim url As String
        Dim time As String
        Dim getByBMCLAPI As Boolean
    End Structure
    Public Sub GetOptiFineBasic()

        '初始化
        OptiFineState = LoadState.Loading
        OptiFineArray = New ArrayList
        OptiFineInfo = ""
        If MODE_OFFLINE Then
            OptiFineState = LoadState.NoConnection
            Exit Sub
        End If

        '官方源

        '获取版本列表
        Try
            log("[DownloadLeft] 获取 OptiFine 版本列表开始")
            OptiFineInfo = GetWebsiteCode("http://www.optifine.net/downloads", Encoding.Default)
            If Len(OptiFineInfo) < 200 Then Throw New WebException("获取到的列表长度不足：" & OptiFineInfo)
        Catch ex As Exception
            ExShow(ex, "获取 OptiFine 版本列表失败")
            OptiFineInfo = ""
            GoTo BMCLAPI
        End Try
        log("[DownloadLeft] 获取 OptiFine 版本列表成功")

        '加载版本列表信息
        Try
            '预处理
            Dim ids As ArrayList = RegexSearch(OptiFineInfo, "(?<=downloadLineFile(First)?'>)[^<]*")
            Dim urls As ArrayList = RegexSearch(OptiFineInfo, "(?<=downloadLineMirror'><a href="")[^""]*")
            Dim times As ArrayList = RegexSearch(OptiFineInfo, "(?<=downloadLineDate'>)[^<]*")
            '长度检查
            If Not (ids.Count = urls.Count And urls.Count = times.Count) Then Throw New WebException("获取到的列表长度不等")
            '添加
            For i = 0 To ids.Count - 1
                OptiFineArray.Add(New OptiFineVersion With {
                                  .id = ids(i).Replace("OptiFine ", ""),
                                  .url = urls(i),
                                  .time = times(i).ToString.Split(".")(2) & "-" & times(i).ToString.Split(".")(1) & "-" & times(i).ToString.Split(".")(0),
                                  .getByBMCLAPI = False})
            Next
            GoTo Success
        Catch ex As Exception
            ExShow(ex, "加载 OptiFine 版本列表信息失败")
            OptiFineInfo = ""
            GoTo BMCLAPI
        End Try

        'BMCLAPI 源
BMCLAPI:

        '获取版本列表
        Try
            log("[DownloadLeft] 使用 BMCLAPI 源获取 OptiFine 版本列表开始")
            OptiFineInfo = GetWebsiteCode("http://bmclapi2.bangbang93.com/optifine/versionList", Encoding.Default)
            If Len(OptiFineInfo) < 200 Then Throw New WebException("获取到的列表长度不足：" & OptiFineInfo)
        Catch ex As Exception
            ExShow(ex, "使用 BMCLAPI 源获取 OptiFine 版本列表失败")
            OptiFineInfo = ""
            OptiFineState = LoadState.Failed
            Exit Sub
        End Try
        log("[DownloadLeft] 使用 BMCLAPI 源获取 OptiFine 版本列表成功")

        '加载版本列表信息
        Try
            '预处理
            Dim filename As ArrayList = RegexSearch(OptiFineInfo, "(?<=filename"":"")[^""]*")
            Dim id As ArrayList = RegexSearch(OptiFineInfo, "(?<=OptiFine_).*?(?=\.jar)")
            '添加
            For i = 0 To filename.Count - 1
                OptiFineArray.Add(New OptiFineVersion With {
                                  .id = id(i).Replace("_", " "),
                                  .url = "http://optifine.net/adloadx?f=" & filename(i),
                                  .time = "",
                                  .getByBMCLAPI = True
                                  })
            Next
            If selecter.SelectIndexName = "OptiFine" Then ShowHint("OptiFine 官方列表获取失败，列表由 BMCLAPI 提供")
            GoTo Success
        Catch ex As Exception
            ExShow(ex, "加载 OptiFine 版本列表信息失败")
            OptiFineInfo = ""
            OptiFineState = LoadState.Failed
            Exit Sub
        End Try

Success:
        log("[DownloadLeft] 加载 OptiFine 版本列表信息成功")
        OptiFineState = LoadState.Success

    End Sub

    Private Sub OptiFineDownloadStart(ByVal sender As ListItem, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles item1.ButtonClick, item2.ButtonClick
        If IsNothing(sender.Tag) Or Not selecter.SelectIndexName = "OptiFine" Then Exit Sub
        '初始化信息
        Dim ver As OptiFineVersion = sender.Tag
        If WebGroups.ContainsKey("OptiFine " & ver.id) Then
            ShowHint(New HintConverter("正在下载中，请勿重复操作", HintState.Warn))
            Exit Sub
        End If
        '获取数据
        Try
            sender.ShowButton = False
            Dim Version As String = Split(ver.id)(0)
            Dim BMCLAPIAddress As String = ""
            If ver.id.Contains("pre") Then
                BMCLAPIAddress = "http://bmclapi2.bangbang93.com/maven/com/optifine/" & ver.id.Split(" ")(0) & "/preview_OptiFine_" & ver.id.Replace(" ", "_").Replace("_pre", "") & ".jar"
            Else
                BMCLAPIAddress = "http://bmclapi2.bangbang93.com/maven/com/optifine/" & ver.id.Split(" ")(0) & "/OptiFine_" & ver.id.Replace(" ", "_") & ".jar"
            End If
            '下载
            log("[DownloadLeft] 下载 OptiFine：" & ver.id)
            ShowHint("OptiFine " & ver.id & " 开始下载")
            SendStat("下载", "OptiFine", ver.id)
            Dim th As New Thread(Sub()
                                     Try
                                         If ver.getByBMCLAPI Then
                                             WebStart({New WebRequireFile With {.WebURLs = New ArrayList From {
                                                          BMCLAPIAddress,
                                                          BMCLAPIAddress
                                                      }, .LocalFolder = PATH_DOWNLOAD, .LocalName = "OptiFine_" & ver.id.Replace(" ", "_") & ".jar", .KnownFileSize = 1024 * 64}
                                                  }, "OptiFine " & ver.id, AddressOf OptiFineDownloadSuccess, AddressOf OptiFineDownloadFail, WebRequireSize.AtLeast)
                                         Else
                                             Dim OfficialAddress As String = ""
                                             Try
                                                 OfficialAddress = RegexSearch(GetWebsiteCode(ver.url, New UTF8Encoding(False)), "(?<=Download[^\\s\\S]+a href="")[^""]+")(0)
                                             Catch
                                             End Try
                                             If ReadIni("setup", "DownOptiFine", "1") = "0" Then
                                                 '官方
                                                 WebStart({New WebRequireFile With {.WebURLs = New ArrayList From {
                                                              "http://optifine.net/" & OfficialAddress,
                                                              BMCLAPIAddress,
                                                              "http://optifine.net/" & OfficialAddress,
                                                              BMCLAPIAddress
                                                          }, .LocalFolder = PATH_DOWNLOAD, .LocalName = "OptiFine_" & ver.id.Replace(" ", "_") & ".jar", .KnownFileSize = 1024 * 64}
                                                      }, "OptiFine " & ver.id, AddressOf OptiFineDownloadSuccess, AddressOf OptiFineDownloadFail, WebRequireSize.AtLeast)
                                             Else
                                                 'BMCLAPI
                                                 WebStart({New WebRequireFile With {.WebURLs = New ArrayList From {
                                                              BMCLAPIAddress,
                                                              "http://optifine.net/" & OfficialAddress,
                                                              BMCLAPIAddress,
                                                              "http://optifine.net/" & OfficialAddress
                                                          }, .LocalFolder = PATH_DOWNLOAD, .LocalName = "OptiFine_" & ver.id.Replace(" ", "_") & ".jar", .KnownFileSize = 1024 * 64}
                                                      }, "OptiFine " & ver.id, AddressOf OptiFineDownloadSuccess, AddressOf OptiFineDownloadFail, WebRequireSize.AtLeast)
                                             End If
                                         End If
                                     Catch ex As Exception
                                         ExShow(ex, "下载 OptiFine 失败：" & ver.id)
                                         Application.Current.Dispatcher.Invoke(CType(AddressOf OptiFineDownloadFail, ParameterizedThreadStart), ver.id)
                                     End Try
                                     frmMain.Dispatcher.Invoke(Sub() sender.ShowButton = True)
                                 End Sub)
            th.Start()
            '检查引用版本
            If GetFileSize(PATH_MC & "versions\" & Version & "\" & Version & ".jar") < 1024 Then
                '引用版本不存在
                log("[DownloadLeft] OptiFine 对应的引用版本不存在：" & Version)
                Dim RequireVersion As MinecraftVersion = MinecraftGet(Version)
                If Not IsNothing(RequireVersion.id) Then MinecraftDownloadStart(RequireVersion)
            End If
        Catch ex As Exception
            ExShow(ex, "处理下载地址时出错", ErrorLevel.MsgboxAndFeedback)
            Exit Sub
        End Try
    End Sub
    Private Sub OptiFineDownloadSuccess(ByVal Name As String)
        Dim Version As String = Name.Replace("OptiFine ", "")
        Try
            'If ReadIni("setup", "DownOptiFineOpen", "True") Then RunCMD(PATH_DOWNLOAD & "OptiFine_" & Version.Replace(" ", "_") & ".jar", False)
            Process.Start(PATH_DOWNLOAD)
            ShowHint(New HintConverter("OptiFine " & Version & " 下载成功", HintState.Finish))
        Catch ex As Exception
            ExShow(ex, "OptiFine 下载结束后的处理异常")
            Try
                File.Delete(PATH_DOWNLOAD & "OptiFine_" & Version.Replace(" ", "_") & ".jar")
            Catch : End Try
            ShowHint(New HintConverter("OptiFine " & Version & " 下载失败：" & GetStringFromException(ex) & "！", HintState.Critical))
        End Try
    End Sub
    Private Sub OptiFineDownloadFail(ByVal Name As String)
        Dim Version As String = Name.Replace("OptiFine ", "")
        ShowHint(New HintConverter("OptiFine " & Version & " 下载失败", HintState.Critical))
    End Sub

#End Region

#Region "ForgeVersion"

    Public ForgeVersionState As LoadState = LoadState.Failed '加载状态
    Public ForgeVersionArray As New ArrayList '代码分析的数组
    Public ForgeVersionInfo As String = "" '获取的代码
    Public Sub GetForgeVersionBasic()

        '初始化
        ForgeVersionState = LoadState.Loading
        ForgeVersionArray = New ArrayList
        ForgeVersionInfo = ""
        If MODE_OFFLINE Then
            ForgeVersionState = LoadState.NoConnection
            Exit Sub
        End If

        '获取版本列表
        Try
            log("[DownloadLeft] 获取Forge可用版本列表开始")
            ForgeVersionInfo = GetWebsiteCode("http://bmclapi2.bangbang93.com/forge/minecraft", Encoding.Default)
            If Len(ForgeVersionInfo) < 40 Then Throw New WebException("获取到的列表长度不足：" & ForgeVersionInfo)
        Catch ex As Exception
            ExShow(ex, "获取Forge可用版本列表失败")
            ForgeVersionInfo = ""
            ForgeVersionState = LoadState.Failed
            Exit Sub
        End Try
        log("[DownloadLeft] 获取Forge可用版本列表成功")

        '加载版本列表信息
        Try
            '添加
            'ForgeVersionArray.AddRange(ForgeVersionInfo.Replace("[", "").Replace("]", "").Replace("""", "").Replace("null", "").Replace(",,", "").Split(",")) 'RegexSearch(ForgeVersionInfo, "1.([7-9]{1}.[0-9]+|[0-9]{2}[0-9.]*)(?="")")
            ForgeVersionArray = RegexSearch(ForgeVersionInfo, "1.([0-9]{1,2}.[0-9]+|[0-9]{1,2}[0-9.]*)(?="")")
            '排序
            Dim CurrentPosition As Integer = 1
            ForgeVersionArray.Reverse()
            Do While CurrentPosition <= ForgeVersionArray.Count - 1
                If CurrentPosition < 1 Then CurrentPosition = 1
                Dim Smaller() As String = ForgeVersionArray(CurrentPosition).ToString.Split(".")
                Dim Larger() As String = ForgeVersionArray(CurrentPosition - 1).ToString.Split(".")
                '比较大小
                For i = 0 To 2
                    Select Case Val(If(Larger.Length >= i + 1, Larger(i), "0"))
                        Case Val(If(Smaller.Length >= i + 1, Smaller(i), "0"))
                            '相等则不处理，继续比较下一位
                        Case Is < Val(If(Smaller.Length >= i + 1, Smaller(i), "0"))
                            '较小
                            GoTo Swap
                        Case Else
                            '较大
                            GoTo Finish
                    End Select
                Next
                GoTo Finish
Swap:
                Dim c As String = ForgeVersionArray(CurrentPosition)
                ForgeVersionArray(CurrentPosition) = ForgeVersionArray(CurrentPosition - 1)
                ForgeVersionArray(CurrentPosition - 1) = c
                CurrentPosition = CurrentPosition - 2
Finish:
                CurrentPosition = CurrentPosition + 1
            Loop
        Catch ex As Exception
            ExShow(ex, "加载Forge可用版本列表信息失败")
            ForgeVersionInfo = ""
            ForgeVersionState = LoadState.Failed
            Exit Sub
        End Try

        log("[DownloadLeft] 加载Forge可用版本列表信息成功")
        ForgeVersionState = LoadState.Success

    End Sub

    Private Sub ChangeForgeVersionSelection(ByVal sender As ListItem, ByVal e As EventArgs)
        If Not sender.Checked Then Exit Sub
        panVersion.Children.Clear()
        scrollVersion.Visibility = Visibility.Collapsed
        scrollVersion.Value = 0
        labRight.Content = "加载中"
        ForgeCurrent = sender.MainText
        Dim th = New Thread(AddressOf GetForgeBasic)
        th.Start(sender.MainText)
    End Sub

#End Region

#Region "Forge"

    Public ForgeState As LoadState = LoadState.Loading '加载状态
    Public ForgeArray As New ArrayList '代码分析的数组
    Public ForgeInfo As String = "" '获取的代码
    Public ForgeCurrent As String = "" '目前的加载版本
    Public Structure ForgeVersion
        Dim version As String
        Dim build As String
        Dim time As String
        Dim branch As String
    End Structure
    Public Sub GetForgeBasic(ByVal version As String)
        Try

            '初始化
            ForgeState = LoadState.Loading
            ForgeArray = New ArrayList
            ForgeInfo = ""
            If MODE_OFFLINE Then
                ForgeState = LoadState.NoConnection
                Exit Sub
            End If

            '获取版本列表
            Try
                log("[DownloadLeft] 获取Forge版本列表开始：" & version)
                Dim Data = GetWebsiteCode("http://bmclapi2.bangbang93.com/forge/minecraft/" & version, Encoding.Default)
                If Not ForgeCurrent = version Then Exit Sub
                ForgeInfo = Data
                If Len(ForgeInfo) < 400 Then Throw New WebException("获取到的列表长度不足：" & ForgeInfo)
            Catch ex As Exception
                ExShow(ex, "获取Forge版本列表失败：" & version)
                ForgeInfo = ""
                ForgeState = LoadState.Failed
                Exit Sub
            End Try
            log("[DownloadLeft] 获取Forge版本列表成功")

            '加载版本列表信息
            Try
                log("[DownloadLeft] 加载Forge版本列表信息开始")
                '预处理
                Dim builds As ArrayList = RegexSearch(ForgeInfo, "(?<=""build"":)[0-9]*")
                Dim versions As ArrayList = RegexSearch(ForgeInfo, "(?<=""version"":"")[0-9.]*")
                Dim branchs As ArrayList = RegexSearch(ForgeInfo, "(?<=""branch"":)[^,]*")
                Dim times As ArrayList = RegexSearch(ForgeInfo, "(?<=modified"":"")[0-9.\-]*")
                '检查
                If Not (builds.Count = versions.Count And versions.Count = times.Count And branchs.Count = times.Count) Then Throw New WebException("获取到的列表长度不等")
                If Not ForgeCurrent = version Then Exit Sub
                '添加
                For i = builds.Count - 1 To 0 Step -1
                    ForgeArray.Add(New ForgeVersion With {.build = builds(i), .version = versions(i), .time = times(i), .branch = branchs(i)})
                Next
            Catch ex As Exception
                ExShow(ex, "加载Forge版本列表信息失败：" & version)
                ForgeInfo = ""
                ForgeState = LoadState.Failed
                Exit Sub
            End Try

            log("[DownloadLeft] 加载Forge版本列表信息成功")
            ForgeState = LoadState.Success

        Catch
        End Try
    End Sub

    Private Sub ForgeDownloadStart(ByVal sender As ListItem, ByVal e As System.Windows.Input.MouseButtonEventArgs)
        If IsNothing(sender.Tag) Or Not selecter.SelectIndexName = "Forge" Then Exit Sub
        '初始化信息
        Dim ver As ForgeVersion = sender.Tag
        If WebGroups.ContainsKey("Forge " & ver.version) Then
            ShowHint(New HintConverter("正在下载中，请勿重复操作", HintState.Warn))
            Exit Sub
        End If

        '选择操作
        If Val(ReadIni("setup", "DownForgeAction", "-1")) = "-1" Then
            WriteIni("setup", "DownForgeAction", MyMsgbox("在 Forge 下载结束后，是自动安装该版本，还是打开下载文件夹但是不安装？你稍候可以在设置页面中再修改这个设置。" & vbCrLf & "如果你不清楚如何手动安装 Forge，建议选择自动安装。", "选择默认动作", "自动安装", "只打开文件夹") - 1)
        End If

        '下载
        log("[DownloadLeft] 下载Forge：" & ver.build)
        ShowHint("Forge " & ver.version & " 开始下载")
        SendStat("下载", "Forge", ver.version, ver.build)
        Dim th As New Thread(Sub()
                                 Try
                                     Dim FileName As String = ForgeCurrent & "-" & ver.version & If(ver.branch = "null", "", "-" & ver.branch.Replace("""", ""))
                                     WebStart({New WebRequireFile With {
                                                     .WebURLs = New ArrayList From {
                                                         "http://bmclapi2.bangbang93.com/forge/download/" & ver.build,
                                                         "http://bmclapi2.bangbang93.com/maven/net/minecraftforge/forge/" & FileName & "/forge-" & FileName & "-installer.jar",
                                                         "http://files.minecraftforge.net/maven/net/minecraftforge/forge/" & FileName & "/forge-" & FileName & "-installer.jar",
                                                         "http://bmclapi2.bangbang93.com/maven/net/minecraftforge/forge/" & FileName & "/forge-" & FileName & "-universal.zip",
                                                         "http://files.minecraftforge.net/maven/net/minecraftforge/forge/" & FileName & "/forge-" & FileName & "-universal.zip",
                                                         "http://bmclapi2.bangbang93.com/maven/net/minecraftforge/forge/" & FileName & "/forge-" & FileName & "-universal.jar",
                                                         "http://files.minecraftforge.net/maven/net/minecraftforge/forge/" & FileName & "/forge-" & FileName & "-universal.jar",
                                                         "http://bmclapi2.bangbang93.com/maven/net/minecraftforge/forge/" & FileName & "/forge-" & FileName & "-client.zip",
                                                         "http://files.minecraftforge.net/maven/net/minecraftforge/forge/" & FileName & "/forge-" & FileName & "-client.zip"},
                                                     .LocalFolder = PATH_DOWNLOAD, .KnownFileSize = 1024 * 16
                                              }}, "Forge " & ver.version, AddressOf ForgeDownloadSuccess, AddressOf ForgeDownloadFail, WebRequireSize.AtLeast)
                                 Catch ex As Exception
                                     ExShow(ex, "下载Forge失败：" & ver.version)
                                     Application.Current.Dispatcher.Invoke(CType(AddressOf ForgeDownloadFail, ParameterizedThreadStart), ver.version)
                                 End Try
                             End Sub)
        th.Start()

        '检查引用版本
        If GetFileSize(PATH_MC & "versions\" & ForgeCurrent & "\" & ForgeCurrent & ".jar") < 1024 Then
            '引用版本不存在
            log("[DownloadLeft] Forge 对应的引用版本不存在：" & ForgeCurrent)
            Dim RequireVersion As MinecraftVersion = MinecraftGet(ForgeCurrent)
            If Not IsNothing(RequireVersion.id) Then MinecraftDownloadStart(RequireVersion)
        End If

    End Sub
    Private Sub ForgeDownloadSuccess(ByVal Name As String)
        Dim Version = Name.Replace("Forge ", "")
        Try
            If Val(ReadIni("setup", "DownForgeAction", "1")) = 0 Then
                '自动安装
                Try
                    Dim FoundFile As Boolean = False
                    For Each File As FileInfo In New DirectoryInfo(PATH_DOWNLOAD).EnumerateFiles
                        If File.Name.Contains(Version) Then
                            log("[DownloadLeft] 自动安装目标文件：" & File.FullName)
                            FoundFile = True

                            If Not File.Name.Contains("installer") Then
NotSupport:
                                ShowHint(New HintConverter("Forge " & Version & " 下载成功，但该版本过老，不支持自动安装", HintState.Finish))
                                Process.Start(PATH_DOWNLOAD)
                                Exit Sub
                            End If

                            '自动安装
                            Dim InstallDir As String = PATH & "PCL\cache\forgeinstall\" & Version
                            Directory.CreateDirectory(InstallDir)
                            Using zip As New ZipFile(File.FullName)
                                zip.ExtractSelectedEntries("install_profile.json", "", InstallDir, ExtractExistingFileAction.OverwriteSilently)
                                Dim Json As JObject = ReadJson(ReadFileToEnd(InstallDir & "\install_profile.json"))

                                '判断是否为 1.5-1.6 的 Forge
                                Dim FilePath As String = Json("install")("filePath").ToString
                                If FilePath.Contains("-1.5") Or FilePath.Contains("-1.6") Then
                                    Directory.Delete(InstallDir, True)
                                    GoTo NotSupport
                                End If
                                ShowHint(New HintConverter("Forge " & Version & " 下载成功", HintState.Finish))

                                '解压Forge主文件
                                Dim LibPath As String = GetPathFromFullPath(GetPathFromLibrary(Json("install")("path")))
                                Directory.CreateDirectory(LibPath)
                                zip.ExtractSelectedEntries(FilePath, "", LibPath, ExtractExistingFileAction.OverwriteSilently)
                                IO.File.Delete(GetPathFromLibrary(Json("install")("path")))
                                FileSystem.Rename(LibPath & FilePath, GetPathFromLibrary(Json("install")("path")))

                                '输出Json
                                Dim VersionId As String = Json("versionInfo")("id")
                                Dim VersionPath As String = PATH_MC & "versions\" & VersionId & "\"
                                Directory.CreateDirectory(VersionPath)
                                WriteFile(VersionPath & VersionId & ".json", Json("versionInfo").ToString)

                                '删除缓存文件夹
                                Directory.Delete(InstallDir, True)

                                '刷新版本列表
                                Dim th As New Thread(AddressOf PoolVersionList)
                                th.Start()

                            End Using
                            ShowHint(New HintConverter("Forge " & Version & " 安装成功", HintState.Finish))
                        End If
                        If FoundFile Then GoTo FoundFile
                    Next
                    ShowHint(New HintConverter("Forge " & Version & " 下载成功，但未找到自动安装文件", HintState.Finish))
FoundFile:
                Catch ex As Exception
                    ExShow(ex, "Forge " & Version & " 安装失败", ErrorLevel.AllUsers)
                End Try
            Else
                '只打开文件夹
                Process.Start(PATH_DOWNLOAD)
                ShowHint(New HintConverter("Forge " & Version & " 下载成功", HintState.Finish))
            End If
        Catch ex As Exception
            ExShow(ex, "Forge下载结束后的处理异常：" & Version)
            Try
                File.Delete(PATH_DOWNLOAD & "Forge_" & Version & ".jar")
            Catch : End Try
            ShowHint(New HintConverter("Forge " & Version & " 下载失败：" & GetStringFromException(ex) & "！", HintState.Critical))
        End Try
    End Sub
    Private Sub ForgeDownloadFail(ByVal Name As String)
        Dim Version As String = Name.Replace("Forge ", "")
        ShowHint(New HintConverter("Forge " & Version & " 下载失败", HintState.Critical))
    End Sub

#End Region

End Class
