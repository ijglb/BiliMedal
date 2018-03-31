using BiliMedalCollection.Models;
using Pomelo.AspNetCore.TimedJob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BiliMedalCollection
{
    public class MedalCollectionJob : Job
    {
        const int StartRoom = 10000;
        const int EndRoom = 11111111;

        [Invoke(Begin = "2018-03-31 00:00", Interval = 500, SkipWhileExecuting = true)]
        public void CollectNewRoom(DbEntitys dbEntitys)
        {
            long room = StartRoom;
            var maxRoom = dbEntitys.Medals.OrderByDescending(m => m.RoomID).FirstOrDefault();
            if (maxRoom != null)
                room = maxRoom.RoomID + 1;
            string medalName = Utils.BiliBili.GetRoomMedal(room);

            dbEntitys.Medals.Add(new Medal { RoomID = room, MedalName = medalName, LastSearchTime = DateTime.Now, LastUpdateTime = DateTime.Now });
            dbEntitys.SaveChanges();
        }

        [Invoke(Begin = "2018-03-31 00:00", Interval = 2000, SkipWhileExecuting = true)]
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
            bool desc = new Random().Next(2) == 1 ? true : false;
            var roomFilter = dbEntitys.Medals.Where(m => string.IsNullOrEmpty(m.MedalName) && DateTime.Now - m.LastSearchTime > TimeSpan.FromDays(5));
            roomFilter = desc ? roomFilter.OrderByDescending(m => m.RoomID) : roomFilter.OrderBy(m => m.RoomID);
            var room = roomFilter.FirstOrDefault();
            if (room != null)
            {
                room.LastSearchTime = DateTime.Now;
                string medalName = Utils.BiliBili.GetRoomMedal(room.RoomID);
                if (!string.IsNullOrEmpty(medalName))
                {
                    room.MedalName = medalName;
                    room.LastUpdateTime = DateTime.Now;
                }
                dbEntitys.SaveChanges();
            }
        }
    }
}
