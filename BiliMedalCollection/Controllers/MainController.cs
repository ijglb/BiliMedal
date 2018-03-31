using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BiliMedalCollection.Models;
using Microsoft.AspNetCore.Mvc;

namespace BiliMedalCollection.Controllers
{
    public class MainController : Controller
    {
        private readonly DbEntitys _DbEntitys;
        public MainController(DbEntitys dbEntitys)
        {
            _DbEntitys = dbEntitys;
        }

        [HttpGet, Route("/Count")]
        public JsonResult Count()
        {
            var count = _DbEntitys.Medals.Where(m => !string.IsNullOrEmpty(m.MedalName)).Count();
            return Json(new { Count = count });
        }

        [HttpGet, Route("/Search")]
        public JsonResult Search(string keyWord)
        {
            List<Medal> list = new List<Medal>();
            if (string.IsNullOrEmpty(keyWord))
                return Json(list);
            keyWord = keyWord.ToUpper();
            list = _DbEntitys.Medals.Where(m => !string.IsNullOrEmpty(m.MedalName) && m.MedalName.ToUpper().Contains(keyWord)).ToList();
            return Json(list);
        }

        [HttpGet, Route("/GetUpdateInfo")]
        public JsonResult GetUpdateInfo()
        {
            var list = _DbEntitys.Medals.Where(m => !string.IsNullOrEmpty(m.MedalName))
                .OrderByDescending(m => m.LastUpdateTime).Take(10).ToList();
            return Json(list);
        }
    }
}
