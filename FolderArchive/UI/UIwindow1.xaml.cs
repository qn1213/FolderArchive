using FastHDLsearch.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.IO;
using System.Text.RegularExpressions;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Windows.Input;
using static System.Net.Mime.MediaTypeNames;

namespace FolderArchive.UI
{
    public partial class UIwindow1 : Window, ILogs
    {
        private Button[] tabbuttons;

        private Compress compress;
        private bool isLowSpec = false;
        private int threadCnt = 0;

        //private string saveFilePath = "config.sav";
        private string configPath = "config.json";
        private string defaultOutputPath;
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
            defaultOutputPath = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName + "\\output";
            DirectoryInfo di = new DirectoryInfo(defaultOutputPath);
            if (di.Exists == false)
                di.Create();

            if (!File.Exists(configPath))
            {
                JObject configData;
                using (File.Create(configPath))
                {
                    configData = new JObject
                    {
                        new JProperty("inputpath", ""),
                        new JProperty("outputpath", defaultOutputPath),
                        new JProperty("islowspec", "false"),
                        new JProperty("threadcnt", "20")
                    };
                }

                File.WriteAllText(configPath, configData.ToString());
                Log("Create Config.json");

                inputPath = String.Empty;
            }
            else
            {
                using (StreamReader file = File.OpenText(configPath))
                {
                    using (JsonTextReader reader = new JsonTextReader(file))
                    {
                        JObject json = (JObject)JToken.ReadFrom(reader);

                        inputPath = json["inputpath"].ToString();
                        outputPath = json["outputpath"].ToString();
                        isLowSpec = (bool)json["islowspec"];
                        threadCnt = (int)json["threadcnt"];

                        xn_TextboxPath.Text = outputPath;
                        xn_isLowSpec.IsChecked = isLowSpec;

                        xn_TextboxThreadCnt.Text = threadCnt.ToString();
                        xn_TextboxThreadCnt.IsEnabled = isLowSpec ? false : true;
                    }
                }
            }

            TB_InPutPath.Text = isCheckPathNull() == true ? "Select Input Folder" : inputPath;
            
            if (isOutputPathNull())
                outputPath = defaultOutputPath;
            TB_OutPutPath.Text = outputPath;  

            Log("Init Program");
        }

        private void DoInit()
        {
            if (isCheckPathNull() == false)
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
            if(isCheckPathNull() || compress.bookIndex == 0)
            {
                MessageBox.Show("압축할 폴더를 불러와주세요!");
                return;
            }
            compress.Start(isLowSpec);
        }

        private void Button_Init(object sender, RoutedEventArgs e)
        {
            if (compress == null)
                return;

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

        private static readonly Regex _regex = new Regex("[^0-9]+");
        private void CheckDigit(object sender, TextChangedEventArgs e)
        {
            string text = ((TextBox)sender).Text;

            if(_regex.IsMatch(text))
            {
                MessageBox.Show("숫자만 입력가능해요!");
                ((TextBox)sender).Text = "";
                return;
            }
        }

        private void CheckCntRange(object sender, EventArgs e)
        {
            string text = ((TextBox)sender).Text;
            if (string.IsNullOrEmpty(text)) return;

            int tmpCnt = Int32.Parse(text);
            if (tmpCnt < 3 || tmpCnt > 100)
            {
                MessageBox.Show("3~100 사이로 입력 가능합니다!");
                ((TextBox)sender).Text = "";
                return;
            }

            threadCnt = tmpCnt;
        }

        private void SaveConfig()
        {
            string json = File.ReadAllText(configPath);
            dynamic jsonObj = JsonConvert.DeserializeObject(json);
            jsonObj["inputpath"] = inputPath;
            jsonObj["outputpath"] = outputPath;
            jsonObj["islowspec"] = isLowSpec;
            jsonObj["threadcnt"] = threadCnt;

            string output = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
            File.WriteAllText(configPath, output);
        }

        private void CheckLowSpecMode(object sender, EventArgs e)
        {
            isLowSpec = xn_isLowSpec.IsChecked.GetValueOrDefault();
            
            xn_TextboxThreadCnt.IsEnabled = isLowSpec ? false : true;
        }

        private void DoClosed(object sender, EventArgs e)
        {
            SaveConfig();
        }

        public bool isCheckPathNull()
        {
            return String.IsNullOrEmpty(inputPath);
        }

        public bool isOutputPathNull()
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
