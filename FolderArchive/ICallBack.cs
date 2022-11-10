using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolderArchive
{
    internal interface ICallBack
    {
        void ChangeStatus(Utill.PROCESS_STAT stat, int partIndex = 0);
    }

    internal interface ILogs
    {
        void Log(string log);
    }
}
