Public Class ListItem

    Public Event Click(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) '单击事件
    Public Event Change(ByVal sender As Object, ByVal e As EventArgs) '选择改变事件
    Public Event ButtonClick(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) '按钮按下事件
    Public Event IconClick(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) '图标按下事件

#Region "自定义属性"

    Private _CanCheck As Boolean = True
    Public Property CanCheck As Boolean
        Get
            Return _CanCheck
        End Get
        Set(ByVal value As Boolean)
            _CanCheck = value
            Checked = False
        End Set
    End Property '能否选中
    Private _Checked As Boolean = False
    Public Property Checked As Boolean
        Get
            Return _Checked
        End Get
        Set(ByVal value As Boolean)
            If Not value = _Checked Then '只有变更时才执行
                _Checked = value
                If Me.IsLoaded Then '防止默认属性加载时导致错误
                    RaiseChange()
                    '同一容器中保证只有1个单选框选中
                    Dim parentControl As Object = Me.Parent
                    Dim checkedControl As ArrayList = New ArrayList
                    Dim checkedCount As Byte = 0
                    '统计单选框控件与选中个数
                    For Each con In parentControl.Children
                        If con.GetType.Name = "ListItem" Then
                            checkedControl.Add(con)
                            If con.Checked Then checkedCount = checkedCount + 1
                        End If
                    Next
                    Select Case checkedCount
                        Case 0 '没有选中项目
                            If AtLeastCheck Then checkedControl(0).Checked = True
                        Case Is > 1 '选中项目多于1个
                            If Me.Checked Then
                                '如果本控件选中则取消其它所有控件的选中
                                For Each con As ListItem In checkedControl
                                    If con.Checked And Not con.Equals(Me) Then con.Checked = False
                                Next
                            Else
                                '如果本控件未选中则只保留第一个选中的控件
                                checkedCount = False '重复使用变量，实则统计是否为第一个选中控件
                                For Each con As ListItem In checkedControl
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
                Refresh()
            End If
        End Set
    End Property '是否选中
    Private _ShowButton As Boolean = True
    Public Property ShowButton As Boolean
        Get
            Return _ShowButton
        End Get
        Set(ByVal value As Boolean)
            If Not value = _ShowButton Then '只有变更时才执行
                _ShowButton = value
                border.Visibility = If(value, Visibility.Visible, Visibility.Hidden)
                If _ShowButton = False Then ListItem_MouseLeave(Nothing, Nothing)
            End If
        End Set
    End Property '是否显示按钮
    Public Property MainText As String
        Get
            Return labMain.Content
        End Get
        Set(ByVal value As String)
            labMain.Content = value
        End Set
    End Property '主内容
    Public Property SubText As String
        Get
            Return labSub.Text
        End Get
        Set(ByVal value As String)
            If value = "" Xor labSub.Text = "" Then
                If Me.Name = "" Then Me.Name = "ListItem" & GetUUID()
                If Me.IsLoaded And UseControlAnimation Then
                    If value = "" Then
                        AniStart({
                                 AaY(labMain, (Me.Height - labMain.Height) / 2 - GetTop(labMain), , , New AniEaseEnd)
                        }, "Li" & Me.Name & "Sub")
                    Else
                        AniStart({
                                 AaY(labMain, -GetTop(labMain), , , New AniEaseEnd)
                        }, "Li" & Me.Name & "Sub")
                    End If
                Else
                    If value = "" Then
                        SetTop(labMain, (Me.Height - labMain.Height) / 2)
                    Else
                        SetTop(labMain, 0)
                    End If
                End If
            End If
            labSub.Text = value
        End Set
    End Property '副内容
    Public Property Logo As ImageSource
        Get
            Return imgLeft.Source
        End Get
        Set(ByVal value As ImageSource)
            imgLeft.Source = value
        End Set
    End Property '图标
    Public Property LogoSize As Double
        Get
            Return imgLeft.Height
        End Get
        Set(ByVal value As Double)
            imgLeft.Height = value
            imgLeft.Width = value
        End Set
    End Property '图标尺寸
    Public Property ButtonLogo As ImageSource
        Get
            Return btnRight.Source
        End Get
        Set(ByVal value As ImageSource)
            btnRight.Source = value
        End Set
    End Property '按钮图标
    Public Property ButtonLogoSize As Double
        Get
            Return btnRight.Height
        End Get
        Set(ByVal value As Double)
            btnRight.Height = value
            btnRight.Width = value
        End Set
    End Property '按钮图标尺寸
    Public Property ButtonBack As Brush
        Get
            Return border.Background
        End Get
        Set(ByVal value As Brush)
            border.Background = value
        End Set
    End Property '按钮背景颜色
    Public AtLeastCheck As Boolean = True '是否必须有一项被选中
    Public Version As New MCVersion("")

#End Region

    '自定义事件
    Public Sub RaiseChange()
        RaiseEvent Change(Me, Nothing)
    End Sub '用于其它控件来引发本控件的Change事件
    Private Sub Refresh()
        Dim shouldOpacity As Double = 0
        If Me.IsMouseOver Then shouldOpacity = 0.1
        If Me._Checked Then shouldOpacity = shouldOpacity / 2 + 0.14
        If shouldOpacity = labBack.Opacity Then
            AniStop("Li" & Me.Name & "Refresh")
            Exit Sub
        End If
        AniStart({
                 AaOpacity(labBack, shouldOpacity - labBack.Opacity, 100)
        }, "Li" & Me.Name & "Refresh", False)
    End Sub
    Private Sub btnRight_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles border.MouseLeftButtonDown
        RaiseEvent ButtonClick(Me, e)
    End Sub

    '系统事件
    '改成MouseDown是因为拖动滚动条后在其上释放鼠标会导致点击
    Private Sub ListItem_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles mouseCheck.MouseLeftButtonDown
        If CanCheck Then
            Me.Checked = True
            RaiseEvent Click(Me, e)
        End If
    End Sub
    Private Sub imgLeft_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles imgLeft.MouseLeftButtonDown
        RaiseEvent IconClick(Me, e)
        ListItem_MouseUp(sender, e)
    End Sub
    Private Sub ListItem_SizeChanged(ByVal sender As Object, ByVal e As System.Windows.SizeChangedEventArgs) Handles Me.SizeChanged
        panBack.Width = Me.Width + 0.51 '防止按钮莫名显示越界
        SetLeft(border, Me.ActualWidth - 40)
    End Sub
    Private Sub ListItem_MouseEnter(ByVal sender As Object, ByVal e As System.Windows.Input.MouseEventArgs) Handles Me.MouseEnter
        If ShowButton Then
            AniStart({
                    AaX(border, Me.ActualWidth - 81 - border.Margin.Left, 200, , New AniEaseJumpEnd(0.6))
                }, "Li" & Me.Name & "PointChange", False)
        End If
        Refresh()
    End Sub
    Private Sub ListItem_MouseLeave(ByVal sender As Object, ByVal e As System.Windows.Input.MouseEventArgs) Handles Me.MouseLeave
        If Me.ActualWidth = 0 Then
            '防止初始化加载
            Refresh()
            Exit Sub
        End If
        AniStart({
                AaX(border, Me.ActualWidth - 40 - border.Margin.Left, 150, , New AniEaseStart)
            }, "Li" & Me.Name & "PointChange", False)
        Refresh()
    End Sub

End Class
