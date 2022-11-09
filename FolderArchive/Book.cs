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
        public int index = 0;
        public int status = 0; // 0 = 대기, 1 = 완료, -1 = 오류

        public string bookName = null;

        public string partName = null;
        public int partCnt = 0;

        public string error = null;
    }
}
