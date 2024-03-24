using API.Helpers;

namespace API.Repository
{
    public interface IEmailSenderRepository
    {
        void SendEmail(Message message);
    }
}
