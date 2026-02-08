using sendmail.Configurations;

namespace sendmail.DTOs
{
    public class SendSmsDto
    {
        public SendSmsDto()
        {

        }

        public SendSmsDto(string numbers, string content)
        {
            campaignName = SmsConfig.CampaignName;
            mask = SmsConfig.Mask;
            this.numbers = numbers;
            this.content = content;
        }

        public string campaignName { get; private set; }
        public string mask { get; private set; }
        public string numbers { get; private set; }
        public string content { get; private set; }
    }
}
