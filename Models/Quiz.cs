namespace MathQuiz.Models
{
    public class MathQuiz
    {
        public EQuizType Type;
        public int MinNumber;
        public int MaxNumber;
        public uint AnswerTime;
        public byte Chance;
        public ERewardType RewardType;
        public uint RewardAmount;

        public MathQuiz(EQuizType type, int minNumber, int maxNumber, uint answerTime, byte chance, ERewardType rewardType, uint rewardAmount)
        {
            Type = type;
            MinNumber = minNumber;
            MaxNumber = maxNumber;
            AnswerTime = answerTime;
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