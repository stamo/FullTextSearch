using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FullTextSearch.Core.Contracts;
using FullTextSearch.Core.Models;
using FullTextSearch.Models;
using Microsoft.AspNetCore.Mvc;

namespace FullTextSearch.Controllers
{
    public class SearchController : Controller
    {
        private readonly ISearchService search;

        public SearchController(ISearchService _search)
        {
            search = _search;
        }

        public IActionResult Index()
        {
            var model = new List<DocumentViewModel>();
            var result = search.MatchAll<DocumentModel>();

            foreach (var document in result.Documents)
            {
                model.Add(new DocumentViewModel()
                {
                    Author = document.Attachment.Author,
                    Date = document.Attachment.Date,
                    FileName = document.FileName,
                    Text = document.Attachment.Content,
                    Title = document.Attachment.Title,
                    Url = document.Url
                });
            }

            return View(model);
        }

        public IActionResult Match(string query)
        {
            var model = new List<DocumentViewModel>();
            var result = search.Match(query);

            foreach (var document in result.Documents)
            {
                string[] highlights = result.Hits
                    .SelectMany(h => h.Highlights.Values)
                    .Where(v => v.DocumentId == document.Id)
                    .SelectMany(v => v.Highlights)
                    .ToArray();
 
                model.Add(new DocumentViewModel()
                {
                    Author = document.Attachment.Author,
                    Date = document.Attachment.Date,
                    FileName = document.FileName,
                    Text = String.Join("<br />", highlights),
                    Title = document.Attachment.Title,
                    Url = document.Url
                });
            }

            return View("index", model);
        }
    }
}