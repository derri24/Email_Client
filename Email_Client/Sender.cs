using System.Collections.Generic;
using MailKit.Net.Smtp;
using MimeKit;

namespace Email_Client
{
    public static class Sender
    {
        private static SmtpClient smtpClient;

        private static string email;
        private static string password;
        private static string host;
        private static int port;


        public static void Authorization(string _host, int _port, string _email, string _password, bool _useSsl)
        {
            host = _host;
            port = _port;
            email = _email;
            password = _password;

            smtpClient = new SmtpClient();
            smtpClient.Connect(host, port, _useSsl);
            smtpClient.Authenticate(email, password);
        }


        public static BodyBuilder CreateMessageContent(string message, List<string> listOfString)
        {
            var builder = new BodyBuilder();
            for (int i = 0; i < listOfString.Count; i++)
            {
                builder.Attachments.Add(listOfString[i]);
            }
            builder.TextBody = message;
            return builder;
        }

        private static MimeMessage CreateMessage(string recipient, string subject, string message,
            List<string> listOfString)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("", email));
            emailMessage.To.Add(new MailboxAddress("", recipient));
            emailMessage.Subject = subject;
            emailMessage.Body = CreateMessageContent(message, listOfString).ToMessageBody();
            return emailMessage;
        }

        public static void SendMessage(string recipient, string subject, string message,  List<string> listOfString)
        {
            MimeMessage emailMessage = CreateMessage(recipient, subject, message, listOfString);
            smtpClient.Send(emailMessage);
        }
        
        public static void CloseConnection()
        {
            if (smtpClient != null) 
            smtpClient.Disconnect(true);
        }
    }
}