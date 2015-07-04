namespace FileSyncUI
{
  partial class MainForm
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.components = new System.ComponentModel.Container();
      this.toolStripMain = new System.Windows.Forms.ToolStrip();
      this.toolStripButtonNew = new System.Windows.Forms.ToolStripButton();
      this.toolStripButtonSave = new System.Windows.Forms.ToolStripButton();
      this.toolStripButtonSync = new System.Windows.Forms.ToolStripButton();
      this.toolStripButtonCancel = new System.Windows.Forms.ToolStripButton();
      this.toolStripButtonExit = new System.Windows.Forms.ToolStripButton();
      this.statusStripMain = new System.Windows.Forms.StatusStrip();
      this.toolStripProgressBar = new System.Windows.Forms.ToolStripProgressBar();
      this.toolStripStatusLabelMain = new System.Windows.Forms.ToolStripStatusLabel();
      this.gridViewWatches = new System.Windows.Forms.DataGridView();
      this.nameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.sourcePathDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.destinationPathDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.includeSubFoldersDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
      this.lastSyncDateDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.watchListBindingSource = new System.Windows.Forms.BindingSource(this.components);
      this.gridViewExcludeFolders = new System.Windows.Forms.DataGridView();
      this.excludeFolderNameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.toolStripMain.SuspendLayout();
      this.statusStripMain.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.gridViewWatches)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.watchListBindingSource)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.gridViewExcludeFolders)).BeginInit();
      this.SuspendLayout();
      // 
      // toolStripMain
      // 
      this.toolStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonNew,
            this.toolStripButtonSave,
            this.toolStripButtonSync,
            this.toolStripButtonCancel,
            this.toolStripButtonExit});
      this.toolStripMain.Location = new System.Drawing.Point(0, 0);
      this.toolStripMain.Name = "toolStripMain";
      this.toolStripMain.Size = new System.Drawing.Size(983, 25);
      this.toolStripMain.TabIndex = 0;
      this.toolStripMain.Text = "toolStripMain";
      // 
      // toolStripButtonNew
      // 
      this.toolStripButtonNew.Image = global::FileSyncUI.Properties.Resources.New;
      this.toolStripButtonNew.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.toolStripButtonNew.Name = "toolStripButtonNew";
      this.toolStripButtonNew.Size = new System.Drawing.Size(88, 22);
      this.toolStripButtonNew.Text = "New Watch";
      this.toolStripButtonNew.ToolTipText = "Adds a new watch to the list";
      this.toolStripButtonNew.Click += new System.EventHandler(this.toolStripButtonNew_Click);
      // 
      // toolStripButtonSave
      // 
      this.toolStripButtonSave.Image = global::FileSyncUI.Properties.Resources.Save;
      this.toolStripButtonSave.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.toolStripButtonSave.Name = "toolStripButtonSave";
      this.toolStripButtonSave.Size = new System.Drawing.Size(51, 22);
      this.toolStripButtonSave.Text = "Save";
      this.toolStripButtonSave.Click += new System.EventHandler(this.toolStripButtonSave_Click);
      // 
      // toolStripButtonSync
      // 
      this.toolStripButtonSync.Image = global::FileSyncUI.Properties.Resources.Sync;
      this.toolStripButtonSync.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.toolStripButtonSync.Name = "toolStripButtonSync";
      this.toolStripButtonSync.Size = new System.Drawing.Size(52, 22);
      this.toolStripButtonSync.Text = "Sync";
      this.toolStripButtonSync.ToolTipText = "Synchronise the watchlist";
      this.toolStripButtonSync.Click += new System.EventHandler(this.toolStripButtonSync_Click);
      // 
      // toolStripButtonCancel
      // 
      this.toolStripButtonCancel.Enabled = false;
      this.toolStripButtonCancel.Image = global::FileSyncUI.Properties.Resources.Cancel;
      this.toolStripButtonCancel.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.toolStripButtonCancel.Name = "toolStripButtonCancel";
      this.toolStripButtonCancel.Size = new System.Drawing.Size(63, 22);
      this.toolStripButtonCancel.Text = "Cancel";
      this.toolStripButtonCancel.Click += new System.EventHandler(this.toolStripButtonCancel_Click);
      // 
      // toolStripButtonExit
      // 
      this.toolStripButtonExit.Image = global::FileSyncUI.Properties.Resources.Exit;
      this.toolStripButtonExit.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.toolStripButtonExit.Name = "toolStripButtonExit";
      this.toolStripButtonExit.Size = new System.Drawing.Size(45, 22);
      this.toolStripButtonExit.Text = "Exit";
      this.toolStripButtonExit.ToolTipText = "Exit the application";
      this.toolStripButtonExit.Click += new System.EventHandler(this.toolStripButtonExit_Click);
      // 
      // statusStripMain
      // 
      this.statusStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripProgressBar,
            this.toolStripStatusLabelMain});
      this.statusStripMain.Location = new System.Drawing.Point(0, 561);
      this.statusStripMain.Name = "statusStripMain";
      this.statusStripMain.Size = new System.Drawing.Size(983, 22);
      this.statusStripMain.TabIndex = 1;
      this.statusStripMain.Text = "statusStrip1";
      // 
      // toolStripProgressBar
      // 
      this.toolStripProgressBar.Name = "toolStripProgressBar";
      this.toolStripProgressBar.Size = new System.Drawing.Size(200, 16);
      // 
      // toolStripStatusLabelMain
      // 
      this.toolStripStatusLabelMain.Name = "toolStripStatusLabelMain";
      this.toolStripStatusLabelMain.Size = new System.Drawing.Size(148, 17);
      this.toolStripStatusLabelMain.Text = "The status will display here";
      // 
      // gridViewWatches
      // 
      this.gridViewWatches.AllowUserToAddRows = false;
      this.gridViewWatches.AllowUserToDeleteRows = false;
      this.gridViewWatches.AutoGenerateColumns = false;
      this.gridViewWatches.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.gridViewWatches.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.nameDataGridViewTextBoxColumn,
            this.sourcePathDataGridViewTextBoxColumn,
            this.destinationPathDataGridViewTextBoxColumn,
            this.includeSubFoldersDataGridViewCheckBoxColumn,
            this.lastSyncDateDataGridViewTextBoxColumn});
      this.gridViewWatches.DataSource = this.watchListBindingSource;
      this.gridViewWatches.Location = new System.Drawing.Point(0, 28);
      this.gridViewWatches.Name = "gridViewWatches";
      this.gridViewWatches.Size = new System.Drawing.Size(545, 530);
      this.gridViewWatches.TabIndex = 2;
      // 
      // nameDataGridViewTextBoxColumn
      // 
      this.nameDataGridViewTextBoxColumn.DataPropertyName = "Name";
      this.nameDataGridViewTextBoxColumn.Frozen = true;
      this.nameDataGridViewTextBoxColumn.HeaderText = "Name";
      this.nameDataGridViewTextBoxColumn.Name = "nameDataGridViewTextBoxColumn";
      // 
      // sourcePathDataGridViewTextBoxColumn
      // 
      this.sourcePathDataGridViewTextBoxColumn.DataPropertyName = "SourcePath";
      this.sourcePathDataGridViewTextBoxColumn.HeaderText = "SourcePath";
      this.sourcePathDataGridViewTextBoxColumn.Name = "sourcePathDataGridViewTextBoxColumn";
      // 
      // destinationPathDataGridViewTextBoxColumn
      // 
      this.destinationPathDataGridViewTextBoxColumn.DataPropertyName = "DestinationPath";
      this.destinationPathDataGridViewTextBoxColumn.HeaderText = "DestinationPath";
      this.destinationPathDataGridViewTextBoxColumn.Name = "destinationPathDataGridViewTextBoxColumn";
      // 
      // includeSubFoldersDataGridViewCheckBoxColumn
      // 
      this.includeSubFoldersDataGridViewCheckBoxColumn.DataPropertyName = "IncludeSubFolders";
      this.includeSubFoldersDataGridViewCheckBoxColumn.HeaderText = "IncludeSubFolders";
      this.includeSubFoldersDataGridViewCheckBoxColumn.Name = "includeSubFoldersDataGridViewCheckBoxColumn";
      // 
      // lastSyncDateDataGridViewTextBoxColumn
      // 
      this.lastSyncDateDataGridViewTextBoxColumn.DataPropertyName = "LastSyncDate";
      this.lastSyncDateDataGridViewTextBoxColumn.HeaderText = "LastSyncDate";
      this.lastSyncDateDataGridViewTextBoxColumn.Name = "lastSyncDateDataGridViewTextBoxColumn";
      // 
      // watchListBindingSource
      // 
      this.watchListBindingSource.DataSource = typeof(FileSync.WatchList);
      // 
      // gridViewExcludeFolders
      // 
      this.gridViewExcludeFolders.AllowUserToDeleteRows = false;
      this.gridViewExcludeFolders.AutoGenerateColumns = false;
      this.gridViewExcludeFolders.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.gridViewExcludeFolders.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.excludeFolderNameDataGridViewTextBoxColumn});
      this.gridViewExcludeFolders.DataMember = "ExcludeFolders";
      this.gridViewExcludeFolders.DataSource = this.watchListBindingSource;
      this.gridViewExcludeFolders.Location = new System.Drawing.Point(551, 28);
      this.gridViewExcludeFolders.Name = "gridViewExcludeFolders";
      this.gridViewExcludeFolders.Size = new System.Drawing.Size(420, 262);
      this.gridViewExcludeFolders.TabIndex = 2;
      this.gridViewExcludeFolders.UserAddedRow += new System.Windows.Forms.DataGridViewRowEventHandler(this.gridViewExcludeFolders_UserAddedRow);
      // 
      // excludeFolderNameDataGridViewTextBoxColumn
      // 
      this.excludeFolderNameDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
      this.excludeFolderNameDataGridViewTextBoxColumn.DataPropertyName = "ExcludeFolderName";
      this.excludeFolderNameDataGridViewTextBoxColumn.HeaderText = "ExcludeFolderName";
      this.excludeFolderNameDataGridViewTextBoxColumn.Name = "excludeFolderNameDataGridViewTextBoxColumn";
      // 
      // MainForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(983, 583);
      this.Controls.Add(this.gridViewExcludeFolders);
      this.Controls.Add(this.gridViewWatches);
      this.Controls.Add(this.statusStripMain);
      this.Controls.Add(this.toolStripMain);
      this.Name = "MainForm";
      this.Text = "File Sync";
      this.toolStripMain.ResumeLayout(false);
      this.toolStripMain.PerformLayout();
      this.statusStripMain.ResumeLayout(false);
      this.statusStripMain.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.gridViewWatches)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.watchListBindingSource)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.gridViewExcludeFolders)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.ToolStrip toolStripMain;
    private System.Windows.Forms.ToolStripButton toolStripButtonNew;
    private System.Windows.Forms.ToolStripButton toolStripButtonSync;
    private System.Windows.Forms.ToolStripButton toolStripButtonExit;
    private System.Windows.Forms.StatusStrip statusStripMain;
    private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar;
    private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelMain;
    private System.Windows.Forms.DataGridView gridViewWatches;
    private System.Windows.Forms.BindingSource watchListBindingSource;
    private System.Windows.Forms.DataGridViewTextBoxColumn nameDataGridViewTextBoxColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn sourcePathDataGridViewTextBoxColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn destinationPathDataGridViewTextBoxColumn;
    private System.Windows.Forms.DataGridViewCheckBoxColumn includeSubFoldersDataGridViewCheckBoxColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn lastSyncDateDataGridViewTextBoxColumn;
    private System.Windows.Forms.DataGridView gridViewExcludeFolders;
    private System.Windows.Forms.DataGridViewTextBoxColumn excludeFolderNameDataGridViewTextBoxColumn;
    private System.Windows.Forms.ToolStripButton toolStripButtonSave;
    private System.Windows.Forms.ToolStripButton toolStripButtonCancel;
  }
}

