using MimeKit;
namespace API;

public class MailService
{
    public void SendEmail(string emailMessage, string userEmail)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("KeepSocial", Environment.GetEnvironmentVariable("fromemail")));
        message.To.Add(new MailboxAddress("User", userEmail));
        message.Subject = "Account Changes";

        message.Body = new TextPart("plain")
        {
            Text = @""+emailMessage
        };

        using (var client = new MailKit.Net.Smtp.SmtpClient())
        {
            client.Connect("smtp.gmail.com", 465, true);
            client.Authenticate(Environment.GetEnvironmentVariable("fromemail"), Environment.GetEnvironmentVariable("frompass") );
            client.Send(message);
            client.Disconnect(true);
        }
    }
}