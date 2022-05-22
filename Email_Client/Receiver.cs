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
        private static ImapClient imapClient;

        private static string email;
        private static string password;
        private static string host;
        private static int port;
        
        public static void Authorization(string _host, int _port, string _email, string _password,bool _Ssl)
        {
            host = _host;
            port = _port;
            email = _email;
            password = _password;

            imapClient = new ImapClient();
            imapClient.Connect(host, port, _Ssl) ;
            imapClient.Authenticate( Encoding.UTF8,email, password);
        }

        private static string GetDataFromField(HeaderList headerList, string field)
        {
            for (int i = 0; i < headerList.Count; i++)
                if (headerList[i].Field == field)
                    return headerList[i].Value;

            return "(Без темы)";
        }

        public static int GetCountMessages()
        { 
            var inbox = imapClient.Inbox;
            inbox.Open(FolderAccess.ReadOnly);
            return inbox.Count;
        }
        public static string GetMessageByIndex(int index)
        {
            var inbox = imapClient.Inbox;
            inbox.Open(FolderAccess.ReadOnly);
            var data = inbox.GetHeaders(index);
            string message = $"Тема: {GetDataFromField(data, "Subject")}<br>"+
                             $"{GetDataFromField(data, "From")}  "+
                             $"{GetDataFromField(data, "Date").Split('+')[0]}<br>" +
                             $"<br>{inbox.GetMessage(index).HtmlBody}";
            return  message;
        }
        public static  List<string>  ReceiveHeaders(int firstIndex, int lastIndex)
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
        
        
        public static List<string>  jhgf()
        {
            List<string> listOfMessages = new List<string>();
           
            var inbox = imapClient.GetFolder(SpecialFolder.Sent);
            inbox.Open(FolderAccess.ReadOnly);
            for (int i = 1; i < 3; i++)
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