using System;
using System.Text;

namespace FolderArchive
{
    static class Utill
    {
        public enum PROCESS_STAT
        {
            ALL_DONE = 1, // 한 폴더가 끝났을 때
            PART_DONE,    // n번째 파트 끝
            PROCESS,      // n번째 파트 압축중
            ERROR,        // 파트중 에러나면 폴더이름에 에러띄움
        }

        public enum STATS
        {
            ERROR = -1,
            WAIT = 0,
            DONE,
            EXCEPT
        }

        public static string Convert64(string msg)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(msg);
            string base64Code = Convert.ToBase64String(bytes);
            string filter1 = base64Code.Replace("=", "_");
            string filter2 = filter1.Replace("+", "_");
            string filter3 = filter2.Replace("/", "_");
            return "b" + filter3;            
        }
    }
}
