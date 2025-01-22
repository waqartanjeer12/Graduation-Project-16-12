using ECommerceCore.DTOs.User.Account;
using System.Net;
using System.Net.Mail;

namespace ECommerceCore.Interfaces
{
    public class EmailSettings
    {
        public static void SendEmail(Email email)
        {
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential("waqar.tanger12@gmail.com", "lkxh bcna ybec iqyw")
            };

            client.Send("waqar.tanger12@gmail.com", email.Recivers, email.Subject, email.Body);
        }
    }
}