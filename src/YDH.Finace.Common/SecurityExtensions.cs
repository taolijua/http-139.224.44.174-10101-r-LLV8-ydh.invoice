using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace YDH.Finace.Common
{
    public static class SecurityExtensions
    {
        /// <summary>
        /// 空字节数组
        /// </summary>
        private static readonly byte[] EmptyBytes = new byte[0];

        #region MD5
        /// <summary>
        /// 为字符串生成MD5摘要（Base64字符串）
        /// </summary>
        /// <param name="srcText"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string MD5Base64(this string srcText, Encoding encoding = null)
        {
            var result = string.Empty;
            if (!string.IsNullOrEmpty(srcText))
            {
                encoding ??= Encoding.UTF8;
                var bytes = MD5Hash(encoding.GetBytes(srcText));
                result = bytes.ToBase64String();
            }
            return result;
        }
        /// <summary>
        /// 为字符串生成MD5摘要(16进制小写)
        /// </summary>
        /// <param name="srcText"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string MD5Hex(this string srcText, Encoding encoding = null)
        {
            var result = string.Empty;
            if (!string.IsNullOrEmpty(srcText))
            {
                encoding ??= Encoding.UTF8;
                var bytes = MD5Hash(encoding.GetBytes(srcText));
                result = bytes.Aggregate(new StringBuilder(), (builder, @byte) => builder.Append(@byte.ToString("x2"))).ToString();
            }
            return result;
        }
        /// <summary>
        /// 为字符串生成MD5摘要
        /// </summary>
        /// <param name="srcText"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static byte[] MD5Hash(this string srcText, Encoding encoding = null)
        {
            var result = EmptyBytes;
            encoding ??= Encoding.UTF8;
            if (!string.IsNullOrEmpty(srcText))
            {
                result = MD5Hash(encoding.GetBytes(srcText));
            }
            return result;
        }
        /// <summary>
        /// 为字节数组生成MD5摘要
        /// </summary>
        /// <param name="srcBytes"></param>
        /// <returns></returns>
        public static byte[] MD5Hash(this byte[] srcBytes)
        {
            var result = EmptyBytes;
            if (srcBytes != null && srcBytes.Length > 0)
            {
                using var md5 = MD5.Create();
                result = md5.ComputeHash(srcBytes);
            }
            return result;
        }
        #endregion

        #region SHA1
        /// <summary>
        /// 为字符串生成SHA1摘要（Base64字符串）
        /// </summary>
        /// <param name="srcText"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string SHA1Base64(this string srcText, Encoding encoding = null)
        {
            var result = string.Empty;
            if (!string.IsNullOrEmpty(srcText))
            {
                encoding ??= Encoding.UTF8;
                var bytes = SHA1Hash(encoding.GetBytes(srcText));
                result = bytes.ToBase64String();
            }
            return result;
        }
        /// <summary>
        /// 为字符串生成SHA1摘要（16进制大写字符串）
        /// </summary>
        /// <param name="srcText"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string SHA1Hex(this string srcText, Encoding encoding = null)
        {
            var result = string.Empty;
            if (!string.IsNullOrEmpty(srcText))
            {
                encoding ??= Encoding.UTF8;
                var bytes = SHA1Hash(encoding.GetBytes(srcText));
                result = bytes.Aggregate(new StringBuilder(), (builder, @byte) => builder.Append(@byte.ToString("X2"))).ToString();
            }
            return result;
        }
        /// <summary>
        /// 为字符串生成SHA1摘要， 默认为UTF8
        /// </summary>
        /// <param name="srcText"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static byte[] SHA1Hash(this string srcText, Encoding encoding = null)
        {
            byte[] result = EmptyBytes;
            encoding ??= Encoding.UTF8;
            if (!string.IsNullOrEmpty(srcText))
            {
                result = SHA1Hash(encoding.GetBytes(srcText));
            }
            return result;
        }
        /// <summary>
        /// 为字节数据生成SHA1摘要
        /// </summary>
        /// <param name="srcBytes"></param>
        /// <returns></returns>
        public static byte[] SHA1Hash(this byte[] srcBytes)
        {
            var result = EmptyBytes;
            if (srcBytes != null && srcBytes.Length > 0)
            {
                using var sha1 = new SHA1CryptoServiceProvider();
                result = sha1.ComputeHash(srcBytes);
            }
            return result;
        }
        #endregion

        #region AES128
        /// <summary>
        /// AES128加密
        /// </summary>
        /// <param name="text"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        public static string AESEncrypt(this string text, string key = null, string iv = null)
        {
            var result = string.Empty;
            if (!string.IsNullOrEmpty(text))
            {
                if (iv == null) iv = "ÿ";
                if (key == null) key = "þ";
                using var aes = new RijndaelManaged { KeySize = 128, BlockSize = 128, Mode = CipherMode.CBC, Padding = PaddingMode.PKCS7 };
                aes.IV = Encoding.UTF8.GetBytes(iv.PadRight(16)).Take(16).ToArray();
                aes.Key = Encoding.UTF8.GetBytes(key.PadRight(16)).Take(16).ToArray();
                using var transform = aes.CreateEncryptor();
                var textBytes = Encoding.UTF8.GetBytes(text);
                var bytes = transform.TransformFinalBlock(textBytes, 0, textBytes.Length);
                result = Convert.ToBase64String(bytes);
            }
            return result;
        }
        /// <summary>
        /// AES128解密
        /// </summary>
        /// <param name="secret"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        public static string AESDecrypt(this string secret, string key = null, string iv = null)
        {
            var result = string.Empty;
            if (!string.IsNullOrEmpty(secret))
            {
                if (iv == null) iv = "ÿ";
                if (key == null) key = "þ";
                using var aes = new RijndaelManaged { KeySize = 128, BlockSize = 128, Mode = CipherMode.CBC, Padding = PaddingMode.PKCS7 };
                aes.IV = Encoding.UTF8.GetBytes(iv.PadRight(16)).Take(16).ToArray();
                aes.Key = Encoding.UTF8.GetBytes(key.PadRight(16)).Take(16).ToArray();
                using var transform = aes.CreateDecryptor();
                var secretBytes = Convert.FromBase64String(secret);
                var bytes = transform.TransformFinalBlock(secretBytes, 0, secretBytes.Length);
                result = Encoding.UTF8.GetString(bytes);
            }
            return result;
        }
        #endregion

        #region base64
        /// <summary>
        /// 字符串 转换为 base64字符串
        /// </summary>
        /// <param name="text"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string ToBase64String(this string text, Encoding encoding = null)
        {
            var result = string.Empty;
            if (text.HasValue())
            {
                encoding ??= Encoding.UTF8;
                var bytes = encoding.GetBytes(text);
                result = bytes.ToBase64String();
            }
            return result;
        }

        /// <summary>
        /// 字节数组 转换为 base64字符串
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string ToBase64String(this byte[] bytes)
        {
            var result = string.Empty;
            if (bytes != null)
            {
                result = Convert.ToBase64String(bytes);
            }
            return result;
         }

        /// <summary>
        /// base64字符串 转换为 文本字符串
        /// </summary>
        /// <param name="text"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string Base64ToString(this string text, Encoding encoding = null)
        {
            var bytes = text.Base64ToBytes();
            if (bytes == null) return string.Empty;
            encoding ??= Encoding.UTF8;
            return encoding.GetString(bytes);
        }
        /// <summary>
        /// base64字符串 转换为 字节数组
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static byte[] Base64ToBytes(this string text)
        {
            var bytes = EmptyBytes;
            if (text.HasValue())
            {
                try
                {
                    bytes = Convert.FromBase64String(text);
                }
                catch { }
            }
            return bytes;

        }
        #endregion
    }
}
