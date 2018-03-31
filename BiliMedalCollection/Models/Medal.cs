using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BiliMedalCollection.Models
{
    public class Medal
    {
        [Key]
        public long RoomID { get; set; }

        public string MedalName { get; set; }

        public DateTime LastSearchTime { get; set; }

        public DateTime LastUpdateTime { get; set; }
    }
}
