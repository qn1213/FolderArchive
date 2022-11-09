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

namespace FolderArchive.UI
{
    /// <summary>
    /// UIwindows.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class UIwindow1 : Window
    {
        Button[] tabbuttons;
        FileInfo saveFile;

        string saveFilePath = "config.sav";
        string inputDir;
        string outputDir;

        public UIwindow1()
        {
            InitializeComponent();

            DataContext = new MainViewModel();
            tabbuttons = new Button[] { xn_tabbutton1, xn_tabbutton2 };

            DoStartInit();


        }

        private void DoStartInit()
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
            inputDir = sr.ReadLine();
            outputDir = sr.ReadLine();

            TB_InPutPath.Text = String.IsNullOrEmpty(inputDir) == true ? "Select Input Folder" : inputDir;
            TB_OutPutPath.Text = String.IsNullOrEmpty(outputDir) == true ? "Select output Folder" : outputDir;
            AddLog("Init Program");
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
            Compress compress = new Compress(this, outputDir, inputDir);
            compress.Start();
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
                if(this.inputDir == ofd.FileName)
                {
                    MessageBox.Show("선택한 폴더와 경로가 같습니다.");
                    return;
                }

                this.inputDir = ofd.FileName;
                TB_InPutPath.Text = this.inputDir;
            }
            AddLog("Add InputDir : " + this.inputDir);           
        }

        private void BT_SetList_Click(object sender, RoutedEventArgs e)
        {
            // 프로그램 종료될 때 패스 저장하기ㅣ
        }

        private void BT_OutputPath_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog ofd = new CommonOpenFileDialog();
            ofd.IsFolderPicker = true;

            if (ofd.ShowDialog() == CommonFileDialogResult.Ok)
            {
                if (this.outputDir == ofd.FileName)
                {
                    MessageBox.Show("선택한 폴더와 경로가 같습니다.");
                    return;
                }

                this.outputDir = ofd.FileName;
                TB_OutPutPath.Text = this.outputDir;
            }
            AddLog("Add outputDir : " + this.outputDir);
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
            SaveConfig(String.IsNullOrEmpty(inputDir) == true ? null : inputDir, this.outputDir);
            SaveConfig(this.inputDir, String.IsNullOrEmpty(outputDir) == true ? null : outputDir);
        }
    }
}
