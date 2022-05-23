using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using MailKit;
using MailKit.Net.Imap;
using MimeKit;


namespace Email_Client
{
    public static class Receiver
    {
        public static byte TypeMessage;
        private static ImapClient imapClient;
        private static IMailFolder mailFolder;

        private static string email;
        private static string password;
        private static string host;
        private static int port;


        public static void Authorization(string _host, int _port, string _email, string _password, bool _Ssl)
        {
            host = _host;
            port = _port;
            email = _email;
            password = _password;

            imapClient = new ImapClient();
            imapClient.Connect(host, port, _Ssl);
            imapClient.Authenticate(Encoding.UTF8, email, password);
            mailFolder = imapClient.Inbox;

        }

        private static string GetDataFromField(HeaderList headerList, string field)
        {
            for (int i = 0; i < headerList.Count; i++)
                if (headerList[i].Field == field)
                    return headerList[i].Value;

            return "(Без темы)";
        }

        //
        public static int GetCountMessages()
        { if (TypeMessage == 0)
                mailFolder = imapClient.Inbox; //receiv
            else if (TypeMessage == 1)
                mailFolder = imapClient.GetFolder(SpecialFolder.Sent); //sen
            mailFolder.Open(FolderAccess.ReadOnly);
            return mailFolder.Count;
        }

        public static string GetMessageByIndex(int index)
        {
            mailFolder.Open(FolderAccess.ReadOnly);
            var data = mailFolder.GetHeaders(index);
            string message = $"<b>Тема:</b> {GetDataFromField(data, "Subject")}<br>" +
                             $"<b>От:</b> {GetDataFromField(data, "From")}<br>" +
                             $"<b>Кому:</b> {GetDataFromField(data, "To")}<br>" +
                             $"<b>Дата:</b> {GetDataFromField(data, "Date").Split('+')[0]}<br>" +
                             $"<br>{mailFolder.GetMessage(index).HtmlBody}";
            return message;
        }

        public static List<string> GetHeaders(int firstIndex, int lastIndex)
        {
            List<string> listOfMessages = new List<string>();
            //var inbox = imapClient.Inbox;
            if (TypeMessage == 0)
                mailFolder = imapClient.Inbox; //receiv
            else if (TypeMessage == 1)
                mailFolder = imapClient.GetFolder(SpecialFolder.Sent); //sen
            mailFolder.Open(FolderAccess.ReadOnly);
            for (int i = firstIndex; i < lastIndex; i++)
            {
                var headerList = mailFolder.GetHeaders(i);

                string headerString =
                    $"{i}" +
                    $"{GetDataFromField(headerList, "Subject")}      " +
                    $"{GetDataFromField(headerList, "From")}      " +
                    $"{GetDataFromField(headerList, "Date").Split('+')[0]}";
                listOfMessages.Add(headerString);
            }

            return listOfMessages;
        }

        public static void CloseConnection()
        {
            imapClient.Disconnect(true);
        }
    }
}