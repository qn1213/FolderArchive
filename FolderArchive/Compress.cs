using FolderArchive.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FolderArchive
{
    internal class Compress
    {
        private string outPutPath;
        private string inputPaht;

        protected UIwindow1 window;
        private Dictionary<string, Book> book;

        public Compress(UIwindow1 window, string outPut, string inPut)
        {
            this.book = new Dictionary<string, Book>();

            this.window = window;

            this.outPutPath = outPut;
            this.inputPaht = inPut;
        }

        public void Start()
        {
            // 인풋 폴더안에 있는 만화, 화수 가져와서 객체화해서 뿌리기
            GetBookandParts();


        }

        private void GetBookandParts()
        {

        }
    }
}
