using Daktbot.Common.Entities;
using Daktbot.Common.Results;
using Daktbot.Common.Services;
using Daktbot.Discord.Core.Client;
using Discord;
using Discord.Net;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daktbot.Discord.Core.Commands
{
    internal class UpdateMe : AbstractDiscordCommand
    {
        string[] validTimezones =
        {
            "(UTC-08:00) Pacific Time (US & Canada)",
            "(UTC-07:00) Arizona",
            "(UTC-07:00) Mountain Time (US & Canada)",
            "(UTC-07:00) Yukon",
            "(UTC-06:00) Central Time (US & Canada)",
            "(UTC-06:00) Easter Island",
            "(UTC-06:00) Guadalajara, Mexico City, Monterrey",
            "(UTC-06:00) Saskatchewan",
            "(UTC-05:00) Eastern Time (US & Canada)",
            "(UTC-05:00) Indiana (East)",
            "(UTC+08:00) Beijing, Chongqing, Hong Kong, Urumqi",
            "(UTC+08:00) Perth",
            "(UTC+08:00) Taipei",
            "(UTC+09:00) Osaka, Sapporo, Tokyo",
            "(UTC+09:00) Pyongyang",
            "(UTC+09:00) Seoul",
            "(UTC+09:30) Darwin",
            "(UTC+10:00) Brisbane",
            "(UTC+10:00) Canberra, Melbourne, Sydney",
            "(UTC+10:00) Guam, Port Moresby",
            "(UTC+10:30) Lord Howe Island",
            "(UTC+11:00) Bougainville Island",
            "(UTC+11:00) Norfolk Island",
            "(UTC+11:00) Solomon Is., New Caledonia",
        };

        internal override ILogger Logger { get; }

        TimeZoneInfo[] timeZones;
        private readonly IPersonService personService;

        internal override string Name => "update-me";

        internal override string Description => "Updates your timezone and other discord information";

        public UpdateMe(
            IPersonService personService,
            ILogger<TestCommand> logger)
        {
            this.timeZones = TimeZoneInfo.GetSystemTimeZones().Where(x => validTimezones.Contains(x.DisplayName)).ToArray();
            this.personService = personService;
            Logger = logger;
        }

        internal override async Task HandleCommand(SocketSlashCommand command)
        {
            Int64? timezoneSelection = command.Data.Options.FirstOrDefault().Value as Int64?;
            if (timezoneSelection == null ||
                command.ChannelId == null
                ) 
            {
                Logger.LogError("Received command with invalid timezone selection");
                return;
            }

            Result<ChannelPerson, RequestError> result = await this.personService.UpsertPerson(new ChannelPerson()
            {
                Id = command.User.Id.ToString(),
                ChannelId = command.ChannelId.Value.ToString(),
                UserName = command.User.Username,
                DisplayName = command.User.GlobalName,
                TimezoneId = timeZones[timezoneSelection.Value].Id
            });

            ChannelPerson returnedPerson;
            if (false == result.Match<bool>(
                m => { returnedPerson = m; return true; },
                failed => { Logger.LogRequestError(failed, "Could not update information for {userid}", command.User.Id); return false; }))
            {
                await command.RespondAsync($"Hello, {command.User.GlobalName}.  We couldn't update your information for reason.  Please bug Dakt");
            }
            else
            {
                await command.RespondAsync($"Hello, {command.User.GlobalName}.  You picked timezone {timeZones[timezoneSelection.Value].DisplayName}");
            }
        }

        internal override async Task Register(IDiscordBotClient client, SocketGuild guild)
        {
            using (Logger.BeginScope("Registering command {command}", Name))
            {
                SlashCommandBuilder command = new SlashCommandBuilder();
                command.WithName(Name.ToLower());
                command.WithDescription(Description);

                SlashCommandOptionBuilder options = new SlashCommandOptionBuilder();
                options = options.WithName("timezone").WithDescription("Your time zone").WithRequired(true).WithType(ApplicationCommandOptionType.Integer);
                for (int i = 0; i < this.timeZones.Length; i++) 
                {
                    options = options.AddChoice(timeZones[i].DisplayName, i);
                }
                command.AddOption(options);

                try
                {
                    await guild.CreateApplicationCommandAsync(command.Build());
                    //await client.Client.CreateGlobalApplicationCommandAsync(command.Build());
                }
                catch (ApplicationCommandException e)
                {
                    Logger.LogError(e, "Could not register command {command}", Name);
                }
            }
        }
    }
}
