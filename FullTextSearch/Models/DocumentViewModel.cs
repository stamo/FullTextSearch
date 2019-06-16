using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FullTextSearch.Models
{
    public class DocumentViewModel
    {
        public string FileName { get; set; }

        public string Title { get; set; }

        public string Url { get; set; }

        public string Author { get; set; }

        public DateTime? Date { get; set; }

        public string Text { get; set; }
    }
}
