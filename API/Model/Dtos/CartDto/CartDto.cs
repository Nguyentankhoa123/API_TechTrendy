namespace API.Model.Dtos.CartDto
{
    public class CartDto
    {
        public string UserId { get; set; }
        public List<CartItemDto> CartItems { get; set; }
    }
}
