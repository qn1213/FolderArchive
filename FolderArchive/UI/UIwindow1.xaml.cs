using FastHDLsearch.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.IO;

namespace FolderArchive.UI
{
    public partial class UIwindow1 : Window, ILogs
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

            Log("Init Program");
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
            if(CheckInputPath() || compress.bookIndex == 0)
            {
                MessageBox.Show("압축할 폴더를 불러와주세요!");
                return;
            }
            compress.Start();
        }

        private void Button_Init(object sender, RoutedEventArgs e)
        {
            ClearLog();
            compress.Init();
        }

        private void BT_InputPath_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog ofd = new CommonOpenFileDialog();
            ofd.IsFolderPicker = true;

            if(ofd.ShowDialog() == CommonFileDialogResult.Ok)
            {
                this.inputPath = ofd.FileName;
                TB_InPutPath.Text = this.inputPath;
            }
            Log("Add InputDir : " + this.inputPath);

            InitCompress();
            Log("Create Book Object Start");
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
            Log("Add outputDir : " + this.outputPath);
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
            string tmp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
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

        public bool CheckInputPath()
        {
            return String.IsNullOrEmpty(inputPath);
        }

        public bool CheckOutputPath()
        {
            return String.IsNullOrEmpty(outputPath);
        }

        public void Log(string log)
        {
            string tmp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string time = "[" + tmp + "] : ";
            TB_Log.Text += time + log + "\n";
        }
    }
}
