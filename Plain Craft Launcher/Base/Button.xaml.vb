Public Class Button

    '声明
    Public Event Click(ByVal sender As Object, ByVal e As EventArgs) '自定义事件

    '自定义属性 
    Public Property Text As String
        Get
            Return labText.Content
        End Get
        Set(ByVal value As String)
            labText.Content = value
        End Set
    End Property '显示文本
    Public Property TextPadding As Thickness
        Get
            Return labText.Padding
        End Get
        Set(ByVal value As Thickness)
            labText.Padding = value
        End Set
    End Property
    Private _ColorType As State = State.COMMON  '配色方案
    Public Property ColorType As State
        Get
            Return _ColorType
        End Get
        Set(ByVal value As State)
            _ColorType = value
            RefreshColor()
        End Set
    End Property
    Public Enum State As Byte
        COMMON = 0
        HIGHLIGHT = 1
        RED = 2
    End Enum

    '自定义事件
    Private Sub RefreshColor() Handles Me.MouseEnter, Me.MouseLeave, Me.Loaded, Me.IsEnabledChanged
        Try
            If Me.IsEnabled Then
                Select Case Me._ColorType '选取颜色种类
                    Case State.COMMON
                        If Me.IsMouseOver Then
                            '指向
                            AniStart({ _
                                     AaBackGround(Border, New MyColor(Color.FromArgb(255, 248, 248, 248)) - Border.Background, 150),
                                     AaBorderBrush(Me, New MyColor(Color.FromArgb(255, 42, 42, 42)) - Me.BorderBrush, 150),
                                     AaOpacity(Me, 1 - Me.Opacity, 200)
                                 }, "BtnColor" & Me.Name)
                        Else
                            '普通
                            AniStart({ _
                                     AaBackGround(Border, New MyColor(Color.FromArgb(128, 248, 248, 248)) - Border.Background, 150),
                                     AaBorderBrush(Me, New MyColor(Color.FromArgb(255, 42, 42, 42)) - Me.BorderBrush, 150),
                                     AaOpacity(Me, 0.8 - Me.Opacity, 200)
                                 }, "BtnColor" & Me.Name)
                        End If
                    Case State.HIGHLIGHT
                        If Me.IsMouseOver Then
                            '高亮指向
                            AniStart({
                                     AaBackGround(Border, New MyColor(Color.FromArgb(255, 248, 248, 248)) - Border.Background, 150),
                                     AaBorderBrush(Me, New MyColor(CType(Application.Current.Resources("Color5"), SolidColorBrush).Color) - Me.BorderBrush, 150),
                                     AaOpacity(Me, 1 - Me.Opacity, 200)
                                 }, "BtnColor" & Me.Name)
                        Else
                            '高亮普通
                            AniStart({
                                     AaBackGround(Border, New MyColor(Color.FromArgb(128, 248, 248, 248)) - Border.Background, 150),
                                     AaBorderBrush(Me, New MyColor(CType(Application.Current.Resources("Color5"), SolidColorBrush).Color) - Me.BorderBrush, 150),
                                     AaOpacity(Me, 0.8 - Me.Opacity, 200)
                                 }, "BtnColor" & Me.Name)
                        End If
                    Case State.RED
                        If Me.IsMouseOver Then
                            '红色指向
                            AniStart({ _
                                     AaBackGround(Border, New MyColor(Color.FromArgb(255, 248, 248, 248)) - Border.Background, 150),
                                     AaBorderBrush(Me, New MyColor(Color.FromArgb(255, 200, 20, 20)) - Me.BorderBrush, 150),
                                     AaOpacity(Me, 1 - Me.Opacity, 200)
                                 }, "BtnColor" & Me.Name)
                        Else
                            '红色普通
                            AniStart({ _
                                     AaBackGround(Border, New MyColor(Color.FromArgb(128, 248, 248, 248)) - Border.Background, 150),
                                     AaBorderBrush(Me, New MyColor(Color.FromArgb(255, 200, 20, 20)) - Me.BorderBrush, 150),
                                     AaOpacity(Me, 0.8 - Me.Opacity, 200)
                                 }, "BtnColor" & Me.Name)
                        End If
                End Select
            Else
                '灰色（不可用）
                AniStart({ _
                        AaBackGround(Border, New MyColor(Color.FromArgb(128, 248, 248, 248)) - Border.Background, 150),
                        AaBorderBrush(Me, New MyColor(Color.FromArgb(255, 144, 144, 144)) - Me.BorderBrush, 150),
                        AaOpacity(Me, 0.8 - Me.Opacity, 200)
                     }, "BtnColor" & Me.Name)
            End If
        Catch ex As Exception
            ExShow(ex, "刷新按钮出错")
        End Try
    End Sub

    '默认事件
    Private Sub Button_Mouse(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles Me.MouseLeftButtonUp
        RaiseEvent Click(sender, e)
    End Sub

End Class
