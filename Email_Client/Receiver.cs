using System;
using MailKit;
using MailKit.Net.Imap;

namespace Email_Client
{
    public class Receiver
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
            imapClient.Connect (host, port, true);
            imapClient.Authenticate (email, password);
           
        }

        public static void ReceiveMessages()
        {
            var inbox = imapClient.Inbox;
            inbox.Open (FolderAccess.ReadOnly);
                 
            for (int i = 0; i < inbox.Count; i++) {
                var message = inbox.GetMessage (i);
                Console.WriteLine ("Subject: {0}", message.Subject);
                Console.WriteLine ("Body: {0}", message.Body);
            }

            imapClient.Disconnect(true);

        }
    }
}