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
using Microsoft.WindowsAPICodePack.Dialogs;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using Microsoft.WindowsAPICodePack.Controls;

namespace FolderArchive.UI
{
    /// <summary>
    /// UIwindows.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class UIwindow1 : Window
    {
        Button[] tabbuttons;
        FileInfo saveFile;
        Compress compress;
        string saveFilePath = "config.sav";
        string defaultOutputPath;
        public string inputPath;
        public string outputPath;

        public UIwindow1()
        {
            InitializeComponent();

            DataContext = new MainViewModel();
            tabbuttons = new Button[] { xn_tabbutton1, xn_tabbutton2 };

            DoProgramStartInit();
            DoInit();
        }

        private void DoProgramStartInit()
        {
            saveFile = new FileInfo(saveFilePath);
            FileStream fs = null;
            if(!saveFile.Exists)
            {
                fs = saveFile.Create();
                fs.Close();
            }

            fs = saveFile.OpenRead();
            StreamReader sr = new StreamReader(fs);
            inputPath = sr.ReadLine();
            outputPath = sr.ReadLine();

            defaultOutputPath = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName + "\\output";
            DirectoryInfo di = new DirectoryInfo(defaultOutputPath);
            if (di.Exists == false)
                di.Create();

            TB_InPutPath.Text = CheckInputPath() == true ? "Select Input Folder" : inputPath;
            if (CheckOutputPath())
            {
                TB_OutPutPath.Text = defaultOutputPath;
                outputPath = defaultOutputPath;
            }
            TB_OutPutPath.Text = outputPath;
            AddLog("Init Program");
        }

        private void DoInit()
        {
            if (CheckInputPath() == false)
            {
                InitCompress();
            }
                
        }

        private void InitCompress()
        {
            if(compress == null)
                compress = new Compress(this);
            compress.InitPath();
            compress.SetBookObject();
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

        private void Button_Start_Click(object sender, RoutedEventArgs e)
        {
            if(CheckInputPath())
            {
                MessageBox.Show("입력폴더가 비어있어요!");
                return;
            }

        }

        private void Button_Init(object sender, RoutedEventArgs e)
        {
            ClearLog();
        }

        private void BT_InputPath_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog ofd = new CommonOpenFileDialog();
            ofd.IsFolderPicker = true;

            if(ofd.ShowDialog() == CommonFileDialogResult.Ok)
            {
                if(this.inputPath == ofd.FileName)
                {
                    MessageBox.Show("선택한 폴더와 경로가 같습니다.");
                    return;
                }

                this.inputPath = ofd.FileName;
                TB_InPutPath.Text = this.inputPath;
            }
            AddLog("Add InputDir : " + this.inputPath);

            InitCompress();
            AddLog("Create Book Object Start");
        }

        private void BT_OutputPath_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog ofd = new CommonOpenFileDialog();
            ofd.IsFolderPicker = true;

            if (ofd.ShowDialog() == CommonFileDialogResult.Ok)
            {
                if (this.outputPath == ofd.FileName)
                {
                    MessageBox.Show("선택한 폴더와 경로가 같습니다.");
                    return;
                }

                this.outputPath = ofd.FileName;
                TB_OutPutPath.Text = this.outputPath;
            }
            AddLog("Add outputDir : " + this.outputPath);
        }

        /* 테스트 코드 */
        public int index = 0;
        public Dictionary<int, StackPanel> process;

        private void Button_test_Click(object sender, RoutedEventArgs e)
        {
            Random rnd = new Random();
            int partCnt = rnd.Next(1, 10);

            WrapPanel wrapPanel = new WrapPanel();

            CheckBox checkBox = new CheckBox();
            checkBox.Name = "check_" + index;
            checkBox.IsChecked = true;
            checkBox.Margin = new Thickness(5);
            wrapPanel.Children.Add(checkBox);

            StackPanel partPanel = new StackPanel();
            partPanel.Margin = new Thickness(10, 4, 0, 0);
            partPanel.Name = "stack_book_" + index;

            for(int i = 0; i < partCnt; i++)
            {
                Label name = new Label();
                name.Content = (i + 1) + "화";
                name.Name = "label_" + index + "_" + i;
                partPanel.Children.Add(name);
            }

            Expander expander = new Expander();
            expander.Name = "expander_" + index++;
            expander.Header = index + " (" + partCnt + ")";
            expander.Content = partPanel;
            expander.Foreground = Brushes.Green;

            wrapPanel.Children.Add(expander);

            jobProc.Children.Add(wrapPanel);
        }

        public void AddLog(string log)
        {
            string tmp = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string time = "[" + tmp + "] : ";
            TB_Log.Text += time + log + "\n";
        }

        private void ClearLog()
        {
            TB_Log.Text = "";
        }

        private void SaveConfig(string input = null, string output = null)
        {
            FileStream fs = saveFile.OpenWrite();
            TextWriter tw = new StreamWriter(fs);

            tw.WriteLine(input);
            tw.Write(output);

            tw.Close();
            fs.Close();
        }

        private void DoClosed(object sender, EventArgs e)
        {
            SaveConfig(CheckInputPath() ? null : inputPath, CheckOutputPath() ? defaultOutputPath : outputPath);
        }

        /// <summary>
        /// intput, output path가 null인지 아닌지 확인용
        /// </summary>
        /// <param name="pathDir"></param> 둘다 비었는지, intput만 비었는지, output만 비었는지
        /// <param name="isNull"></param> 비어있는지, 입력 되어있는지
        public bool CheckInputPath()
        {
            return String.IsNullOrEmpty(inputPath);
        }

        public bool CheckOutputPath()
        {
            return String.IsNullOrEmpty(outputPath);
        }
    }
}
