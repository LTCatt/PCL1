Imports System.Drawing.Imaging
Imports System.Reflection
Imports System.Security.Cryptography
Imports System.Windows.Threading
Imports Ionic.Zip

Public Module Modules

#Region "Review完成 | 声明"

    ''' <summary>
    ''' 程序的启动路径，以“\”结尾。
    ''' </summary>
    ''' <remarks></remarks>
    Public PATH As String = GetPathFromFullPath(System.Windows.Forms.Application.ExecutablePath)

    ''' <summary>
    ''' 随机数。
    ''' </summary>
    ''' <remarks></remarks>
    Public Rnd As New Random

    ''' <summary>
    ''' 正在拖动的滚动条。
    ''' </summary>
    ''' <remarks></remarks>
    Public DragingScroll As VScroll = Nothing

    ''' <summary>
    ''' 位图缓存。
    ''' </summary>
    ''' <remarks></remarks>
    Public BitmapCache As New Dictionary(Of String, MyBitmap)

#End Region

#Region "Review完成 | API"

    ''' <summary>
    ''' 获取指定窗口的句柄。返回查找到的第一个窗口的句柄。
    ''' </summary>
    ''' <param name="lpClassName">窗口的类名，使用Spy++可以查看。</param>
    ''' <param name="lpWindowName">窗口的标题。</param>
    ''' <returns>查找到的第一个窗口的句柄。</returns>
    ''' <remarks></remarks>
    Public Declare Function FindWindow Lib "user32" Alias "FindWindowA" (ByVal lpClassName As String, ByVal lpWindowName As String) As Integer
    ''' <summary>
    ''' 彻底释放某个句柄对应项目的资源。
    ''' </summary>
    ''' <param name="hObject">需要释放资源的句柄。</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Declare Function DeleteObject Lib "gdi32" Alias "DeleteObject" (ByVal hObject As IntPtr) As Boolean
    ''' <summary>
    ''' 根据句柄设置对应窗口的标题。
    ''' </summary>
    ''' <param name="hwnd">窗口句柄。</param>
    ''' <param name="lpString">要设置的标题。</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Declare Function SetWindowText Lib "user32" Alias "SetWindowTextA" (ByVal hwnd As Integer, ByVal lpString As String) As Integer
    ''' <summary>
    ''' 获取某个句柄对应的窗口的所属进程ID。
    ''' </summary>
    ''' <param name="hwnd">窗口句柄。</param>
    ''' <param name="lpdwProcessId">获取到的ID。需要变量承载。</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Declare Function GetWindowThreadProcessId Lib "user32" Alias "GetWindowThreadProcessId" (ByVal hwnd As Integer, ByVal lpdwProcessId As Integer) As Integer
    ''' <summary>
    ''' 设置某个句柄对应的窗口的属性。
    ''' </summary>
    ''' <param name="hwnd">窗口句柄。</param>
    ''' <param name="hWndInsertAfter">置顶标记。-1 为置顶，-2 为不置顶。</param>
    ''' <param name="x">窗口的 X 坐标。</param>
    ''' <param name="y">窗口的 Y 坐标。</param>
    ''' <param name="cx">窗口的宽度。</param>
    ''' <param name="cy">窗口的高度。</param>
    ''' <param name="wFlags">利用 Or 连接的参数。
    ''' SWP_HIDEWINDOW：隐藏窗口。
    ''' SWP_SHOWWINDOW：显示窗口。
    ''' SWP_NOMOVE：维持当前位置（忽略 X 和 Y 参数）。
    ''' SWP_NOSIZE：维持当前尺寸（忽略 cx 和 cy 参数）。</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Declare Function SetWindowPos Lib "user32" Alias "SetWindowPos" (ByVal hwnd As Integer, ByVal hWndInsertAfter As Integer, ByVal x As Integer, ByVal y As Integer, ByVal cx As Integer, ByVal cy As Integer, ByVal wFlags As Integer) As Integer
    ''' <summary>
    ''' 播放 BGM。
    ''' </summary>
    ''' <param name="lpszSoundName">音乐文件的完整路径。</param>
    ''' <param name="hModule">填写“0”。</param>
    ''' <param name="dwFlags">填写“131072 Or 1 Or 8”。</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Declare Function PlaySound Lib "winmm.dll" (ByVal lpszSoundName As String, ByVal hModule As Integer, ByVal dwFlags As Integer) As Integer
    ''' <summary>
    ''' 播放 MP3 相关项。
    ''' </summary>
    ''' <param name="lpstrCommand"></param>
    ''' <param name="lpstrRetumString"></param>
    ''' <param name="uReturnLength"></param>
    ''' <param name="hwndCallback"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Declare Function mciSendString Lib "winmm.dll" Alias "mciSendStringA" (ByVal lpstrCommand As String, ByVal lpstrRetumString As String, ByVal uReturnLength As Integer, ByVal hwndCallback As Integer) As Integer

    Public Const SWP_HIDEWINDOW As Integer = 8 * 16 '0x0080
    Public Const SWP_SHOWWINDOW As Integer = 4 * 16 '0x0040
    Public Const SWP_NOMOVE As Integer = 2 '0x0002
    Public Const SWP_NOSIZE As Integer = 1 '0x0001

#End Region

#Region "类"

    ''' <summary>
    ''' 支持浮点数与常见类型隐式转换的颜色。
    ''' </summary>
    ''' <remarks></remarks>
    Public Class MyColor

        '属性
        Public A As Double = 0
        Public R As Double = 0
        Public G As Double = 0
        Public B As Double = 0

        '类型转换
        Public Shared Widening Operator CType(ByVal col As Color) As MyColor
            Return New MyColor(col)
        End Operator
        Public Shared Widening Operator CType(ByVal conv As MyColor) As Color
            Return Color.FromArgb(MathByte(conv.A), MathByte(conv.R), MathByte(conv.G), MathByte(conv.B))
        End Operator
        Public Shared Widening Operator CType(ByVal conv As MyColor) As System.Drawing.Color
            Return System.Drawing.Color.FromArgb(MathByte(conv.A), MathByte(conv.R), MathByte(conv.G), MathByte(conv.B))
        End Operator
        Public Shared Widening Operator CType(ByVal bru As SolidColorBrush) As MyColor
            Return New MyColor(bru.Color)
        End Operator
        Public Shared Widening Operator CType(ByVal conv As MyColor) As SolidColorBrush
            Return New SolidColorBrush(Color.FromArgb(MathByte(conv.A), MathByte(conv.R), MathByte(conv.G), MathByte(conv.B)))
        End Operator
        Public Shared Widening Operator CType(ByVal bru As Brush) As MyColor
            Return New MyColor(ColorConverter.ConvertFromString(bru.ToString))
        End Operator
        Public Shared Widening Operator CType(ByVal conv As MyColor) As Brush
            Return New SolidColorBrush(Color.FromArgb(MathByte(conv.A), MathByte(conv.R), MathByte(conv.G), MathByte(conv.B)))
        End Operator

        '颜色运算
        Shared Operator +(ByVal a As MyColor, ByVal b As MyColor) As MyColor
            Return New MyColor With {.A = a.A + b.A, .B = a.B + b.B, .G = a.G + b.G, .R = a.R + b.R}
        End Operator
        Shared Operator -(ByVal a As MyColor, ByVal b As MyColor) As MyColor
            Return New MyColor With {.A = a.A - b.A, .B = a.B - b.B, .G = a.G - b.G, .R = a.R - b.R}
        End Operator
        Shared Operator *(ByVal a As MyColor, ByVal b As Double) As MyColor
            Return New MyColor With {.A = a.A * b, .B = a.B * b, .G = a.G * b, .R = a.R * b}
        End Operator
        Shared Operator /(ByVal a As MyColor, ByVal b As Double) As MyColor
            Return New MyColor With {.A = a.A / b, .B = a.B / b, .G = a.G / b, .R = a.R / b}
        End Operator

        '构造函数
        Public Sub New()
        End Sub
        Public Sub New(ByVal col As Color)
            Me.A = col.A
            Me.R = col.R
            Me.G = col.G
            Me.B = col.B
        End Sub
        Public Sub New(ByVal newA As Double, ByVal col As Color)
            Me.A = newA
            Me.R = col.R
            Me.G = col.G
            Me.B = col.B
        End Sub
        Public Sub New(ByVal newR As Double, ByVal newG As Double, ByVal newB As Double)
            Me.A = 255
            Me.R = newR
            Me.G = newG
            Me.B = newB
        End Sub
        Public Sub New(ByVal newA As Double, ByVal newR As Double, ByVal newG As Double, ByVal newB As Double)
            Me.A = newA
            Me.R = newR
            Me.G = newG
            Me.B = newB
        End Sub

        '复写
        Public Shadows Function toString() As String
            Return "(" & A & "," & R & "," & G & "," & B & ")"
        End Function

    End Class

    Public Class AdvancedRect

        '属性
        Public Property Width As Double = 0
        Public Property Height As Double = 0
        Public Property Left As Double = 0
        Public Property Top As Double = 0

        '构造函数
        Public Sub New()
        End Sub
        Public Sub New(ByVal left As Double, ByVal top As Double, ByVal width As Double, ByVal height As Double)
            Me.Left = left
            Me.Top = top
            Me.Width = width
            Me.Height = height
        End Sub

    End Class '浮点矩形
    Public Class FileSize

        Public Value As Integer = 0

        '类型转换
        Public Shared Widening Operator CType(ByVal value As Long) As FileSize
            Return New FileSize(value)
        End Operator
        Public Shared Widening Operator CType(ByVal value As FileSize) As Long
            Return value.Value
        End Operator
        Public Shared Widening Operator CType(ByVal value As Integer) As FileSize
            Return New FileSize(value)
        End Operator
        Public Shared Widening Operator CType(ByVal value As FileSize) As Integer
            Return value.Value
        End Operator

        '运算
        Shared Operator +(ByVal a As FileSize, ByVal b As FileSize) As FileSize
            Return a.Value + b.Value
        End Operator
        Shared Operator -(ByVal a As FileSize, ByVal b As FileSize) As FileSize
            Return a.Value - b.Value
        End Operator
        Shared Operator *(ByVal a As FileSize, ByVal b As FileSize) As Integer
            Return a.Value * b.Value
        End Operator
        Shared Operator /(ByVal a As FileSize, ByVal b As FileSize) As Double
            Return MathRange(a.Value / b.Value, 0)
        End Operator
        Shared Operator =(ByVal a As FileSize, ByVal b As FileSize) As Boolean
            Return a.Value = b.Value
        End Operator
        Shared Operator <>(ByVal a As FileSize, ByVal b As FileSize) As Boolean
            Return Not a.Value = b.Value
        End Operator
        Shared Operator >(ByVal a As FileSize, ByVal b As FileSize) As Boolean
            Return a.Value > b.Value
        End Operator
        Shared Operator <(ByVal a As FileSize, ByVal b As FileSize) As Boolean
            Return a.Value < b.Value
        End Operator
        Shared Operator &(ByVal a As FileSize, ByVal b As String) As String
            Return a.ToString & b
        End Operator
        Shared Operator &(ByVal a As String, ByVal b As FileSize) As String
            Return a & b.ToString
        End Operator

        '构造函数
        Public Sub New()
        End Sub
        Public Sub New(ByVal value As Integer)
            Me.Value = value
        End Sub

        '复写
        Public Shadows Function ToString() As String
            Select Case Me.Value
                Case Is < 0
                    Return "未知"
                Case Is < 1024
                    Return Me.Value & "B"
                Case Is < 1024 ^ 2
                    Return Math.Round(Me.Value / 1024, 1) & "K"
                Case Is < 1024 ^ 3
                    Return Math.Round(Me.Value / 1024 ^ 2, 2) & "M"
                Case Else
                    Return Math.Round(Me.Value / 1024 ^ 3, 2) & "G"
            End Select
        End Function

    End Class '文件大小

    Public Class Time

        Private Property Value As Integer = 0

        '类型转换
        Public Shared Widening Operator CType(ByVal value As Integer) As Time
            Return New Time(value)
        End Operator
        Public Shared Widening Operator CType(ByVal value As Time) As Integer
            Return value.Value
        End Operator
        Public Shared Widening Operator CType(ByVal value As Int64) As Time
            Return New Time(value)
        End Operator
        Public Shared Widening Operator CType(ByVal value As Time) As Int64
            Return value.Value
        End Operator

        '运算
        Shared Operator +(ByVal a As Time, ByVal b As Time) As Time
            Return a.Value + b.Value
        End Operator
        Shared Operator -(ByVal a As Time, ByVal b As Time) As Time
            Return a.Value - b.Value
        End Operator
        Shared Operator *(ByVal a As Time, ByVal b As Time) As Integer
            Return a.Value * b.Value
        End Operator
        Shared Operator /(ByVal a As Time, ByVal b As Time) As Double
            Return MathRange(a.Value / b.Value, 0)
        End Operator
        Shared Operator =(ByVal a As Time, ByVal b As Time) As Boolean
            Return a.Value = b.Value
        End Operator
        Shared Operator <>(ByVal a As Time, ByVal b As Time) As Boolean
            Return Not a.Value = b.Value
        End Operator
        Shared Operator >(ByVal a As Time, ByVal b As Time) As Boolean
            Return a.Value > b.Value
        End Operator
        Shared Operator <(ByVal a As Time, ByVal b As Time) As Boolean
            Return a.Value < b.Value
        End Operator
        Shared Operator &(ByVal a As Time, ByVal b As String) As String
            Return a.toString & b
        End Operator
        Shared Operator &(ByVal a As String, ByVal b As Time) As String
            Return a & b.toString
        End Operator

        '构造函数
        Public Sub New()
        End Sub
        Public Sub New(ByVal value As Integer)
            Me.Value = value
        End Sub

        '复写
        Public Shadows Function toString() As String
            Dim left As Integer = Me.Value
            Dim a As Integer
            a = Math.Floor(left / 3600)
            toString = FillLength(a.ToString, "0", 2)
            left = left - a * 3600
            a = Math.Floor(left / 60)
            toString = toString & ":" & FillLength(a.ToString, "0", 2) & ":" & FillLength((left - a * 60), "0", 2)
        End Function

    End Class '时间

#End Region

#Region "控件"

    ''' <summary>
    ''' 获取控件或窗体的左边距。无视Alignment。
    ''' </summary>
    ''' <param name="control"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetLeft(ByVal control) As Double
        GetLeft = 0
        If control.GetType.BaseType.Name.Equals("Window") Then
            Return control.Left
        Else
            Select Case control.HorizontalAlignment
                Case HorizontalAlignment.Left, HorizontalAlignment.Stretch, HorizontalAlignment.Center
                    Return control.Margin.Left
                Case HorizontalAlignment.Right
                    Return CType(control.Parent, Object).ActualWidth - control.ActualWidth - control.Margin.Right
                    'Case HorizontalAlignment.Stretch
                    '    Dim MaxWidth As Double = CType(control.Parent, Object).ActualWidth - control.Margin.Left - control.Margin.Right
                    '    Return If(control.ActualWidth >= MaxWidth, control.Margin.Left, control.Margin.Left + (MaxWidth - control.ActualWidth) / 2)
                    'Case HorizontalAlignment.Center
                    '    Dim CenterPos As Double = control.Margin.Left + (CType(control.Parent, Object).ActualWidth - control.Margin.Left - control.Margin.Right) / 2
                    '    Return CenterPos - control.ActualWidth / 2
            End Select
        End If
    End Function
    ''' <summary>
    ''' 获取控件或窗体的顶边距。无视Alignment。
    ''' </summary>
    ''' <param name="control"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetTop(ByVal control) As Double
        GetTop = 0
        If control.GetType.BaseType.Name.Equals("Window") Then
            Return control.Top
        Else
            Select Case control.VerticalAlignment
                Case VerticalAlignment.Top, VerticalAlignment.Stretch, VerticalAlignment.Center
                    Return control.Margin.Top
                Case VerticalAlignment.Bottom
                    Return -control.Margin.Bottom
                    'Return CType(control.Parent, Object).ActualHeight - control.ActualHeight - control.Margin.Bottom
                    'Case VerticalAlignment.Stretch
                    '    Dim MaxHeight As Double = CType(control.Parent, Object).ActualHeight - control.Margin.Top - control.Margin.Bottom
                    '    Return If(control.ActualHeight >= MaxHeight, control.Margin.Top, control.Margin.Top + (MaxHeight - control.ActualHeight) / 2)
                    'Case VerticalAlignment.Center
                    '    Dim CenterPos As Double = control.Margin.Top + (CType(control.Parent, Object).ActualHeight - control.Margin.Top - control.Margin.Bottom) / 2
                    '    Return CenterPos - control.ActualHeight / 2
            End Select
        End If
    End Function

    ''' <summary>
    ''' 将一个IList中的内容添加到另外一个IList。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub IListCopy(ByVal list As IList, ByVal arr As IList)
        For Each item In arr
            list.Add(item)
        Next
    End Sub

    Public Sub SetLeft(ByVal control As Object, ByVal newValue As Double)
        If Double.IsNaN(newValue) Or Double.IsInfinity(newValue) Then Exit Sub '安全性检查

        If control.GetType.BaseType.Name.Equals("Window") Then
            control.Left = newValue
        Else
            Select Case control.HorizontalAlignment
                Case HorizontalAlignment.Left, HorizontalAlignment.Stretch, HorizontalAlignment.Center
                    control.Margin = New Thickness(newValue, control.Margin.Top, control.Margin.Right, control.Margin.Bottom)
                Case HorizontalAlignment.Right
                    control.Margin = New Thickness(control.Margin.Left, control.Margin.Top, CType(control.Parent, Object).ActualWidth - control.ActualWidth - newValue, control.Margin.Bottom)
            End Select
        End If
    End Sub '设置Left
    Public Sub SetTop(ByVal control As Object, ByVal newValue As Double)
        If Double.IsNaN(newValue) Or Double.IsInfinity(newValue) Then Exit Sub '安全性检查

        If control.GetType.BaseType.Name.Equals("Window") Then
            control.Top = newValue
        Else
            Select Case control.VerticalAlignment
                Case VerticalAlignment.Top, VerticalAlignment.Stretch, VerticalAlignment.Center
                    control.Margin = New Thickness(control.Margin.Left, newValue, control.Margin.Right, control.Margin.Bottom)
                Case VerticalAlignment.Bottom
                    control.Margin = New Thickness(control.Margin.Left, control.Margin.Top, control.Margin.Right, -newValue)
                    'control.Margin = New Thickness(control.Margin.Left, control.Margin.Top, control.Margin.Right, CType(control.Parent, Object).ActualHeight - control.ActualHeight - newValue)
            End Select
        End If
    End Sub '设置Top

    ''' <summary>
    ''' 拖动滚动条。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub ControlStartRun()
        Dim timer As New DispatcherTimer()
        timer.Interval = TimeSpan.FromMilliseconds(20)
        AddHandler timer.Tick, Sub()

                                   If IsNothing(DragingScroll) Then Exit Sub

                                   If Mouse.LeftButton = MouseButtonState.Pressed Then
                                       DragingScroll.Draging(Mouse.GetPosition(DragingScroll.btnDrag).Y)
                                   Else
                                       '鼠标放下
                                       If Not DragingScroll.IsMouseOver Then DragingScroll.DragMouseLeave()
                                       DragingScroll = Nothing
                                   End If

                               End Sub
        timer.Start()
    End Sub

#End Region

#Region "数学"

    ''' <summary>
    ''' 获取两数间的百分比。小数点精确到6位。
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function MathPercent(ByVal ValueA As Double, ByVal ValueB As Double, ByVal Percent As Object) As Double
        Return Math.Round(ValueA * (1 - Percent) + ValueB * Percent, 6) '解决Double计算错误
    End Function
    ''' <summary>
    ''' 获取两颜色间的百分比，根据RGB计算。小数点精确到6位。
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function MathPercent(ByVal ValueA As MyColor, ByVal ValueB As MyColor, ByVal Percent As Object) As MyColor
        Return MathRound(ValueA * (1 - Percent) + ValueB * Percent, 6) '解决Double计算错误
    End Function

    ''' <summary>
    ''' 检查一个数字是否正确（如检查无限、NaN等）。
    ''' </summary>
    ''' <param name="Num">需要检查的数字。</param>
    ''' <returns>是否正确。</returns>
    ''' <remarks></remarks>
    Public Function MathCheck(ByVal Num As Double) As Boolean
        Return Not (Double.IsInfinity(Num) Or Double.IsNaN(Num))
    End Function

    '贝塞尔曲线计算
    Public Function MathBezier(ByVal t As Double, ByVal x1 As Double, ByVal y1 As Double, ByVal x2 As Double, ByVal y2 As Double) As Double
        Dim a, b
        a = t
        Do
            b = 3 * a * ((1 / 3 + x1 - x2) * a * a + (x2 - 2 * x1) * a + x1)
            a = a + (t - b) * 0.5
        Loop Until Math.Abs(b - t) < 0.01 '精度1%
        Return 3 * a * ((1 / 3 + y1 - y2) * a * a + (y2 - 2 * y1) * a + y1)
    End Function
    'Byte合理化
    Public Function MathByte(ByVal d As Double) As Byte
        If d < 0 Then d = 0
        If d > 255 Then d = 255
        Return Math.Round(d)
    End Function
    '提供扩展类支持的Round
    Public Function MathRound(ByVal col As MyColor, Optional ByVal w As Integer = 0) As MyColor
        Return New MyColor With {.A = Math.Round(col.A, w), .R = Math.Round(col.R, w), .G = Math.Round(col.G, w), .B = Math.Round(col.B, w)}
    End Function
    '把数值限定范围
    Public Function MathRange(ByVal value As Double, ByVal min As Double, Optional ByVal max As Double = Double.MaxValue) As Double
        Return Math.Max(min, Math.Min(max, value))
    End Function
    'Byte类溢出处理
    Public Function ByteOverflow(ByVal int As Integer) As Byte
        Dim Original = int
        Dim ReturnValue As Byte
        Try
            If int < 0 Then
                Do Until int >= 0
                    int = int + 256
                Loop
                ReturnValue = int
            Else
                ReturnValue = int Mod 256
            End If
            Return ReturnValue
        Catch ex As Exception
            ExShow(ex, "Byte 溢出处理出错：" & int)
            Return 0
        End Try
    End Function

#End Region

#Region "文本"
    '填充长度
    Public Function FillLength(ByVal str As String, ByVal code As String, ByVal length As Byte)
        If (Len(str) > length) Then Return Mid(str, 1, length)
        Return Mid(str.Replace(" ", "-").PadRight(length), str.Length + 1).Replace(" ", code) & str
    End Function
    '选择其一
    Public Function OneOf(ByVal objects As Array)
        Return objects.GetValue(RandomInteger(0, objects.Length - 1))
    End Function
    Public Function OneOf(ByVal objects As ArrayList)
        Return objects(RandomInteger(0, objects.Count - 1))
    End Function
    '正则搜索
    Public Function RegexSearch(ByVal str As String, ByVal regex As String) As ArrayList
        Try
            RegexSearch = New ArrayList
            Dim RegexSearchRes = (New RegularExpressions.Regex(regex)).Matches(str)
            If IsNothing(RegexSearchRes) Then Return RegexSearch
            For Each item As RegularExpressions.Match In RegexSearchRes
                RegexSearch.Add(item.Value)
            Next
        Catch
            Return New ArrayList
        End Try
    End Function
    '加密解密
    Public Function SerAdd(ByVal str As String) As String
        Dim text As Byte() = Encoding.Unicode.GetBytes(str)
        '-----------------------------------------------1
        Array.Reverse(text)
        '-----------------------------------------------2
        Dim out(text.Length - 1) As Byte
        For i As Integer = 0 To text.Length - 4 Step 4
            out(i) = text(i + 2)
            out(i + 1) = text(i + 1)
            out(i + 2) = text(i + 3)
            out(i + 3) = text(i)
        Next i
        If text.Length Mod 4 = 2 Then
            out(text.Length - 2) = text(text.Length - 1)
            out(text.Length - 1) = text(text.Length - 2)
        End If
        text = out
        '-----------------------------------------------3
        ReDim out(text.Length - 1)
        For i As Integer = 0 To text.Length - 3 Step 3
            out(i) = ByteOverflow(text(i + 2) + 12)
            out(i + 1) = ByteOverflow(text(i) - 85)
            out(i + 2) = ByteOverflow(text(i + 1) + 36)
        Next i
        Select Case text.Length Mod 3
            Case 1
                out(text.Length - 1) = ByteOverflow(text(text.Length - 1) - 52)
            Case 2
                out(text.Length - 1) = ByteOverflow(text(text.Length - 1) + 16)
                out(text.Length - 2) = ByteOverflow(text(text.Length - 2) + 48)
        End Select
        text = out
        '-----------------------------------------------4
        For i As Integer = 0 To text.Length - 1
            text(i) = ByteOverflow(text(i) + i)
        Next i
        '-----------------------------------------------E
        SerAdd = ""
        For Each bt As Byte In text
            SerAdd = SerAdd & BitConverter.ToString({bt})
        Next
    End Function
    Public Function SerRemove(ByVal str As String) As String
        Dim line1 As String, line2 As String
        line1 = "这就是解密函数……自己写的……很口胡很辣鸡……做一个加密只是为了防那些脑残熊孩子，既然都能搞到这函数了，那加密也没用，所以就别吐槽这辣鸡加密了吧233"
        line2 = "by 龙腾猫跃"
        Dim text(Len(str) / 2 - 1) As Byte
        For i As Integer = 0 To Len(str) - 2 Step 2
            text(i / 2) = Byte.Parse(Mid(str, i + 1, 2), Globalization.NumberStyles.AllowHexSpecifier)
        Next
        '-----------------------------------------------4
        For i As Integer = 0 To text.Length - 1
            text(i) = ByteOverflow(text(i) - i)
        Next i
        '-----------------------------------------------3
        Dim out(text.Length - 1) As Byte
        For i As Integer = 0 To text.Length - 3 Step 3
            out(i) = ByteOverflow(text(i + 1) + 85)
            out(i + 1) = ByteOverflow(text(i + 2) - 36)
            out(i + 2) = ByteOverflow(text(i) - 12)
        Next i
        Select Case text.Length Mod 3
            Case 1
                out(text.Length - 1) = ByteOverflow(text(text.Length - 1) + 52)
            Case 2
                out(text.Length - 1) = ByteOverflow(text(text.Length - 1) - 16)
                out(text.Length - 2) = ByteOverflow(text(text.Length - 2) - 48)
        End Select
        text = out
        '-----------------------------------------------2
        ReDim out(text.Length - 1)
        For i As Integer = 0 To text.Length - 4 Step 4
            out(i) = text(i + 3)
            out(i + 1) = text(i + 1)
            out(i + 2) = text(i)
            out(i + 3) = text(i + 2)
        Next i
        If text.Length Mod 4 = 2 Then
            out(text.Length - 2) = text(text.Length - 1)
            out(text.Length - 1) = text(text.Length - 2)
        End If
        text = out
        '-----------------------------------------------1
        Array.Reverse(text)
        '-----------------------------------------------E
        SerRemove = Encoding.Unicode.GetString(text)
    End Function
    ''' <summary>
    ''' 从用户输入的字符串中提取数字
    ''' </summary>
    ''' <param name="str">用户输入的字符串</param>
    ''' <returns>字符串中的数字，没有任何数字返回0</returns>
    ''' <remarks></remarks>
    Public Function GetDoubleFromString(ByVal str As String) As Double
        Dim regResult As ArrayList = RegexSearch(str, "(-?[0-9]\d*)?\.?[0-9]\d*")
        If regResult.Count = 0 Then
            Return 0
        Else
            Return Val(regResult(0))
        End If
    End Function
    '把Exception转为字符串
    Public Function GetStringFromException(ByVal ex As Exception, Optional ByVal isLong As Boolean = False) As String
        '构造基本信息与InnerException信息
        GetStringFromException = ex.Message.Replace(vbCrLf, "")
        If Not IsNothing(ex.InnerException) Then
            GetStringFromException = GetStringFromException & vbCrLf & "InnerException: " & ex.InnerException.Message.Replace(vbCrLf, "")
            If Not IsNothing(ex.InnerException.InnerException) Then
                GetStringFromException = GetStringFromException & vbCrLf & "InnerException: " & ex.InnerException.InnerException.Message.Replace(vbCrLf, "")
            End If
        End If
        '添加堆栈信息
        If isLong Then
            GetStringFromException = GetStringFromException & vbCrLf
            For Each Stack As String In ex.StackTrace.Split(vbCrLf)
                If Stack.Contains("pcl.") Then GetStringFromException = GetStringFromException & vbCrLf & Stack
            Next
        End If
        '清除双回车
        GetStringFromException = GetStringFromException.Replace(vbCrLf & vbCrLf, vbCrLf).Replace(vbCrLf & vbCrLf, vbCrLf)
    End Function
#End Region

#Region "Review完成 | 图片"

    ''' <summary>
    ''' 裁剪一个 BitmapSource。
    ''' </summary>
    ''' <param name="img">源图片。</param>
    Public Function CutBitmap(ByVal img As BitmapSource, ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer) As BitmapSource
        Try
            Return New CroppedBitmap(img, New Int32Rect(x, y, width, height))
        Catch ex As Exception
            ExShow(ex, "裁剪图片失败", ErrorLevel.AllUsers)
            Return Nothing
        End Try
    End Function

#End Region

#Region "文件"

    ''' <summary>
    ''' 从应用资源释放文件。
    ''' </summary>
    ''' <param name="FileName">需要释放的文件在应用资源中的名称。</param>
    ''' <param name="OutputPath">释放的路径，包含文件名。</param>
    ''' <remarks></remarks>
    Public Sub OutputFileInResource(ByVal FileName As String, ByVal OutputPath As String, ByVal PassOnExists As Boolean)
        Try
            Dim Res = My.Resources.ResourceManager.GetObject(FileName)
            If IsNothing(Res) Then Throw New FileNotFoundException("没有找到名为" & FileName & "的资源")
            If PassOnExists And File.Exists(OutputPath) Then
                If GetFileSize(OutputPath) = Res.Length Then Exit Sub
            End If
            Directory.CreateDirectory(Mid(OutputPath, 1, OutputPath.LastIndexOf("\")))
            Using OutputStream As New FileStream(OutputPath, FileMode.OpenOrCreate, FileAccess.Write)
                OutputStream.Write(Res, 0, Res.Length)
                OutputStream.Flush()
            End Using
            log("[System] 释放文件：" & FileName)
        Catch ex As Exception
            ExShow(ex, "释放文件失败", ErrorLevel.DebugOnly)
            Throw ex
        End Try
    End Sub

    ''' <summary>
    ''' 解压文件。
    ''' </summary>
    ''' <param name="SourceFile">需要解压的文件的完整路径。</param>
    ''' <param name="UnzipDir">解压的目标文件夹。</param>
    ''' <param name="RunThread">解压结束后（无论成功还是失败）执行的方法。</param>
    ''' <remarks></remarks>
    Public Sub UnzipFile(ByVal SourceFile As String, ByVal UnzipDir As String, Optional ByVal RunThread As ParameterizedThreadStart = Nothing)
        Dim th As New Thread(Sub()
                                 Try
                                     Directory.CreateDirectory(UnzipDir)
                                     Using zip As New ZipFile(SourceFile)
                                         zip.ExtractAll(UnzipDir)
                                     End Using
                                 Catch ex As Exception
                                     ExShow(ex, "解压文件失败：" & SourceFile)
                                 End Try
                                 If Not IsNothing(RunThread) Then RunThread.Method.Invoke(Nothing, {Nothing})
                             End Sub)
        th.Start()
    End Sub '解压ZIP文件

    ''' <summary>
    ''' 弹出选取文件对话框并且要求选取文件。
    ''' </summary>
    ''' <param name="FileFilter">要求的格式。如：“常用图片文件(*.png;*.jpeg)|*.png;*.jpg;*.jpeg”。</param>
    ''' <param name="Title">弹窗的标题。</param>
    ''' <returns>选择的唯一文件的完整路径，如果没有选择可能为空字符串。</returns>
    ''' <remarks></remarks>
    Public Function SelectFile(ByVal FileFilter As String, ByVal Title As String) As String
        Using fileDialog As New Forms.OpenFileDialog
            fileDialog.AddExtension = True
            fileDialog.AutoUpgradeEnabled = True
            fileDialog.CheckFileExists = True
            fileDialog.Filter = FileFilter
            fileDialog.Multiselect = False
            fileDialog.Title = Title
            fileDialog.ShowDialog()
            Return fileDialog.FileName
        End Using
    End Function

    ''' <summary>
    ''' 弹出选取文件夹对话框并且要求选取文件夹。
    ''' </summary>
    ''' <param name="Title">弹窗的标题。</param>
    ''' <returns>选择的唯一文件夹的完整路径，如果没有选择可能为空字符串。</returns>
    ''' <remarks></remarks>
    Public Function SelectFolder(ByVal Title As String) As String
        Using folderDialog As New Forms.FolderBrowserDialog
            folderDialog.Description = Title
            folderDialog.ShowNewFolderButton = True
            folderDialog.ShowDialog()
            Return folderDialog.SelectedPath
        End Using
    End Function

    ''' <summary>
    ''' 从文件路径或者URL获取不包含文件名的路径。不包含路径将会抛出异常。
    ''' </summary>
    ''' <param name="FilePath">文件路径，以标识符结尾。如果不包含文件名则必须以标识符结尾。</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetPathFromFullPath(ByVal FilePath As String) As String
        If Not (FilePath.Contains("\") Or FilePath.Contains("/")) Then Throw New FileFormatException("不包含路径：" & FilePath)
        If FilePath.EndsWith("\") Or FilePath.EndsWith("/") Then Return FilePath
        GetPathFromFullPath = Left(FilePath, FilePath.LastIndexOfAny({"\", "/"}) + 1)
        If GetPathFromFullPath = "" Then Throw New FileFormatException("不包含路径：" & FilePath)
    End Function

    ''' <summary>
    ''' 从文件路径或者URL获取不包含路径的文件名。不包含文件名将会抛出异常。
    ''' </summary>
    ''' <param name="FilePath">文件路径。可以不完整。</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetFileNameFromPath(ByVal FilePath As String) As String
        If FilePath.EndsWith("\") Or FilePath.EndsWith("/") Then Throw New FileFormatException("不包含文件名：" & FilePath)
        If Not (FilePath.Contains("\") Or FilePath.Contains("/")) Then Return FilePath
        GetFileNameFromPath = Mid(FilePath, FilePath.LastIndexOfAny({"\", "/"}) + 2)
        If GetFileNameFromPath.Contains("\\") And GetFileNameFromPath.Contains("?") Then
            '包含网络参数
            GetFileNameFromPath = Left(GetFileNameFromPath, FilePath.LastIndexOf("?"))
        End If
        If GetFileNameFromPath = "" Then Throw New FileFormatException("不包含文件名：" & FilePath)
    End Function

    ''' <summary>
    ''' 获取文件的版本信息。
    ''' </summary>
    ''' <param name="Path">文件目录。</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetFileVersion(ByVal Path As String) As String
        Try
            Return Diagnostics.FileVersionInfo.GetVersionInfo(Path).FileVersion
        Catch ex As Exception
            ExShow(ex, "获取文件版本失败：" & Path, ErrorLevel.Slient)
            Return ""
        End Try
    End Function

    ''' <summary>
    ''' 获取一个文件夹下的所有文件的完整路径。
    ''' </summary>
    ''' <param name="SourcePath">源文件夹，不以“\”结尾。</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetFilesFromPath(ByVal SourcePath As String) As ArrayList
        GetFilesFromPath = New ArrayList
        GetFilesFromPath(SourcePath, GetFilesFromPath)
    End Function
    Private Function GetFilesFromPath(ByVal SourcePath As String, ByRef CurrentList As ArrayList) As ArrayList
        Try
            If Not Directory.Exists(SourcePath) Then Return New ArrayList
            '列举文件
            For Each File As FileInfo In (New DirectoryInfo(SourcePath)).GetFiles
                CurrentList.Add(File.FullName)
            Next
            '列举目录
            For Each Folder As DirectoryInfo In (New DirectoryInfo(SourcePath)).GetDirectories
                GetFilesFromPath(Folder.FullName, CurrentList)
            Next
        Catch ex As Exception
            ExShow(ex, "列举文件失败：" & SourcePath, ErrorLevel.Slient)
        End Try
        '返回
        Return CurrentList
    End Function

    '获取文件大小（HTTP URL或文件路径）
    Public Function GetFileSize(ByVal Path As String, Optional ByRef FileMission As WebFile = Nothing) As Integer
        Try
            If Path.StartsWith("http") Then
                If MODE_OFFLINE Then Return 0
                '获取网络文件大小
                Dim Res As Net.WebRequest = Net.WebRequest.Create(Path)
                Dim Response As WebResponse = Res.GetResponse
                Dim Size As Integer = Response.ContentLength()
                If Not IsNothing(FileMission) Then
                    '用Ref返回实际地址
                    If Not FileMission.AllWebAddress(0) = Response.ResponseUri.ToString Then
                        If FileMission.RefreshName Then FileMission.LocalName = GetFileNameFromPath(Response.ResponseUri.ToString)
                        FileMission.AllWebAddress(0) = Response.ResponseUri.ToString
                    End If
                End If
                '释放资源
                Response.Close()
                Res.Abort()
                Return Size
            Else
                If File.Exists(Path) Then
                    Return New FileInfo(Path).Length
                Else
                    Return 0
                End If
            End If
        Catch ex As Exception
            ExShow(ex, "获取文件大小失败：" & Path, ErrorLevel.Slient)
            Return 0
        End Try
    End Function
    '获取MD5
    Public Function GetFileMD5(ByVal filepath As String) As String
        Try
            '网上抄的啊，自己改了改而已XD
            Dim result As String = ""
            Using fstream As New FileStream(filepath, FileMode.Open, FileAccess.Read)
                Dim dataToHash(fstream.Length - 1) As Byte
                fstream.Read(dataToHash, 0, fstream.Length)
                Dim hashvalue As Byte() = CType(CryptoConfig.CreateFromName("MD5"), HashAlgorithm).ComputeHash(dataToHash)
                Dim i As Double
                For i = 0 To hashvalue.Length - 1
                    result += FillLength(Hex(hashvalue(i)).ToLower, "0", 2)
                Next
            End Using
            Return result
        Catch ex As Exception
            ExShow(ex, "获取文件MD5失败：" & filepath, ErrorLevel.Slient)
            Return ""
        End Try
    End Function
    '读取文件
    Public Function ReadFileToEnd(ByVal filePath As String, Optional ByVal encode As Encoding = Nothing, Optional ByVal isFullPath As Boolean = True) As String
        Try
            Dim actualPath As String = If(isFullPath, "", PATH) & filePath
            If File.Exists(actualPath) Then
                encode = If(IsNothing(encode), New UTF8Encoding(False), encode)
                Using reader As New StreamReader(actualPath, encode)
                    ReadFileToEnd = reader.ReadToEnd
                End Using
            Else
                Return ""
            End If
        Catch ex As Exception
            ExShow(ex, "读取文件出错：" & filePath)
            Return ""
        End Try
    End Function
    '写入文件
    Public Sub WriteFile(ByVal filePath As String, ByVal text As String, Optional ByVal add As Boolean = False, Optional ByVal encode As Encoding = Nothing, Optional ByVal isFullPath As Boolean = True)
        Try
            Dim actualpath As String = If(isFullPath, "", PATH) & filePath
            If Not File.Exists(actualpath) Then
                Directory.CreateDirectory(GetPathFromFullPath(actualpath))
                File.Create(actualpath).Dispose()
            End If
            encode = If(IsNothing(encode), New UTF8Encoding(False), encode)
            Using writer As New StreamWriter(actualpath, add, encode)
                writer.Write(text)
                writer.Flush()
            End Using
        Catch ex As Exception
            ExShow(ex, "写入文件时出错：" & filePath)
        End Try
    End Sub

    Private Class IniCache
        Public content As String = ""
        Public cache As New Dictionary(Of String, String)
    End Class
    Private CacheIni As New Dictionary(Of String, IniCache)
    ''' <summary>
    ''' 获取Ini文件内容，这可能会使用到缓存。
    ''' </summary>
    ''' <param name="FileName">文件路径或简写。简写将会使用“PCL\文件名.ini”作为路径。</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetIni(ByVal FileName As String) As IniCache
        If Not FileName.Contains(":\") Then FileName = PATH & "PCL\" & FileName & ".ini"
        If CacheIni.ContainsKey(FileName) Then
            '返回缓存中的信息
            Dim Cache As New IniCache
            CacheIni.TryGetValue(FileName, Cache)
            Return Cache '防止ByRef导致缓存变更
        Else
            If File.Exists(FileName) Then
                '返回文件信息并且记入缓存
                Dim Cache As String = (vbCrLf & ReadFileToEnd(FileName) & vbCrLf).Replace(vbCrLf, vbCr).Replace(vbLf, vbCr).Replace(vbCr, vbCrLf).Replace(vbCrLf & vbCrLf, vbCrLf)
                Dim Ini As New IniCache With {.content = Cache}
                CacheIni.Add(FileName, Ini)
                Return Ini
            Else
                '返回空信息
                Dim Ini As New IniCache With {.content = ""}
                CacheIni.Add(FileName, Ini)
                Return Ini
            End If
        End If
    End Function
    ''' <summary>
    ''' 读取Ini文件，这可能会使用到缓存。
    ''' </summary>
    ''' <param name="FileName">文件路径或简写。简写将会使用“PCL\文件名.ini”作为路径。</param>
    ''' <param name="Key">键。</param>
    ''' <param name="DefaultValue">没有找到键时返回的默认值。</param>
    Public Function ReadIni(ByVal FileName As String, ByVal Key As String, Optional ByVal DefaultValue As String = "") As String
        Try
            '获取目前文件
            Dim NowIni As IniCache = GetIni(FileName)
            If IsNothing(NowIni) Then Return DefaultValue
            '使用缓存
            If NowIni.cache.ContainsKey(Key) Then Return If(NowIni.cache(Key), DefaultValue)
            '新读取文件
            If NowIni.content.Contains(vbCrLf & Key & ":") Then
                Dim Ret As String = Mid(NowIni.content, NowIni.content.IndexOf(vbCrLf & Key & ":") + 3)
                Ret = If(Ret.Contains(vbCrLf), Mid(Ret, 1, Ret.IndexOf(vbCrLf)), Ret).Replace(Key & ":", "")
                NowIni.cache.Add(Key, If(Ret = vbLf, "", Ret))
                Return If(Ret = vbLf, "", Ret)
            Else
                Return DefaultValue
            End If
        Catch ex As Exception
            '读取失败
            Return DefaultValue
        End Try
    End Function
    ''' <summary>
    ''' 写入ini文件，这会更新缓存。
    ''' </summary>
    ''' <param name="FileName">文件路径或简写。简写将会使用“PCL\文件名.ini”作为路径。</param>
    ''' <param name="Key">键。</param>
    ''' <param name="Value">值。</param>
    ''' <remarks></remarks>
    Public Sub WriteIni(ByVal FileName As String, ByVal Key As String, ByVal Value As String)
        On Error Resume Next
        FileName = If(FileName.Contains(":\"), FileName, PATH & "PCL\" & FileName & ".ini")
        If IsNothing(Value) Then Value = ""
        Value = Value.Replace(vbCrLf, "")
        '创建文件夹
        If Not Directory.Exists(GetPathFromFullPath(FileName)) Then Directory.CreateDirectory(GetPathFromFullPath(FileName))
        '获取目前文件
        Dim NowFile As String = GetIni(FileName).content
        '如果值一样就不处理
        If NowFile.Contains(vbCrLf & Key & ":" & Value & vbCrLf) Then Exit Sub
        '处理文件
        Dim FindResult As String = ReadIni(FileName, Key)
        If FindResult = "" And Not NowFile.Contains(vbCrLf & Key & ":") Then
            '不存在这个键
            NowFile = NowFile & vbCrLf & Key & ":" & Value
        Else
            '存在这个键
            NowFile = NowFile.Replace(vbCrLf & Key & ":" & FindResult & vbCrLf, vbCrLf & Key & ":" & Value & vbCrLf)
        End If
        WriteFile(FileName, NowFile)
        '刷新目前缓存
        CacheIni(FileName).content = NowFile
        CacheIni(FileName).cache.Remove(Key)
        CacheIni(FileName).cache.Add(Key, Value)
    End Sub
#End Region

#Region "Review完成 | 网络"

    ''' <summary>
    ''' 获取网页源代码。
    ''' </summary>
    ''' <param name="URL">网页的URL。</param>
    ''' <param name="Encode">网页的编码，通常为UTF-8。</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetWebsiteCode(ByVal URL As String, ByVal Encode As Encoding) As String
        If MODE_OFFLINE Then Return ""

        '初始化
        Dim req As HttpWebRequest = Nothing
        Dim res As HttpWebResponse = Nothing
        Dim strm As StreamReader = Nothing
        GetWebsiteCode = ""

        Try
            req = WebRequest.Create(URL)
            req.Timeout = 6000
            res = req.GetResponse()
            strm = New StreamReader(res.GetResponseStream(), Encode)
            GetWebsiteCode = strm.ReadToEnd
        Catch ex As Exception
            If Not URL.Contains("https://sessionserver.mojang.com/session/minecraft/profile/") Then ExShow(ex, "获取网页源代码失败：" & URL)
        Finally
            '释放资源
            If Not IsNothing(strm) Then strm.Dispose()
            If Not IsNothing(res) Then res.Close()
            If Not IsNothing(req) Then req.Abort()
        End Try
    End Function

    ''' <summary>
    ''' 从网络中直接下载文件。
    ''' </summary>
    ''' <param name="URL">网络URL。</param>
    ''' <param name="localFile">下载的本地地址。</param>
    ''' <returns>是否下载成功。</returns>
    ''' <remarks></remarks>
    Public Function DownloadFile(ByVal URL As String, ByVal LocalFile As String) As Boolean
        If MODE_OFFLINE Then Return False

        '初始化
        Try
            '建立目录
            Directory.CreateDirectory(GetPathFromFullPath(LocalFile))
            '尝试删除原文件
            File.Delete(LocalFile)
        Catch ex As Exception
            ExShow(ex, "预处理文件失败：" & LocalFile)
            Return False
        End Try

        '下载
        Using Client As New WebClient
            Try
                Client.DownloadFile(URL, LocalFile)
                log("[System] 直接下载文件成功：" & URL)
                Return True
            Catch ex As Exception
                ExShow(ex, "直接下载文件失败：" & URL)
                Return False
            End Try
        End Using

    End Function

    ''' <summary>
    ''' 进行Ping测试。这不会异步进行，失败将直接抛出异常。
    ''' </summary>
    ''' <param name="IP">要进行Ping的IP。</param>
    ''' <param name="MaxPingTime">最大响应时间，单位为毫秒。</param>
    ''' <returns>Ping用时，单位为毫秒。</returns>
    ''' <remarks></remarks>
    Public Function Ping(ByVal IP As String, ByVal MaxPingTime As Integer) As Integer
        If MODE_OFFLINE Then Throw New WebException("没有网络连接。")
        Using pin = New System.Net.NetworkInformation.Ping
            Return pin.Send(IP, MaxPingTime).RoundtripTime
        End Using
    End Function

#End Region

#Region "系统"

    ''' <summary>
    ''' 测试某段代码用时，单位为毫秒/次。
    ''' </summary>
    ''' <param name="run">需要测试的代码。</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function TimeTest(ByVal run As ThreadStart) As Double
        Dim StartTime As Long = My.Computer.Clock.TickCount
        Dim RunCount As Integer = 0
        Do While True
            run.Invoke()
            RunCount = RunCount + 1
            If My.Computer.Clock.TickCount - StartTime > 1000 Or RunCount > 100000 Then Exit Do
        Loop
        Return (My.Computer.Clock.TickCount - StartTime) / RunCount
    End Function

    ''' <summary>
    ''' 打开文件（等同于使用“运行”窗口）。
    ''' </summary>
    ''' <param name="FileName">文件名。可以为“notepad”等缩写。</param>
    ''' <param name="Arguments">运行参数。</param>
    ''' <param name="WaitForExit">是否等待该程序结束。默认为False。</param>
    ''' <remarks></remarks>
    Public Sub Shell(ByVal FileName As String, Optional ByVal Arguments As String = "", Optional ByVal WaitForExit As Boolean = False)
        Try
            Dim Program As New Process
            Program.StartInfo.Arguments = Arguments
            Program.StartInfo.FileName = FileName
            Program.Start()
            If WaitForExit Then Program.WaitForExit()
        Catch ex As Exception
            ExShow(ex, "执行命令失败：" & FileName, ErrorLevel.AllUsers)
        End Try
    End Sub

    ''' <summary>
    ''' 返回一个枚举对应的字符串。
    ''' </summary>
    ''' <param name="EnumData">一个已经实例化的枚举类型。</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetStringFromEnum(ByVal EnumData As Object) As String
        Return [Enum].GetName(EnumData.GetType, EnumData)
    End Function

    ''' <summary>
    ''' 从指定的枚举中查找某字符串对应的枚举项。
    ''' </summary>
    ''' <param name="EnumData">源枚举。</param>
    ''' <param name="Value">对应枚举一项的字符串。</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetEnumFromString(ByVal EnumData As Object, ByVal Value As String)
        Return [Enum].Parse(EnumData.GetType, Value)
    End Function

    ''' <summary>
    ''' 获取格式类似于“11:08:52.037”的当前时间的字符串。
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetTime() As String
        Return Date.Now.ToLongTimeString & "." & FillLength(Date.Now.Millisecond, "0", 3)
    End Function

    ''' <summary>
    ''' 数组去重。
    ''' </summary>
    ''' <param name="array"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ArrayNoDouble(ByVal array As Array) As ArrayList
        Dim ResultArray As New ArrayList

        For i = 0 To UBound(array)
            For ii = i + 1 To UBound(array)
                If array(i).Equals(array(ii)) Then GoTo NextElement
            Next
            ResultArray.Add(array(i))
NextElement:
        Next i

        Return ResultArray
    End Function

    ''' <summary>
    ''' 将数组从大到小排序。
    ''' </summary>
    ''' <param name="Array">纯数字的ArrayList。</param>
    ''' <remarks></remarks>
    Public Sub ArrayBigToSmall(ByRef Array As ArrayList)
        If Array.Count < 2 Then Exit Sub
        For i = 1 To Array.Count - 1
            If Array(i) > Array(i - 1) Then
                Dim c = Array(i - 1)
                Array(i - 1) = Array(i)
                Array(i) = c
                If i >= 2 Then i = i - 2
            End If
        Next i
    End Sub

    ''' <summary>
    ''' 强制将一个支持IList接口的对象转换为任意格式的一维数组。
    ''' </summary>
    ''' <typeparam name="T">转换后的数组类型。</typeparam>
    ''' <param name="Source"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ArrayConventer(Of T, B As IList)(ByVal Source As B) As T()
        Dim Re(Source.Count - 1) As T
        For i = 0 To Source.Count - 1
            Re(i) = Source(i)
        Next
        Return Re
    End Function

    ''' <summary>
    ''' 检查是否拥有某一文件夹的读取权限。如果文件夹不存在，会返回 False。
    ''' </summary>
    ''' <param name="Path">检查目录。</param>
    Public Function CheckDirectoryPermission(ByVal Path As String) As Boolean
        Try
            If Path = "" Then Return False
            If Not Directory.Exists(Path) Then Return False
            Dim folderCheck As New DirectoryInfo(Path)
            folderCheck.EnumerateFiles()
            Return True
        Catch
            Return False
        End Try
    End Function

    ''' <summary>
    ''' 获取程序启动参数。
    ''' </summary>
    ''' <param name="Name">参数名。</param>
    ''' <param name="DefaultValue">默认值。</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetProgramArgument(ByVal Name As String, Optional ByVal DefaultValue As Object = "")
        Dim AllArguments() As String = Microsoft.VisualBasic.Command.Split(" ")
        For i = 0 To AllArguments.Length - 1
            If AllArguments(i) = "-" & Name Then
                If AllArguments.Length = i + 1 Then Return True
                If AllArguments(i + 1).StartsWith("-") Then Return True
                Return AllArguments(i + 1)
            End If
        Next
        Return DefaultValue
    End Function

    ''' <summary>
    ''' 获取Json对象。
    ''' </summary>
    ''' <param name="Text"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ReadJson(ByVal Text As String) As JObject
        Try
            Return CType(Newtonsoft.Json.JsonConvert.DeserializeObject(Text), JObject)
        Catch ex As Exception
            ExShow(ex, "Json读取失败：" & Left(Text, 30))
            Throw New FileFormatException("错误的 Json：" & GetStringFromException(ex, True))
        End Try
    End Function

    '执行命令行函数
    'Public Function RunCMD(ByVal Commands As String, ByVal Wait As Boolean) As String
    '    Dim myProcess As New Process()
    '    Dim myProcessStartInfo As New ProcessStartInfo("cmd.exe")
    '    myProcessStartInfo.UseShellExecute = False
    '    myProcessStartInfo.RedirectStandardOutput = True
    '    myProcessStartInfo.CreateNoWindow = True
    '    myProcessStartInfo.Arguments = "/c """ & Commands & """"
    '    myProcess.StartInfo = myProcessStartInfo
    '    myProcess.Start()
    '    If Wait Then
    '        myProcess.WaitForExit()
    '        Dim myStreamReader As IO.StreamReader = myProcess.StandardOutput
    '        Dim myString As String = myStreamReader.ReadToEnd()
    '        myProcess.Close()
    '        myProcess.Dispose()
    '        Return myString
    '    Else
    '        Return ""
    '    End If
    'End Function
    '注册表IO
    Public Function ReadReg(ByVal Key As String, Optional ByVal DefaultValue As String = "") As String
        Try
            Dim parentKey As Microsoft.Win32.RegistryKey, softKey As Microsoft.Win32.RegistryKey
            parentKey = My.Computer.Registry.CurrentUser
            softKey = parentKey.OpenSubKey("Software\" & APPLICATION_SHORT_NAME, True)
            If softKey Is Nothing Then
                ReadReg = DefaultValue '不存在则返回默认值
            Else
                Dim readValue As New System.Text.StringBuilder
                readValue.AppendLine(softKey.GetValue(Key))
                Dim value = readValue.ToString.Replace(vbCrLf, "") '去除莫名的回车
                Return If(value = "", DefaultValue, value) '错误则返回默认值
            End If
        Catch ex As Exception
            ExShow(ex, "读取注册表出错：" & Key, ErrorLevel.Slient)
            Return DefaultValue
        End Try
    End Function
    Public Sub WriteReg(ByVal Key As String, ByVal Value As String, Optional ByVal ShowException As Boolean = False)
        Try
            Dim parentKey As Microsoft.Win32.RegistryKey, softKey As Microsoft.Win32.RegistryKey
            parentKey = My.Computer.Registry.CurrentUser
            softKey = parentKey.OpenSubKey("Software\" & APPLICATION_SHORT_NAME, True)
            If softKey Is Nothing Then softKey = parentKey.CreateSubKey("Software\" & APPLICATION_SHORT_NAME) '如果不存在就创建  
            softKey.SetValue(Key, Value)
        Catch ex As Exception
            ExShow(ex, "写入注册表出错：" & Key, If(ShowException, modMain.ErrorLevel.DebugOnly, modMain.ErrorLevel.Slient))
            If ShowException Then Throw '如果显示错误就丢一个
        End Try
    End Sub

    Private UUID As Integer = 0
    ''' <summary>
    ''' 获取一个全程序内不会重复的数字（伪UUID）。
    ''' </summary>
    ''' <returns>UUID，一个大于0的有序整数。</returns>
    ''' <remarks></remarks>
    Public Function GetUUID() As Integer
        UUID = UUID + 1
        Return UUID
    End Function

#End Region

#Region "Review完成 | 窗体"

    ''' <summary>
    ''' 等待显示的弹窗。
    ''' </summary>
    ''' <remarks></remarks>
    Public WaitingMyMsgbox As ArrayList = If(IsNothing(WaitingMyMsgbox), New ArrayList, WaitingMyMsgbox)
    ''' <summary>
    ''' 显示弹窗。
    ''' </summary>
    Public Function MyMsgbox(ByVal Converter As MyMsgboxConverter) As Integer
        Try

            '存入等待列表
            If Thread.CurrentThread.IsBackground Or Not Thread.CurrentThread.GetApartmentState = ApartmentState.STA Or Not AniRunning Then
                If IsNothing(WaitingMyMsgbox) Then WaitingMyMsgbox = New ArrayList '初始化
                WaitingMyMsgbox.Add(Converter)
                '返回值
                If Converter.IsWaitExit Then
                    Do While Converter.ReturnCode = 0
                        Thread.Sleep(50)
                    Loop
                    Return Converter.ReturnCode
                Else
                    Return 0
                End If
            End If

            '在主线程，显示窗体
            Dim MsgBox As New MetroMsgbox(Converter) With {.Owner = frmMain, .Width = MAINFORM_WIDTH - 20, .Height = MAINFORM_HEIGHT - 20, .Left = (My.Computer.Screen.WorkingArea.Width - MAINFORM_WIDTH) / 2 + 10, .Top = (My.Computer.Screen.WorkingArea.Height - MAINFORM_HEIGHT) / 2 + 10}
            If Converter.IsWaitExit Then
                MsgBox.ShowDialog()
                Return Converter.ReturnCode
            Else
                MsgBox.Show()
                Return 0
            End If

        Catch ex As Exception
            ExShow(ex, "显示弹窗失败：" & Converter.Title, ErrorLevel.MsgboxAndFeedback)
            Return 0
        End Try
    End Function
    ''' <summary>
    ''' 显示弹窗。
    ''' </summary>
    ''' <param name="Title">弹窗的标题。</param>
    ''' <param name="Caption">弹窗的内容。</param>
    ''' <param name="Button1">显示的第一个按钮，默认为“确定”。</param>
    ''' <param name="Button2">显示的第二个按钮，默认为空。</param>
    ''' <param name="Button3">显示的第三个按钮，默认为空。</param>
    ''' <param name="IsWarn">是否为警告弹窗，若为 True，第一个按钮会显示为红色。</param>
    ''' <param name="IsWaitExit">是否等待弹窗关闭。若为 True，将会返回从 1 开始的点击的按钮编号，否则返回 0。</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function MyMsgbox(ByVal Caption As String, Optional ByVal Title As String = APPLICATION_FULL_NAME, Optional ByVal Button1 As String = "确定", Optional ByVal Button2 As String = "", Optional ByVal Button3 As String = "", Optional ByVal IsWarn As Boolean = False, Optional ByVal IsWaitExit As Boolean = True) As Integer
        Dim NewMsgBoxConverter As New MyMsgboxConverter With {.Button1 = Button1, .Button2 = Button2, .Button3 = Button3, .Caption = Caption, .IsWarn = IsWarn, .Title = Title, .IsWaitExit = IsWaitExit}
        Return MyMsgbox(NewMsgBoxConverter)
    End Function
    ''' <summary>
    ''' 存储弹窗信息的转换器。
    ''' </summary>
    ''' <remarks></remarks>
    Public Class MyMsgboxConverter
        Public Title As String
        Public Caption As String = ""
        Public Button1 As String = "确定"
        Public Button2 As String = ""
        Public Button3 As String = ""
        Public IsWarn As Boolean = False
        Public IsWaitExit As Boolean = True
        ''' <summary>
        ''' 弹窗是否已经关闭。
        ''' </summary>
        ''' <remarks></remarks>
        Public IsExited As Boolean = False
        ''' <summary>
        ''' 点击的按钮编号，从 1 开始。
        ''' </summary>
        ''' <remarks></remarks>
        Public ReturnCode As Integer = 0
    End Class

    'Public Function MyMsgbox(ByVal title As String, ByVal page As Grid, Optional ByVal caption As String = "", Optional ByVal btn1 As String = "确定", Optional ByVal btn2 As String = "", Optional ByVal btn3 As String = "", Optional ByVal wait As Boolean = False) As Object
    '    Dim wind As New MetroMsgbox With {.Owner = frmMain, .Width = MAINFORM_WIDTH - 20, .Height = MAINFORM_HEIGHT - 20, .Left = (My.Computer.Screen.WorkingArea.Width - MAINFORM_WIDTH) / 2 + 10, .Top = (My.Computer.Screen.WorkingArea.Height - MAINFORM_HEIGHT) / 2 + 10}
    '    wind.labTitle.Text = title
    '    wind.panCaption.Child = page
    '    wind.btn1.Text = btn1
    '    wind.btn2.Text = btn2
    '    wind.btn3.Text = btn3
    '    wind.btn2.Visibility = If(btn2 = "", Visibility.Collapsed, Visibility.Visible)
    '    wind.btn3.Visibility = If(btn3 = "", Visibility.Collapsed, Visibility.Visible)
    '    msgboxCallback = 0
    '    If wait Then
    '        wind.Show()
    '        Return wind
    '    Else
    '        wind.ShowDialog()
    '        Return msgboxCallback
    '    End If
    'End Function

    ''' <summary>设置 WebBrowser 静默，即是否不产生弹窗。</summary>
    ''' <param name="webBrowser">需要改变的 WebBrowser。</param>
    ''' <param name="IsSilent">是否开启静默模式。</param>
    Public Sub SetWebBrowserSilent(ByVal WebBrowser As WebBrowser, ByVal IsSilent As Boolean)
        Try

            Dim fi As FieldInfo = WebBrowser.GetType.GetField("_axIWebBrowser2", BindingFlags.Instance Or BindingFlags.NonPublic)
            If Not IsNothing(fi) Then
                Dim browser = fi.GetValue(WebBrowser)
                If Not IsNothing(browser) Then
                    browser.GetType().InvokeMember("Silent", BindingFlags.SetProperty, Nothing, browser, {IsSilent})
                End If
            End If

        Catch ex As Exception
            ExShow(ex, "设置浏览器失败", ErrorLevel.AllUsers)
        End Try
    End Sub
#End Region

#Region "随机"

    '打乱数组
    Public Function RandomArray(ByVal array As ArrayList) As ArrayList
        RandomArray = New ArrayList
        Do While array.Count > 0
            Dim i As Integer = RandomInteger(0, array.Count - 1)
            RandomArray.Add(array(i))
            array.RemoveAt(i)
        Loop
    End Function
    '获取随机整数
    Public Function RandomInteger(ByVal min As Integer, ByVal max As Integer) As Integer
        Return Math.Round((max - min) * Rnd.NextDouble) + min
    End Function

#End Region

#Region "Review 完成 | 音乐"

    ''' <summary>
    ''' 播放音乐。
    ''' </summary>
    ''' <param name="FilePath">音乐文件完整路径。</param>
    ''' <remarks></remarks>
    Public Sub PlayBgm(ByVal FilePath As String, ByVal IsStop As Boolean, Optional ByVal IsShowHint As Boolean = False)
        Try
            If IsStop Then
                '停止
                log("[System] 停止播放音乐：" & FilePath)
                mciSendString("close " & FilePath, 0, 0, 0)
                PlaySound(Nothing, Nothing, Nothing)
            Else
                '播放
                log("[System] 播放音乐：" & FilePath)
                mciSendString("open " & FilePath, "", 0, 0)
                Dim code = mciSendString("play " & FilePath & " repeat", "", 0, 0)
                If code = 259 Then
                    PlaySound(FilePath, 0, 131072 Or 1 Or 8) '换用 PlaySound 播放不支持循环的 .wav
                    'If IsShowHint Then ShowHint("该音乐不支持循环播放，音乐将在播放完一次后停止。")
                    'code = mciSendString("play " & FilePath, "", 0, 0)
                ElseIf code = 277 Then
                    If IsShowHint Then MyMsgbox("无效的音乐文件。" & vbCrLf & "PCL 不支持部分 MP3 的播放，请尝试更换文件。目前已知网易云下载的 MP3 会出现此问题，原因不明。", "播放音乐失败")
                ElseIf Not code = 0 Then
                    Throw New Exception(code)
                End If
            End If
            'PlaySound(FilePath, 0, 131072 Or 1 Or 8)
        Catch ex As Exception
            ExShow(ex, "播放音乐失败", ErrorLevel.AllUsers)
        End Try
    End Sub

#End Region

End Module '基础函数

'Module ComputerInfo
'    '声明
'    'Private cpuCounter As New PerformanceCounter("Processor", "% Processor Time", "_Total")
'    'Private downloadCounter As New PerformanceCounter("Network Interface", "Bytes Received/sec", New PerformanceCounterCategory("Network Interface").GetInstanceNames()(0))
'    'Private uploadCounter As New PerformanceCounter("Network Interface", "Bytes Sent/sec", New PerformanceCounterCategory("Network Interface").GetInstanceNames()(0))
'    Private pcInfo As New Microsoft.VisualBasic.Devices.ComputerInfo
'    '启动代码
'    Public Sub InfoStartRun(ByVal time As Integer)
'        Dim timer As New DispatcherTimer()
'        timer.Interval = TimeSpan.FromMilliseconds(time)
'        AddHandler timer.Tick, AddressOf InfoTimer
'        timer.Start()
'    End Sub
'    '每次执行的代码
'    Private Sub InfoTimer()
'        '防止断网时获取错误
'        On Error Resume Next
'        infoDownloadSpeed = 0
'        infoUploadSpeed = 0
'        '获取值
'        'infoCpuPercent = cpuCounter.NextValue
'        'infoDownloadSpeed = downloadCounter.NextValue / 1024
'        'infoUploadSpeed = uploadCounter.NextValue / 1024
'    End Sub
'    '获取信息
'    Public infoCpuPercent As Integer = 0 'CPU使用率
'    Public infoDownloadSpeed As Integer = 0 '下载速度(KB)
'    Public infoUploadSpeed As Integer = 0 '上传速度(KB)
'    Public Function infoNetworkSpeed() As Integer
'        Return infoDownloadSpeed + infoUploadSpeed
'    End Function '网络速度(KB)
'    Public Function infoRamPercent() As Integer
'        Return (pcInfo.TotalPhysicalMemory - pcInfo.AvailablePhysicalMemory) / pcInfo.TotalPhysicalMemory * 100
'    End Function '内存使用率
'    Public Function infoRamUsed() As Integer
'        Return (pcInfo.TotalPhysicalMemory - pcInfo.AvailablePhysicalMemory) / 1024 / 1024
'    End Function '内存使用量(MB)
'    Public Function infoRamTotal() As Integer
'        Return pcInfo.TotalPhysicalMemory / 1024 / 1024
'    End Function '内存总量(MB)
'    Public Function infoRamAvailable() As Integer
'        Return pcInfo.AvailablePhysicalMemory / 1024 / 1024
'    End Function '内存可用量(MB)
'End Module '电脑信息获取

Public Module MyAnimationer

#Region "声明"
    ''' <summary>
    ''' 动画组列表。（组名→ArrayList）
    ''' </summary>
    ''' <remarks></remarks>
    Private AniGroups As New Dictionary(Of String, ArrayList)
    ''' <summary>
    ''' 上一次记刻的时间。
    ''' </summary>
    ''' <remarks></remarks>
    Public AniLastTick As Integer
    ''' <summary>
    ''' 动画执行是否开启。
    ''' </summary>
    ''' <remarks></remarks>
    Public AniRunning As Boolean = False
#End Region

#Region "类与枚举"

    ''' <summary>
    ''' 单个动画对象。
    ''' </summary>
    ''' <remarks></remarks>
    Public Structure Animation

        ''' <summary>
        ''' 动画种类。
        ''' </summary>
        ''' <remarks></remarks>
        Public AniType As AniType
        ''' <summary>
        ''' 动画副种类。
        ''' </summary>
        ''' <remarks></remarks>
        Public AniSubType As AniSubType

        ''' <summary>
        ''' 动画总长度。
        ''' </summary>
        ''' <remarks></remarks>
        Public TotalLength As Integer
        ''' <summary>
        ''' 已经执行的动画长度。如果为负数则为延迟。
        ''' </summary>
        ''' <remarks></remarks>
        Public FinishLength As Integer
        ''' <summary>
        ''' 已经完成的百分比。
        ''' </summary>
        ''' <remarks></remarks>
        Public Percent As Double
        ''' <summary>
        ''' 是否为“以后”。
        ''' </summary>
        ''' <remarks></remarks>
        Public After As Boolean

        ''' <summary>
        ''' 插值器类型。
        ''' </summary>
        ''' <remarks></remarks>
        Public Ease As AniEase
        ''' <summary>
        ''' 动画对象。
        ''' </summary>
        ''' <remarks></remarks>
        Public Obj As Object
        ''' <summary>
        ''' 动画值。
        ''' </summary>
        ''' <remarks></remarks>
        Public Value As Object
        ''' <summary>
        ''' 上次执行时的动画值。
        ''' </summary>
        ''' <remarks></remarks>
        Public Last As Object

        Public Overrides Function ToString() As String
            Return GetStringFromEnum(AniType) & " | " & FinishLength & "/" & TotalLength & "(" & Math.Round(Percent * 100) & "%) | " & Obj.name & "(" & Obj.GetType.Name & ")"
        End Function

    End Structure
    ''' <summary>
    ''' 动画基础种类。
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum AniType As Byte
        ''' <summary>
        ''' 单个Double的动画，包括位置、长宽、透明度等。这需要附属类型。
        ''' </summary>
        ''' <remarks></remarks>
        DoubleAnimation = 0
        ''' <summary>
        ''' 颜色属性的动画。这需要附属类型。
        ''' </summary>
        ''' <remarks></remarks>
        ColorAnimation = 1
        ''' <summary>
        ''' 缩放控件大小。比起4个DoubleAnimation来说效率更高。
        ''' </summary>
        ''' <remarks></remarks>
        Scale = 2
        ''' <summary>
        ''' 文字一个个出现。
        ''' </summary>
        ''' <remarks></remarks>
        TextAppear = 3
        ''' <summary>
        ''' 执行代码。
        ''' </summary>
        ''' <remarks></remarks>
        Code = 4
        ''' <summary>
        ''' 执行按照规定格式书写的伪代码。比起Code来说这有很大的局限性，但效率更高。
        ''' </summary>
        ''' <remarks></remarks>
        CodeEvent = 5
        ''' <summary>
        ''' 以 WPF 方式缩放控件。
        ''' </summary>
        ScaleTransform = 6
        ''' <summary>
        ''' 以 WPF 方式旋转控件。
        ''' </summary>
        RotateTransform = 7
    End Enum
    ''' <summary>
    ''' 动画扩展种类。
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum AniSubType As Byte

        X = 0
        Y = 1
        Width = 2
        Height = 3
        Opacity = 4
        Value = 5
        Radius = 10
        BorderThickness = 11

        Background = 6
        BorderBrush = 7
        Foreground = 8
        Stroke = 9

    End Enum

#End Region

#Region "种类"

    'DoubleAnimation

    ''' <summary>
    ''' 移动X轴的动画。
    ''' </summary>
    ''' <param name="Obj">动画的对象。</param>
    ''' <param name="Value">进行移动的值。</param>
    ''' <param name="Time">动画长度（毫秒）。</param>
    ''' <param name="Delay">动画延迟执行的时间（毫秒）。</param>
    ''' <param name="Ease">插值器类型。</param>
    ''' <param name="After">是否等到以前的动画完成后才继续本动画。</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function AaX(ByVal Obj As Object, ByVal Value As Double, Optional ByVal Time As Integer = 400, Optional ByVal Delay As Integer = 0, Optional ByVal Ease As AniEase = Nothing, Optional ByVal After As Boolean = False) As Animation
        Return New Animation With {.AniType = AniType.DoubleAnimation, .AniSubType = AniSubType.X,
                                   .TotalLength = Time, .Ease = If(Ease, New AniEaseNone), .Obj = Obj, .Value = Value, .After = After, .FinishLength = -Delay}
    End Function
    ''' <summary>
    ''' 移动Y轴的动画。
    ''' </summary>
    ''' <param name="Obj">动画的对象。</param>
    ''' <param name="Value">进行移动的值。</param>
    ''' <param name="Time">动画长度（毫秒）。</param>
    ''' <param name="Delay">动画延迟执行的时间（毫秒）。</param>
    ''' <param name="Ease">插值器类型。</param>
    ''' <param name="After">是否等到以前的动画完成后才继续本动画。</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function AaY(ByVal Obj As Object, ByVal Value As Double, Optional ByVal Time As Integer = 400, Optional ByVal Delay As Integer = 0, Optional ByVal Ease As AniEase = Nothing, Optional ByVal After As Boolean = False) As Animation
        Return New Animation With {.AniType = AniType.DoubleAnimation, .AniSubType = AniSubType.Y,
                                   .TotalLength = Time, .Ease = If(Ease, New AniEaseNone), .Obj = Obj, .Value = Value, .After = After, .FinishLength = -Delay}
    End Function
    ''' <summary>
    ''' 改变宽度的动画。
    ''' </summary>
    ''' <param name="Obj">动画的对象。</param>
    ''' <param name="Value">宽度改变的值。</param>
    ''' <param name="Time">动画长度（毫秒）。</param>
    ''' <param name="Delay">动画延迟执行的时间（毫秒）。</param>
    ''' <param name="Ease">插值器类型。</param>
    ''' <param name="After">是否等到以前的动画完成后才继续本动画。</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function AaWidth(ByVal Obj As Object, ByVal Value As Double, Optional ByVal Time As Integer = 400, Optional ByVal Delay As Integer = 0, Optional ByVal Ease As AniEase = Nothing, Optional ByVal After As Boolean = False) As Animation
        Return New Animation With {.AniType = AniType.DoubleAnimation, .AniSubType = AniSubType.Width,
                                   .TotalLength = Time, .Ease = If(Ease, New AniEaseNone), .Obj = Obj, .Value = Value, .After = After, .FinishLength = -Delay}
    End Function
    ''' <summary>
    ''' 改变高度的动画。
    ''' </summary>
    ''' <param name="Obj">动画的对象。</param>
    ''' <param name="Value">高度改变的值。</param>
    ''' <param name="Time">动画长度（毫秒）。</param>
    ''' <param name="Delay">动画延迟执行的时间（毫秒）。</param>
    ''' <param name="Ease">插值器类型。</param>
    ''' <param name="After">是否等到以前的动画完成后才继续本动画。</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function AaHeight(ByVal Obj As Object, ByVal Value As Double, Optional ByVal Time As Integer = 400, Optional ByVal Delay As Integer = 0, Optional ByVal Ease As AniEase = Nothing, Optional ByVal After As Boolean = False) As Animation
        Return New Animation With {.AniType = AniType.DoubleAnimation, .AniSubType = AniSubType.Height,
                                   .TotalLength = Time, .Ease = If(Ease, New AniEaseNone), .Obj = Obj, .Value = Value, .After = After, .FinishLength = -Delay}
    End Function
    ''' <summary>
    ''' 改变透明度的动画。
    ''' </summary>
    ''' <param name="Obj">动画的对象。</param>
    ''' <param name="Value">透明度改变的值。</param>
    ''' <param name="Time">动画长度（毫秒）。</param>
    ''' <param name="Delay">动画延迟执行的时间（毫秒）。</param>
    ''' <param name="Ease">插值器类型。</param>
    ''' <param name="After">是否等到以前的动画完成后才继续本动画。</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function AaOpacity(ByVal Obj As Object, ByVal Value As Double, Optional ByVal Time As Integer = 400, Optional ByVal Delay As Integer = 0, Optional ByVal Ease As AniEase = Nothing, Optional ByVal After As Boolean = False) As Animation
        Return New Animation With {.AniType = AniType.DoubleAnimation, .AniSubType = AniSubType.Opacity,
                                   .TotalLength = Time, .Ease = If(Ease, New AniEaseNone), .Obj = Obj, .Value = Value, .After = After, .FinishLength = -Delay}
    End Function
    ''' <summary>
    ''' 改变对象的Value属性的动画。
    ''' </summary>
    ''' <param name="Obj">动画的对象。</param>
    ''' <param name="Value">Value属性改变的值。</param>
    ''' <param name="Time">动画长度（毫秒）。</param>
    ''' <param name="Delay">动画延迟执行的时间（毫秒）。</param>
    ''' <param name="Ease">插值器类型。</param>
    ''' <param name="After">是否等到以前的动画完成后才继续本动画。</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function AaValue(ByVal Obj As Object, ByVal Value As Double, Optional ByVal Time As Integer = 400, Optional ByVal Delay As Integer = 0, Optional ByVal Ease As AniEase = Nothing, Optional ByVal After As Boolean = False) As Animation
        Return New Animation With {.AniType = AniType.DoubleAnimation, .AniSubType = AniSubType.Value,
                                   .TotalLength = Time, .Ease = If(Ease, New AniEaseNone), .Obj = Obj, .Value = Value, .After = After, .FinishLength = -Delay}
    End Function
    ''' <summary>
    ''' 改变对象的Radius属性的动画。
    ''' </summary>
    ''' <param name="Obj">动画的对象。</param>
    ''' <param name="Value">Radius属性改变的值。</param>
    ''' <param name="Time">动画长度（毫秒）。</param>
    ''' <param name="Delay">动画延迟执行的时间（毫秒）。</param>
    ''' <param name="Ease">插值器类型。</param>
    ''' <param name="After">是否等到以前的动画完成后才继续本动画。</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function AaRadius(ByVal Obj As Object, ByVal Value As Double, Optional ByVal Time As Integer = 400, Optional ByVal Delay As Integer = 0, Optional ByVal Ease As AniEase = Nothing, Optional ByVal After As Boolean = False) As Animation
        Return New Animation With {.AniType = AniType.DoubleAnimation, .AniSubType = AniSubType.Radius,
                                   .TotalLength = Time, .Ease = If(Ease, New AniEaseNone), .Obj = Obj, .Value = Value, .After = After, .FinishLength = -Delay}
    End Function
    ''' <summary>
    ''' 改变对象的BorderThickness属性的动画。
    ''' </summary>
    ''' <param name="Obj">动画的对象。</param>
    ''' <param name="Value">BorderThickness属性改变的值。</param>
    ''' <param name="Time">动画长度（毫秒）。</param>
    ''' <param name="Delay">动画延迟执行的时间（毫秒）。</param>
    ''' <param name="Ease">插值器类型。</param>
    ''' <param name="After">是否等到以前的动画完成后才继续本动画。</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function AaBorderThickness(ByVal Obj As Object, ByVal Value As Double, Optional ByVal Time As Integer = 400, Optional ByVal Delay As Integer = 0, Optional ByVal Ease As AniEase = Nothing, Optional ByVal After As Boolean = False) As Animation
        Return New Animation With {.AniType = AniType.DoubleAnimation, .AniSubType = AniSubType.BorderThickness,
                                   .TotalLength = Time, .Ease = If(Ease, New AniEaseNone), .Obj = Obj, .Value = Value, .After = After, .FinishLength = -Delay}
    End Function

    'ColorAnimation

    ''' <summary>
    ''' 改变Background颜色属性的动画。
    ''' </summary>
    ''' <param name="Obj">动画的对象。</param>
    ''' <param name="Value">颜色改变的值。以RGB加减法进行计算。不用担心超额。</param>
    ''' <param name="Time">动画长度（毫秒）。</param>
    ''' <param name="Delay">动画延迟执行的时间（毫秒）。</param>
    ''' <param name="Ease">插值器类型。</param>
    ''' <param name="After">是否等到以前的动画完成后才继续本动画。</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function AaBackGround(ByVal Obj As Object, ByVal Value As MyColor, Optional ByVal Time As Integer = 400, Optional ByVal Delay As Integer = 0, Optional ByVal Ease As AniEase = Nothing, Optional ByVal After As Boolean = False) As Animation
        Return New Animation With {.AniType = AniType.ColorAnimation, .AniSubType = AniSubType.Background,
                                   .TotalLength = Time, .Ease = If(Ease, New AniEaseNone), .Obj = Obj, .Value = Value, .After = After, .FinishLength = -Delay, .Last = New MyColor}
    End Function
    ''' <summary>
    ''' 改变BorderBrush颜色属性的动画。
    ''' </summary>
    ''' <param name="Obj">动画的对象。</param>
    ''' <param name="Value">颜色改变的值。以RGB加减法进行计算。不用担心超额。</param>
    ''' <param name="Time">动画长度（毫秒）。</param>
    ''' <param name="Delay">动画延迟执行的时间（毫秒）。</param>
    ''' <param name="Ease">插值器类型。</param>
    ''' <param name="After">是否等到以前的动画完成后才继续本动画。</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function AaBorderBrush(ByVal Obj As Object, ByVal Value As MyColor, Optional ByVal Time As Integer = 400, Optional ByVal Delay As Integer = 0, Optional ByVal Ease As AniEase = Nothing, Optional ByVal After As Boolean = False) As Animation
        Return New Animation With {.AniType = AniType.ColorAnimation, .AniSubType = AniSubType.BorderBrush,
                                   .TotalLength = Time, .Ease = If(Ease, New AniEaseNone), .Obj = Obj, .Value = Value, .After = After, .FinishLength = -Delay, .Last = New MyColor}
    End Function
    ''' <summary>
    ''' 改变Foreground颜色属性的动画。
    ''' </summary>
    ''' <param name="Obj">动画的对象。</param>
    ''' <param name="Value">颜色改变的值。以RGB加减法进行计算。不用担心超额。</param>
    ''' <param name="Time">动画长度（毫秒）。</param>
    ''' <param name="Delay">动画延迟执行的时间（毫秒）。</param>
    ''' <param name="Ease">插值器类型。</param>
    ''' <param name="After">是否等到以前的动画完成后才继续本动画。</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function AaForeGround(ByVal Obj As Object, ByVal Value As MyColor, Optional ByVal Time As Integer = 400, Optional ByVal Delay As Integer = 0, Optional ByVal Ease As AniEase = Nothing, Optional ByVal After As Boolean = False) As Animation
        Return New Animation With {.AniType = AniType.ColorAnimation, .AniSubType = AniSubType.Foreground,
                                   .TotalLength = Time, .Ease = If(Ease, New AniEaseNone), .Obj = Obj, .Value = Value, .After = After, .FinishLength = -Delay, .Last = New MyColor}
    End Function
    ''' <summary>
    ''' 改变Stroke颜色属性的动画。
    ''' </summary>
    ''' <param name="Obj">动画的对象。</param>
    ''' <param name="Value">颜色改变的值。以RGB加减法进行计算。不用担心超额。</param>
    ''' <param name="Time">动画长度（毫秒）。</param>
    ''' <param name="Delay">动画延迟执行的时间（毫秒）。</param>
    ''' <param name="Ease">插值器类型。</param>
    ''' <param name="After">是否等到以前的动画完成后才继续本动画。</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function AaStroke(ByVal Obj As Object, ByVal Value As MyColor, Optional ByVal Time As Integer = 400, Optional ByVal Delay As Integer = 0, Optional ByVal Ease As AniEase = Nothing, Optional ByVal After As Boolean = False) As Animation
        Return New Animation With {.AniType = AniType.ColorAnimation, .AniSubType = AniSubType.Stroke,
                                   .TotalLength = Time, .Ease = If(Ease, New AniEaseNone), .Obj = Obj, .Value = Value, .After = After, .FinishLength = -Delay, .Last = New MyColor}
    End Function

    'Scale

    ''' <summary>
    ''' 缩放控件的动画。
    ''' </summary>
    ''' <param name="Obj">动画的对象。</param>
    ''' <param name="Value">大小改变的百分比（如-0.6）或值。</param>
    ''' <param name="Time">动画长度（毫秒）。</param>
    ''' <param name="Delay">动画延迟执行的时间（毫秒）。</param>
    ''' <param name="Ease">插值器类型。</param>
    ''' <param name="After">是否等到以前的动画完成后才继续本动画。</param>
    ''' <param name="Absolute">大小改变是否为绝对值。若为False则会按照控件当前大小计算相对缩放值。</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function AaScale(ByVal Obj As Object, ByVal Value As Double, Optional ByVal Time As Integer = 400, Optional ByVal Delay As Integer = 0, Optional ByVal Ease As AniEase = Nothing, Optional ByVal After As Boolean = False, Optional ByVal Absolute As Boolean = False) As Animation
        Dim ChangeRect As New AdvancedRect
        If Absolute Then
            ChangeRect = New AdvancedRect(-0.5 * Value, -0.5 * Value, Value, Value)
        Else
            ChangeRect = New AdvancedRect(-0.5 * Obj.ActualWidth * Value, -0.5 * Obj.ActualHeight * Value, Obj.ActualWidth * Value, Obj.ActualHeight * Value)
        End If
        Return New Animation With {.AniType = AniType.Scale, .TotalLength = Time, .Ease = If(Ease, New AniEaseNone), .Obj = Obj, .Value = ChangeRect, .After = After, .FinishLength = -Delay}
    End Function

    'TextAppear

    ''' <summary>
    ''' 让一段文字一个个字出现或消失的动画。
    ''' </summary>
    ''' <param name="Obj">动画的对象。必须是Label或TextBlock。</param>
    ''' <param name="Hide">是否为一个个字隐藏。默认为False（一个个字出现）。这些字必须已经存在了。</param>
    ''' <param name="TimePerText">是否采用根据文本长度决定时间的方式。</param>
    ''' <param name="Time">动画长度（毫秒）。若TimePerText为True，这代表每个字所占据的时间。</param>
    ''' <param name="Delay">动画延迟执行的时间（毫秒）。</param>
    ''' <param name="After">是否等到以前的动画完成后才继续本动画。</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function AaTextAppear(ByVal Obj As Object, Optional ByVal Hide As Boolean = False, Optional ByVal TimePerText As Boolean = True, Optional ByVal Time As Integer = 70, Optional ByVal Delay As Integer = 0, Optional ByVal After As Boolean = False) As Animation
        'Are we cool yet？
        Return New Animation With {.AniType = AniType.TextAppear, .TotalLength = If(TimePerText, Time * Len(If(Obj.GetType.Name = "TextBlock", Obj.Text, Obj.Context.ToString)), Time), .Obj = Obj, .Value = {If(Obj.GetType.Name = "TextBlock", Obj.Text, Obj.Context.ToString), Hide}, .After = After, .FinishLength = -Delay}
    End Function

    'Code

    ''' <summary>
    ''' 执行代码。
    ''' </summary>
    ''' <param name="Code">一个ThreadStart。这将会在执行时在主线程调用。</param>
    ''' <param name="Delay">代码延迟执行的时间（毫秒）。</param>
    ''' <param name="After">是否等到以前的动画完成后才执行。</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function AaCode(ByVal Code As ThreadStart, Optional ByVal Delay As Integer = 0, Optional ByVal After As Boolean = False) As Animation
        Return New Animation With {.AniType = AniType.Code,
                                   .TotalLength = 1, .Value = Code, .After = After, .FinishLength = -Delay}
    End Function

    'CodeEvent

    ''' <summary>
    ''' 执行按照规定格式书写的伪代码。这有很大的局限性，但不会建立线程，性能也更好。
    ''' </summary>
    ''' <param name="Code">伪代码元素数组。</param>
    ''' <param name="Delay">代码延迟执行的时间（毫秒）。</param>
    ''' <param name="After">是否等到以前的动画完成后才执行。</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function AaCode(ByVal Code As Array, Optional ByVal Delay As Integer = 0, Optional ByVal After As Boolean = False) As Animation
        Return New Animation With {.AniType = AniType.CodeEvent,
                                   .TotalLength = 1, .Value = Code, .After = After, .FinishLength = -Delay}
    End Function

    'ScaleTransform

    ''' <summary>
    ''' 按照 WPF 方式缩放控件的动画。
    ''' </summary>
    ''' <param name="Obj">动画的对象。它必须已经拥有了单一的 ScaleTransform 值。</param>
    ''' <param name="Value">大小改变的百分比（如-0.6）。</param>
    ''' <param name="Time">动画长度（毫秒）。</param>
    ''' <param name="Delay">动画延迟执行的时间（毫秒）。</param>
    ''' <param name="Ease">插值器类型。</param>
    ''' <param name="After">是否等到以前的动画完成后才继续本动画。</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function AaScaleTransform(ByVal Obj As Object, ByVal Value As Double, Optional ByVal Time As Integer = 400, Optional ByVal Delay As Integer = 0, Optional ByVal Ease As AniEase = Nothing, Optional ByVal After As Boolean = False) As Animation
        Return New Animation With {.AniType = AniType.ScaleTransform, .TotalLength = Time, .Ease = If(Ease, New AniEaseNone), .Obj = Obj, .Value = Value, .After = After, .FinishLength = -Delay}
    End Function

    'RotateTransform

    ''' <summary>
    ''' 按照 WPF 方式旋转控件的动画。
    ''' </summary>
    ''' <param name="Obj">动画的对象。它必须已经拥有了单一的 ScaleTransform 值。</param>
    ''' <param name="Value">大小改变的百分比（如-0.6）。</param>
    ''' <param name="Time">动画长度（毫秒）。</param>
    ''' <param name="Delay">动画延迟执行的时间（毫秒）。</param>
    ''' <param name="Ease">插值器类型。</param>
    ''' <param name="After">是否等到以前的动画完成后才继续本动画。</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function AaRotateTransform(ByVal Obj As Object, ByVal Value As Double, Optional ByVal Time As Integer = 400, Optional ByVal Delay As Integer = 0, Optional ByVal Ease As AniEase = Nothing, Optional ByVal After As Boolean = False) As Animation
        Return New Animation With {.AniType = AniType.RotateTransform, .TotalLength = Time, .Ease = If(Ease, New AniEaseNone), .Obj = Obj, .Value = Value, .After = After, .FinishLength = -Delay}
    End Function

    '特殊

    ''' <summary>
    ''' 将一个StackPanel中的各个项目依次显示。
    ''' </summary>
    ''' <remarks></remarks>
    Public Function AaStack(ByVal Stack As StackPanel) As ArrayList
        AaStack = New ArrayList
        Dim AniDelay As Integer = 0
        For Each Item In Stack.Children
            Item.Opacity = 0
            AaStack.Add(AaOpacity(Item, 1, 100, AniDelay))
            AniDelay = AniDelay + 25
        Next
    End Function

#End Region

#Region "插值器"

    '基类
    Public MustInherit Class AniEase
        ''' <summary>
        ''' 获取增加值。
        ''' </summary>
        ''' <param name="t1">较大的时间百分比。</param>
        ''' <param name="t0">较小的时间百分比。</param>
        ''' <returns></returns>
        Public MustOverride Function GetDelta(t1 As Double, t0 As Double) As Double
    End Class

    ''' <summary>
    ''' 线性，无缓动。
    ''' </summary>
    Public Class AniEaseNone
        Inherits AniEase
        Public Overrides Function GetDelta(t1 As Double, t0 As Double) As Double
            t1 = MathRange(t1, 0, 1)
            t0 = MathRange(t0, 0, 1)
            Return t1 - t0
        End Function
    End Class

    ''' <summary>
    ''' 平滑开始。
    ''' </summary>
    Public Class AniEaseStart
        Inherits AniEase
        Public Overrides Function GetDelta(t1 As Double, t0 As Double) As Double
            t1 = MathRange(t1, 0, 1)
            t0 = MathRange(t0, 0, 1)
            Return (t1 - t0) * (t1 + t0)
        End Function
    End Class

    ''' <summary>
    ''' 平滑结束。
    ''' </summary>
    Public Class AniEaseEnd
        Inherits AniEase
        Public Overrides Function GetDelta(t1 As Double, t0 As Double) As Double
            t1 = MathRange(t1, 0, 1)
            t0 = MathRange(t0, 0, 1)
            Return (t1 - t0) * (2 - t1 - t0)
        End Function
    End Class

    ''' <summary>
    ''' 跳跃开始。
    ''' </summary>
    Public Class AniEaseJumpStart
        Inherits AniEase

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="JumpEndPercent">跳跃结束时占整个动画的百分比。</param>
        Public Sub New(JumpEndPercent As Double)
            o = JumpEndPercent
        End Sub
        Private o As Double
        Public Overrides Function GetDelta(t1 As Double, t0 As Double) As Double
            t1 = MathRange(t1, 0, 1)
            t0 = MathRange(t0, 0, 1)
            Return (t1 - t0) * (t1 + t0 - o) / (1 - o)
        End Function
    End Class

    ''' <summary>
    ''' 跳跃结束。
    ''' </summary>
    Public Class AniEaseJumpEnd
        Inherits AniEase

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="JumpStartPercent">跳跃开始时占整个动画的百分比。</param>
        Public Sub New(JumpStartPercent As Double)
            o = JumpStartPercent
        End Sub
        Private o As Double
        Public Overrides Function GetDelta(t1 As Double, t0 As Double) As Double
            t1 = MathRange(t1, 0, 1)
            t0 = MathRange(t0, 0, 1)
            Return (t1 - t0) * (o + 1 - t1 - t0) / o
        End Function
    End Class

#End Region

    '等待处理的动画组列表
    Private AniWaitingList As New ArrayList
    Private AniWaitingListName As New ArrayList

    ''' <summary>
    ''' 开始一个动画组。
    ''' </summary>
    ''' <param name="AniGroup">由Aa开头的函数初始化的Animation对象集合。</param>
    ''' <param name="Name">动画组的名称。如果重复会直接停止同名动画组。</param>
    ''' <param name="RefreshTime">是否重新开始这一帧的计时。如果该动画组连续执行请设置为False。</param>
    ''' <remarks></remarks>
    Public Sub AniStart(ByVal AniGroup As ArrayList, ByVal Name As String, Optional ByVal refreshTime As Boolean = True)

        '把组动画（如 AniStack）分解
        Dim AniGroupSet As New ArrayList
        For i = 0 To AniGroup.Count - 1
            If AniGroup(i).GetType.ToString.Contains("ArrayList") Then
                AniGroupSet.AddRange(AniGroup(i))
            Else
                AniGroupSet.Add(AniGroup(i))
            End If
        Next i

        '添加到正在执行的动画组
        If refreshTime Then AniLastTick = Computer.Clock.TickCount '避免处理动画时已经造成了极大的延迟，导致动画突然结束
        AniStop(Name)
        AniGroups.Add(Name, AniGroupSet)

    End Sub
    ''' <summary>
    ''' 开始一个动画组。
    ''' </summary>
    ''' <param name="AniGroup">由Aa开头的函数初始化的Animation对象集合。</param>
    ''' <param name="Name">动画组的名称。如果重复会直接停止同名动画组。</param>
    ''' <param name="RefreshTime">是否重新开始这一帧的计时。如果该动画组连续执行请设置为False。</param>
    ''' <remarks></remarks>
    Public Sub AniStart(ByVal aniGroup As Array, ByVal name As String, Optional ByVal refreshTime As Boolean = True)
        '让Array和ArrayList都可以添加
        AniStart(New ArrayList(aniGroup), name, refreshTime)
    End Sub
    ''' <summary>
    ''' 停止一个动画组。
    ''' </summary>
    ''' <param name="name">需要停止的动画组的名称。</param>
    ''' <remarks></remarks>
    Public Sub AniStop(ByVal Name As String)
        If AniGroups.ContainsKey(Name) Then AniGroups.Remove(Name)
    End Sub

    Private AniMissions As New ArrayList

    Private AniFPSCounter As Integer = 0
    Private AniFPSTimer As Long = 0
    ''' <summary>
    ''' 当前的动画 FPS。
    ''' </summary>
    Public AniFPS As Integer = 0

    ''' <summary>
    ''' 开始动画执行。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub AniStartRun()
        '初始化计时器
        AniLastTick = My.Computer.Clock.TickCount
        AniFPSTimer = My.Computer.Clock.TickCount
        AniRunning = True '标记动画执行开始

        Dim th = New Thread(Sub()
                                Do While True
                                    '两帧之间的间隔时间
                                    Dim DeltaTime As Integer = My.Computer.Clock.TickCount - AniLastTick
                                    If DeltaTime < 2 Then GoTo Sleeper
                                    AniLastTick = My.Computer.Clock.TickCount
                                    '执行动画
                                    frmMain.Dispatcher.Invoke(Sub() AniTimer(DeltaTime))
                                    '记录 FPS
                                    If AniLastTick - AniFPSTimer >= 1000 Then
                                        AniFPS = AniFPSCounter
                                        AniFPSCounter = 0
                                        AniFPSTimer = AniLastTick
                                        If MODE_DEVELOPER Then frmMain.Dispatcher.Invoke(Sub() frmMain.Title = "PCL / FPS : " & AniFPS)
                                    End If
                                    AniFPSCounter = AniFPSCounter + 1
Sleeper:
                                    '控制 FPS
                                    Thread.Sleep(1)
                                Loop
                            End Sub)
        th.Start()
    End Sub

    Private AniCodeList As New ArrayList
    ''' <summary>
    ''' 动画定时器事件。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub AniTimer(DeltaTime As Integer)
        Rnd.NextDouble() '重置随机数发生器

        For Each Code In AniCodeList
            frmMain.Dispatcher.Invoke(Code)
        Next
        AniCodeList.Clear()

        Try

            Dim i As Integer = 0
            '循环每个动画组
            Do While i < AniGroups.Count
                '初始化
                Dim aniGroup = AniGroups.Values(i)
                Dim CanRemoveAfter = True '是否应该去除“之后”标记
                Dim ii = 0

                '循环每个动画
                Do While ii < aniGroup.Count
                    Dim Anim As Animation = aniGroup(ii)
                    '执行种类
                    If Anim.After = False Then '之前
                        CanRemoveAfter = False '取消“之后”标记 
                        '增加执行时间
                        Anim.FinishLength = Anim.FinishLength + DeltaTime
                        '执行动画
                        If Anim.FinishLength > 0 Then Anim = AniRun(Anim) '执行动画
                        '如果当前动画已执行完毕
                        If Anim.FinishLength >= Anim.TotalLength Then
                            '删除
                            aniGroup.RemoveAt(ii)
                            GoTo NextAni
                        End If
                        aniGroup(ii) = Anim
                    Else '之后
                        If CanRemoveAfter Then
                            '之后改为之前
                            CanRemoveAfter = False
                            Anim.After = False
                            aniGroup(ii) = Anim
                            '重新循环该动画
                            GoTo NextAni
                        Else
                            '不能去除该“之后”标记，结束该动画组
                            Exit Do
                        End If
                    End If
                    ii = ii + 1
NextAni:
                Loop

                '如果当前动画组都执行完毕则删除
                If aniGroup.Count = 0 Then
                    AniGroups.Remove(AniGroups.Keys(i))
                    i = i - 1
                End If
                '继续循环
                i = i + 1
            Loop

        Catch ex As Exception
            ExShow(ex, "动画刻执行失败", ErrorLevel.Slient)
        End Try
    End Sub

    ''' <summary>
    ''' 执行一个动画。
    ''' </summary>
    ''' <param name="Ani">执行的动画对象。</param>
    ''' <remarks></remarks>
    Private Function AniRun(ByVal Ani As Animation) As Animation
        Try
            Select Case Ani.AniType

                Case AniType.DoubleAnimation
                    Dim Delta As Double = MathPercent(0, Ani.Value, Ani.Ease.GetDelta(Ani.FinishLength / Ani.TotalLength, Ani.Percent))
                    Select Case Ani.AniSubType
                        Case AniSubType.X
                            SetLeft(Ani.Obj, GetLeft(Ani.Obj) + Delta)
                        Case AniSubType.Y
                            SetTop(Ani.Obj, GetTop(Ani.Obj) + Delta)
                        Case AniSubType.Opacity
                            Ani.Obj.Opacity = MathRange(Ani.Obj.Opacity + Delta, 0, 1)
                        Case AniSubType.Width
                            Ani.Obj.Width = MathRange(Ani.Obj.Width + Delta, 0)
                        Case AniSubType.Height
                            Ani.Obj.Height = MathRange(Ani.Obj.Height + Delta, 0)
                        Case AniSubType.Value
                            Ani.Obj.Value = Ani.Obj.Value + Delta
                        Case AniSubType.Radius
                            Ani.Obj.Radius = Ani.Obj.Radius + Delta
                        Case AniSubType.BorderThickness
                            Ani.Obj.BorderThickness = New Thickness(CType(Ani.Obj.BorderThickness, Thickness).Bottom + Delta)
                    End Select

                Case AniType.ColorAnimation
                    '利用Last记录了余下的小数值
                    Dim Delta As MyColor = MathPercent(New MyColor, Ani.Value, Ani.Ease.GetDelta(Ani.FinishLength / Ani.TotalLength, Ani.Percent)) + Ani.Last
                    Select Case Ani.AniSubType
                        Case AniSubType.Background
                            Dim NewColor As MyColor = New MyColor(Ani.Obj.Background.Color) + Delta
                            Ani.Obj.Background = NewColor
                            Ani.Last = NewColor - New MyColor(Ani.Obj.Background.Color)
                        Case AniSubType.Foreground
                            Dim NewColor As MyColor = New MyColor(Ani.Obj.Foreground.Color) + Delta
                            Ani.Obj.Foreground = NewColor
                            Ani.Last = NewColor - New MyColor(Ani.Obj.Foreground.Color)
                        Case AniSubType.Stroke
                            Dim NewColor As MyColor = New MyColor(Ani.Obj.Stroke.Color) + Delta
                            Ani.Obj.Stroke = NewColor
                            Ani.Last = NewColor - New MyColor(Ani.Obj.Stroke.Color)
                        Case AniSubType.BorderBrush
                            Dim NewColor As MyColor = New MyColor(Ani.Obj.BorderBrush.Color) + Delta
                            Ani.Obj.BorderBrush = NewColor
                            Ani.Last = NewColor - New MyColor(Ani.Obj.BorderBrush.Color)
                    End Select

                Case AniType.Scale
                    Dim Delta As Double = Ani.Ease.GetDelta(Ani.FinishLength / Ani.TotalLength, Ani.Percent)
                    Ani.Obj.Margin = New Thickness(Ani.Obj.Margin.Left + MathPercent(0, Ani.Value.Left, Delta), Ani.Obj.Margin.Top + MathPercent(0, Ani.Value.Top, Delta), Ani.Obj.Margin.Right, Ani.Obj.Margin.Bottom)
                    Ani.Obj.Width = Math.Max(Ani.Obj.Width + MathPercent(0, Ani.Value.Width, Delta), 0)
                    Ani.Obj.Height = Math.Max(Ani.Obj.Height + MathPercent(0, Ani.Value.Height, Delta), 0)

                Case AniType.TextAppear
                    Dim NewText As String = Mid(Ani.Value(0), 1, If(Ani.Value(1), Len(Ani.Value(0)), 0) + Math.Round(Len(Ani.Value(0)) * If(Ani.Value(1), -1, 1) * Ani.Ease.GetDelta(Ani.FinishLength / Ani.TotalLength, 0)))
                    If Ani.Obj.GetType.Name = "TextBlock" Then
                        Ani.Obj.Text = NewText
                    Else
                        Ani.Obj.Context = NewText
                    End If

                Case AniType.Code
                    AniCodeList.Add(Ani.Value)

                Case AniType.CodeEvent
                    Select Case Ani.Value(0)
                        Case "Close"
                            Ani.Value(1).Close()
                        Case "Clear"
                            Ani.Value(1).Clear()
                        Case "Remove"
                            Ani.Value(1).Remove(Ani.Value(2))
                        Case "Add"
                            Ani.Value(1).Add(Ani.Value(2))
                        Case "Set"
                            Ani.Value(1) = Ani.Value(2)
                        Case "Tag"
                            Ani.Value(1).Tag = Ani.Value(2)
                        Case "Enabled"
                            Ani.Value(1).IsEnabled = Ani.Value(2)
                        Case "State"
                            Ani.Value(1).State = Ani.Value(2)
                        Case "IsHitTestVisible"
                            Ani.Value(1).IsHitTestVisible = Ani.Value(2)
                        Case "ShowInTaskbar"
                            Ani.Value(1).ShowInTaskbar = Ani.Value(2)
                        Case "Nothing"
                            Ani.Value(1) = Nothing
                        Case "Visible"
                            Ani.Value(1).Visibility = If(Ani.Value(2), Visibility.Visible, Visibility.Collapsed)
                        Case "End"
                            EndForce()
                        Case Else
                            AaCodeCall(Ani.Value(0), Ani.Value)
                    End Select

                Case AniType.ScaleTransform
                    Dim Delta As Double = MathPercent(0, Ani.Value, Ani.Ease.GetDelta(Ani.FinishLength / Ani.TotalLength, Ani.Percent))
                    CType(Ani.Obj.RenderTransform, ScaleTransform).ScaleX = CType(Ani.Obj.RenderTransform, ScaleTransform).ScaleX + Delta
                    CType(Ani.Obj.RenderTransform, ScaleTransform).ScaleY = CType(Ani.Obj.RenderTransform, ScaleTransform).ScaleY + Delta

                Case AniType.RotateTransform
                    Dim Delta As Double = MathPercent(0, Ani.Value, Ani.Ease.GetDelta(Ani.FinishLength / Ani.TotalLength, Ani.Percent))
                    CType(Ani.Obj.RenderTransform, RotateTransform).Angle = CType(Ani.Obj.RenderTransform, RotateTransform).Angle + Delta

            End Select
            Ani.Percent = Ani.FinishLength / Ani.TotalLength '修改执行百分比
        Catch ex As Exception
            ExShow(ex, "执行动画失败：" & GetStringFromEnum(Ani.AniType), ErrorLevel.Slient)
        End Try

        Return Ani
    End Function

End Module '动画

Public Module Web

    ''' <summary>
    ''' 正在下载的列表。
    ''' </summary>
    ''' <remarks></remarks>
    Public WebGroups As New Dictionary(Of String, WebGroup)

    Private WebDownloadingList As New ArrayList
    Public WebDownloadCountMax As Integer = ReadIni("setup", "DownMaxinum", "20")

#Region "类与枚举"

    ''' <summary>
    ''' 下载文件的文件大小处理方式。
    ''' </summary>
    Public Enum WebRequireSize As Integer
        ''' <summary>
        ''' 在下载前已经知道了文件大小。
        ''' </summary>
        Known = 0
        ''' <summary>
        ''' 下载前不知道文件大小，需要从网络中获取。
        ''' </summary>
        Require = 1
        ''' <summary>
        ''' 下载前不知道文件大小，也不获取大小。
        ''' </summary>
        DontNeed = 2
        ''' <summary>
        ''' 不需要知道准确大小，只要求文件至少有一定的大小。
        ''' </summary>
        AtLeast = 3
    End Enum

    Public Class WebGroup
        Public Name As String
        ''' <summary>
        ''' 文件列表（WebFile）。
        ''' </summary>
        ''' <remarks></remarks>
        Public Files As ArrayList
        ''' <summary>
        ''' 是否尝试从服务器确认文件大小。
        ''' </summary>
        ''' <remarks></remarks>
        Public GetServerFileSize As WebRequireSize
        ''' <summary>
        ''' 下载成功时执行的线程。
        ''' </summary>
        ''' <remarks></remarks>
        Public OnSuccess As System.Threading.ParameterizedThreadStart
        ''' <summary>
        ''' 下载失败时执行的线程。
        ''' </summary>
        ''' <remarks></remarks>
        Public OnFail As System.Threading.ParameterizedThreadStart

        ''' <summary>
        ''' 已下载的文件数目（下载成功或失败的）。（用于文件数较多情况下的快速进度计算）
        ''' </summary>
        Public FinishedCount As Integer = 0
        ''' <summary>
        ''' 下载完成的文件数目的百分比。取值为0到1。
        ''' </summary>
        ''' <remarks></remarks>
        Public ReadOnly Property Percent As Double
            Get
                Select Case Files.Count
                    Case 0
                        Percent = 0
                    Case 1
                        Percent = Math.Round(CType(Files(0), WebFile).Percent, 4)
                    Case Is < 60
                        Dim TotalPercent As Double = 0
                        For i = 0 To Files.Count - 1
                            TotalPercent = TotalPercent + CType(Files(i), WebFile).Percent
                        Next
                        Percent = Math.Round(TotalPercent / Files.Count, 4)
                    Case Else
                        Percent = Math.Round(FinishedCount / Files.Count, 4)
                End Select
            End Get
        End Property
        ''' <summary>
        ''' 该文件组的唯一标识符。
        ''' </summary>
        ''' <remarks></remarks>
        Public UUID As Integer = GetUUID()
    End Class

    Public Class WebFile

        ''' <summary>
        ''' 文件所有可能的网络URL。
        ''' </summary>
        ''' <remarks></remarks>
        Public AllWebAddress As ArrayList
        ''' <summary>
        ''' 文件当前的网络URL。
        ''' </summary>
        ''' <remarks></remarks>
        Public ReadOnly Property WebAddress As String
            Get
                Return If(AllWebAddress(0), "")
            End Get
        End Property

        ''' <summary>
        ''' 文件在本地的完整地址。
        ''' </summary>
        ''' <remarks></remarks>
        Public ReadOnly Property LocalAddress As String
            Get
                Return LocalFolder & LocalName
            End Get
        End Property
        ''' <summary>
        ''' 文件在本地的所在的文件夹，以“\”结尾。
        ''' </summary>
        ''' <remarks></remarks>
        Public LocalFolder As String

        ''' <summary>
        ''' 是否需要刷新文件名。
        ''' </summary>
        ''' <remarks></remarks>
        Public RefreshName As Boolean = False
        Private _LocalName As String = ""
        ''' <summary>
        ''' 当前的文件名。如果需要刷新文件名，它有可能改变。
        ''' </summary>
        ''' <remarks></remarks>
        Public Property LocalName As String
            Get
                If _LocalName = "" Or IsNothing(_LocalName) Then _LocalName = GetFileNameFromPath(WebAddress)
                Return _LocalName
            End Get
            Set(ByVal value As String)
                _LocalName = value
            End Set
        End Property

        ''' <summary>
        ''' 使用下一个下载地址。
        ''' </summary>
        ''' <remarks></remarks>
        Public Function NextAddress() As Boolean
            If AllWebAddress.Count > 1 Then
                AllWebAddress.RemoveAt(0)
                If RefreshName Then LocalName = GetFileNameFromPath(WebAddress)
                Return True
            Else
                Return False
            End If
        End Function

        Private _State As WebDownloadStats = WebDownloadStats.FirstLoad
        ''' <summary>
        ''' 状态标记。
        ''' </summary>
        ''' <remarks></remarks>
        Public Property State As WebDownloadStats
            Get
                Return _State
            End Get
            Set(ByVal value As WebDownloadStats)
                If value = _State Then Exit Property

                '改变状态
                _State = value

                Select Case value
                    Case WebDownloadStats.FirstLoad
                        '等待下载
                        GetSize = 0
                        Speed = 0
                        Percent = 0
                        SyncLock WebWaitingLockObj
                            WebWaitingList.Add(Me)
                        End SyncLock
                    Case WebDownloadStats.Wait
                        '等待下载
                        frmDownloadRight.RefreshFile(Me)
                        GetSize = 0
                        Speed = 0
                        Percent = 0
                        SyncLock WebWaitingLockObj
                            WebWaitingList.Insert(0, Me)
                        End SyncLock
                        WebWaitingToRun()
                    Case WebDownloadStats.GetSize
                        '获取大小
                        frmDownloadRight.RefreshFile(Me)
                        WebStateGetSize(Me)
                    Case WebDownloadStats.Download
                        '下载
                        frmDownloadRight.RefreshFile(Me)
                        WebStateDownload(Me)
                    Case WebDownloadStats.Success
                        '下载成功
                        Speed = 0
                        Percent = 1
                        WebStateSuccess(Me)
                        WebGroups(GroupName).FinishedCount = WebGroups(GroupName).FinishedCount + 1
                        frmDownloadRight.RefreshFile(Me)
                        WebFinishCheck(Me.GroupName)
                    Case WebDownloadStats.Fail
                        '下载失败
                        GetSize = 0
                        Speed = 0
                        Percent = 0
                        If Not WebStateFail(Me) Then
                            WebGroups(GroupName).FinishedCount = WebGroups(GroupName).FinishedCount + 1
                            frmDownloadRight.RefreshFile(Me)
                            WebFinishCheck(Me.GroupName)
                        End If
                    Case WebDownloadStats.Retry
                        '重试
                        _State = value
                        frmDownloadRight.RefreshFile(Me)
                        Thread.Sleep(If(WebGroups(GroupName).Files.Count = 1, 200, 1000))
#Disable Warning BC42026
                        State = WebDownloadStats.Wait
                End Select
            End Set
        End Property

        ''' <summary>
        ''' 该文件的唯一标识符。
        ''' </summary>
        ''' <remarks></remarks>
        Public UUID As Integer = GetUUID()

        Public GetServerFileSize As WebRequireSize
        Public GroupName As String
        Public RefreshThread As Thread

        ''' <summary>
        ''' 文件总大小，可能为 0。
        ''' </summary>
        ''' <remarks></remarks>
        Public TotalSize As FileSize = 0
        ''' <summary>
        ''' 已经下载了的部分的大小。
        ''' </summary>
        ''' <remarks></remarks>
        Public GetSize As FileSize = 0
        ''' <summary>
        ''' 下载速度，单位为秒。
        ''' </summary>
        ''' <remarks></remarks>
        Public Speed As FileSize = 0
        ''' <summary>
        ''' 已经下载的百分比。取值为 0 到 1。
        ''' </summary>
        ''' <remarks></remarks>
        Public Percent As Double = 0
        ''' <summary>
        ''' 计算下载速度的计刻。
        ''' </summary>
        ''' <remarks></remarks>
        Public SpeedCount As Integer

    End Class

    Public Enum WebDownloadStats As Byte
        ''' <summary>
        ''' 第一次加入列表。等同于 Wait，但是不会刷新队列。
        ''' </summary>
        FirstLoad = 6
        ''' <summary>
        ''' 等待中。若队列有空闲，需要获取大小的转为 GetSize，不需要获取大小的转为 Download。
        ''' </summary>
        ''' <remarks></remarks>
        Wait = 0
        ''' <summary>
        ''' 正在获取大小。获取大小成功的转为 Download，失败的转为 Fail。
        ''' </summary>
        ''' <remarks></remarks>
        GetSize = 1
        ''' <summary>
        ''' 正在下载。下载成功的转为 Success，失败的转为 Fail。
        ''' </summary>
        ''' <remarks></remarks>
        Download = 2
        ''' <summary>
        ''' 下载成功。
        ''' </summary>
        ''' <remarks></remarks>
        Success = 3
        ''' <summary>
        ''' 下载失败。可以重试的转为 Retry。
        ''' </summary>
        ''' <remarks></remarks>
        Fail = 4
        ''' <summary>
        ''' 等待重试。半秒后转为 Wait。
        ''' </summary>
        Retry = 5
    End Enum

#End Region

    '开始下载

    ''' <summary>
    ''' 需要下载的文件。
    ''' </summary>
    Public Class WebRequireFile
        ''' <summary>
        ''' 所有可能的下载地址。若前一个地址下载失败会自动尝试下一个地址。
        ''' </summary>
        Public WebURLs As ArrayList
        ''' <summary>
        ''' 本地文件夹地址。不包含文件名。
        ''' </summary>
        Public LocalFolder As String = ""
        ''' <summary>
        ''' 本地文件名，不包含文件夹路径。若为空，则会从下载地址自动生成文件名。
        ''' </summary>
        Public LocalName As String = ""
        ''' <summary>
        ''' 根据文件大小处理方式在这里提交准确大小或至少应有的大小。
        ''' </summary>
        Public KnownFileSize As Integer = 0
    End Class
    ''' <summary>
    ''' 开始下载一个列表中的文件。
    ''' </summary>
    ''' <param name="SourceFiles">WebRequireFile 的一维数组。</param>
    ''' <param name="Name">下载列表的名称。</param>
    ''' <param name="OnSuccess">下载成功时执行的代码。</param>
    ''' <param name="OnFail">下载失败时执行的代码。</param>
    ''' <param name="RequireSize">文件大小处理方式。</param>
    ''' <remarks></remarks>
    Public Sub WebStart(ByVal SourceFiles() As WebRequireFile, ByVal Name As String, ByVal OnSuccess As ParameterizedThreadStart, ByVal OnFail As ParameterizedThreadStart, ByVal RequireSize As WebRequireSize)
        Dim th As New Thread(Sub()

                                 Try

                                     '检查哪些文件需要下载，并将真正需要下载的文件整理为 CheckedFiles 数组

                                     Dim CheckedFiles As New ArrayList(SourceFiles)

                                     '检查文件是否存在文件名，如果不存在文件名则跳过过滤
                                     If Not CheckedFiles(0).LocalName = "" Then

                                         '过滤已经存在的文件
                                         Select Case RequireSize
                                             Case WebRequireSize.Known
                                                 '已知文件大小，检查文件：如果大小一致则不下载

                                                 For i = 0 To CheckedFiles.Count - 1
                                                     If CheckedFiles.Count - 1 < i Then Exit For
                                                     If GetFileSize(CheckedFiles(i).LocalFolder & CheckedFiles(i).LocalName) = CheckedFiles(i).KnownFileSize Then
                                                         CheckedFiles.RemoveAt(i)
                                                         i = i - 1
                                                     End If
                                                 Next

                                             Case WebRequireSize.Require
                                                 '不知道文件大小，所以不进行过滤

                                             Case WebRequireSize.DontNeed
                                                 '不需要知道文件大小，检查文件：如果文件存在则不下载

                                                 For i = 0 To CheckedFiles.Count - 1
                                                     If CheckedFiles.Count - 1 < i Then Exit For
                                                     If File.Exists(CheckedFiles(i).LocalFolder & CheckedFiles(i).LocalName) Then
                                                         CheckedFiles.RemoveAt(i)
                                                         i = i - 1
                                                     End If
                                                 Next

                                             Case WebRequireSize.AtLeast
                                                 '文件有大小要求，检查文件：如果文件大小大于指定值则不下载

                                                 For i = 0 To CheckedFiles.Count - 1
                                                     If CheckedFiles.Count - 1 < i Then Exit For
                                                     If GetFileSize(CheckedFiles(i).LocalFolder & CheckedFiles(i).LocalName) >= Math.Max(1, CheckedFiles(i).KnownFileSize) Then
                                                         CheckedFiles.RemoveAt(i)
                                                         i = i - 1
                                                     End If
                                                 Next

                                         End Select

                                         '去重
                                         CheckedFiles = ArrayNoDouble(CheckedFiles.ToArray)

                                         '如果没有剩余文件直接成功
                                         If CheckedFiles.Count = 0 Then
                                             '直接成功
                                             frmMain.Dispatcher.Invoke(OnSuccess, Name)
                                             Exit Sub
                                         End If

                                     End If

                                     '初始化文件组
                                     Dim NewGroup As New WebGroup With {.OnSuccess = OnSuccess, .OnFail = OnFail, .Files = New ArrayList, .GetServerFileSize = RequireSize, .Name = Name}

                                     '二次处理文件，将 WebRequireFile 转为 WebFile

                                     For i = 0 To CheckedFiles.Count - 1
                                         Dim NewFile As WebFile
                                         If CheckedFiles(0).LocalName = "" Then
                                             NewFile = New WebFile With {.AllWebAddress = CheckedFiles(i).WebURLs, .GetServerFileSize = RequireSize, .GroupName = Name,
                                                                                   .LocalFolder = CheckedFiles(i).LocalFolder, .TotalSize = New FileSize(CheckedFiles(i).KnownFileSize), .RefreshName = True}
                                         Else
                                             NewFile = New WebFile With {.AllWebAddress = CheckedFiles(i).WebURLs, .GetServerFileSize = RequireSize, .GroupName = Name,
                                                                                   .LocalFolder = CheckedFiles(i).LocalFolder, .TotalSize = New FileSize(CheckedFiles(i).KnownFileSize), .LocalName = CheckedFiles(i).LocalName, .RefreshName = False}
                                         End If
                                         NewGroup.Files.Add(NewFile)
                                     Next
                                     WebWaitingList.AddRange(NewGroup.Files)

                                     '添加进总列表
                                     If NewGroup.Files.Count = 0 Then Exit Sub
                                     If WebGroups.ContainsKey(Name) Then
                                         frmDownloadRight.RemoveGroup(WebGroups(Name))
                                         WebGroups.Remove(Name)
                                     End If
                                     log("[System] 开始下载列表文件：" & Name)
                                     WebTaskbarShow = ReadIni("setup", "DownProcess", "True") = "True"
                                     WebGroups.Add(Name, NewGroup)
                                     frmDownloadRight.AddGroup(NewGroup)

                                     '要求执行刷新
                                     WebWaitingToRun()

                                 Catch ex As Exception
                                     ExShow(ex, "处理下载列表失败：" & Name, ErrorLevel.MsgboxAndFeedback)
                                     frmMain.Dispatcher.Invoke(OnFail, Name)
                                 End Try

                             End Sub)
        th.Start()
    End Sub

    '下载中

    ''' <summary>
    ''' 处于 Waiting 状态的文件。
    ''' </summary>
    Private WebWaitingList As New ArrayList
    Public WebWaitingLockObj As Object = New Object
    ''' <summary>
    ''' 若队列有空闲，尝试让 Waiting 状态的文件开始下载。
    ''' </summary>
    Private Sub WebWaitingToRun()
        SyncLock WebWaitingLockObj
            For i = 0 To Math.Min(6, WebWaitingList.Count - 1)

                Try
                    If WebDownloadingList.Count >= WebDownloadCountMax Or i > Math.Min(6, WebWaitingList.Count - 1) Then Exit Sub
                    If WebDownloadingList.Contains(WebWaitingList(i).UUID) Then GoTo NextFile

                    WebDownloadingList.Add(WebWaitingList(i).UUID)
                    WebWaitingList(i).State = If(WebWaitingList(i).GetServerFileSize = WebRequireSize.Require Or WebWaitingList(i).GetServerFileSize = WebRequireSize.AtLeast,
                                WebDownloadStats.GetSize, WebDownloadStats.Download)
                    WebWaitingList.RemoveAt(i) '移出等待队列
                    i = i - 1
                Catch
                    i = i - 1
                End Try

NextFile:
            Next
        End SyncLock
    End Sub

    ''' <summary>
    ''' 获取文件大小，之后转入 Fail 或 Download 状态。
    ''' </summary>
    Private Sub WebStateGetSize(File As WebFile)
        Dim th As New Thread(Sub()

                                 Try

                                     If MODE_OFFLINE Then File.State = WebDownloadStats.Fail : Exit Sub
                                     Select Case File.GetServerFileSize
                                         Case WebRequireSize.AtLeast
                                             '要求至少有一定大小
                                             Dim TrueSize = GetFileSize(File.WebAddress, File)
                                             If TrueSize < 1 Then
                                                 Throw New Exception("获取文件大小失败！")
                                             ElseIf TrueSize < File.TotalSize Then
                                                 Throw New Exception("文件过小！（" & New FileSize(TrueSize).ToString & "/" & File.TotalSize.ToString & "）")
                                             End If
                                             File.TotalSize = TrueSize
                                         Case WebRequireSize.Require
                                             '只是需要知道大小
                                             File.TotalSize = GetFileSize(File.WebAddress, File)
                                             If File.TotalSize < 1 Then Throw New Exception("获取文件大小失败！")
                                     End Select
                                     '获取大小成功，开始下载
                                     File.State = WebDownloadStats.Download

                                 Catch ex As Exception
                                     ExShow(ex, "获取文件大小失败（" & File.LocalName & "）", ErrorLevel.Slient)
                                     File.State = WebDownloadStats.Fail
                                 End Try

                             End Sub)
        th.Start()
    End Sub

    ''' <summary>
    ''' 下载文件，之后转入 Fail 或 Success 状态。
    ''' </summary>
    Private Sub WebStateDownload(File As WebFile)
        Dim th As New Thread(Sub()

                                 Try
                                     '准备
                                     If Not Directory.Exists(File.LocalFolder) Then Directory.CreateDirectory(File.LocalFolder)
                                     File.SpeedCount = My.Computer.Clock.TickCount
                                     '下载
                                     My.Computer.Network.DownloadFile(File.WebAddress, File.LocalAddress & DOWNLOADING_END, "", "", False, 10000, True)
                                     '下载完毕检查
                                     File.GetSize = GetFileSize(File.LocalAddress & DOWNLOADING_END)
                                     If File.GetServerFileSize = WebRequireSize.DontNeed Or (File.GetSize = File.TotalSize And File.GetSize > 1) Then
                                         '下载成功
                                         IO.File.Delete(File.LocalAddress)
                                         Rename(File.LocalAddress & DOWNLOADING_END, File.LocalAddress)
                                         log("[Download] 下载文件成功：" & File.WebAddress)
                                         File.State = WebDownloadStats.Success
                                     Else
                                         '下载失败
                                         log("[Download] 下载文件失败（文件大小不匹配）：" & File.LocalAddress & "，所需大小：" & File.TotalSize.ToString & "，实际大小：" & File.GetSize.ToString & "（" & File.GetSize.Value & "）", True)
                                         IO.File.Delete(File.LocalAddress & DOWNLOADING_END)
                                         File.State = WebDownloadStats.Fail
                                     End If
                                 Catch ex As Exception
                                     ExShow(ex, "下载文件失败（" & File.LocalName & "）", ErrorLevel.Slient)
                                     File.State = WebDownloadStats.Fail
                                 End Try

                             End Sub)
        th.Start()

        '刷新显示线程
        File.RefreshThread = New Thread(Sub()
                                            Try

                                                Do While File.State = WebDownloadStats.Download
                                                    Thread.Sleep(950)

                                                    If Not File.State = WebDownloadStats.Download Then Exit Sub
                                                    Dim NewSize = GetFileSize(File.LocalAddress & DOWNLOADING_END)
                                                    File.Speed = (NewSize - File.GetSize) / (My.Computer.Clock.TickCount - File.SpeedCount) * 1000
                                                    File.SpeedCount = My.Computer.Clock.TickCount
                                                    File.GetSize = NewSize
                                                    File.Percent = If(File.TotalSize = 0, 0, Math.Round(File.GetSize / File.TotalSize, 3))
                                                    If Not File.State = WebDownloadStats.Download Then Exit Sub
                                                    frmDownloadRight.RefreshFile(File)
                                                Loop

                                            Catch
                                            End Try
                                        End Sub)
        File.RefreshThread.Start()
    End Sub

    ''' <summary>
    ''' 下载失败，之后可能转入 Retry。若重试成功才返回 True。
    ''' </summary>
    ''' <param name="File"></param>
    Private Function WebStateFail(File As WebFile)
        '重试
        If File.NextAddress() And Not MODE_OFFLINE Then
            '重试成功
            log("[Download] 重试地址：" & File.WebAddress)
            File.State = WebDownloadStats.Retry
            WebStateFail = True
        Else
            '重试失败
            WebStateFail = False
        End If
        '队列处理
        WebDownloadingList.Remove(File.UUID)
        WebWaitingToRun()
    End Function

    ''' <summary>
    ''' 下载成功。
    ''' </summary>
    Private Sub WebStateSuccess(File As WebFile)
        '队列处理
        WebDownloadingList.Remove(File.UUID)
        WebWaitingToRun()
    End Sub

    '任务栏进度条
    Public WebTaskbarShow As Boolean = True
    Private WebTaskbarLastState As TaskbarItemProgressState = TaskbarItemProgressState.None
    Private WebTaskbarLastValue As Double = 0

    ''' <summary>
    ''' 下载完成检查。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub WebFinishCheck(ByVal Name As String)
        Try

            If Not WebGroups.Keys.Contains(Name) Then Exit Sub
            Dim Group As WebGroup = WebGroups(Name)
            If IsNothing(Group.Files) Then Exit Sub
            If Not Group.Files.Count = Group.FinishedCount Then Exit Sub
            '遍历文件：如果存在失败的文件，将 isFail 设置为 True
            Dim isFail As Boolean = False
            For i = Group.Files.Count - 1 To 0 Step -1
                If Group.Files(i).State = WebDownloadStats.Fail Then isFail = True
            Next i
            '下载完成
            log("[Download] 列表文件下载完成：" & Name)
            '结束
            WebGroups.Remove(Name)
            frmDownloadRight.RemoveGroup(Group)
            If isFail Then
                frmMain.Dispatcher.Invoke(Group.OnFail, Name)
            Else
                frmMain.Dispatcher.Invoke(Group.OnSuccess, Name)
            End If

        Catch ex As Exception
            ExShow(ex, "检查下载列表出错")
        End Try
    End Sub

    ''' <summary>
    ''' 停止下载列表文件。
    ''' </summary>
    ''' <param name="Name"></param>
    ''' <remarks></remarks>
    Public Sub WebDownloadStop(ByVal Name As String)
        If WebGroups.ContainsKey(Name) Then
            frmDownloadRight.RemoveGroup(WebGroups(Name))
            WebGroups.Remove(Name)
        End If
    End Sub

    Public Sub WebStartRun()
        Dim th As New Thread(Sub()
                                 Do While True
                                     WebTimer()
                                     Thread.Sleep(250)
                                 Loop
                             End Sub)
        th.Priority = ThreadPriority.BelowNormal
        th.Start()
    End Sub
    Public Sub WebTimer()

        If Not WebTaskbarShow Then Exit Sub

        '应有值
        Dim ShouldState As TaskbarItemProgressState
        Dim ShouldValue As Double

        '获取应有值
        Try
            If WebGroups.Count > 0 Then
                Dim TotalPercent As Double = 0
                For Each Group As WebGroup In WebGroups.Values
                    TotalPercent = TotalPercent + Group.Percent
                Next
                ShouldState = If(TotalPercent = 0, TaskbarItemProgressState.Indeterminate, TaskbarItemProgressState.Normal)
                ShouldValue = TotalPercent / WebGroups.Count
            Else
                ShouldState = TaskbarItemProgressState.None
                ShouldValue = 0
            End If
        Catch
        End Try

        '更新值
        If Not WebTaskbarLastState = ShouldState Then
            frmMain.Dispatcher.Invoke(Sub() frmMain.TaskbarItemInfo.ProgressState = ShouldState)
            WebTaskbarLastState = ShouldState
        End If
        If Not WebTaskbarLastValue = ShouldValue Then
            frmMain.Dispatcher.Invoke(Sub() frmMain.TaskbarItemInfo.ProgressValue = ShouldValue)
            WebTaskbarLastValue = ShouldValue
        End If

    End Sub

End Module 'HTTP下载

Public Module BitDrawer

    Public GifList As New ArrayList 'Gif列表

    'Timer
    Public Sub GifStartRun()
        Dim time As New DispatcherTimer()
        time.Interval = TimeSpan.FromMilliseconds(1)
        AddHandler time.Tick, AddressOf GifTimer
        time.Start()
    End Sub
    Public Sub GifTimer()
        For i = 0 To GifList.Count - 1
            Dim gif As MyBitmap = GifList(i)
            '切换Gif的帧
            If My.Computer.Clock.TickCount - gif.RoundLastTime >= gif.RoundTime Then
                gif.RoundLastTime = My.Computer.Clock.TickCount
                If gif.CurrentFlame = gif.MaxFlame - 1 Then
                    If gif.PlayOnce Then
                        '如果只播放一次那么拜拜
                        GifList.Remove(gif)
                    Else
                        '如果不只播放一次那么继续
                        gif.SetGifFrame(0)
                    End If
                Else
                    gif.SetGifFrame(gif.CurrentFlame + 1)
                End If
            End If
            '刷新绑定控件的显示
            If Not IsNothing(gif.Control) Then gif.Control.Source = New MyBitmap(gif.Pic)
        Next
    End Sub

    '类
    Public Class MyBitmap

        ''' <summary>
        ''' 存储的图片
        ''' </summary>
        ''' <remarks></remarks>
        Public Pic As System.Drawing.Bitmap
        ''' <summary>
        ''' (Gif)绑定的控件
        ''' </summary>
        ''' <remarks></remarks>
        Public Control As System.Windows.Controls.Image = Nothing
        ''' <summary>
        ''' (Gif)每一帧的时间
        ''' </summary>
        ''' <remarks></remarks>
        Public RoundTime As Integer
        ''' <summary>
        ''' (Gif)上一次改变时的时间
        ''' </summary>
        ''' <remarks></remarks>
        Public RoundLastTime As Integer
        ''' <summary>
        ''' (Gif)是否只播放一次
        ''' </summary>
        ''' <remarks></remarks>
        Public PlayOnce As Boolean = False
        ''' <summary>
        ''' (Gif)当前帧，从0开始
        ''' </summary>
        ''' <remarks></remarks>
        Public CurrentFlame As Integer = 0
        ''' <summary>
        ''' (Gif)最大帧数，从1开始
        ''' </summary>
        ''' <remarks></remarks>
        Public MaxFlame As Integer

        '类型转换
        '支持的类：Image   ImageSource   Bitmap   ImageBrush
        Public Shared Widening Operator CType(ByVal Image As System.Drawing.Image) As MyBitmap
            Return New MyBitmap(Image)
        End Operator
        Public Shared Widening Operator CType(ByVal Image As MyBitmap) As System.Drawing.Image
            Return Image.Pic
        End Operator
        Public Shared Widening Operator CType(ByVal Image As ImageSource) As MyBitmap
            Return New MyBitmap(Image)
        End Operator
        Public Shared Widening Operator CType(ByVal Image As MyBitmap) As ImageSource
            Dim ptr As IntPtr = Image.Pic.GetHbitmap()
            Dim re As ImageSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                ptr,
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions())
            DeleteObject(ptr)
            Return re
        End Operator
        Public Shared Widening Operator CType(ByVal Image As System.Drawing.Bitmap) As MyBitmap
            Return New MyBitmap(Image)
        End Operator
        Public Shared Widening Operator CType(ByVal Image As MyBitmap) As System.Drawing.Bitmap
            Return Image.Pic
        End Operator
        Public Shared Widening Operator CType(ByVal Image As ImageBrush) As MyBitmap
            Return New MyBitmap(Image)
        End Operator
        Public Shared Widening Operator CType(ByVal Image As MyBitmap) As ImageBrush
            Return New ImageBrush(New MyBitmap(Image.Pic))
        End Operator

        '构造函数
        Public Sub New()
        End Sub
        Public Sub New(ByVal FilePath As String)
            Try
                If FilePath.StartsWith(PATH_IMAGE) Then
                    '使用缓存
                    If BitmapCache.ContainsKey(FilePath) Then
                        Pic = BitmapCache(FilePath).Pic
                    Else
                        Pic = New MyBitmap(CType((New ImageSourceConverter).ConvertFromString(FilePath), ImageSource))
                        BitmapCache.Add(FilePath, Pic)
                    End If
                Else
                    Using inputStream As New FileStream(FilePath, FileMode.Open)
                        Pic = New System.Drawing.Bitmap(inputStream)
                    End Using
                End If
            Catch ex As Exception
                Pic = My.Application.TryFindResource(FilePath)
                If IsNothing(Pic) Then
                    Pic = New System.Drawing.Bitmap(1, 1)
                    ExShow(ex, "加载图片失败，该图片加载已被跳过：" & FilePath, ErrorLevel.DebugOnly)
                End If
            End Try
        End Sub
        Public Sub New(ByVal Image As ImageSource)
            Using MS = New MemoryStream()
                Dim Encoder = New PngBitmapEncoder()
                Encoder.Frames.Add(BitmapFrame.Create(Image))
                Encoder.Save(MS)
                Pic = New System.Drawing.Bitmap(MS)
            End Using
        End Sub
        Public Sub New(ByVal Image As System.Drawing.Image)
            Pic = Image
        End Sub
        Public Sub New(ByVal Image As System.Drawing.Bitmap)
            Pic = Image
        End Sub
        Public Sub New(ByVal Image As ImageBrush)
            Using MS = New MemoryStream()
                Dim Encoder = New BmpBitmapEncoder()
                Encoder.Frames.Add(BitmapFrame.Create(Image.ImageSource))
                Encoder.Save(MS)
                Pic = New System.Drawing.Bitmap(MS)
            End Using
        End Sub

        ''' <summary>
        ''' 设置Gif的当前帧
        ''' </summary>
        ''' <param name="count">帧的编号，从0开始，超限制报错</param>
        ''' <remarks></remarks>
        Public Sub SetGifFrame(ByVal count As Integer)
            Pic.SelectActiveFrame(New FrameDimension(Pic.FrameDimensionsList()(0)), count)
            CurrentFlame = count
        End Sub

        ''' <summary>
        ''' 把这个对象视作Gif初始化
        ''' </summary>
        ''' <param name="round">两帧之间的间隔（毫秒）。</param>
        ''' <param name="once">是否只播放一次，True为是。</param>
        ''' <param name="control">绑定的控件。</param>
        ''' <remarks></remarks>
        Public Sub GifLoad(ByVal round As Integer, ByVal once As Boolean, Optional ByVal control As System.Windows.Controls.Image = Nothing)
            RoundTime = round
            RoundLastTime = My.Computer.Clock.TickCount
            PlayOnce = once
            Me.Control = control
            GifList.Add(Me)
            MaxFlame = Pic.GetFrameCount(New FrameDimension(Pic.FrameDimensionsList()(0)))
        End Sub

        ''' <summary>
        ''' 获取旋转的图片，这个方法不会导致原对象改变
        ''' </summary>
        ''' <param name="angle">旋转角度，单位为角度不是弧度啊</param>
        ''' <returns>旋转后的Bitmap</returns>
        ''' <remarks></remarks>
        Public Function Rotation(ByVal angle As Double) As System.Drawing.Bitmap
            With Me
                Dim img As System.Drawing.Image = Me.Pic
                Dim bitSize As Single = Math.Sqrt(img.Width ^ 2 + img.Height ^ 2)
                bitSize = img.Width
                Dim bmp As System.Drawing.Bitmap = New System.Drawing.Bitmap(CInt(bitSize), CInt(bitSize))
                Using g As System.Drawing.Graphics = System.Drawing.Graphics.FromImage(bmp)
                    g.TranslateTransform(bitSize / 2, bitSize / 2)
                    g.RotateTransform(angle)
                    g.TranslateTransform(-bitSize / 2, -bitSize / 2)
                    g.DrawImage(img, New System.Drawing.Rectangle(0, 0, img.Width, img.Width))
                End Using
                Return bmp
            End With
        End Function

        ''' <summary>
        ''' 获取左右翻转的图片，这个方法不会导致原对象改变
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function LeftRightFilp() As System.Drawing.Bitmap
            Dim bmp As System.Drawing.Bitmap = New System.Drawing.Bitmap(Me.Pic)
            bmp.RotateFlip(System.Drawing.RotateFlipType.RotateNoneFlipX)
            Return bmp
        End Function

    End Class

    '合并Bitmap
    Public Function BitmapAdd(ByVal background As System.Drawing.Bitmap, ByVal position As System.Drawing.Point, ByVal img As System.Drawing.Bitmap) As System.Drawing.Bitmap
        Dim graph As System.Drawing.Graphics = System.Drawing.Graphics.FromImage(background)
        graph.DrawImage(img, position)
        Return background
    End Function

    '裁剪Bitmap
    Public Function BitmapCut(ByVal img As BitmapSource, ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer) As BitmapSource
        Return New CroppedBitmap(img, New Int32Rect(x, y, width, height))
    End Function

End Module '绘图

'Public Module FTP

'    Public Function DownloadFTPFile(ByVal ftpclient As FTPDownloader, ByVal webPath As String, ByVal localPath As String)
'        File.Delete(localPath)
'        DownloadFTPFile = False
'        Try
'            If Not ftpclient.ftpIsLogin Then ftpclient.Login()
'            If webPath.Contains("/") Then ftpclient.ChangeDirectory(Mid(webPath, 1, webPath.LastIndexOf("/")))
'            ftpclient.DownloadFile(Mid(webPath, webPath.LastIndexOf("/") + 2), localPath)
'            Return True
'        Catch ex As Exception
'        End Try
'    End Function '直接下载单个文件

'    Public Class FTPDownloader

'#Region "变量声明"
'        Private m_sRemoteHost, m_sRemotePath, m_sRemoteUser As String
'        Private m_sRemotePassword, m_sMess As String
'        Private m_iRemotePort, m_iBytes As Int32
'        Private m_objClientSocket As Socket
'        Private m_iRetValue As Int32
'        Public ftpIsLogin As Boolean
'        Private m_sMes, m_sReply As String
'        '设置用户来对FTP服务器读取和写入数据的数据包的大小
'        Public Const BLOCK_SIZE = 512
'        Private m_aBuffer(BLOCK_SIZE) As Byte
'        Private ASCII As Encoding = Encoding.ASCII
'        Public flag_bool As Boolean
'        '普通变量定义
'        Private m_sMessageString As String
'#End Region

'#Region "构造函数"
'        Public Sub New()
'            m_sRemoteHost = "localhost"
'            m_sRemotePath = "."
'            m_sRemoteUser = "anonymous"
'            m_sRemotePassword = ""
'            m_sMessageString = ""
'            m_iRemotePort = 21
'            ftpIsLogin = False
'        End Sub
'        Public Sub New(ByVal sRemoteHost As String, ByVal sRemotePath As String, ByVal sRemoteUser As String, ByVal sRemotePassword As String, ByVal iRemotePort As Int32)
'            m_sRemoteHost = sRemoteHost
'            m_sRemotePath = sRemotePath
'            m_sRemoteUser = sRemoteUser
'            m_sRemotePassword = sRemotePassword
'            m_sMessageString = ""
'            m_iRemotePort = iRemotePort
'            ftpIsLogin = False
'        End Sub
'#End Region

'#Region "属性"
'        '设置或获取FTP服务器的名称
'        Public Property RemoteHostFTPServer() As String
'            Get
'                Return m_sRemoteHost
'            End Get
'            Set(ByVal Value As String)
'                m_sRemoteHost = Value
'            End Set
'        End Property
'        '设置或获取FTP服务器的端口
'        Public Property RemotePort() As Int32
'            Get
'                Return m_iRemotePort
'            End Get
'            Set(ByVal Value As Int32)
'                m_iRemotePort = Value
'            End Set
'        End Property
'        '设置或获取FTP服务器的远程路径
'        Public Property RemotePath() As String
'            Get
'                Return m_sRemotePath
'            End Get
'            Set(ByVal Value As String)
'                m_sRemotePath = Value
'            End Set
'        End Property
'        '设置或获取FTP服务器的用户
'        Public Property RemoteUser() As String
'            Get
'                Return m_sRemoteUser
'            End Get
'            Set(ByVal Value As String)
'                m_sRemoteUser = Value
'            End Set
'        End Property
'        '设置FTP服务器的密码
'        Public Property RemotePassword() As String
'            Get
'                Return m_sRemotePassword
'            End Get
'            Set(ByVal Value As String)
'                m_sRemotePassword = Value
'            End Set
'        End Property
'        '设置信息
'        Public Property MessageString() As String
'            Get
'                Return m_sMessageString
'            End Get
'            Set(ByVal Value As String)
'                m_sMessageString = Value
'            End Set
'        End Property
'#End Region

'        '登录FTP服务器
'        Public Function Login() As Boolean
'            If ftpIsLogin Then Return True
'            m_objClientSocket = _
'             New Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
'            Dim ep As New IPEndPoint(Dns.GetHostEntry(m_sRemoteHost).AddressList(0), m_iRemotePort)
'            Try
'                m_objClientSocket.Connect(ep)
'            Catch ex As Exception
'                MessageString = m_sReply
'                Throw New IOException("无法链接到FTP服务器。")
'            End Try
'            ReadReply()
'            If (m_iRetValue <> 220) Then
'                CloseConnection()
'                MessageString = m_sReply
'                Throw New IOException(m_sReply.Substring(4))
'            End If
'            '为了发送一个对服务器的用户登录ID，发送一个FTP命令
'            SendCommand("USER " & m_sRemoteUser)
'            If (Not (m_iRetValue = 331 Or m_iRetValue = 230)) Then
'                Cleanup()
'                MessageString = m_sReply
'                Throw New IOException(m_sReply.Substring(4))
'            End If
'            If (m_iRetValue <> 230) Then
'                '为了发送一个对服务器的用户密码，发送一个FTP命令
'                SendCommand("PASS " & m_sRemotePassword)
'                If (Not (m_iRetValue = 230 Or m_iRetValue = 202)) Then
'                    Cleanup()
'                    MessageString = m_sReply
'                    Throw New IOException(m_sReply.Substring(4))
'                End If
'            End If
'            ftpIsLogin = True
'            '为了改变映射的远程服务器的文件夹的目录，调用用户定义的ChangeDirectory函数
'            ChangeDirectory(m_sRemotePath)
'            '返回最终结果
'            Return ftpIsLogin
'        End Function

'        '获取文件列表
'        Public Function GetFileList(ByVal sMask As String) As String()
'            Dim cSocket As Socket
'            Dim bytes As Int32
'            Dim seperator As Char = ControlChars.Lf
'            Dim mess() As String
'            m_sMes = ""
'            '检查是否登录
'            If (Not (ftpIsLogin)) Then Login()
'            cSocket = CreateDataSocket()
'            '发送FTP命令
'            SendCommand("NLST " & sMask)
'            If (Not (m_iRetValue = 150 Or m_iRetValue = 125)) Then
'                MessageString = m_sReply
'                Throw New IOException(m_sReply.Substring(4))
'            End If
'            m_sMes = ""
'            Do While (True)
'                Array.Clear(m_aBuffer, 0, m_aBuffer.Length)
'                bytes = cSocket.Receive(m_aBuffer, m_aBuffer.Length, 0)
'                m_sMes += ASCII.GetString(m_aBuffer, 0, bytes)
'                If (bytes < m_aBuffer.Length) Then
'                    Exit Do
'                End If
'            Loop
'            mess = m_sMes.Split(seperator)
'            cSocket.Close()
'            ReadReply()
'            If (m_iRetValue <> 226) Then
'                MessageString = m_sReply
'                Throw New IOException(m_sReply.Substring(4))
'            End If
'            Return mess
'        End Function
'        '获取文件大小
'        Public Function GetFileSize(ByVal sFileName As String) As Integer
'            Dim size As Integer
'            '检查是否登录
'            If Not ftpIsLogin Then Login()
'            '发送FTP命令
'            SetBinaryMode(True)
'            SendCommand("SIZE " & sFileName)
'            size = 0
'            If (m_iRetValue = 213) Then
'                size = Int64.Parse(m_sReply.Substring(4))
'            Else
'                MessageString = m_sReply
'                Throw New IOException(m_sReply.Substring(4))
'            End If
'            Return size
'        End Function
'        '下载文件
'        Public Sub DownloadFile(ByVal fileName As String, Optional ByVal localFile As String = "", Optional ByVal isResume As Boolean = False)
'            Dim st As Stream
'            Dim cSocket As Socket
'            Dim offset, npos As Integer
'            If Not ftpIsLogin Then Login() '检查是否登录
'            SetBinaryMode(True) '设置下载模式
'            '检查下载目录
'            If localFile = "" Then localFile = fileName
'            If Not (File.Exists(localFile)) Then
'                st = File.Create(localFile)
'                st.Close()
'            End If
'            '下载
'            Using output As New FileStream(localFile, FileMode.Open)
'                cSocket = CreateDataSocket()
'                offset = 0
'                If isResume Then
'                    offset = output.Length
'                    If offset > 0 Then
'                        '发送一个FTP命令重新启动
'                        SendCommand("REST " & offset)
'                        If (m_iRetValue <> 350) Then offset = 0
'                        npos = output.Seek(offset, SeekOrigin.Begin)
'                    End If
'                End If
'                '发送一个FTP命令重新找到一个文件
'                SendCommand("RETR " & fileName)
'                If (Not (m_iRetValue = 150 Or m_iRetValue = 125)) Then
'                    MessageString = m_sReply
'                    Throw New IOException(m_sReply.Substring(4))
'                End If
'                Do While (True)
'                    Array.Clear(m_aBuffer, 0, m_aBuffer.Length)
'                    m_iBytes = cSocket.Receive(m_aBuffer, m_aBuffer.Length, 0)
'                    output.Write(m_aBuffer, 0, m_iBytes)
'                    If (m_iBytes <= 0) Then
'                        Exit Do
'                    End If
'                Loop
'            End Using
'            If (cSocket.Connected) Then
'                cSocket.Close()
'            End If
'            ReadReply()
'            If (Not (m_iRetValue = 226 Or m_iRetValue = 250)) Then
'                MessageString = m_sReply
'                Throw New IOException(m_sReply.Substring(4))
'            End If
'        End Sub

'        '设置下载模式
'        Public Sub SetBinaryMode(ByVal bMode As Boolean)
'            '发送FTP命令，设置为二进制模式(True)或ASCII模式(False)
'            SendCommand(If(bMode, "TYPE I", "TYPE A"))
'            If (m_iRetValue <> 200) Then
'                MessageString = m_sReply
'                Throw New IOException(m_sReply.Substring(4))
'            End If
'        End Sub

'        'Dim str_file_path As String = "C:\test.txt" '要上传的文件名及路径
'        'Dim str_filename As String = IO.Path.GetFileName(str_file_path) '要上传的文件名
'        'Dim MyFileSize As Integer = FileLen(str_file_path)   '要上传得文件大小
'        'If (ftpClient.Login() = True) Then
'        '    '创建一个新文件夹
'        '    ftpClient.CreateDirectory("FTPFOLDERNEW")
'        '    '将新的文件夹设置为活动文件夹。
'        '    ftpClient.ChangeDirectory("FTPFOLDERNEW")
'        '    '设置FTP模式
'        '    ftpClient.SetBinaryMode(True)
'        '    '从你的硬盘上上载一个文件到FTP网页
'        '    ftpClient.UploadFile(str_file_path)
'        '    '获得刚刚上传的文件的大小
'        '    Dim lng_filesize As Integer
'        '    lng_filesize = ftpClient.GetFileSize(str_filename)
'        '    If lng_filesize = MyFileSize Then
'        '        MsgBox("成功上传文件" & str_filename & " 文件大小" & lng_filesize)
'        '    Else
'        '        MsgBox("上传失败")
'        '    End If
'        '    '对上载文件重命名
'        '    'ftpClient.RenameFile("SampleFile.xml", "SampleFile_new.xml")
'        '    '删除一个文件
'        '    'ftpClient.DeleteFile("SampleFile_new.xml")
'        '    ftpClient.CloseConnection()
'        'End If
'        ''这是一个从你的本地硬盘上向你的FTP文件夹中上载文件的函数
'        'Public Sub UploadFile(ByVal sFileName As String)
'        '    UploadFile(sFileName, False)
'        'End Sub
'        ''这是一个从你的本地硬盘上向你的FTP网页上上载的函数和设置恢复标志
'        'Public Sub UploadFile(ByVal sFileName As String,
'        ' ByVal bResume As Boolean)
'        '    Dim cSocket As Socket
'        '    Dim offset As Integer
'        '    Dim input As FileStream
'        '    Dim bFileNotFound As Boolean
'        '    If (Not (ftpIsLogin)) Then
'        '        Login()
'        '    End If
'        '    cSocket = CreateDataSocket()
'        '    offset = 0
'        '    If (bResume) Then
'        '        Try
'        '            SetBinaryMode(True)
'        '            offset = GetFileSize(sFileName)
'        '        Catch ex As Exception
'        '            offset = 0
'        '        End Try
'        '    End If
'        '    If (offset > 0) Then
'        '        SendCommand("REST " & offset)
'        '        If (m_iRetValue <> 350) Then
'        '            '远程服务器可能不支持恢复。
'        '            offset = 0
'        '        End If
'        '    End If
'        '    '发送一个FTP命令，存储一个文件。 
'        '    SendCommand("STOR " & Path.GetFileName(sFileName))
'        '    If (Not (m_iRetValue = 125 Or m_iRetValue = 150)) Then
'        '        MessageString = m_sReply
'        '        Throw New IOException(m_sReply.Substring(4))
'        '    End If
'        '    '在上载之前，检查文件是否存在。
'        '    bFileNotFound = False
'        '    If (File.Exists(sFileName)) Then
'        '        '打开输入流读取源文件
'        '        input = New FileStream(sFileName, FileMode.Open)
'        '        If (offset <> 0) Then
'        '            input.Seek(offset, SeekOrigin.Begin)
'        '        End If
'        '        '上载这个文件
'        '        m_iBytes = input.Read(m_aBuffer, 0, m_aBuffer.Length)
'        '        Do While (m_iBytes > 0)
'        '            cSocket.Send(m_aBuffer, m_iBytes, 0)
'        '            m_iBytes = input.Read(m_aBuffer, 0, m_aBuffer.Length)
'        '        Loop
'        '        input.Close()
'        '    Else
'        '        bFileNotFound = True
'        '    End If
'        '    If (cSocket.Connected) Then
'        '        cSocket.Close()
'        '    End If
'        '    '如果找不到文件，检查返回值
'        '    If (bFileNotFound) Then
'        '        MessageString = m_sReply
'        '        Throw New IOException("The file: " & sFileName & " was not found. " & _
'        '       "Cannot upload the file to the FTP site")
'        '    End If
'        '    ReadReply()
'        '    If (Not (m_iRetValue = 226 Or m_iRetValue = 250)) Then
'        '        MessageString = m_sReply
'        '        Throw New IOException(m_sReply.Substring(4))
'        '    End If
'        'End Sub
'        ''从远程FTP服务器上删除一个文件。
'        'Public Function DeleteFile(ByVal sFileName As String) As Boolean
'        '    Dim bResult As Boolean
'        '    bResult = True
'        '    If (Not (ftpIsLogin)) Then
'        '        Login()
'        '    End If
'        '    '发送一个FTP命令，删除一个文件。
'        '    SendCommand("DELE " & sFileName)
'        '    If (m_iRetValue <> 250) Then
'        '        bResult = False
'        '        MessageString = m_sReply
'        '    End If
'        '    '返回最终结果
'        '    Return bResult
'        'End Function
'        ''在远程FTP服务器上重命名一个文件
'        'Public Function RenameFile(ByVal sOldFileName As String,
'        'ByVal sNewFileName As String) As Boolean
'        '    Dim bResult As Boolean
'        '    bResult = True
'        '    If (Not (ftpIsLogin)) Then
'        '        Login()
'        '    End If
'        '    '发送一个FTP命令，对一个文件重命名
'        '    SendCommand("RNFR " & sOldFileName)
'        '    If (m_iRetValue <> 350) Then
'        '        MessageString = m_sReply
'        '        Throw New IOException(m_sReply.Substring(4))
'        '    End If
'        '    '发送一个FTP命令，对一个文件更改为新名称
'        '    '如果新的文件名存在，会被覆盖。
'        '    SendCommand("RNTO " & sNewFileName)
'        '    If (m_iRetValue <> 250) Then
'        '        MessageString = m_sReply
'        '        Throw New IOException(m_sReply.Substring(4))
'        '    End If
'        '    '返回最终结果
'        '    Return bResult
'        'End Function
'        ''这是一个在远程服务器上创建目录的函数
'        'Public Function CreateDirectory(ByVal sDirName As String) As Boolean
'        '    Dim bResult As Boolean
'        '    bResult = True
'        '    If (Not (ftpIsLogin)) Then
'        '        Login()
'        '    End If
'        '    '发送一个FTP命令，在FTP服务器上制作一个目录
'        '    SendCommand("MKD " & sDirName)
'        '    If (m_iRetValue <> 257) Then
'        '        bResult = False
'        '        MessageString = m_sReply
'        '    End If
'        '    '返回最终结果
'        '    Return bResult
'        'End Function
'        ''这是一个在远程FTP服务器上删除目录的函数
'        'Public Function RemoveDirectory(ByVal sDirName As String) As Boolean
'        '    Dim bResult As Boolean
'        '    bResult = True
'        '    '检查是否已登录FTP服务器
'        '    If (Not (ftpIsLogin)) Then
'        '        Login()
'        '    End If
'        '    '发送一个FTP命令，删除在FTP服务器上的目录
'        '    SendCommand("RMD " & sDirName)
'        '    If (m_iRetValue <> 250) Then
'        '        bResult = False
'        '        MessageString = m_sReply
'        '    End If
'        '    '返回最终结果
'        '    Return bResult
'        'End Function

'        '改变当前工作目录
'        Public Function ChangeDirectory(ByVal sDirName As String) As Boolean
'            Dim bResult As Boolean
'            bResult = True
'            '检查你是否在根目录
'            If (sDirName.Equals(".")) Then Return False
'            '检查是否已登录FTP服务器
'            If Not (ftpIsLogin) Then Login()
'            '发送FTP命令，改变在FTP服务器上的目录。
'            SendCommand("CWD " & sDirName)
'            If (m_iRetValue <> 250) Then
'                bResult = False
'                MessageString = m_sReply
'            End If
'            Me.m_sRemotePath = sDirName
'            '返回最终结果
'            Return bResult
'        End Function
'        '关闭FTP链接
'        Public Sub CloseConnection()
'            If Not (m_objClientSocket Is Nothing) Then
'                '发送一个FTP命令，结束FTP服务系统
'                SendCommand("QUIT")
'            End If
'            Cleanup()
'        End Sub

'#Region "内部函数"
'        '从FTP服务器得到回应。
'        Private Sub ReadReply()
'            m_sMes = ""
'            m_sReply = ReadLine()
'            m_iRetValue = Int32.Parse(m_sReply.Substring(0, 3))
'        End Sub
'        '重置
'        Private Sub Cleanup()
'            If Not (m_objClientSocket Is Nothing) Then
'                m_objClientSocket.Close()
'                m_objClientSocket = Nothing
'            End If
'            ftpIsLogin = False
'        End Sub
'        '从FTP服务器读取一行
'        Private Function ReadLine(Optional ByVal bClearMes As Boolean = False) As String
'            Dim seperator As Char = ControlChars.Lf
'            Dim mess() As String
'            If (bClearMes) Then
'                m_sMes = ""
'            End If
'            Do While (True)
'                Array.Clear(m_aBuffer, 0, BLOCK_SIZE)
'                Try
'                    m_iBytes = m_objClientSocket.Receive(m_aBuffer, m_aBuffer.Length, 0)
'                Catch ex As Exception
'                    Login()
'                    m_iBytes = m_objClientSocket.Receive(m_aBuffer, m_aBuffer.Length, 0)
'                End Try
'                m_sMes += ASCII.GetString(m_aBuffer, 0, m_iBytes)
'                If (m_iBytes < m_aBuffer.Length) Then
'                    Exit Do
'                End If
'            Loop
'            mess = m_sMes.Split(seperator)
'            If (m_sMes.Length > 2) Then
'                m_sMes = mess(mess.Length - 2)
'            Else
'                m_sMes = mess(0)
'            End If
'            If (Not (m_sMes.Substring(3, 1).Equals(" "))) Then
'                Return ReadLine(True)
'            End If
'            Return m_sMes
'        End Function
'        '向FTP服务器发送命令
'        Private Sub SendCommand(ByVal command As String)
'            command = command & ControlChars.CrLf
'            Dim cmdbytes As Byte() = ASCII.GetBytes(command)
'            m_objClientSocket.Send(cmdbytes, cmdbytes.Length, 0)
'            ReadReply()
'        End Sub
'        '创建一个数据包 
'        Private Function CreateDataSocket() As Socket
'            Dim index1, index2, len As Int32
'            Dim partCount, i, port As Int32
'            Dim ipData, buf, ipAddress As String
'            Dim parts(6) As Int32
'            Dim ch As Char
'            Dim s As Socket
'            Dim ep As IPEndPoint
'            '发送一个FTP命令，用于被动数据链接
'            SendCommand("PASV")
'            If (m_iRetValue <> 227) Then
'                MessageString = m_sReply
'                Throw New IOException(m_sReply.Substring(4))
'            End If
'            index1 = m_sReply.IndexOf("(")
'            index2 = m_sReply.IndexOf(")")
'            ipData = m_sReply.Substring(index1 + 1, index2 - index1 - 1)
'            len = ipData.Length
'            partCount = 0
'            buf = ""
'            For i = 0 To ((len - 1) And partCount <= 6)
'                ch = Char.Parse(ipData.Substring(i, 1))
'                If (Char.IsDigit(ch)) Then
'                    buf += ch
'                ElseIf (ch <> ",") Then
'                    MessageString = m_sReply
'                    Throw New IOException("Malformed PASV reply: " & m_sReply)
'                End If
'                If ((ch = ",") Or (i + 1 = len)) Then
'                    Try
'                        parts(partCount) = Int32.Parse(buf)
'                        partCount += 1
'                        buf = ""
'                    Catch ex As Exception
'                        MessageString = m_sReply
'                        Throw New IOException("Malformed PASV reply: " & m_sReply)
'                    End Try
'                End If
'            Next
'            ipAddress = parts(0) & "." & parts(1) & "." & parts(2) & "." & parts(3)
'            '在Visual Basic .Net 2002中进行调用。你想移动8位。在Visual Basic .NET 2002中，你必须将此数乘2的8次方。
'            '端口=parts(4)*(2^8)
'            '进行这个调用，并且用Visual Basic .NET 2003解释当前行。

'            port = parts(4) << 8
'            '确定数据端口数
'            port = port + parts(5)
'            s = New Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
'            ep = New IPEndPoint(Dns.GetHostEntry(ipAddress).AddressList(0), port)
'            Try
'                s.Connect(ep)
'            Catch ex As Exception
'                MessageString = m_sReply
'                Throw New IOException("Cannot connect to remote server.")
'                '如果你不能链接到特定的FTP服务器，也就是说，将其布尔值设置为False。
'                flag_bool = False
'            End Try
'            '如果你能够链接到特定的FTP服务器，将布尔值设置为True。
'            flag_bool = True
'            Return s
'        End Function
'#End Region

'    End Class

'End Module 'FTP协议下载 