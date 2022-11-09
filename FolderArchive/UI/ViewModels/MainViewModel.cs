using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;

namespace FastHDLsearch.ViewModels
{
    internal class MainViewModel :INotifyPropertyChanged
    {
        public ObservableCollection<string> MyMessages { get; set; }

        //public RelayCommand MessageBoxCommand { get; private set; }
        public RelayCommand TabButtonCommand { get; set; }
        public RelayCommand HyperlinkButtonCommand { get; set; }
        //public RelayCommand ButtonStart { get; set; }

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
            //MessageBoxCommand = new RelayCommand(DisplayInMessageBox, MessageBoxCanUse);
            this.TabButtonCommand = new RelayCommand(TabButtonClicked);
            this.SelectedbackgroundIndex = this.SelectedIndex + 1;
            this.HyperlinkButtonCommand = new RelayCommand(OpenHyperlink);
            //this.ButtonStart = new RelayCommand(StartCompress);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void Notify(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public void TabButtonClicked(object index)
        {
            Debug.WriteLine("Tab" + this.SelectedIndex);
            this.SelectedIndex = int.Parse(index.ToString() ?? "0");
            this.SelectedbackgroundIndex = this.SelectedIndex+1;
            
        }
        public void OpenHyperlink(object uri)
        {
            if ((string)uri == "")
            {
                System.Diagnostics.Process.Start(new ProcessStartInfo("https://github.com/qn1213/FolderArchive") { UseShellExecute = true });
            }
            else
            {
                System.Diagnostics.Process.Start(new ProcessStartInfo(uri.ToString() ?? "https://null") { UseShellExecute = true });
            }            
        }

        //public void StartCompress(object inputPath)
        //{
        //    MessageBox.Show((string)inputPath);
        //}
    }
}
