using FullTextSearch.Core.Contracts;
using FullTextSearch.Core.Models;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using Nest;
using System;
using System.Collections.Generic;
using System.Text;

namespace FullTextSearch.Core.Services
{
    public class IndexService : IIndexService
    {
        private readonly IMongoCdnService cdnService;

        private readonly IConfiguration config;

        public IndexService(
            IMongoCdnService _cdnService,
            IConfiguration _config)
        {
            cdnService = _cdnService;
            config = _config;
        }

        public void IngestAttachment(string id)
        {
            ObjectId fileId;

            if (ObjectId.TryParse(id, out fileId))
            {
                DocumentModel model = null;

                using (var stream = cdnService.DownloadAsync(fileId).Result)
                {
                    byte[] buffer = new byte[stream.Length];
                    stream.Read(buffer, 0, buffer.Length);
                    string data = Convert.ToBase64String(buffer);
                    string downloadUrl = config.GetValue<string>("DownloadUrl");

                    model = new DocumentModel()
                    {
                        Content = data,
                        FileName = stream.FileInfo.Filename,
                        Id = id,
                        Url = downloadUrl + id
                    };
                }
                
                SendToElasticSearch(model);
            }
            else
            {
                throw new ArgumentException("Invalid cdn id");
            }
        }

        private void SendToElasticSearch(DocumentModel model)
        {
            string uri = config.GetValue<string>("ElasticSearchURI");
            var settings = new ConnectionSettings(new Uri(uri))
                .DefaultIndex("files");
            var client = new ElasticClient(settings);

            client.PutPipeline("attachments", p => p
                .Description("Document attachment pipeline")
                .Processors(pr => pr
                    .Attachment<DocumentModel>(a => a
                        .Field(f => f.Content)
                        .TargetField(f => f.Attachment)
                    )
                    .Remove<DocumentModel>(r => r
                        .Field(f => f.Content)
                    )
                )
            );

            var responce = client.Index(model, i => i.Pipeline("attachments"));
        }
    }
}
