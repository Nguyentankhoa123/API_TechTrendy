using API.Model.Entity;
using API.Services.Common;

namespace API.Model.Dtos.ProductDto
{
    public class ProductResponse : Product
    {
    }

    public class ProductDiscountReponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class ProductObjectResponse : ObjectResponse<ProductResponse> { }
}
