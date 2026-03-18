using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeQLTour.Models
{
    [Table("TourDuLiches")]
    public class TourDuLich
    {
        [Key]
        [Column("matour")]
        public int MaTour { get; set; }

        [Column("tentour")]
        [Required(ErrorMessage = "Vui lòng nhập tên tour")]
        public string TenTour { get; set; }

        [Column("giave")]
        [Required(ErrorMessage = "Vui lòng nhập giá vé")]
        public double? GiaVe { get; set; }

        [Column("thoigiandi")]
        [Required(ErrorMessage = "Vui lòng nhập thời gian tour")]
        public string ThoiGianDi { get; set; }

        [Column("tinhtrang")]
        [Required(ErrorMessage = "Vui lòng chọn tình trạng")]
        public string TinhTrang { get; set; }

        [Column("hinhanh")]
        public string? HinhAnh { get; set; }

        public string? ViTri { get; set; }
    }
}