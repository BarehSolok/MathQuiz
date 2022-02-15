using System.Collections.Generic;
using System.Xml.Serialization;
using RFMathQuiz.Enums;
using RFMathQuiz.Models;
using Rocket.API;

namespace RFMathQuiz
{
    public class Configuration : IRocketPluginConfiguration
    {
        public bool Enabled;
        public string MessageIconUrl, MessageColor, UnfavorableMessageColor;
        public float IntervalInSeconds;
        public List<Quiz> Quizzes;
        public void LoadDefaults()
        {
            Enabled = true;
            MessageColor = "yellow";
            UnfavorableMessageColor = "red";
            // MessageIconUrl = "https://i.pinimg.com/originals/8e/80/2a/8e802a0b020d2a38f427ac80a70d23b0.png";
            MessageIconUrl = "https://cdn.jsdelivr.net/gh/RiceField-Plugins/UnturnedImages@images/plugin/RFMathQuiz/RFMathQuiz.png";
            IntervalInSeconds = 30;
            Quizzes = new List<Quiz>
            {
                new Quiz(EQuiz.ADDITION, 1, 1000, 45, EReward.EXPERIENCE, 100),
                new Quiz(EQuiz.SUBTRACTION, 1, 1000, 45, EReward.EXPERIENCE, 100),
                new Quiz(EQuiz.MULTIPLICATION, 1, 10, 25, EReward.EXPERIENCE, 100),
                new Quiz(EQuiz.DIVISION, 1, 10, 25, EReward.EXPERIENCE, 100),
            };
        }
    }
}