''' <summary>
''' A wrapper around the PROJ CS2CS app for converting coordinates
''' </summary>
Public Class CS2CS
    Private CsProc As Process
    Private InputStream As IO.StreamWriter
    Private OutPutStream As IO.StreamReader
    Private ErrorStream As IO.StreamReader
    Private Logger As NLog.Logger

    Public Sub New(Logger As NLog.Logger)
        Me.Logger = Logger
        Dim StInf As New ProcessStartInfo
        StInf.FileName = "C:\OSGeo4W\bin\cs2cs.exe"
        StInf.Arguments = $"+init=epsg:25832 +to +init=epsg:4326  -f {Chr(34)}%.5F{Chr(34)}"
        StInf.StandardInputEncoding = System.Text.ASCIIEncoding.ASCII
        StInf.StandardOutputEncoding = System.Text.ASCIIEncoding.ASCII
        StInf.RedirectStandardInput = True
        StInf.RedirectStandardOutput = True
        StInf.RedirectStandardError = True
        StInf.CreateNoWindow = True
        Try
            CsProc = New Process
            CsProc.StartInfo = StInf
            CsProc.Start()
            InputStream = CsProc.StandardInput
            OutPutStream = CsProc.StandardOutput
            ErrorStream = CsProc.StandardError
        Catch ex As Exception
            Logger.Error("while booting CS2CS:")
            Logger.Error(ex)
        End Try



    End Sub
    Public Sub Close()
        InputStream?.Close()
        OutPutStream?.Close()
        ErrorStream?.Close()
        CsProc?.Kill()
    End Sub
    Protected Overrides Sub Finalize()
        On Error Resume Next
        Close()
        MyBase.Finalize()
    End Sub

    Public Function Euref89ToOSM(POINT As String) As PointF
        'POINT(891000.46 6120114.84)
        'echo 444479 6159314 | cs2cs +init=epsg:25832 +to +init=epsg:4326  -f "%.5F"
        Dim out As String
        Dim outP As New PointF
        If String.IsNullOrWhiteSpace(POINT) Then
            Logger.Info("Found SDFE building with no coordinates.")
            Return New PointF
        End If
        POINT = POINT.ToUpper.Replace("POINT(", String.Empty)
        POINT = POINT.Replace(")", String.Empty).Trim
        Me.InputStream.WriteLine(POINT)
        Threading.Thread.Sleep(10)
        out = OutPutStream.ReadLine().Replace(vbTab, " ")
        If String.IsNullOrWhiteSpace(out) Then
            Logger.Error("No conversion:")
            Logger.Error(ErrorStream.ReadToEnd)
        Else
            Dim s() As String = out.Split(" ")
            outP.Y = Double.Parse(s(0), Globalization.CultureInfo.InvariantCulture)
            outP.X = Double.Parse(s(1), Globalization.CultureInfo.InvariantCulture)
        End If
        Return outP
    End Function

End Class
