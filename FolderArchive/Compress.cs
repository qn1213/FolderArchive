using FolderArchive.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using System.IO.Compression;
using System.Windows.Controls;
using System.IO;

namespace FolderArchive
{
    internal class Compress
    {
        private string inputPath;
        private string outPutPath;
        
        protected UIwindow1 window;
        private Dictionary<int, Book> book;

        public Compress(UIwindow1 window)
        {
            this.book = new Dictionary<int, Book>();
            this.window = window;
        }

        public void InitPath()
        {
            inputPath = window.inputPath;
            outPutPath = window.outputPath;
        }

        public void Start()
        {


            // 인풋 폴더안에 있는 만화, 화수 가져와서 객체화해서 뿌리기
        }

        public void SetBookObject()
        {
            window.jobProc.Children.Clear();

            System.IO.DirectoryInfo dirInfo = new System.IO.DirectoryInfo(inputPath);

            if (!dirInfo.Exists)
            {
                MessageBox.Show("경로가 잘못됐습니다!!");
                window.AddLog("Fail to Get Input folder");
                return;
            }

            // 맨위에 인풋 폴더 열기 버튼 생성
            Button inputOpenButton = new Button();
            inputOpenButton.Content = "Click to open input folder";
            inputOpenButton.Tag = inputPath; // 이거 필요없으면 지우기
            inputOpenButton.Click += delegate (object sender, RoutedEventArgs e)
            {
                var values = ((Button)sender).Tag.ToString();

                try
                {
                    System.Diagnostics.Process.Start(values);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    window.AddLog(ex.Message);
                    throw;
                }
            };
            window.jobProc.Children.Add(inputOpenButton);

            // 폴더 및 화수 불러오기
            DirectoryInfo[] info = dirInfo.GetDirectories("*", SearchOption.TopDirectoryOnly);
            foreach(DirectoryInfo infoDir in info)
            {
                string path = infoDir.FullName;
                string name = infoDir.Name;

                CheckBox cb = new CheckBox();
                cb.IsChecked = true;
                cb.Content = name;
                //cb.Name = path;

                window.jobProc.Children.Add(cb);
            }
        }

        private void AddObject()
        {

        }


    }
}
