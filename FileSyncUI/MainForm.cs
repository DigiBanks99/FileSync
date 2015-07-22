using FileSync;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace FileSyncUI
{
  public partial class MainForm : Form
  {
    private FileSyncManager _manager = null;
    private WatchList _watchList = null;

    public MainForm()
    {
      LoadControl();
    }

    public void InitialiseBindingSource()
    {
      _manager = new FileSyncManager();
      _manager.WatchFilePath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "FileSync", "WatchList.dfs");

      try
      {
        var dir = Path.GetDirectoryName(_manager.WatchFilePath);
        if (!Directory.Exists(dir))
          Directory.CreateDirectory(dir);

        if (!File.Exists(_manager.WatchFilePath))
        {
          StringBuilder sb = new StringBuilder();

          sb.AppendLine("<WatchList>");
          sb.AppendLine("</WatchList>");

          using (StreamWriter writer = new StreamWriter(_manager.WatchFilePath))
          {
            writer.Write(sb.ToString());
          }
        }

        _watchList = _manager.ReadWatchList();
      }
      catch (IOException ex)
      {
        MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }

      if (_watchList == null)
        _watchList = new WatchList();

      watchListBindingSource.DataSource = _watchList;
    }

    public void Sync(WatchList watchList)
    {
      try
      {
        _manager.Sync(watchList);

        if (_manager.SyncedFiles.Keys.Count == 0)
        {
          MessageBox.Show("All files up to date.", "Nothing to sync", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        else
        {
          var sb = new StringBuilder();
          foreach (var key in _manager.SyncedFiles.Keys)
          {
            sb.AppendLine(string.Format("Added the following files for {0}:", key));
            foreach (var file in _manager.SyncedFiles[key])
              sb.AppendLine(string.Format("\t{0}", file));
          }
          MessageBox.Show(sb.ToString(), "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
      }
      catch (System.IO.IOException ex)
      {
        MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    #region Private Methods
    private void LoadControl()
    {
      try
      {
        InitializeComponent();
        InitialiseBindingSource();
        SetupEvents();
      }
      catch (Exception ex)
      {
        MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void SetupEvents()
    {
      _manager.OnProgressChanged += ProgressChanged;
      _manager.OnProgressChanging += ProgressChanging;
      _manager.OnCancel += Canceled;
    }

    private void RemoveEvents()
    {
      _manager.OnProgressChanged -= ProgressChanged;
      _manager.OnProgressChanging -= ProgressChanging;
      _manager.OnCancel -= Canceled;
    }

    private void AddWatch()
    {
      WatchItem watchItem = null;

      using (AddWatchControl addWatchControl = new AddWatchControl(_watchList))
      {
        addWatchControl.ShowDialog();
        if (addWatchControl.Result == DialogResult.OK)
          watchItem = addWatchControl.WatchItem;
      }

      if (watchItem != null)
      {
        _manager.AddNewWatch(watchItem);
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

    private void toolStripButtonSync_Click(object sender, EventArgs e)
    {
      try
      {
        toolStripButtonCancel.Enabled = true;
        toolStripButtonNew.Enabled = false;
        toolStripButtonSave.Enabled = false;
        toolStripButtonSync.Enabled = false;
        _manager.Cancel = false;
        Sync(_watchList);
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
        _manager.Cancel = false;
      }
    }

    private void toolStripButtonExit_Click(object sender, EventArgs e)
    {
      Application.Exit();
    }

    private void toolStripButtonSave_Click(object sender, EventArgs e)
    {
      try
      {
        toolStripButtonCancel.Enabled = false;
        toolStripButtonNew.Enabled = false;
        toolStripButtonSave.Enabled = false;
        toolStripButtonSync.Enabled = false;
        var watchItem = watchListBindingSource.Current as WatchItem;
        if (watchItem != null)
        {
          var updateList = _manager.UpdateWatchItem(watchItem);
          _manager.UpdateWatchList(updateList);
        }
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
        _manager.Cancel = true;
      }
      catch (Exception ex)
      {
        MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void gridViewExcludeFolders_UserAddedRow(object sender, DataGridViewRowEventArgs e)
    {

    }

    private void ProgressChanged(object sender, FileSyncEventArgs e)
    {
      UpdateStatus(e);
    }

    private void ProgressChanging(object sender, FileSyncEventArgs e)
    {
      UpdateStatus(e);
    }

    private void Canceled(object sender, CancelEventArgs e)
    {
      if (e.Cancelled)
        UpdateStatus(e.Message);
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
