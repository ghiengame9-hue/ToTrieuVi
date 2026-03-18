using System;
using System.ComponentModel.DataAnnotations;

namespace CodeQLTour.Models
{
    public class KhachHang
    {
        [Key]
        public int MaKH { get; set; }

        public string? TenKH { get; set; }

        public DateTime? NgaySinh { get; set; }

        public string? SoDienThoai { get; set; }

        public string? Email { get; set; }

        public string? DiaChi { get; set; }

        public int AccountId { get; set; }
    }
}