using PagedList;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.WebPages;
using Web_Tour.Models;

namespace Web_Tour.Controllers
{
    public class AdminController : Controller
    {
        databaseDataContext data = new databaseDataContext("");

        public bool IsAdmin()
        {
            if (Session["account-info"] == null || Session["account-info"].ToString() == "")
                return false;
            else
            {
                THANH_VIEN thanhVien = ((THANH_VIEN)Session["account-info"]);

                ADMIN admin = data.ADMINs.SingleOrDefault(n => n.ID_THANH_VIEN == thanhVien.ID_THANH_VIEN);

                if (admin == null)
                    return false;
            }

            return true;
        }

        public bool IsThanhVienAdmin(int idThanhVien)
        {
            var admin = data.ADMINs.SingleOrDefault(n => n.ID_THANH_VIEN == idThanhVien);

            return admin != null;
        }

        public ActionResult ToggleAdmin(int idThanhVien)
        {
            var admin = data.ADMINs.SingleOrDefault(n => n.ID_THANH_VIEN == idThanhVien);

            if (admin != null)
                data.ADMINs.DeleteOnSubmit(admin);
            else
            {
                ADMIN newAdmin = new ADMIN();

                newAdmin.ID_THANH_VIEN = idThanhVien;

                data.ADMINs.InsertOnSubmit(newAdmin);
            }

            data.SubmitChanges();

            return RedirectToAction("Users");
        }

        public ActionResult Index()
        {
            if (!IsAdmin())
                return RedirectToAction("Login");

            return View();
        }

        // --- TOURS

        [HttpGet]
        public ActionResult NewTour()
        {
            if (!IsAdmin())
                return RedirectToAction("Login");

            List<string> id = new List<string>
            {
                "Thêm mã mới"
            };

            foreach (TOUR element in data.TOURs.ToList())
            {
                // Tách chuỗi ID_TOUR tại dấu '-'
                string[] parts = element.ID_TOUR.Split('-');

                // Lấy phần đầu tiên và thêm vào danh sách id
                if (parts.Length > 0)
                {
                    id.Add(parts[0]);
                }
            }

            ViewBag.ID_TOUR = id.Select(i => new SelectListItem
            {
                Value = i,        // Giá trị sẽ được gửi lên server
                Text = i         // Văn bản hiển thị cho người dùng
            }).ToList();
            ViewBag.ID_TRANG_THAI = new SelectList(data.TRANG_THAI_TOURs.ToList().OrderBy(i => i.TEN_TRANG_THAI), "ID_TRANG_THAI", "TEN_TRANG_THAI");
            ViewBag.ID_HDV = new SelectList(data.HUONGDANVIENs.ToList().OrderBy(i => i.HO_TEN_HDV), "ID_HDV", "HO_TEN_HDV");

            return View();
        }

        [HttpGet]
        public JsonResult GetTourDetails(string id)
        {
            var tour = data.TOURs.FirstOrDefault(t => t.ID_TOUR.StartsWith(id));

            if (tour == null)
                return null;

            return Json(new
            {
                tenTour = tour.TEN_TOUR,
                moTa = tour.MO_TA,
                diaDiemKhoiHanh = tour.DIA_DIEM_KHOI_HANH,
                diaDiemThamQuan = tour.DIA_DIEM_THAM_QUAN,
                anhBiaTour = tour.ANH_BIA_TOUR // Đường dẫn đến ảnh bìa
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult NewTour(FormCollection form, TOUR tour, HttpPostedFileBase anhbiatour)
        {
            if (!IsAdmin())
                return RedirectToAction("Login");

            int identityValue = data.TOURs.OrderByDescending(i => i.STT).First().STT;

            tour.TEN_TOUR = form["ten-tour"];
            tour.ID_TRANG_THAI = form["ID_TRANG_THAI"].AsInt();
            tour.MO_TA = form["mo-ta"];
            tour.NGAY_DI = String.Format("{0:dd/MM/yyyy}", form["ngay-di"]).AsDateTime();
            tour.NGAY_VE = String.Format("{0:dd/MM/yyyy}", form["ngay-ve"]).AsDateTime();

            var tempIDTour = form["ID_TOUR"].AsInt();

            if (tempIDTour == 0)
            {
                tour.ID_TOUR = "TOUR" + identityValue + "-" + String.Format("{0:dd/MM/yyyy}", form["ngay-di"]).AsDateTime().ToString("ddMMyyyy");
            }
            else
            {
                tour.ID_TOUR = data.TOURs.FirstOrDefault(t => t.ID_TOUR.StartsWith(form["id-tour"])) + "-" + String.Format("{0:dd/MM/yyyy}", form["ngay-di"]).AsDateTime().ToString("ddMMyyyy");
            }

            tour.DIA_DIEM_KHOI_HANH = form["dia-diem-khoi-hanh"];
            tour.DIA_DIEM_THAM_QUAN = form["dia-diem-tham-quan"];
            tour.HINH_THUC_DI_CHUYEN = form["hinh-thuc-di-chuyen"];
            tour.ID_HDV = form["ID_HDV"].AsInt();
            tour.GIAM_GIA = form["giam-gia"].AsInt();
            tour.GIA_NGUOI_LON = form["gia-nguoi-lon"].AsInt();
            tour.GIA_TRE_EM = form["gia-tre-em"].AsInt();

            var filename = Path.GetFileName(anhbiatour.FileName);
            // Luu duong dan cua file
            var path = Path.Combine(Server.MapPath("~/Images/tour"), filename);
            // Kiem tra hinh anh ton tai chua?
            if (!System.IO.File.Exists(path))
                anhbiatour.SaveAs(path);


            tour.ANH_BIA_TOUR = filename;

            data.TOURs.InsertOnSubmit(tour);
            data.SubmitChanges();

            return RedirectToAction("Tours");
        }

        [HttpGet]
        public ActionResult EditTour(string id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login");

            TOUR tour = data.TOURs.SingleOrDefault(i => i.ID_TOUR == id);

            if (tour == null)
            {
                Response.StatusCode = 404;

                return null;
            }

            ViewBag.ID_TRANG_THAI = new SelectList(data.TRANG_THAI_TOURs.ToList().OrderBy(i => i.TEN_TRANG_THAI), "ID_TRANG_THAI", "TEN_TRANG_THAI", tour.TRANG_THAI_TOUR.ID_TRANG_THAI);
            ViewBag.ID_HDV = new SelectList(data.HUONGDANVIENs.ToList().OrderBy(i => i.HO_TEN_HDV), "ID_HDV", "HO_TEN_HDV", tour.ID_HDV);

            return View(tour);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditTour(FormCollection form, HttpPostedFileBase anhbiatour)
        {
            if (!IsAdmin())
                return RedirectToAction("Login");

            ViewBag.ID_TRANG_THAI = new SelectList(data.TRANG_THAI_TOURs.ToList().OrderBy(i => i.TEN_TRANG_THAI), "ID_TRANG_THAI", "TEN_TRANG_THAI");
            ViewBag.ID_HDV = new SelectList(data.HUONGDANVIENs.ToList().OrderBy(i => i.HO_TEN_HDV), "ID_HDV", "HO_TEN_HDV");

            TOUR tour = data.TOURs.SingleOrDefault(i => i.ID_TOUR == form["ID_TOUR"]);

            tour.TEN_TOUR = form["ten-tour"];
            tour.ID_TRANG_THAI = form["ID_TRANG_THAI"].AsInt();
            tour.MO_TA = form["mo-ta"];
            tour.NGAY_DI = String.Format("{0:dd/MM/yyyy}", form["ngay-di"]).AsDateTime();
            tour.NGAY_VE = String.Format("{0:dd/MM/yyyy}", form["ngay-ve"]).AsDateTime();
            tour.DIA_DIEM_KHOI_HANH = form["dia-diem-khoi-hanh"];
            tour.DIA_DIEM_THAM_QUAN = form["dia-diem-tham-quan"];
            tour.HINH_THUC_DI_CHUYEN = form["hinh-thuc-di-chuyen"];
            tour.ID_HDV = form["ID_HDV"].AsInt();
            tour.GIAM_GIA = form["giam-gia"].AsInt();
            tour.GIA_NGUOI_LON = form["gia-nguoi-lon"].AsInt();
            tour.GIA_TRE_EM = form["gia-tre-em"].AsInt();

            if (ModelState.IsValid)
            {
                if (anhbiatour != null)
                {
                    var filename = Path.GetFileName(anhbiatour.FileName);
                    // Luu duong dan cua file
                    var path = Path.Combine(Server.MapPath("~/Images/tour"), filename);
                    // Kiem tra hinh anh ton tai chua?
                    if (!System.IO.File.Exists(path))
                        anhbiatour.SaveAs(path);

                    tour.ANH_BIA_TOUR = filename;
                }
                else
                {
                    tour.ANH_BIA_TOUR = form["anhbiahientai"];
                }

                data.SubmitChanges();
            }

            return RedirectToAction("Tours");
        }

        public ActionResult TourDetails(string id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login");

            TOUR tour = data.TOURs.SingleOrDefault(i => i.ID_TOUR == id);

            if (tour == null)
            {
                Response.StatusCode = 404;

                return null;
            }

            return View(tour);
        }

        public ActionResult DeleteTour(string id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login");

            TOUR tour = data.TOURs.SingleOrDefault(i => i.ID_TOUR == id);

            if (tour == null)
            {
                Response.StatusCode = 404;

                return null;
            }

            return View(tour);
        }

        public ActionResult ConfirmDeleteTour(string id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login");

            TOUR tour = data.TOURs.SingleOrDefault(i => i.ID_TOUR == id);

            if (tour == null)
            {
                Response.StatusCode = 404;

                return null;
            }

            data.TOURs.DeleteOnSubmit(tour);
            data.SubmitChanges();

            return RedirectToAction("Tours");
        }

        public ActionResult Tours(int ? page)
        {
            if (!IsAdmin())
                return RedirectToAction("Login");

            int pageNumber = (page ?? 1);
            int pageSize = 7;

            return View(data.TOURs.ToList().OrderBy(n => n.NGAY_DI).ToPagedList(pageNumber, pageSize));
        }

        // -/- TOURS

        // --- TOUR Guides

        public ActionResult NewTourGuide()
        {
            if (!IsAdmin())
                return RedirectToAction("Login");

            return View();
        }

        [HttpPost]
        public ActionResult NewTourGuide(FormCollection form)
        {
            if (!IsAdmin())
                return RedirectToAction("Login");

            HUONGDANVIEN hdv = new HUONGDANVIEN();

            hdv.HO_TEN_HDV = form["ten-hdv"];
            hdv.SDT_HDV = form["sdt-hdv"];
            hdv.EMAIL_HDV = form["email-hdv"];
            hdv.DIA_CHI_HDV = form["address-hdv"];

            data.HUONGDANVIENs.InsertOnSubmit(hdv);
            data.SubmitChanges();

            return RedirectToAction("TourGuides");
        }

        public ActionResult TourGuides(int ? page)
        {
            if (!IsAdmin())
                return RedirectToAction("Login");

            int pageNumber = (page ?? 1);
            int pageSize = 7;

            return View(data.HUONGDANVIENs.ToList().OrderBy(n => n.ID_HDV).ToPagedList(pageNumber, pageSize));
        }

        public ActionResult TourGuideEdit(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login");

            HUONGDANVIEN hdv = data.HUONGDANVIENs.SingleOrDefault(i => i.ID_HDV == id);

            if (hdv == null)
            {
                Response.StatusCode = 404;

                return null;
            }

            return View(hdv);
        }

        [HttpPost]
        public ActionResult TourGuideEdit(FormCollection form, int id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login");

            HUONGDANVIEN hdv = data.HUONGDANVIENs.SingleOrDefault(i =>i.ID_HDV == id);

            hdv.HO_TEN_HDV = form["ten-hdv"];
            hdv.SDT_HDV = form["sdt-hdv"];
            hdv.EMAIL_HDV = form["email-hdv"];
            hdv.DIA_CHI_HDV = form["address-hdv"];

            data.SubmitChanges();

            return RedirectToAction("TourGuides");
        }

        public ActionResult DeleteTourGuide(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login");

            HUONGDANVIEN hdv = data.HUONGDANVIENs.SingleOrDefault(i => i.ID_HDV == id);

            if (hdv == null)
            {
                Response.StatusCode = 404;

                return null;
            }

            return View(hdv);
        }

        public ActionResult ConfirmDeleteTourGuide(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login");

            HUONGDANVIEN hdv = data.HUONGDANVIENs.SingleOrDefault(i => i.ID_HDV == id);

            if (hdv == null)
            {
                Response.StatusCode = 404;

                return null;
            }

            data.HUONGDANVIENs.DeleteOnSubmit(hdv);
            data.SubmitChanges();

            return RedirectToAction("TourGuides");
        }

        // -/- TOUR Guides

        // --- NEWS

        public ActionResult NewArticle()
        {
            if (!IsAdmin())
                return RedirectToAction("Login");

            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult NewArticle(FormCollection form, int idThanhVien, HttpPostedFileBase anhbiabaiviet)
        {
            if (!IsAdmin())
                return RedirectToAction("Login");

            ADMIN admin = data.ADMINs.SingleOrDefault(i => i.ID_THANH_VIEN == idThanhVien);

            BAI_VIET baiViet = new BAI_VIET();

            baiViet.THOI_GIAN_TAO = DateTime.Now;
            baiViet.ID_ADMIN = admin.ID_ADMIN;
            baiViet.TIEU_DE = form["tieu-de"];
            baiViet.NOI_DUNG = form["noidung"];

            var filename = Path.GetFileName(anhbiabaiviet.FileName);
            // Luu duong dan cua file
            var path = Path.Combine(Server.MapPath("~/Images/articles"), filename);
            // Kiem tra hinh anh ton tai chua?
            if (!System.IO.File.Exists(path))
                anhbiabaiviet.SaveAs(path);

            baiViet.ANH_BIA_BAI_VIET = filename;

            data.BAI_VIETs.InsertOnSubmit(baiViet);
            data.SubmitChanges();

            return RedirectToAction("News");
        }

        public ActionResult ArticleDetails(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login");

            BAI_VIET baiViet = data.BAI_VIETs.SingleOrDefault(i => i.ID_BAI_VIET == id);

            return View(baiViet);
        }

        public ActionResult News(int ? page)
        {
            if (!IsAdmin())
                return RedirectToAction("Login");

            int pageNumber = (page ?? 1);
            int pageSize = 7;

            return View(data.BAI_VIETs.ToList().OrderBy(n => n.THOI_GIAN_TAO).ToPagedList(pageNumber, pageSize));
        }

        // -/- NEWS

        // --- TOUR User

        public ActionResult Users(int ? page)
        {
            if (!IsAdmin())
                return RedirectToAction("Login");

            int pageNumber = (page ?? 1);
            int pageSize = 7;

            return View(data.THANH_VIENs.ToList().OrderBy(n => n.TEN_THANH_VIEN).ToPagedList(pageNumber, pageSize));
        }

        // -/- TOUR User

        // --- TOUR Bought List

        public ActionResult TourBoughtList(int ? page)
        {
            if (!IsAdmin())
                return RedirectToAction("Login");

            int pageNumber = (page ?? 1);
            int pageSize = 7;

            return View(data.DAT_TOURs.ToList().OrderBy(n => n.THOI_GIAN_DAT).ToPagedList(pageNumber, pageSize));
        }

        public ActionResult TourBoughtDetails(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction("TourBoughtDetails", "Admin");
            }

            if (!IsAdmin())
                return RedirectToAction("Login");

            var datTour = data.DAT_TOURs.SingleOrDefault(i => i.ID_DAT_TOUR == id);

            return View(datTour);
        }

        [HttpGet]
        public ActionResult CancelBookedTour(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction("TourBoughtList", "Admin");
            }

            if (!IsAdmin())
                return RedirectToAction("Login");

            var datTour = data.DAT_TOURs.SingleOrDefault(i => i.ID_DAT_TOUR == id);

            return View(datTour);

        }

        public ActionResult ConfirmCancelBookedTour(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction("TourBoughtList", "Admin");
            }

            if (!IsAdmin())
                return RedirectToAction("Login");

            var datTour = data.DAT_TOURs.SingleOrDefault(i => i.ID_DAT_TOUR == id);

            datTour.HUY_DAT_TOUR = true;

            if (datTour.THANH_TOAN.ID_TRANG_THAI == 2)
                datTour.THANH_TOAN.ID_TRANG_THAI = 3;

            data.SubmitChanges();

            return RedirectToAction("TourBoughtList", "Admin");
        }

        public ActionResult Payment(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction("TourBoughtList", "Admin");
            }

            if (!IsAdmin())
                return RedirectToAction("Login");

            var datTour = data.DAT_TOURs.SingleOrDefault(i => i.ID_DAT_TOUR == id);

            return View(datTour);
        }

        [HttpPost]
        public ActionResult Payment(string id, int paymentAmount)
        {
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction("TourBoughtList", "Admin");
            }

            if (!IsAdmin())
                return RedirectToAction("Login");

            var datTour = data.DAT_TOURs.SingleOrDefault(i => i.ID_DAT_TOUR == id);

            if (paymentAmount >= datTour.TONG_SO_TIEN)
            {
                datTour.THANH_TOAN.SO_TIEN_DA_NHAN = paymentAmount;
                datTour.THANH_TOAN.ID_TRANG_THAI = 2;
            } 
            else
            {
                datTour.THANH_TOAN.SO_TIEN_DA_NHAN += paymentAmount;
            }

            datTour.THANH_TOAN.NGAY_GIO_THANH_TOAN = DateTime.Now;

            data.SubmitChanges();

            return RedirectToAction("TourBoughtList", "Admin");
        }

        // -/- TOUR Bought List

        // --- TOUR Rate List

        public ActionResult RateList(int ? page)
        {
            if (!IsAdmin())
                return RedirectToAction("Login");

            int pageNumber = (page ?? 1);
            int pageSize = 7;

            return View(data.DANH_GIAs.ToList().OrderByDescending(n => n.SO_LUONG_SAO).ToPagedList(pageNumber, pageSize));
        }

        public ActionResult RateDetails(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login");

            DANH_GIA danhGia = data.DANH_GIAs.SingleOrDefault(i => i.ID_DANH_GIA == id);

            if (danhGia == null)
            {
                Response.StatusCode = 404;

                return null;
            }

            return View(danhGia);
        }

        public ActionResult DeleteRate(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login");

            DANH_GIA danhGia = data.DANH_GIAs.SingleOrDefault(i => i.ID_DANH_GIA == id);

            if (danhGia == null)
            {
                Response.StatusCode = 404;

                return null;
            }

            return View(danhGia);
        }

        public ActionResult ConfirmDeleteRate(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login");

            DANH_GIA danhGia = data.DANH_GIAs.SingleOrDefault(i => i.ID_DANH_GIA == id);

            if (danhGia == null)
            {
                Response.StatusCode = 404;

                return null;
            }

            data.DANH_GIAs.DeleteOnSubmit(danhGia);
            data.SubmitChanges();

            return RedirectToAction("RateList");
        }

        // --- TOUR Rate List

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

            if (thanhVien == null)
            {
                ViewBag.Error = "Sai tài khoản hoặc mật khẩu!";

                return View();
            }
            else
            {
                ADMIN admin = data.ADMINs.SingleOrDefault(n => n.ID_THANH_VIEN == thanhVien.ID_THANH_VIEN);

                if (admin == null)
                {
                    ViewBag.Error = "Bạn không phải là admin!";

                    return View();
                }
            }

            Session["account-info"] = thanhVien;
            return RedirectToAction("Index", "Admin");
        }
    }
}