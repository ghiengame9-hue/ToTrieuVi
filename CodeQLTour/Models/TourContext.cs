using Microsoft.EntityFrameworkCore;

namespace CodeQLTour.Models
{
    public class TourContext : DbContext
    {
        public TourContext(DbContextOptions<TourContext> options) : base(options)
        {
        }

        public DbSet<NhanVien> NhanViens { get; set; }

        public DbSet<KhachHang> KhachHangs { get; set; }

        public DbSet<TourDuLich> TourDuLiches { get; set; }

        public DbSet<PhieuDatTour> PhieuDatTour { get; set; }

        public DbSet<VeDuLich> VeDuLiches { get; set; }


        public DbSet<Account> Accounts { get; set; }

    }
}