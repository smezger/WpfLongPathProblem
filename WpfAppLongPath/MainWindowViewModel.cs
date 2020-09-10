using System;
using System.Collections.Generic;
using System.Text;

namespace WpfAppLongPath
{
  public class MainWindowViewModel
  {
    public DropFileCommand DropFileCommand
    {
      get
      {
        return _DropFileCommand ?? (_DropFileCommand = new DropFileCommand(OpenFile,".*"));
      }
      set
      {
        _DropFileCommand = value;        
      }
    }
    private DropFileCommand _DropFileCommand;

    public void OpenFile(string file)
    {


    }
  }
}
