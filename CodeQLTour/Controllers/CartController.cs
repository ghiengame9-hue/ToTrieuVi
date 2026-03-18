using Microsoft.AspNetCore.Mvc;
using CodeQLTour.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CodeQLTour.Controllers
{
    public class CartController : Controller
    {
        private readonly TourContext db;

        public CartController(TourContext context)
        {
            db = context;
        }

        // CHẶN NHÂN VIÊN
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

        // ================= GIỎ HÀNG =================

        public IActionResult Index()
        {
            var makh = HttpContext.Session.GetInt32("MaKH");

            if (makh == null)
                return RedirectToAction("Auth", "Account");

            var cart = db.PhieuDatTour
                .Where(x => x.MaKH == makh && x.LoaiTour == "Cart")
                .Join(db.TourDuLiches,
                    p => p.MaTour,
                    t => t.MaTour,
                    (p, t) => new
                    {
                        p.SoPhieuDatTour,
                        p.MaTour,
                        t.TenTour,
                        t.GiaVe,
                        t.HinhAnh,
                        p.SoLuong,
                        p.NgayKhoiHanh,
                        TongTien = p.SoLuong * t.GiaVe
                    })
                .ToList();

            return View(cart);
        }
        // ================= THÊM TOUR =================

        public IActionResult Add(int id, DateTime? ngayKhoiHanh)
        {
            var makh = HttpContext.Session.GetInt32("MaKH");

            // chưa đăng nhập
            if (makh == null)
                return RedirectToAction("Auth", "Account");

            if (ngayKhoiHanh == null)
                ngayKhoiHanh = DateTime.Today.AddDays(3);

            var old = db.PhieuDatTour
                .FirstOrDefault(x => x.MaKH == makh && x.MaTour == id && x.LoaiTour == "Cart");

            if (old != null)
            {
                old.SoLuong += 1;
            }
            else
            {
                var item = new PhieuDatTour
                {
                    MaKH = makh.Value,
                    MaTour = id,
                    LoaiTour = "Cart",
                    SoLuong = 1,
                    TrangThai = "Chờ xác nhận",
                    NgayKhoiHanh = ngayKhoiHanh.Value,
                    NgayDatTour = DateTime.Now
                };

                db.PhieuDatTour.Add(item);
            }

            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // ================= XÓA TOUR =================

        public IActionResult Remove(int id)
        {
            var item = db.PhieuDatTour.FirstOrDefault(x => x.SoPhieuDatTour == id);

            if (item != null)
            {
                db.PhieuDatTour.Remove(item);
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        // ================= TĂNG SỐ LƯỢNG =================

        public IActionResult Increase(int id)
        {
            var makh = HttpContext.Session.GetInt32("MaKH");

            var item = db.PhieuDatTour
                .FirstOrDefault(x => x.MaKH == makh && x.MaTour == id && x.LoaiTour == "Cart");

            if (item != null)
            {
                item.SoLuong += 1;
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        // ================= GIẢM SỐ LƯỢNG =================

        public IActionResult Decrease(int id)
        {
            var makh = HttpContext.Session.GetInt32("MaKH");

            var item = db.PhieuDatTour
                .FirstOrDefault(x => x.MaKH == makh && x.MaTour == id && x.LoaiTour == "Cart");

            if (item != null)
            {
                if (item.SoLuong > 1)
                    item.SoLuong -= 1;
                else
                    db.PhieuDatTour.Remove(item);

                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        // ================= ĐỔI NGÀY =================

        public IActionResult ChangeDate(int id, DateTime date)
        {
            var makh = HttpContext.Session.GetInt32("MaKH");

            var item = db.PhieuDatTour
                .FirstOrDefault(x => x.MaKH == makh && x.MaTour == id && x.LoaiTour == "Cart");

            if (item != null)
            {
                item.NgayKhoiHanh = date;
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        // ================= CHECKOUT =================

        public IActionResult Checkout()
        {
            var makh = HttpContext.Session.GetInt32("MaKH");

            if (makh == null)
                return RedirectToAction("Auth", "Account");

            var cart = db.PhieuDatTour
                .Where(x => x.MaKH == makh && x.LoaiTour == "Cart")
                .Join(db.TourDuLiches,
                    p => p.MaTour,
                    t => t.MaTour,
                    (p, t) => new
                    {
                        p.SoPhieuDatTour,
                        p.MaTour,
                        p.SoLuong,
                        p.NgayKhoiHanh,
                        t.TenTour,
                        t.GiaVe,
                        t.HinhAnh,
                        TongTien = p.SoLuong * t.GiaVe
                    })
                .ToList();

            var kh = db.KhachHangs.FirstOrDefault(x => x.MaKH == makh);

            ViewBag.TenKH = kh?.TenKH;
            ViewBag.SDT = kh?.SoDienThoai;
            ViewBag.DiaChi = kh?.DiaChi;

            return View(cart);
        }

        // ================= ĐẶT TOUR =================

        [HttpPost]
        public IActionResult DatTour()
        {
            var makh = HttpContext.Session.GetInt32("MaKH");
            var user = HttpContext.Session.GetString("User");
            if (makh == null)
                return RedirectToAction("Auth", "Account");

            var cart = db.PhieuDatTour
                .Where(x => x.MaKH == makh && x.LoaiTour == "Cart")
                .ToList();

            foreach (var item in cart)
            {
                item.LoaiTour = "DaDat";
                item.TrangThai = "Chờ xác nhận";
                item.NgayDatTour = DateTime.Now;
                item.NguoiXuLy = user;
            }

            db.SaveChanges();

            return RedirectToAction("LichSu");
        }
        [HttpPost]
        public IActionResult XacNhanDon(int id)
        {
            var don = db.PhieuDatTour.Find(id);

            if (don != null)
            {
                don.TrangThai = "Đã xác nhận";
                don.NguoiXuLy = HttpContext.Session.GetString("User");
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
                don.NguoiXuLy = HttpContext.Session.GetString("User");
                db.SaveChanges();
            }

            return RedirectToAction("DatTour");
        }

        // ================= LỊCH SỬ =================

        public IActionResult LichSu()
        {
            var makh = HttpContext.Session.GetInt32("MaKH");

            if (makh == null)
                return RedirectToAction("Auth", "Account");

            var data = db.PhieuDatTour
                .Where(x => x.MaKH == makh && x.LoaiTour == "DaDat")
                .Join(db.TourDuLiches,
                    p => p.MaTour,
                    t => t.MaTour,
                    (p, t) => new
                    {
                        p.SoPhieuDatTour,
                        t.TenTour,
                        t.GiaVe,
                        p.SoLuong,
                        p.NgayKhoiHanh,
                        p.NgayDatTour,
                        TrangThai = p.TrangThai ?? "Chờ xác nhận"
                    })
                .ToList();

            return View(data);
        }

        // ================= HỦY ĐƠN =================

        public IActionResult HuyDon(int id)
        {
            var don = db.PhieuDatTour.FirstOrDefault(x => x.SoPhieuDatTour == id);

            if (don != null && don.TrangThai == "Chờ xác nhận")
            {
                don.TrangThai = "Đã hủy";
                db.SaveChanges();
            }

            return RedirectToAction("LichSu");
        }
    }
}