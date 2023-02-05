namespace GitHubApiTests
{
    public class Issue
    {
        public int number { get; set; }

        public string title { get; set; }
        public string body { get; set; }

        public long id { get; set; }


        public List<Label> labels { get; set; }

        public string state { get; set; }

    }
}