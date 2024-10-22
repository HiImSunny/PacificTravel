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
            if (Session["account-info"] != null || Session["account-info"].ToString() != "")
                return RedirectToAction("Login", "Auth");

            var tour = from tourIndex in data.TOURs where tourIndex.ID_TOUR == id select tourIndex;

            return View(tour.Single());
        }

        [HttpPost]
        public ActionResult Booking(FormCollection form, TOUR tour, THANH_VIEN thanhVien)
        {
            THANH_TOAN thanhToan = new THANH_TOAN();

            thanhToan.ID_PHUONG_THUC = form["method"].AsInt();
            thanhToan.SO_TIEN_DA_NHAN = 0;

            Debug.WriteLine(thanhToan.ID_PHUONG_THUC);

            data.THANH_TOANs.InsertOnSubmit(thanhToan);
            data.SubmitChanges();

            DAT_TOUR datTour = new DAT_TOUR();
            datTour.ID_DAT_TOUR = tour.ID_TOUR + "-0" + (CountBookedTour(tour.ID_TOUR) + 1); // Ex: TOUR1-24122024-01


            return View(datTour);
        }

        private int CountBookedTour(string idTour)
        {
            return data.DAT_TOURs
                     .Where(d => d.ID_DAT_TOUR.StartsWith(idTour))
                     .Count();
        }
          
        public ActionResult BookingWaitingPayment()
        {
            return View();
        }

        public ActionResult CashPayment()
        {
            return View();
        }

        public ActionResult VietQRPayment()
        {
            return View();
        }
    }
}