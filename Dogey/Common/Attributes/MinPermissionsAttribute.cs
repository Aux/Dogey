﻿using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Dogey.Enums;

namespace Dogey.Attributes
{
    /// <summary>
    /// Set the minimum permission required to use a module or command.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class MinPermissionsAttribute : PreconditionAttribute
    {
        private AccessLevel Level;
        
        public MinPermissionsAttribute(AccessLevel level)
        {
            Level = level;
        }

        public override Task<PreconditionResult> CheckPermissions(IUserMessage context, Command executingCommand, object moduleInstance)
        {
            var access = GetPermission(context);

            if (access >= Level)
                return Task.FromResult(PreconditionResult.FromSuccess());
            else
                return Task.FromResult(PreconditionResult.FromError("Insufficient permissions."));
        }

        public AccessLevel GetPermission(IUserMessage msg)
        {
            if (msg.Author.IsBot)
                return AccessLevel.Blocked;

            if (Globals.Config.Owners.Contains(msg.Author.Id))
                return AccessLevel.Owner;

            return AccessLevel.User;
        }
    }
}