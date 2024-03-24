using API.Shared.Enums;

namespace API.Services.Common
{
    public class SuccessResponse
    {
        public ResponseCode StatusCode { get; set; } = ResponseCode.OK;

        public string? Message { get; set; }

    }
}
