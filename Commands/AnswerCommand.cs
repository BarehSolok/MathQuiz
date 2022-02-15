using System;
using System.Collections.Generic;
using RFMathQuiz.Enums;
using RFMathQuiz.Managers;
using RFMathQuiz.Models;
using RFMathQuiz.Utils;
using RFRocketLibrary.Helpers;
using RFRocketLibrary.Hooks;
using Rocket.API;
using Rocket.Core.Logging;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Color = UnityEngine.Color;

namespace RFMathQuiz.Commands
{
    public class AnswerCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;
        public string Name => "answer";
        public string Help => "Answer RFMathQuiz";
        public string Syntax => "/re <answer>' or '/answer <answer>";
        public List<string> Aliases => new List<string> {"ans", "re"};
        public List<string> Permissions => new List<string> {"answer"};

        public void Execute(IRocketPlayer caller, string[] command)
        {
            try
            {
                var player = (UnturnedPlayer) caller;
                if (Plugin.NoQuestion)
                {
                    var noQuestion = QuizUtil.TranslateRich(EResponse.NO_QUESTION.ToString());
                    ChatHelper.Broadcast(noQuestion, Plugin.UnfavorMsgColor, Plugin.Conf.MessageIconUrl);
                }
                else
                {
                    if (command.Length >= 1)
                    {
                        if (command[0] == Plugin.Result.ToString() && !Plugin.NoQuestion)
                        {
                            Plugin.NoQuestion = true;

                            var currentQuiz = Plugin.CurrentQuiz;
                            var currentBalance = BalanceManager.Increase(currentQuiz.Reward, player.CSteamID.m_SteamID,
                                currentQuiz.RewardAmount);
                            var moneySymbol = BalanceManager.GetMoneySymbol(currentQuiz.Reward);
                            var rightanswer = QuizUtil.TranslateRich(EResponse.TRUE_ANSWER.ToString(),
                                currentQuiz.RewardAmount.ToString(), moneySymbol, currentBalance);
                            ChatHelper.Say(player, rightanswer, Plugin.MsgColor, Plugin.Conf.MessageIconUrl);

                            var winner = QuizUtil.TranslateRich(EResponse.BROADCAST_WINNER.ToString(),
                                caller.DisplayName,
                                Plugin.Result.ToString(), currentQuiz.RewardAmount.ToString(), moneySymbol);
                            ChatHelper.Broadcast(winner, Plugin.MsgColor, Plugin.Conf.MessageIconUrl);
                        }

                        if (command[0] != Plugin.Result.ToString() && !Plugin.NoQuestion)
                            ChatHelper.Say(caller, QuizUtil.TranslateRich(EResponse.WRONG_ANSWER.ToString()),
                                Plugin.UnfavorMsgColor, Plugin.Conf.MessageIconUrl);
                    }
                    else
                        ChatHelper.Say(caller, QuizUtil.TranslateRich(EResponse.INVALID_PARAMETER.ToString(), Syntax),
                            Plugin.MsgColor, Plugin.Conf.MessageIconUrl);
                }
            }
            catch (Exception exception)
            {
                Logger.LogError($"[{Plugin.Inst.Name}] [ERROR] Details: " + exception);
            }
        }
    }
}