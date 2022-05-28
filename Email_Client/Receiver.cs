using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using MimeKit;


namespace Email_Client
{
    public static class Receiver
    {
        public static MessageType TypeMessage { get; set; }
        public static bool IsSearch { get; set; }

        private static ImapClient _imapClient;
        private static IMailFolder _mailFolder;

        private static string _email;
        private static string _password;
        private static string _host;
        private static int _port;
        private static bool _ssl;


        public static async Task Authorization(string host, int port, string email, string password, bool ssl)
        {
            await Task.Run(() =>
            {
                _host = host;
                _port = port;
                _email = email;
                _password = password;
                _ssl = ssl;

                _imapClient = new ImapClient();
                _imapClient.Connect(host, port, ssl);
                _imapClient.Authenticate(Encoding.UTF8, email, password);

                _mailFolder = _imapClient.Inbox;
            });
        }

        private static string GetDataFromField(HeaderList headerList, string field)
        {
            for (int i = 0; i < headerList.Count; i++)
                if (headerList[i].Field == field)
                    return headerList[i].Value;

            return "(Без темы)";
        }
        
        public static async Task Update()
        {
           await CloseConnection();
            await Authorization(_host, _port, _email, _password, _ssl);
            if (TypeMessage == MessageType.Received)
                _mailFolder = _imapClient.Inbox;
            else
                _mailFolder = _imapClient.GetFolder(SpecialFolder.Sent);
        }
        
        public static async Task<int> GetCountMessages()
        {
            await Task.Run(() =>
            {
                if (TypeMessage == MessageType.Received)
                    _mailFolder = _imapClient.Inbox;
                else if (TypeMessage == MessageType.Sent)
                    _mailFolder = _imapClient.GetFolder(SpecialFolder.Sent);
                _mailFolder.Open(FolderAccess.ReadOnly);
            });
            return _mailFolder.Count;
        }

        public static IEnumerable<MimeEntity> GetAttachments(int index)
        {
            _mailFolder.Open(FolderAccess.ReadOnly);
            return _mailFolder.GetMessage(index).Attachments;
        }

        public static string GetMessageByIndex(int index)
        {
            string message = "";
           
                _mailFolder.Open(FolderAccess.ReadOnly);

                HeaderList data;
                string bodyString;
                if (IsSearch)
                {
                    data = _mailFolder.GetHeaders(listOfUniqueIds[index]);
                    bodyString = _mailFolder.GetMessage(listOfUniqueIds[index]).HtmlBody;
                }
                else
                {
                    data = _mailFolder.GetHeaders(index);
                    bodyString = _mailFolder.GetMessage(index).HtmlBody;
                }

                message = $"<b>Тема:</b> {GetDataFromField(data, "Subject")}<br>" +
                          $"<b>От:</b> {GetDataFromField(data, "From")}<br>" +
                          $"<b>Кому:</b> {GetDataFromField(data, "To")}<br>" +
                          $"<b>Дата:</b> {GetDataFromField(data, "Date").Split('+')[0]}<br>" +
                          $"<br>{bodyString}";
        
            return message;
        }


        public static async Task<List<string>> GetHeaders(int firstIndex, int lastIndex)
        {
            List<string> listOfMessages = new List<string>();
            await Task.Run(() =>
            {
                if (TypeMessage == MessageType.Received)
                    _mailFolder = _imapClient.Inbox;
                else if (TypeMessage == MessageType.Sent)
                    _mailFolder = _imapClient.GetFolder(SpecialFolder.Sent);
                _mailFolder.Open(FolderAccess.ReadOnly);
                for (int i = firstIndex; i < lastIndex; i++)
                {
                    var headerList = _mailFolder.GetHeaders(i);
                    listOfMessages.Add(GetHeaderString(headerList, i));
                }
            });
            return listOfMessages;
        }

        private static string GetHeaderString(HeaderList headerList, int i)
        {
            return 
                   $"{GetDataFromField(headerList, "Subject")}      " +
                   $"{GetDataFromField(headerList, "From")}      " +
                   $"{GetDataFromField(headerList, "Date").Split('+')[0]}";
        }

        public static void AppendSentMessage(MimeMessage message)
        {
            _imapClient.GetFolder(SpecialFolder.Sent).Append(new AppendRequest(message));
        }
        private static IList<UniqueId> listOfUniqueIds;

        public static async Task<List<string>> GetFoundHeaders(int firstIndex, int lastIndex, string searchString)
        {
            List<string> listOfMessages = new List<string>();
            await Task.Run(() =>
            {
                _mailFolder.Open(FolderAccess.ReadOnly);
                listOfUniqueIds = _mailFolder.Search(SearchQuery.FromContains(searchString)
                    .Or(SearchQuery.SubjectContains(searchString)));

                for (int i = firstIndex; i < lastIndex; i++)
                {
                    var headerList = _mailFolder.GetHeaders(listOfUniqueIds[i]);
                    listOfMessages.Add(GetHeaderString(headerList, i));
                }
            });
            return listOfMessages;
        }

        public static async Task<int> GetCountFoundMessages(string searchString)
        {
            await Task.Run(() =>
            {
                listOfUniqueIds =
                    _mailFolder.Search(SearchQuery.FromContains(searchString)
                        .Or(SearchQuery.SubjectContains(searchString)));
            });
            return listOfUniqueIds.Count;
        }

        public static async Task CloseConnection()
        {
            await Task.Run(() =>
            {
                if (_imapClient != null)
                    _imapClient.Disconnect(true);
            });
        }
    }
}