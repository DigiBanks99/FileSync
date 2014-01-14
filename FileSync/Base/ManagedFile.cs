using System;
using System.IO;

namespace FileSync
{
  public class ManagedFile : IFile
  {
    #region Constructors

    public ManagedFile(string fullName)
    {
      FullName = fullName;
      PopulateFileDetails(fullName);
    }

    public ManagedFile(string name, string fullName, long size, DateTime dateCreated, string type)
    {
      Name = name;
      FullName = fullName;
      Size = size;
      DateCreated = dateCreated;
      Type = type;
    }

    public ManagedFile()
      : this(string.Empty, string.Empty, -1, DateTime.Now, string.Empty)
    {

    }

    #endregion // Constructors

    #region Properties & Members

    public string Name { get; set; }
    public string FullName { get; set; }
    public long Size { get; set; }
    public DateTime DateCreated { get; set; }
    public string Type { get; set; }

    #endregion // Properties & Members

    #region Public Methods

    public int Compare(IFile file)
    {
      if (!Name.Equals(file.Name))
        return FileConsts.ERROR;
      if (!Size.Equals(file.Size))
        return FileConsts.ERROR;
      if (!DateCreated.Equals(file.DateCreated))
        return FileConsts.ERROR;
      if (!Type.Equals(file.Type))
        return FileConsts.ERROR;

      return FileConsts.SUCCESS;
    }

    public int Copy(IFile file, string dir)
    {
      if (!File.Exists(file.FullName)) return FileConsts.NOACTION;
      try
      {
        string newFile = string.Format(@"{0}\{1}.{2}", dir, file.Name, file.Type);
        if (!File.Exists(string.Format(@"{0}\{1}.{2}", dir, file.Name, file.Type)))
          File.Copy(file.FullName, newFile);
      }
      catch (IOException ex)
      {
        Console.WriteLine("ERROR: " + ex.Message);
        return FileConsts.ERROR;
      }
      return FileConsts.SUCCESS;
    }

    public int Delete(IFile file, string dir)
    {
      if (!File.Exists(file.FullName)) return FileConsts.NOACTION;
      try
      {
        File.Delete(file.FullName);
      }
      catch (IOException ex)
      {
        Console.WriteLine("ERROR: " + ex.Message);
        return FileConsts.ERROR;
      }
      return FileConsts.SUCCESS;
    }
    
    public void PopulateFileDetails(string fName)
    {
      FullName = fName;
      Type = fName.Substring(fName.LastIndexOf('.') + 1);
      Name = fName.Substring(fName.LastIndexOf('\\') + 1, fName.LastIndexOf('.') - fName.LastIndexOf('\\') - 1);
    }

    #endregion // Public Methods
  }
}