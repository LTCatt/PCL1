Public Class Checkbox

    Public Event Change(ByVal sender As Object, ByVal raiseByMouse As Boolean)

    '自定义属性
    Private _Checked As Boolean = True   '是否选中
    Public Property Checked As Boolean
        Get
            Return _Checked
        End Get
        Set(ByVal value As Boolean)
            If Not value = _Checked Then '只有改变了才触发
                _Checked = value
                If Me.IsLoaded Then RaiseEvent Change(Me, False) 'Change事件只在控件加载后触发，以防止默认属性设置触发事件
                If Me.IsLoaded And UseControlAnimation Then '防止默认属性设置触发动画
                    Refresh()
                Else
                    Check.Opacity = If(_Checked, 1, 0)
                End If
            End If
        End Set
    End Property
    Public Property Context As String
        Get
            Return label.Content
        End Get
        Set(ByVal value As String)
            label.Content = value
        End Set
    End Property '内容

    '自定义事件
    Private Sub Refresh()
        'Checked变更
        If _Checked Then
            AniStop("Ch" & Me.Name & "CheckedChange") '停止旧动画实行相对动画
            AniStart({
                     AaScale(border, 4 / border.Width - 1, 150),
                     AaOpacity(Check, 1 - Check.Opacity, 1, , , True),
                     AaX(imgBlack, -14 - imgBlack.Margin.Left, 1),
                     AaY(imgBlack, -14 - imgBlack.Margin.Left, 1),
                     AaX(imgBlue, -14 - imgBlue.Margin.Left, 1),
                     AaY(imgBlue, -14 - imgBlue.Margin.Left, 1),
                     AaX(imgGray, -14 - imgGray.Margin.Left, 1),
                     AaY(imgGray, -14 - imgGray.Margin.Left, 1),
                     AaX(imgBlack, 14, 150, , , True),
                     AaY(imgBlack, 14, 150),
                     AaX(imgBlue, 14, 150),
                     AaY(imgBlue, 14, 150),
                     AaX(imgGray, 14, 150),
                     AaY(imgGray, 14, 150),
                     AaScale(border, 14 / border.Width, 150)
            }, "Ch" & Me.Name & "CheckedChange")
        Else
            AniStop("Ch" & Me.Name & "CheckedChange")
            AniStart({ _
                     AaX(imgBlack, imgBlack.Margin.Left - 14, 150),
                     AaY(imgBlack, imgBlack.Margin.Left - 14, 150),
                     AaX(imgBlue, imgBlue.Margin.Left - 14, 150),
                     AaY(imgBlue, imgBlue.Margin.Left - 14, 150),
                     AaX(imgGray, imgGray.Margin.Left - 14, 150),
                     AaY(imgGray, imgGray.Margin.Left - 14, 150),
                     AaScale(border, 4 / border.Width - 1, 150),
                     AaOpacity(Check, -Check.Opacity, 1, , , True),
                     AaScale(border, 14 / border.Width, 150) _
            }, "Ch" & Me.Name & "CheckedChange")
        End If
    End Sub

    '默认事件
    Private Sub Checkbox_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles Me.MouseDown
        _Checked = Not _Checked
        If Me.IsLoaded Then
            RaiseEvent Change(Me, True) 'Change事件只在控件加载后触发，以防止默认属性设置触发事件
            Refresh()
        Else
            Check.Opacity = If(_Checked, 1, 0)
        End If
    End Sub
    Private Sub Checkbox_IsEnabledChanged(ByVal sender As Object, ByVal e As System.Windows.DependencyPropertyChangedEventArgs) Handles Me.IsEnabledChanged
        If Me.IsLoaded And UseControlAnimation Then  '防止默认属性设置触发动画
            If Me.IsEnabled Then
                AniStop("Ch" & Me.Name & "PointChange")
                AniStart({
                         AaOpacity(imgBlack, 1 - imgBlack.Opacity, 150),
                         AaBorderBrush(border, New MyColor(50, 50, 50) - border.BorderBrush, 150),
                         AaForeGround(Me, New MyColor(50, 50, 50) - Me.Foreground, 150),
                         AaOpacity(imgBlue, -imgBlue.Opacity, 150, 50),
                         AaOpacity(imgGray, -imgGray.Opacity, 150, 50)
                }, "Ch" & Me.Name & "PointChange")
            Else
                AniStop("Ch" & Me.Name & "PointChange")
                AniStart({
                         AaOpacity(imgGray, 1 - imgGray.Opacity, 150),
                         AaBorderBrush(border, New MyColor(145, 145, 145) - border.BorderBrush, 150),
                         AaForeGround(Me, New MyColor(145, 145, 145) - Me.Foreground, 150),
                         AaOpacity(imgBlue, -imgBlue.Opacity, 150, 50),
                         AaOpacity(imgBlack, -imgBlack.Opacity, 150, 50)
                }, "Ch" & Me.Name & "PointChange")
            End If
        Else
            If Me.IsEnabled Then
                imgGray.Opacity = 0
                imgBlack.Opacity = 1
                border.BorderBrush = New MyColor(50, 50, 50)
                Me.Foreground = New MyColor(50, 50, 50)
            Else
                imgGray.Opacity = 1
                imgBlack.Opacity = 0
                border.BorderBrush = New MyColor(145, 145, 145)
                Me.Foreground = New MyColor(145, 145, 145)
            End If
        End If
    End Sub

    '鼠标指向
    Private Sub Checkbox_MouseEnter(ByVal sender As Object, ByVal e As System.Windows.Input.MouseEventArgs) Handles Me.MouseEnter
        If Me.Name = "" Then Me.Name = "Check" & GetUUID()
        AniStop("Ch" & Me.Name & "PointChange")
        AniStart({
                     AaOpacity(panMain, 1 - panMain.Opacity, 200)
        }, "Ch" & Me.Name & "PointChange")
    End Sub
    Private Sub Checkbox_MouseLeave(ByVal sender As Object, ByVal e As System.Windows.Input.MouseEventArgs) Handles Me.MouseLeave
        'MouseLeave比IsEnabledChanged后执行，所以如果自定义事件修改了IsEnabled，将导致颜色显示错误，这个判断语句为了修复此bug
        If Me.IsEnabled Then
            AniStop("Ch" & Me.Name & "PointChange")
            AniStart({
                     AaOpacity(panMain, 0.8 - panMain.Opacity, 200)
            }, "Ch" & Me.Name & "PointChange")
        End If
    End Sub

End Class
