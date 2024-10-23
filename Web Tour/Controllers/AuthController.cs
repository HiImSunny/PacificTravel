using System;
using System.Linq;
using System.Web.Mvc;
using Web_Tour.Models;

namespace Web_Tour.Controllers
{
    public class AuthController : Controller
    {

        databaseDataContext data = new databaseDataContext("");

        public Boolean IsAuthenticated() {
            return Session["account-info"] != null || Session["account-info"].ToString() != "";
    
        }

        [HttpGet]
        public ActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SignUp(FormCollection form, THANH_VIEN thanhVien)
        {
            var hoten = form["signup-name"];
            var email = form["signup-email"];
            var password = form["signup-pass"];
            var phone = form["signup-phone"];
            var address = form["signup-address"];

            thanhVien.TEN_THANH_VIEN = hoten;
            thanhVien.EMAIL_THANH_VIEN = email;
            thanhVien.MAT_KHAU = password;
            thanhVien.SDT_THANH_VIEN = phone;
            thanhVien.DIA_CHI_THANH_VIEN = address;
            thanhVien.NGAY_TAO = DateTime.Now;

            data.THANH_VIENs.InsertOnSubmit(thanhVien);
            data.SubmitChanges();

            return RedirectToAction("Login");
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(FormCollection form)
        {
            var email = form["login-email"];
            var password = form["login-pass"];

            THANH_VIEN thanhVien = data.THANH_VIENs.SingleOrDefault(n => n.EMAIL_THANH_VIEN == email && n.MAT_KHAU == password);

            if (thanhVien == null) {
                ViewBag.Error = "Sai tài khoản hoặc mật khẩu!";

                return View();
            }

            Session["account-info"] = thanhVien;

            if (Session["redirect-to"] != null)
            {
                return Redirect(Session["redirect-to"].ToString());
            }

            return RedirectToAction("Home", "User");
        }

        public ActionResult ProfileInfo()
        {
            if (!IsAuthenticated())
                return RedirectToAction("Login", "User");

            ViewBag.AccountName = ((THANH_VIEN)Session["account-info"]).TEN_THANH_VIEN;
            ViewBag.AccountEmail = ((THANH_VIEN)Session["account-info"]).EMAIL_THANH_VIEN;
            ViewBag.AccountPhone = ((THANH_VIEN)Session["account-info"]).SDT_THANH_VIEN == "" ? "\u00A0" : ((THANH_VIEN)Session["account-info"]).SDT_THANH_VIEN;
            ViewBag.AccountAddress = ((THANH_VIEN)Session["account-info"]).DIA_CHI_THANH_VIEN == "" ? "\u00A0" : ((THANH_VIEN)Session["account-info"]).DIA_CHI_THANH_VIEN;

            return View();
        }

        public int ProfileID()
        {
            return ((THANH_VIEN)Session["account-info"]).ID_THANH_VIEN;
        }

        public ActionResult ProfileName() {
            ViewBag.AccountName = ((THANH_VIEN) Session["account-info"]).TEN_THANH_VIEN;

            return PartialView();
        }

        public ActionResult LogOut()
        {
            Session["account-info"] = null;

            return RedirectToAction("Login");
        }
    }
}