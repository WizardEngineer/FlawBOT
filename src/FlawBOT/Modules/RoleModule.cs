﻿using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using FlawBOT.Common;
using FlawBOT.Properties;
using FlawBOT.Services;
using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlawBOT.Modules
{
    [SlashCommandGroup("role", "Slash command group for role commands.")]
    public class RoleModule : ApplicationCommandModule
    {
        /// <summary>
        /// Changes server role color.
        /// </summary>
        //[SlashCommand("color", "Changes server role color.")]
        //[SlashRequirePermissions(Permissions.ManageRoles)]
        //public async Task SetRoleColor(InteractionContext ctx, [Option("color", "Server role's new HEX color code.")] DiscordColor color, [Option("role", "Server role to recolor.")] DiscordRole role)
        //{
        //    var regex = new Regex("^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$", RegexOptions.Compiled).Match(color.ToString());
        //    if (!regex.Success)
        //    {
        //        await BotServices.SendResponseAsync(ctx, "Invalid color code. Please enter a HEX color code like #E7B53B", ResponseType.Warning).ConfigureAwait(false);
        //        return;
        //    }

        //    await role.ModifyAsync(color: color).ConfigureAwait(false);
        //    var output = new DiscordEmbedBuilder()
        //        .WithTitle("Successfully set the color for the role " + Formatter.Bold(role.Name) + " to " + Formatter.InlineCode(role.Color.ToString()))
        //        .WithColor(color);
        //    await ctx.CreateResponseAsync(output.Build()).ConfigureAwait(false);
        //}

        /// <summary>
        /// Creates a new server role.
        /// </summary>
        [SlashCommand("create", "Creates a mew server role.")]
        [SlashRequirePermissions(Permissions.ManageRoles)]
        public async Task CreateRole(InteractionContext ctx, [Option("role", "New role name.")] string role = "")
        {
            if (string.IsNullOrWhiteSpace(role))
            {
                await BotServices.SendResponseAsync(ctx, Resources.ERR_ROLE_NAME, ResponseType.Warning).ConfigureAwait(false);
                return;
            }

            await ctx.Guild.CreateRoleAsync(role).ConfigureAwait(false);
            await ctx.CreateResponseAsync("Successfully created the server role " + Formatter.Bold(role)).ConfigureAwait(false);
        }

        /// <summary>
        /// Deletes server role.
        /// </summary>
        [SlashCommand("delete", "Deletes server role.")]
        [SlashRequirePermissions(Permissions.ManageRoles)]
        public async Task DeleteRole(InteractionContext ctx, [Option("role", "Server role to remove.")] DiscordRole role = null)
        {
            if (role is null)
            {
                await BotServices.SendResponseAsync(ctx, Resources.ERR_ROLE_EXISTING, ResponseType.Warning).ConfigureAwait(false);
                return;
            }

            await role.DeleteAsync().ConfigureAwait(false);
            await ctx.CreateResponseAsync("Successfully removed the server role " + Formatter.Bold(role.Name)).ConfigureAwait(false);
        }

        /// <summary>
        /// Returns server role information.
        /// </summary>
        [SlashCommand("info", "Returns server role information.")]
        public async Task GetRoleInfo(InteractionContext ctx, [Option("role", "Server role name.")] DiscordRole role = null)
        {
            if (role is null)
            {
                await BotServices.SendResponseAsync(ctx, Resources.ERR_ROLE_EXISTING, ResponseType.Warning).ConfigureAwait(false);
                return;
            }

            var output = new DiscordEmbedBuilder()
                .WithTitle(role.Name)
                .WithDescription("ID: " + role.Id)
                .AddField("Creation Date", role.CreationTimestamp.DateTime.ToString(CultureInfo.InvariantCulture), true)
                .AddField("Hoisted", role.IsHoisted ? ":heavy_check_mark:" : ":x:", true)
                .AddField("Mentionable", role.IsMentionable ? ":heavy_check_mark:" : ":x:", true)
                .AddField("Permissions", role.Permissions.ToPermissionString())
                .WithThumbnail(ctx.Guild.IconUrl)
                .WithFooter($"{ctx.Guild.Name} / #{ctx.Channel.Name} / {DateTime.Now}")
                .WithColor(role.Color);
            await ctx.CreateResponseAsync(output.Build()).ConfigureAwait(false);
        }

        /// <summary>
        /// Returns list of users in a given role.
        /// </summary>
        [SlashCommand("inrole", "Returns list of users in a given role.")]
        public async Task GetRoleUsers(InteractionContext ctx, [Option("role", "Server role name.")] DiscordRole role = null)
        {
            if (role is null)
            {
                await BotServices.SendResponseAsync(ctx, Resources.ERR_ROLE_EXISTING, ResponseType.Warning).ConfigureAwait(false);
                return;
            }

            var userCount = 0;
            var usersList = new StringBuilder();
            var users = (await ctx.Guild.GetAllMembersAsync().ConfigureAwait(false)).ToArray();
            foreach (var user in users)
                if (user.Roles.Contains(role))
                {
                    userCount++;
                    if (user.Equals(users.Last()))
                        usersList.Append(user.DisplayName);
                    else
                        usersList.Append(user.DisplayName).Append(", ");
                }

            if (usersList.Length == 0)
                await BotServices.SendResponseAsync(ctx, Formatter.Bold(role.Name) + " has no members").ConfigureAwait(false);
            else
                await BotServices.SendResponseAsync(ctx, Formatter.Bold(role.Name) + " has " + Formatter.Bold(userCount.ToString()) + " member(s): " + usersList).ConfigureAwait(false);
        }

        /// <summary>
        /// Toggles whether the role can be mentioned by others.
        /// </summary>
        [SlashCommand("mention", "Toggles whether the role can be mentioned by others.")]
        [SlashRequirePermissions(Permissions.ManageRoles)]
        public async Task SetRoleMention(InteractionContext ctx, [Option("role", "Server role name.")] DiscordRole role)
        {
            if (role is null) return;
            if (role.IsMentionable)
            {
                await role.ModifyAsync(mentionable: false).ConfigureAwait(false);
                await BotServices.SendResponseAsync(ctx, Formatter.Bold(role.Name) + " is now " + Formatter.Bold("not-mentionable")).ConfigureAwait(false);
            }
            else
            {
                await role.ModifyAsync(mentionable: true).ConfigureAwait(false);
                await BotServices.SendResponseAsync(ctx, Formatter.Bold(role.Name) + " is now " + Formatter.Bold("mentionable")).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Revokes server role from a user.
        /// </summary>
        //[SlashCommand("revoke", "Revokes server role from the user.")]
        //[SlashRequirePermissions(Permissions.ManageRoles)]
        //public async Task RevokeUserRole(InteractionContext ctx, [Option("member", "Server user to get revoked.")] DiscordMember member, [Option("role", "Server role name.")] DiscordRole role)
        //{
        //    if (role is null) return;
        //    member ??= ctx.Member;
        //    await member.RevokeRoleAsync(role).ConfigureAwait(false);
        //    await ctx.CreateResponseAsync(Formatter.Bold(member.DisplayName) + " has been removed from the role " + Formatter.Bold(role.Name)).ConfigureAwait(false);
        //}

        /// <summary>
        /// Revokes all server roles from a user.
        /// </summary>
        //[SlashCommand("revokeall", "Revokes all server roles from a user.")]
        //[SlashRequirePermissions(Permissions.ManageRoles)]
        //public async Task RevokeUserRoles(InteractionContext ctx, [Option("member", "Server user name.")] DiscordMember member)
        //{
        //    if (!member.Roles.Any())
        //    {
        //        await BotServices.SendResponseAsync(ctx, Resources.ERR_ROLE_NONE, ResponseType.Warning).ConfigureAwait(false);
        //        return;
        //    }

        //    if (member.Roles.Max(r => r.Position) >= ctx.Member.Roles.Max(r => r.Position))
        //    {
        //        await BotServices.SendResponseAsync(ctx, Resources.ERR_ROLE_NOT_ALLOWED, ResponseType.Warning).ConfigureAwait(false);
        //        return;
        //    }

        //    await member.ReplaceRolesAsync(Enumerable.Empty<DiscordRole>()).ConfigureAwait(false);
        //    await ctx.CreateResponseAsync("Removed all roles from " + Formatter.Bold(member.DisplayName)).ConfigureAwait(false);
        //}

        /// <summary>
        /// Assigns a server role to a user.
        /// </summary>
        //[SlashCommand("setrole", "Assigns a server role to a user.")]
        //[SlashRequirePermissions(Permissions.ManageRoles)]
        //public async Task SetUserRole(InteractionContext ctx, [Option("member", "Server user name.")] DiscordMember member, [Option("role", "Server role name.")] DiscordRole role)
        //{
        //    member ??= ctx.Member;
        //    await member.GrantRoleAsync(role).ConfigureAwait(false);
        //    await ctx.CreateResponseAsync(member.DisplayName + " been granted the role " + Formatter.Bold(role.Name)).ConfigureAwait(false);
        //}

        /// <summary>
        /// Toggles whether the role can be seen by others.
        /// </summary>
        //[SlashCommand("show", "Toggles whether the role can be seen by others.")]
        //[SlashRequirePermissions(Permissions.ManageRoles)]
        //public async Task SetRoleVisibility(InteractionContext ctx, [Option("role", "Server role name.")] DiscordRole role)
        //{
        //    if (role is null) return;

        //    if (role.IsHoisted)
        //    {
        //        await role.ModifyAsync(hoist: false).ConfigureAwait(false);
        //        await BotServices.SendResponseAsync(ctx, Formatter.Bold(role.Name) + " is now " + Formatter.Bold("hidden")).ConfigureAwait(false);
        //    }
        //    else
        //    {
        //        await role.ModifyAsync(hoist: true).ConfigureAwait(false);
        //        await BotServices.SendResponseAsync(ctx, Formatter.Bold(role.Name) + " is now " + Formatter.Bold("displayed")).ConfigureAwait(false);
        //    }
        //}
    }
}