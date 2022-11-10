using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Xml.Linq;

namespace FolderArchive
{
    internal class Book
    {
        public enum BASE_TYPE
        {
            BOOK = 1,
            PARTS
        }

        public int index = 0;
        public int status = 0; // 0 = 대기, 1 = 완료, -1 = 오류

        public string bookName = null;
        public string bookName64 = null;
        public string bookPath = null;

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
        }

        public void SetBook(string name, string path)
        {
            status = 0;
            bookName = name;
            bookName64 = Utill.Convert64(bookName);
            bookPath = path;
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
    }

    class Part
    {
        public List<string> partName;
        public Dictionary<string, string> partNameWith64;

        public int partCnt = 0;
        public List<string> partPath;

        public Part()
        {
            partName = new List<string>();

            partNameWith64 = new Dictionary<string, string>();

            partPath = new List<string>();
        }

        public void AddPartInfo(string name, string path)
        {
            partName.Add(name);

            partNameWith64.Add(name, Utill.Convert64(name));

            partPath.Add(path);
        }

        public string GetPartName64(string partName)
        {
            return partNameWith64.ContainsKey(partName) ? partNameWith64[partName] : null;
        }
    }
}
