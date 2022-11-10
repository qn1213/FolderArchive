﻿using FolderArchive.UI;
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

        public int bookIndex = 0;
        
        protected UIwindow1 window;

        private Dictionary<int, Book> dic_book;
        private List<Book> list_exception_book;

        private bool isFisrt = true;

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
            // 싱글 스레드용
            StartLowSpec();
        }

        // 그냥 순차적으로 싱글쓰레드로 압축시키는 방식
        private void StartLowSpec()
        {
            Dictionary<int, Book>.KeyCollection keys = dic_book.Keys;
            foreach(int key in keys)
            {
                dic_book[key].ChangeStatus(Utill.PROCESS_STAT.PROCESS);

                string outputBookFolderName = $"{outPutPath}\\{dic_book[key].bookName}";
                DirectoryInfo outputBookFolder = new DirectoryInfo(outputBookFolderName);
                if (!outputBookFolder.Exists)
                    outputBookFolder.Create();

                Part part = dic_book[key].parts;
                for (int i = 0; i < part.partCnt; i++)
                {
                    string outputFileName = $"{outputBookFolderName}\\{part.partPathWithName[part.partPath[i]]}.zip";
                    FileInfo checkFile = new FileInfo(outputFileName);
                    if(checkFile.Exists)
                    {
                        dic_book[key].ChangeStatus(Utill.PROCESS_STAT.ERROR, i);
                        window.AddLog($"{part.partPath[i]} => 파일이 이미 존재합니다.");
                        continue;
                    }

                    try
                    {
                        ZipFile.CreateFromDirectory(part.partPath[i], outputFileName);
                        dic_book[key].ChangeStatus(Utill.PROCESS_STAT.PART_DONE, i);
                    }
                    catch (Exception ex)
                    {
                        dic_book[key].ChangeStatus(Utill.PROCESS_STAT.ERROR, i);
                        window.AddLog($"{part.partPath[i]} is error. => {ex.Message}");
                    }
                }

                dic_book[key].ChangeStatus(Utill.PROCESS_STAT.ALL_DONE);
            }
        }

        private async Task<int> Zip()
        {
            var task = new Task<int>(() =>
            {
                int sum = 0;

                return sum;
            });

            task.Start();
            await task;

            return task.Result;
        }

        public void Init()
        {
            if (!isFisrt)
            {
                window.jobProc.Children.Clear();
                InitPath();
                bookIndex = 0;
                dic_book.Clear();
                list_exception_book.Clear();
            }
            isFisrt = false;
        }

        public void SetBookObject()
        {
            Init();

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

            if(list_exception_book.Count > 0)
            {
                window.AddLog("################ 로드 실패한 폴더 ################");
                logIndex = 1;
                foreach (Book items in list_exception_book)
                {
                    window.AddLog(logIndex++ + ". : " + items.bookPath + " => " + items.Error);
                }
            }            
        }

        private bool AddObject(DirectoryInfo rootInfo, ref Book book)
        {
            try
            {
                DirectoryInfo[] part = rootInfo.GetDirectories("*", SearchOption.TopDirectoryOnly);
                if(part.Length <= 0)
                {
                    book.bookPath = rootInfo.FullName;
                    book.Error = "Part count is 0. It's excluded.";
                    return false;
                }

                book.SetObjectBookInfo(ref part, rootInfo.Name, rootInfo.FullName, ref window);
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
