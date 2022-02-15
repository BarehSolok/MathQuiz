using System.Xml.Serialization;
using RFMathQuiz.Enums;

namespace RFMathQuiz.Models
{
    public class Quiz
    {
        [XmlAttribute("Type")]
        public EQuiz Type;
        [XmlAttribute("MinNumber")]
        public int MinNumber;
        [XmlAttribute("MaxNumber")]
        public int MaxNumber;
        [XmlAttribute("Chance")]
        public byte Chance;
        [XmlAttribute("RewardType")]
        public EReward Reward;
        [XmlAttribute("RewardAmount")]
        public uint RewardAmount;

        public Quiz(){}
        public Quiz(EQuiz type, int minNumber, int maxNumber, byte chance, EReward reward, uint rewardAmount)
        {
            Type = type;
            MinNumber = minNumber;
            MaxNumber = maxNumber;
            Chance = chance;
            Reward = reward;
            RewardAmount = rewardAmount;
        }
    }
}