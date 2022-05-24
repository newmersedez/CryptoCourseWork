using System;
using System.Collections.ObjectModel;
using CryptoCourseWork.MVVM.Model;
using MVVM.Core.ViewModel;

namespace CryptoCourseWork.MVVM.ViewModel
{
    public sealed class MainWindowViewModel : ViewModelBase
    {
        public ObservableCollection<ContactModel> Contacts { get; set; }
        public ObservableCollection<MessageModel> Messages { get; set; }
        
        public MainWindowViewModel()
        {
            Contacts = new ObservableCollection<ContactModel>();
            Messages = new ObservableCollection<MessageModel>();

            Messages.Add(new MessageModel()
            {
                Username = "newmersedez",
                UsernameColor="Black",
                ImageSource = "https://i.imgur.com/yMWvLXd.png",
                Message="Lalka",
                Time=DateTime.Now,
                IsNativeOrigin = false,
                FirstMessage = true
            });

            for (var i = 0; i < 3; ++i)
            {
                Messages.Add(new MessageModel()
                {
                    Username = "newmersedez",
                    UsernameColor="Black",
                    ImageSource = "https://i.imgur.com/yMWvLXd.png",
                    Message="Lalka",
                    Time=DateTime.Now,
                    IsNativeOrigin = true,
                });
            }
            
            for (var i = 0; i < 3; ++i)
            {
                Messages.Add(new MessageModel()
                {
                    Username = "lalka",
                    UsernameColor="Black",
                    ImageSource = "https://i.imgur.com/yMWvLXd.png",
                    Message="Lalka",
                    Time=DateTime.Now,
                    IsNativeOrigin = true,
                });
            }
            
            for (var i = 0; i < 3; ++i)
            {
                Messages.Add(new MessageModel()
                {
                    Username = "valyavalya",
                    UsernameColor="Black",
                    ImageSource = "https://i.imgur.com/yMWvLXd.png",
                    Message="Lalka",
                    Time=DateTime.Now,
                    IsNativeOrigin = true,
                });
            }

            for (var i = 0; i < 3; ++i)
            {
                Contacts.Add(new ContactModel
                {
                    Username = $"Alison {i}",
                    ImageSource = "https://i.imgur.com/yMWvLXd.png",
                    Messages = Messages
                });
            }
        }
    }

}