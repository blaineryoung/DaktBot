using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daktbot.Storage.Infrastructure
{
    [AttributeUsage(System.AttributeTargets.Class)]
    internal class StorageContainerAttribute : Attribute
    {
        public string DatabaseName { get; init; } = Constants.DEFAULT_DATABASE_NAME;

        public string? PartitionKey { get; init; } = "id";

        internal string ContainerName { get; init; }

        public StorageContainerAttribute(string containerName) 
        {
            ContainerName = containerName;
        }
    }
}
