using FullTextSearch.Core.Contracts;
using FullTextSearch.Core.Models;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver.GridFS;
using Nest;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using Tesseract;

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
                    string data = String.Empty;
                    string contentType = stream.FileInfo.Metadata
                        .FirstOrDefault(k => k.Name == "contentType")
                        .Value
                        .ToString()
                        .ToLower();

                    if (contentType.Contains("image"))
                    {
                        data = GetTextFromOcr(stream.FileInfo.Filename, buffer);
                    }
                    else
                    {
                        data = Convert.ToBase64String(buffer);
                    }
                    
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

        private string GetTextFromOcr(string filename, byte[] data)
        {
            string result = String.Empty;

            using (var file = new FileStream(filename, FileMode.Create))
            {
                file.Write(data, 0, data.Length);
                file.Flush();
                file.Close();
            }

            using (var engine = new TesseractEngine(@"./Languages", "bul", EngineMode.Default))
            {

                using (Pix img = Pix.LoadFromFile(filename))
                {
                    var page = engine.Process(img);
                    result = page.GetText();
                }
            }

            File.Delete(filename);

            return Convert.ToBase64String(Encoding.UTF8.GetBytes(result));
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
