FileSync - Keep track of changed files
======================================

`FileSync` is a portable app coded in C# that keeps track of files in a watchlist and updates the files from and to the locations
specified in the watchlist.

The delete watch functionality is not working at the moment. Also, not all the properties are currently being used.

Commands
========

The following commands can be used:

  Copy the files in the default watchlist
```
    filesync sync
```


  Add a new file to the watch list
```
    filesync add <name> <sourcePath> <destinationPath> [-e] [-skip]
```

  Delete a watch from the list
```
    filesync del
```

  See the help
```
    filesync help
```