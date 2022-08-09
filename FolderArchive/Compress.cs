using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolderArchive
{
    internal class Compress
    {
        private string outPutPath;
        private string inputPaht;

        protected MainWindow mainWindow;
        private Dictionary<string, Book> book;

        public Compress(MainWindow window, string outPut, string inPut)
        {
            this.book = new Dictionary<string, Book>();

            this.mainWindow = window;
            this.outPutPath = outPut;
            this.inputPaht = inPut;
        }

        public bool Start()
        {

            return true;
        }

        // 인풋 폴더안에 있는 만화, 화수가져와서 객체화하기
        private void GetBookandParts()
        {

        }
    }
}
