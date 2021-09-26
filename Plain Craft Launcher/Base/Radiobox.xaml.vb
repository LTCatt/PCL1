Public Class Radiobox

    Public Event Change(ByVal sender As Object, ByVal raiseByMouse As Boolean)

    '自定义属性
    Private _Checked As Boolean = False    '是否选中
    Public Property Checked As Boolean
        Get
            Return _Checked
        End Get
        Set(ByVal value As Boolean)
            SetChecked(value, False)
        End Set
    End Property
    Public Sub SetChecked(ByVal value As Boolean, ByVal raiseByHand As Boolean)
        If Not value = _Checked Then '只有变更时才执行
            _Checked = value
            If Me.IsLoaded Then '防止默认属性加载时导致错误
                RaiseEvent Change(Me, raiseByHand)
                '同一容器中保证只有1个单选框选中
                Dim parentControl As Object = Me.Parent
                If IsNothing(parentControl) Then Exit Sub
                Dim checkedControl As ArrayList = New ArrayList
                Dim checkedCount As Byte = 0
                '统计单选框控件与选中个数
                For Each con In parentControl.Children
                    If con.GetType.Name = "Radiobox" Then
                        checkedControl.Add(con)
                        If con.Checked Then checkedCount = checkedCount + 1
                    End If
                Next
                Select Case checkedCount
                    Case 0 '没有选中项目
                        checkedControl(0).Checked = True
                    Case Is > 1 '选中项目多于1个
                        If Me.Checked Then
                            '如果本控件选中则取消其它所有控件的选中
                            For Each con As Radiobox In checkedControl
                                If con.Checked And Not con.Equals(Me) Then con.Checked = False
                            Next
                        Else
                            '如果本控件未选中则只保留第一个选中的控件
                            checkedCount = False '重复使用变量，实则统计是否为第一个选中控件
                            For Each con As Radiobox In checkedControl
                                If con.Checked Then
                                    If checkedCount Then
                                        con.Checked = False '修改Checked会自动触发Change事件，所以不用额外触发
                                    Else
                                        checkedCount = True
                                    End If
                                End If
                            Next
                        End If
                End Select
            End If

            If Me.IsLoaded And UseControlAnimation Then '防止默认属性变更触发动画
                If _Checked Then
                    AniStop("Ra" & Me.Name & "CheckedChange")
                    AniStart({
                             AaScale(border, 4 / border.Width - 1, 140),
                             AaOpacity(inner, 1 - inner.Opacity, 1, , , True),
                             AaScale(inner, 10 - inner.Width, 140, , , , True),
                             AaScale(border, 14 / border.Width, 140)
                    }, "Ra" & Me.Name & "CheckedChange")
                Else
                    AniStop("Ra" & Me.Name & "CheckedChange")
                    AniStart({
                             AaScale(border, 4 / border.Width - 1, 140),
                             AaScale(inner, 4 - inner.Width, 40, 100, , , True),
                             AaOpacity(inner, -inner.Opacity, 1, , , True),
                             AaScale(border, 14 / border.Width, 140)
                    }, "Ra" & Me.Name & "CheckedChange")
                End If
            Else
                inner.Opacity = If(_Checked, 1, 0)
            End If
        End If
    End Sub
    Public Property Context As String
        Get
            Return label.Content
        End Get
        Set(ByVal value As String)
            label.Content = value
        End Set
    End Property '内容

    '自定义事件
    Public Sub RaiseChange()
        RaiseEvent Change(Me, False)
    End Sub '用于其它控件来引发本控件的Change事件

    '默认事件
    Private Sub Radiobox_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles Me.MouseLeftButtonDown
        SetChecked(True, True)
    End Sub
    Private Sub Radiobox_IsEnabledChanged(ByVal sender As Object, ByVal e As System.Windows.DependencyPropertyChangedEventArgs) Handles Me.IsEnabledChanged
        If Me.IsLoaded And UseControlAnimation Then '防止默认属性变更触发动画
            If Me.IsEnabled Then
                AniStop("Ra" & Me.Name & "PointChange")
                AniStart({
                         AaBorderBrush(border, New MyColor(50, 50, 50) - border.BorderBrush, 150),
                         AaStroke(inner, New MyColor(50, 50, 50) - inner.Stroke, 150),
                         AaForeGround(label, New MyColor(50, 50, 50) - label.Foreground, 150)
                }, "Ra" & Me.Name & "PointChange")
            Else
                AniStop("Ra" & Me.Name & "PointChange")
                AniStart({
                         AaBorderBrush(border, New MyColor(145, 145, 145) - border.BorderBrush, 150),
                         AaStroke(inner, New MyColor(145, 145, 145) - inner.Stroke, 150),
                         AaForeGround(label, New MyColor(145, 145, 145) - label.Foreground, 150)
                }, "Ra" & Me.Name & "PointChange")
            End If
        Else
            If Me.IsEnabled Then
                inner.Stroke = New MyColor(50, 50, 50)
                border.BorderBrush = New MyColor(50, 50, 50)
                label.Foreground = New MyColor(50, 50, 50)
            Else
                inner.Stroke = New MyColor(145, 145, 145)
                border.BorderBrush = New MyColor(145, 145, 145)
                label.Foreground = New MyColor(145, 145, 145)
            End If
        End If
    End Sub

    '鼠标指向
    Private Sub Radiobox_MouseEnter(ByVal sender As Object, ByVal e As System.Windows.Input.MouseEventArgs) Handles Me.MouseEnter
        If Me.Name = "" Then Me.Name = "Radio" & GetUUID()
        AniStop("Ra" & Me.Name & "PointChange")
        AniStart({
                     AaOpacity(panMain, 1 - panMain.Opacity, 200)
        }, "Ra" & Me.Name & "PointChange")
    End Sub
    Private Sub Radiobox_MouseLeave(ByVal sender As Object, ByVal e As System.Windows.Input.MouseEventArgs) Handles Me.MouseLeave
        'MouseLeave比IsEnabledChanged后执行，所以如果自定义事件修改了IsEnabled，将导致颜色显示错误，这个判断语句为了修复此bug
        If Me.IsEnabled Then
            AniStop("Ra" & Me.Name & "PointChange")
            AniStart({
                     AaOpacity(panMain, 0.8 - panMain.Opacity, 200)
            }, "Ra" & Me.Name & "PointChange")
        End If
    End Sub

End Class