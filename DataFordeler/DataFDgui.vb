Imports Json = System.Text.Json
Public Class DataFDgui
    Public UserName As String
    Public Password As String
    Public Const APIURL As String = "https://services.datafordeler.dk/"
    Private Job As Task(Of Boolean)
    Private cl As New Net.Http.HttpClient()
    Private ubi As UriBuilder
    Private Logger As NLog.Logger
    Public LastBuilding As String = String.Empty
    Private Cs As CS2CS
    ''' <summary>
    ''' Contains all the buildings in one husnummer
    ''' </summary>
    Private buildingBuffer As Collections.Generic.Dictionary(Of String, Building.FormattedBuilding)
    ''' <summary>
    ''' GET URL as key, response as byte() as value
    ''' </summary>
    Private DFcache As Dictionary(Of String, Byte())


    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

        Me.txtUsername.Text = My.Settings.username
        Me.txtPassword.Text = My.Settings.password
        Dim pwf As New IO.FileInfo("C:\Users\mico.PANTOINSPECT\OneDrive\Documents\sdfe2osm.txt")
        If pwf.Exists Then
            Dim pw = pwf.OpenText
            UserName = pw.ReadLine
            Password = pw.ReadLine
            pw.Close()
        Else
            UserName = Me.txtUsername.Text
            Password = Me.txtPassword.Text
        End If


    End Sub

    Public Sub SetLogger(Logger As NLog.Logger)
        ubi = New UriBuilder(APIURL)
        Me.Logger = Logger
        Cs = New CS2CS(Logger)
        buildingBuffer = New Dictionary(Of String, Building.FormattedBuilding)
        DFcache = New Dictionary(Of String, Byte())
        LoadCache()
    End Sub


    Public Async Function GetBuilding(HusNummer As String, JordStykke As String) As Task(Of Building.FormattedBuilding())
        Dim s As New Text.StringBuilder
        Dim b(1) As Building.Building
        Dim resp As Net.Http.HttpResponseMessage
        Dim resC() As Byte
        If String.IsNullOrWhiteSpace(HusNummer) AndAlso String.IsNullOrWhiteSpace(JordStykke) Then
            Logger.Error("GetBuilding: No data to query")
            Throw New ArgumentNullException("HusNummer")
        End If
        If String.IsNullOrWhiteSpace(HusNummer) Then
            s.Append("jordstykke=")
            s.Append(JordStykke.Trim)
        Else
            s.Append("husnummer=")
            s.Append(HusNummer)
        End If
        s.Append($"&username={UserName}&password={Password}")
        ubi = New UriBuilder(APIURL)
        ubi.Path = "BBR/BBRPublic/1/rest/bygning"
        ubi.Query = s.ToString

        Try
            If DFcache.ContainsKey(ubi.Uri.ToString) Then
                resp = New Net.Http.HttpResponseMessage(Net.HttpStatusCode.OK)
            Else
                Using cl = New Net.Http.HttpClient
                    cl.BaseAddress = New Uri(APIURL)
                    Logger.Info($"Asking datafordeler: {ubi.Uri.ToString}")
                    resp = Await cl.GetAsync(ubi.Uri)
                End Using
            End If

        Catch ex As Exception
            Debug.WriteLine($"Failed to get API data: {ex.Message}")
            resp = New Net.Http.HttpResponseMessage
            resp.StatusCode = Net.HttpStatusCode.ExpectationFailed
            Logger.Error("Failed to get API data:")
            Logger.Error(ex)
        End Try

        If resp.IsSuccessStatusCode Then
            If DFcache.ContainsKey(ubi.Uri.ToString) Then
                resC = DFcache(ubi.Uri.ToString)
            Else
                resC = Await resp.Content.ReadAsByteArrayAsync()
                DFcache.Add(ubi.Uri.ToString, resC)
            End If

            If resC.Contains(13) Then Throw New Exception
            Try
                Dim utf8 As System.Text.Encoding = System.Text.Encoding.UTF8
                Me.LastBuilding = utf8.GetString(resC)
                b = Json.JsonSerializer.Deserialize(resC, GetType(Building.Building()))
            Catch ex As Json.JsonException
                Debug.WriteLine(ex.Message)
                Me.LastBuilding &= vbCrLf & ex.Message
                Dim utf8 As System.Text.Encoding = System.Text.Encoding.UTF8
                Clipboard.SetText(utf8.GetString(resC))
            Catch gex As Exception
                Logger.Error(gex)
                Debug.WriteLine(gex.Message)
            End Try


        Else
            Debug.WriteLine($"Failed HTTP request to datafordeler: {resp.ReasonPhrase}")

        End If

        Dim fb1 As Building.FormattedBuilding
        Dim bygS As String
        If IsNothing(Cs) Then Cs = New CS2CS(Logger)
        buildingBuffer.Clear()
        For Each be In b
            fb1 = Building.FormatBuilding(be)
            fb1.OSMCoordinate = Cs.Euref89ToOSM(fb1.byg404Koordinat)
            bygS = $"Bygning {fb1.Bygningsnummer}: {[Enum].GetName(fb1.Anvendelse)} i tilstand {[Enum].GetName(fb1.LivsCyclus)} "
            If buildingBuffer.ContainsKey(bygS) Then
                Logger.Error($"Double building number found at {fb1.Husnummer}: {bygS}")
            Else
                buildingBuffer.Add(bygS, fb1)
            End If

        Next

        Me.lstBygningNummer.Items.Clear()
        For Each sInd In buildingBuffer.Keys
            Me.lstBygningNummer.Items.Add(sInd)
        Next


        Return buildingBuffer.Values.ToArray
    End Function

    Protected Overrides Sub Finalize()
        SaveCache()
        On Error Resume Next
        Cs?.Close()
        MyBase.Finalize()
    End Sub

    Private Sub lstBygningNummer_SelectedIndexChanged(sender As Object, e As EventArgs) Handles lstBygningNummer.SelectedIndexChanged
        Dim fb As Building.FormattedBuilding
        If Me.lstBygningNummer.SelectedIndex > buildingBuffer.Count Then Exit Sub
        fb = buildingBuffer(buildingBuffer.Keys(Me.lstBygningNummer.SelectedIndex))

        Me.lstProperties.Items.Clear()
        Me.lstProperties.Items.Add($"Areal: {fb.Areal} m2")
        Me.lstProperties.Items.Add($"Anvendelse: {[Enum].GetName(fb.Anvendelse)}")
        Me.lstProperties.Items.Add($"Synlige etager: {fb.Levels}")
        Me.lstProperties.Items.Add($"Tag etager: {fb.LevelsInRoof}")
        Me.lstProperties.Items.Add($"Tagmateriale: {[Enum].GetName(fb.Tagdækningsmateriale)}")
        Me.lstProperties.Items.Add($"YdervæggensMateriale: {[Enum].GetName(fb.YdervæggensMateriale)}")
    End Sub

    Private Async Sub cmdTest_Click(sender As Object, e As EventArgs) Handles cmdTest.Click
        Try
            My.Settings.Item("password") = Me.txtPassword.Text
            My.Settings.Item("username") = Me.txtUsername.Text
            My.Settings.Save()
            Dim b = GetBuilding("0a3f5087-3636-32b8-e044-0003ba298018", String.Empty) 'Vordingborg Vor Frue kirke

            Dim bg = Await b

            If IsNothing(b) Then
                MsgBox("No data  :-(",, "www.datafordeler.dk")
            Else
                MsgBox("Success!!",, "www.datafordeler.dk")
            End If

        Catch ex As Exception
            MsgBox(ex.Message)
            Logger.Error(ex)
        End Try


    End Sub
    Public Sub SaveCache()
        Dim f As IO.FileInfo

        Try
            Dim dt As Dictionary(Of String, Byte()) = Me.DFcache
            Dim s As String = IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), System.Reflection.Assembly.GetEntryAssembly.GetName.Name, "DatafordelerCache.xml")
            f = New IO.FileInfo(s)
            Dim e As New System.Xml.Serialization.XmlSerializer(dt.GetType)
            If f.Exists Then f.Delete()
            Dim st As IO.Stream = IO.File.Open(f.ToString, IO.FileMode.Create, IO.FileAccess.ReadWrite)
            e.Serialize(st, dt)
            st.Flush()
            st.Close()
        Catch ex As Exception
            Logger.Error("While saving DF Cache:")
            Logger.Error(ex.Message)
        End Try

    End Sub
    Private Sub LoadCache()
        Dim f As IO.FileInfo
        Dim st As IO.Stream

        Try
            Dim dt As Dictionary(Of String, Byte()) = Me.DFcache
            Dim s As String = IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), System.Reflection.Assembly.GetEntryAssembly.GetName.Name, "DatafordelerCache.xml")
            f = New IO.FileInfo(s)
            Dim e As New System.Xml.Serialization.XmlSerializer(dt.GetType)
            If Not f.Exists Then Exit Sub
            st = IO.File.Open(f.ToString, IO.FileMode.Open, IO.FileAccess.Read)
            dt = e.Deserialize(st)
            st.Close()
            'For Each n In dt
            '    If Not Me.iNodes.ContainsKey(n.Id) Then Me.iNodes.Add(n.Id, n)
            'Next
        Catch ex As Exception
            Logger.Error("While loading Cache:")
            Logger.Error(ex.Message)

            st?.Close()
            If f.Exists Then f.Delete()
        End Try
    End Sub
End Class
