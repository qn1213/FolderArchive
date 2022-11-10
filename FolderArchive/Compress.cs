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
using System.Windows.Media;

namespace FolderArchive
{
    internal class Compress
    {
        private string inputPath;
        private string outPutPath;

        private int bookIndex = 0;
        
        protected UIwindow1 window;
        private Dictionary<int, Book> dic_book;
        private List<Book> list_exception_book;

        public Compress(UIwindow1 window)
        {
            dic_book = new Dictionary<int, Book>();
            list_exception_book = new List<Book>();
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

            DirectoryInfo dirInfo = new DirectoryInfo(inputPath);

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
            bookIndex = 0;
            DirectoryInfo[] info = dirInfo.GetDirectories("*", SearchOption.TopDirectoryOnly);
            foreach(DirectoryInfo infoDir in info)
            {
                Book book = new Book();
                book.index = bookIndex;

                if (AddObject(infoDir, ref book))
                    dic_book.Add(bookIndex++, book);
                else
                    list_exception_book.Add(book);
            }

            // 불러오기 성공/실패 목록 출력
            window.AddLog("################ 로드한 폴더 ################");
            int logIndex = 1;
            foreach(KeyValuePair<int, Book> items in dic_book)
            {
                window.AddLog(logIndex++ + ". : " + items.Value.bookPath);
            }

            window.AddLog("################ 로드 실패한 폴더 ################");
            logIndex = 1;
            foreach (Book items in list_exception_book)
            {
                window.AddLog(logIndex++ + ". : " + items.bookPath + " => " + items.Error);
            }
        }

        private bool AddObject(DirectoryInfo rootInfo, ref Book book)
        {
            try
            {
                DirectoryInfo[] part = rootInfo.GetDirectories("*", SearchOption.TopDirectoryOnly);

                WrapPanel wrapPanel = new WrapPanel();

                book.SetBook(rootInfo.Name, rootInfo.FullName);
                book.parts.partCnt = part.Length;
                if (book.parts.partCnt == 0)
                {
                    book.Error = "Part Count is 0";
                    return false;
                }

                CheckBox checkBox = new CheckBox();
                checkBox.Name = book.GetBase64Name(Book.BASE_TYPE.BOOK);
                checkBox.IsChecked = true;
                checkBox.Margin = new Thickness(5);
                wrapPanel.Children.Add(checkBox);

                StackPanel partPanel = new StackPanel();
                partPanel.Margin = new Thickness(10, 4, 0, 0);
                //partPanel.Name = "";

                // 화수
                foreach (DirectoryInfo partDir in part)
                {
                    book.parts.AddPartInfo(partDir.Name, partDir.FullName);
                    Label partLabel = new Label();
                    partLabel.Content = "· " + partDir.Name;
                    partLabel.Name = book.GetBase64Name(Book.BASE_TYPE.PARTS, partDir.Name);
                    partPanel.Children.Add(partLabel);
                }

                Expander expander = new Expander();
                expander.Name = book.GetBase64Name(Book.BASE_TYPE.BOOK);
                expander.Header = book.bookName + " (총 " + book.parts.partCnt + "화)";
                expander.Content = partPanel;

                wrapPanel.Children.Add(expander);

                // 완료시 이걸로!
                //wrapPanel.Background = Brushes.Green;

                window.jobProc.Children.Add(wrapPanel);
            }
            catch (Exception ex)
            {
                book.Error = ex.Message;
                window.AddLog("### 이게 보이면 issue 등록좀 ###");
                window.AddLog(book.Error);
                window.AddLog("################################");
                return false;
            }

            return true;
        }
    }
}
