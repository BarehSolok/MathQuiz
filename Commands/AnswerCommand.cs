using System;
using System.Collections.Generic;
using fr34kyn01535.Uconomy;
using RFMathQuiz.Models;
using Rocket.API;
using Rocket.Core.Logging;
using Rocket.Core.Plugins;
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
        public string Syntax => "/answer <integer>";
        public List<string> Aliases => new List<string> { "ans", "re" };
        public List<string> Permissions => new List<string> { "answer" };
        public void Execute(IRocketPlayer caller, string[] command)
        {
            try
            {
                var player = (UnturnedPlayer)caller;
                if (Plugin.NoQuestion)
                {
                    var noQuestion = Plugin.Inst.Translate("mathquiz_no_question");
                    ChatManager.serverSendMessage(noQuestion, UnturnedChat.GetColorFromName(Plugin.Conf.UnfavorableMessageColor, Color.red), null, ((UnturnedPlayer)caller).SteamPlayer(), EChatMode.SAY, Plugin.Conf.AnnouncerImageUrl, true);
                }
                else
                {
                    if (command.Length >= 1)
                    {
                        if (command[0] == Plugin.Result.ToString() && !Plugin.NoQuestion)
                        {
                            Plugin.NoQuestion = true;
                            if (Plugin.Inst.CurrentQuizModel.RewardType == ERewardType.Uconomy)
                            {
                                RocketPlugin.ExecuteDependencyCode("Uconomy", (IRocketPlugin plugin) =>
                                {
                                    var uconomy = (Uconomy)plugin;
                                    uconomy.Database.IncreaseBalance(player.CSteamID.ToString(), Plugin.Inst.CurrentQuizModel.RewardAmount);
                                    var rightanswer = Plugin.Inst.Translate("mathquiz_true_answer", Plugin.Inst.CurrentQuizModel.RewardAmount.ToString(), uconomy.Configuration.Instance.MoneyName, uconomy.Database.GetBalance(player.CSteamID.ToString()));
                                    ChatManager.serverSendMessage(rightanswer, UnturnedChat.GetColorFromName(Plugin.Conf.MessageColor, Color.yellow), null, player.SteamPlayer(), EChatMode.SAY, Plugin.Conf.AnnouncerImageUrl, true);
                                    var winner = Plugin.Inst.Translate("mathquiz_broadcast_winner", caller.DisplayName, Plugin.Result.ToString(), Plugin.Inst.CurrentQuizModel.RewardAmount.ToString(), uconomy.Configuration.Instance.MoneyName);
                                    ChatManager.serverSendMessage(winner, UnturnedChat.GetColorFromName(Plugin.Conf.MessageColor, Color.yellow), null, null, EChatMode.GLOBAL, Plugin.Conf.AnnouncerImageUrl, true);
                                });
                            }
                            else
                            {
                                player.Experience += Plugin.Inst.CurrentQuizModel.RewardAmount;
                                var correctAnswer = Plugin.Inst.Translate("mathquiz_true_answer", Plugin.Inst.CurrentQuizModel.RewardAmount.ToString(), "Experience", player.Experience);
                                ChatManager.serverSendMessage(correctAnswer, UnturnedChat.GetColorFromName(Plugin.Conf.MessageColor, Color.yellow), null, player.SteamPlayer(), EChatMode.SAY, Plugin.Conf.AnnouncerImageUrl, true);
                                var winner = Plugin.Inst.Translate("mathquiz_broadcast_winner", caller.DisplayName, Plugin.Result.ToString(), Plugin.Inst.CurrentQuizModel.RewardAmount.ToString(), "Experience");
                                ChatManager.serverSendMessage(winner, UnturnedChat.GetColorFromName(Plugin.Conf.MessageColor, Color.yellow), null, null, EChatMode.GLOBAL, Plugin.Conf.AnnouncerImageUrl, true);
                            }
                        }
                        if (command[0] != Plugin.Result.ToString() && !Plugin.NoQuestion) ChatManager.serverSendMessage(Plugin.Inst.Translate("mathquiz_wrong_answer"), UnturnedChat.GetColorFromName(Plugin.Conf.UnfavorableMessageColor, Color.red), null,
                            ((UnturnedPlayer)caller).SteamPlayer(), EChatMode.SAY, Plugin.Conf.AnnouncerImageUrl, true);
                    }
                    else ChatManager.serverSendMessage(Plugin.Inst.Translate("mathquiz_invalid_parameter"), UnturnedChat.GetColorFromName(Plugin.Conf.MessageColor, Color.yellow), null,
                        ((UnturnedPlayer)caller).SteamPlayer(), EChatMode.SAY, Plugin.Conf.AnnouncerImageUrl, true);
                }
            }
            catch (Exception err)
            {
                Logger.LogError("[RFMathQuiz] Error: " + err);
            }
        }
    }
}