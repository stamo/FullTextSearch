using FullTextSearch.Core.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace FullTextSearch.Core.Services
{
    public class ConsoleTaskRecieverService : IConsoleTaskRecieverService
    {
        public void RecieveMessage(string message)
        {
            Console.WriteLine(message);
        }
    }
}
