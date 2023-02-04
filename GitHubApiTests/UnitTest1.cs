using RestSharp;
using RestSharp.Authenticators;
using System.Net;
using System.Text.Json;

namespace GitHubApiTests
{
    public class ApiTests
    {
        private RestClient client;
        private const string baseurl = "https://api.github.com";
        private const string partialUrl = "repos/dianadilova/postman/issues";
        private const string username = "dianadilova";
        private const string password = "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";

        [SetUp]

        public void Setup()
        {
            this.client = new RestClient(baseurl);
            this.client.Authenticator = new HttpBasicAuthenticator(username, password);
        }



        [Test]
        public void Test_Get_Single_Issue()
        {
            var request = new RestRequest($"{partialUrl}/2", Method.Get);
            var response = this.client.Execute(request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var issue = JsonSerializer.Deserialize<Issue>(response.Content);

            Assert.That(issue.title, Is.EqualTo("Second Issue"));
            Assert.That(issue.number, Is.EqualTo(2));
        }

        [Test]
        public void Test_Get_Single_IssueWithLabels()
        {
            var request = new RestRequest($"{partialUrl}/1", Method.Get);
            var response = this.client.Execute(request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var issue = JsonSerializer.Deserialize<Issue>(response.Content);

            for (int i = 0; i < issue.labels.Count; i++)
            {
                Assert.That(issue.labels[i].name, Is.Not.Null);
            }
       
        }

        [Test]
        public void Test_Get_All_Issue()
        {
            var request = new RestRequest(partialUrl, Method.Get);
            var response = this.client.Execute(request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var issues = JsonSerializer.Deserialize<List<Issue>>(response.Content);

            foreach (var issue in issues)
            {
                Assert.That(issue.title, Is.Not.Empty);
                Assert.That(issue.number, Is.GreaterThan(0));
            }

        }

        [Test]

        public void Test_CreateNewIssue()
        {
            var request = new RestRequest(partialUrl, Method.Post);
            var issueBody = new
            {
                title = "RestSharp api test" + DateTime.Now.Ticks,
                body = "some body for my issue",
                labels = new string[] { "bug", "critical", "release" }

            };

            request.AddBody(issueBody);

            //Act 

            var response = this.client.Execute(request);
            var issue = JsonSerializer.Deserialize<Issue>(response.Content);

            //Assert

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
            Assert.That(issue.number, Is.GreaterThan(0));
            Assert.That(issue.title, Is.EqualTo(issueBody.title));
            Assert.That(issue.body, Is.EqualTo(issueBody.body));

        }

        [Test]
        public void Test_EditIssue()
        {

            var issueBody = new
            {
                title = "EDITED: Test issue from RestSharp" + DateTime.Now.Ticks,

            };

            var issue = EditIssue(issueBody);

            Assert.That(issue.number, Is.GreaterThan(0));
            Assert.That(issue.title, Is.EqualTo(issueBody.title));
        }
            

        private Issue EditIssue(object body)
        {
            var request = new RestRequest($"{partialUrl}/1", Method.Patch);
            request.AddBody(body);

            var response = this.client.Execute(request);
            var issue = JsonSerializer.Deserialize<Issue>(response.Content);

            return issue;

        }

        [TestCase("US","90210","United States")]
        public void Test_Zippopotamus_DD(string countryCode, string zipCode, string expectedCountry)
        {
            var restClient = new RestClient("http://api.zippopotam.us");
            var request = new RestRequest(countryCode + "/" + zipCode, Method.Get);

            var response = restClient.Execute(request);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "HTTP Status Code property");

            var location = JsonSerializer.Deserialize<Location>(response.Content);

            Assert.That(location.country, Is.EqualTo(expectedCountry));
        }

        // added by me 

        [Test]

        public void Test_GetIssueByValidNumber()
        {
            var request = new RestRequest($"{partialUrl}/2", Method.Get);
            var response = this.client.Execute(request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            var issue = JsonSerializer.Deserialize<Issue>(response.Content);


            Assert.That(issue.number, Is.GreaterThan(0));
            Assert.That(issue.id, Is.GreaterThan(0));
        }

        [Test]

        public void Test_GetIssueByInvalidNumber()
        {
            var request = new RestRequest($"{partialUrl}/6316334126", Method.Get);
            var response = this.client.Execute(request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        public void Test_CreateIssue_Unauthenticated()
        {
            var unauthenticatedClient = new RestClient(baseurl);
            var request = new RestRequest(partialUrl, Method.Post);
            var issueBody = new
            {
                title = "RestSharp api test" + DateTime.Now.Ticks,
                body = "some body for my issue",
                labels = new string[] { "bug", "critical", "release" }
            };

            request.AddBody(issueBody);

            var response = unauthenticatedClient.Execute(request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound), "Expected response status code to be 404 Not Found");
        }

        [Test]
        public void Test_CreateIssue_WithInvalidBody_MissingTitle()
        {
            var request = new RestRequest(partialUrl, Method.Post);
            var issueBody = new
            {
                body = "some body for my issue",
                labels = new string[] { "bug", "critical", "release" }
            };

            request.AddBody(issueBody);

            var response = this.client.Execute(request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.UnprocessableEntity));
        }
    }
}