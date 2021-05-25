using System.Collections.Generic;
using System.Xml.Serialization;
using RFMathQuiz.Models;
using Rocket.API;

namespace RFMathQuiz
{
    public class Configuration : IRocketPluginConfiguration
    {
        public bool Enabled;
        public string AnnouncerImageUrl, MessageColor, UnfavorableMessageColor;
        public float IntervalInSeconds;
        [XmlArrayItem("Quiz")]
        public List<QuizModel> Quizzes;
        public void LoadDefaults()
        {
            Enabled = true;
            AnnouncerImageUrl = "https://i.pinimg.com/originals/8e/80/2a/8e802a0b020d2a38f427ac80a70d23b0.png";
            IntervalInSeconds = 300;
            MessageColor = "yellow";
            UnfavorableMessageColor = "red";
            Quizzes = new List<QuizModel>
            {
                new QuizModel(EQuizType.Addition, 1, 1000, 25, ERewardType.Experience, 100),
                new QuizModel(EQuizType.Subtraction, 1, 1000, 25, ERewardType.Experience, 100),
                new QuizModel(EQuizType.Multiplication, 1, 10, 25, ERewardType.Experience, 100),
                new QuizModel(EQuizType.Division, 1, 10, 25, ERewardType.Experience, 100),
            };
        }
    }
}