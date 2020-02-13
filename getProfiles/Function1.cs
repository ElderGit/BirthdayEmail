using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using RestSharp;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace getProfiles
{
    public static class Function1
    {
        private static SendGridMessage message;

        [FunctionName("Function1")]
        public static void Run([TimerTrigger("0 */1 * * * *")]TimerInfo myTimer, ILogger log)
        {
            

            var client = new RestClient("https://app.xerpa.com.br/api/g");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Authorization", "Bearer fHOoWDl4LK1f0Nw5pvOnNTzbWxo_S6GQTPEOLa6wh-c=");
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", "{\"query\":\"query{company(id:\\\"4930\\\"){profile_search(query:\\\"page_size=3000&page=1&source=false&filter[status][]=active%2Cin-admission%2Coffboarding\\\"){profiles{id,name,username,birthday,status}}}}\",\"variables\":{\"companyId\":4930}}",
                       ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
           
            
           JObject json = JObject.Parse(response.Content);
           var profiles = json["data"]["company"]["profile_search"]["profiles"];

            var day = DateTime.Today.ToString("d");
            var daySplit = day.Split("/");
            var dayDate = daySplit[0] + "/" + daySplit[1];

            message = new SendGridMessage();
            message.AddTo("elder.barbosa.lima@gmail.com");
            message.AddContent("text/html", "<div>teste</div>");
            message.SetFrom(new EmailAddress("elder.lima@base2.com.br"));
            message.SetSubject("test");
           
            foreach (JObject pro in profiles)
            {
                var birthday = pro["birthday"] != null ? (string)pro["birthday"] : null;
                var birthdayDate = "";
                if (birthday != null)
                {
                    var birthdaySplit = birthday.Split("-");
                    birthdayDate = birthdaySplit[2] + "/" + birthdaySplit[1];
                }
                if (birthdayDate== dayDate)
                {
                    message = new SendGridMessage();
                    message.AddTo("elder.barbosa.lima@gmail.com");
                    message.AddContent("text/html", "<div>teste</div>");
                    message.SetFrom(new EmailAddress("elder.lima@base2.com.br"));
                    message.SetSubject("test");

                }

            }
           
        }
       
    }
}
