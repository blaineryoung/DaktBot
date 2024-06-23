using Daktbot.Common.Services;
using Daktbot.Common.Stores;
using Daktbot.Discord.Core.Client;
using Discord.Net;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Daktbot.Common.Results;
using Daktbot.Common.Entities;
using Daktbot.Common.Utilities;
using Microsoft.Extensions.Options;

namespace Daktbot.Discord.Core.Commands
{
    internal class AddRaid : AbstractDiscordCommand
    {
        private readonly IRaidService raidService;
        private readonly IPersonService personService;

        private const string DAY_OPTION = "day";
        private const string TIME_OPTION = "time";

        internal override ILogger Logger { get; }

        internal override string Name => "add-raid";

        internal override string Description => "Adds a raid to the channel's schedule.";

        public AddRaid(
            IRaidService raidService,
            IPersonService personService,
            ILogger<TestCommand> logger)
        {
            this.raidService = raidService;
            this.personService = personService;
            Logger = logger;
        }

        internal override async Task HandleCommand(SocketSlashCommand command)
        {
            using (Logger.BeginScope("Handling add raid command for user {user}", command.User.GlobalName))
            {
                ChannelPerson caller = null;
                Result<ChannelPerson, RequestError> getPersonResult = await personService.GetPersonForChannel(command.Channel.Id, command.User.Id);
                if (false == getPersonResult.Match<bool>(
                    c => { caller = c; return true; },
                    error => { Logger.LogRequestError(error, "Could not get information for caller"); return false; }))
                {
                    await command.RespondAsync("We couldn't add the raid.  Please bug Dakt");
                    return;
                }

                Int64? optionValue = command.Data.Options.FirstOrDefault(x => string.Equals(DAY_OPTION, x.Name)).Value as Int64?;

                DayOfWeek? requestedDay = Enum.Parse(typeof(DayOfWeek), optionValue.Value.ToString()) as DayOfWeek?;
                string requestedTime = command.Data.Options.FirstOrDefault(x => string.Equals(TIME_OPTION, x.Name)).Value as string;

                if (!TimeUtilities.ContainsTime(requestedTime))
                {
                    Logger.LogWarning("Could not parse provided time {request}", requestedTime);
                    await command.RespondAsync($"Test successful!  You executed {command.Data.Name}");
                    return;
                }

                Result<ChannelRaid, RequestError> upsertRaidResult = await raidService.UpsertRaid(new ChannelRaid()
                {
                    Id = IdentityHelper.GenerateIdentity("raid"),
                    ChannelId = caller.ChannelId,
                    CreatorId = caller.Id,
                    Day = requestedDay.ToString(),
                    Time = requestedTime,
                    TimezoneId = caller.TimezoneId
                });
                ChannelRaid createdRaid = null;
                if (false == upsertRaidResult.Match<bool>(
                    c => { createdRaid = c; return true; },
                    error => { Logger.LogRequestError(error, "Could not register raid"); return false; }))
                {
                    await command.RespondAsync("We couldn't add the raid.  Please bug Dakt");
                    return;
                }

                await command.RespondAsync($"Created raid {createdRaid.Id}.  It is {createdRaid.Day} at {createdRaid.Time}");
            }
        }

        internal override async Task Register(IDiscordBotClient client, SocketGuild guild)
        {
            using (Logger.BeginScope("Registering command {command}", Name))
            {
                SlashCommandBuilder command = new SlashCommandBuilder();
                command.WithName(Name.ToLower());
                command.WithDescription(Description);

                SlashCommandOptionBuilder dayOption = new SlashCommandOptionBuilder();
                dayOption = dayOption.WithName(DAY_OPTION).WithDescription("Day of the week of the raid").WithRequired(true).WithType(ApplicationCommandOptionType.Integer);
                foreach (DayOfWeek dow in Enum.GetValues<DayOfWeek>())
                {
                    dayOption.AddChoice(dow.ToString(), (int)dow);
                }

                SlashCommandOptionBuilder timeOption = new SlashCommandOptionBuilder();
                timeOption = timeOption.WithName(TIME_OPTION).WithDescription("Time of the raid in your timezone").WithRequired(true).WithType(ApplicationCommandOptionType.String);
                command.AddOption(dayOption);
                command.AddOption(timeOption);

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
