using Microsoft.AspNetCore.Mvc;
using CodeQLTour.Models;
using Microsoft.EntityFrameworkCore;

namespace CodeQLTour.Controllers
{
    public class AccountController : Controller
    {
        private readonly TourContext db;

        public AccountController(TourContext context)
        {
            db = context;
        }

        // =========================
        // MỞ TRANG LOGIN + REGISTER
        // =========================
        [HttpGet]
        public IActionResult Auth()
        {
            return View();
        }

        // =========================
        // ĐĂNG KÝ TÀI KHOẢN
        // =========================
        [HttpPost]
        public IActionResult Register(string Username, string Password, string ConfirmPassword, string Phone)
        {
            if (Password != ConfirmPassword)
            {
                ViewBag.ErrorRegister = "Mật khẩu nhập lại không đúng";
                return View("Auth");
            }

            var check = db.Accounts.FirstOrDefault(x => x.Username == Username);

            if (check != null)
            {
                ViewBag.ErrorRegister = "Tài khoản đã tồn tại";
                return View("Auth");
            }

            // tạo account
            Account acc = new Account
            {
                Username = Username,
                Password = Password,
                Phone = Phone,
                VaiTro = "KhachHang"
            };

            db.Accounts.Add(acc);
            db.SaveChanges();

            // tạo khách hàng
            KhachHang kh = new KhachHang
            {
                TenKH = "Khách mới",
                SoDienThoai = Phone,
                AccountId = acc.Id
            };

            db.KhachHangs.Add(kh);
            db.SaveChanges();

            // LƯU SESSION
            HttpContext.Session.SetString("Username", acc.Username);
            HttpContext.Session.SetString("VaiTro", acc.VaiTro);
            HttpContext.Session.SetInt32("MaKH", kh.MaKH);
            HttpContext.Session.SetInt32("AccountId", acc.Id);
            // 
            ViewBag.Success = "Đăng ký thành công, hãy đăng nhập.";
            return View("Auth");
        }
        // =========================
        // ĐĂNG NHẬP
        // =========================
        [HttpPost]
        public IActionResult Login(string Username, string Password)
        {
            var acc = db.Accounts
                .FirstOrDefault(x => x.Username == Username && x.Password == Password);

            if (acc == null)
            {
                ViewBag.Error = "Sai tài khoản hoặc mật khẩu";
                return View("Auth");
            }

            // lưu session
            HttpContext.Session.SetString("User", acc.Username);
            HttpContext.Session.SetString("VaiTro", acc.VaiTro);
            HttpContext.Session.SetInt32("AccountId", acc.Id);
            // tìm khách hàng
            // tìm khách hàng
            var kh = db.KhachHangs.FirstOrDefault(x => x.AccountId == acc.Id);
            if (kh != null)
            {
                HttpContext.Session.SetInt32("MaKH", kh.MaKH);
            }

            // tìm nhân viên
            var nv = db.NhanViens.FirstOrDefault(x => x.AccountId == acc.Id);

            if (nv != null)
            {
                HttpContext.Session.SetInt32("MaNV", nv.MaNV);
            }
            else if (acc.VaiTro == "NhanVien")
            {
                // nếu bảng nhân viên chưa có dữ liệu
                HttpContext.Session.SetInt32("MaNV", acc.Id);
            }
            // phân quyền theo vai trò
            if (acc.VaiTro == "Admin")
            {
                return RedirectToAction("Index", "Admin");
            }

            else if (acc.VaiTro == "NhanVien")
            {
                // lấy account làm mã nhân viên tạm
                HttpContext.Session.SetInt32("MaNV", acc.Id);

                return RedirectToAction("DatTour", "Admin");
            }
            else
            {
                return RedirectToAction("Index", "Tour");
            }
        }
        // =========================
        // THÊM THÔNG TIN KHÁCH HÀNG
        // =========================
        [HttpPost]
        public IActionResult CreateKhachHang(KhachHang kh)
        {
            var username = HttpContext.Session.GetString("User");

            if (username == null)
            {
                return RedirectToAction("Auth");
            }

            var acc = db.Accounts.FirstOrDefault(x => x.Username == username);

            if (acc != null)
            {
                var existing = db.KhachHangs.FirstOrDefault(x => x.AccountId == acc.Id);

                if (existing != null)
                {
                    existing.TenKH = kh.TenKH;
                    existing.Email = kh.Email;
                    existing.SoDienThoai = acc.Phone;

                    db.SaveChanges();
                }
            }

            return RedirectToAction("Profile", "KhachHang");
        }

        // =========================
        // ĐĂNG XUẤT
        // =========================
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Tour");
        }
    }
}