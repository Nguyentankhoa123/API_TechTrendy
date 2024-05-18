using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace API.Model.Dtos.TabletDto
{
    public class TabletRequest
    {
        [DisplayName("Tên Sản phẩm")]
        [Required(ErrorMessage = "Tên sản phẩm không được để trống")]
        public string Name { get; set; }
        [DisplayName("Tên hệ điều hành")]
        [Required(ErrorMessage = "Tên hệ điều hành không được để trống")]
        public string OS { get; set; }
        [DisplayName("CPU")]
        [Required(ErrorMessage = "Tên CPU không được để trống")]
        public string CPU { get; set; }

        [DisplayName("Màn hình")]
        [Required(ErrorMessage = "Màn hình không được để trống")]
        public string Screen { get; set; }
        [DisplayName("Ram")]
        [Required(ErrorMessage = "RAM không được để trống")]
        public string RAM { get; set; }
        [DisplayName("Ổ cứng")]
        [Required(ErrorMessage = "Ổ cứng không được để trống")]
        public string Storage { get; set; }

        [DisplayName("Cổng kết nối")]
        [Required(ErrorMessage = "Cổng kết nối không được để trống")]
        public List<string> Ports { get; set; }

        [DisplayName("Chất liệu")]
        [Required(ErrorMessage = "Chất liệu không được để trống")]
        public string Design { get; set; }

        [DisplayName("Kích thước")]
        [Required(ErrorMessage = "Kích thước không được để trống")]
        public string Weight { get; set; }

        [Required]
        [DisplayName("Giá tiền")]
        [Range(1, double.MaxValue, ErrorMessage = "Giá tiền không hợp lệ")]
        [DisplayFormat(DataFormatString = "{0:#,###.##đ}")]
        public double Price { get; set; }

        [DisplayName("Hình ảnh")]
        [Required(ErrorMessage = "Ảnh không được để trống")]
        public List<string> PictureUrls { get; set; }

        [DisplayName("Mô tả sản phẩm")]
        [Required(ErrorMessage = "Mô tả sản phẩm không được để trống")]
        public string Description { get; set; }

        public int ReleaseDate { get; set; }
        public int Quantity { get; set; }

        [Required]
        public string Connectivity { get; set; }
        [Required]
        public string Sim { get; set; }
        [Required]
        public string RearCamera { get; set; }
        [Required]

        public string BackCamera { get; set; }
        [Required]
        public string Battery { get; set; }
    }
}
