using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using Messanger.ClientWPF.MVVM.Model;
using Messanger.ClientWPF.Net;
using MVVM.Core.Command;
using MVVM.Core.ViewModel;

namespace Messanger.ClientWPF.MVVM.ViewModel
{
    public sealed class MainWindowViewModel : ViewModelBase
    {
        //Key = 1334440654591915542993625911497130241

        private readonly Client _client;

        public string Username { get; set; }
        public string Message { get; set; }
        public string Key { get; set; }

        public ObservableCollection<UserModel> Users { get; }
        public ObservableCollection<string> Messages { get; }
        
        public RelayCommand ConnectToServerCommand { get; } 
        public RelayCommand SendMessageCommand { get; set; }

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
            
            ConnectToServerCommand = new RelayCommand(
                _ => _client.ConnectToServer(Username, Key), 
                _ => !string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Key) && !_client.IsConnectedToServer());
            SendMessageCommand = new RelayCommand(
                _ =>
                {
                    _client.SendMessageToServer(Message);
                    Message = "";
                    RaisePropertyChanged(nameof(Message));
                },
                _ => !string.IsNullOrEmpty(Message) && _client.IsConnectedToServer());
        }
        
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
    }    
}
