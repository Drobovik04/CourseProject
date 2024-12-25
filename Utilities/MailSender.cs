using System;
using System.IO;
using RestSharp;
using RestSharp.Authenticators;

namespace CourseProject.Utilities
{
    public class MailSender
    {
        public static RestResponse SendSimpleMessage(string emailReceiver)
        {
            RestClientOptions options = new RestClientOptions("https://api.mailgun.net/v3")
            {
                Authenticator = new HttpBasicAuthenticator("api", "0c2f743f4d037118a99ebed8f25b7687-2e68d0fb-0ec5c06b")
            };
            RestClient client = new RestClient(options);

            RestRequest request = new RestRequest();
            request.AddParameter("domain", "sandbox60ac654b6562444c835c8749fd089198.mailgun.org", ParameterType.UrlSegment);
            request.Resource = "{domain}/messages";
            request.AddParameter("from", "Excited User <mailgun@sandbox60ac654b6562444c835c8749fd089198.mailgun.org>");
            request.AddParameter("to", emailReceiver);
            request.AddParameter("to", "YOU@sandbox60ac654b6562444c835c8749fd089198.mailgun.org");
            request.AddParameter("subject", "Hello");
            request.AddParameter("text", "Testing some Mailgun awesomeness!");
            request.Method = Method.Post;
            return client.Execute(request);
        }
    }
}
