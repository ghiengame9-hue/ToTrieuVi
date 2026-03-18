using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeQLTour.Models
{
    [Table("PhieuDatTour")]
    public class PhieuDatTour
    {
        [Key]
        [Column("sophieudattour")]
        public int SoPhieuDatTour { get; set; }

        [Column("makh")]
        public int MaKH { get; set; }

        [Column("MaNV")]
        public int? MaNV { get; set; }

        [Column("matour")]
        public int MaTour { get; set; }

        [Column("loaitour")]
        public string? LoaiTour { get; set; }

        [Column("ngaykhoihanh")]
        public DateTime NgayKhoiHanh { get; set; }
        [Column("soluong")]
        public int SoLuong { get; set; }
        [Column("trangthai")]
        public string TrangThai { get; set; } = "Chờ xác nhận";

        public DateTime NgayDatTour { get; set; }
        public string? NguoiXuLy { get; set; }

    }
}