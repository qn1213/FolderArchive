using System;
using System.Text;

namespace FolderArchive
{
    static class Utill
    {
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
