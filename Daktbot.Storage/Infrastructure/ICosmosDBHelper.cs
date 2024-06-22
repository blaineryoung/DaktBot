using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daktbot.Storage.Infrastructure
{
    internal interface ICosmosDBHelper
    {
        Container GetContainer(string databaseName, string containerName); 
    }
}
