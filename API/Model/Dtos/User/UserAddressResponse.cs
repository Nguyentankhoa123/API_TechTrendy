using API.Services.Common;
using System.ComponentModel;

namespace API.Model.Dtos.User
{
    public class UserAddressResponse
    {
        public int Id { get; set; }
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
    }

    public class UserAddressObjectResponse : ObjectResponse<UserAddressResponse> { }

    public class UserAddressListObjectResponse : ObjectResponse<List<UserAddressResponse>> { }
}
