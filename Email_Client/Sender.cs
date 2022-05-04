using MailKit.Net.Smtp;
using MimeKit;

namespace Email_Client
{
    public class Sender
    {
        private static SmtpClient smtpClient;

        private static string email;
        private static string password;
        private static string host;
        private static int port;


        public static void Authorization(string _host, int _port, string _email, string _password)
        {
            host = _host;
            port = _port;
            email = _email;
            password = _password;

            smtpClient = new SmtpClient();
            smtpClient.Connect(host, port, false);
            smtpClient.Authenticate(email, password);
        }


        public static BodyBuilder CreateMessageContent(string message, string pathForAttachment)
        {
            var builder = new BodyBuilder();
            if (pathForAttachment != null)
                builder.Attachments.Add(pathForAttachment);
            builder.TextBody = message;
            return builder;
        }

        private static MimeMessage CreateMessage(string recipient, string subject, string message,
            string pathForAttachment)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("", email));
            emailMessage.To.Add(new MailboxAddress("", recipient));
            emailMessage.Subject = subject;
            emailMessage.Body = CreateMessageContent(message, pathForAttachment).ToMessageBody();
            return emailMessage;
        }

        public static void SendMessage(string recipient, string subject, string message, string pathForAttachment)
        {
            MimeMessage emailMessage = CreateMessage(recipient, subject, message,pathForAttachment);
            smtpClient.Send(emailMessage);
            smtpClient.Disconnect(true);
        }
    }
}