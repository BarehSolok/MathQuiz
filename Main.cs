using System;
using System.Collections.Generic;
using System.Linq;
using fr34kyn01535.Uconomy;
using Rocket.API;
using Rocket.API.Collections;
using Rocket.Core.Plugins;
using MathQuiz.Models;
using SDG.Unturned;
using UnityEngine;
using Color = UnityEngine.Color;
using Logger = Rocket.Core.Logging.Logger;

namespace MathQuiz
{
    public class Main : RocketPlugin<Configuration>
    {
        private static Coroutine _quizCor;
        internal static Main Inst;
        internal static Configuration Conf;
        internal static bool NoQuestion;
        
        protected override void Load()
        {
            Inst = this;
            Conf = Configuration.Instance;
            
            Logger.LogWarning("[MathQuiz] Plugin loaded successfully!");
            if (Configuration.Instance.Enabled)
            {
                _quizCor = StartCoroutine(AutoRepeat());
                
                Logger.LogWarning("[MathQuiz] MathQuiz v1.0.0");
                Logger.LogWarning("[MathQuiz] Author: BarehSolok#2548");
                Logger.LogWarning("[MathQuiz] Enjoy the plugin! ;)");
            }
            else
            {
                Logger.LogError("[MathQuiz] MathQuiz: DISABLED. Check configuration files!");
                Logger.LogError("[MathQuiz] Unloading plugin...");
                
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
            
            Logger.LogWarning("[MathQuiz] Plugin unloaded successfully!");
        }
        public override TranslationList DefaultTranslations =>
            new TranslationList()
            {
                {"mathquiz_broadcast_question", "[MathQuiz] '{0} {2} {1} = ?' Know the question and earn {3} {4} '/re <answer>'"},
                {"mathquiz_broadcast_winner", "[MathQuiz] {0} answered the question correctly and earned {2} {3}! Answer: {1}"},
                {"mathquiz_true_answer", "[MathQuiz] Congratulations! You have earned {0} {1} by answering correctly! Your new balance: {2} {1}"},
                {"mathquiz_wrong_answer", "[MathQuiz] Wrong answer!"},
                {"mathquiz_invalid_parameter", "[MathQuiz] Usage: '/re <answer>' or '/answer <answer>'"},
                {"mathquiz_no_question", "[MathQuiz] No active questions yet!"},
                {"mathquiz_disabled", "[MathQuiz] Disabled!"}
            };

        private IEnumerator<WaitForSeconds> AutoRepeat()
        {
            while (Conf.Enabled)
            {
                AutoQuiz();
                yield return new WaitForSeconds(Conf.IntervalInSeconds);
            }
        }
        
        internal Quiz CurrentQuiz;
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

                RandomizeQuiz(out CurrentQuiz, out Symbol, ref Number1, ref Number2, ref Result);

                if (CurrentQuiz.RewardType == ERewardType.Uconomy)
                {
                    ExecuteDependencyCode("Uconomy", plugin =>
                    {
                        var uconomy = (Uconomy)plugin;
                        var broadcast = Translate("mathquiz_broadcast_question", Number1.ToString(), Number2.ToString(), Symbol, CurrentQuiz.RewardAmount.ToString(), uconomy.Configuration.Instance.MoneyName);
                        ChatManager.serverSendMessage(broadcast, Color.yellow, null, null, EChatMode.GLOBAL, Conf.AnnouncerImageUrl, true);
                    });
                }
                else
                {
                    var broadcast = Translate("mathquiz_broadcast_question", Number1.ToString(), Number2.ToString(), Symbol, CurrentQuiz.RewardAmount.ToString(), "Experience");
                    ChatManager.serverSendMessage(broadcast, Color.yellow, null, null, EChatMode.GLOBAL, Conf.AnnouncerImageUrl, true);
                }
                NoQuestion = false;
            }
            catch (Exception ex)
            {
                Logger.LogError("[MathQuiz] Error: " + ex);
            }
        }
        private void RandomizeQuiz(out Quiz selectedQuiz, out string symbol, ref int number1, ref int number2,
            ref int result)
        {
            selectedQuiz = null;
            var totalChance = Conf.Quizzes.Sum(q => q.Chance);
            //var totalChance = Conf.Quizzes.Aggregate(0, (current, quiz) => current + quiz.Chance);
            symbol = "";
            
            var random = new System.Random();
            int i = 0, diceRoll = random.Next(0, totalChance);
            foreach (var quiz in Conf.Quizzes)
            {
                if (diceRoll > i && diceRoll <= i + quiz.Chance)
                {
                    selectedQuiz = quiz;
                    break;
                }
                i += quiz.Chance;
            }

            if (selectedQuiz == null)
            {
                Logger.LogWarning("[MathQuiz] Warning: Please setup MathQuiz configuration files first!");
                Unload();
                return;
            }
            
            number1 = random.Next(selectedQuiz.MinNumber, selectedQuiz.MaxNumber);
            number2 = random.Next(selectedQuiz.MinNumber, selectedQuiz.MaxNumber);
            switch (selectedQuiz.Type)
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
                    var num1 = Convert.ToSingle(random.Next(selectedQuiz.MinNumber, selectedQuiz.MaxNumber));
                    var num2 = Convert.ToSingle(random.Next(selectedQuiz.MinNumber, selectedQuiz.MaxNumber));
                    var res = num1 / num2;
                    while (res % 1 != 0)
                    {
                        num1 = Convert.ToSingle(random.Next(selectedQuiz.MinNumber, selectedQuiz.MaxNumber));
                        num2 = Convert.ToSingle(random.Next(selectedQuiz.MinNumber, selectedQuiz.MaxNumber));
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
