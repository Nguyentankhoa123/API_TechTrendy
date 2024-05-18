using API.Model.Dtos.ProductDto;
using API.Services.Common;
using API.Shared.Enums;

namespace API.Model.Dtos.DiscountDto
{
    public class DiscountResponse
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public double Percent { get; set; }
        public double? MaxDiscountAmount { get; set; }
        public double? MinOrderValue { get; set; }
        public string Code { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int MaxUse { get; set; }
        public int UseCount { get; set; }
        public bool IsActive { get; set; }
        public DiscountApply ApplyTo { get; set; }
        public List<int>? ProductId { get; set; }

        public List<ProductDiscountReponse> Products { get; set; }
    }

    public class DiscountProductResponse
    {
        public double Percent { get; set; }
        public double? MaxDiscountAmount { get; set; }
        public double? MinOrderValue { get; set; }
        public List<ProductDiscountReponse> Products { get; set; }

    }

    public class DiscountUserResponse
    {
        public string Code { get; set; }
        public double Percent { get; set; }
        public double? MaxDiscountAmount { get; set; }
        public double? MinOrderValue { get; set; }

        public string Name { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

    }


    public class DiscountObjectResponse : ObjectResponse<DiscountResponse> { }

    public class DiscountProductObjectResponse : ObjectResponse<DiscountProductResponse> { }

    public class DiscountUserObjectResponse : ObjectResponse<List<DiscountUserResponse>> { }
}
