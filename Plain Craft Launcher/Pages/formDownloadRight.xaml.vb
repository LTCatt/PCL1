Public Class formDownloadRight

    '初始化滚动条
    Private Sub panShow_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles panShow.Loaded
        If scroll.SetControl(panShow, True) Then AddHandler panBack.MouseWheel, AddressOf scroll.RunMouseWheel
        RefreshPage()
    End Sub

    Private ControlDict As New Dictionary(Of String, Object)

    ''' <summary>
    ''' 添加一个文件组到 UI。
    ''' </summary>
    ''' <param name="Group"></param>
    Public Sub AddGroup(Group As WebGroup)
        frmMain.Dispatcher.Invoke(Sub()


                                      panShow.UpdateLayout()
                                      Dim NewGrid As New Grid With {.Width = 203, .Margin = New Thickness(0, 5, 0, 9)}
                                      ControlDict.Add("MainGrid" & Group.UUID, NewGrid)

                                      Dim NewPath As New Shapes.Path With {.Data = New GeometryConverter().ConvertFromString("F1 M 38,19C 47.3888,19 55,21.0147 55,23.5038L 55,25.5C 55,27.9853 47.3888,30 38,30C 28.6112,30 21,27.9853 21,25.5L 21,23.5C 21,21.0147 28.6112,19 38,19 Z M 55,52.5C 55,54.9853 47.3888,57 38,57C 28.6112,57 21,54.9853 21,52.5L 21,46.5C 21,48.9853 28.6112,51 38,51C 47.384,51 54.9921,48.9874 55,46.5039L 55,52.5 Z M 55,43.5C 55,45.9853 47.3888,48 38,48C 28.6112,48 21,45.9853 21,43.5L 21,37.5C 21,39.9853 28.6112,42 38,42C 47.384,42 54.9921,39.9874 55,37.5038L 55,43.5 Z M 55,34.5C 55,36.9853 47.3888,39 38,39C 28.6112,39 21,36.9853 21,34.5L 21,28.5C 21,30.9853 28.6112,33 38,33C 47.384,33 54.9921,30.9874 55,28.5038L 55,34.5 Z"), .Height = 19, .Stretch = Stretch.Fill, .Width = 17, .HorizontalAlignment = HorizontalAlignment.Left, .VerticalAlignment = VerticalAlignment.Top, .UseLayoutRounding = False, .Margin = New Thickness(9, 9, 0, 0)}
                                      NewPath.SetResourceReference(Shape.FillProperty, "Color4")
                                      NewGrid.Children.Add(NewPath)

                                      Dim NewTitle As New Label With {.Content = Group.Name, .FontSize = 16, .VerticalContentAlignment = VerticalAlignment.Center, .Margin = New Thickness(35, 0, 0, 0), .Height = 36, .VerticalAlignment = VerticalAlignment.Top, .Padding = New Thickness(0), .UseLayoutRounding = False, .MaxWidth = 160, .HorizontalAlignment = HorizontalAlignment.Left}
                                      NewTitle.SetResourceReference(ForegroundProperty, "Color4")
                                      AddHandler NewTitle.MouseEnter, Sub()
                                                                          NewTitle.ToolTip = If(NewTitle.ActualWidth > 159, Group.Name, Nothing)
                                                                      End Sub
                                      NewGrid.Children.Add(NewTitle)

                                      Dim NewProgress1 As New TextBlock With {.Height = 2, .VerticalAlignment = VerticalAlignment.Top, .Margin = New Thickness(5, 37, 0, 0), .HorizontalAlignment = HorizontalAlignment.Left, .Width = 193, .UseLayoutRounding = True}
                                      NewProgress1.SetResourceReference(TextBlock.BackgroundProperty, "Color2")
                                      NewGrid.Children.Add(NewProgress1)

                                      Dim NewProgress2 As New TextBlock With {.Height = 2, .VerticalAlignment = VerticalAlignment.Top, .Margin = New Thickness(5, 37, 0, 0), .HorizontalAlignment = HorizontalAlignment.Left, .Width = 0, .UseLayoutRounding = True, .MaxWidth = 193}
                                      NewProgress2.SetResourceReference(TextBlock.BackgroundProperty, "Color4")
                                      NewGrid.Children.Add(NewProgress2)
                                      ControlDict.Add("Progress" & Group.UUID, NewProgress2)

                                      Dim NewHost As New StackPanel With {.HorizontalAlignment = HorizontalAlignment.Left, .Margin = New Thickness(5, 45, 0, 0), .VerticalAlignment = VerticalAlignment.Top, .Width = 193}
                                      ControlDict.Add("Host" & Group.UUID, NewHost)

                                      NewGrid.Children.Add(NewHost)
                                      panShow.Children.Add(NewGrid)

                                      AddFileList(Group.Files, NewHost)
                                      RefreshGroup(Group)
                                      RefreshPage()


                                  End Sub)
    End Sub

    ''' <summary>
    ''' 刷新一个 WebGroup 在 UI 中的显示。
    ''' </summary>
    ''' <param name="Group"></param>
    Public Sub RefreshGroup(Group As WebGroup)
        frmMain.Dispatcher.Invoke(Sub()


                                      '刷新进度条
                                      AniStart({
                                               AaWidth(ControlDict("Progress" & Group.UUID), 193 * Group.Percent - ControlDict("Progress" & Group.UUID).Width, 250, , New AniEaseJumpEnd(0.6))
                                           }, "Progress" & Group.UUID)


                                  End Sub)
    End Sub

    ''' <summary>
    ''' 添加一堆文件到 UI 中的 StackPanel。
    ''' </summary>
    Private Sub AddFileList(Files As ArrayList, Host As StackPanel)
        For Each File As WebFile In Files

            Dim f As WebFile = File
            Dim NewFileGrid As New Grid With {.Height = 22, .UseLayoutRounding = True, .Background = New MyColor(1, 255, 255, 255), .Width = 193, .HorizontalAlignment = HorizontalAlignment.Left}
            NewFileGrid.ColumnDefinitions.Add(New ColumnDefinition With {.Width = (New GridLengthConverter).ConvertFromString("1*")})
            NewFileGrid.ColumnDefinitions.Add(New ColumnDefinition With {.Width = GridLength.Auto})
            AddHandler NewFileGrid.MouseEnter, Sub()
                                                   NewFileGrid.ToolTip = "下载地址：" & f.WebAddress &
                                                                            If(f.WebAddress.Contains("minecraft.net/") Or f.WebAddress.Contains("mojang.com/"), vbCrLf & "下载来源：Mojang 官方", "") &
                                                                            If(f.WebAddress.Contains("optifine.net/"), vbCrLf & "下载来源：OptiFine 官方", "") &
                                                                            If(f.WebAddress.Contains("minecraftforge.net/"), vbCrLf & "下载来源：Forge 官方", "") &
                                                                            If(f.WebAddress.Contains("bangbang93.com/"), vbCrLf & "下载来源：BMCLAPI by bangbang93", "") &
                                                                            If(f.WebAddress.Contains(TX_SREVER_1) Or f.WebAddress.Contains(TX_SREVER_2) Or f.WebAddress.Contains("bkt.clouddn.com/"), vbCrLf & "下载来源：PCL 服务器", "") &
                                                                            If(f.State = WebDownloadStats.Download And Not f.GetServerFileSize = WebRequireSize.DontNeed,
                                                                                  vbCrLf & "文件大小：" & f.GetSize.ToString & " / " & f.TotalSize.ToString &
                                                                                  vbCrLf & "下载速度：" & f.Speed.ToString & "/s",
                                                                              "")
                                               End Sub
            ControlDict.Add("FileGrid" & File.UUID, NewFileGrid)

            Dim FileImage As New Image With {.Height = 14, .HorizontalAlignment = HorizontalAlignment.Left, .Margin = New Thickness(4, 4, 0, 0), .VerticalAlignment = VerticalAlignment.Top, .Width = 14, .UseLayoutRounding = True, .Tag = "", .Opacity = 0.8}
            NewFileGrid.Children.Add(FileImage)
            ControlDict.Add("Image" & File.UUID, FileImage)

            Dim LeftLabel As New Label With {.Margin = New Thickness(23, 0, 0, 0), .Padding = New Thickness(0, 0, 4, 0), .VerticalContentAlignment = VerticalAlignment.Center, .Foreground = New MyColor(255, 75, 75, 75), .FontSize = 12}
            NewFileGrid.Children.Add(LeftLabel)
            ControlDict.Add("FileName" & File.UUID, LeftLabel)

            Dim RightLabel As New Label With {.Name = "Label" & File.UUID, .FontSize = 12, .Foreground = New MyColor(255, 75, 75, 75), .HorizontalAlignment = HorizontalAlignment.Right, .Padding = New Thickness(0), .VerticalContentAlignment = VerticalAlignment.Center, .Margin = New Thickness(0, 0, 3, 0), .Opacity = 0.6}
            NewFileGrid.Children.Add(RightLabel)
            ControlDict.Add("Label" & File.UUID, RightLabel)
            Grid.SetColumn(RightLabel, 1)

            Host.Children.Add(NewFileGrid)
            RefreshFile(File)


        Next
    End Sub

    ''' <summary>
    ''' 刷新一个 WebFile 在 UI 中的显示。
    ''' </summary>
    Public Sub RefreshFile(File As WebFile)
        frmMain.Dispatcher.Invoke(Sub()
                                      Try

                                          '查找控件
                                          Dim FileGrid As Grid = ControlDict("FileGrid" & File.UUID)
                                          Dim FileImage As Image = ControlDict("Image" & File.UUID)
                                          Dim FileName As Label = ControlDict("FileName" & File.UUID)
                                          Dim FileLabel As Label = ControlDict("Label" & File.UUID)

                                          '刷新 Grid 的指向信息
                                          If FileGrid.IsMouseOver Then
                                              FileGrid.ToolTip = "下载地址：" & File.WebAddress &
                                                                            If(File.WebAddress.Contains("minecraft.net/") Or File.WebAddress.Contains("mojang.com/"), vbCrLf & "下载来源：Mojang 官方", "") &
                                                                            If(File.WebAddress.Contains("optifine.net/"), vbCrLf & "下载来源：OptiFine 官方", "") &
                                                                            If(File.WebAddress.Contains("minecraftforge.net/"), vbCrLf & "下载来源：Forge 官方", "") &
                                                                            If(File.WebAddress.Contains("bangbang93.com/"), vbCrLf & "下载来源：BMCLAPI by bangbang93", "") &
                                                                            If(File.WebAddress.Contains(TX_SREVER_1) Or File.WebAddress.Contains(TX_SREVER_2) Or File.WebAddress.Contains("bkt.clouddn.com/"), vbCrLf & "下载来源：PCL 服务器", "") &
                                                                            If(File.State = WebDownloadStats.Download And Not File.GetServerFileSize = WebRequireSize.DontNeed,
                                                                                  vbCrLf & "文件大小：" & File.GetSize.ToString & " / " & File.TotalSize.ToString &
                                                                                  vbCrLf & "下载速度：" & File.Speed.ToString & "/s",
                                                                              "")
                                          End If

                                          '刷新图标
                                          Dim PictureSource As String = ""
                                          Select Case File.State
                                              Case WebDownloadStats.Success
                                                  PictureSource = PATH_IMAGE & "Stats-Finish.png"
                                              Case WebDownloadStats.Download, WebDownloadStats.GetSize
                                                  PictureSource = PATH_IMAGE & "Stats-Running.png"
                                              Case WebDownloadStats.Fail
                                                  PictureSource = PATH_IMAGE & "Stats-Fail.png"
                                              Case WebDownloadStats.Wait, WebDownloadStats.FirstLoad
                                                  PictureSource = PATH_IMAGE & "Stats-Wait.png"
                                              Case Else
                                                  PictureSource = PATH_IMAGE & "Stats-Retry.png"
                                          End Select
                                          If Not FileImage.Tag = PictureSource Then
                                              FileImage.Tag = PictureSource
                                              FileImage.Source = New BitmapImage(New Uri(PictureSource, UriKind.Absolute))
                                          End If

                                          '刷新右边的文本
                                          Dim ShouldText As String
                                          Select Case File.State
                                              Case WebDownloadStats.Download
                                                  ShouldText = If(File.TotalSize > 0, File.Percent * 100 & "%", "")
                                              Case WebDownloadStats.Fail
                                                  ShouldText = "下载失败"
                                              Case WebDownloadStats.Wait, WebDownloadStats.FirstLoad
                                                  ShouldText = ""
                                              Case WebDownloadStats.GetSize
                                                  ShouldText = "准备中"
                                              Case WebDownloadStats.Success
                                                  ShouldText = "下载成功"
                                              Case Else
                                                  ShouldText = "等待重试"
                                          End Select
                                          If Not ShouldText = FileLabel.Content Then
                                              FileLabel.Content = ShouldText
                                              '移除下载成功的文件
                                              If ShouldText = "下载成功" Then
                                                  RemoveFile(File)
                                              End If
                                          End If

                                          '刷新文件名
                                          If Not FileName.Content = File.LocalName Then FileName.Content = File.LocalName

                                      Catch
                                      End Try
                                  End Sub)
    End Sub

    ''' <summary>
    ''' 从 UI 中删除一个文件组。
    ''' </summary>
    Public Sub RemoveGroup(Group As WebGroup)
        frmMain.Dispatcher.Invoke(Sub()


                                      Dim Target As Grid = ControlDict("MainGrid" & Group.UUID)
                                      AniStart({
                                                   AaOpacity(Target, -1, 250),
                                                   AaHeight(Target, -Target.ActualHeight, 250, , New AniEaseEnd),
                                                   AaCode({"Remove", panShow.Children, Target}, , True)
                                               }, "Remove" & GetUUID())
                                      Target.Height = Target.ActualHeight
                                      RefreshPage()


                                  End Sub)
    End Sub

    ''' <summary>
    ''' 从 UI 中删除一个文件。
    ''' </summary>
    Public Sub RemoveFile(File As WebFile)
        frmMain.Dispatcher.Invoke(Sub()
                                      Try


                                          If Not WebGroups.Keys.Contains(File.GroupName) Then Exit Sub
                                          Dim FileGrid As Grid = ControlDict("FileGrid" & File.UUID)
                                          Dim Stack As StackPanel = ControlDict("Host" & WebGroups(File.GroupName).UUID)
                                          If Stack.Children.Count > 30 Then
                                              AniStart({
                                                  AaCode({"Remove", Stack.Children, FileGrid}, 700)
                                               }, "File" & File.UUID, False)
                                          Else
                                              AniStart({
                                                  AaX(FileGrid, -191, 200, 600, New AniEaseJumpStart(0.25)),
                                                  AaOpacity(FileGrid, -FileGrid.Opacity, 150, 700),
                                                  AaHeight(FileGrid, -22, 100, 750),
                                                  AaCode({"Remove", Stack.Children, FileGrid}, 850)
                                               }, "File" & File.UUID, False)
                                          End If


                                      Catch
                                      End Try
                                  End Sub)
    End Sub

    Private LastPageVisible As Boolean = True
    ''' <summary>
    ''' 刷新下载进度页可见性。
    ''' </summary>
    Private Sub RefreshPage()

        If WebGroups.Count = 0 Then
            '没有下载任务
            If scroll.IsHitTestVisible = True Or (LastPageVisible Xor frmDownloadLeft.panMain.Visibility = Visibility.Visible) Then
                LastPageVisible = frmDownloadLeft.panMain.Visibility = Visibility.Visible
                scroll.IsHitTestVisible = False
                If frmDownloadLeft.panMain.Visibility = Visibility.Collapsed Then
                    '下载页被隐藏
                    panMain.Width = 230
                    panMain.Opacity = 1
                    AniStart({
                                 AaOpacity(panBack, -panBack.Opacity, 200),
                                 AaOpacity(scroll, -scroll.Opacity, 200),
                                 AaOpacity(labNull, 0.9 - labNull.Opacity, 200),
                                 AaY(labNull, 146 - labNull.Margin.Top, 200, , New AniEaseEnd)
                             }, "DownloadNothing")
                Else
                    '下载页未被隐藏
                    AniStart({
                                 AaWidth(panMain, -panMain.Width, 200, , New AniEaseEnd),
                                 AaOpacity(panMain, -panMain.Opacity, 150),
                                 AaWidth(frmDownloadLeft.panMain, 227 + 504 - frmDownloadLeft.panMain.Width, 250, , New AniEaseEnd)
                             }, "DownloadNothing")
                End If
            End If
        Else
            '有下载任务
            If scroll.IsHitTestVisible = False Or (LastPageVisible Xor frmDownloadLeft.panMain.Visibility = Visibility.Visible) Then
                LastPageVisible = frmDownloadLeft.panMain.Visibility = Visibility.Visible
                scroll.IsHitTestVisible = True
                If frmDownloadLeft.panMain.Visibility = Visibility.Collapsed Then
                    '下载页被隐藏
                    panMain.Width = 230
                    panMain.Opacity = 1
                    AniStart({
                                 AaOpacity(panBack, 1 - panBack.Opacity, 200),
                                 AaOpacity(labNull, -labNull.Opacity, 200),
                                 AaY(labNull, 152 - labNull.Margin.Top, 200, , New AniEaseStart)
                             }, "DownloadNothing")
                Else
                    '下载页未被隐藏
                    AniStart({
                                 AaWidth(panMain, 230 - panMain.Width, 200, , New AniEaseEnd),
                                 AaOpacity(panMain, 1 - panMain.Opacity, 150, 50),
                                 AaWidth(frmDownloadLeft.panMain, 504 - frmDownloadLeft.panMain.Width, 250, , New AniEaseEnd)
                             }, "DownloadNothing")
                End If
            End If
        End If


    End Sub

End Class
