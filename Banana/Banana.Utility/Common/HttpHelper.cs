/***********************************
 * Developer: Lio.Huang
 * Date：2018-11-21
 **********************************/

using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace Banana.Utility.Common
{
    /// <summary>
    /// Http Method Helper
    /// </summary>
    public static class HttpHelper
    {
        private static HttpClient _instance = null;
        public static HttpClient GetClient()
        {
            _instance = _instance ?? new HttpClient();
            return _instance;
        }

        /// <summary>
        /// Get Method
        /// </summary>
        public static async Task<T> Get<T>(string url)
        {
            try
            {
                var client = GetClient();
                var responseMsg = await client.GetAsync(url);
                if (responseMsg.IsSuccessStatusCode)
                {
                    string strJson = await responseMsg.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<T>(strJson);
                }
                else
                {
                    return default(T);
                }
            }
            catch
            {
                _instance = new HttpClient();
                return default(T);
            }
        }

        /// <summary>
        /// Post Method
        /// </summary>
        public static async Task<T> Post<T>(string url, object para, string contentType = "application/json")
        {
            try
            {
                if (para != null)
                {
                    var requestJson = JsonConvert.SerializeObject(para);
                    HttpContent httpContent = new StringContent(requestJson);
                    httpContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
                    var client = GetClient();

                    var responseJson = await client.PostAsync(url, httpContent).Result.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<T>(responseJson);
                }
                return default(T);
            }
            catch
            {
                _instance = new HttpClient();
                return default(T);
            }
        }
    }
}
