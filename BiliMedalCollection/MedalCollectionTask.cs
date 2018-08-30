using BiliMedalCollection.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace BiliMedalCollection
{
    public class MedalCollectionTask
    {
        const int StartRoom = 10000;
        const int EndRoom = 15000000;
        static bool _EndCollectNew = false;
        static long _WithoutMedalRoom = 0;

        public static void StartWork()
        {
            //进入WAL模式
            using (DbEntitys dbEntitys = new DbEntitys())
            using (var connection = dbEntitys.Database.GetDbConnection())
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "PRAGMA journal_mode=WAL;";
                    string result = command.ExecuteScalar() as string;
                    Console.WriteLine("切换journal_mode结果：" + result);
                }
            }

            Task.Factory.StartNew(CollectNewRoom);
            Task.Factory.StartNew(CheckOldRoom);
            Task.Factory.StartNew(CheckWithoutMedalRoom);
        }

        private static void CollectNewRoom()
        {
            while (true)
            {
                System.Threading.Thread.Sleep(1000);
                using (DbEntitys dbEntitys = new DbEntitys())
                {
                    if (_EndCollectNew)
                        continue;
                    long room = StartRoom;
                    var maxRoom = dbEntitys.Medals.OrderByDescending(m => m.RoomID).FirstOrDefault();
                    if (maxRoom != null)
                        room = maxRoom.RoomID + 1;
                    if (room > EndRoom)
                    {
                        _EndCollectNew = true;
                        continue;
                    }
                    string medalName = Utils.BiliBili.GetRoomMedal(room);

                    dbEntitys.Medals.Add(new Medal { RoomID = room, MedalName = medalName, LastSearchTime = DateTime.Now, LastUpdateTime = DateTime.Now });
                    dbEntitys.SaveChanges();
                }
            }
        }

        private static void CheckOldRoom()
        {
            while (true)
            {
                System.Threading.Thread.Sleep(4000);
                using (DbEntitys dbEntitys = new DbEntitys())
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
            }
        }

        private static void CheckWithoutMedalRoom()
        {
            while (true)
            {
                System.Threading.Thread.Sleep(1000);
                using (DbEntitys dbEntitys = new DbEntitys())
                {
                    if (_WithoutMedalRoom == 0)//重启后
                    {
                        //搜索时间倒序，取上一次的最后一个
                        var first = dbEntitys.Medals.Where(m => string.IsNullOrEmpty(m.MedalName)).OrderByDescending(m => m.LastSearchTime).FirstOrDefault();
                        if (first != null)
                            _WithoutMedalRoom = first.RoomID;
                    }
                    if (_WithoutMedalRoom == 0)
                        continue;
                    //常规，取房号大于上一次搜索的第一个
                    var current = dbEntitys.Medals.Where(m => string.IsNullOrEmpty(m.MedalName) && m.RoomID > _WithoutMedalRoom).OrderBy(m => m.RoomID).FirstOrDefault();
                    if (current == null)
                    {
                        _WithoutMedalRoom = StartRoom;//未找到更大的房号，说明已经到末尾，复原从头开始搜索
                        continue;
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
    }
}
