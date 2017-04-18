using Discord.Commands;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Discord.Addons.OwnerTools
{
    internal class ActivityModule
    {
        private CommandService _service;

        public ActivityModule(CommandService service)
        {
            _service = service;
        }

        public Task LoadAsync()
            => LoadAsync(new ActivityCommandConfig());

        public Task LoadAsync(Action<ActivityCommandConfig> options)
        {
            var config = new ActivityCommandConfig();
            options.Invoke(config);
            return LoadAsync(config);
        }

        public async Task LoadAsync(ActivityCommandConfig config)
        {
            await _service.CreateModuleAsync(null, module =>
            {
                module.AddAliases(config.Aliases.ToArray());

                module.AddCommand(null, GetActivityAsync, cmd =>
                {
                    cmd.Remarks = config.Remarks;
                    cmd.Summary = config.Summary;

                    foreach (var pcondition in config.Preconditions)
                        cmd.AddPrecondition(pcondition);
                });

                module.AddCommand(null, SetActivityAsync, cmd =>
                {
                    cmd.Remarks = config.Remarks;
                    cmd.Summary = config.Summary;

                    cmd.AddParameter<string>("name", x => { });

                    foreach (var pcondition in config.Preconditions)
                        cmd.AddPrecondition(pcondition);
                });
            });
        }

        private Task GetActivityAsync(ICommandContext context, object[] parameters, IDependencyMap map)
        {
            throw new NotImplementedException();
        }

        private Task SetActivityAsync(ICommandContext context, object[] parameters, IDependencyMap map)
        {
            string name = parameters.First().ToString();

            throw new NotImplementedException();
        }
    }
}
