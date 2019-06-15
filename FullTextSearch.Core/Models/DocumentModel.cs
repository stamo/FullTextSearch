using Nest;
using System;
using System.Collections.Generic;
using System.Text;

namespace FullTextSearch.Core.Models
{
    public class DocumentModel
    {
        public string Id { get; set; }

        public string Content { get; set; }

        public string FileName { get; set; }

        public string Url { get; set; }

        public Attachment Attachment { get; set; }
    }
}
