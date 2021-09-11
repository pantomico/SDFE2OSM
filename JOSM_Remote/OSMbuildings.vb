Public Class OSMbuildings
    Public OSMBuildings As Collections.Generic.List(Of OSMbuildings.OSMbuilding)
    Private SDFEbuildings As List(Of DataFordeler.Building.FormattedBuilding)

    ''' <summary>
    ''' A class for matching OSM buildings to their SDDFE counterparts
    ''' </summary>
    Public Sub New()
        OSMBuildings = New List(Of OSMbuilding)
        SDFEbuildings = New List(Of DataFordeler.Building.FormattedBuilding)

    End Sub


    Public Sub AddOSMbuilding(building As OsmSharp.Complete.CompleteWay)
        Dim b As New OSMbuilding With {.way = building}
        If building.Nodes.Count > 0 Then
            OSMBuildings.Add(b)
            Debug.WriteLine($"Adding OSM building {building.Id}")
        Else
            Debug.WriteLine($"Not adding OSM building {building.Id} because it contains no nodes.")
        End If


    End Sub

    Public Sub AddSDFEbuilding(building As DataFordeler.Building.FormattedBuilding)
        SDFEbuildings.Add(building)
        Debug.WriteLine($"Adding SDFE building {building.Bygningsnummer} with area {building.Areal} ({building.OSMbuilding}) house {building.Husnummer}")
    End Sub
    ''' <summary>
    ''' matches SDFE buildings with their OSM counterparts
    ''' </summary>
    Public Sub Sort()
        Try
            For Each ob In OSMBuildings
                For Each sb In SDFEbuildings
                    If Mathfunctions.IsNodeInWay(sb.OSMCoordinate, ob.way) Then
                        If Not ob.SDFEbuildings.ContainsKey(sb.Husnummer) Then
                            ob.SDFEbuildings.Add(sb.Husnummer, New List(Of DataFordeler.Building.FormattedBuilding))
                        End If
                        ob.SDFEbuildings(sb.Husnummer).Add(sb)
                    End If
                Next
            Next
        Catch ex As Exception
            Debug.WriteLine(ex.Message)
        End Try

    End Sub

    ''' <summary>
    ''' An OSM way which can contain 0 or more SDFE buildings
    ''' </summary>
    Public Class OSMbuilding
        ''' <summary>
        ''' OSM building way with all nodes
        ''' </summary>
        Public way As OsmSharp.Complete.CompleteWay
        Public SDFEbuildings As Dictionary(Of String, List(Of DataFordeler.Building.FormattedBuilding))
        Public TagsToAdd As Dictionary(Of String, String)

        Public Sub New()
            SDFEbuildings = New Dictionary(Of String, List(Of DataFordeler.Building.FormattedBuilding))
            TagsToAdd = New Dictionary(Of String, String)
        End Sub

        Private ReadOnly Property BygningCount As Integer
            Get
                Dim i As Integer = 0
                For Each h In Me.SDFEbuildings
                    i += h.Value.Count
                Next
                Return i
            End Get
        End Property
        Private ReadOnly Property BiggestBuilding As DataFordeler.Building.FormattedBuilding
            Get
                Dim b As New DataFordeler.Building.FormattedBuilding With {.Areal = 0}
                If Me.BygningCount = 0 Then Return b
                If Me.BygningCount = 1 Then
                    For Each h In Me.SDFEbuildings
                        For Each bg In h.Value
                            b = bg
                        Next
                    Next
                End If
                For Each h In Me.SDFEbuildings
                    For Each bg In h.Value
                        If bg.IsStandingNow And bg.Areal > b.Areal Then b = bg
                    Next
                Next

                Return b
            End Get
        End Property
        ''' <summary>
        ''' Do all buildings have the same characteristics?
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Homogenous As Boolean
            Get
                If Me.BygningCount < 2 Then Return True

                Dim r As Boolean = True
                Dim floors As Integer = Me.SDFEbuildings(0)(0).Levels
                Dim rfloors As Integer = Me.SDFEbuildings(0)(0).LevelsInRoof
                Dim rmat As String = Me.SDFEbuildings(0)(0).OSMroofMaterial
                Dim desc As String = Me.SDFEbuildings(0)(0).OSMbuilding
                For Each h In Me.SDFEbuildings
                    For Each b In h.Value
                        If Not String.IsNullOrWhiteSpace(b.OSMroofMaterial) Then
                            rmat = b.OSMroofMaterial
                            Exit For
                        End If
                    Next
                Next

                For Each h In Me.SDFEbuildings
                    For Each b In h.Value
                        If Not String.IsNullOrWhiteSpace(b.OSMroofMaterial) Then
                            rmat = b.OSMroofMaterial
                            Exit For
                        End If
                    Next
                Next
                For Each h In Me.SDFEbuildings
                    For Each b In h.Value
                        If (b.OSMroofMaterial <> rmat) AndAlso (Not String.IsNullOrWhiteSpace(b.OSMroofMaterial)) Then r = False
                        If b.Levels <> floors Then r = False
                        If b.LevelsInRoof <> rfloors Then r = False
                        If b.OSMbuilding <> desc Then r = False
                    Next
                Next
                Return r
            End Get
        End Property
        Public ReadOnly Property IsDemolished As Boolean
            Get
                Dim r As Boolean = True
                If Me.SDFEbuildings.Count < 1 Then Return False
                For Each h In Me.SDFEbuildings
                    For Each b In h.Value
                        If b.IsStandingNow Then r = False
                    Next
                Next
                If r Then
                    Debug.WriteLine("demolished")
                End If
                Return r
            End Get
        End Property
        Public ReadOnly Property LastChange As Date
            Get
                Dim d As Date = Date.MinValue
                If Me.SDFEbuildings.Count < 1 Then Return d
                For Each h In Me.SDFEbuildings
                    For Each b In h.Value
                        If b.ValidityDate > d Then d = b.ValidityDate
                    Next
                Next
                Return d
            End Get
        End Property
        ''' <summary>
        ''' Return roof Material all existing buildings have in common
        ''' </summary>
        ''' <param name="Levels"></param>
        ''' <returns>false if buildings are different Material</returns>
        Private Function TryGetCommonRoofMaterial(ByRef Material As String) As Boolean
            Dim m As String = String.Empty
            If Me.SDFEbuildings.Count < 1 Then Return False
            'find first level
            For Each h In Me.SDFEbuildings
                For Each b In h.Value
                    If b.LivsCyclus = DataFordeler.Building.LivsCyclus.Opført Then
                        If Not String.IsNullOrEmpty(b.OSMroofMaterial) Then m = b.OSMroofMaterial
                    End If
                Next
            Next
            If String.IsNullOrEmpty(m) Then
                For Each h In Me.SDFEbuildings
                    For Each b In h.Value
                        If b.LivsCyclus = DataFordeler.Building.LivsCyclus.UnderOpførsel Then
                            If Not String.IsNullOrEmpty(b.OSMroofMaterial) Then m = b.OSMroofMaterial
                        End If
                    Next
                Next
            End If
            If String.IsNullOrEmpty(m) Then
                For Each h In Me.SDFEbuildings
                    For Each b In h.Value
                        If Not String.IsNullOrEmpty(b.OSMroofMaterial) Then m = b.OSMroofMaterial
                    Next
                Next
            End If

            'now compare to others
            For Each h In Me.SDFEbuildings
                For Each b In h.Value
                    If b.LivsCyclus = DataFordeler.Building.LivsCyclus.Opført Then
                        If b.OSMroofMaterial <> m Then Return False
                    End If
                Next
            Next
            'al roofs have same Material
            Material = m
            Return True
        End Function

        ''' <summary>
        ''' Return roof Material all existing buildings have in common
        ''' </summary>
        ''' <param name="Levels"></param>
        ''' <returns>false if buildings are different Material</returns>
        Private Function TryGetCommonMaterial(ByRef Material As String) As Boolean
            Dim m As String = String.Empty
            If Me.SDFEbuildings.Count < 1 Then Return False
            'find first level
            For Each h In Me.SDFEbuildings
                For Each b In h.Value
                    If b.LivsCyclus = DataFordeler.Building.LivsCyclus.Opført Then
                        If Not String.IsNullOrEmpty(b.OSMbuildingMaterial) Then m = b.OSMbuildingMaterial
                    End If
                Next
            Next
            If String.IsNullOrEmpty(m) Then
                For Each h In Me.SDFEbuildings
                    For Each b In h.Value
                        If b.LivsCyclus = DataFordeler.Building.LivsCyclus.UnderOpførsel Then
                            If Not String.IsNullOrEmpty(b.OSMbuildingMaterial) Then m = b.OSMbuildingMaterial
                        End If
                    Next
                Next
            End If
            If String.IsNullOrEmpty(m) Then
                For Each h In Me.SDFEbuildings
                    For Each b In h.Value
                        If Not String.IsNullOrEmpty(b.OSMbuildingMaterial) Then m = b.OSMbuildingMaterial
                    Next
                Next
            End If

            'now compare to others
            For Each h In Me.SDFEbuildings
                For Each b In h.Value
                    If b.LivsCyclus = DataFordeler.Building.LivsCyclus.Udført Then
                        If b.OSMbuildingMaterial <> m Then Return False
                    End If
                Next
            Next
            'al roofs have same Material
            Material = m
            Return True
        End Function
        ''' <summary>
        ''' Return the number of levels all existing buildings have in common
        ''' </summary>
        ''' <param name="Levels"></param>
        ''' <returns>false if buildings are different height</returns>
        Private Function TryGetCommonLevels(ByRef Levels As Integer) As Boolean
            Dim l As Integer = -1
            If Me.SDFEbuildings.Count < 1 Then Return False
            'find first level
            For Each h In Me.SDFEbuildings
                For Each b In h.Value
                    If b.LivsCyclus = DataFordeler.Building.LivsCyclus.Opført Then
                        If b.Levels > l Then l = b.Levels
                    End If
                Next
            Next
            If l = -1 Then
                For Each h In Me.SDFEbuildings
                    For Each b In h.Value
                        If b.LivsCyclus = DataFordeler.Building.LivsCyclus.UnderOpførsel Then
                            If b.Levels > l Then l = b.Levels
                        End If
                    Next
                Next
            End If
            If l = -1 Then
                For Each h In Me.SDFEbuildings
                    For Each b In h.Value
                        If b.Levels > l Then l = b.Levels
                    Next
                Next
            End If
            If l < 1 Then l = 1
            'now compare to others
            For Each h In Me.SDFEbuildings
                For Each b In h.Value
                    If b.LivsCyclus = DataFordeler.Building.LivsCyclus.Opført Then
                        If (b.Levels > 0) And (b.Levels <> l) Then Return False
                    End If
                Next
            Next
            'al roofs have same height
            Levels = l
            Return True
        End Function
        ''' <summary>
        ''' Return the number of roof levels all existing buildings have in common
        ''' </summary>
        ''' <param name="Levels"></param>
        ''' <returns>false if buildings are different height</returns>
        Private Function TryGetCommonRoofLevels(ByRef Levels As Integer) As Boolean
            Dim l As Integer = -1
            If Me.SDFEbuildings.Count < 1 Then Return False
            'find first level
            For Each h In Me.SDFEbuildings
                For Each b In h.Value
                    If b.LivsCyclus = DataFordeler.Building.LivsCyclus.Opført Then
                        If b.LevelsInRoof > l Then l = b.LevelsInRoof
                    End If
                Next
            Next
            If l = -1 Then
                For Each h In Me.SDFEbuildings
                    For Each b In h.Value
                        If b.LivsCyclus = DataFordeler.Building.LivsCyclus.UnderOpførsel Then
                            If b.LevelsInRoof > l Then l = b.LevelsInRoof
                        End If
                    Next
                Next
            End If
            If l = -1 Then
                For Each h In Me.SDFEbuildings
                    For Each b In h.Value
                        If b.LevelsInRoof > l Then l = b.LevelsInRoof
                    Next
                Next
            End If
            If l < 0 Then l = 0
            'now compare to others
            For Each h In Me.SDFEbuildings
                For Each b In h.Value
                    If b.LivsCyclus = DataFordeler.Building.LivsCyclus.Opført Then
                        If (b.LevelsInRoof <> l) Then Return False
                    End If
                Next
            Next
            'al roofs have same height
            Levels = l
            Return True
        End Function
        ''' <summary>
        ''' Returns building=* tag for biggest building
        ''' Returns "yes" if it is not at least 3 times as big as everything else
        ''' </summary>
        ''' <returns></returns>
        Private Function TryGetCommonPurpose() As String
            'find first purp
            Dim bui As String = Me.BiggestBuilding.OSMbuilding
            Dim sur As Integer = Me.BiggestBuilding.Areal
            For Each h In Me.SDFEbuildings
                For Each b In h.Value
                    If b.LivsCyclus = DataFordeler.Building.LivsCyclus.Opført Then
                        If ((b.Areal * 3) > sur) AndAlso (b.OSMbuilding <> BiggestBuilding.OSMbuilding) Then bui = "yes"
                    End If
                Next
            Next
            Return bui
        End Function


        ''' <summary>
        ''' Loads all the OSM tags which can be set fully automatically
        ''' </summary>
        Public Sub SetAutomaticTags()
            Dim i As Integer
            Dim m As String = String.Empty
            Const BUILDING As String = "building"
            Dim OverWriteTag As Boolean
            'get the tags already there
            If way?.Tags?.Count > 0 Then
                For Each t In way?.Tags
                    AddTag(t.Key, t.Value, overwrite:=True)
                Next
            Else
                way.Tags = New OsmSharp.Tags.TagsCollection
            End If
            If Not TagsToAdd.ContainsKey(BUILDING) Then Throw New Exception
            'try add common properties
            If TryGetCommonLevels(i) Then AddTag("building:levels", i)
            If TryGetCommonRoofLevels(i) Then AddTag("roof:levels", i)
            If TryGetCommonMaterial(m) Then AddTag("building:material", m)
            If TryGetCommonRoofMaterial(m) Then AddTag("roof:material", m)

            'get some data from biggest building

            If Me.SDFEbuildings.Count = 1 Then
                Dim bb = Me.BiggestBuilding
                Dim purpose As String = Me.TryGetCommonPurpose
                'only 1 husnummer
                AddTag("bbr:husnummer", bb.Husnummer)

                OverWriteTag = Not (purpose = "yes")
                If way.Tags(BUILDING) = "yes" Then AddTag("building", purpose, overwrite:=OverWriteTag)
                If way.Tags(BUILDING) = "house" Then AddTag("building", purpose, overwrite:=OverWriteTag)
                If way.Tags(BUILDING) = "residential" Then AddTag("building", purpose, overwrite:=OverWriteTag)
                AddTag(BUILDING, purpose, overwrite:=False)

                If bb.Tagdækningsmateriale = DataFordeler.Building.Tagdækningsmateriale.TagpapMedLilleHældning Then AddTag("roof:shape", "flat")

                If (bb.Opførelsesår > 1200) And (bb.Opførelsesår <= Now.Year) Then
                    AddTag("start_date", bb.Opførelsesår)
                End If
            End If
            If Me.IsDemolished Then AddTag("fixme", $"Verify if demolished before {Me.LastChange.ToString("MMM yyyy")}")


        End Sub
        Public Sub AddTag(name As String, value As String, Optional overwrite As Boolean = False)
            Dim x As Boolean = (way.Tags.ContainsKey(name.ToLower.Trim))
            If String.IsNullOrWhiteSpace(name) Then Throw New ArgumentNullException("name")
            If String.IsNullOrWhiteSpace(value) Then Exit Sub
            If (Not x) Or overwrite Then
                If TagsToAdd.ContainsKey(name) Then
                    TagsToAdd(name) = value
                Else
                    TagsToAdd.Add(name, value)
                End If
            End If
        End Sub
    End Class
End Class
