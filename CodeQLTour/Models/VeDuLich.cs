using System;
using System.ComponentModel.DataAnnotations;

namespace CodeQLTour.Models
{
    public class VeDuLich
    {
        [Key]
        public int SoVe { get; set; }

        public string TenTour { get; set; }

        public double GiaVe { get; set; }

        public DateTime NgayKhoiHanh { get; set; }

        public int SoPhieuDatTour { get; set; }
    }
}