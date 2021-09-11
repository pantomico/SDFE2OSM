Public Class Nlogger
    Public WithEvents Logger As NLog.Logger

    Public Sub New(logDebug As Boolean)
        Try
            'set up logging
            Dim lconf As New NLog.Config.LoggingConfiguration
            Dim ltarg As New NLog.Targets.FileTarget(System.Reflection.Assembly.GetEntryAssembly.GetName.Name)
            Dim lCons As New NLog.Targets.ColoredConsoleTarget("Console")
            Dim s As String = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)

            s = System.IO.Path.Combine(s, System.Reflection.Assembly.GetEntryAssembly.GetName.Name)
            s = System.IO.Path.Combine(s, ltarg.Name & ".log")
            With ltarg
                .AutoFlush = True
                .CreateDirs = True
                .DeleteOldFileOnStartup = True
                .FileName = s
            End With
            If logDebug Then
                lconf.AddRuleForAllLevels(ltarg)
            Else
                lconf.AddRuleForOneLevel(NLog.LogLevel.Error, ltarg)
            End If

            lconf.AddRuleForAllLevels(lCons)
            NLog.LogManager.Configuration = lconf
            'Logger = NLog.LogManager.GetLogger(My.Application.Info.AssemblyName)
            Logger = NLog.LogManager.GetCurrentClassLogger
            Logger.Log(NLog.LogLevel.Info, "Logging has started")
            Debug.WriteLine(ltarg.FileName)
        Catch ex As Exception
            Debug.WriteLine("Exception:" & ex.Message, "Exception")
        End Try
    End Sub

    Protected Overrides Sub Finalize()
        Logger.Log(NLog.LogLevel.Info, "Logging has ended")
        Logger = Nothing

        MyBase.Finalize()
    End Sub
End Class
