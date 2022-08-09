using FastHDLsearch.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;


namespace FolderArchive.UI
{
    /// <summary>
    /// UIwindows.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class UIwindow1 : Window
    {
        Button[] tabbuttons;
        public UIwindow1()
        {
            InitializeComponent();

            DataContext = new MainViewModel();
            tabbuttons = new Button[] { xn_tabbutton1, xn_tabbutton2, xn_tabbutton3 };
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            if (outputPathSettings_combobox.SelectedIndex == 2)
            {
                outputPathSettings.IsEnabled = true;
                outputPathSettings_button.Visibility = Visibility.Visible;
            }
            else
            {
                outputPathSettings.IsEnabled = false;
                outputPathSettings_button.Visibility = Visibility.Hidden;
            }
        }

        private void xn_TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (var btit in tabbuttons.Select((value, index) => new { value, index }))
            {
                if (btit.index == xn_TabControl.SelectedIndex)
                {
                    btit.value.IsEnabled = false;
                }
                else
                {
                    btit.value.IsEnabled = true;
                }

            }
        }
    }
}
