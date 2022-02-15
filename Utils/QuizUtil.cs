using System;
using System.Collections.Generic;
using System.Linq;
using RFMathQuiz.Enums;
using RFMathQuiz.Managers;
using RFMathQuiz.Models;
using RFRocketLibrary.Helpers;
using Rocket.API;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;
using Random = UnityEngine.Random;

namespace RFMathQuiz.Utils
{
    public static class QuizUtil
    {
        internal static IEnumerator<WaitForSeconds> AutoRepeat()
        {
            while (Plugin.Conf.Enabled)
            {
                AutoQuiz();
                yield return new WaitForSeconds(Plugin.Conf.IntervalInSeconds);
            }
        }

        internal static string TranslateRich(string s, params object[] objects)
        {
            return Plugin.Inst.Translate(s, objects).Replace("-=", "<").Replace("=-", ">");
        }

        private static void AutoQuiz()
        {
            if (!Plugin.Conf.Enabled)
                return;

            try
            {
                if (Plugin.Conf.IntervalInSeconds == 0)
                    return;

                RandomizeQuiz(out Plugin.CurrentQuiz, out Plugin.Symbol, ref Plugin.Number1, ref Plugin.Number2,
                    ref Plugin.Result);
                var broadcast = TranslateRich(EResponse.BROADCAST_QUESTION.ToString(), Plugin.Number1.ToString(),
                    Plugin.Number2.ToString(), Plugin.Symbol, Plugin.CurrentQuiz.RewardAmount.ToString(),
                    BalanceManager.GetMoneySymbol(Plugin.CurrentQuiz.Reward));
                ChatHelper.Broadcast(broadcast, Plugin.MsgColor, Plugin.Conf.MessageIconUrl);

                Plugin.NoQuestion = false;
            }
            catch (Exception ex)
            {
                Logger.LogError($"[{Plugin.Inst.Name}] [ERROR] Details: " + ex);
            }
        }

        private static void RandomizeQuiz(out Quiz selectedQuiz, out string symbol, ref int number1, ref int number2,
            ref int result)
        {
            try
            {
                if (Plugin.Conf.Quizzes.Count == 0)
                {
                    selectedQuiz = null;
                    symbol = null;

                    Logger.LogWarning(
                        $"[{Plugin.Inst.Name}] Warning: Please setup RFMathQuiz configuration files first!");
                    return;
                }

                selectedQuiz = null;
                symbol = "";
                var totalChance = Plugin.Conf.Quizzes.Sum(q => q.Chance);
                var random = new System.Random();
                int i = 0, roll = random.Next(1, totalChance + 1);
                foreach (var quiz in Plugin.Conf.Quizzes)
                {
                    if (roll > i && roll <= i + quiz.Chance)
                    {
                        selectedQuiz = quiz;
                        break;
                    }

                    i += quiz.Chance;
                }

                if (selectedQuiz == null)
                {
                    RandomizeQuiz(out selectedQuiz, out symbol, ref number1, ref number2, ref result);
                    return;
                }

                number1 = random.Next(selectedQuiz.MinNumber, selectedQuiz.MaxNumber);
                number2 = random.Next(selectedQuiz.MinNumber, selectedQuiz.MaxNumber);
                switch (selectedQuiz.Type)
                {
                    case EQuiz.ADDITION:
                        symbol = "+";
                        result = number1 + number2;
                        break;
                    case EQuiz.SUBTRACTION:
                        symbol = "-";
                        result = number1 - number2;
                        break;
                    case EQuiz.MULTIPLICATION:
                        symbol = "×";
                        result = number1 * number2;
                        break;
                    case EQuiz.DIVISION:
                        symbol = "÷";
                        var num1 = Convert.ToSingle(number1);
                        var num2 = Convert.ToSingle(number2);
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
            catch (Exception e)
            {
                Logger.LogError($"[{Plugin.Inst.Name}] [ERROR] Details: {e}");
                throw;
            }
        }
    }
}