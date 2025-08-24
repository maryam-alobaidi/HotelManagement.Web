

namespace HotelManagement.BLL.Interfaces
{
    public interface IEmailService
    {
        // Method to send an email
        void SendEmail(string to, string subject, string body);

    }
}
