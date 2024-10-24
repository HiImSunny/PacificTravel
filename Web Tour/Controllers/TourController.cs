using System.Web.Mvc;
using Web_Tour.Models;
using System.Linq;
using System.Web.WebPages;
using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace Web_Tour.Controllers
{
    public class TourController : Controller
    {

        databaseDataContext data = new databaseDataContext("");

        public class TourDetailsViewModel
        {
            public TOUR Tour { get; set; }
            public List<DANH_GIA> Ratings { get; set; }
        }

        public ActionResult Details(string id)
        {
            var tour = data.TOURs.SingleOrDefault(t => t.ID_TOUR == id);

            var ratings = data.DANH_GIAs
                              .Where(d => d.ID_TOUR == id)
                              .ToList();

            // Prepare a ViewModel to pass to the view if needed
            var viewModel = new TourDetailsViewModel
            {
                Tour = tour,
                Ratings = ratings
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult RateTour(FormCollection form, DANH_GIA danhGia)
        {
            // Assuming you have a data context called 'data'
            if (ModelState.IsValid)
            {
                danhGia.SO_LUONG_SAO = form["SoLuongSao"].AsInt();
                danhGia.NOI_DUNG_DANH_GIA = form["NoiDungDanhGia"];

                // Save the rating to the database
                data.DANH_GIAs.InsertOnSubmit(danhGia);
                data.SubmitChanges();
                return RedirectToAction("Details", new { id = danhGia.ID_TOUR }); // Redirect back to the tour details page
            }

            // If validation fails, return the view with errors
            return View("Details", danhGia); // Adjust this to return the correct view
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
            datTour.SO_NGUOI_LON = form["adult"].AsInt();
            datTour.SO_TRE_EM = form["child"].AsInt();
            datTour.ID_TOUR = form["ID_TOUR"];
            datTour.ID_THANH_TOAN = thanhToan.ID_THANH_TOAN;
            datTour.ID_THANH_VIEN = form["ID_THANH_VIEN"].AsInt();
            datTour.ID_LOAI = form["tour-type"].AsInt();
            datTour.HUY_DAT_TOUR = false;

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

        private DAT_TOUR GetDatTour(string id)
        {
            return data.DAT_TOURs.SingleOrDefault(i => i.ID_DAT_TOUR == id);
        }
          
        public ActionResult BookingComplete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction("Home", "User");
            }

            var datTour = GetDatTour(id);

            if (Session["account-info"] == null)
            {
                return RedirectToAction("Login", "Auth");
            } else if (((THANH_VIEN)Session["account-info"]).ID_THANH_VIEN != datTour.ID_THANH_VIEN)
                return RedirectToAction("Home", "User");

            ViewBag.tour = data.TOURs.SingleOrDefault(i => i.ID_TOUR == datTour.ID_TOUR);

            return View(datTour);
        }

        public ActionResult CashPayment(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction("Home", "User");
            }

            if (Session["account-info"] == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            else if (((THANH_VIEN)Session["account-info"]).ID_THANH_VIEN != GetDatTour(id).ID_THANH_VIEN)
                return RedirectToAction("Home", "User");

            ViewBag.id = id;

            return View();
        }

        public ActionResult VietQRPayment(string id, string amount)
        {
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction("Home", "User");
            }

            if (Session["account-info"] == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            else if (((THANH_VIEN)Session["account-info"]).ID_THANH_VIEN != GetDatTour(id).ID_THANH_VIEN)
                return RedirectToAction("Home", "User");

            ViewBag.id = id;
            ViewBag.amount = amount;

            return View();
        }

        public ActionResult BookedDetailsPage(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction("Home", "User");
            }

            var datTour = GetDatTour(id);

            if (Session["account-info"] == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            else if (((THANH_VIEN)Session["account-info"]).ID_THANH_VIEN != datTour.ID_THANH_VIEN)
                return RedirectToAction("Home", "User");

            return View(datTour);
        }

        public ActionResult BookedDetails(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction("Home", "User");
            }

            var datTour = GetDatTour(id);

            if (Session["account-info"] == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            else if (((THANH_VIEN)Session["account-info"]).ID_THANH_VIEN != datTour.ID_THANH_VIEN)
                return RedirectToAction("Home", "User");

            return PartialView(datTour);
        }

        [HttpGet]
        public ActionResult CancelBookedTour(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction("Home", "User");
            }

            var datTour = GetDatTour(id);

            if (Session["account-info"] == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            else if (((THANH_VIEN)Session["account-info"]).ID_THANH_VIEN != datTour.ID_THANH_VIEN)
                return RedirectToAction("Home", "User");


            return View(datTour);

        }

        public ActionResult ConfirmCancelBookedTour(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction("Home", "User");
            }

            var datTour = GetDatTour(id);

            if (Session["account-info"] == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            else if (((THANH_VIEN)Session["account-info"]).ID_THANH_VIEN != datTour.ID_THANH_VIEN)
                return RedirectToAction("Home", "User");

            datTour.HUY_DAT_TOUR = true;

            if (datTour.THANH_TOAN.ID_TRANG_THAI == 2)
                datTour.THANH_TOAN.ID_TRANG_THAI = 3;

            data.SubmitChanges();

            return RedirectToAction("ProfileInfo", "Auth");
        }

    }
}