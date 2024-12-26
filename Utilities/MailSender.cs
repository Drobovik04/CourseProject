using System;
using System.IO;
using RestSharp;
using RestSharp.Authenticators;

namespace CourseProject.Utilities
{
    public class MailSender
    {
        public static RestResponse SendMessage(string emailReceiver, string filePath)
        {
            RestClientOptions options = new RestClientOptions("https://api.mailgun.net/v3")
            {
                Authenticator = new HttpBasicAuthenticator("api", "0861e3a008b27be980a2b985d4787756-2e68d0fb-3afd8e8e")
            };
            RestClient client = new RestClient(options);

            RestRequest request = new RestRequest();
            request.AddParameter("domain", "sandboxc20f2e0511a6418b8c32c9ecc47ccb8d.mailgun.org", ParameterType.UrlSegment);
            request.Resource = "{domain}/messages";
            request.AddParameter("from", "Excited User <mailgun@sandboxc20f2e0511a6418b8c32c9ecc47ccb8d.mailgun.org>");
            request.AddParameter("to", emailReceiver);
            request.AddParameter("subject", "Your answers");
            request.AddFile("attachment", filePath);
            request.Method = Method.Post;
            return client.Execute(request);
        }
    }
}
