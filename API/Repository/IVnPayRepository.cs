using API.Model.Dtos.VnPayDto;

namespace API.Repository
{
    public interface IVnPayRepository
    {
        string CreatePaymentUrl(HttpContext context, VnPayRequest request);

        VnPayResponse PaymentExecute(IQueryCollection collections);
    }
}
