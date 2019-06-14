using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FullTextSearch.Contracts
{
    public interface IMessageService
    {
        bool PublishMessage(string message);

        bool PublishMessage(byte[] message);
    }
}
