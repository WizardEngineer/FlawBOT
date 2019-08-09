﻿using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using FlawBOT.Common;
using FlawBOT.Framework.Models;
using FlawBOT.Framework.Services;

namespace FlawBOT.Modules
{
    [Group("bot")]
    [Description("Basic commands for interacting with FlawBOT")]
    [Cooldown(3, 5, CooldownBucketType.Channel)]
    public class BotModule : BaseCommandModule
    {
        #region COMMAND_INFO

        [Command("info")]
        [Aliases("i")]
        [Description("Retrieve FlawBOT information")]
        public async Task BotInfo(CommandContext ctx)
        {
            var uptime = DateTime.Now - SharedData.ProcessStarted;
            var output = new DiscordEmbedBuilder()
                .WithTitle(SharedData.Name)
                .WithDescription("A multipurpose Discord bot written in C# with [DSharpPlus](https://github.com/DSharpPlus/DSharpPlus/).")
                .AddField(":clock1: Uptime", $"{(int)uptime.TotalDays:00} days {uptime.Hours:00}:{uptime.Minutes:00}:{uptime.Seconds:00}", true)
                .AddField(":link: Links", $"[Commands]({SharedData.GitHubLink}wiki) **|** [Invite]({SharedData.InviteLink}) **|** [GitHub]({SharedData.GitHubLink})", true)
                .WithFooter("Thank you for using " + SharedData.Name + $" (v{SharedData.Version})")
                .WithUrl(SharedData.GitHubLink)
                .WithColor(SharedData.DefaultColor);
            await ctx.RespondAsync(embed: output.Build());
        }

        #endregion COMMAND_INFO

        #region COMMAND_LEAVE

        [Command("leave")]
        [Description("Make FlawBOT leave the current server")]
        [RequireUserPermissions(Permissions.Administrator)]
        public async Task LeaveAsync(CommandContext ctx)
        {
            await BotServices.SendEmbedAsync(ctx, $"Are you sure you want {SharedData.Name} to leave this server?\nRespond with **yes** to proceed or wait 10 seconds to cancel this operation.");
            var interactivity = await ctx.Client.GetInteractivity().WaitForMessageAsync(m => m.Channel.Id == ctx.Channel.Id && m.Author.Id == ctx.User.Id && m.Content.ToLowerInvariant() == "yes", TimeSpan.FromSeconds(10));
            if (interactivity.Result == null)
                await BotServices.SendEmbedAsync(ctx, "Request timed out...");
            else
            {
                await BotServices.SendEmbedAsync(ctx, "Thank you for using " + SharedData.Name);
                await ctx.Guild.LeaveAsync();
            }
        }

        #endregion COMMAND_LEAVE

        #region COMMAND_PING

        [Command("ping")]
        [Aliases("pong")]
        [Description("Ping the FlawBOT client")]
        public async Task Ping(CommandContext ctx)
        {
            await ctx.RespondAsync($":ping_pong: Pong! Ping: **{ctx.Client.Ping}**ms");
        }

        #endregion COMMAND_PING

        #region COMMAND_REPORT

        [Command("report"), Hidden]
        [Aliases("issue")]
        [Description("Report a problem with FlawBOT to the developer. Please do not abuse.")]
        public async Task ReportIssue(CommandContext ctx,
            [Description("Detailed description of the issue")] [RemainingText] string report)
        {
            if (string.IsNullOrWhiteSpace(report) || report.Length < 50)
                await ctx.RespondAsync("Please provide more information on the issue (50 characters minimum).");
            else
            {
                await BotServices.SendEmbedAsync(ctx, "The following information will be sent to the developer for investigation: User ID, Server ID, Server Name and Server Owner Name.\nRespond with **yes** in the next 10 seconds to proceed, otherwise the operation will be cancelled.");
                var interactivity = await ctx.Client.GetInteractivity().WaitForMessageAsync(m => m.Channel.Id == ctx.Channel.Id && m.Author.Id == ctx.User.Id && m.Content.ToLowerInvariant() == "yes", TimeSpan.FromSeconds(10));
                if (interactivity.Result == null)
                    await BotServices.SendEmbedAsync(ctx, "Request timed out...");
                else
                {
                    var dm = await ctx.Member.CreateDmChannelAsync();
                    var output = new DiscordEmbedBuilder()
                        .WithAuthor(ctx.Guild.Owner.Username + "#" + ctx.Guild.Owner.Discriminator, iconUrl: ctx.User.AvatarUrl ?? ctx.User.DefaultAvatarUrl)
                        .AddField("Issue", report)
                        .AddField("Sent By", ctx.User.Username + "#" + ctx.User.Discriminator)
                        .AddField("Server", ctx.Guild.Name + $" (ID: {ctx.Guild.Id})")
                        .AddField("Owner", ctx.Guild.Owner.Username + "#" + ctx.Guild.Owner.Discriminator)
                        .AddField("Confirm", $"[Click here to add this issue to GitHub]({SharedData.GitHubLink}/issues/new)")
                        .WithColor(SharedData.DefaultColor);
                    await dm.SendMessageAsync(embed: output.Build());
                    await BotServices.SendEmbedAsync(ctx, "Thank You! Your report has been submitted.", EmbedType.Good);
                }
            }
        }

        #endregion COMMAND_REPORT

        #region COMMAND_SAY

        [Command("say"), Hidden]
        [Aliases("echo")]
        [Description("Make FlawBOT repeat a message")]
        public Task Say(CommandContext ctx,
            [Description("Message for the bot to repeat")] [RemainingText] string message)
        {
            return ctx.RespondAsync((string.IsNullOrWhiteSpace(message)) ? ":thinking:" : message);
        }

        #endregion COMMAND_SAY

        #region COMMAND_UPTIME

        [Command("uptime")]
        [Description("Retrieve the FlawBOT uptime")]
        public async Task Uptime(CommandContext ctx)
        {
            var uptime = DateTime.Now - SharedData.ProcessStarted;
            var days = (uptime.Days > 0) ? $"({uptime.Days:00} days)" : null;
            await BotServices.SendEmbedAsync(ctx, ":clock1: " + SharedData.Name + $" has been online for {uptime.Hours:00}:{uptime.Minutes:00} {days}");
        }

        #endregion COMMAND_UPTIME
    }
}