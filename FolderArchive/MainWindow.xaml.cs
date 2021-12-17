using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Threading;
using System.IO.Compression;
using System.IO;

namespace FolderArchive
{
    public partial class MainWindow : Window
    {
        public int indexs = 0; // 작업 완료 체크용
        public string rootName;
        public string outPutFolder;
        public List<string> subName;
        public List<string> procName; // 실제로 압축 처리할 리스트

        public List<CheckBox> l_CheckBox;

        public bool isFirst = true;
        public MainWindow()
        {            
            InitializeComponent();
            l_CheckBox = new List<CheckBox>();
            subName = new List<string>();
            procName = new List<string>();

            BT_FolderOpen.IsEnabled = false;
            BT_Compress.IsEnabled = false;
            BT_Clear.IsEnabled = false;

            Label label = new Label();
            label.Content = "How To Use\n1. 출력할 폴더를 불러온다\n2. 압축할 폴더를 불러온다. (체크한것만 압축함)\n3. 압축버튼을 한번만 누르고 기다린다.\n\n[약속된 폴더 구조]\n메인 폴더\n   ㄴ서브폴더\n      ㄴ폴더1\n      ㄴ폴더2\n폴더1, 2만 압축되서 [출력폴더]에 저장됨";
            wrapPanel.Children.Add(label);
            lb_FolderCount.Content = "0개폴더 중 0개 완료";
            lbl_outputPath.Content = "Select First!!";
        }

        private void BTFolderOpen(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;

            if(dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                rootName = dialog.FileName;

                GetDirectory(rootName);

                BT_FolderOpen.IsEnabled = false;
                BT_Compress.IsEnabled = true;
            }
        }

        private void BTCompress(object sender, RoutedEventArgs e)
        {
            if(l_CheckBox.Count <= 0)
            {
                MessageBox.Show("Select Folder");
                return;
            }

            BT_FolderOpen.IsEnabled = false;
            BT_Compress.IsEnabled = false;
            BT_Select_Ouput.IsEnabled = false;
            BT_Clear.IsEnabled = false;

            foreach(string folderName in subName)
            {
                foreach(CheckBox checkBox in l_CheckBox)
                {
                    if(checkBox.IsChecked == true && folderName.Contains(checkBox.Content.ToString()))
                    {
                        procName.Add(folderName);
                        break;
                    }
                }
            }

            for (int i = 0; i < procName.Count(); i++) 
            {
                ThreadPool.QueueUserWorkItem(ArchiveImageWork, procName[i]);
            }
        }

        private void ArchiveImageWork(object _path)
        {
            string path = (string) _path;
            string lbName = null;
            string cbName = null;
            string originFilePath = null;

            try
            {
                //////////////////////////////
                // System.IO.Compress Zip
                //////////////////////////////
                System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(path);
                System.IO.DirectoryInfo[] infos = di.GetDirectories("*.*", System.IO.SearchOption.TopDirectoryOnly); // 하위 폴더 불러오고
                                
                foreach (System.IO.DirectoryInfo info in infos)
                {
                    string[] split = info.FullName.Split('\\');
                    string sZipName = split[split.Length - 1];

                    // 체크박스, 레이블 이름 얻기
                    StringBuilder builder = new StringBuilder();

                    // 루트경로, 서브폴더 경로 얻기
                    string tmpLB = null;
                    string tmpCB = null;
                    for (int i = 0; i < split.Length - 2; i++)
                    {
                        builder.Append(split[i]);

                        if (i == ((split.Length - 2) - 1))
                            break;

                        builder.Append("\\");
                    }
                    tmpLB = builder.ToString();

                    builder.Clear();

                    for (int i = 0; i < split.Length - 1; i++)
                    {
                        builder.Append(split[i]);

                        if (i == ((split.Length - 1) - 1))
                            break;

                        builder.Append("\\");
                    }
                    tmpCB = builder.ToString();

                    // 라벨, 체크버튼의 Name속성
                    lbName = null;
                    cbName = null;

                    // 라벨
                    byte[] bytes1 = Encoding.Unicode.GetBytes(tmpLB);
                    string tmpBase64 = Convert.ToBase64String(bytes1);
                    string tmpStep1 = tmpBase64.Replace("=", "_");
                    string tmpStep2 = tmpStep1.Replace("+", "_");
                    string tmpPath = tmpStep2.Replace("/", "_");

                    lbName = tmpPath;
                                        
                    tmpBase64 = tmpStep1 = tmpStep2 = tmpPath = null;

                    // 체크버튼
                    byte[] bytes2 = Encoding.Unicode.GetBytes(tmpCB);
                    tmpBase64 = Convert.ToBase64String(bytes2);
                    tmpStep1 = tmpBase64.Replace("=", "_");
                    tmpStep2 = tmpStep1.Replace("+", "_");
                    tmpPath = tmpStep2.Replace("/", "_");
                    cbName = tmpPath;

                    // 저장폴더 설정
                    originFilePath = $"{path}\\{sZipName}.zip";
                    string outputFilePath = $"{outPutFolder}\\{sZipName}.zip";

                    // UI 설정
                    Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
                    {
                        Label lb = FindChild<Label>(wrapPanel, lbName);
                        lb.Background = Brushes.Yellow;
                        lb.Foreground = Brushes.Black;

                        CheckBox cb = FindChild<CheckBox>(wrapPanel, cbName);
                        cb.Background = Brushes.Yellow;
                        cb.Foreground = Brushes.Black;
                    }));


                    // 압축하기전 출력폴더에 해당 파일이 있는지 확인할것.
                    System.IO.FileInfo fileInfo = new System.IO.FileInfo(outputFilePath);
                    if(fileInfo.Exists)
                    {
                        Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
                        {
                            lb_log.Text += info.FullName + " => 이미 존재함!!!!!\n";
                        }));
                        continue;
                    }
                    ZipFile.CreateFromDirectory(info.FullName, originFilePath);

                    // Root \ Sub \ SubSub
                    // -1 => Root \ Sub \ SubSub
                    // -2 => Root \ Sub
                    string sOutputFolderName = split[split.Length - 2]; // 책 제목(output폴더에 만들어질 폴더 이름

                    string savePath = outPutFolder + "\\" + sOutputFolderName;
                    System.IO.DirectoryInfo outPutFolderPath = new System.IO.DirectoryInfo(savePath);
                    if (!outPutFolderPath.Exists)
                        outPutFolderPath.Create();

                    File.Move(originFilePath, savePath + "\\" + sZipName + ".zip");

                    Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
                    {
                        lb_log.Text += info.FullName + " : Done!" + "\n";
                        Label lb = FindChild<Label>(wrapPanel, lbName);
                        lb.Background = Brushes.Yellow;
                        lb.Foreground = Brushes.Black;
                    }));
                }

                Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
                {
                    lb_FolderCount.Content = procName.Count() + "개폴더 중 " + Interlocked.Increment(ref indexs) + "개 완료";

                    CheckBox cb = FindChild<CheckBox>(wrapPanel, cbName);
                    cb.Background = Brushes.Green;
                    cb.Foreground = Brushes.Black;


                    if (procName.Count() == indexs)
                    {
                        Label lb = FindChild<Label>(wrapPanel, lbName);
                        lb.Background = Brushes.Green;
                        lb.Foreground = Brushes.Black;

                        BT_Clear.IsEnabled = true;
                    }
                }));
            }
            catch (Exception e)
            {
                Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
                {
                    lb_log.Text += "\n\n" + originFilePath + " : Error!!!!" + "\n" + e.Message.ToString() + "\n\n";

                    Label lb = FindChild<Label>(wrapPanel, lbName);
                    lb.Background = Brushes.Orange;
                    lb.Foreground = Brushes.Black;

                    CheckBox cb = FindChild<CheckBox>(wrapPanel, cbName);
                    cb.Background = Brushes.Red;
                    cb.Foreground = Brushes.Black;

                    BT_Clear.IsEnabled= true;
                }));
                return;
            }
        }

        private void GetDirectory(string path)
        {
            if(isFirst)
            {
                wrapPanel.Children.Clear();
                isFirst = false;
            }
            System.IO.DirectoryInfo dirInfo = new System.IO.DirectoryInfo(path);

            if(dirInfo.Exists)
            {
                byte[] obytes = Encoding.Unicode.GetBytes(path);
                string otmpBase64 = Convert.ToBase64String(obytes);
                string otmpStep1 = otmpBase64.Replace("=", "_");
                string otmpStep2 = otmpStep1.Replace("+", "_");
                string tmpLBPath = otmpStep2.Replace("/", "_");

                Label lbl = new Label();
                lbl.Content = path;
                lbl.FontSize = 15;
                lbl.Foreground = System.Windows.Media.Brushes.Black;             
                lbl.Name = tmpLBPath;                
                wrapPanel.Children.Add(lbl);

                Button btn = new Button();
                btn.Content = "Open Dir";
                btn.Tag = path;
                btn.Click += new RoutedEventHandler(OpenFolder);
                wrapPanel.Children.Add(btn);

                //System.IO.DirectoryInfo[] Cinfo = dirInfo.GetDirectories("*", System.IO.SearchOption.AllDirectories);
                System.IO.DirectoryInfo[] Cinfo = dirInfo.GetDirectories("*", System.IO.SearchOption.TopDirectoryOnly);
                foreach(System.IO.DirectoryInfo info in Cinfo)
                {
                    byte[] bytes = Encoding.Unicode.GetBytes(info.FullName);
                    string tmpBase64 = Convert.ToBase64String(bytes);
                    string tmpStep1 = tmpBase64.Replace("=", "_");
                    string tmpStep2 = tmpStep1.Replace("+", "_");
                    string tmpCBPath = tmpStep2.Replace("/", "_");

                    CheckBox cb = new CheckBox();
                    cb.IsChecked = true;
                    cb.Content = info.Name;
                    cb.Name = tmpCBPath;
                    wrapPanel.Children.Add(cb);

                    l_CheckBox.Add(cb);
                    subName.Add(info.FullName);
                }                
            }
            else
            {
                MessageBox.Show("폴더 불러오는 부분에서 에러남");
                return;
            }        
        }

        private void CheckChanged(object sender, RoutedEventArgs e)
        {
            bool newVal = (cbAllCheck.IsChecked == true);

            foreach(CheckBox check in l_CheckBox)
            {
                check.IsChecked = newVal;
            }
        }

        private void OpenFolder(object sender, RoutedEventArgs e)
        {
            var values = ((Button)sender).Tag.ToString();

            try
            {
                System.Diagnostics.Process.Start(values);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                throw;
            }
        }

        private void BTClear(object sender, RoutedEventArgs e)
        {
            isFirst = true;

            indexs = 0;

            rootName = null;
            //outPutFolder = null;
            lbl_outputPath.Content = "Select First!!";

            subName.Clear();
            procName.Clear();
            l_CheckBox.Clear();
            wrapPanel.Children.Clear();

            Label label = new Label();
            label.Content = "How To Use\n1. 출력할 폴더를 불러온다\n2. 압축할 폴더를 불러온다. (체크한것만 압축함)\n3. 압축버튼을 한번만 누르고 기다린다.\n\n[약속된 폴더 구조]\n메인 폴더\n   ㄴ서브폴더\n      ㄴ폴더1\n      ㄴ폴더2\n폴더1, 2만 압축되서 [출력폴더]에 저장됨";
            wrapPanel.Children.Add(label);
            lb_FolderCount.Content = "0개폴더 중 0개 완료";
            lb_log.Text = "";

            BT_FolderOpen.IsEnabled = true;
            BT_Compress.IsEnabled = false;
            BT_Select_Ouput.IsEnabled = true;
        }

        private void BTSelectOuput(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                outPutFolder = dialog.FileName;
                lbl_outputPath.Content = dialog.FileName;

                BT_FolderOpen.IsEnabled = true;
                BT_Clear.IsEnabled = true;
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

        private void LogChanged(object sender, TextChangedEventArgs e)
        {
            lb_log.SelectionStart = lb_log.Text.Length;
            lb_log.ScrollToEnd();
        }
    }
}
