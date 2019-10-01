using System;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Market.BLL.Interfaces;

namespace Market.BLL.Extensions
{
    public static class EmailSenderExtensions
    {
        public static Task SendEmailConfirmationAsync(this IEmailSender emailSender, string email, string link)
        {
            if (emailSender == null)
            {
                throw new ArgumentNullException(nameof(emailSender));
            }

            return emailSender.SendEmailAsync(email, "Confirm your email",
                $"Please confirm your account by clicking this link: <a href='{HtmlEncoder.Default.Encode(link)}'>link</a>");
        }
    }
}