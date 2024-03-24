using API.Services.Common;

namespace API.Model.Dtos.CartDto
{
    public class CartResponse
    {
        public string UserId { get; set; }
        public List<CartItemDto> CartItems { get; set; }
    }

    public class CartObjectResponse : ObjectResponse<CartResponse> { }
}
