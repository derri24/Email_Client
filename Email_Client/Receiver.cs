using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MailKit;
using MailKit.Net.Imap;
using MimeKit;



namespace Email_Client
{
    public static class Receiver
    {
        private static ImapClient imapClient;

        private static string email;
        private static string password;
        private static string host;
        private static int port;

        //проверка на порт с защитой..
        public static void Authorization(string _host, int _port, string _email, string _password)
        {
            host = _host;
            port = _port;
            email = _email;
            password = _password;

            imapClient = new ImapClient();
            imapClient.Connect(host, port, true) ;
            imapClient.Authenticate( Encoding.UTF8,email, password);
        }

        private static string GetDataFromField(HeaderList headerList, string field)
        {
            for (int i = 0; i < headerList.Count; i++)
                if (headerList[i].Field == field)
                {
                    return headerList[i].Value;
                }

            return "";
        }

        public static int GetCountMessages()
        { 
            var inbox = imapClient.Inbox;
            inbox.Open(FolderAccess.ReadOnly);
            return inbox.Count;
        }


        public static string GetMessageBodyByIndex(int index)
        {
            var inbox = imapClient.Inbox;
            inbox.Open(FolderAccess.ReadOnly);
            var a = inbox.GetMessage(index);
           return  inbox.GetMessage(index).HtmlBody;
        }
        public static List<string> ReceiveHeaders(int firstIndex, int lastIndex)
        {
            List<string> listOfMessages = new List<string>();
            var inbox = imapClient.Inbox;
            inbox.Open(FolderAccess.ReadOnly);
            for (int i = firstIndex; i < lastIndex; i++)
            {
                var headerList = inbox.GetHeaders(i);

                string headerString =
                    $"{i}"+
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