using System;
using System.IO;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Linq;
using System.Text;
using System.Net;
using System.Reflection;


namespace RITS.StrymonEditor.AutoUpdate
{
    /// <summary>
    /// Helper methods for dealing with HTTP requests and responses
    /// </summary>
    public static class HttpUtils
    {
        /// <summary>
        /// Save the specified url to the specified target path using binary writer, used mostly for binary file types like images
        /// </summary>
        /// <param name="sourceUrl"></param>
        /// <param name="targetPath"></param>
        public static bool SaveUrl(string sourceUrl, string targetPath)
        {
            using (HttpWebResponse response = Request(sourceUrl).GetResponse() as HttpWebResponse)
            {
                Stream responseStream = null;
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    responseStream = response.GetResponseStream();

                    using (FileStream file = new FileStream(targetPath, FileMode.Create))
                    {
                        byte[] buffer = new byte[32 * 1024];
                        int read;

                        while ((read = responseStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            file.Write(buffer, 0, read);
                        }
                    }
                    return true;
                }
            }
            return false;

        }

        // HttpWebRequest helper
        private static HttpWebRequest Request(string url)
        {
            Uri uri = new Uri(url);
            HttpWebRequest request = WebRequest.Create(uri) as HttpWebRequest;
            request.Accept = "text/html, image/gif, image/jpeg, image/pjpeg, image/pjpeg, application/x-shockwave-flash, application/x-silverlight, application/x-ms-application, application/x-ms-xbap, application/vnd.ms-xpsdocument, application/xaml+xml, application/vnd.ms-excel, application/vnd.ms-powerpoint, application/msword, */*";
            request.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 5.1; Trident/4.0; .NET CLR 2.0.50727; .NET CLR 3.0.04506.648; .NET CLR 3.5.21022; .NET CLR 1.1.4322; .NET CLR 3.0.4506.2152; .NET CLR 3.5.30729)";
            request.Timeout = 10000; // TODO configure timeout
            return request;
        }        
    }
}
