using System.Web.Mvc;
using Web_Tour.Models;
using System.Linq;
using System;
using HtmlAgilityPack;

namespace Web_Tour.Controllers
{
    public class UserController : Controller
    {
        databaseDataContext data = new databaseDataContext("");

        public ActionResult Home()
        {
            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Tours()
        {
            var tour = from i in data.TOURs select i;

            return View(tour);
        }

        public string ConvertHtmlToPlainText(string html)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);
            string plainText = doc.DocumentNode.InnerText;

            return plainText;
        }

        public ActionResult News()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }

        public ActionResult Auth()
        {
            return View();
        }
    }
}