using Discord.Commands;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Discord.Addons.OwnerTools
{
    internal class AvatarModule
    {
        private CommandService _service;

        public AvatarModule(CommandService service)
        {
            _service = service;
        }

        public Task LoadAsync()
            => LoadAsync(new AvatarCommandConfig());

        public Task LoadAsync(Action<AvatarCommandConfig> options)
        {
            var config = new AvatarCommandConfig();
            options.Invoke(config);
            return LoadAsync(config);
        }

        public async Task LoadAsync(AvatarCommandConfig config)
        {
            await _service.CreateModuleAsync(null, module =>
            {
                module.AddAliases(config.Aliases.ToArray());

                module.AddCommand(null, GetAvatarAsync, cmd =>
                {
                    cmd.Remarks = config.Remarks;
                    cmd.Summary = config.Summary;

                    foreach (var pcondition in config.Preconditions)
                        cmd.AddPrecondition(pcondition);
                });

                module.AddCommand(null, SetAvatarAsync, cmd =>
                {
                    cmd.Remarks = config.Remarks;
                    cmd.Summary = config.Summary;

                    cmd.AddParameter<string>("url", x => { });

                    foreach (var pcondition in config.Preconditions)
                        cmd.AddPrecondition(pcondition);
                });
            });
        }

        private Task GetAvatarAsync(ICommandContext context, object[] parameters, IDependencyMap map)
        {
            string avatarUrl = context.Client.CurrentUser.GetAvatarUrl();
            return context.Channel.SendMessageAsync(avatarUrl);
        }
        
        private async Task SetAvatarAsync(ICommandContext context, object[] parameters, IDependencyMap map)
        {
            string urlParam = parameters.First().ToString();
            if (!Uri.TryCreate(urlParam, UriKind.RelativeOrAbsolute, out Uri url))
                throw new ArgumentException($"`{urlParam}` is not a valid image url");
            
            var request = new HttpRequestMessage(new HttpMethod("GET"), url);

            using (var client = new HttpClient())
            {
                var response = await client.SendAsync(request);
                var stream = await response.Content.ReadAsStreamAsync();

                var self = context.Client.CurrentUser;
                await self.ModifyAsync(x =>
                {
                    x.Avatar = new Image(stream);
                });
                await context.Channel.SendMessageAsync("👍");
            }
        }
    }
}
