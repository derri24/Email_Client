using System.Collections.Generic;
using MailKit;
using MailKit.Net.Smtp;
using MimeKit;

namespace Email_Client
{
    public static class Sender
    {
        private static SmtpClient _smtpClient;

        private static string _email;
        private static string _password;
        private static bool _isConnected;
        public static bool IsConnected => _isConnected;

        public static BodyBuilder CreateMessageContent(string message, List<string> listOfString)
        {
            var builder = new BodyBuilder();
            for (int i = 0; i < listOfString.Count; i++)
                builder.Attachments.Add(listOfString[i]);
            builder.HtmlBody = message;
            return builder;
        }

        private static MimeMessage CreateMessage(string recipient, string subject, string message,
            List<string> listOfString)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("", _email));
            emailMessage.To.Add(new MailboxAddress("", recipient));
            emailMessage.Subject = subject;
            emailMessage.Body = CreateMessageContent(message, listOfString).ToMessageBody();
            return emailMessage;
        }

        public static void Authenticate(string email, string password)
        {
            _email = email;
            _password = password;
        }

        public static void Connect(string host, int port, bool ssl)
        {
            _smtpClient = new SmtpClient();
            _smtpClient.Connect(host, port, ssl);
            _smtpClient.Authenticate(_email, _password);
            _isConnected = true;
        }

        public static void SendMessage(string recipient, string subject, string message, List<string> listOfString)
        {
            MimeMessage emailMessage = CreateMessage(recipient, subject, message, listOfString);
            _smtpClient.Send(emailMessage);
        }

        public static void CloseConnection()
        {
            if (_smtpClient != null)
                _smtpClient.Disconnect(true);
        }
    }
}