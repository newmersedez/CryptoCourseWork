using System.IO;
using System.Linq;
using System.Windows;
using MVVM.Core.Command;

namespace Messanger.ClientWPF.Themes;

public partial class ServerFilesDialog
{
    private const string ServerFiles = "\\Messanger.Server\\Files\\";

    public string FileName => (string) ServerFilesListView.SelectedItem;
    public RelayCommand CancelCommand { get; }
    public RelayCommand OkCommand { get; }

    public ServerFilesDialog()
    {
        InitializeComponent();
        Loaded += WindowLoaded;

        CancelCommand = new RelayCommand(_ => CancelCommandMethod());
        OkCommand = new RelayCommand(_ => OkCommandMethod(), _ => !string.IsNullOrEmpty(FileName));
    }
    
    private void WindowLoaded(object sender, RoutedEventArgs e)
    {
        var directory = new DirectoryInfo(Directory.GetCurrentDirectory());
        while (directory != null && !directory.GetFiles("*.sln").Any())
        {
            directory = directory.Parent;
        }
        var serverFilesFullPath = directory + ServerFiles;
        var fileEntries = Directory.GetFiles(serverFilesFullPath);
        foreach (var fileName in fileEntries)
        {
            var index = fileName.LastIndexOf('\\');
            ServerFilesListView.Items.Add(fileName.Substring(index + 1, fileName.Length - index - 1));
        }
    }
    
    private void OkCommandMethod()
    {
        DialogResult = true;
        Close();
    }

    private void CancelCommandMethod()
    {
        Close();
    }
}