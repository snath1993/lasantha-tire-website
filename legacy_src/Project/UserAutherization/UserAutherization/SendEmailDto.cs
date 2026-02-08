namespace sendmailDTOs
{
    public class SendEmailDto
    {
        public SendEmailDto()
        {

        }
        public SendEmailDto(string receiver, string fileName, string attachmentUrl, string attachmentName)
        {
            Receiver = receiver;
            FileName = fileName;
            AttachmentUrl = attachmentUrl;
            AttachmentName = attachmentName;
        }
        public string Receiver { get; private set; }
        public string AttachmentUrl { get; private set; }
        public string FileName { get; private set; }
        public string AttachmentName { get; private set; }
    }
}
