using System;
using System.Collections.Generic;
using System.Linq;
using Rocket.API;
using Rocket.API.Collections;
using Rocket.Core.Plugins;
using RFMathQuiz.Models;
using RFRocketLibrary.Hooks;
using SDG.Unturned;
using UnityEngine;
using Color = UnityEngine.Color;
using Logger = Rocket.Core.Logging.Logger;

namespace RFMathQuiz
{
    public class Plugin : RocketPlugin<Configuration>
    {
        private static Coroutine _quizCor;
        internal static Plugin Inst;
        internal static Configuration Conf;
        internal static bool NoQuestion;

        protected override void Load()
        {
            Inst = this;
            Conf = Configuration.Instance;

            if (Configuration.Instance.Enabled)
            {
                if (UconomyHook.CanBeLoaded())
                    UconomyHook.Load();
                _quizCor = StartCoroutine(AutoRepeat());

                Logger.LogWarning("[RFMathQuiz] Plugin loaded successfully!");
                Logger.LogWarning("[RFMathQuiz] RFMathQuiz v1.0.0");
                Logger.LogWarning("[RFMathQuiz] Made with 'rice' by RiceField Plugins!");
            }
            else
            {
                Logger.LogWarning("[RFMathQuiz] Plugin loaded successfully!");
                Logger.LogError("[RFMathQuiz] RFMathQuiz: DISABLED. Check configuration files!");
                Logger.LogError("[RFMathQuiz] Unloading plugin...");

                Unload();
            }
        }

        protected override void Unload()
        {
            Inst = null;
            Conf = null;
            if (_quizCor != null)
            {
                StopCoroutine(_quizCor);
                _quizCor = null;
            }

            Logger.LogWarning("[RFMathQuiz] Plugin unloaded successfully!");
        }

        public override TranslationList DefaultTranslations =>
            new TranslationList()
            {
                {
                    "mathquiz_broadcast_question",
                    "[RFMathQuiz] '{0} {2} {1} = ?' Know the question and earn {3} {4} '/re <answer>'"
                },
                {
                    "mathquiz_broadcast_winner",
                    "[RFMathQuiz] {0} answered the question correctly and earned {2} {3}! Answer: {1}"
                },
                {
                    "mathquiz_true_answer",
                    "[RFMathQuiz] Congratulations! You have earned {0} {1} by answering correctly! Your new balance: {2} {1}"
                },
                {"mathquiz_wrong_answer", "[RFMathQuiz] Wrong answer!"},
                {"mathquiz_invalid_parameter", "[RFMathQuiz] Usage: '/re <answer>' or '/answer <answer>'"},
                {"mathquiz_no_question", "[RFMathQuiz] No active questions yet!"},
                {"mathquiz_disabled", "[RFMathQuiz] Disabled!"}
            };

        private IEnumerator<WaitForSeconds> AutoRepeat()
        {
            while (Conf.Enabled)
            {
                AutoQuiz();
                yield return new WaitForSeconds(Conf.IntervalInSeconds);
            }
        }

        internal QuizModel CurrentQuizModel;
        internal static int Number1, Number2, Result;
        internal static string Symbol;

        private void AutoQuiz()
        {
            if (!Conf.Enabled) return;
            try
            {
                if (State != PluginState.Loaded || Configuration.Instance.IntervalInSeconds == 0)
                    return;
                var quizzes = Conf.Quizzes.ToList();

                RandomizeQuiz(out CurrentQuizModel, out Symbol, ref Number1, ref Number2, ref Result);

                if (CurrentQuizModel.RewardType == ERewardType.Uconomy)
                {
                    var broadcast = Translate("mathquiz_broadcast_question", Number1.ToString(), Number2.ToString(),
                        Symbol, CurrentQuizModel.RewardAmount.ToString(), UconomyHook.MoneyName);
                    ChatManager.serverSendMessage(broadcast, Color.yellow, null, null, EChatMode.GLOBAL,
                        Conf.AnnouncerImageUrl, true);
                }
                else
                {
                    var broadcast = Translate("mathquiz_broadcast_question", Number1.ToString(), Number2.ToString(),
                        Symbol, CurrentQuizModel.RewardAmount.ToString(), "Experience");
                    ChatManager.serverSendMessage(broadcast, Color.yellow, null, null, EChatMode.GLOBAL,
                        Conf.AnnouncerImageUrl, true);
                }

                NoQuestion = false;
            }
            catch (Exception ex)
            {
                Logger.LogError("[RFMathQuiz] Error: " + ex);
            }
        }

        private void RandomizeQuiz(out QuizModel selectedQuizModel, out string symbol, ref int number1, ref int number2,
            ref int result)
        {
            selectedQuizModel = null;
            var totalChance = Conf.Quizzes.Sum(q => q.Chance);
            symbol = "";

            var random = new System.Random();
            int i = 0, diceRoll = random.Next(0, totalChance);
            foreach (var quiz in Conf.Quizzes)
            {
                if (diceRoll > i && diceRoll <= i + quiz.Chance)
                {
                    selectedQuizModel = quiz;
                    break;
                }

                i += quiz.Chance;
            }

            if (selectedQuizModel == null)
            {
                Logger.LogWarning("[RFMathQuiz] Warning: Please setup RFMathQuiz configuration files first!");
                return;
            }

            number1 = random.Next(selectedQuizModel.MinNumber, selectedQuizModel.MaxNumber);
            number2 = random.Next(selectedQuizModel.MinNumber, selectedQuizModel.MaxNumber);
            switch (selectedQuizModel.Type)
            {
                case EQuizType.Addition:
                    symbol = "+";
                    result = number1 + number2;
                    break;
                case EQuizType.Subtraction:
                    symbol = "-";
                    result = number1 - number2;
                    break;
                case EQuizType.Multiplication:
                    symbol = "×";
                    result = number1 * number2;
                    break;
                case EQuizType.Division:
                    symbol = "÷";
                    var num1 = Convert.ToSingle(number1);
                    var num2 = Convert.ToSingle(number2);
                    var res = num1 / num2;
                    while (res % 1 != 0)
                    {
                        num1 = Convert.ToSingle(random.Next(selectedQuizModel.MinNumber, selectedQuizModel.MaxNumber));
                        num2 = Convert.ToSingle(random.Next(selectedQuizModel.MinNumber, selectedQuizModel.MaxNumber));
                        res = num1 / num2;
                    }

                    number1 = Convert.ToInt32(num1);
                    number2 = Convert.ToInt32(num2);
                    result = Convert.ToInt32(res);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}