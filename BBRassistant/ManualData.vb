Public Class ManualData
    Public Const KeyRoofMaterial As String = "roof:material"

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Me.RadioButton2.Checked = True
        Me.chkRoofMixed.Checked = True
        Me.chkTarPaperMixed.Checked = True
        Me.ChkRfMixed.Checked = True
    End Sub

    Public Sub AddManualData(ByRef toBuilding As JOSM_Remote.OSMbuildings.OSMbuilding)
        If toBuilding.SDFEbuildings.Count = 0 Then
            'no known BBR buildings
            If chkUnknSingleFloor.Checked Then toBuilding.AddTag("building:levels", 1)
            If chkUnkShed.Checked Then toBuilding.AddTag("building:levels", 1)

        End If
        If toBuilding.TagsToAdd.ContainsKey(KeyRoofMaterial) Then
            If toBuilding.TagsToAdd(KeyRoofMaterial) = "roof_tiles" Then
                If chkRoofBlack.Checked Then toBuilding.AddTag("roof:colour", "black")
                If Me.chkRoofGray.Checked Then toBuilding.AddTag("roof:colour", "slategray")
                If Me.chkRoofOrange.Checked Then toBuilding.AddTag("roof:colour", "sandybrown")
            End If
            If toBuilding.TagsToAdd(KeyRoofMaterial) = "tar_paper" Then
                If Me.chkTarPaperGray.Checked Then toBuilding.AddTag("roof:colour", "slategray")

            End If
            If toBuilding.TagsToAdd(KeyRoofMaterial) = "eternit" Then
                If Me.chkEternitGray.Checked Then toBuilding.AddTag("roof:colour", "slategray")
                If Me.chkEternitBrown.Checked Then toBuilding.AddTag("roof:colour", "saddlebrown")
            End If

            'set roof shape
            If toBuilding.TagsToAdd(KeyRoofMaterial) = "roof_tiles" Then
                If Me.chkRfGabled.Checked Then toBuilding.AddTag("roof:shape", "gabled")
                If Me.chkRfHipped.Checked Then toBuilding.AddTag("roof:shape", "hipped")
                If Me.chkRfHalfHipped.Checked Then toBuilding.AddTag("roof:shape", "half-hipped")
                If Me.chkRfPyramid.Checked Then toBuilding.AddTag("roof:shape", "pyramidal")
                If Me.chkRfRound.Checked Then toBuilding.AddTag("roof:shape", "round")
                If Me.ChkRfSalt.Checked Then toBuilding.AddTag("roof:shape", "saltbox")
            End If

            If toBuilding.TagsToAdd(KeyRoofMaterial) = "tar_paper" Then
                If Me.chkRfGabled.Checked Then toBuilding.AddTag("roof:shape", "gabled")
                If Me.chkRfHipped.Checked Then toBuilding.AddTag("roof:shape", "hipped")
                If Me.chkRfHalfHipped.Checked Then toBuilding.AddTag("roof:shape", "half-hipped")
                If Me.chkRfPyramid.Checked Then toBuilding.AddTag("roof:shape", "pyramidal")
                If Me.chkRfRound.Checked Then toBuilding.AddTag("roof:shape", "round")
                If Me.ChkRfSalt.Checked Then toBuilding.AddTag("roof:shape", "saltbox")
            End If

            If toBuilding.TagsToAdd(KeyRoofMaterial) = "eternit" Then
                If Me.chkRfGabled.Checked Then toBuilding.AddTag("roof:shape", "gabled")
                If Me.chkRfHipped.Checked Then toBuilding.AddTag("roof:shape", "hipped")
                If Me.chkRfHalfHipped.Checked Then toBuilding.AddTag("roof:shape", "half-hipped")
                If Me.chkRfPyramid.Checked Then toBuilding.AddTag("roof:shape", "pyramidal")
                If Me.chkRfRound.Checked Then toBuilding.AddTag("roof:shape", "round")
                If Me.ChkRfSalt.Checked Then toBuilding.AddTag("roof:shape", "saltbox")
            End If



        End If


    End Sub



    Private Sub ChkRfMixed_CheckedChanged(sender As Object, e As EventArgs) Handles ChkRfMixed.CheckedChanged
        Me.chkRfAlsoMetal.Enabled = Not Me.chkRoofMixed.Checked
    End Sub

    Private Sub chkRfGabled_CheckedChanged(sender As Object, e As EventArgs) Handles chkRfGabled.CheckedChanged
        Me.chkRfAlsoMetal.Enabled = Not Me.chkRoofMixed.Checked
    End Sub

    Private Sub chkRfHipped_CheckedChanged(sender As Object, e As EventArgs) Handles chkRfHipped.CheckedChanged
        Me.chkRfAlsoMetal.Enabled = Not Me.chkRoofMixed.Checked
    End Sub

    Private Sub chkRfPyramid_CheckedChanged(sender As Object, e As EventArgs) Handles chkRfPyramid.CheckedChanged
        Me.chkRfAlsoMetal.Enabled = Not Me.chkRoofMixed.Checked
    End Sub
End Class
