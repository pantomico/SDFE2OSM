Public Class RC
    Private WithEvents CLPwatcher As New Timer
    ''' <summary>
    '''  large cache Of nodes, Not all are needed In current operation
    ''' </summary>

    Private iNodes As New Collections.Generic.Dictionary(Of UInt64, OsmSharp.Node)
    Private iWays As New Collections.Generic.Dictionary(Of UInt64, OsmSharp.Complete.CompleteWay)
    Private iRelations As New Dictionary(Of UInt64, OsmSharp.Relation)
    Public Event ClipBoardUpdated()
    Private ReadOnly clientFactory As OsmSharp.IO.API.ClientsFactory
    Private client As OsmSharp.IO.API.NonAuthClient
    ''' <summary>
    ''' list of node id's to use in current operation
    ''' </summary>
    Private Nids As New Collection
    ''' <summary>
    ''' list of way id's to use in current operation
    ''' </summary>
    Private Wids As New Collection
    ''' <summary>
    ''' list of relation id's to use in current operation
    ''' </summary>
    Private Rids As New Collection
    Private JOSM As Net.Http.HttpClient
    Private TagCollection As Dictionary(Of String, String)
    Public FindPrimaryNodes As Task
    Public FindNodesInWays As Task
    Private CachedNodes As Dictionary(Of Int64, OsmSharp.Node)
    Private Logger As NLog.Logger

    Public ReadOnly Property Nodes As Collections.Generic.Dictionary(Of UInt64, OsmSharp.Node)
        Get
            Dim n As New Dictionary(Of UInt64, OsmSharp.Node)
            For Each i In Nids
                If iNodes.ContainsKey(i) Then n.Add(i, iNodes(i))
            Next
            Return n
        End Get
    End Property
    Public ReadOnly Property Ways As Collections.Generic.Dictionary(Of UInt64, OsmSharp.Complete.CompleteWay)
        Get
            Dim n As New Dictionary(Of UInt64, OsmSharp.Complete.CompleteWay)
            For Each i In Wids
                If iWays.ContainsKey(i) Then n.Add(i, iWays(i))
            Next
            Return n
        End Get
    End Property


    Public Sub New(Logger As NLog.Logger)
        Me.Logger = Logger

        CLPwatcher.Interval = 500
        CLPwatcher.Start()
        '   Create a client factoryto connect to OSM directly
        'clientFactory = New OsmSharp.IO.API.ClientsFactory(Nothing, New System.Net.Http.HttpClient(), "https://master.apis.dev.openstreetmap.org/api/")
        clientFactory = New OsmSharp.IO.API.ClientsFactory(Nothing, New System.Net.Http.HttpClient(), "https://api.openstreetmap.org/api/")

        client = clientFactory.CreateNonAuthClient
        'Create connection to JOSM on Localhost
        JOSM = New Net.Http.HttpClient()
        JOSM.BaseAddress = New Uri("http://127.0.0.1:8111/")

        LoadCache()
    End Sub
    ''' <summary>
    ''' See if any new JOSM data has shown up on the Windows Clipboard.  Shoot event if it does.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub CLPwatcher_Tick(sender As Object, e As EventArgs) Handles CLPwatcher.Tick
        Static LastClip As String = String.Empty
        Dim found As Boolean = False
        If Not (Clipboard.ContainsText AndAlso Clipboard.GetText <> LastClip) Then Exit Sub
        'user has copied!


        LastClip = Clipboard.GetText
        Logger.Info($"User copied data: {LastClip.Replace(vbCrLf, "|")}")
        Dim lines() As String = LastClip.Split(vbCrLf)
        Nids.Clear()
        Wids.Clear()
        Rids.Clear()
        TagCollection?.Clear()

        For Each line In lines
            If String.IsNullOrWhiteSpace(line) Then Continue For
            If line.StartsWith("node") Then
                Nids.Add(Val(line.Substring(4)))
                found = True
            End If
            If line.StartsWith("way") Then
                Wids.Add(Val(line.Substring(4)))
                found = True
            End If
            If line.StartsWith("relation") Then
                Rids.Add(Val(line.Substring(4)))
                found = True
            End If
        Next
        'if you download just a few ways, you probably messed with them, so delete from buffer
        If Wids.Count < 10 Then
            For Each w In Wids
                If iWays.ContainsKey(w) Then
                    iWays.Remove(w)
                End If
            Next

        End If
            If found Then
            DownloadNewData()

        End If
    End Sub
    ''' <summary>
    ''' Downloads a fresh dataset and raises clipboard event
    ''' </summary>
    Private Async Sub DownloadNewData()
        Await LoadPrimaryNodes()
        If iNodes.Count = 0 Then Logger.Error("No primary nodes downloaded!")
        'FindNodesInWays = Task.Factory.StartNew(AddressOf LoadAllNodes)
        FindNodesInWays = Task.Run(AddressOf LoadAllNodes)
        Logger.Info("RaiseEvent ClipBoardUpdated")
        If iNodes.Count = 0 Then Exit Sub
        RaiseEvent ClipBoardUpdated()

    End Sub
    Private Async Function LoadPrimaryNodes() As Task
        Try

            For Each n In Nids
                If Not iNodes.ContainsKey(n) Then iNodes.Add(n, Await client.GetNode(n))
                Debug.Write(".")
            Next

            Logger.Info($"downloaded {iNodes.Count} primary nodes")
            Me.SaveCache()
        Catch ex As Exception
            Logger.Error("Fail while LoadPrimaryNodes")
            Logger.Error(ex.Message)
        End Try




        Return
    End Function

    ''' <summary>
    ''' Download ways and relations
    ''' </summary>
    Private Async Function LoadAllNodes() As Task
        Dim sw As New OsmSharp.Complete.CompleteWay
        Try
            Logger.Info($"Loading extra nodes async.")
            For Each w In Wids
                If Not iWays.ContainsKey(w) Then
                    sw = Await client.GetCompleteWay(w)
                End If
                If sw?.Nodes?.Length > 0 Then
                    iWays.Add(w, sw)
                    Debug.Write(".")
                Else
                    Logger.Error($"OSM Download went wrong on way {w}")
                End If

            Next
            For Each r In Rids
                If Not iRelations.ContainsKey(r) Then iRelations.Add(r, Await client.GetRelation(r))
                Debug.Write(".")
            Next
            Logger.Info($"Loaded extra {iWays.Count } ways async.")
            Return
        Catch ex As Exception
            Logger.Error("Fail while LoadAllNodes")
            Logger.Error(ex.Message)
        End Try


    End Function



    Public Async Sub SelectWay(Bygning As OSMbuildings.OSMbuilding)
        Dim way As New OsmSharp.Complete.CompleteWay
        Dim node As OsmSharp.Node
        Dim MinLat As Double = Double.MaxValue
        Dim MaxLat As Double = Double.MinValue
        Dim MinLon As Double = Double.MaxValue
        Dim MaxLon As Double = Double.MinValue
        Dim Req As New UriBuilder
        Dim sb As New Text.StringBuilder
        Dim Resp As Net.Http.HttpResponseMessage

        way = Bygning.way
        If way.Nodes?.Length > 0 Then

            For Each node In way.Nodes
                If node.Latitude < MinLat Then MinLat = node.Latitude
                If node.Latitude > MaxLat Then MaxLat = node.Latitude
                If node.Longitude < MinLon Then MinLon = node.Longitude
                If node.Longitude > MaxLon Then MaxLon = node.Longitude
            Next

            If Double.IsNegativeInfinity(MinLat) Or (MinLat = Double.MinValue) Then
                Logger.Error("Zoombox cannot be defined")
            End If
            'expand download box
            MinLat -= (MaxLat - MinLat)
            MaxLat += (MaxLat - MinLat)
            MinLon -= (MaxLon - MinLon)
            MaxLon += (MaxLon - MinLon)
            'force decimal points
            Application.CurrentCulture = New System.Globalization.CultureInfo("en-US")
            'select area and way
            Req.Path = "/load_and_zoom"
            sb.Append($"left={MinLon}&right={MaxLon}&top={MaxLat}&bottom={MinLat}")
            'select
            sb.Append($"&select=way{way.Id}")
            'addtags
            sb.Append($"&addtags=")
            For Each T In Bygning.TagsToAdd
                sb.Append($"{T.Key}={T.Value}|")
            Next
            sb.Append("ENDOFTAGS")
            Req.Query = sb.ToString.Replace("|ENDOFTAGS", String.Empty) 'remove trailing |
            Req.Port = 8111
            Try
                Resp = Await JOSM.GetAsync(Req.Uri)
                If Not Resp.IsSuccessStatusCode Then
                    Logger.Error($"Call to JOSM remote: {Resp.ReasonPhrase}")
                    Logger.Error(Req.Uri.ToString)

                End If
            Catch nex As Net.Http.HttpRequestException
                Logger.Error("HttpRequestException sending command to JOSM:")
                Logger.Error(nex.Message)
            Catch tex As TaskCanceledException
                Logger.Error("TaskCanceledException sending command to JOSM:")
                Logger.Error(tex.Message)
            Catch ex As Exception
                Logger.Error("Error sending command to JOSM:")
                Logger.Error(ex.Message)
            End Try

        Else
            Logger.Error("Way not found")
        End If
    End Sub
    Private Sub SaveCache()
        Dim f As IO.FileInfo

        Try
            Dim dt() As OsmSharp.Node = Me.iNodes.Values.ToArray
            Dim s As String = IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), System.Reflection.Assembly.GetEntryAssembly.GetName.Name, "OSMnodesCache.xml")
            f = New IO.FileInfo(s)
            Dim e As New System.Xml.Serialization.XmlSerializer(dt.GetType)
            If f.Exists Then f.Delete()
            Dim st As IO.Stream = IO.File.Open(f.ToString, IO.FileMode.Create, IO.FileAccess.ReadWrite)
            e.Serialize(st, dt)
            st.Flush()
            st.Close()
        Catch ex As Exception
            Logger.Error("While saving Cache:")
            Logger.Error(ex.Message)
        End Try

    End Sub
    Private Sub LoadCache()
        Dim f As IO.FileInfo
        Dim st As IO.Stream

        Try
            Dim dt() As OsmSharp.Node = Me.iNodes.Values.ToArray
            Dim s As String = IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), System.Reflection.Assembly.GetEntryAssembly.GetName.Name, "OSMnodesCache.xml")
            f = New IO.FileInfo(s)
            Dim e As New System.Xml.Serialization.XmlSerializer(dt.GetType)
            If Not f.Exists Then Exit Sub
            st = IO.File.Open(f.ToString, IO.FileMode.Open, IO.FileAccess.Read)
            dt = e.Deserialize(st)
            st.Close()
            For Each n In dt
                If Not Me.iNodes.ContainsKey(n.Id) Then Me.iNodes.Add(n.Id, n)
            Next
        Catch ex As Exception
            Logger.Error("While loading Cache:")
            Logger.Error(ex.Message)

            st?.Close()
            If f.Exists Then f.Delete()
        End Try

    End Sub
End Class
