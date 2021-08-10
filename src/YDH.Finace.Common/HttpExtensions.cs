using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace YDH.Finace.Common
{
    /// <summary>
    /// Http相关扩展
    /// </summary>
    public static class HttpExtensions
    {
        /// <summary>
        /// 检查path开头是否与value相符（不分大小写）
        /// </summary>
        /// <param name="path"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool StartsWith(this PathString path, string value)
        {
            return path.Value?.StartsWith(value, StringComparison.OrdinalIgnoreCase) ?? false;
        }
        /// <summary>
        /// 是否WebApi("/api/")
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool IsWebApi(this PathString path)
        {
            return path.StartsWith("/api/");
        }

        /// <summary>
        /// GET请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="Timeout"></param>
        /// <returns></returns>
        public static string HttpGet(string Url, string contentType)
        {
                string retString = string.Empty;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
                request.Method = "GET";
                request.ContentType = contentType;

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream myResponseStream = response.GetResponseStream();
                StreamReader streamReader = new StreamReader(myResponseStream);
                retString = streamReader.ReadToEnd();
                streamReader.Close();
                myResponseStream.Close();
                return retString;
        
        }

    }
}
