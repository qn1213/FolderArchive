using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace FolderArchive
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public string outputFolder;
        public string inputFolder;

        // 테스트 코드
        public int index = 0;
        public Dictionary<int, StackPanel> process;

        // 로그
        LogWindow logWindow;
        ILog logs;

        public MainWindow()
        {
            InitializeComponent();

            logWindow = new LogWindow();
            this.logs = logWindow;

            process = new Dictionary<int, StackPanel>();
        }

        private void Reset()
        {
            logs.Clear();
            process.Clear();
        }

        public void BT_Test_Click(object sender, RoutedEventArgs e)
        {
            // 테스트 코드
            Random rnd = new Random();
            int partCount = rnd.Next(1, 100);

            StackPanel bookPanel = new StackPanel();
            WrapPanel wrapPanel = new WrapPanel();

            // 체크박스
            CheckBox checkBox = new CheckBox();
            checkBox.Name = "check_" + index; // 여기서 index는 책 이름이 될거같음
            checkBox.IsChecked = true;
            checkBox.Margin = new Thickness(5);
            wrapPanel.Children.Add(checkBox);

            // 익스펜더에 들어갈 스택패널
            StackPanel partPanel = new StackPanel();
            partPanel.Margin = new Thickness(10, 4, 0, 0);
            partPanel.Name = "stack_book_" + index;

            for (int i = 0; i < partCount; i++)
            {// 여기서 i가 화수가 될듯
                Label name = new Label();
                name.Content = (i + 1) + "화";
                name.Name = "label_" + index + "_" + i;
                partPanel.Children.Add(name);
            }

            // 익스팬더에 등록
            Expander expander = new Expander();
            expander.Name = "expander_" + index;
            expander.Header = index + " (" + partCount + ")";
            expander.Content = partPanel;
            expander.Foreground = Brushes.Green;
            // 최종적으로 책의 파트들 완성
            wrapPanel.Children.Add(expander);

            // 최종적으로 책 한권 완성
            bookPanel.Children.Add(wrapPanel);

            process.Add(index++, bookPanel);
            jobProc.Children.Add(bookPanel);
        }

        private void BT_TEST_Click(object sender, RoutedEventArgs e)
        {
            int bookIndex = Convert.ToInt32(TB_Book.Text);
            int partIndex = Convert.ToInt32(TB_Part.Text);

            StackPanel bookPanel = process[bookIndex];

            StackPanel partPanel = FindChild<StackPanel>(bookPanel, "stack_book_" + partIndex);

            if (partIndex == -1)
            {
                foreach (Label item in partPanel.Children)
                {
                    item.Background = Brushes.Green;
                }
            }
            else
            {
                Label item = (Label)partPanel.Children[partIndex];
                item.Background = Brushes.Green;
            }
        }

        public static T FindChild<T>(DependencyObject parent, string childName)
           where T : DependencyObject
        {
            // Confirm parent and childName are valid. 
            if (parent == null) return null;

            T foundChild = null;

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                // If the child is not of the request child type child
                T childType = child as T;
                if (childType == null)
                {
                    // recursively drill down the tree
                    foundChild = FindChild<T>(child, childName);

                    // If the child is found, break so we do not overwrite the found child. 
                    if (foundChild != null) break;
                }
                else if (!string.IsNullOrEmpty(childName))
                {
                    var frameworkElement = child as FrameworkElement;
                    // If the child's name is set for search
                    if (frameworkElement != null && frameworkElement.Name == childName)
                    {
                        // if the child's name is of the request name
                        foundChild = (T)child;
                        break;
                    }
                }
                else
                {
                    // child element found.
                    foundChild = (T)child;
                    break;
                }
            }

            return foundChild;
        }

        private void BT_SetOutputFolder_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                if (this.inputFolder == dialog.FileName)
                {
                    MessageBox.Show("인풋 폴더와 경로가 같습니다.");
                    return;
                }

                this.outputFolder = dialog.FileName;
                LB_OutputPath.Content = this.outputFolder;

                this.logs.Log("[Output Set] : " + this.outputFolder);
            }
        }

        private void BT_SetInputFolder_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                if (string.IsNullOrEmpty(this.inputFolder) == false)
                {
                    Reset();
                }

                if (this.outputFolder == dialog.FileName)
                {
                    MessageBox.Show("출력 폴더와 경로가 같습니다.");
                    return;
                }

                this.inputFolder = dialog.FileName;
                LB_InputPath.Content = this.inputFolder;

                this.logs.Log("[Input Set] : " + this.inputFolder);
            }
        }

        private void BT_ShowOutputFolder_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.outputFolder))
            {
                MessageBox.Show("결과물을 저장할 폴더를 지정해주세요!");
                return;
            }

            System.Diagnostics.Process.Start(this.outputFolder);
        }

        private void BT_ShowInputFolder_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(this.inputFolder))
            {
                MessageBox.Show("불러올 폴더를 지정해주세요!");
                return;
            }

            System.Diagnostics.Process.Start(this.inputFolder);
        }

        private void BT_Start_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(outputFolder) || string.IsNullOrEmpty(inputFolder))
            {
                MessageBox.Show("출력, 입력폴더를 확인해주세요");
                return;
            }

            Compress compress = new Compress(this, this.outputFolder, this.inputFolder);
        }

        private void BT_Log_Click(object sender, RoutedEventArgs e)
        {
            logWindow.Top = this.Top - 5;
            logWindow.Left = this.Left - 300;
            logWindow.Show();
        }

        private void OnClosed(object sender, EventArgs e)
        {
            App.Current.Shutdown();
        }
    }
}
