Public Class msgVersion

    Private CanWrite As Boolean = False
    Private Version As MCVersion

    Public Sub New(ByVal version As MCVersion)
        InitializeComponent()
        UseControlAnimation = False
        Me.Version = version
        panMain.UpdateLayout()

        '读取设置
        Select Case Val(ReadIni(version.Path & "\PCL\AdvancedSetup.ini", "CustomType", "0"))
            Case 1
                radioType1.Checked = True
            Case 2
                radioType2.Checked = True
            Case 3
                radioType3.Checked = True
            Case Else
                radioType0.Checked = True
        End Select

        UseControlAnimation = True
        CanWrite = True '可以写入设置
    End Sub

    Private Sub WriteSetup() Handles radioType0.MouseUp, radioType1.MouseUp, radioType2.MouseUp, radioType3.MouseUp
        If CanWrite Then
            File.Create(Version.Path & "\PCL\AdvancedSetup.ini").Dispose()
            Using iniWriter As New StreamWriter(Version.Path & "\PCL\AdvancedSetup.ini", False, Encoding.Unicode)
                iniWriter.WriteLine()
                If radioType1.Checked Then
                    iniWriter.WriteLine("CustomType:1")
                ElseIf radioType2.Checked Then
                    iniWriter.WriteLine("CustomType:2")
                ElseIf radioType3.Checked Then
                    iniWriter.WriteLine("CustomType:3")
                End If
                iniWriter.Flush()
            End Using
        End If
    End Sub

End Class
