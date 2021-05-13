using System.Xml.Serialization;

namespace MathQuiz.Models
{
    public class Quiz
    {
        [XmlAttribute("Type")]
        public EQuizType Type;
        [XmlAttribute("MinNumber")]
        public int MinNumber;
        [XmlAttribute("MaxNumber")]
        public int MaxNumber;
        [XmlAttribute("Chance")]
        public byte Chance;
        [XmlAttribute("RewardType")]
        public ERewardType RewardType;
        [XmlAttribute("RewardAmount")]
        public uint RewardAmount;

        public Quiz(){}
        public Quiz(EQuizType type, int minNumber, int maxNumber, byte chance, ERewardType rewardType, uint rewardAmount)
        {
            Type = type;
            MinNumber = minNumber;
            MaxNumber = maxNumber;
            Chance = chance;
            RewardType = rewardType;
            RewardAmount = rewardAmount;
        }
    }

    public enum EQuizType
    {
        Addition,
        Subtraction,
        Multiplication,
        Division,
    }

    public enum ERewardType
    {
        Experience,
        Uconomy,
    }
}