using FullTextSearch.Core.Contracts;
using FullTextSearch.Core.Models;
using FullTextSearch.Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FullTextSearch.Core.Services
{
    public class MongoCdnService : MongoDbCdnContext, IMongoCdnService
    {
        public MongoCdnService(IConfiguration config) : base(config)
        {

        }

        public async Task<string> UploadAsync(IFormFile file)
        {
            var options = new GridFSUploadOptions
            {
                Metadata = new BsonDocument("contentType", file.ContentType)
            };

            using (var reader = new StreamReader((Stream)file.OpenReadStream()))
            {
                var stream = reader.BaseStream;
                var fileId = await GridFsBucket.UploadFromStreamAsync(file.FileName, stream, options);

                return fileId.ToString();
            }
        }

        public async Task<bool> AnyAsync(ObjectId id)
        {
            var filter = Builders<GridFSFileInfo>.Filter.Eq("_id", id);

            return await GridFsBucket.Find(filter).AnyAsync();
        }

        public Task<bool> AnyAsync(string fileName)
        {
            var filter = Builders<GridFSFileInfo>.Filter.Where(x => x.Filename == fileName);

            return GridFsBucket.Find(filter).AnyAsync();
        }

        public async Task DeleteAsync(string fileName)
        {
            var fileInfo = await GetFileInfoAsync(fileName);

            if (fileInfo != null)
            {
                await DeleteAsync(fileInfo.Id);
            }
        }

        public async Task DeleteAsync(ObjectId id)
        {
            await GridFsBucket.DeleteAsync(id);
        }

        private async Task<GridFSFileInfo> GetFileInfoAsync(string fileName)
        {
            var filter = Builders<GridFSFileInfo>.Filter.Eq(x => x.Filename, fileName);
            var fileInfo = await GridFsBucket.Find(filter).FirstOrDefaultAsync();

            return fileInfo;
        }

        public async Task<GridFSDownloadStream<ObjectId>> DownloadAsync(ObjectId id)
        {
            return await GridFsBucket.OpenDownloadStreamAsync(id);
        }

        public async Task<GridFSDownloadStream<ObjectId>> DownloadAsync(string fileName)
        {
            return await GridFsBucket.OpenDownloadStreamByNameAsync(fileName);
        }

        public IEnumerable<MongoItemVM> GetAllFilesByContentType(string contentType, int skip, int take)
        {
            var filter = Builders<GridFSFileInfo>.Filter
                .Eq(info => info.Metadata, new BsonDocument(new BsonElement("contentType", contentType)));
            var options = new GridFSFindOptions
            {
                Limit = take,
                Skip = skip,
            };

            var stream = GridFsBucket.Find(filter, options)
                .ToList()
                .Select(s => new MongoItemVM
                {
                    Id = s.Id,
                    Filename = s.Filename,
                    MetaData = s.Metadata,
                    Length = s.Length + "",
                    UploadDateTime = s.UploadDateTime,
                }).ToList();

            return stream;
        }

        public IEnumerable<MongoItemVM> GetAllFiles(int skip, int take)
        {
            var options = new GridFSFindOptions
            {
                Limit = take,
                Skip = skip,
            };

            var stream = GridFsBucket
                .Find(new BsonDocumentFilterDefinition<GridFSFileInfo<ObjectId>>(new BsonDocument()), options)
                .ToList()
               .Select(s => new MongoItemVM
               {
                   Id = s.Id,
                   Filename = s.Filename,
                   MetaData = s.Metadata,
                   Length = s.Length + "",
                   UploadDateTime = s.UploadDateTime,
               }).ToList();

            return stream;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
