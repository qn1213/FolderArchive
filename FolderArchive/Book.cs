using FolderArchive.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Xml.Linq;
using System.Windows;

namespace FolderArchive
{
    internal class Book : ICallBack
    {
        public enum BASE_TYPE
        {
            BOOK = 1,
            PARTS
        }

        public WrapPanel wrapPanel;
        public CheckBox checkBox;
        public StackPanel stackPanel;
        public List<Label> labels;
        public Expander expander;        

        public int index = 0;
        public int status = 0; // 0 = 대기, 1 = 완료, -1 = 오류

        public string bookName = null;
        public string bookName64 = null;
        public string bookPath = null;

        public object colorLock = null;

        public Part parts;

        public string Error
        {
            get { return this.error; }
            set { this.error = value; }
        }

        private string error = null;

        public Book()
        {
            parts = new Part();
            colorLock = new object();
        }

        public void SetBook(string name, string path)
        {
            status = 0;
            bookName = name;
            bookName64 = Utill.Convert64(bookName);
            bookPath = path;
        }

        public void SetObjectBookInfo(ref DirectoryInfo[] part, string name, string path, ref UIwindow1 window)
        {
            status = 0;
            bookName = name;
            bookName64 = Utill.Convert64(bookName);
            bookPath = path;
            parts.partCnt = part.Length;

            labels = new List<Label>();

            wrapPanel = new WrapPanel();
            checkBox = new CheckBox();
            checkBox.Name = bookName64;
            checkBox.Margin = new System.Windows.Thickness(5);
            wrapPanel.Children.Add(checkBox);

            stackPanel = new StackPanel();
            stackPanel.Margin = new System.Windows.Thickness(10, 4, 0, 0);

            int index = 0;
            foreach(DirectoryInfo partDir in part)
            {
                parts.AddPartInfo(partDir.Name, partDir.FullName);
                Label partLabel = new Label();
                partLabel.Content = "· " + partDir.Name;
                partLabel.Name = GetBase64Name(Book.BASE_TYPE.PARTS, partDir.Name);
                labels.Add(partLabel);
                stackPanel.Children.Add(labels[index++]);
            }

            expander = new Expander();
            expander.Name = GetBase64Name(BASE_TYPE.BOOK);
            expander.Header = bookName + " (총 " + parts.partCnt + "화)";
            expander.Content = stackPanel;

            wrapPanel.Children.Add(expander);

            window.jobProc.Children.Add(wrapPanel);
        }

        public void SetObjectPartsInfo()
        {

        }

        public string GetBase64Name(BASE_TYPE type, string partName = null)
        {
            switch (type)
            {
                case BASE_TYPE.BOOK:
                    return bookName64;
                case BASE_TYPE.PARTS:
                    return parts.GetPartName64(partName);
                default:
                    return null;
            }
        }

        public void ChangeStatus(Utill.PROCESS_STAT stat, int partIndex = 0)
        {
            //lock(colorLock)
            //{
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
            {
                bool isAllGood = true;
                switch (stat)
                {
                    case Utill.PROCESS_STAT.ALL_DONE:
                        wrapPanel.Background = Brushes.Green;
                        foreach (Label label in labels)
                        {
                            if (label.Background != Brushes.Green)
                            {
                                isAllGood = false;
                                continue;
                            }

                            label.Background = Brushes.Green;
                        }

                        if (!isAllGood) wrapPanel.Background = Brushes.OrangeRed;
                        break;

                    case Utill.PROCESS_STAT.PART_DONE:
                        labels[partIndex].Background = Brushes.Green;
                        break;

                    case Utill.PROCESS_STAT.PROCESS:
                        wrapPanel.Background = Brushes.Yellow;
                        foreach (Label label in labels)
                            label.Background = Brushes.White;
                        break;

                    case Utill.PROCESS_STAT.ERROR:
                        labels[partIndex].Background = Brushes.Red;
                        break;

                    default:
                        break;
                }
            }));
            
            //}
        }
    }

    class Part
    {
        public List<string> partName;
        public Dictionary<string, string> partNameWith64;

        public int partCnt = 0;
        public List<string> partPath;
        public Dictionary<string, string> partPathWithName;

        public Part()
        {
            partName = new List<string>();

            partNameWith64 = new Dictionary<string, string>();

            partPath = new List<string>();

            partPathWithName = new Dictionary<string, string>();
        }

        public void AddPartInfo(string name, string path)
        {
            partName.Add(name);

            partNameWith64.Add(name, Utill.Convert64(name));

            partPath.Add(path);

            partPathWithName.Add(path, name);
        }

        public string GetPartName64(string partName)
        {
            return partNameWith64.ContainsKey(partName) ? partNameWith64[partName] : null;
        }
    }
}
