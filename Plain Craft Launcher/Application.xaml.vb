Imports System.Reflection

Class Application

    Private Sub Application_Startup(ByVal sender As Object, ByVal e As System.Windows.StartupEventArgs) Handles Me.Startup
        Try
            Process.GetCurrentProcess.PriorityClass = ProcessPriorityClass.High
            Application.Current.Dispatcher.Thread.Priority = ThreadPriority.AboveNormal
            '基础初始化
            AddHandler AppDomain.CurrentDomain.AssemblyResolve, AddressOf AssemblyResolve '动态 DLL 调用
            LoadTimeCost = My.Computer.Clock.TickCount
            Directory.CreateDirectory(PATH & "PCL")
            Directory.CreateDirectory(PATH & "PCL\cache")
            Directory.CreateDirectory(PATH & "PCL\download")
            Try
                File.Delete(PATH & "PCL\log.txt")
            Catch
            End Try
            log("[Application] 程序启动，版本：" & VERSION_NAME & "（" & VERSION_CODE & "）")
            '读取设置
            MODE_DEBUG = ReadReg("SysTest", "False")
            MODE_DEVELOPER = File.Exists(PATH & "Plain Craft Launcher Developer Tag")
            '配置初始化
            Net.ServicePointManager.DefaultConnectionLimit = 1024
        Catch ex As Exception
            If MsgBox("程序初始化时出现异常：" & GetStringFromException(ex, True) & vbCrLf & "是否愿意打开反馈页面来反馈这个问题？" & vbCrLf & "这会帮助作者解决问题，否则它将永远不会得到解决！", MsgBoxStyle.Critical + MsgBoxStyle.YesNo, "炸了") = MsgBoxResult.Yes Then Feedback()
        End Try
    End Sub
    Private Sub Application_DispatcherUnhandledException(ByVal sender As Object, ByVal e As System.Windows.Threading.DispatcherUnhandledExceptionEventArgs) Handles Me.DispatcherUnhandledException
        On Error Resume Next
        log("[Application] 请把这个 Log 文件发送给作者，以助于改进这个启动器，谢谢~")
        log("[Application] 异常的详细信息：" & vbCrLf & GetStringFromException(e.Exception, True))
        If MsgBox("程序出现异常：" & GetStringFromException(e.Exception, True) & vbCrLf & "是否愿意打开反馈页面来反馈这个问题？" & vbCrLf & "这会帮助作者解决问题，否则它将永远不会得到解决！", MsgBoxStyle.Critical + MsgBoxStyle.YesNo, "炸了") = MsgBoxResult.Yes Then Feedback()
        EndForce()
    End Sub

    '动态 DLL 调用
    Private Ionic As Assembly = Assembly.Load(My.Resources.ResourceManager.GetObject("Ionic_Zip"))
    Private Newtonsoft As Assembly = Assembly.Load(My.Resources.ResourceManager.GetObject("Newtonsoft_Json"))
    Private Function AssemblyResolve(sender As Object, args As ResolveEventArgs) As Assembly
        Dim Name As String = New AssemblyName(args.Name).Name
        Select Case Name
            Case "Ionic.Zip"
                Return Ionic
            Case "Newtonsoft.Json"
                Return Newtonsoft
            Case Else
                Return Nothing
        End Select
    End Function

End Class
