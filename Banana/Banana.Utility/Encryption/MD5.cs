/***********************************
 * Coder：EminemJK
 * Date：2018-11-26
 **********************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banana.Utility.Encryption
{
    public static class MD5
    {
        /// <summary>
        /// 计算字符MD5加密值
        /// </summary>
        /// <param name="str">要计算的字符串</param>
        /// <param name="code">MD5的加密位数</param>
        /// <returns></returns>
        public static string Encrypt(string str, int code = 32)
        {
            string md5code = string.Empty;
            using (var md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] retVal = md5.ComputeHash(Encoding.UTF8.GetBytes(str));
                StringBuilder md5sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    md5sb.Append(retVal[i].ToString("x2"));
                }
                md5code = md5sb.ToString();
            }

            if (code == 16) //16位MD5加密（取32位加密的9~25字符） 
            {
                return md5code.Substring(8, 16);
            }

            if (code == 32) //32位加密 
            {
                return md5code;
            }

            return md5code;
        }
    }
}
