using System;

namespace BiliMedalCollection.Utils
{
    public class BiliBili
    {
        public static string GetRoomMedal(long roomID)
        {
            try
            {
                using (HttpHelper http = new HttpHelper())
                {
                    http.AddHeaders("Referer", string.Format("https://live.bilibili.com/{0}", roomID))
                        .AddHeaders("Origin", "https://live.bilibili.com");
                    //取uid
                    var json = http.GetJObjectAsync(string.Format("https://api.live.bilibili.com/room/v1/Room/get_info?room_id={0}", roomID)).Result;
                    if (json["code"].ToString() != "0")//错误，未找到房间
                        return null;
                    string uid = json["data"]["uid"].ToString();
                    //通过uid取粉丝榜
                    json = http.GetJObjectAsync(string.Format("https://api.live.bilibili.com/rankdb/v1/RoomRank/webMedalRank?ruid={0}", uid)).Result;
                    if (json["data"]["list"].HasValues)
                        return json["data"]["list"][0]["medal_name"].ToString();
                    else
                        return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
