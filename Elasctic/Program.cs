using Elasticsearch.Net;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Elasctic
{
    class Program
    {
        private static ElasticClient _client = null;
        //private static ElasticsearchClient _nativeClient = null;
        internal static bool IsLoggingEnabled = false;
        static void Main(string[] args)
        {
            string term = "cek";
            NativeSuggest(term);
        }
        static void NativeSuggest(string term, int catId = 0)
        {

            string index = "suggest";
            var response2 = Client.Search<Assignment>(s => s
            .Index(index)
            .Suggest(
                su => su
            .Completion("suggest", cs => cs
                .Field(f => f.Suggest)
                .Prefix(term)
                //elastic kendi beceresine göre benzerlik göstereni getiyor ona göre score veriyor.
                //.Fuzzy(f => f
                //    .Fuzziness(Fuzziness.Auto)
                //)
                .Size(10)
                .Analyzer("simple")
            )
        )
    );
            var suggestions =
                from suggest in response2.Suggest["suggest"]
                from option in suggest.Options
                select new
                {
                    Id = option.Id,
                    Text = option.Source.Suggest.Input,
                    Score = option.Score,
                    SearchTerm = option.Text,
                    Context = option.Contexts
                };

            if (true) { }
        }
        public static ElasticClient Client
        {
            get
            {
                if (_client == null)
                {
                    lock (new Object())
                    {
                        if (_client == null)
                        {
                            List<Uri> nodes = new List<Uri>();
                            nodes.Add(new Uri("http://192.168.77.7:9200"));
                            var pool = new SniffingConnectionPool(nodes);
                            ConnectionSettings settings = null;
                            settings = new ConnectionSettings(new Uri("http://192.168.77.7:9200"));
                            _client = new ElasticClient(settings);
                        }
                    }
                }
                return _client;
            }
        }
        public class Assignment
        {
            public int Id { get; set; }
            public decimal Text { get; set; }
            public string Name { get; set; }

            public CompletionField Suggest { get; set; }
        }
    }
}
