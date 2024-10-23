using System.Web.Mvc;
using Web_Tour.Models;
using System.Linq;
using System.Web.WebPages;
using System;
using System.Diagnostics;

namespace Web_Tour.Controllers
{
    public class TourController : Controller
    {

        databaseDataContext data = new databaseDataContext("");

        public ActionResult Details(string id)
        {
            var tour = from tourIndex in data.TOURs where tourIndex.ID_TOUR == id select tourIndex;

            return View(tour.Single());
        }

        public ActionResult Booking(string id)
        {
            if (Session["account-info"] == null || Session["account-info"].ToString() == "")
            {
                Session["redirect-to"] = "/Booking?id=" + id;

                return RedirectToAction("Login", "Auth");
            }

            var tour = from tourIndex in data.TOURs where tourIndex.ID_TOUR == id select tourIndex;

            return View(tour.Single());
        }

        [HttpPost]
        public ActionResult Booking(FormCollection form)
        {
            THANH_TOAN thanhToan = new THANH_TOAN();

            thanhToan.ID_PHUONG_THUC = form["method"].AsInt();
            thanhToan.ID_TRANG_THAI = 1;
            thanhToan.SO_TIEN_DA_NHAN = 0;

            data.THANH_TOANs.InsertOnSubmit(thanhToan);
            data.SubmitChanges();

            DAT_TOUR datTour = new DAT_TOUR();
            datTour.ID_DAT_TOUR = form["ID_TOUR"] + "-0" + (CountBookedTour(form["ID_TOUR"]) + 1); // Ex: TOUR1-24122024-01
            datTour.THOI_GIAN_DAT = DateTime.Now;
            datTour.TONG_SO_TIEN = form["submit-total-price"].AsInt();
            datTour.ID_TOUR = form["ID_TOUR"];
            datTour.ID_THANH_TOAN = thanhToan.ID_THANH_TOAN;
            datTour.ID_THANH_VIEN = form["ID_THANH_VIEN"].AsInt();
            datTour.ID_LOAI = form["tour-type"].AsInt();

            data.DAT_TOURs.InsertOnSubmit(datTour);
            data.SubmitChanges();

            if (thanhToan.ID_PHUONG_THUC == 1)
                return Redirect("/Tour/CashPayment?id=" + datTour.ID_DAT_TOUR);
            else
                return Redirect("/Tour/VietQRPayment?id=" + datTour.ID_DAT_TOUR + "&amount=" + datTour.TONG_SO_TIEN);
        }

        private int CountBookedTour(string idTour)
        {
            return data.DAT_TOURs
                     .Where(d => d.ID_DAT_TOUR.StartsWith(idTour))
                     .Count();
        }
          
        public ActionResult BookingComplete(string id)
        {
            var datTour = data.DAT_TOURs.SingleOrDefault(i => i.ID_DAT_TOUR == id);

            ViewBag.tour = data.TOURs.SingleOrDefault(i => i.ID_TOUR == datTour.ID_TOUR);

            return View(datTour);
        }

        public ActionResult CashPayment(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction("Home", "User");
            }

            ViewBag.id = id;

            return View();
        }

        public ActionResult VietQRPayment(string id, string amount)
        {
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction("Home", "User");
            }

            ViewBag.id = id;
            ViewBag.amount = amount;

            return View();
        }
    }
}