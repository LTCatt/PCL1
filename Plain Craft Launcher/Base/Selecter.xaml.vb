Public Class Selecter

    Public Event SelectionChange(ByVal Selection As String)
    Public Event Click(ByVal Selection As String, ByVal Index As Integer)
    Public NowSelect As New Label

#Region "属性"
    '线条宽度
    Public Property LineWidth As Integer
        Get
            Return lineSelect.Width
        End Get
        Set(ByVal value As Integer)
            lineSelect.Width = value
        End Set
    End Property
    '显示的选项
    Private _ShowList As Object() = {}
    Public Property ShowList As Object()
        Get
            Return _ShowList
        End Get
        Set(ByVal value As Object())
            If _ShowList.Equals(value) Then Exit Property
            _ShowList = value

            Try
                panLabs.Children.Clear() '清空已有选项
                Dim i = 0
                For Each selection As String In value
                    '每个选项
                    Dim newLabel As New Label With {.Padding = New Thickness(0), .Name = "LabSelect" & GetUUID(), .Content = selection, .Height = 32, .HorizontalContentAlignment = HorizontalAlignment.Left, .VerticalContentAlignment = VerticalAlignment.Center, .Background = New MyColor(1, 200, 200, 200), .Opacity = 0.5, .FontSize = panBack.FontSize}
                    newLabel.SetBinding(Control.ForegroundProperty, New Binding("Foreground") With {.ElementName = "panBack"})
                    If HintList.Length > i Then newLabel.ToolTip = HintList(i)
                    AddHandler newLabel.MouseEnter, AddressOf LabMouseEnter
                    AddHandler newLabel.MouseLeave, AddressOf LabMouseLeave
                    AddHandler newLabel.MouseUp, AddressOf LabMouseUp
                    '添加Label
                    panLabs.Children.Add(newLabel)
                    i = i + 1
                Next
                NowSelect = panLabs.Children(0)
                SelectIndex = 0
                AniStop(Me.Name)
                NowSelect.Opacity = 1
                SetTop(lineSelect, 4)
                panLabs.IsHitTestVisible = True
            Catch ex As Exception
                ExShow(ex, "设置选择条选项失败", ErrorLevel.MsgboxAndFeedback)
            End Try
        End Set
    End Property
    '选择选项
    Public SelectIndexName As String = ""
    Private _SelectIndex As Byte = 0
    Public Property SelectIndex As Byte
        Get
            Return _SelectIndex
        End Get
        Set(ByVal value As Byte)
            Try
                value = MathRange(value, 0, panLabs.Children.Count - 1)
                Dim sender As Label = panLabs.Children(value)
                If sender.Content.Equals(SelectIndexName) Then Exit Property
                SelectIndexName = sender.Content
                If If(IsNothing(Me.Name), True, Me.Name = "selecter") Then Me.Name = "selecter" & GetUUID()
                '动画
                panLabs.IsHitTestVisible = False
                Dim AnimationList As New ArrayList
                For Each Lab As Label In panLabs.Children
                    AniStart({AaOpacity(Lab, If(Lab.Equals(sender), 1, 0.5) - Lab.Opacity, 100)}, Lab.Name, False)
                Next
                AnimationList.Add(AaY(lineSelect, 4 + panLabs.Children.IndexOf(sender) * 32 - lineSelect.Margin.Top, , , New AniEaseJumpEnd(0.7)))
                AnimationList.Add(AaCode({"IsHitTestVisible", panLabs, True}, , True))
                AniStart(AnimationList, Me.Name)
                '切换选项
                NowSelect = sender
                '引发事件
                _SelectIndex = panLabs.Children.IndexOf(sender)
                RaiseEvent SelectionChange(NowSelect.Content)
            Catch ex As Exception
                ExShow(ex, "切换选择条选项失败", ErrorLevel.MsgboxAndFeedback)
            End Try
        End Set
    End Property
    '点击可以改变
    Public Property ClickToChange As Boolean = True
    '各个选项的指向提示
    Public Property HintList As String() = {}
#End Region

    Private Sub LabMouseEnter(ByVal sender As Label, ByVal e As System.Windows.Input.MouseEventArgs)
        If Not NowSelect.Equals(sender) Then
            AniStart({AaOpacity(sender, 0.8 - sender.Opacity, 70)}, sender.Name)
        End If
    End Sub
    Private Sub LabMouseLeave(ByVal sender As Label, ByVal e As System.Windows.Input.MouseEventArgs)
        If Not NowSelect.Equals(sender) Then
            AniStart({AaOpacity(sender, 0.5 - sender.Opacity, 70)}, sender.Name)
        End If
    End Sub
    Private Sub LabMouseUp(ByVal sender As Label, ByVal e As System.Windows.Input.MouseButtonEventArgs)
        If ClickToChange And Not NowSelect.Equals(sender) Then SelectIndex = panLabs.Children.IndexOf(sender)
        RaiseEvent Click(sender.Content, panLabs.Children.IndexOf(sender))
    End Sub
End Class
