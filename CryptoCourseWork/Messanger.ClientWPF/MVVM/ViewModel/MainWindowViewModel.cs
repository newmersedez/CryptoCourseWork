using System.Collections.ObjectModel;
using System.Linq;
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
        private readonly Server _server;
        public string Username { get; set; }
        public string Message { get; set; }
        public ObservableCollection<UserModel> Users { get; set; }
        public ObservableCollection<string> Messages { get; set; }
        
        public RelayCommand ConnectToServerCommand { get; set; }
        public RelayCommand SendMessageCommand { get; set; }
        public RelayCommand OpenClientFilesCommand { get; set; }
        public RelayCommand OpenServerFilesCommand { get; set; }

        public MainWindowViewModel()
        {
            Users = new ObservableCollection<UserModel>();
            Messages = new ObservableCollection<string>();
            _server = new Server();

            _server.ConnectedEvent += UserConnected;
            _server.MessageReceivedEvent += MessageReceived;
            _server.UserDisconnectedEvent += UserDisconnected;
            
            ConnectToServerCommand = new RelayCommand(
                o => _server.ConnectToServer(Username), 
                o => !string.IsNullOrEmpty(Username) && !_server.IsConnectedToServer());
            OpenClientFilesCommand = new RelayCommand(
                o => OpenClientFileDialog(),
                o => _server.IsConnectedToServer());
            OpenServerFilesCommand = new RelayCommand(
                o => OpenServerFileDialog(),
                o => _server.IsConnectedToServer());
            SendMessageCommand = new RelayCommand(
                o =>
                {
                    if (Message != null) 
                        _server.SendMessageToServer(Message);
                    Message = "";
                    RaisePropertyChanged(nameof(Message));
                },
                o =>
                {
                    return !string.IsNullOrEmpty(Message) && _server.IsConnectedToServer();
                });
        }

        private void OpenClientFileDialog()
        {
            // var openFileDialog = new OpenFileDialog();
            // if (openFileDialog.ShowDialog() == true)
            // {
            //     var file = openFileDialog.FileName;
            //     _server.SendFileToServer(file);
            // }
        }

        private void OpenServerFileDialog()
        {
            // var openFileDialog = new ServerFilesDialog();
            // if (openFileDialog.ShowDialog() == true)
            // {
            //     var filename = openFileDialog.FileName;
            //     MessageBox.Show(filename);
            // }
        }

        private void UserConnected()
        {
            var user = new UserModel
            {
                Username = _server.PacketReader.ReadMessage(),
                UID = _server.PacketReader.ReadMessage()
            };

            if (!Users.Any(x => x.UID == user.UID))
            {
                Application.Current.Dispatcher.Invoke(() => Users.Add(user));
            }
        }
        
        private void MessageReceived()
        {
            var message = _server.PacketReader.ReadMessage();
            Application.Current.Dispatcher.Invoke(() => Messages.Add(message));
        }
        
        private void UserDisconnected()
        {
            var uid = _server.PacketReader.ReadMessage();
            var user = Users.Where(x => x.UID == uid).FirstOrDefault();
            Application.Current.Dispatcher.Invoke(() => Users.Remove(user));
        }
    }    
}
