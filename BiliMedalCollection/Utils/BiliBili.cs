using System;

namespace BiliMedalCollection.Utils
{
    public class BiliBili
    {
        public static string GetRoomMedal(long roomID)
        {
            try
            {
                HttpHelper http = new HttpHelper()
                .AddHeaders("Referer", string.Format("http://live.bilibili.com/{0}", roomID))
                .AddHeaders("Origin", "http://live.bilibili.com");
                //同步
                var json = http.GetJObjectAsync(string.Format("http://live.bilibili.com/liveact/ajaxGetMedalRankList?roomid={0}", roomID)).Result;
                if (json["data"]["list"].HasValues)
                    return json["data"]["list"][0]["medalName"].ToString();
                else
                    return null;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
