using System;
using System.IO;

namespace FileSync
{
  public interface IFile
  {
    string Name { get; set; }
    string FullName { get; set; }
    long Size { get; set; }
    DateTime DateCreated { get; set; }
    string Type { get; set; }

    int Compare(IFile file);
    int Copy(IFile file, string dir);
    int Delete(IFile file, string dir);

    void PopulateFileDetails(string fName);
  }
}
