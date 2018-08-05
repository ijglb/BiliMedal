using BiliMedalCollection.Utils.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BiliMedalCollection.Utils
{
    public class HttpHelper: HttpClient
    {
        private HttpContent _httpContent;

        public HttpHelper()
        {
            base.DefaultRequestHeaders.Clear();
            base.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/65.0.3325.181 Safari/537.36");
            _httpContent = new StringContent("");
        }
        /// <summary>
        /// 添加请求头
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public HttpHelper AddHeaders(string name, string value)
        {
            base.DefaultRequestHeaders.Add(name, value);
            return this;
        }
        /// <summary>
        /// 添加请求内容
        /// </summary>
        /// <param name="httpContent"></param>
        /// <returns></returns>
        public HttpHelper AddHttpContent(HttpContent httpContent)
        {
            _httpContent = httpContent;
            return this;
        }
        /// <summary>
        /// 添加json实体请求内容
        /// </summary>
        /// <param name="jsonContent"></param>
        /// <returns></returns>
        public HttpHelper AddJsonContent(object jsonObject) => AddHttpContent(new JsonContent(jsonObject));
        /// <summary>
        /// 添加字符串请求内容
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public HttpHelper AddStringContent(string str) => AddHttpContent(new StringContent(str, Encoding.UTF8, "application/x-www-form-urlencoded"));

        public async Task<JObject> GetJObjectAsync(string url)
        {
            string jsonStr = await base.GetStringAsync(url);
            return JsonConvert.DeserializeObject<JObject>(jsonStr);
        }

        public async Task<HttpResponseMessage> PostAsync(string url) => await base.PostAsync(url, _httpContent);

        public async Task<string> PostStringAsync(string url)
        {
            var response = await PostAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<JObject> PostJObjectAsync(string url)
        {
            string jsonStr = await PostStringAsync(url);
            return JsonConvert.DeserializeObject<JObject>(jsonStr);
        }
    }
}
