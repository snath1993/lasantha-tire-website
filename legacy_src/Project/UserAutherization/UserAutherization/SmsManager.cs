
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using sendmail.Configurations;
using sendmail.DTOs;
using Newtonsoft.Json;





namespace sendmail.CommunicationManager
{
    public class SmsManager
    {
        private string _accessToken;

        public async Task SendSms(SendSmsDto sms)
        {
            try
            {
                if (_accessToken == null)
                {
                    await AuthenticateAndRetry(sms);
                }
                else
                {
                    var httpclient = new HttpClient();
                    httpclient.DefaultRequestHeaders.Accept.Clear();

                    var json = JsonConvert.SerializeObject(sms);
                    var data = new StringContent(json, Encoding.UTF8, "application/json");

                    var headers = httpclient.DefaultRequestHeaders;
                    headers.Add("X-API-VERSION", "v1");
                    headers.Add("Authorization", "Bearer " + _accessToken);

                    var response = await httpclient.PostAsync(SmsConfig.SmsUrl, data);
                    if (response.IsSuccessStatusCode)
                    {
                        var result = response.Content.ReadAsStringAsync().Result;
                        var reference = JsonConvert.DeserializeObject<SmsServerRefDto>(result);
                    }
                    else
                    {
                        await AuthenticateAndRetry(sms);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task Authenticate()
        {
            var user = new SmsLoginDto
            {
                username = SmsConfig.Username,
                password = SmsConfig.Password
            };

            try
            {
                var httpclient = new HttpClient();
                httpclient.DefaultRequestHeaders.Accept.Clear();

                var json = JsonConvert.SerializeObject(user);
                var data = new StringContent(json, Encoding.UTF8, "application/json");

                var headers = httpclient.DefaultRequestHeaders;
                headers.Add("X-API-VERSION", "v1");

                var response = await httpclient.PostAsync(SmsConfig.LoginUrl, data);
                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    var token = JsonConvert.DeserializeObject<SmsAuthTokenResponse>(result);
                    _accessToken = token.AccessToken;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task AuthenticateAndRetry(SendSmsDto sms)
        {
            await Authenticate();
            await SendSms(sms);
        }
    }
}
