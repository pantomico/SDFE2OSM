<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
        Me.lstProgress = New System.Windows.Forms.ListBox()
        Me.lstWarning = New System.Windows.Forms.ListBox()
        Me.TabControl1 = New System.Windows.Forms.TabControl()
        Me.TabPage1 = New System.Windows.Forms.TabPage()
        Me.TabPage2 = New System.Windows.Forms.TabPage()
        Me.DatafDgui1 = New DataFordeler.DataFDgui()
        Me.TabPage3 = New System.Windows.Forms.TabPage()
        Me.ManualData1 = New BBRassistant.ManualData()
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        Me.TabControl1.SuspendLayout()
        Me.TabPage1.SuspendLayout()
        Me.TabPage2.SuspendLayout()
        Me.TabPage3.SuspendLayout()
        Me.SuspendLayout()
        '
        'SplitContainer1
        '
        Me.SplitContainer1.Cursor = System.Windows.Forms.Cursors.HSplit
        Me.SplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer1.Location = New System.Drawing.Point(4, 4)
        Me.SplitContainer1.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.SplitContainer1.Name = "SplitContainer1"
        Me.SplitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'SplitContainer1.Panel1
        '
        Me.SplitContainer1.Panel1.Controls.Add(Me.lstProgress)
        '
        'SplitContainer1.Panel2
        '
        Me.SplitContainer1.Panel2.Controls.Add(Me.lstWarning)
        Me.SplitContainer1.Size = New System.Drawing.Size(1726, 1077)
        Me.SplitContainer1.SplitterDistance = 578
        Me.SplitContainer1.SplitterWidth = 6
        Me.SplitContainer1.TabIndex = 3
        '
        'lstProgress
        '
        Me.lstProgress.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lstProgress.FormattingEnabled = True
        Me.lstProgress.ItemHeight = 30
        Me.lstProgress.Location = New System.Drawing.Point(0, 0)
        Me.lstProgress.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.lstProgress.Name = "lstProgress"
        Me.lstProgress.Size = New System.Drawing.Size(1726, 578)
        Me.lstProgress.TabIndex = 0
        '
        'lstWarning
        '
        Me.lstWarning.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lstWarning.FormattingEnabled = True
        Me.lstWarning.ItemHeight = 30
        Me.lstWarning.Location = New System.Drawing.Point(0, 0)
        Me.lstWarning.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.lstWarning.Name = "lstWarning"
        Me.lstWarning.Size = New System.Drawing.Size(1726, 493)
        Me.lstWarning.TabIndex = 0
        '
        'TabControl1
        '
        Me.TabControl1.Controls.Add(Me.TabPage1)
        Me.TabControl1.Controls.Add(Me.TabPage2)
        Me.TabControl1.Controls.Add(Me.TabPage3)
        Me.TabControl1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TabControl1.Location = New System.Drawing.Point(0, 0)
        Me.TabControl1.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.TabControl1.Name = "TabControl1"
        Me.TabControl1.SelectedIndex = 0
        Me.TabControl1.Size = New System.Drawing.Size(1742, 1128)
        Me.TabControl1.TabIndex = 4
        '
        'TabPage1
        '
        Me.TabPage1.Controls.Add(Me.SplitContainer1)
        Me.TabPage1.Location = New System.Drawing.Point(4, 39)
        Me.TabPage1.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.TabPage1.Name = "TabPage1"
        Me.TabPage1.Padding = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.TabPage1.Size = New System.Drawing.Size(1734, 1085)
        Me.TabPage1.TabIndex = 0
        Me.TabPage1.Text = "Log"
        Me.TabPage1.UseVisualStyleBackColor = True
        '
        'TabPage2
        '
        Me.TabPage2.Controls.Add(Me.DatafDgui1)
        Me.TabPage2.Location = New System.Drawing.Point(4, 39)
        Me.TabPage2.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.TabPage2.Name = "TabPage2"
        Me.TabPage2.Padding = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.TabPage2.Size = New System.Drawing.Size(1734, 1085)
        Me.TabPage2.TabIndex = 1
        Me.TabPage2.Text = "Datafordeler.dk"
        Me.TabPage2.UseVisualStyleBackColor = True
        '
        'DatafDgui1
        '
        Me.DatafDgui1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.DatafDgui1.Location = New System.Drawing.Point(4, 4)
        Me.DatafDgui1.Margin = New System.Windows.Forms.Padding(6, 6, 6, 6)
        Me.DatafDgui1.Name = "DatafDgui1"
        Me.DatafDgui1.Size = New System.Drawing.Size(1726, 1077)
        Me.DatafDgui1.TabIndex = 0
        '
        'TabPage3
        '
        Me.TabPage3.Controls.Add(Me.ManualData1)
        Me.TabPage3.Location = New System.Drawing.Point(4, 39)
        Me.TabPage3.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.TabPage3.Name = "TabPage3"
        Me.TabPage3.Padding = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.TabPage3.Size = New System.Drawing.Size(1734, 1085)
        Me.TabPage3.TabIndex = 2
        Me.TabPage3.Text = "Sæt 'i hånden'"
        Me.TabPage3.UseVisualStyleBackColor = True
        '
        'ManualData1
        '
        Me.ManualData1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ManualData1.Location = New System.Drawing.Point(4, 4)
        Me.ManualData1.Margin = New System.Windows.Forms.Padding(6, 6, 6, 6)
        Me.ManualData1.Name = "ManualData1"
        Me.ManualData1.Size = New System.Drawing.Size(1726, 1077)
        Me.ManualData1.TabIndex = 0
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(12.0!, 30.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1742, 1128)
        Me.Controls.Add(Me.TabControl1)
        Me.Name = "Form1"
        Me.Text = "SDFE 2 OSM"
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer1.ResumeLayout(False)
        Me.TabControl1.ResumeLayout(False)
        Me.TabPage1.ResumeLayout(False)
        Me.TabPage2.ResumeLayout(False)
        Me.TabPage3.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents SplitContainer1 As SplitContainer
    Friend WithEvents lstProgress As ListBox
    Friend WithEvents lstWarning As ListBox
    Friend WithEvents TabControl1 As TabControl
    Friend WithEvents TabPage1 As TabPage
    Friend WithEvents TabPage2 As TabPage
    Friend WithEvents DatafDgui1 As DataFordeler.DataFDgui
    Friend WithEvents TabPage3 As TabPage
    Friend WithEvents ManualData1 As ManualData
End Class
