using Microsoft.AspNetCore.Mvc;
using CodeQLTour.Models;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CodeQLTour.Controllers
{
    public class TourController : Controller
    {
        private readonly TourContext db;

        public TourController(TourContext context)
        {
            db = context;
        }

        // CHẶN NHÂN VIÊN
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var role = HttpContext.Session.GetString("VaiTro");

            // nhân viên không được vào trang bán tour
            if (role == "NhanVien")
            {
                context.Result = RedirectToAction("DatTour", "Admin");
                return;
            }

            base.OnActionExecuting(context);
        }

        // TRANG CHỦ TOUR
        public IActionResult Index()
        {
            var tours = db.TourDuLiches.ToList();

            ViewBag.UuDai = tours.Where(t => t.ViTri == "UuDai").ToList();
            ViewBag.NhatHan = tours.Where(t => t.ViTri == "NhatHan").ToList();
            ViewBag.TrungHongDai = tours.Where(t => t.ViTri == "TrungHongDai").ToList();
            ViewBag.SingMalayIndo = tours.Where(t => t.ViTri == "SingMalayIndo").ToList();
            ViewBag.PhoBien = tours.Where(t => t.ViTri == "PhoBien").ToList();

            return View(tours);
        }

        // CHI TIẾT TOUR
        public IActionResult Details(int id)
        {
            var tour = db.TourDuLiches.FirstOrDefault(t => t.MaTour == id);

            if (tour == null)
            {
                return NotFound();
            }

            return View(tour);
        }

        // MUA TOUR
        public IActionResult MuaTour(int id)
        {
            var role = HttpContext.Session.GetString("VaiTro");

            // chưa đăng nhập
            if (role == null)
            {
                return RedirectToAction("Auth", "Account");
            }

            var tour = db.TourDuLiches.FirstOrDefault(t => t.MaTour == id);

            if (tour == null)
            {
                return NotFound();
            }

            return View(tour);
        }

        // ======================
        // ADMIN CRUD TOUR
        // ======================

        [HttpPost]
        public IActionResult Create(TourDuLich tour)
        {
            var role = HttpContext.Session.GetString("VaiTro");

            if (role != "Admin")
            {
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                db.TourDuLiches.Add(tour);
                db.SaveChanges();
            }

            return RedirectToAction("Tours", "Admin");
        }

        public IActionResult Edit(int id)
        {
            var role = HttpContext.Session.GetString("VaiTro");

            if (role != "Admin")
            {
                return RedirectToAction("Index");
            }

            var tour = db.TourDuLiches.Find(id);

            if (tour == null)
            {
                return NotFound();
            }

            return View(tour);
        }

        [HttpPost]
        public IActionResult Edit(TourDuLich tour)
        {
            var role = HttpContext.Session.GetString("VaiTro");

            if (role != "Admin")
            {
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                db.TourDuLiches.Update(tour);
                db.SaveChanges();
            }

            return RedirectToAction("Tours", "Admin");
        }

        public IActionResult Delete(int id)
        {
            var role = HttpContext.Session.GetString("VaiTro");

            if (role != "Admin")
            {
                return RedirectToAction("Index");
            }

            var tour = db.TourDuLiches.Find(id);

            if (tour != null)
            {
                db.TourDuLiches.Remove(tour);
                db.SaveChanges();
            }

            return RedirectToAction("Tours", "Admin");
        }
    }
}