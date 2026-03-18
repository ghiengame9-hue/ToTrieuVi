using Microsoft.AspNetCore.Mvc;
using CodeQLTour.Models;
using Microsoft.AspNetCore.Mvc.Filters;
namespace CodeQLTour.Controllers
{
    public class KhachHangController : Controller
    {
        private readonly TourContext db;

        public KhachHangController(TourContext context)
        {
            db = context;
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var role = HttpContext.Session.GetString("VaiTro");

            if (role == "NhanVien")
            {
                context.Result = RedirectToAction("DatTour", "Admin");
                return;
            }

            base.OnActionExecuting(context);
        }
        // HIỂN THỊ PROFILE
        public IActionResult Profile()
        {
            var accountId = HttpContext.Session.GetInt32("AccountId");

            if (accountId == null)
            {
                return RedirectToAction("Auth", "Account");
            }

            var kh = db.KhachHangs.FirstOrDefault(x => x.AccountId == accountId);

            if (kh == null)
            {
                return RedirectToAction("Auth", "Account");
            }

            return View(kh);
        }

        // CẬP NHẬT THÔNG TIN
        [HttpPost]
        public IActionResult UpdateProfile(KhachHang kh)
        {
            var role = HttpContext.Session.GetString("VaiTro");
            var makh = HttpContext.Session.GetInt32("MaKH");

            // chưa đăng nhập
            if (role == null)
            {
                return RedirectToAction("Auth", "Account");
            }

            // admin không có profile khách hàng
            if (role == "Admin")
            {
                return RedirectToAction("Index", "Tour");
            }

            var old = db.KhachHangs.Find(makh);

            if (old != null)
            {
                old.TenKH = kh.TenKH;
                old.NgaySinh = kh.NgaySinh;
                old.SoDienThoai = kh.SoDienThoai;
                old.Email = kh.Email;
                old.DiaChi = kh.DiaChi;

                db.SaveChanges();
            }

            return RedirectToAction("Profile");
        }

        // XÓA TÀI KHOẢN
        public IActionResult Delete()
        {
            var accountId = HttpContext.Session.GetInt32("AccountId");

            if (accountId == null)
            {
                return RedirectToAction("Auth", "Account");
            }

            var kh = db.KhachHangs.FirstOrDefault(x => x.AccountId == accountId);

            if (kh != null)
            {
                db.KhachHangs.Remove(kh);
                db.SaveChanges();
            }

            HttpContext.Session.Clear();

            return RedirectToAction("Auth", "Account");
        }
        [HttpPost]
        public IActionResult ChangePassword(string OldPassword, string NewPassword, string ConfirmPassword)
        {
            var accountId = HttpContext.Session.GetInt32("AccountId");

            if (accountId == null)
            {
                return RedirectToAction("Auth", "Account");
            }

            var acc = db.Accounts.FirstOrDefault(x => x.Id == accountId);

            if (acc == null)
            {
                return RedirectToAction("Auth", "Account");
            }

            // kiểm tra mật khẩu cũ
            if (acc.Password != OldPassword)
            {
                ViewBag.Error = "Mật khẩu cũ không đúng";
                return View("Profile", GetKhachHang());
            }

            // kiểm tra nhập lại mật khẩu
            if (NewPassword != ConfirmPassword)
            {
                ViewBag.Error = "Mật khẩu nhập lại không khớp";
                return View("Profile", GetKhachHang());
            }

            // cập nhật mật khẩu
            acc.Password = NewPassword;
            db.SaveChanges();

            ViewBag.Success = "Đổi mật khẩu thành công";

            return View("Profile", GetKhachHang());
        }
        public KhachHang GetKhachHang()
        {
            var accountId = HttpContext.Session.GetInt32("AccountId");

            if (accountId == null)
            {
                return null;
            }

            return db.KhachHangs.FirstOrDefault(x => x.AccountId == accountId);
        }
    }
}