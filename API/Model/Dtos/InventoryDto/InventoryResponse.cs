using API.Services.Common;

namespace API.Model.Dtos.InventoryDto
{
    public class InventoryResponse
    {
        public int Id { get; set; }

        public string Location { get; set; } = "unKnow";
        public int Stock { get; set; }

        public int ProductId { get; set; }
    }

    public class InventoryObjectResponse : ObjectResponse<InventoryResponse> { }
}
