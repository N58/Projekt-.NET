using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using PortalKulinarny.Configuration;

namespace PortalKulinarny.Services
{
    public class MessageService
    {
        private readonly EmailSettings _emailSettings;

        public MessageService(IOptions<EmailSettings> emailOptions)
        {
            _emailSettings = emailOptions.Value;
        }
        public async Task SendEmailAsync(String email, String subject, String message)
        {
            using (var client = new HttpClient
            {
                BaseAddress = new Uri(_emailSettings.ApiBaseUri)
            })
            {
                client.DefaultRequestHeaders.Authorization =
                  new AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(Encoding.ASCII.GetBytes(_emailSettings.ApiKey)));

                var content = new FormUrlEncodedContent(new[]
                {
                        new KeyValuePair<string, string>("from", _emailSettings.From),
                        new KeyValuePair<string, string>("to", email),
                        new KeyValuePair<string, string>("subject", subject),
                        new KeyValuePair<string, string>("html", message)
                    });
                await client.PostAsync(_emailSettings.RequestUri, content).ConfigureAwait(false);
            }
        }
    }
}
