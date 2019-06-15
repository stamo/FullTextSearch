using FullTextSearch.Core.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace FullTextSearch.Core.Services
{
    public class ConsoleTaskRecieverService : IConsoleTaskRecieverService
    {
        private readonly IIndexService indexService;

        public ConsoleTaskRecieverService(IIndexService _indexService)
        {
            indexService = _indexService;
        }
        public void RecieveMessage(string message)
        {
            indexService.IngestAttachment(message);
        }
    }
}
