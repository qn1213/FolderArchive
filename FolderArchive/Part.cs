using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace FolderArchive
{
    internal class Part
    {
        public string name; // 파트 이름 (1화, 2화)
        public string path; // 저장경로/책이름/파트

        public int count; // 한파트에 몇장 있는가

        public Label lb_Name; // 파트 레이블

        public bool isDone; // 끝났는지 여부

        public Part()
        {

        }

    }
}
