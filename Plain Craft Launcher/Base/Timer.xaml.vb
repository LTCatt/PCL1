Imports System.Windows.Threading

Public Class Timer

    '声明
    Public Event Tick() '自定义事件

    Public Timer As New DispatcherTimer() '时钟
    Public TimerLoaded As Boolean = False '是否已经加载过了

    '自定义属性
    Private _Interval As Integer = 1000 '执行间隔
    Public Property Interval As Integer
        Get
            Try
                NameShow.Content = If(Me.Name.StartsWith("Timer") Or Me.Name.StartsWith("timer"), Me.Name.Remove(0, 5), Me.Name)
            Catch ex As Exception
                NameShow.Content = Me.Name
            End Try
            Return _Interval
        End Get
        Set(ByVal value As Integer)
            _Interval = value
            Timer.Interval = TimeSpan.FromMilliseconds(_Interval) '设置时钟Interval
            TimeShow.Content = If(_Interval < 11, "MAX", Interval) '设置显示的Interval（由于每秒最多执行64次，故设置10及以下显示MAX）
        End Set
    End Property

    '默认属性
    Private Sub Timer_IsEnabledChanged(ByVal sender As Object, ByVal e As System.Windows.DependencyPropertyChangedEventArgs) Handles Me.IsEnabledChanged
        Timer.IsEnabled = Me.IsEnabled '修改时钟Enabled
        If Me.IsEnabled Then '修改显示颜色
            Me.Background = New MyColor(Color.FromArgb(160, 85, 164, 255)) '蓝色
            Me.BorderBrush = New MyColor(Color.FromArgb(220, 10, 120, 255))
        Else
            Me.Background = New MyColor(Color.FromArgb(160, 255, 20, 0)) '红色
            Me.BorderBrush = New MyColor(Color.FromArgb(220, 255, 20, 0))
        End If
    End Sub

    '自定义事件
    ''' <summary>
    ''' 重新开始本次计时。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub Reset()
        Timer.Stop()
        Timer.Start()
        Timer.IsEnabled = Me.IsEnabled '修改时钟Enabled
    End Sub

    '默认事件
    Private Sub Timer_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        If TimerLoaded Then Exit Sub

        '初始化时钟
        TimerLoaded = True
        Timer.Interval = TimeSpan.FromMilliseconds(_Interval)
        AddHandler Timer.Tick, Sub() RaiseEvent Tick()
        Timer.Start()
        Timer.IsEnabled = Me.IsEnabled '修改时钟Enabled

        '修改Name显示
        Try
            NameShow.Content = If(Me.Name.StartsWith("Timer") Or Me.Name.StartsWith("timer"), Me.Name.Remove(0, 5), Me.Name)
        Catch ex As Exception
            NameShow.Content = Me.Name
        End Try

    End Sub

End Class
