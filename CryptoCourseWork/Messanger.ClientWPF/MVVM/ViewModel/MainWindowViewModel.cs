using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Windows;
using Messanger.ClientWPF.MVVM.Model;
using Messanger.ClientWPF.Net;
using MVVM.Core.Command;
using MVVM.Core.ViewModel;

namespace Messanger.ClientWPF.MVVM.ViewModel
{
    public sealed class MainWindowViewModel : ViewModelBase
    {
        public ObservableCollection<UserModel> Users { get; set; }
        public ObservableCollection<string> Messages { get; set; }
        public RelayCommand ConnectToServerCommand { get; set; }
        public RelayCommand SendMessageCommand { get; set; }
        public string Username { get; set; }
        public string Message { get; set; }

        private Server _server;
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
                o => !string.IsNullOrEmpty(Username));
            SendMessageCommand = new RelayCommand(
                o => _server.SendMessageToServer(Message),
                o => !string.IsNullOrEmpty(Message));
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
