using FullTextSearch.Core.Models;
using Nest;
using System;
using System.Collections.Generic;
using System.Text;

namespace FullTextSearch.Core.Contracts
{
    public interface ISearchService
    {
        ISearchResponse<T> MatchAll<T>() where T : class;

        ISearchResponse<DocumentModel> Match(string query);
    }
}
