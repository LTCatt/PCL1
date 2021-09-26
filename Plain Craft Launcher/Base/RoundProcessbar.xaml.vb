Public Class RoundProcessbar

    '自定义属性
    Private _Max As Integer = 100 '最大值
    Public Property Max As Integer
        Get
            Return _Max
        End Get
        Set(ByVal value As Integer)
            _Max = value
            Refresh()
        End Set
    End Property
    Private _Value As Double = 66 '当前值
    Public Property Value As Double
        Get
            Return _Value
        End Get
        Set(ByVal value As Double)
            Me._Value = value
            Refresh()
        End Set
    End Property
    Private _Radius As Integer = 10 '半径
    Public Property Radius As Integer
        Get
            Return _Radius
        End Get
        Set(ByVal value As Integer)
            _Radius = value
            Refresh()
        End Set
    End Property
    Private _ShowAtTaskbar As Boolean = False '是否绑定到任务栏
    Public Property ShowAtTaskbar As Boolean
        Get
            Return _ShowAtTaskbar
        End Get
        Set(ByVal value As Boolean)
            If value <> Me._ShowAtTaskbar Then
                _ShowAtTaskbar = value
                Refresh()
            End If
        End Set
    End Property
    Private _TaskbarState As Integer = TaskbarItemProgressState.Normal  '显示在任务栏的状态
    Public Property TaskbarState As TaskbarItemProgressState
        Get
            Return _TaskbarState
        End Get
        Set(ByVal value As TaskbarItemProgressState)
            If _ShowAtTaskbar Then
                If Not value = _TaskbarState Then
                    Me._TaskbarState = value
                    Refresh()
                End If
            Else
                Me._TaskbarState = value
            End If
        End Set
    End Property
    Private _OverValue As Boolean = False '是否允许超范围的Value
    Public Property OverValue As Boolean
        Get
            Return _OverValue
        End Get
        Set(ByVal value As Boolean)
            If value <> Me._OverValue Then
                _OverValue = value
                Refresh()
            End If
        End Set
    End Property

    '默认事件
    Private Sub RoundProcessbar_SizeChanged(ByVal sender As Object, ByVal e As System.Windows.SizeChangedEventArgs) Handles Me.SizeChanged
        Refresh()
    End Sub

    '自定义事件
    Private Sub Refresh()
        Me._Value = If(Me._OverValue, Me._Value, MathRange(Me._Value, 0, Me._Max))
        Dim Angle As Double = 2 * Math.PI * MathRange(Me._Value / Me._Max, 0, 1) '计算饼状图角度
        If Me._Value / Me._Max > 0.5 Then
            Round.Data = New GeometryConverter().ConvertFrom( _
                "M" & Me.Width / 2 & ",0" & _
                "L" & Me.Width / 2 & "," & Me._Radius & _
                "A" & Me.Width / 2 - Me._Radius & "," & Me.Width / 2 - Me._Radius & ",0,0,1," & Me.Width / 2 & "," & Me.Width - Me._Radius & _
                "A" & Me.Width / 2 - Me._Radius & "," & Me.Width / 2 - Me._Radius & ",0,0,1," & Me.Width / 2 + (Me.Width / 2 - Me._Radius) * Math.Sin(Angle) & "," & Me.Width / 2 - (Me.Width / 2 - Me._Radius) * Math.Cos(Angle) & _
                "L" & Me.Width / 2 * (1 + Math.Sin(Angle)) & "," & Me.Width / 2 * (1 - Math.Cos(Angle)) & _
                "A" & Me.Width / 2 & "," & Me.Width / 2 & ",0,0,0," & Me.Width / 2 & "," & Me.Width & _
                "A" & Me.Width / 2 & "," & Me.Width / 2 & ",0,0,0," & Me.Width / 2 & ",0" & _
                "Z")
        Else
            Round.Data = New GeometryConverter().ConvertFrom( _
                "M" & Me.Width / 2 & ",0" & _
                "L" & Me.Width / 2 & "," & Me._Radius & _
                "A" & Me.Width / 2 - Me._Radius & "," & Me.Width / 2 - Me._Radius & ",0,0,1," & Me.Width / 2 + (Me.Width / 2 - Me._Radius) * Math.Sin(Angle) & "," & Me.Width / 2 - (Me.Width / 2 - Me._Radius) * Math.Cos(Angle) & _
                "L" & Me.Width / 2 * (1 + Math.Sin(Angle)) & "," & Me.Width / 2 * (1 - Math.Cos(Angle)) & _
                "A" & Me.Width / 2 & "," & Me.Width / 2 & ",0,0,0," & Me.Width / 2 & ",0" & _
                "Z")
        End If

        If _ShowAtTaskbar Then '如果在任务栏显示
            frmMain.Dispatcher.Invoke(Sub()
                                          If Me._Value / Me._Max < 0.0001 Then '没有进度时（兼容<0的Value）
                                              frmMain.TaskbarItemInfo.ProgressState = TaskbarItemProgressState.Indeterminate '设置为准备模式
                                          Else
                                              frmMain.TaskbarItemInfo.ProgressValue = MathRange(Me._Value, 0) / Me._Max '设置进度
                                              frmMain.TaskbarItemInfo.ProgressState = Me._TaskbarState '设置模式
                                          End If
                                      End Sub)
        End If

    End Sub '重绘饼状图

End Class
