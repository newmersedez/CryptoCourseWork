using System;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Enumeration;
using System.Linq;
using System.Text;
using System.Windows;
using Messanger.ClientWPF.MVVM.Model;
using Messanger.ClientWPF.Net;
using Messanger.ClientWPF.Themes;
using Microsoft.Win32;
using MVVM.Core.Command;
using MVVM.Core.ViewModel;

namespace Messanger.ClientWPF.MVVM.ViewModel
{
    public sealed class MainWindowViewModel : ViewModelBase
    {
        private const string UserFiles = "\\Messanger.ClientWPF\\UserFiles\\";
        private readonly Client _client;

        public string Username { get; set; }
        public string Message { get; set; }
        public string Key { get; set; }

        public ObservableCollection<UserModel> Users { get; }
        public ObservableCollection<string> Messages { get; }
        
        public RelayCommand ConnectToServerCommand { get; } 
        public RelayCommand SendMessageCommand { get; set; }
        public RelayCommand OpenServerFilesCommand { get; }
        public RelayCommand OpenClientFilesCommand { get; set; }

        public MainWindowViewModel()
        {
            Username = string.Empty;
            Message = string.Empty;
            Key = string.Empty;
            Users = new ObservableCollection<UserModel>();
            Messages = new ObservableCollection<string>();
            
            _client = new Client();
            _client.ConnectedEvent += UserConnected;
            _client.MessageReceivedEvent += MessageReceived;
            _client.UserDisconnectedEvent += UserDisconnected;
            _client.FileReceivedEvent += FileReceived;
            
            ConnectToServerCommand = new RelayCommand(
                _ =>
                {
                    try
                    {
                        _client.ConnectToServer(Username, Key);
                    }
                    catch (Exception)
                    {
                        MessageBox.Show($"Failed to connect to the server");
                    }
                }, 
                _ => !string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Key) && !_client.IsConnectedToServer());
            SendMessageCommand = new RelayCommand(
                _ =>
                {   try
                    {
                        _client.SendMessageToServer(Message);
                        Message = "";
                        RaisePropertyChanged(nameof(Message));
                    }
                    catch (Exception)
                    {
                        MessageBox.Show($"Failed to send message");
                    }
                },
                _ => !string.IsNullOrEmpty(Message) && _client.IsConnectedToServer());
            OpenServerFilesCommand = new RelayCommand(
                _ => OpenServerFileDialog(),
                _ => _client.IsConnectedToServer());
            OpenClientFilesCommand = new RelayCommand(
                _ => OpenClientFileDialog(),
                _ => _client.IsConnectedToServer());
        }

        private void FileReceived()
        {
            var filename = Encoding.Default.GetString(_client.PacketReader.ReadMessage());
            var message = _client.Algorithm.Decrypt(_client.PacketReader.ReadMessage());
            var directory = new DirectoryInfo(Directory.GetCurrentDirectory());
            while (directory != null && !directory.GetFiles("*.sln").Any())
            {
                directory = directory.Parent;
            }

            var fullPath = directory + UserFiles + filename;
            File.WriteAllBytesAsync(fullPath, message);
;        }

        private void UserConnected()
        {
            var user = new UserModel
            {
                Username = Encoding.Default.GetString(_client.PacketReader.ReadMessage()),
                Uid = Encoding.Default.GetString(_client.PacketReader.ReadMessage())
            };

            if (Users.All(x => x.Uid != user.Uid))
            {
                Application.Current.Dispatcher.Invoke(() => Users.Add(user));
            }
        }
        
        private void MessageReceived()
        {
            var username = Encoding.Default.GetString(_client.PacketReader.ReadMessage());
            var separator = Encoding.Default.GetString(_client.PacketReader.ReadMessage());
            var message = _client.Algorithm.Decrypt(_client.PacketReader.ReadMessage());
            var buildMessage = username + separator + Encoding.Default.GetString(message);
            Application.Current.Dispatcher.Invoke(() => Messages.Add(buildMessage));
        }
        
        private void UserDisconnected()
        {
            var uid = Encoding.Default.GetString(_client.PacketReader.ReadMessage());
            var user = Users.FirstOrDefault(x => x.Uid == uid);
            Application.Current.Dispatcher.Invoke(() => user != null && Users.Remove(user));
        }
        
        private void OpenClientFileDialog()
        {
            var openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                var fullPath = openFileDialog.FileName;
                _client.SendFileToServer(fullPath);
            }
        }
        
        private void OpenServerFileDialog()
        {
            var openFileDialog = new ServerFilesDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                var fullPath = openFileDialog.FileName;
                var index = fullPath.LastIndexOf('\\');
                var filename = fullPath.Substring(index + 1, fullPath.Length - index - 1);
                _client.GetFileFromServer(filename);
            }
        }
    }    
}
