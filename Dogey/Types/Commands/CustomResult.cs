using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dogey.Modules.Commands
{
    public class CustomResult : IResult
    {
        public CommandError? Error { get; set; }
        public string ErrorReason { get; set; }
        public bool IsSuccess { get; set; }
    }
}
