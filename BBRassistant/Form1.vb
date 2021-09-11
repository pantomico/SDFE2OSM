Imports OsmSharp

Public Class Form1
    Private WithEvents RC As JOSM_Remote.RC
    Public Logger As DataFordeler.Nlogger

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Logger = New DataFordeler.Nlogger(logDebug:=True)
        ' DF = New DataFordeler.DataFordeler(Logger.Logger)
        Logger.Logger.Info("Starting app")
        Debug.WriteLine("Starting DataFordeler")
        Me.DatafDgui1.SetLogger(Logger.Logger)
        Logger.Logger.Info("Starting JOSM remote")
        RC = New JOSM_Remote.RC(Logger.Logger)
    End Sub

    Private Async Sub RC_ClipBoardUpdated() Handles RC.ClipBoardUpdated

        Dim s As String
        Dim BuildingSorter As New JOSM_Remote.OSMbuildings
        Dim FoundBuildings() As DataFordeler.Building.FormattedBuilding
        Dim osakFound As Boolean = False
        Dim nd As New Node
        Dim ToDoTag As String = String.Empty
        Logger.Logger.Info("Primary nodes found")
        For Each n In RC.Nodes
            If n.Value.Tags?.ContainsKey("osak:identifier") Then
                osakFound = True
                nd = n.Value
                s = nd.Tags("osak:identifier")
                Me.lstProgress.Items.Add($"Husnummer: {s}")
                While Me.lstProgress.Items.Count > 10
                    Me.lstProgress.Items.RemoveAt(1)
                End While
                FoundBuildings = Await Me.DatafDgui1.GetBuilding(s, String.Empty)

                For Each fb In FoundBuildings
                    BuildingSorter.AddSDFEbuilding(fb)
                Next
            End If
        Next
        Me.DatafDgui1.SaveCache()
        'download all way nodes so you can do localisation
        Logger.Logger.Info("Waiting for data...")
        'make sure all data is downloaded
        If RC.FindNodesInWays.Wait(New TimeSpan(0, 3, 0)) Then
            Logger.Logger.Info("nodes are downloaded ")
        Else
            Logger.Logger.Error("nodes not downloaded after 3 minutes")
            Exit Sub
        End If
        Debug.WriteLine("...task finished.")
        For Each w In RC.Ways
            BuildingSorter.AddOSMbuilding(w.Value)
        Next
        BuildingSorter.Sort()

        For Each Building In BuildingSorter.OSMBuildings
            If Not Building.way.Tags.ContainsKey("building") Then Continue For
            Building.SetAutomaticTags()
            Me.ManualData1.AddManualData(Building)
            RC.SelectWay(Building)
        Next
        If osakFound Then
            Debug.WriteLine("osak:identifier found")
        Else
            Debug.WriteLine("osak:identifier not found")
        End If

    End Sub

    Private Sub chkTodo_CheckedChanged(sender As Object, e As EventArgs) 

    End Sub

    Private Sub TabPage2_Click(sender As Object, e As EventArgs) Handles TabPage2.Click

    End Sub
End Class
