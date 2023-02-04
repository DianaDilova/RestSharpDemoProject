using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Reflection.Emit;
using System.Reflection.PortableExecutable;
using System.Text.Json;

namespace RestSharpDemoProject
{
    internal class Program
    {
        static void Main(string[] args)
        {
            RestClient client = new RestClient("https://api.github.com");

            client.Authenticator = new HttpBasicAuthenticator("dianadilova", "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");

            // Create Issue
            //  RestRequest request = new RestRequest("/repos/{user}/{repoName}/issues", Method.Post);

            //   var issueBoddy = new
            //  {
            // title = "Test issue from RestSharp " + DateTime.Now.Ticks,
            // body = "some body for my issue",
            // labels = new string[] { "bug" , "critical", "release"}
            //   };

            //   request.AddBody(issueBoddy);


            //   request.AddUrlSegment("user", "dianadilova");
            // request.AddUrlSegment("repoName", "postman");



            // var response = client.Execute(request);


            //var issue = JsonSerializer.Deserialize<Issue>(response.Content);

            //Console.WriteLine("Issue title: " + issue.title);
            //Console.WriteLine("Issue number: " + issue.number);

            //Get First Issue
            RestRequest request = new RestRequest("/repos/{user}/{repoName}/issues/{id}", Method.Get);

            request.AddUrlSegment("user", "DianaDilova");
            request.AddUrlSegment("id", "1");
            request.AddUrlSegment("repoName", "Postman");

            var response = client.Execute(request);

            Console.WriteLine("Status code: " + response.StatusCode);

            var issue = JsonSerializer.Deserialize<Issue>(response.Content);

            Console.WriteLine("Issue name: " + issue.title);
            Console.WriteLine("Issue number: " + issue.number);
            Console.WriteLine();

            // Issues Labels
            RestRequest request1 = new RestRequest("/repos/{user}/{repoName}/issues/{id}/labels", Method.Get);
            request1.AddUrlSegment("user", "DianaDilova");
            request1.AddUrlSegment("repoName", "Postman");
            request1.AddUrlSegment("id", "1");

            var response1 = client.Execute(request1);

            var labels = JsonSerializer.Deserialize<List<Labels>>(response1.Content);

            foreach (var label in labels)
            {
                Console.WriteLine("Label ID: " + label.id);
                Console.WriteLine("Label name: " + label.name);
                Console.WriteLine();

            }
            //Requesting all repos

            var request2 = new RestRequest("/users/{user}/repos", Method.Get);
            request2.AddUrlSegment("user", "DianaDilova");

            var response2 = client.Execute(request2);
            Console.WriteLine("Status code for get all repos: " + response2.StatusCode);

            var repos = JsonSerializer.Deserialize<List<Repo>>(response2.Content);

            foreach (var rep in repos)
            {
                Console.WriteLine("Repo name: " + rep.name);
                Console.WriteLine("Repo ID: " + rep.id);
                Console.WriteLine();

            }
           
            
        }
    }
}