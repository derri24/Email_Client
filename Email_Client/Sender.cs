﻿using System.Collections.Generic;
using System.Threading.Tasks;
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

        private static BodyBuilder CreateMessageContent(string message, List<string> listOfPath)
        {
            var builder = new BodyBuilder();
            for (int i = 0; i < listOfPath.Count; i++)
                builder.Attachments.Add(listOfPath[i]);
            builder.HtmlBody = message;
            return builder;
        }

        private static MimeMessage CreateMessage(string recipient, string subject, string message,
            List<string> listOfPath)
        {
            MimeMessage emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("", _email));
            emailMessage.To.Add(new MailboxAddress("", recipient));
            emailMessage.Subject = subject;
            emailMessage.Body = CreateMessageContent(message, listOfPath).ToMessageBody();
            return emailMessage;
        }

        public static async Task Authenticate(string email, string password)
        {
            await Task.Run(() =>
            {
                _email = email;
                _password = password;
            });
        }

        public static async Task Connect(string host, int port, bool ssl)
        {
            await Task.Run(() =>
            {
                _smtpClient = new SmtpClient();
                _smtpClient.Connect(host, port, ssl);
                _smtpClient.Authenticate(_email, _password);
                _isConnected = ssl;
            });
        }

        public static async Task SendMessage(string recipient, string subject, string message,
            List<string> listOfPath)
        {
            if (subject == "")
                subject = "(Без темы)";
            MimeMessage emailMessage = CreateMessage(recipient, subject, message, listOfPath);
            await Task.Run(() =>
            {
                _smtpClient.Send(emailMessage);
                Receiver.AppendSentMessage(emailMessage);
            });
        }

        public static async Task CloseConnection()
        {
            await Task.Run(() =>
            {
                if (_smtpClient != null)
                    _smtpClient.Disconnect(true);
            });
        }
    }
}