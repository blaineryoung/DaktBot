using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daktbot.Common.Entities
{
    public record ChannelRaid : BaseEntity
    {
        public string ChannelId { get; init; }

        public string Day { get; init; }

        public string TimezoneId { get; init; }

        public string Time { get; init; }

        public string CreatorId { get; init; }
    }
}
