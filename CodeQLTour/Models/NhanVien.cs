using System;
using System.ComponentModel.DataAnnotations;

namespace CodeQLTour.Models
{
    public class NhanVien
    {
        [Key]
        public int MaNV { get; set; }

        public string? TenNV { get; set; }

        public DateTime? NgaySinh { get; set; }

        public string? GioiTinh { get; set; }

        public string? SoDienThoai { get; set; }

        public string? Email { get; set; }

        public string? DiaChi { get; set; }

        public string? ChucVu { get; set; }

        public int? AccountId { get; set; }
    }
}