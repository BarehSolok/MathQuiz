using System;
using System.Collections.Generic;
using System.Linq;
using RFMathQuiz.Enums;
using RFMathQuiz.Managers;
using Rocket.API;
using Rocket.API.Collections;
using Rocket.Core.Plugins;
using RFMathQuiz.Models;
using RFMathQuiz.Utils;
using RFRocketLibrary.Helpers;
using RFRocketLibrary.Hooks;
using Rocket.Unturned.Chat;
using SDG.Unturned;
using UnityEngine;
using Color = UnityEngine.Color;
using Logger = Rocket.Core.Logging.Logger;

namespace RFMathQuiz
{
    public class Plugin : RocketPlugin<Configuration>
    {
        private static int Major = 1;
        private static int Minor = 1;
        private static int Patch = 0;

        internal static Plugin Inst;
        internal static Configuration Conf;
        
        internal static Color MsgColor;
        internal static Color UnfavorMsgColor;

        private static Coroutine _quizCor;
        internal static bool NoQuestion;
        internal static Quiz CurrentQuiz;
        internal static int Number1, Number2, Result;
        internal static string Symbol;

        protected override void Load()
        {
            Inst = this;
            Conf = Configuration.Instance;

            if (Conf.Enabled)
            {
                MsgColor = UnturnedChat.GetColorFromName(Conf.MessageColor, Color.green);
                UnfavorMsgColor = UnturnedChat.GetColorFromName(Conf.UnfavorableMessageColor, Color.red);

                if (UconomyHook.CanBeLoaded())
                    UconomyHook.Load();

                if (AviEconomyHook.CanBeLoaded())
                    AviEconomyHook.Load();

                _quizCor = StartCoroutine(QuizUtil.AutoRepeat());
            }
            else
            {
                Logger.LogError($"[{Name}] Plugin: DISABLED");
            }

            Logger.LogWarning($"[{Name}] Plugin loaded successfully!");
            Logger.LogWarning($"[{Name}] {Name} v{Major}.{Minor}.{Patch}");
            Logger.LogWarning($"[{Name}] Made with 'rice' by RiceField Plugins!");
        }

        protected override void Unload()
        {
            Inst = null;
            Conf = null;
            if (_quizCor != null)
            {
                StopAllCoroutines();
                _quizCor = null;
            }

            Logger.LogWarning($"[{Name}] Plugin unloaded successfully!");
        }

        public override TranslationList DefaultTranslations =>
            new TranslationList()
            {
                {
                    $"{EResponse.BROADCAST_QUESTION}",
                    "[RFMathQuiz] '{0} {2} {1} = ??' Know the question and earn {3} {4} '/re <answer>'"
                },
                {
                    $"{EResponse.BROADCAST_WINNER}",
                    "[RFMathQuiz] {0} answered the question correctly and earned {2} {3}! Answer: {1}"
                },
                {
                    $"{EResponse.TRUE_ANSWER}",
                    "[RFMathQuiz] Congratulations! You have earned {0} {1} by answering correctly! Your new balance: {2} {1}"
                },
                {$"{EResponse.WRONG_ANSWER}", "[RFMathQuiz] Wrong answer!"},
                {$"{EResponse.INVALID_PARAMETER}", "[RFMathQuiz] Usage: {0}"},
                {$"{EResponse.NO_QUESTION}", "[RFMathQuiz] No active questions yet!"},
                {$"{EResponse.DISABLED}", "[RFMathQuiz] Disabled!"}
            };
    }
}