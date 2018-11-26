/***********************************
 * Coder：EminemJK
 * Date：2018-11-26
 **********************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;

namespace Banana.Utility.Encryption
{
    /// <summary>
    /// DES加密解密类
    /// </summary>
    public class DigestHelper
    {
        /// <summary>
        /// 密钥 8位
        /// </summary>
        private const string KEY = "banana07";

        /// <summary>
        /// 偏移量 8位
        /// </summary>
        private const string IV = "eminemjk";

        /// <summary>
        /// 密钥长度，无需修改
        /// </summary>
        private const int KeyLengt = 24;

        /// <summary>
        /// DES 加密
        /// </summary>
        public static string DesEncrypt(string sourceString, string key = KEY, string iv = IV)
        {
            try
            {
                byte[] btKey = Encoding.UTF8.GetBytes(key);

                byte[] btIV = Encoding.UTF8.GetBytes(iv);

                byte[] allKey = BufferCopy(KeyLengt, btKey);

                var des = TripleDES.Create();

                using (MemoryStream ms = new MemoryStream())
                {
                    byte[] inData = Encoding.UTF8.GetBytes(sourceString);
                    try
                    {
                        using (CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(allKey, btIV), CryptoStreamMode.Write))
                        {
                            cs.Write(inData, 0, inData.Length);

                            cs.FlushFinalBlock();
                        }

                        return Convert.ToBase64String(ms.ToArray());
                    }
                    catch
                    {
                        return sourceString;
                    }
                }
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// DES 解密
        /// </summary>
        public static string DesDecrypt(string encryptedString, string key = KEY, string iv = IV)
        {
            byte[] btKey = Encoding.UTF8.GetBytes(key);

            byte[] btIV = Encoding.UTF8.GetBytes(iv);

            byte[] allKey = BufferCopy(KeyLengt, btKey);

            var des = TripleDES.Create();

            using (MemoryStream ms = new MemoryStream())
            {
                byte[] inData = Convert.FromBase64String(encryptedString);
                try
                {
                    using (CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(allKey, btIV), CryptoStreamMode.Write))
                    {
                        cs.Write(inData, 0, inData.Length);

                        cs.FlushFinalBlock();
                    }

                    return Encoding.UTF8.GetString(ms.ToArray());
                }
                catch (Exception ex)
                {
                    return encryptedString;
                }

            }
        }

        private static byte[] BufferCopy(int iLengt, byte[] sourceByte)
        {
            byte[] allKey = new byte[iLengt];
            Buffer.BlockCopy(sourceByte, 0, allKey, 0, 8);
            Buffer.BlockCopy(sourceByte, 0, allKey, 8, 8);
            Buffer.BlockCopy(sourceByte, 0, allKey, 16, 8);
            return allKey;
        }
    }
}
