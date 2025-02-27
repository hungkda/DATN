using DATN.ViewModels;

namespace DATN.Services
{
    public interface IEmailServices
    {
        void SendEmail(Message message);
    }
}
