using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
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

            int identityValue = data.ExecuteQuery<int>("SELECT COUNT('ID_TOUR') FROM TOUR").FirstOrDefault() + 1;
            
            tour.TEN_TOUR = form["ten-tour"];
            tour.ID_TRANG_THAI = form["ID_TRANG_THAI"].AsInt();
            tour.MO_TA = form["mo-ta"];
            tour.NGAY_DI = String.Format("{0:dd/MM/yyyy}", form["ngay-di"]).AsDateTime();
            tour.NGAY_VE = String.Format("{0:dd/MM/yyyy}", form["ngay-ve"]).AsDateTime();

            var tempIDTour = form["id-tour"].AsInt();

            if (tempIDTour == 0)
            {
                tour.ID_TOUR = "TOUR" + identityValue + "-" + String.Format("{0:dd/MM/yyyy}", form["ngay-di"]).AsDateTime().ToString("ddMMyyyy");
            } else
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

        public ActionResult Tours()
        {
            if (!IsAdmin())
                return RedirectToAction("Login");

            var tour = from i in data.TOURs select i;

            return View(tour);
        }

        // -/- TOURS

        // --- TOUR Types

        [HttpGet]
        public ActionResult NewTourType()
        {
            if (!IsAdmin())
                return RedirectToAction("Login");

            return View();
        }

        [HttpPost]
        public ActionResult NewTourType(FormCollection form, LOAI_TOUR loaiTour)
        {
            if (!IsAdmin())
                return RedirectToAction("Login");

            var typeName = form["tour-type-name"];

            loaiTour.TEN_LOAI = typeName;

            data.LOAI_TOURs.InsertOnSubmit(loaiTour);
            data.SubmitChanges();

            return RedirectToAction("TourTypes");
        }

        [HttpGet]
        public ActionResult EditTourType(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login");

            LOAI_TOUR loaiTour = data.LOAI_TOURs.SingleOrDefault(i => i.ID_LOAI == id);

            if (loaiTour == null)
            {
                Response.StatusCode = 404;

                return null;
            }

            return View(loaiTour);
        }

        [HttpPost]
        public ActionResult EditTourType(LOAI_TOUR loaiTour)
        {
            if (!IsAdmin())
                return RedirectToAction("Login");

            LOAI_TOUR existingloaiTour = data.LOAI_TOURs.SingleOrDefault(i => i.ID_LOAI == loaiTour.ID_LOAI);

            if (existingloaiTour == null)
            {
                Response.StatusCode = 404;

                return null;
            }

            existingloaiTour.TEN_LOAI = loaiTour.TEN_LOAI;

            data.SubmitChanges();

            return RedirectToAction("TourTypes");
        }

        [HttpGet]
        public ActionResult DeleteTourType(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login");

            LOAI_TOUR loaiTour = data.LOAI_TOURs.SingleOrDefault(i => i.ID_LOAI == id);

            if (loaiTour == null)
            {
                Response.StatusCode = 404;

                return null;
            }

            return View(loaiTour);
        }

        [HttpPost, ActionName("DeleteTourType")]
        public ActionResult ConfirmDeleteTourType(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login");

            LOAI_TOUR existingloaiTour = data.LOAI_TOURs.SingleOrDefault(i => i.ID_LOAI == id);

            if (existingloaiTour == null)
            {
                Response.StatusCode = 404;

                return null;
            }

            data.LOAI_TOURs.DeleteOnSubmit(existingloaiTour);
            data.SubmitChanges();

            int identityValue = data.ExecuteQuery<int>("SELECT COUNT('ID_LOAI') FROM LOAI_TOUR").FirstOrDefault();

            if (id == identityValue)
                data.ExecuteCommand("DBCC CHECKIDENT ('[dbo].[LOAI_TOUR]', RESEED, {0})", id - 1);

            return RedirectToAction("TourTypes");
        }

        public ActionResult TourTypes()
        {
            if (!IsAdmin())
                return RedirectToAction("Login");

            var loaiTour = from i in data.LOAI_TOURs select i;

            return View(loaiTour);
        }

        // -/- TOUR Types

        // --- TOUR Status

        [HttpGet]
        public ActionResult NewTourStatus()
        {
            if (!IsAdmin())
                return RedirectToAction("Login");

            return View();
        }

        [HttpPost]
        public ActionResult NewTourStatus(FormCollection form, TRANG_THAI_TOUR trangThai)
        {
            if (!IsAdmin())
                return RedirectToAction("Login");

            var statusName = form["tour-status-name"];

            trangThai.TEN_TRANG_THAI = statusName;

            data.TRANG_THAI_TOURs.InsertOnSubmit(trangThai);
            data.SubmitChanges();

            return RedirectToAction("TourStatuses");
        }

        [HttpGet]
        public ActionResult EditTourStatus(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login");

            TRANG_THAI_TOUR trangThai = data.TRANG_THAI_TOURs.SingleOrDefault(i => i.ID_TRANG_THAI == id);

            if (trangThai == null)
            {
                Response.StatusCode = 404;

                return null;
            }

            return View(trangThai);
        }

        [HttpPost]
        public ActionResult EditTourStatus(TRANG_THAI_TOUR loaiTour)
        {
            if (!IsAdmin())
                return RedirectToAction("Login");

            TRANG_THAI_TOUR existingTrangThai = data.TRANG_THAI_TOURs.SingleOrDefault(i => i.ID_TRANG_THAI == loaiTour.ID_TRANG_THAI);

            if (existingTrangThai == null)
            {
                Response.StatusCode = 404;

                return null;
            }

            existingTrangThai.TEN_TRANG_THAI = loaiTour.TEN_TRANG_THAI;

            data.SubmitChanges();

            return RedirectToAction("TourStatuses");
        }

        [HttpGet]
        public ActionResult DeleteTourStatus(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login");

            TRANG_THAI_TOUR trangThai = data.TRANG_THAI_TOURs.SingleOrDefault(i => i.ID_TRANG_THAI == id);

            if (trangThai == null)
            {
                Response.StatusCode = 404;

                return null;
            }

            return View(trangThai);
        }

        [HttpPost, ActionName("DeleteTourStatus")]
        public ActionResult ConfirmDeleteTourStatus(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login");

            TRANG_THAI_TOUR existingTrangThai = data.TRANG_THAI_TOURs.SingleOrDefault(i => i.ID_TRANG_THAI == id);

            if (existingTrangThai == null)
            {
                Response.StatusCode = 404;

                return null;
            }

            data.TRANG_THAI_TOURs.DeleteOnSubmit(existingTrangThai);
            data.SubmitChanges();

            int identityValue = data.ExecuteQuery<int>("SELECT COUNT('ID_TRANG_THAI') FROM TRANG_THAI").FirstOrDefault();

            if (id == identityValue)
                data.ExecuteCommand("DBCC CHECKIDENT ('[dbo].[TRANG_THAI]', RESEED, {0})", id - 1);

            return RedirectToAction("TourStatuses");
        }

        public ActionResult TourStatuses()
        {
            if (!IsAdmin())
                return RedirectToAction("Login");

            var loaiTrangThai = from i in data.TRANG_THAI_TOURs select i;

            return View(loaiTrangThai);
        }

        // -/- TOUR Status

        // --- TOUR Guides

        public ActionResult NewTourGuide()
        {
            if (!IsAdmin())
                return RedirectToAction("Login");

            return View();
        }

        public ActionResult TourGuides()
        {
            if (!IsAdmin())
                return RedirectToAction("Login");

            return View();
        }

        // -/- TOUR Guides

        // --- NEWS

        public ActionResult NewArticle()
        {
            if (!IsAdmin())
                return RedirectToAction("Login");

            return View();
        }

        public ActionResult News()
        {
            if (!IsAdmin())
                return RedirectToAction("Login");

            return View();
        }

        // -/- NEWS

        // --- TOUR Location

        public ActionResult NewLocation()
        {
            if (!IsAdmin())
                return RedirectToAction("Login");

            return View();
        }

        public ActionResult Location()
        {
            if (!IsAdmin())
                return RedirectToAction("Login");

            return View();
        }

        // -/- TOUR Location

        // --- TOUR User

        public ActionResult NewUser()
        {
            if (!IsAdmin())
                return RedirectToAction("Login");

            return View();
        }

        public ActionResult Users()
        {
            if (!IsAdmin())
                return RedirectToAction("Login");

            return View();
        }

        // -/- TOUR User

        // --- TOUR Bought List

        public ActionResult TourBoughtList()
        {
            if (!IsAdmin())
                return RedirectToAction("Login");

            return View();
        }

        // -/- TOUR Bought List

        // --- TOUR Rate List

        public ActionResult RateList()
        {
            if (!IsAdmin())
                return RedirectToAction("Login");

            return View();
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
            } else
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