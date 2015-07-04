namespace FileSyncUI
{
  partial class AddWatchControl
  {
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.components = new System.ComponentModel.Container();
      this.panelMain = new System.Windows.Forms.Panel();
      this.checkBoxIncludeSubFolders = new System.Windows.Forms.CheckBox();
      this.textBoxDestinationDirectory = new System.Windows.Forms.TextBox();
      this.watchItemBindingSource = new System.Windows.Forms.BindingSource(this.components);
      this.textBoxSourceDirectory = new System.Windows.Forms.TextBox();
      this.textBoxName = new System.Windows.Forms.TextBox();
      this.labelIncludeSubFolders = new System.Windows.Forms.Label();
      this.labelDestinationPath = new System.Windows.Forms.Label();
      this.labelSourcePath = new System.Windows.Forms.Label();
      this.labelName = new System.Windows.Forms.Label();
      this.panelButtons = new System.Windows.Forms.Panel();
      this.buttonCancel = new System.Windows.Forms.Button();
      this.buttonOk = new System.Windows.Forms.Button();
      this.panelMain.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.watchItemBindingSource)).BeginInit();
      this.panelButtons.SuspendLayout();
      this.SuspendLayout();
      // 
      // panelMain
      // 
      this.panelMain.Controls.Add(this.checkBoxIncludeSubFolders);
      this.panelMain.Controls.Add(this.textBoxDestinationDirectory);
      this.panelMain.Controls.Add(this.textBoxSourceDirectory);
      this.panelMain.Controls.Add(this.textBoxName);
      this.panelMain.Controls.Add(this.labelIncludeSubFolders);
      this.panelMain.Controls.Add(this.labelDestinationPath);
      this.panelMain.Controls.Add(this.labelSourcePath);
      this.panelMain.Controls.Add(this.labelName);
      this.panelMain.Controls.Add(this.panelButtons);
      this.panelMain.Dock = System.Windows.Forms.DockStyle.Fill;
      this.panelMain.Location = new System.Drawing.Point(0, 0);
      this.panelMain.Name = "panelMain";
      this.panelMain.Size = new System.Drawing.Size(416, 130);
      this.panelMain.TabIndex = 0;
      // 
      // checkBoxIncludeSubFolders
      // 
      this.checkBoxIncludeSubFolders.AutoSize = true;
      this.checkBoxIncludeSubFolders.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.watchItemBindingSource, "IncludeSubFolders", true));
      this.checkBoxIncludeSubFolders.Location = new System.Drawing.Point(118, 83);
      this.checkBoxIncludeSubFolders.Name = "checkBoxIncludeSubFolders";
      this.checkBoxIncludeSubFolders.Size = new System.Drawing.Size(15, 14);
      this.checkBoxIncludeSubFolders.TabIndex = 6;
      this.checkBoxIncludeSubFolders.UseVisualStyleBackColor = true;
      // 
      // textBoxDestinationDirectory
      // 
      this.textBoxDestinationDirectory.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.watchItemBindingSource, "DestinationPath", true));
      this.textBoxDestinationDirectory.Location = new System.Drawing.Point(118, 56);
      this.textBoxDestinationDirectory.Name = "textBoxDestinationDirectory";
      this.textBoxDestinationDirectory.Size = new System.Drawing.Size(251, 20);
      this.textBoxDestinationDirectory.TabIndex = 5;
      // 
      // watchItemBindingSource
      // 
      this.watchItemBindingSource.DataSource = typeof(FileSync.WatchItem);
      // 
      // textBoxSourceDirectory
      // 
      this.textBoxSourceDirectory.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.watchItemBindingSource, "SourcePath", true));
      this.textBoxSourceDirectory.Location = new System.Drawing.Point(118, 30);
      this.textBoxSourceDirectory.Name = "textBoxSourceDirectory";
      this.textBoxSourceDirectory.Size = new System.Drawing.Size(251, 20);
      this.textBoxSourceDirectory.TabIndex = 5;
      // 
      // textBoxName
      // 
      this.textBoxName.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.watchItemBindingSource, "Name", true));
      this.textBoxName.Location = new System.Drawing.Point(118, 4);
      this.textBoxName.Name = "textBoxName";
      this.textBoxName.Size = new System.Drawing.Size(251, 20);
      this.textBoxName.TabIndex = 5;
      // 
      // labelIncludeSubFolders
      // 
      this.labelIncludeSubFolders.AutoSize = true;
      this.labelIncludeSubFolders.Location = new System.Drawing.Point(7, 83);
      this.labelIncludeSubFolders.Name = "labelIncludeSubFolders";
      this.labelIncludeSubFolders.Size = new System.Drawing.Size(95, 13);
      this.labelIncludeSubFolders.TabIndex = 4;
      this.labelIncludeSubFolders.Text = "Include Subfolders";
      // 
      // labelDestinationPath
      // 
      this.labelDestinationPath.AutoSize = true;
      this.labelDestinationPath.Location = new System.Drawing.Point(7, 59);
      this.labelDestinationPath.Name = "labelDestinationPath";
      this.labelDestinationPath.Size = new System.Drawing.Size(105, 13);
      this.labelDestinationPath.TabIndex = 3;
      this.labelDestinationPath.Text = "Destination Directory";
      // 
      // labelSourcePath
      // 
      this.labelSourcePath.AutoSize = true;
      this.labelSourcePath.Location = new System.Drawing.Point(7, 33);
      this.labelSourcePath.Name = "labelSourcePath";
      this.labelSourcePath.Size = new System.Drawing.Size(86, 13);
      this.labelSourcePath.TabIndex = 2;
      this.labelSourcePath.Text = "Source Directory";
      // 
      // labelName
      // 
      this.labelName.AutoSize = true;
      this.labelName.Location = new System.Drawing.Point(7, 7);
      this.labelName.Name = "labelName";
      this.labelName.Size = new System.Drawing.Size(35, 13);
      this.labelName.TabIndex = 1;
      this.labelName.Text = "Name";
      // 
      // panelButtons
      // 
      this.panelButtons.Controls.Add(this.buttonCancel);
      this.panelButtons.Controls.Add(this.buttonOk);
      this.panelButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.panelButtons.Location = new System.Drawing.Point(0, 100);
      this.panelButtons.Name = "panelButtons";
      this.panelButtons.Size = new System.Drawing.Size(416, 30);
      this.panelButtons.TabIndex = 0;
      // 
      // buttonCancel
      // 
      this.buttonCancel.Location = new System.Drawing.Point(257, 4);
      this.buttonCancel.Name = "buttonCancel";
      this.buttonCancel.Size = new System.Drawing.Size(75, 23);
      this.buttonCancel.TabIndex = 1;
      this.buttonCancel.Text = "&Cancel";
      this.buttonCancel.UseVisualStyleBackColor = true;
      this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
      // 
      // buttonOk
      // 
      this.buttonOk.Location = new System.Drawing.Point(338, 4);
      this.buttonOk.Name = "buttonOk";
      this.buttonOk.Size = new System.Drawing.Size(75, 23);
      this.buttonOk.TabIndex = 0;
      this.buttonOk.Text = "&Ok";
      this.buttonOk.UseVisualStyleBackColor = true;
      this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
      // 
      // AddWatchControl
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(416, 130);
      this.Controls.Add(this.panelMain);
      this.Name = "AddWatchControl";
      this.panelMain.ResumeLayout(false);
      this.panelMain.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.watchItemBindingSource)).EndInit();
      this.panelButtons.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Panel panelMain;
    private System.Windows.Forms.Label labelName;
    private System.Windows.Forms.Panel panelButtons;
    private System.Windows.Forms.Button buttonCancel;
    private System.Windows.Forms.Button buttonOk;
    private System.Windows.Forms.TextBox textBoxName;
    private System.Windows.Forms.Label labelIncludeSubFolders;
    private System.Windows.Forms.Label labelDestinationPath;
    private System.Windows.Forms.Label labelSourcePath;
    private System.Windows.Forms.BindingSource watchItemBindingSource;
    private System.Windows.Forms.CheckBox checkBoxIncludeSubFolders;
    private System.Windows.Forms.TextBox textBoxDestinationDirectory;
    private System.Windows.Forms.TextBox textBoxSourceDirectory;
  }
}
