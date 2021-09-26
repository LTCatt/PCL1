Imports System.IO
Imports System.Threading

Namespace My
    Partial Friend Class MyApplication
        Public Shared Sub Main()
            Try
                '处理启动参数
                Dim StartScript As String = ""
                For Each Child As String In Application.CommandLineArgs
                    StartScript = StartScript & Child & " "
                Next
                If StartScript.EndsWith(" ") Then StartScript = Mid(StartScript, 1, Len(StartScript) - 1)

                '判断启动参数头
                Dim Data As String = StartScript.Replace(Split(StartScript, ":")(0) & ":", "")
                Select Case Split(StartScript, ":")(0)

                    Case "Set Environment Variable"
                        '设置环境变量
                        Environment.SetEnvironmentVariable("Path", Data, EnvironmentVariableTarget.Machine)

                    Case "Auto Update"
                        '自动更新
                        Dim RetryCount As Integer = 0
                        Dim ProcessId As Integer = Val(Data.Split("|")(1))
                        Data = Data.Split("|")(0)
Retry:
                        '关闭PCL进程
                        Try
                            Dim Process As Process = Process.GetProcessById(ProcessId)
                            Process.Kill()
                        Catch
                        End Try
                        Try
                            Dim f = New Func
                            f.CopyDerictory(New DirectoryInfo(Data & "PCL\Update\"), New DirectoryInfo(Data))
                            Directory.Delete(Data & "PCL\Update", True)
                            Shell(Data & "PCL.exe", AppWinStyle.NormalFocus)
                        Catch ex As Exception
                            RetryCount = RetryCount + 1
                            If RetryCount < 10 Then
                                Thread.Sleep(1500)
                                GoTo Retry
                            End If
                            MsgBox("自动更新失败：" & ex.ToString, MsgBoxStyle.Critical, "Plain Craft Launcher - PCLAM")
                        End Try

                End Select

            Catch ex As Exception
                MsgBox("处理失败：" & ex.ToString, MsgBoxStyle.Critical, "Plain Craft Launcher - PCLAM")
            End Try
        End Sub

    End Class
End Namespace

Public Class Func


    Public Sub CopyDerictory(ByVal DirectorySrc As DirectoryInfo, ByVal DirectoryDes As DirectoryInfo)
        Dim strDirectoryDesPath As String = DirectoryDes.FullName '& "" & DirectorySrc.Name
        If Not Directory.Exists(strDirectoryDesPath) Then Directory.CreateDirectory(strDirectoryDesPath)
        Dim f, fs() As FileInfo
        fs = DirectorySrc.GetFiles()
        For Each f In fs
            File.Copy(f.FullName, strDirectoryDesPath & f.Name.ToString, True)
        Next
        Dim DirSrc, Dirs() As DirectoryInfo
        Dirs = DirectorySrc.GetDirectories()
        '递归调用自身
        For Each DirSrc In Dirs
            Dim DirDes As New DirectoryInfo(strDirectoryDesPath)
            CopyDerictory(DirSrc, DirDes)
        Next
    End Sub

End Class