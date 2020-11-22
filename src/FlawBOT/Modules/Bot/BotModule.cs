﻿using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using FlawBOT.Common;
using FlawBOT.Properties;
using FlawBOT.Services;

namespace FlawBOT.Modules
{
    [Group("bot")]
    [Description("Basic commands for interacting with FlawBOT")]
    [Cooldown(3, 5, CooldownBucketType.Channel)]
    public class BotModule : BaseCommandModule
    {
        #region COMMAND_INFO

        [Command("info"), Aliases("i", "about")]
        [Description("Retrieve FlawBOT information")]
        public async Task BotInfo(CommandContext ctx)
        {
            var uptime = DateTime.Now - SharedData.ProcessStarted;
            var output = new DiscordEmbedBuilder()
                .WithTitle(SharedData.Name)
                .WithDescription(
                    "A multipurpose Discord bot written in C# with [DSharpPlus](https://github.com/DSharpPlus/DSharpPlus/).")
                .AddField(":clock1: Uptime",
                    $"{(int)uptime.TotalDays:00} days {uptime.Hours:00}:{uptime.Minutes:00}:{uptime.Seconds:00}", true)
                .AddField(":link: Links",
                    $"[Commands]({SharedData.GitHubLink}wiki) **|** [GitHub]({SharedData.GitHubLink})", true)
                .WithFooter("Thank you for using " + SharedData.Name + $" (v{SharedData.Version})")
                .WithUrl(SharedData.GitHubLink)
                .WithColor(SharedData.DefaultColor);
            await ctx.RespondAsync(embed: output.Build()).ConfigureAwait(false);
        }

        #endregion COMMAND_INFO

        #region COMMAND_LEAVE

        [Command("leave")]
        [Description("Make FlawBOT leave the current server")]
        [RequireUserPermissions(Permissions.Administrator)]
        public async Task LeaveServer(CommandContext ctx)
        {
            await ctx.RespondAsync($"Are you sure you want {SharedData.Name} to leave this server?")
                .ConfigureAwait(false);
            var message = await ctx
                .RespondAsync(Resources.INFO_RESPOND)
                .ConfigureAwait(false);
            var interactivity = await BotServices.GetUserInteractivity(ctx, "yes", 10).ConfigureAwait(false);
            if (interactivity.Result is null)
            {
                await message.ModifyAsync("~~" + message.Content + "~~ " + Resources.INFO_REQ_TIMEOUT)
                    .ConfigureAwait(false);
                return;
            }

            await BotServices.SendEmbedAsync(ctx, "Thank you for using " + SharedData.Name).ConfigureAwait(false);
            await ctx.Guild.LeaveAsync().ConfigureAwait(false);
        }

        #endregion COMMAND_LEAVE

        #region COMMAND_PING

        [Command("ping"), Aliases("pong")]
        [Description("Ping the FlawBOT client")]
        public async Task Ping(CommandContext ctx)
        {
            await BotServices.SendEmbedAsync(ctx, $":ping_pong: Pong! Ping: **{ctx.Client.Ping}**ms")
                .ConfigureAwait(false);
        }

        #endregion COMMAND_PING

        #region COMMAND_REPORT

        [Hidden, Command("report"), Aliases("issue")]
        [Description("Report a problem with FlawBOT to the developer. Please do not abuse.")]
        public async Task ReportIssue(CommandContext ctx,
            [Description("Detailed description of the issue"), RemainingText]
            string report)
        {
            if (string.IsNullOrWhiteSpace(report) || report.Length < 50)
            {
                await ctx.RespondAsync(Resources.ERR_REPORT_LENGTH).ConfigureAwait(false);
                return;
            }

            await ctx.RespondAsync(
                    "The following information will be sent to the developer for investigation: User ID, Server ID, Server Name and Server Owner Name.")
                .ConfigureAwait(false);
            var message = await ctx
                .RespondAsync(Resources.INFO_RESPOND)
                .ConfigureAwait(false);
            var interactivity = await BotServices.GetUserInteractivity(ctx, "yes", 10).ConfigureAwait(false);
            if (interactivity.Result is null)
            {
                await message.ModifyAsync("~~" + message.Content + "~~ " + Resources.INFO_REQ_TIMEOUT)
                    .ConfigureAwait(false);
            }
            else
            {
                var dm = await ctx.Member.CreateDmChannelAsync().ConfigureAwait(false);
                var output = new DiscordEmbedBuilder()
                    .WithAuthor(ctx.Guild.Owner.Username + "#" + ctx.Guild.Owner.Discriminator,
                        iconUrl: ctx.User.AvatarUrl ?? ctx.User.DefaultAvatarUrl)
                    .AddField("Issue", report)
                    .AddField("Sent By", ctx.User.Username + "#" + ctx.User.Discriminator)
                    .AddField("Server", ctx.Guild.Name + $" (ID: {ctx.Guild.Id})")
                    .AddField("Owner", ctx.Guild.Owner.Username + "#" + ctx.Guild.Owner.Discriminator)
                    .AddField("Confirm",
                        $"[Click here to add this issue to GitHub]({SharedData.GitHubLink}/issues/new)")
                    .WithColor(SharedData.DefaultColor);
                await dm.SendMessageAsync(embed: output.Build()).ConfigureAwait(false);
                await ctx.RespondAsync("Thank You! Your report has been submitted.").ConfigureAwait(false);
            }
        }

        #endregion COMMAND_REPORT

        #region COMMAND_SAY

        [Hidden, Command("say"), Aliases("echo")]
        [Description("Make FlawBOT repeat a message")]
        public async Task Say(CommandContext ctx,
            [Description("Message for the bot to repeat"), RemainingText]
            string message)
        {
            await BotServices.RemoveMessage(ctx.Message).ConfigureAwait(false);
            await ctx.RespondAsync(message ?? ":thinking:").ConfigureAwait(false);
        }

        #endregion COMMAND_SAY

        #region COMMAND_TTS

        [Command("tts"), Aliases("talk")]
        [Description("Make FlawBOT repeat a message in text-to-speech")]
        [RequirePermissions(Permissions.SendTtsMessages)]
        public async Task SayTts(CommandContext ctx,
            [Description("Message for the bot to convert to speech"), RemainingText]
            string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                await ctx.RespondAsync(":thinking:").ConfigureAwait(false);
            else
                await ctx.RespondAsync(Formatter.BlockCode(Formatter.Strip(message)), true).ConfigureAwait(false);
        }

        #endregion COMMAND_TTS

        #region COMMAND_UPTIME

        [Command("uptime"), Aliases("time")]
        [Description("Retrieve the FlawBOT uptime")]
        public async Task Uptime(CommandContext ctx)
        {
            var uptime = DateTime.Now - SharedData.ProcessStarted;
            var days = uptime.Days > 0 ? $"({uptime.Days:00} days)" : string.Empty;
            await BotServices.SendEmbedAsync(ctx,
                    $":clock1: {SharedData.Name} has been online for {uptime.Hours:00}:{uptime.Minutes:00}:{uptime.Seconds} {days}")
                .ConfigureAwait(false);
        }

        #endregion COMMAND_UPTIME

        #region OWNERS-ONLY

        #region COMMAND_ACTIVITY

        [RequireOwner, Hidden, Command("activity"), Aliases("setactivity")]
        [Description("Set FlawBOT's activity")]
        public async Task SetBotActivity(CommandContext ctx,
            [Description("Name of the activity"), RemainingText]
            string activity)
        {
            if (string.IsNullOrWhiteSpace(activity))
            {
                await ctx.Client.UpdateStatusAsync().ConfigureAwait(false);
                return;
            }

            var game = new DiscordActivity(activity);
            await ctx.Client.UpdateStatusAsync(game).ConfigureAwait(false);
            await ctx.RespondAsync($"{SharedData.Name} activity has been changed to Playing {game.Name}").ConfigureAwait(false);
        }

        #endregion COMMAND_ACTIVITY

        #region COMMAND_AVATAR

        [RequireOwner, Hidden, Command("avatar"), Aliases("setavatar", "pfp", "photo")]
        [Description("Set FlawBOT's avatar")]
        public async Task SetBotAvatar(CommandContext ctx,
            [Description("Image URL. Must be in jpg, png or img format.")]
            string query)
        {
            var stream = BotServices.CheckImageInput(ctx, query).Result;
            if (stream.Length <= 0) return;
            await ctx.Client.UpdateCurrentUserAsync(avatar: stream).ConfigureAwait(false);
            await ctx.RespondAsync($"{SharedData.Name} avatar has been updated!").ConfigureAwait(false);
        }

        #endregion COMMAND_AVATAR

        #region COMMAND_STATUS

        [RequireOwner, Hidden, Command("status"), Aliases("setstatus", "state")]
        [Description("Set FlawBOT's status")]
        public async Task SetBotStatus(CommandContext ctx,
            [Description("Activity Status. Online, Idle, DND or Offline"), RemainingText]
            string status)
        {
            status ??= "ONLINE";
            switch (status.Trim().ToUpperInvariant())
            {
                case "OFF":
                case "OFFLINE":
                    await ctx.Client.UpdateStatusAsync(userStatus: UserStatus.Offline).ConfigureAwait(false);
                    await BotServices.SendEmbedAsync(ctx, $"{SharedData.Name} status has been changed to Offline")
                        .ConfigureAwait(false);
                    break;

                case "INVISIBLE":
                    await ctx.Client.UpdateStatusAsync(userStatus: UserStatus.Invisible).ConfigureAwait(false);
                    await BotServices.SendEmbedAsync(ctx, $"{SharedData.Name} status has been changed to Invisible")
                        .ConfigureAwait(false);
                    break;

                case "IDLE":
                    await ctx.Client.UpdateStatusAsync(userStatus: UserStatus.Idle).ConfigureAwait(false);
                    await BotServices.SendEmbedAsync(ctx, $"{SharedData.Name} status has been changed to Idle")
                        .ConfigureAwait(false);
                    break;

                case "DND":
                case "DO NOT DISTURB":
                    await ctx.Client.UpdateStatusAsync(userStatus: UserStatus.DoNotDisturb).ConfigureAwait(false);
                    await BotServices
                        .SendEmbedAsync(ctx, $"{SharedData.Name} status has been changed to Do Not Disturb")
                        .ConfigureAwait(false);
                    break;

                default:
                    await ctx.Client.UpdateStatusAsync(userStatus: UserStatus.Online).ConfigureAwait(false);
                    await BotServices.SendEmbedAsync(ctx, $"{SharedData.Name} status has been changed to Online")
                        .ConfigureAwait(false);
                    break;
            }
        }

        #endregion COMMAND_STATUS

        #region COMMAND_USERNAME

        [RequireOwner, Hidden, Command("username"), Aliases("setusername", "name", "setname", "nickname", "nick")]
        [Description("Set FlawBOT's username")]
        public async Task SetBotUsername(CommandContext ctx,
            [Description("New bot username"), RemainingText]
            string name)
        {
            var oldName = ctx.Client.CurrentUser.Username;
            var newName = string.IsNullOrWhiteSpace(name) ? SharedData.Name : name;
            await ctx.Client.UpdateCurrentUserAsync(newName).ConfigureAwait(false);
            await BotServices.SendEmbedAsync(ctx, $"{oldName}'s username has been changed to {newName}")
                .ConfigureAwait(false);
        }

        #endregion COMMAND_USERNAME

        #endregion OWNERS-ONLY
    }
}