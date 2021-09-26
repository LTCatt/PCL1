Public Class InfoBox

    ''' <summary>
    ''' 推荐源。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Source As String
    ''' <summary>
    ''' 内容（标题）。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Title As String
    ''' <summary>
    ''' 点击时进入的URL。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property OnClickURL As String
    ''' <summary>
    ''' Path的资料。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property IconData As String
    ''' <summary>
    ''' 图片的本地文件地址，可能为程序内部地址。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property PictureLocalURL As String

    Public IsSendStat As Boolean = False

    ''' <summary>
    ''' 处理源。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property MySource As InfoBoxSource

    '鼠标事件
    Private Const ANIMATE_SPEED As Integer = 250
    Private Sub InfoBox_MouseEnter(ByVal sender As Object, ByVal e As System.Windows.Input.MouseEventArgs) Handles Me.MouseEnter
        'If If(Me.Name, "") = "" Then Me.Name = "InfoBox" & GetUUID()
        'If IsTextOnly Then Exit Sub
        'AniStart({
        '         AaOpacity(panInfo, 1 - panInfo.Opacity, ANIMATE_SPEED), AaRadius(blur, 10 - blur.Radius, ANIMATE_SPEED)
        '     }, "InfoBoxMouse" & Me.Name)
    End Sub
    Private Sub InfoBox_MouseLeave(ByVal sender As Object, ByVal e As System.Windows.Input.MouseEventArgs) Handles Me.MouseLeave
        MouseDowned = False
        'If IsTextOnly Then Exit Sub
        'AniStart({
        '         AaOpacity(panInfo, -panInfo.Opacity, ANIMATE_SPEED), AaRadius(blur, -blur.Radius, ANIMATE_SPEED)
        '     }, "InfoBoxMouse" & Me.Name)
    End Sub

    '点击确认
    Private MouseDowned As Boolean = False
    Private Sub InfoBox_MouseLeftButtonDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles Me.MouseLeftButtonDown
        MouseDowned = True
    End Sub
    Private Sub InfoBox_MouseLeftButtonUp(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles Me.MouseLeftButtonUp

        '保证是在这里按下的左键
        If MouseDowned Then
            MouseDowned = False
        Else
            Exit Sub
        End If

        log("[InfoBox] 点击InfoBox，命令：" & OnClickURL)
        SendStat("推荐源", "点击", OnClickURL)
        If IsNothing(OnClickURL) Then
            Exit Sub
        ElseIf OnClickURL.StartsWith("http") Then
            Try
                Process.Start(OnClickURL)
            Catch ex As Exception
                ExShow(ex, "信息框点击事件处理失败", ErrorLevel.AllUsers)
            End Try
        End If
    End Sub

    ''' <summary>
    ''' 显示信息框。
    ''' </summary>
    ''' <param name="source">信息框源。</param>
    ''' <remarks></remarks>
    Public Sub Show(ByVal source As InfoBoxSource)
        Try

            '设置信息
            With Me
                .Source = source.Source
                .Title = source.Title
                .OnClickURL = source.OnClickURL
                .IconData = source.IconData
                .MySource = source
                .PictureLocalURL = source.PictureLocalURL
                .Visibility = Visibility.Visible
            End With

            '初始化UI
            path.Data = New GeometryConverter().ConvertFrom(IconData)
            labTitle.Content = source.Source
            labContent.Text = source.Title
            imgBack.Source = New MyBitmap(source.PictureLocalURL)

            '尝试保存截图
            LoadAdd()

        Catch ex As Exception
            ExShow(ex, "信息框显示失败", ErrorLevel.AllUsers)
        End Try
    End Sub

    Private LoadedProcess As Integer = 0 '控件加载后+1，信息加载后+1
    '确保控件与信息都加载后截屏
    Private Sub LoadAdd()
        LoadedProcess = LoadedProcess + 1
        If LoadedProcess = 2 Then
            Dim rtb = New RenderTargetBitmap(Width, Height, 96, 96, PixelFormats.Default)
            rtb.Render(Me.panBackAll)
            Me.Background = New ImageBrush(rtb)
            Me.panBackAll.Visibility = Visibility.Collapsed
        End If
    End Sub

    Private Sub InfoBox_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        LoadAdd()
    End Sub


End Class


''' <summary>
''' InfoBox加载源。
''' </summary>
''' <remarks></remarks>
Public Class InfoBoxSource

    '以下信息由加载时给出
    ''' <summary>
    ''' 推荐源。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Source As String
    ''' <summary>
    ''' 内容（标题）。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Title As String
    ''' <summary>
    ''' 推荐的种类。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Type As String = ""
    ''' <summary>
    ''' 点击时进入的URL。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property OnClickURL As String
    ''' <summary>
    ''' 给出的图片信息，可能为颜色名。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property PictureName As String

    '以下为内部信息
    ''' <summary>
    ''' Path的资料。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property IconData As String
    ''' <summary>
    ''' 加载状态。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property State As LoadState = LoadState.Waiting
    ''' <summary>
    ''' 图片的网络文件地址，可能为空。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property PictureWebURL As String
    ''' <summary>
    ''' 图片的本地文件地址，可能为程序内部地址。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property PictureLocalURL As String

    ''' <summary>
    ''' 开始加载。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub Load()

        '如果已经开始加载了则跳出
        If State = LoadState.Loading Or State = LoadState.Loaded Then Exit Sub
        Try

            Title = Title.Replace("&amp;", "&")
            '初始化信息
            If Title.StartsWith("【") Then Title = Title & " "
            Select Case Type
                Case "新闻", "公告"
                    '小圈圈的“i”
                    IconData = "F1M38,19C48.4934,19 57,27.5066 57,38 57,48.4934 48.4934,57 38,57 27.5066,57 19,48.4934 19,38 19,27.5066 27.5066,19 38,19z M33.25,33.25L33.25,36.4167 36.4166,36.4167 36.4166,47.5 33.25,47.5 33.25,50.6667 44.3333,50.6667 44.3333,47.5 41.1666,47.5 41.1666,36.4167 41.1666,33.25 33.25,33.25z M38.7917,25.3333C37.48,25.3333 36.4167,26.3967 36.4167,27.7083 36.4167,29.02 37.48,30.0833 38.7917,30.0833 40.1033,30.0833 41.1667,29.02 41.1667,27.7083 41.1667,26.3967 40.1033,25.3333 38.7917,25.3333z"
                Case "创作", "教程"
                    '带图片的书
                    IconData = "F1M22,46.9996C26.4235,48.3026,34.4825,48.8053,37.2083,52.2153L37.2083,32.9996C34.4826,29.5896,26.4235,29.0869,22,27.7839L22,46.9996z M22,24.3078L22,24.028C26.4235,25.331,34.4825,25.8337,37.2083,29.2437L38,29.4716 38.7917,29.2157C41.5174,25.8057,49.5765,25.303,54,24L54,24.2798C55.2286,24.6498,56,24.9716,56,24.9716L56,27.9716 59,26.8258 59,50.9716C59,50.9716,41.1667,52.2216,38,57.7633L37.9999,57.7913C34.8333,52.2496,17,50.9996,17,50.9996L17,26.8538 20,27.9996 20,24.9996C20,24.9996,20.7714,24.6778,22,24.3078z M23.5,44.506L23.5,41.3844C27.269,42.243,32.4604,42.8187,35.5,44.7496L35.5,47.8712C32.4604,45.9402,27.269,45.3646,23.5,44.506z M23.5,39.1212L23.5,35.9996C27.269,36.8582,32.4604,37.4338,35.5,39.3648L35.5,42.4864C32.4604,40.5554,27.269,39.9798,23.5,39.1212z M23.5,33.6344L23.5,30.5128C27.269,31.3714,32.4604,31.947,35.5,33.878L35.5,36.9996C32.4604,35.0686,27.269,34.493,23.5,33.6344z M54,46.9716L54,27.7559C49.5765,29.0589,41.5174,29.5616,38.7917,32.9716L38.7917,52.1873C41.5175,48.7773,49.5765,48.2746,54,46.9716z M52.5,44.478C48.731,45.3366,43.5395,45.9122,40.5,47.8432L40.5,44.7216C43.5395,42.7906,48.731,42.215,52.5,41.3564L52.5,44.478z M52.5,39.0932C48.731,39.9518,43.5395,40.5274,40.5,42.4584L40.5,39.3368C43.5396,37.4058,48.731,36.8302,52.5,35.9716L52.5,39.0932z M52.5,33.6064C48.731,34.465,43.5395,35.0406,40.5,36.9716L40.5,33.85C43.5395,31.919,48.731,31.3434,52.5,30.4848L52.5,33.6064z"
                Case "软件", "懒人包", "Mod", "皮肤", "资源包"
                    '盒子
                    IconData = "F1M38,19L57,28.5 57,47.5 38,57 19,47.5 19,28.5 38,19z M22.1667,30.0833L22.1667,45.9167 25.3333,47.5 36.4167,53.0417 36.4167,37.2083 22.1667,30.0833z M50.6666,47.5L53.8333,45.9167 53.8333,30.0833 39.5833,37.2083 39.5833,53.0417 50.6666,47.5z M38,22.1667L25.3333,28.5 38,34.8334 50.6666,28.5 38,22.1667z"
                Case "多人"
                    '地球
                    IconData = "F1M38,15.8333C50.2423,15.8333 60.1667,25.7577 60.1667,38 60.1667,50.2423 50.2423,60.1667 38,60.1667 25.7577,60.1667 15.8333,50.2423 15.8333,38 15.8333,25.7577 25.7577,15.8333 38,15.8333z M19.065,36.4167L25.3651,36.4167C25.4708,33.796 25.8368,31.3011 26.4182,29.0106 24.9471,28.4945 23.5896,27.8831 22.3719,27.1913 20.5281,29.8522 19.3463,33.0068 19.065,36.4167z M30.0541,20.7363C27.8969,21.7308 25.9579,23.1177 24.3286,24.8056 25.236,25.2756 26.2395,25.6989 27.3232,26.0677 28.064,24.0419 28.9879,22.241 30.0541,20.7363z M36.4167,36.4167L36.4167,30.8432C33.9463,30.7436 31.5878,30.4126 29.4069,29.8881 28.9321,31.8962 28.6282,34.0974 28.5325,36.4167L36.4167,36.4167z M36.4167,19.2627C33.9024,20.1063 31.7231,22.9251 30.2911,26.8939 32.1894,27.3157 34.2515,27.5865 36.4167,27.6758L36.4167,19.2627z M56.9349,36.4167C56.6536,33.0068 55.4719,29.8523 53.6281,27.1913 52.4104,27.8831 51.0528,28.4946 49.5818,29.0107 50.1631,31.3011 50.5291,33.796 50.6348,36.4167L56.9349,36.4167z M45.9459,20.7363C47.012,22.241 47.9359,24.042 48.6767,26.0677 49.7605,25.6989 50.7639,25.2756 51.6714,24.8056 50.0421,23.1177 48.1031,21.7308 45.9459,20.7363z M39.5833,36.4167L47.4674,36.4167C47.3718,34.0974 47.0678,31.8962 46.5931,29.8881 44.4122,30.4126 42.0536,30.7436 39.5833,30.8432L39.5833,36.4167z M39.5833,19.2627L39.5833,27.6758C41.7484,27.5865 43.8106,27.3157 45.7088,26.8939 44.2769,22.9251 42.0975,20.1064 39.5833,19.2627z M56.9349,39.5834L50.6348,39.5834C50.5291,42.204 50.1631,44.6989 49.5818,46.9894 51.0528,47.5055 52.4104,48.1169 53.6281,48.8087 55.4719,46.1478 56.6536,42.9932 56.9349,39.5834z M45.9459,55.2638C48.1031,54.2692 50.0421,52.8823 51.6714,51.1944 50.764,50.7244 49.7605,50.3011 48.6767,49.9323 47.9359,51.9581 47.012,53.7591 45.9459,55.2638z M39.5833,39.5834L39.5833,45.1568C42.0536,45.2564 44.4122,45.5874 46.5931,46.1119 47.0678,44.1038 47.3718,41.9027 47.4675,39.5834L39.5833,39.5834z M39.5833,56.7373C42.0975,55.8937 44.2769,53.075 45.7089,49.1061 43.8106,48.6843 41.7484,48.4135 39.5833,48.3242L39.5833,56.7373z M19.065,39.5834C19.3463,42.9932 20.5281,46.1478 22.3719,48.8087 23.5896,48.1169 24.9471,47.5055 26.4182,46.9894 25.8368,44.6989 25.4708,42.204 25.3651,39.5834L19.065,39.5834z M30.0541,55.2638C28.988,53.7591 28.0641,51.9581 27.3232,49.9323 26.2395,50.3011 25.236,50.7244 24.3286,51.1945 25.9579,52.8823 27.8969,54.2693 30.0541,55.2638z M36.4167,39.5834L28.5325,39.5834C28.6282,41.9027 28.9321,44.1039 29.4069,46.1119 31.5878,45.5874 33.9463,45.2564 36.4167,45.1568L36.4167,39.5834z M36.4167,56.7373L36.4167,48.3242C34.2515,48.4135 32.1893,48.6843 30.2911,49.1061 31.7231,53.075 33.9024,55.8937 36.4167,56.7373z"
                Case "建筑"
                    '城市
                    IconData = "F1M44.3333,30.0833L57,30.0833 57,57 44.3333,57 44.3333,30.0833z M46.3125,35.2292L46.3125,38 49.0833,38 49.0833,35.2292 46.3125,35.2292z M52.25,35.2292L52.25,38 55.0208,38 55.0208,35.2292 52.25,35.2292z M46.3125,39.9792L46.3125,42.75 49.0833,42.75 49.0833,39.9792 46.3125,39.9792z M52.25,39.9792L52.25,42.75 55.0208,42.75 55.0208,39.9792 52.25,39.9792z M46.3125,44.7292L46.3125,47.5 49.0833,47.5 49.0833,44.7292 46.3125,44.7292z M52.25,44.7292L52.25,47.5 55.0208,47.5 55.0208,44.7292 52.25,44.7292z M46.3125,49.4792L46.3125,52.25 49.0833,52.25 49.0833,49.4792 46.3125,49.4792z M52.25,49.4792L52.25,52.25 55.0208,52.25 55.0208,49.4792 52.25,49.4792z M23.75,25.3333L25.3333,22.1667 26.9167,22.1667 26.9167,18.2084 28.5,18.2084 28.5,22.1667 31.6667,22.1667 31.6667,18.2084 33.25,18.2084 33.25,22.1667 34.8333,22.1667 36.4167,25.3333 36.4167,34.8334 38.7917,34.8333 41.1667,37.2083 41.1667,57 19,57 19,37.2083 21.375,34.8333 23.75,34.8334 23.75,25.3333z M25.7291,27.3125L25.7291,30.0834 28.1041,30.0834 28.1041,27.3125 25.7291,27.3125z M32.0625,27.3125L32.0625,30.0834 34.4375,30.0834 34.4375,27.3125 32.0625,27.3125z M25.7291,32.0625L25.7291,34.8334 28.1041,34.8334 28.1041,32.0625 25.7291,32.0625z M32.0625,32.0625L32.0625,34.8334 34.4375,34.8334 34.4375,32.0625 32.0625,32.0625z M30.875,39.9792L28.8958,39.9792 28.8958,42.75 30.875,42.75 30.875,39.9792z M24.5416,39.9792L24.5416,42.75 26.9166,42.75 26.9166,39.9792 24.5416,39.9792z M36.0208,39.9792L33.25,39.9792 33.25,42.75 36.0208,42.75 36.0208,39.9792z M30.875,44.7292L28.8958,44.7292 28.8958,47.5 30.875,47.5 30.875,44.7292z M26.9166,44.7292L24.5416,44.7292 24.5416,47.5 26.9166,47.5 26.9166,44.7292z M36.0208,44.7292L33.25,44.7292 33.25,47.5 36.0208,47.5 36.0208,44.7292z M30.875,49.4792L28.8958,49.4792 28.8958,52.25 30.875,52.25 30.875,49.4792z M26.9166,49.4792L24.5416,49.4792 24.5417,52.25 26.9167,52.25 26.9166,49.4792z M36.0208,49.4792L33.25,49.4792 33.25,52.25 36.0208,52.25 36.0208,49.4792z"
                Case "地图"
                    '药水
                    IconData = "F1M28,18L48,18 48,22 45,22 45,29 57,52 54,58 22,58 19,52 31,29 31,22 28,22 28,18z M28,54L37,36 43.8461,36 41,31 41,22 35,22 35,31 23.8461,54 28,54z"
                Case Else
                    ''方块
                    'IconData = "F1M18,21.7037L43.9259,18 58,25.4074 58,54.2963 32.8148,58 18,49.1111 18,21.7037z"
                    '盒子
                    IconData = "F1M38,19L57,28.5 57,47.5 38,57 19,47.5 19,28.5 38,19z M22.1667,30.0833L22.1667,45.9167 25.3333,47.5 36.4167,53.0417 36.4167,37.2083 22.1667,30.0833z M50.6666,47.5L53.8333,45.9167 53.8333,30.0833 39.5833,37.2083 39.5833,53.0417 50.6666,47.5z M38,22.1667L25.3333,28.5 38,34.8334 50.6666,28.5 38,22.1667z"
            End Select
            If PictureName.Contains("/") Then
                '是网络地址
                PictureWebURL = PictureName
                PictureLocalURL = PATH & "PCL\cache\source\" & Left(SerAdd(Source), 100) & "\" & Title.GetHashCode & ".png"
                State = LoadState.Loading
                If File.Exists(PictureLocalURL) Then
                    State = LoadState.Loaded
                    Exit Sub
                End If
            Else
                '不是网络地址
                PictureWebURL = ""
                If PictureName = "blue" Or PictureName = "brown" Or PictureName = "green" Or PictureName = "orange" Or PictureName = "purple" Or PictureName = "yellow" Then
                    PictureLocalURL = PATH_IMAGE & "infobox-" & PictureName & ".png"
                Else
                    PictureLocalURL = PATH_IMAGE & "infobox-" & OneOf(New String() {"blue", "brown", "green", "orange", "purple", "yellow"}) & ".png"
                End If
                Thread.Sleep(RandomInteger(100, 500))
                State = LoadState.Loaded
                Exit Sub
            End If

            Dim th As New Thread(Sub()
                                     Try
                                         If DownloadFile(PictureWebURL, PictureLocalURL & DOWNLOADING_END) Then
                                             File.Delete(PictureLocalURL)
                                             FileSystem.Rename(PictureLocalURL & DOWNLOADING_END, PictureLocalURL)
                                             State = LoadState.Loaded
                                         Else
                                             State = LoadState.Failed
                                             log("下载信息框图片失败：" & PictureLocalURL)
                                         End If
                                     Catch ex As Exception
                                         State = LoadState.Failed
                                         ExShow(ex, "下载信息框图片失败")
                                     End Try
                                 End Sub)
            Pool.Add(th)

        Catch ex As Exception
            ExShow(ex, "信息框加载失败", ErrorLevel.AllUsers)
        End Try
    End Sub

End Class