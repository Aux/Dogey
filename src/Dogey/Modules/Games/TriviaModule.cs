using Discord;
using Discord.Commands;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dogey.Modules.Games
{
    public class TriviaModule : DogeyModuleBase
    {
        private readonly TriviaApiService _trivia;
        private readonly ResponsiveService _responsive;

        private IEmote _emote = Emote.Parse("<a:DogeDance:393566339639279636>");

        public TriviaModule(TriviaApiService trivia, ResponsiveService responsive, RootController root)
            : base(root)
        {
            _trivia = trivia;
            _responsive = responsive;
        }
        
        [Command("question")]
        public async Task QuestionAsync()
        {
            var question = await _trivia.GetQuestionAsync(Context.Guild.Id);
            question.Unbase64ify();

            var embed = new EmbedBuilder()
                .WithTitle(question.Category)
                .WithDescription(question.Question);

            switch (question.Type)
            {
                case "boolean":
                    await HandleBooleanQuestion(question, embed);
                    break;
                case "multiple":
                    await HandleMultipleQuestion(question, embed);
                    break;
                default:
                    await ReplyAsync($"Found a question of type `{question.Type}` that I don't know how to handle :T");
                    break;
            }
        }
        
        private async Task HandleBooleanQuestion(TriviaQuestion question, EmbedBuilder embed)
        {
            embed.WithFooter("Reply true/false to answer this question!");

            var correctAnswer = bool.Parse(question.CorrectAnswer);
            var questionMsg = await ReplyEmbedAsync(embed);
            
            while (true)
            {
                var reply = await _responsive.WaitForMessageAsync((msg) => msg.Channel.Id == Context.Channel.Id);
                if (reply == null) return;
                
                if (!StringHelper.TryParseBoolean(reply.Content, out bool answer))
                    continue;
                
                if (answer == correctAnswer)
                {
                    await ReplyAsync($"{_emote} {reply.Author.Mention} answered correctly! {_emote}");
                    return;
                }
            }
        }

        private async Task HandleMultipleQuestion(TriviaQuestion question, EmbedBuilder embed)
        {
            embed.WithFooter("Reply with a number to answer this question!");
            
            var answers = new List<string>(question.WrongAnswers);
            answers.Add(question.CorrectAnswer);
            answers.Shuffle();

            var choices = new Dictionary<int, string>();
            for (int i = 0; i < answers.Count - 1; i++)
            {
                choices.Add(i + 1, answers[i]);
                embed.AddField((i + 1).ToString() + ".", answers[i], true);
            }

            var correctAnswer = choices.FirstOrDefault(x => x.Value == question.CorrectAnswer);
            var questionMsg = await ReplyEmbedAsync(embed);

            var answeredIndexes = new List<int>();
            int remaining = choices.Count;
            while (true)
            {
                var reply = await _responsive.WaitForMessageAsync((msg) => msg.Channel.Id == Context.Channel.Id);
                if (reply == null) return;
                
                if (!int.TryParse(reply.Content, out int answer)) continue;
                if (answer >= choices.Count) continue;

                if (answer != correctAnswer.Key)
                {
                    remaining--;
                    if (remaining == 1)
                    {
                        await ReplyAsync($"No tries remaining, the correct answer was `{correctAnswer.Value}`");
                        return;
                    }

                    answeredIndexes.Add(answer);
                    var embedBuilder = new EmbedBuilder()
                        .WithTitle(question.Category)
                        .WithDescription(question.Question)
                        .WithFooter("Reply with a number to answer this question!");

                    foreach (var choice in choices)
                    {
                        if (answeredIndexes.Any(x => x == choice.Key))
                            embedBuilder.AddField(choice.Key.ToString() + ".", Format.Strikethrough(choice.Value), true);
                        else
                            embedBuilder.AddField(choice.Key.ToString() + ".", choice.Value, true);
                    }

                    await questionMsg.ModifyAsync(x => x.Embed = embedBuilder.Build());
                    continue;
                }

                await ReplyAsync($"{_emote} {reply.Author.Mention} answered correctly! {_emote}");
                return;
            }
        }
    }
}
