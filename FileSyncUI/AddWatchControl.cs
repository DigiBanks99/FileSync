using FileSync;
using System.Windows.Forms;

namespace FileSyncUI
{
  public partial class AddWatchControl : Form
  {
    private WatchItem _watchItem = null;
    private WatchList _parentList = null;
    private DialogResult _result;

    public AddWatchControl(WatchList parentList)
    {
      InitializeComponent();

      _parentList = parentList;
      WatchItem = _parentList.AddNew();
    }

    public AddWatchControl(WatchItem watchItem)
    {
      InitializeComponent();

      WatchItem = watchItem;
    }

    public WatchItem WatchItem
    {
      get { return _watchItem; }
      private set
      {
        _watchItem = value;
        watchItemBindingSource.DataSource = WatchItem;
      }
    }

    public DialogResult Result
    {
      get { return _result; }
    }

    /// <summary> 
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    private void buttonOk_Click(object sender, System.EventArgs e)
    {
      _result = DialogResult.OK;
      this.Close();
    }

    private void buttonCancel_Click(object sender, System.EventArgs e)
    {
      _result = DialogResult.Cancel;

      if (_parentList != null)
        _parentList.Remove(_watchItem);

      this.Close();
    }
  }
}
