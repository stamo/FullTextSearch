using FileProcessorService.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace FileProcessorService.Services
{
    public class ConsoleTaskRecieverService : IConsoleTaskRecieverService
    {
        public void RecieveMessage(string message)
        {
            Console.WriteLine(message);
        }
    }
}
