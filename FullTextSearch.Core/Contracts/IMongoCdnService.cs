using FullTextSearch.Core.Models;
using Microsoft.AspNetCore.Http;
using MongoDB.Bson;
using MongoDB.Driver.GridFS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FullTextSearch.Core.Contracts
{
    public interface IMongoCdnService
    {
        Task<string> UploadAsync(IFormFile file);
        Task<bool> AnyAsync(ObjectId id);
        Task<bool> AnyAsync(string fileName);
        Task DeleteAsync(string fileName);
        Task DeleteAsync(ObjectId id);
        Task<GridFSDownloadStream<ObjectId>> DownloadAsync(string fileName);
        Task<GridFSDownloadStream<ObjectId>> DownloadAsync(ObjectId id);
        IEnumerable<MongoItemVM> GetAllFilesByContentType(string contentType, int skip, int take);
        IEnumerable<MongoItemVM> GetAllFiles(int skip, int take);
        byte[] GetFile(ObjectId id);
    }
}
