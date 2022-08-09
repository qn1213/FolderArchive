using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace FastHDLsearch.ViewModels
{
    internal class MainViewModel :INotifyPropertyChanged
    {
        public ObservableCollection<string> MyMessages { get; set; }

        //public RelayCommand MessageBoxCommand { get; private set; }
        public RelayCommand TabButtonCommand { get; set; }
        public RelayCommand HyperlinkButtonCommand { get; set; }
        private int _selectedIndex = 0;
        public int SelectedIndex
        {
            get
            {
                return _selectedIndex;
            }
            set
            {
                _selectedIndex = value;
                Notify("SelectedIndex");
            }
        }
        private int _selectedbackgroundIndex=1;
        public int SelectedbackgroundIndex
        {
            get
            {
                return _selectedbackgroundIndex;
            }
            set
            {
                _selectedbackgroundIndex = value;
                Notify("SelectedbackgroundIndex");
            }
        }

        public MainViewModel()
        {
            MyMessages = new ObservableCollection<string>()
            {
                "Hello World!",
                "My name is Joe",
                "I love learning commands",
                "Im a message box",
                "Im a console"
            };

            //MessageBoxCommand = new RelayCommand(DisplayInMessageBox, MessageBoxCanUse);
            TabButtonCommand = new RelayCommand(TabButtonClicked);
            SelectedbackgroundIndex = SelectedIndex + 1;
            HyperlinkButtonCommand = new RelayCommand(OpenHyperlink);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void Notify(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }


        //public void DisplayInMessageBox(object message)
        //{
        //    MessageBox.Show((string)message);
        //}

        public void TabButtonClicked(object index)
        {
            Debug.WriteLine("test"+SelectedIndex);
            SelectedIndex = int.Parse(index.ToString() ?? "0");
            SelectedbackgroundIndex = SelectedIndex+1;
            
        }
        public void OpenHyperlink(object uri)
        {
            if (uri == "")
            {
                System.Diagnostics.Process.Start(new ProcessStartInfo("https://null") { UseShellExecute = true });
            }
            else
            {
                System.Diagnostics.Process.Start(new ProcessStartInfo(uri.ToString() ?? "https://null") { UseShellExecute = true });
            }
            
        }

        public bool MessageBoxCanUse(object message)
        {
            if ((string)message == "Im a console")
                return false;

            return true;
        }

        public bool ConsoleCanUse(object message)
        {
            if ((string)message == "Im a message box")
                return false;

            return true;
        }
    }
}
