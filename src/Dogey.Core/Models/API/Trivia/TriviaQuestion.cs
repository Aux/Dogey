using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dogey
{
    public class TriviaQuestion
    {
        [JsonProperty("category")]
        public string Category { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("difficulty")]
        public string Difficulty { get; set; }
        [JsonProperty("question")]
        public string Question { get; set; }
        [JsonProperty("correct_answer")]
        public string CorrectAnswer { get; set; }
        [JsonProperty("incorrect_answers")]
        public List<string> WrongAnswers { get; set; }

        public void Unbase64ify()
        {
            Category = Category.FromBase64();
            Type = Type.FromBase64();
            Difficulty = Difficulty.FromBase64();
            Question = Question.FromBase64();
            CorrectAnswer = CorrectAnswer.FromBase64();

            if (WrongAnswers != null)
            {
                var answers = new List<string>();
                foreach (var answer in WrongAnswers)
                    answers.Add(answer.FromBase64());
                WrongAnswers = answers;
            }
        }
    }
}
