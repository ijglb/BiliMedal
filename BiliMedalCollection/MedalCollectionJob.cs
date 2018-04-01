using BiliMedalCollection.Models;
using Pomelo.AspNetCore.TimedJob;
using System;
using System.Linq;

namespace BiliMedalCollection
{
    public class MedalCollectionJob : Job
    {
        const int StartRoom = 10000;
        const int EndRoom = 11111111;
        static bool _EndCollectNew = false;
        static long _WithoutMedalRoom = 0;

        [Invoke(Begin = "2018-03-31 00:00", Interval = 500, SkipWhileExecuting = true)]
        public void CollectNewRoom(DbEntitys dbEntitys)
        {
            if (_EndCollectNew)
                return;
            long room = StartRoom;
            var maxRoom = dbEntitys.Medals.OrderByDescending(m => m.RoomID).FirstOrDefault();
            if (maxRoom != null)
                room = maxRoom.RoomID + 1;
            if (room > EndRoom)
            {
                _EndCollectNew = true;
                return;
            }
            string medalName = Utils.BiliBili.GetRoomMedal(room);

            dbEntitys.Medals.Add(new Medal { RoomID = room, MedalName = medalName, LastSearchTime = DateTime.Now, LastUpdateTime = DateTime.Now });
            dbEntitys.SaveChanges();
        }

        [Invoke(Begin = "2018-03-31 00:00", Interval = 5000, SkipWhileExecuting = true)]
        public void CheckOldRoom(DbEntitys dbEntitys)
        {
            var room = dbEntitys.Medals.Where(m => !string.IsNullOrEmpty(m.MedalName) && DateTime.Now - m.LastSearchTime > TimeSpan.FromDays(10)).OrderBy(m => m.RoomID).FirstOrDefault();
            if (room != null)
            {
                room.LastSearchTime = DateTime.Now;
                string medalName = Utils.BiliBili.GetRoomMedal(room.RoomID);
                if (!string.IsNullOrEmpty(medalName) && room.MedalName != medalName)
                {
                    room.MedalName = medalName;
                    room.LastUpdateTime = DateTime.Now;
                }
                dbEntitys.SaveChanges();
            }
        }

        [Invoke(Begin = "2018-03-31 00:00", Interval = 1000, SkipWhileExecuting = true)]
        public void CheckWithoutMedalRoom(DbEntitys dbEntitys)
        {
            if (_WithoutMedalRoom == 0)//重启后
            {
                //搜索时间倒序，取上一次的最后一个
                var first = dbEntitys.Medals.Where(m => string.IsNullOrEmpty(m.MedalName)).OrderByDescending(m => m.LastSearchTime).FirstOrDefault();
                if (first != null)
                    _WithoutMedalRoom = first.RoomID;
            }
            if (_WithoutMedalRoom == 0)
                return;
            //常规，取房号大于上一次搜索的第一个
            var current = dbEntitys.Medals.Where(m => string.IsNullOrEmpty(m.MedalName) && m.RoomID > _WithoutMedalRoom).OrderBy(m => m.RoomID).FirstOrDefault();
            if (current == null)
            {
                _WithoutMedalRoom = StartRoom;//未找到更大的房号，说明已经到末尾，复原从头开始搜索
                return;
            }

            _WithoutMedalRoom = current.RoomID;
            current.LastSearchTime = DateTime.Now;
            string medalName = Utils.BiliBili.GetRoomMedal(current.RoomID);
            if (!string.IsNullOrEmpty(medalName))
            {
                current.MedalName = medalName;
                current.LastUpdateTime = DateTime.Now;
            }
            dbEntitys.SaveChanges();
        }
    }
}
