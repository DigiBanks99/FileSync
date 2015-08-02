using FileSync;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileSyncUI
{
  public partial class MainForm : Form
  {
    #region Constructor
    public MainForm()
    {
      LoadControl();
    }
    #endregion Constructor

    #region Properties
    public WatchList WatchList
    {
      get { return watchListBindingSource.DataSource as WatchList; }
      set { watchListBindingSource.DataSource = value; }
    }
    #endregion Properties

    #region Public Methods
    public void InitialiseBindingSource()
    {
      FileSyncManager.Manager.WatchFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "FileSync", "WatchList.dfs");

      try
      {
        var dir = Path.GetDirectoryName(FileSyncManager.Manager.WatchFilePath);
        if (!Directory.Exists(dir))
          Directory.CreateDirectory(dir);

        if (!File.Exists(FileSyncManager.Manager.WatchFilePath))
        {
          StringBuilder sb = new StringBuilder();

          sb.AppendLine("<WatchList>");
          sb.AppendLine("</WatchList>");

          using (StreamWriter writer = new StreamWriter(FileSyncManager.Manager.WatchFilePath))
          {
            writer.Write(sb.ToString());
          }
        }

        RefreshBindingSource();
      }
      catch (IOException ex)
      {
        MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    public void RefreshBindingSource()
    {
      WatchList = FileSyncManager.Manager.ReadWatchList();
      Refresh();
    }

    public async Task Sync(WatchList watchList)
    {
      try
      {
        toolStripProgressBar.Maximum = watchList.Count;
        toolStripProgressBar.Minimum = 0;
        toolStripProgressBar.Value = 0;

        await Task.Factory.StartNew(() => FileSyncManager.Manager.SyncAsync(watchList));

        if (FileSyncManager.Manager.Cancel)
          MessageBox.Show("Sync cancelled.", "Sync Cancelled", MessageBoxButtons.OK, MessageBoxIcon.Stop);
        else
          MessageBox.Show("All files up to date.", "Nothing to sync", MessageBoxButtons.OK, MessageBoxIcon.Information);

        //TODO: some kind of feedback for the copied list. A messagebox is too compact

        RefreshBindingSource();

        if (FileSyncManager.Manager.Cancel)
          UpdateStatus("Sync cancelled.");
        else
          UpdateStatus("Sync completed.");
      }
      catch (IOException ex)
      {
        MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private async Task Save()
    {
      foreach (WatchItem watchItem in WatchList as WatchList)
      {
        var updateList = FileSyncManager.Manager.UpdateWatchItem(watchItem);
        await FileSyncManager.Manager.UpdateWatchListAsync(updateList);
      }
      RefreshBindingSource();
      MessageBox.Show("Save successful.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
    #endregion Public Methods

    #region Private Methods
    private void LoadControl()
    {
      try
      {
        InitializeComponent();
        InitializeControls();
        InitialiseBindingSource();
        SetupEvents();
        UpdateStatus("Ready");
      }
      catch (Exception ex)
      {
        MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void InitializeControls()
    {
      Logger.SurpressLogging = true;
#if DEBUG
      Logger.SurpressLogging = false;
      toolStripButtonResetForDev.Visible = true;
#endif
    }

    private void SetupEvents()
    {
      FileSyncManager.Manager.OnFileCopied += FileCopied;
      FileSyncManager.Manager.OnFileCopying += FileCopying;
      FileSyncManager.Manager.OnCancel += Canceled;
      FileSyncManager.Manager.OnProgressChanged += Manager_OnProgressChanged;
    }

    private void RemoveEvents()
    {
      FileSyncManager.Manager.OnFileCopied -= FileCopied;
      FileSyncManager.Manager.OnFileCopying -= FileCopying;
      FileSyncManager.Manager.OnCancel -= Canceled;
      FileSyncManager.Manager.OnProgressChanged -= Manager_OnProgressChanged;
    }

    private void AddWatch()
    {
      WatchItem watchItem = null;

      using (AddWatchControl addWatchControl = new AddWatchControl(WatchList))
      {
        addWatchControl.ShowDialog();
        if (addWatchControl.Result == DialogResult.OK)
          watchItem = addWatchControl.WatchItem;
      }

      if (watchItem != null)
      {
        FileSyncManager.Manager.AddNewWatch(watchItem);
      }
    }

    private void UpdateStatus(FileSyncEventArgs e)
    {
      if (e == null || e.Message == null || e.Message == string.Empty)
        return;

      UpdateStatus(e.Message);
    }

    private void UpdateStatus(string message)
    {
      toolStripStatusLabelMain.Text = message;
      this.Refresh();
    }

    private void ResetForDev()
    {
      foreach (WatchItem watchItem in WatchList)
      {
        watchItem.LastSyncDate = null;
      }
    }
    #endregion Private Methods

    #region Events
    private void toolStripButtonNew_Click(object sender, EventArgs e)
    {
      try
      {
        AddWatch();
      }
      catch (Exception ex)
      {
        MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private async void toolStripButtonSync_Click(object sender, EventArgs e)
    {
      try
      {
        toolStripButtonCancel.Enabled = true;
        toolStripButtonNew.Enabled = false;
        toolStripButtonSave.Enabled = false;
        toolStripButtonSync.Enabled = false;
        FileSyncManager.Manager.Cancel = false;
        CancellationToken cancel = new CancellationToken(FileSyncManager.Manager.Cancel);
        await Sync(WatchList);
      }
      catch (Exception ex)
      {
        MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
      finally
      {
        toolStripButtonCancel.Enabled = false;
        toolStripButtonNew.Enabled = true;
        toolStripButtonSave.Enabled = true;
        toolStripButtonSync.Enabled = true;
        FileSyncManager.Manager.Cancel = false;
      }
    }

    private void toolStripButtonExit_Click(object sender, EventArgs e)
    {
      if (MessageBox.Show("Are you sure you want to exit the application?", "Exiting", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
        Application.Exit();
    }

    private async void toolStripButtonSave_Click(object sender, EventArgs e)
    {
      try
      {
        toolStripButtonCancel.Enabled = false;
        toolStripButtonNew.Enabled = false;
        toolStripButtonSave.Enabled = false;
        toolStripButtonSync.Enabled = false;
        await Save();
      }
      catch (NullReferenceException ex)
      {
        MessageBox.Show(@"The list of watch items was not properly initialised.
                           \nPlease contact the developers at https://github.com/DigiBanks99/FileSync/issues.
                           \nPlease copy and paste the following:" + ex.StackTrace);
      }
      catch (Exception ex)
      {
        MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
      finally
      {
        toolStripButtonCancel.Enabled = false;
        toolStripButtonNew.Enabled = true;
        toolStripButtonSave.Enabled = true;
        toolStripButtonSync.Enabled = true;
      }
    }

    private void toolStripButtonCancel_Click(object sender, EventArgs e)
    {
      try
      {
        FileSyncManager.Manager.Cancel = true;
      }
      catch (Exception ex)
      {
        MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void toolStripButtonResetForDev_Click(object sender, EventArgs e)
    {
      try
      {
        ResetForDev();
        MessageBox.Show("Reset for Development.", "Reset successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
      }
      catch (Exception ex)
      {
        MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void FileCopied(object sender, FileSyncEventArgs e)
    {
      UpdateStatus(e);
    }

    private void FileCopying(object sender, FileSyncEventArgs e)
    {
      UpdateStatus(e);
    }

    private void Canceled(object sender, CancelEventArgs e)
    {
      if (e.Cancelled)
        UpdateStatus(e.Message);
    }

    private void Manager_OnProgressChanged(object sender, int newValue)
    {
      if (newValue == 0)
        return;

      toolStripProgressBar.Value = newValue;
    }
    #endregion Events

    #region IDisposable
    public void DisposeControl()
    {
      RemoveEvents();
    }

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      DisposeControl();
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }
    #endregion IDisposable
  }
}
