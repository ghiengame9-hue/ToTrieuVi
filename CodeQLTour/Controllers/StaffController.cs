using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using CodeQLTour.Models;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Filters;
namespace CodeQLTour.Controllers
{
    public class StaffController : Controller
    {
        private readonly TourContext db;

        public StaffController(TourContext context)
        {
            db = context;
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var role = HttpContext.Session.GetString("VaiTro");

            if (string.IsNullOrEmpty(role))
            {
                context.Result = RedirectToAction("Auth", "Account");
                return;
            }

            if (role != "NhanVien")
            {
                context.Result = RedirectToAction("Index", "Tour");
                return;
            }

            base.OnActionExecuting(context);
        }
        // Trang hồ sơ nhân viên
        public IActionResult ProfileNhanVien()
        {
            var accountId = HttpContext.Session.GetInt32("AccountId");
            var role = HttpContext.Session.GetString("VaiTro"); // nhớ đúng tên session

            if (accountId == null)
                return RedirectToAction("Auth", "Account");

            // Chỉ cho Nhân viên vào
            if (role != "NhanVien")
                return RedirectToAction("Index", "Tour");

            var nv = db.NhanViens.FirstOrDefault(x => x.AccountId == accountId);

            return View(nv);
        }

        // Cập nhật thông tin nhân viên
        [HttpPost]
        public IActionResult UpdateProfile(NhanVien nv)
        {
            var accountId = HttpContext.Session.GetInt32("AccountId");

            if (accountId == null)
                return RedirectToAction("Auth", "Account");

            var existing = db.NhanViens.FirstOrDefault(x => x.AccountId == accountId);

            if (existing == null)
            {
                var newNV = new NhanVien
                {
                    TenNV = nv.TenNV,
                    NgaySinh = nv.NgaySinh,
                    GioiTinh = nv.GioiTinh,
                    SoDienThoai = nv.SoDienThoai,
                    Email = nv.Email,
                    DiaChi = nv.DiaChi,
                    ChucVu = "Nhân viên",
                    AccountId = accountId.Value
                };

                db.NhanViens.Add(newNV);
            }
            else
            {
                existing.TenNV = nv.TenNV;
                existing.NgaySinh = nv.NgaySinh;
                existing.GioiTinh = nv.GioiTinh;
                existing.SoDienThoai = nv.SoDienThoai;
                existing.Email = nv.Email;
                existing.DiaChi = nv.DiaChi;
            }

            db.SaveChanges();

            return RedirectToAction("ProfileNhanVien");
        }

        // Đổi mật khẩu
        [HttpPost]
        public IActionResult ChangePassword(string OldPassword, string NewPassword, string ConfirmPassword)
        {
            var accountId = HttpContext.Session.GetInt32("AccountId");

            if (accountId == null)
                return RedirectToAction("Auth", "Account");

            var acc = db.Accounts.FirstOrDefault(x => x.Id == accountId);

            if (acc == null)
                return RedirectToAction("Auth", "Account");

            if (acc.Password != OldPassword)
            {
                ViewBag.Error = "Mật khẩu cũ không đúng";
                var nv = db.NhanViens.FirstOrDefault(x => x.AccountId == accountId);
                return View("ProfileNhanVien", nv);
            }

            if (NewPassword != ConfirmPassword)
            {
                ViewBag.Error = "Mật khẩu nhập lại không khớp";
                var nv = db.NhanViens.FirstOrDefault(x => x.AccountId == accountId);
                return View("ProfileNhanVien", nv);
            }

            acc.Password = NewPassword;

            db.SaveChanges();

            ViewBag.Success = "Đổi mật khẩu thành công";

            var nhanVien = db.NhanViens.FirstOrDefault(x => x.AccountId == accountId);

            return View("ProfileNhanVien", nhanVien);
        }
      

    }
}