using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daktbot.Common.Entities
{
    public record ChannelPerson : BaseEntity
    {
        public string ChannelId { get; init; }

        public string UserName { get; init; }
        public string TimezoneId { get; init; }
        public string DisplayName { get; init; }

    }
}
