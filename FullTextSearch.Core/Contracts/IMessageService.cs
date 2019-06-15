using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FullTextSearch.Core.Contracts
{
    public interface IMessageService
    {
        void PublishMessage(string message);

        void PublishMessage(byte[] message);
    }
}
