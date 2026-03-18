using Microsoft.AspNetCore.Mvc;

namespace CodeQLTour.Models
{
    public class NhanVienViewModel
    {
        public int? MaNV { get; set; }

        public int? AccountId { get; set; }

        public string? Username { get; set; }

        public string? TenNV { get; set; }

        public string? ChucVu { get; set; }

        public string? SoDienThoai { get; set; }
    }
}
