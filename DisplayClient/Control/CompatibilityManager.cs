using Remote_Content_Show_Container;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DisplayClient
{
    public static class CompatibilityManager
    {
        public static bool IsCompatibleImage(FileResource resource)
        {
            string[] supported = new string[] { ".jpg", ".png", ".bmp", ".gif" };
            bool compatible = false;

            foreach (string extension in supported)
            {
                if (resource.Path.EndsWith(extension))
                {
                    return true;
                }
            }

            return compatible;
        }

        public static bool IsCompatibleVideo(FileResource resource)
        {
            string[] supported = new string[] { ".mp4" };
            bool compatible = false;

            foreach (string extension in supported)
            {
                if (resource.Path.EndsWith(extension))
                {
                    return true;
                }
            }

            return compatible;
        }

        public static async Task<bool> IsAvailableWebResource(string url)
        {
            if (url.StartsWith("http://") || url.StartsWith("https://"))
            {
                /*HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

                HttpWebResponse response = (HttpWebResponse)(await request.GetResponseAsync());

                return response.StatusCode == HttpStatusCode.OK;*/

                using (HttpClient client = new HttpClient())
                using (HttpResponseMessage response = await client.GetAsync(url))
                {
                    return response.StatusCode == HttpStatusCode.OK;
                }
            }            

            return false;
        }
    }
}
