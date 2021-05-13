using System;
using System.Collections.Generic;
using fr34kyn01535.Uconomy;
using MathQuiz.Models;
using Rocket.API;
using Rocket.Core.Logging;
using Rocket.Core.Plugins;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Color = UnityEngine.Color;

namespace MathQuiz.Commands
{
    public class AnswerCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;
        public string Name => "answer";
        public string Help => "Answer MathQuiz";
        public string Syntax => "/answer <integer>";
        public List<string> Aliases => new List<string> { "ans", "re" };
        public List<string> Permissions => new List<string> { "answer" };
        public void Execute(IRocketPlayer caller, string[] command)
        {
            try
            {
                var player = (UnturnedPlayer)caller;
                if (Main.NoQuestion)
                {
                    var noQuestion = Main.Inst.Translate("mathquiz_no_question");
                    ChatManager.serverSendMessage(noQuestion, UnturnedChat.GetColorFromName(Main.Conf.UnfavorableMessageColor, Color.red), null, ((UnturnedPlayer)caller).SteamPlayer(), EChatMode.SAY, Main.Conf.AnnouncerImageUrl, true);
                }
                else
                {
                    if (command.Length >= 1)
                    {
                        if (command[0] == Main.Result.ToString() && !Main.NoQuestion)
                        {
                            Main.NoQuestion = true;
                            if (Main.Inst.CurrentQuiz.RewardType == ERewardType.Uconomy)
                            {
                                RocketPlugin.ExecuteDependencyCode("Uconomy", (IRocketPlugin plugin) =>
                                {
                                    var uconomy = (Uconomy)plugin;
                                    uconomy.Database.IncreaseBalance(player.CSteamID.ToString(), Main.Inst.CurrentQuiz.RewardAmount);
                                    var rightanswer = Main.Inst.Translate("mathquiz_true_answer", Main.Inst.CurrentQuiz.RewardAmount.ToString(), uconomy.Configuration.Instance.MoneyName, uconomy.Database.GetBalance(player.CSteamID.ToString()));
                                    ChatManager.serverSendMessage(rightanswer, UnturnedChat.GetColorFromName(Main.Conf.MessageColor, Color.yellow), null, player.SteamPlayer(), EChatMode.SAY, Main.Conf.AnnouncerImageUrl, true);
                                    var winner = Main.Inst.Translate("mathquiz_broadcast_winner", caller.DisplayName, Main.Result.ToString(), Main.Inst.CurrentQuiz.RewardAmount.ToString(), uconomy.Configuration.Instance.MoneyName);
                                    ChatManager.serverSendMessage(winner, UnturnedChat.GetColorFromName(Main.Conf.MessageColor, Color.yellow), null, null, EChatMode.GLOBAL, Main.Conf.AnnouncerImageUrl, true);
                                });
                            }
                            else
                            {
                                player.Experience += Main.Inst.CurrentQuiz.RewardAmount;
                                var correctAnswer = Main.Inst.Translate("mathquiz_true_answer", Main.Inst.CurrentQuiz.RewardAmount.ToString(), "Experience", player.Experience);
                                ChatManager.serverSendMessage(correctAnswer, UnturnedChat.GetColorFromName(Main.Conf.MessageColor, Color.yellow), null, player.SteamPlayer(), EChatMode.SAY, Main.Conf.AnnouncerImageUrl, true);
                                var winner = Main.Inst.Translate("mathquiz_broadcast_winner", caller.DisplayName, Main.Result.ToString(), Main.Inst.CurrentQuiz.RewardAmount.ToString(), "Experience");
                                ChatManager.serverSendMessage(winner, UnturnedChat.GetColorFromName(Main.Conf.MessageColor, Color.yellow), null, null, EChatMode.GLOBAL, Main.Conf.AnnouncerImageUrl, true);
                            }
                        }
                        if (command[0] != Main.Result.ToString() && !Main.NoQuestion) ChatManager.serverSendMessage(Main.Inst.Translate("mathquiz_wrong_answer"), UnturnedChat.GetColorFromName(Main.Conf.UnfavorableMessageColor, Color.red), null,
                            ((UnturnedPlayer)caller).SteamPlayer(), EChatMode.SAY, Main.Conf.AnnouncerImageUrl, true);
                    }
                    else ChatManager.serverSendMessage(Main.Inst.Translate("mathquiz_invalid_parameter"), UnturnedChat.GetColorFromName(Main.Conf.MessageColor, Color.yellow), null,
                        ((UnturnedPlayer)caller).SteamPlayer(), EChatMode.SAY, Main.Conf.AnnouncerImageUrl, true);
                }
            }
            catch (Exception err)
            {
                Logger.LogError("[MathQuiz] Error: " + err);
            }
        }
    }
}