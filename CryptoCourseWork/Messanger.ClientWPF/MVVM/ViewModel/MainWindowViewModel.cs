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
        private readonly Client _client;
        
        public string Username { get; set; }
        public string Message { get; set; }
        
        public ObservableCollection<UserModel> Users { get; set; }
        public ObservableCollection<string> Messages { get; set; }
        
        public RelayCommand ConnectToServerCommand { get; set; }
        public RelayCommand SendMessageCommand { get; set; }

        public MainWindowViewModel()
        {
            Users = new ObservableCollection<UserModel>();
            Messages = new ObservableCollection<string>();
            _client = new Client();

            _client.ConnectedEvent += UserConnected;
            _client.MessageReceivedEvent += MessageReceived;
            _client.UserDisconnectedEvent += UserDisconnected;
            
            ConnectToServerCommand = new RelayCommand(
                o => _client.ConnectToServer(Username), 
                o => !string.IsNullOrEmpty(Username) && !_client.IsConnectedToServer());
            SendMessageCommand = new RelayCommand(
                o =>
                {
                    if (Message != null)
                    {
                        _client.SendMessageToServer(Message);
                    }
                    Message = "";
                    RaisePropertyChanged(nameof(Message));
                },
                o =>
                {
                    return !string.IsNullOrEmpty(Message) && _client.IsConnectedToServer();
                });
        }
        
        private void UserConnected()
        {
            var user = new UserModel
            {
                Username = Encoding.Default.GetString(_client.PacketReader.ReadMessage()),
                UID = Encoding.Default.GetString(_client.PacketReader.ReadMessage())
            };

            if (!Users.Any(x => x.UID == user.UID))
            {
                Application.Current.Dispatcher.Invoke(() => Users.Add(user));
            }
        }
        
        private void MessageReceived()
        {
            var message = Encoding.Default.GetString(_client.PacketReader.ReadMessage());
            Application.Current.Dispatcher.Invoke(() => Messages.Add(message));
        }
        
        private void UserDisconnected()
        {
            var uid = Encoding.Default.GetString(_client.PacketReader.ReadMessage());
            var user = Users.FirstOrDefault(x => x.UID == uid);
            Application.Current.Dispatcher.Invoke(() => user != null && Users.Remove(user));
        }
        
    }    
}
