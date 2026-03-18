using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using CodeQLTour.Models;
using System.Linq;

namespace CodeQLTour.Controllers
{
    public class AdminController : Controller
    {
        private readonly TourContext db;

        public AdminController(TourContext context)
        {
            db = context;
        }

        // ================= KIỂM TRA QUYỀN =================

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var role = HttpContext.Session.GetString("VaiTro");

            if (string.IsNullOrEmpty(role))
            {
                context.Result = RedirectToAction("Auth", "Account");
                return;
            }

            if (role == "KhachHang")
            {
                context.Result = RedirectToAction("Index", "Tour");
                return;
            }

            base.OnActionExecuting(context);
        }

        // ================= DASHBOARD =================

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("VaiTro") != "Admin")
            {
                return RedirectToAction("DatTour");
            }

            ViewBag.Tours = db.TourDuLiches.Count();
            ViewBag.Khach = db.Accounts.Count(x => x.VaiTro == "KhachHang");
            ViewBag.NhanVien = db.Accounts.Count(x => x.VaiTro == "NhanVien");
            ViewBag.DatTour = db.PhieuDatTour.Count();

            return View();
        }

        // ================= QUẢN LÝ TOUR =================

        public IActionResult Tours()
        {
            if (HttpContext.Session.GetString("VaiTro") != "Admin")
                return RedirectToAction("DatTour");

            return View(db.TourDuLiches.ToList());
        }

        public IActionResult CreateTour()
        {
            if (HttpContext.Session.GetString("VaiTro") != "Admin")
                return RedirectToAction("DatTour");

            return View(new TourDuLich());
        }

        [HttpPost]
        public IActionResult CreateTour(TourDuLich t)
        {
            if (HttpContext.Session.GetString("VaiTro") != "Admin")
                return RedirectToAction("DatTour");

            if (!ModelState.IsValid)
                return View(t);

            db.TourDuLiches.Add(t);
            db.SaveChanges();

            return RedirectToAction("Tours");
        }

        public IActionResult EditTour(int id)
        {
            if (HttpContext.Session.GetString("VaiTro") != "Admin")
                return RedirectToAction("DatTour");

            var tour = db.TourDuLiches.Find(id);

            if (tour == null)
                return NotFound();

            return View(tour);
        }

        [HttpPost]
        public IActionResult EditTour(TourDuLich t)
        {
            if (HttpContext.Session.GetString("VaiTro") != "Admin")
                return RedirectToAction("DatTour");

            if (!ModelState.IsValid)
                return View(t);

            db.TourDuLiches.Update(t);
            db.SaveChanges();

            return RedirectToAction("Tours");
        }

        [HttpPost]
        public IActionResult DeleteTour(int id)
        {
            if (HttpContext.Session.GetString("VaiTro") != "Admin")
                return RedirectToAction("DatTour");

            var tour = db.TourDuLiches.Find(id);

            if (tour != null)
            {
                db.TourDuLiches.Remove(tour);
                db.SaveChanges();
            }

            return RedirectToAction("Tours");
        }

        // ================= QUẢN LÝ TÀI KHOẢN =================

        public IActionResult Accounts()
        {
            if (HttpContext.Session.GetString("VaiTro") != "Admin")
                return RedirectToAction("DatTour");

            return View(db.Accounts.ToList());
        }

        public IActionResult CreateAccount()
        {
            if (HttpContext.Session.GetString("VaiTro") != "Admin")
                return RedirectToAction("DatTour");

            return View();
        }

        [HttpPost]
        public IActionResult CreateAccount(Account a)
        {
            if (HttpContext.Session.GetString("VaiTro") != "Admin")
                return RedirectToAction("DatTour");

            if (string.IsNullOrEmpty(a.Username) || string.IsNullOrEmpty(a.Password))
            {
                ModelState.AddModelError("", "Tên tài khoản và mật khẩu không được để trống");
                return View(a);
            }

            var check = db.Accounts.FirstOrDefault(x => x.Username == a.Username);

            if (check != null)
            {
                ModelState.AddModelError("Username", "Tên tài khoản đã tồn tại");
                return View(a);
            }

            a.VaiTro = "KhachHang";

            db.Accounts.Add(a);
            db.SaveChanges();

            // ⭐ THÊM PHẦN NÀY
            var kh = new KhachHang
            {
                AccountId = a.Id,
                TenKH = "",
                SoDienThoai = "",
                Email = ""
            };

            db.KhachHangs.Add(kh);
            db.SaveChanges();

            return RedirectToAction("KhachHang");
        }

        [HttpPost]
        public IActionResult DeleteAccount(int id)
        {
            if (HttpContext.Session.GetString("VaiTro") != "Admin")
                return RedirectToAction("DatTour");

            var acc = db.Accounts.Find(id);

            if (acc != null)
            {
                db.Accounts.Remove(acc);
                db.SaveChanges();
            }

            return RedirectToAction("Accounts");
        }

        // ================= QUẢN LÝ KHÁCH HÀNG =================

        public IActionResult KhachHang()
        {
            if (HttpContext.Session.GetString("VaiTro") != "Admin")
            {
                return RedirectToAction("DatTour");
            }
            var data = from a in db.Accounts
                       join k in db.KhachHangs
                       on a.Id equals k.AccountId into gj
                       from sub in gj.DefaultIfEmpty()
                       where a.VaiTro == "KhachHang"
                       select new KhachHangViewModel
                       {
                           MaKH = sub != null ? sub.MaKH : -1,
                           AccountId = a.Id,
                           Username = a.Username,
                           TenKH = sub != null ? sub.TenKH : "",
                           SoDienThoai = sub != null ? sub.SoDienThoai : "",
                           Email = sub != null ? sub.Email : ""
                       };

            return View(data.ToList());
        }

        // ================= QUẢN LÝ NHÂN VIÊN =================

        public IActionResult NhanVien()
        {
            var data = from a in db.Accounts
                       join nv in db.NhanViens
                       on a.Id equals nv.AccountId into gj
                       from sub in gj.DefaultIfEmpty()
                       where a.VaiTro == "NhanVien"
                       select new NhanVienViewModel
                       {
                           MaNV = sub != null ? sub.MaNV : 0,
                           AccountId = a.Id,
                           Username = a.Username,
                           TenNV = sub != null ? sub.TenNV : "",
                           ChucVu = sub != null ? sub.ChucVu : "",
                           SoDienThoai = sub != null ? sub.SoDienThoai : ""
                       };

            return View(data.ToList());
        }

        [HttpPost]
        public IActionResult DeleteNhanVien(int id)
        {
            var acc = db.Accounts.FirstOrDefault(x => x.Id == id);

            if (acc != null)
            {
                db.Accounts.Remove(acc);
                db.SaveChanges();
            }

            return RedirectToAction("NhanVien");
        }
        // ================= ĐƠN ĐẶT TOUR =================

        public IActionResult DatTour()
        {
            var data = from p in db.PhieuDatTour
                       join t in db.TourDuLiches on p.MaTour equals t.MaTour
                       join k in db.KhachHangs on p.MaKH equals k.MaKH
                       join a in db.Accounts on k.AccountId equals a.Id
                       select new
                       {
                           p.SoPhieuDatTour,
                           t.TenTour,
                           k.TenKH,
                           a.Username,
                           p.NgayKhoiHanh,
                           p.SoLuong,
                           p.TrangThai,
                           p.NgayDatTour,
                           p.NguoiXuLy
                       };

            return View(data.ToList());
        }

        // ================= XỬ LÝ ĐƠN =================

        [HttpPost]
        public IActionResult XacNhanDon(int id)
        {
            var don = db.PhieuDatTour.Find(id);

            if (don != null)
            {
                don.TrangThai = "Đã xác nhận";
                don.NguoiXuLy = HttpContext.Session.GetString("Username"); // lưu người xử lý
                db.SaveChanges();
            }

            return RedirectToAction("DatTour");
        }

        [HttpPost]
        public IActionResult TuChoiDon(int id)
        {
            var don = db.PhieuDatTour.Find(id);

            if (don != null)
            {
                don.TrangThai = "Đã hủy";
                don.NguoiXuLy = HttpContext.Session.GetString("Username"); // lưu người xử lý
                db.SaveChanges();
            }

            return RedirectToAction("DatTour");
        }
        public IActionResult KhachHangDetail(int id)
        {   
            if (HttpContext.Session.GetString("VaiTro") != "Admin")
            {
                return RedirectToAction("DatTour");
            }

            var acc = db.Accounts.FirstOrDefault(x => x.Id == id);

            var data = from a in db.Accounts
                       join k in db.KhachHangs on a.Id equals k.AccountId into gj
                       from sub in gj.DefaultIfEmpty()
                       where a.Id == id
                       select new KhachHangViewModel
                       {
                           MaKH = sub != null ? sub.MaKH : 0,
                           AccountId = a.Id,
                           Username = a.Username,
                           TenKH = sub != null ? sub.TenKH : "",
                           SoDienThoai = sub != null ? sub.SoDienThoai : "",
                           Email = sub != null ? sub.Email : "",
                           NgaySinh = sub != null ? sub.NgaySinh : null,
                           DiaChi = sub != null ? sub.DiaChi : ""
                       };

            var kh = data.FirstOrDefault();

            ViewBag.Account = acc;   // ⭐ THÊM DÒNG NÀY

            return View(kh);
        }
        [HttpPost]
        public IActionResult UpdateKhach(KhachHangViewModel model)
        {   
            if (HttpContext.Session.GetString("VaiTro") != "Admin")
                return RedirectToAction("DatTour");

            var kh = db.KhachHangs.FirstOrDefault(x => x.MaKH == model.MaKH);

            if (kh != null)
            {
                kh.TenKH = model.TenKH;
                kh.SoDienThoai = model.SoDienThoai;
                kh.Email = model.Email;
                kh.NgaySinh = model.NgaySinh;
                kh.DiaChi = model.DiaChi;

                db.SaveChanges();
            }

            return RedirectToAction("KhachHang");
        }



        [HttpPost]
        public IActionResult DeleteKhach(int id)
        {
            if (HttpContext.Session.GetString("VaiTro") != "Admin")
            {
                return RedirectToAction("DatTour");
            }
            var acc = db.Accounts.FirstOrDefault(x => x.Id == id);

            if (acc != null)
            {
                db.Accounts.Remove(acc);
                db.SaveChanges();
            }

            return RedirectToAction("KhachHang");
        }
        public IActionResult CreateNhanVienAccount()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateNhanVienAccount(Account acc)
        {
            if (HttpContext.Session.GetString("VaiTro") != "Admin")
            {
                return Redirect(Request.Headers["Referer"].ToString());
            }
            var check = db.Accounts.FirstOrDefault(x => x.Username == acc.Username);

            if (check != null)
            {
                ModelState.AddModelError("Username", "Tên tài khoản đã tồn tại");
                return View(acc);
            }

            acc.VaiTro = "NhanVien";

            db.Accounts.Add(acc);
            db.SaveChanges();

            var nv = new NhanVien
            {
                AccountId = acc.Id,
                TenNV = "",
                ChucVu = "",
                SoDienThoai = ""
            };

            db.NhanViens.Add(nv);
            db.SaveChanges();

            return RedirectToAction("NhanVien");
        }
        public IActionResult EditNhanVien(int id)
        {
            if (HttpContext.Session.GetString("VaiTro") != "Admin")
            {
                return Redirect(Request.Headers["Referer"].ToString());
            }
            var acc = db.Accounts.FirstOrDefault(x => x.Id == id);

            if (acc == null)
                return NotFound();

            var nv = db.NhanViens.FirstOrDefault(x => x.AccountId == id);

            if (nv == null)
            {
                nv = new NhanVien
                {
                    AccountId = id
                };
            }

            ViewBag.Account = acc;

            return View(nv);
        }
        [HttpPost]
        public IActionResult UpdateNhanVien(int MaNV, string TenNV, string ChucVu, string SoDienThoai, int id, string username, string password)
        {
            if (HttpContext.Session.GetString("VaiTro") != "Admin")
            {
                return Redirect(Request.Headers["Referer"].ToString());
            }
            var nv = db.NhanViens.FirstOrDefault(x => x.MaNV == MaNV);

            if (nv != null)
            {
                nv.TenNV = TenNV;
                nv.ChucVu = ChucVu;
                nv.SoDienThoai = SoDienThoai;
            }

            var acc = db.Accounts.FirstOrDefault(x => x.Id == id);

            if (acc != null)
            {
                acc.Username = username;
                acc.Password = password;
            }

            db.SaveChanges();

            return RedirectToAction("NhanVien");
        }
        public IActionResult DoanhThu()
        {
            if (HttpContext.Session.GetString("VaiTro") != "Admin")
            {
                return Redirect(Request.Headers["Referer"].ToString());
            }
            var data = from p in db.PhieuDatTour
                       join t in db.TourDuLiches
                       on p.MaTour equals t.MaTour
                       join k in db.KhachHangs
                       on p.MaKH equals k.MaKH
                       where p.TrangThai == "Đã xác nhận"
                       select new
                       {
                           p.SoPhieuDatTour,
                           k.TenKH,
                           t.TenTour,
                           p.SoLuong,
                           t.GiaVe,
                           ThanhTien = p.SoLuong * t.GiaVe,
                           p.NgayDatTour
                       };

            ViewBag.TongDoanhThu = data.Sum(x => x.ThanhTien);

            return View(data.ToList());
        }
    }
}