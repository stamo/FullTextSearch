using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FullTextSearch.Core.Models
{
    public class MongoItemVM
    {
        public ObjectId Id { get; set; }
        public string Filename { get; set; }
        public BsonDocument MetaData { get; set; }
        public string Length { get; set; }
        public DateTime UploadDateTime { get; set; }
    }
}
