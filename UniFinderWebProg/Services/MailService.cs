using System.Net.Mail;
using System.Net;

public class MailService
{
    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        var smtpClient = new SmtpClient("smtp.gmail.com")
        {
            Port = 587,
            Credentials = new NetworkCredential("unifinderr@gmail.com", "uzzz iutq ofgh xudu"),
            EnableSsl = true
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress("unifinderr@gmail.com"),
            Subject = subject,
            Body = body,
            IsBodyHtml = true,
        };

        mailMessage.To.Add(toEmail);

        try
        {
            await smtpClient.SendMailAsync(mailMessage);
        }
        catch (Exception ex)
        {
            throw new Exception("E-posta gönderim hatası: " + ex.Message);
        }
    }
}
