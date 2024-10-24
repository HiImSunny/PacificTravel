using System.Web.Mvc;
using Web_Tour.Models;
using System.Linq;
using System;
using HtmlAgilityPack;
using System.Web.UI;
using PagedList;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Text;

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

        public ActionResult Tours(string sort, int page = 1)
        {
            var tours = data.TOURs.Where(t => t.ID_TRANG_THAI == 1).AsQueryable();

            switch (sort)
            {
                case "min-price":
                    tours = tours.OrderBy(t => t.GIA_NGUOI_LON); // Sort by lowest price
                    break;
                case "popular":
                    tours = tours.OrderByDescending(t => t.GIAM_GIA); // Sort by most popular (you can adjust the logic)
                    break;
                default:
                    tours = tours.OrderBy(t => t.NGAY_DI); // Sort by latest date
                    break;
            }

            // Pagination logic
            int pageSize = 5; // or any number you prefer
            var pagedTours = tours.ToPagedList(page, pageSize);

            return View(pagedTours);
        }

        public ActionResult SearchTour(string searchName, int ? page)
        {
            ViewBag.searchName = searchName;

            if (!String.IsNullOrEmpty(searchName))
            {
                var tour = data.TOURs.Where(i => i.TEN_TOUR.Contains(searchName) || i.TEN_TOUR.Contains(searchName)); // TODO: XÓA DẤU

                int pageNumber = (page ?? 1);
                int pageSize = 5;

                return View(tour.ToList().OrderBy(n => n.NGAY_DI).ToPagedList(pageNumber, pageSize));
            }

            return View();
        }

        public string ConvertHtmlToPlainText(string html)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);
            string plainText = doc.DocumentNode.InnerText;

            return plainText;
        }

        public ActionResult News(int? page)
        {
            int pageNumber = (page ?? 1);
            int pageSize = 5;

            return View(data.BAI_VIETs.ToList().OrderBy(n => n.THOI_GIAN_TAO).ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Article(int id)
        {
            BAI_VIET baiViet = data.BAI_VIETs.SingleOrDefault(i => i.ID_BAI_VIET == id);

            return View(baiViet);
        }

        public ActionResult Contact()
        {
            return View();
        }

        public ActionResult Auth()
        {
            return View();
        }

        public string RemoveDiacritics(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            // Thay thế các ký tự có dấu bằng ký tự không dấu
            string normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (char c in normalizedString)
            {
                // Lấy ký tự không dấu
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}