using Discord.Commands;
using System.Threading.Tasks;

namespace Dogey.SQLite.Modules
{
    [Group("tags")]
    [Remarks("Search and view available tags.")]
    public class TagsModule : ModuleBase<SocketCommandContext>
    {
        private TagDatabase _db;

        protected override void BeforeExecute()
        {
            _db = new TagDatabase();
        }

        protected override void AfterExecute()
        {
            _db.Dispose();
        }

        [Command]
        public Task TagsAsync()
        {
            return Task.CompletedTask;
        }

        [Command]
        public Task TagsAsync([Remainder]string name)
        {
            return Task.CompletedTask;
        }
    }
}
