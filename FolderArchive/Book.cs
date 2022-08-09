using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace FolderArchive
{
    internal class Book
    {
        public string name; // 책 이름
        public string path; // 저장경로/책이름

        public List<Part> parts { get; private set; } // 한개의 책이 몇개의 파트를 가지고 있는가 저장용
        public int count { get; private set; } // 파트가 몇개인가

        public Book(string name, string path)
        {
            this.name = name;
            this.path = path;
            parts = new List<Part>();
        }

        public void AddParts(Part part)
        {
            parts.Add(part);
        }
    }
}
