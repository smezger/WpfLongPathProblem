using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace WpfAppLongPath
{
  public class DropFileCommand : ICommand
  {
    protected Action<IDataObject> _execute;
    protected Predicate<IDataObject> _canExecute;

    private readonly Action<string> _onDrop;

    private readonly IEnumerable<string> _fileExtensions;

    public DropFileCommand(Action<string> onDrop) : this(onDrop, Enumerable.Empty<string>())
    {
    }

    public DropFileCommand(Action<string> onDrop, string fileExtension) : this(onDrop, new string[] { fileExtension })
    {
    }

    public DropFileCommand(Action<string> onDrop, IEnumerable<string> fileExtensions)
    {
      _execute = Drop;
      _canExecute = CanDrop;
      _onDrop = onDrop;
      _fileExtensions = fileExtensions;
    }

    private string GetFile(IDataObject dataObject)
    {
      var files = (string[])dataObject.GetData(DataFormats.FileDrop) ?? Enumerable.Empty<string>();
      var filteredFiles = _fileExtensions.Count() > 0 && !_fileExtensions.Contains(".*") ? files.Where(x => _fileExtensions.Contains(Path.GetExtension(x).ToLower())) : files;
      return filteredFiles.Count() != 1 ? null : filteredFiles.First();
    }

    public void Drop(IDataObject dataObject)
    {
      var file = GetFile(dataObject);
      if (file != null)
      {
        _onDrop(file);
      }
    }

    public bool CanDrop(IDataObject dataObject)
    {
      return GetFile(dataObject) != null;
    }

    public bool CanExecute(object parameter)
    {
      return _canExecute == null || _canExecute((IDataObject)parameter);
    }

    public void Execute(object parameter)
    {
      _execute((IDataObject)parameter);
    }
    public event EventHandler CanExecuteChanged
    {
      add { CommandManager.RequerySuggested += value; }
      remove { CommandManager.RequerySuggested -= value; }
    }
  }
}
