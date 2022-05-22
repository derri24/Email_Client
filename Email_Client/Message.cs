namespace Email_Client
{
    public class Message
    {
        public string Subject { get; set; }
        public string From { get; set; }
        public string Date { get; set; }
        public string Body { get; set; }
        
        public override string ToString()
        {
            return $"{Subject} {From} {Date}";
        }
        public Message(string subject, string from,string date)
        {
            Subject = subject;
            From = from;
            Date = date;
        }  
    }
}