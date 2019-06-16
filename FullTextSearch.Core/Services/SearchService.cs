using FullTextSearch.Core.Contracts;
using FullTextSearch.Core.Models;
using Microsoft.Extensions.Configuration;
using Nest;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace FullTextSearch.Core.Services
{
    public class SearchService : ISearchService
    {
        private readonly ElasticClient client;

        public SearchService(IConfiguration config)
        {
            string uri = config.GetValue<string>("ElasticSearchURI");
            var settings = new ConnectionSettings(new Uri(uri))
                .DefaultIndex("files");
            client = new ElasticClient(settings);
        }
        public ISearchResponse<T> MatchAll<T>() where T : class
        {
            return client
                .Search<T>(s => 
                    s.Query(q => 
                        q.MatchAll()));
        }

        public ISearchResponse<DocumentModel> Match(string query)
        {
            return client
                .Search<DocumentModel>(s =>
                    s.Query(q =>
                        q.Match(c => c
                            .Field(p => p.Attachment.Content)
                            .Analyzer("standard")
                            .Boost(1.1)
                            .CutoffFrequency(0.001)
                            .Query(query)
                            .Fuzziness(Fuzziness.AutoLength(3, 6))
                            .Lenient()
                            .FuzzyTranspositions()
                            .MinimumShouldMatch(2)
                            .Operator(Operator.Or)
                            .FuzzyRewrite(MultiTermQueryRewrite.TopTermsBlendedFreqs(10))
                            .Name("match_document_content_query")
                         )
                    )
                    .Highlight(h => h
                        .PreTags("<mark>")
                        .PostTags("</mark>")
                        .Encoder(HighlighterEncoder.Html)
                        .Fields(
                            fs => fs
                                .Field(p => p.Attachment.Content)
                                .Type("plain")
                                .ForceSource()
                                .FragmentSize(150)
                                .Fragmenter(HighlighterFragmenter.Span)
                                .NumberOfFragments(3)
                                .NoMatchSize(150)
                        )
                    )
                );
        }
    }
}
