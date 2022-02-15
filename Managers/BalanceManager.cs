using System;
using RFMathQuiz.Enums;
using RFRocketLibrary.Hooks;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;

namespace RFMathQuiz.Managers
{
    internal static class BalanceManager
    {
        internal static decimal Increase(EReward reward, ulong steamId, decimal amount)
        {
            switch (reward)
            {
                case EReward.EXPERIENCE:
                    var player = PlayerTool.getPlayer(new CSteamID(steamId));
                    player.skills.ServerModifyExperience((int) amount);
                    return player.skills.experience;
                case EReward.UCONOMY:
                    return UconomyHook.Deposit(steamId, amount);
                case EReward.AVIECONOMY:
                    return AviEconomyHook.Deposit(UnturnedPlayer.FromCSteamID(new CSteamID(steamId)), amount) ?? 0;
                default:
                    throw new ArgumentOutOfRangeException(nameof(reward), reward, null);
            }
        }

        internal static string GetMoneySymbol(EReward reward)
        {
            switch (reward)
            {
                case EReward.EXPERIENCE:
                    return "XP";
                case EReward.UCONOMY:
                    return UconomyHook.MoneySymbol;
                case EReward.AVIECONOMY:
                    return AviEconomyHook.CurrencySymbol ?? string.Empty;
                default:
                    throw new ArgumentOutOfRangeException(nameof(reward), reward, null);
            }
        }
    }
}