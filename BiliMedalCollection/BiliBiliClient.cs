using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace BiliMedalCollection
{
    public class BiliBiliClient
    {
        public HttpClient Client { get; private set; }

        public BiliBiliClient(HttpClient httpClient)
        {
            httpClient.BaseAddress = new Uri("http://api.live.bilibili.com/");//.net core 2.1.3 在centos下SSL报错，暂时使用http
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/65.0.3325.181 Safari/537.36");
            httpClient.DefaultRequestHeaders.Add("Origin", "https://live.bilibili.com");
            Client = httpClient;
        }

        public async Task<JObject> GetJObjectAsync(string url)
        {
            string jsonStr = await Client.GetStringAsync(url);
            return JsonConvert.DeserializeObject<JObject>(jsonStr);
        }
        /// <summary>
        /// 获取房间勋章 无则返回null
        /// </summary>
        /// <param name="roomID"></param>
        /// <returns></returns>
        public async Task<string> GetRoomMedal(long roomID)
        {
            try
            {
                Client.DefaultRequestHeaders.Referrer = new Uri($"https://live.bilibili.com/{roomID}");
                //取uid
                var json = await GetJObjectAsync($"/room/v1/Room/get_info?room_id={roomID}");
                if (json["code"].ToString() != "0")//错误，未找到房间
                    return null;
                string uid = json["data"]["uid"].ToString();
                //通过uid取粉丝榜
                json = await GetJObjectAsync($"/rankdb/v1/RoomRank/webMedalRank?ruid={uid}");
                if (json["data"]["list"].HasValues)
                    return json["data"]["list"][0]["medal_name"].ToString();
                else
                    return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }
    }
}
