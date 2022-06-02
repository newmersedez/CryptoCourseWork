using System;
using System.Numerics;
using MVVM.Core.Command;
using MVVM.Core.ViewModel;

namespace Messanger.KeysGeneratorWPF.MVVM.ViewModel
{
    internal sealed class MainWindowViewModel : ViewModelBase
    {
        public string Key { get; set; }
        public RelayCommand GenerateButtonCommand { get; }

        public MainWindowViewModel()
        {
            GenerateButtonCommand = new RelayCommand(_ =>
            {
                Key = GenerateKey();
                RaisePropertiesChanged(nameof(Key));
            });
        }

        private string GenerateKey()
        {
            var byteKey = new byte[16];
            var random = new Random();
            for (var i = 0; i < 16; ++i)
            {
                byteKey[i] = (byte)random.Next(0, 255);
            }
            var key = new BigInteger(byteKey);
            return key.ToString();
        }
    }
}