using System.ComponentModel;

namespace API.Model.Entity
{
    public class UserAddress
    {
        public int Id { get; set; }
        [DisplayName("Tên người nhận")]
        public string Name { get; set; }
        [DisplayName("Số điện thoại giao hàng")]
        public string Contact { get; set; }
        [DisplayName("Địa chỉ")]
        public string Street { get; set; }
        [DisplayName("Xã/Phường")]
        public string Ward { get; set; }
        [DisplayName("Quận/Huyện")]
        public string District { get; set; }
        [DisplayName("Tỉnh/Thành phố")]
        public string Province { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}
