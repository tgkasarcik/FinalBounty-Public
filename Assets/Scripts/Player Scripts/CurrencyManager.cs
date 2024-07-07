using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerObject;

public static class CurrencyManager
{
    private static int StartingCurrency;
    private static Dictionary<IPlayer, int> playerCurrencies;

	public static void Initialize(int StartingCurrencyTemp = 100)
    {
        StartingCurrency = StartingCurrencyTemp;
        playerCurrencies = new Dictionary<IPlayer, int>();

        //inits each players starting value
        foreach(IPlayer player in PlayerManager.players)
        {
            playerCurrencies.Add(player, StartingCurrency);
        }
    }

	/// <summary>
	/// Get the current amount of currency
	/// </summary>
	/// <returns></returns>
	public static int GetCurrency(IPlayer player)
	{
		return playerCurrencies[player];
	}

	/// <summary>
	/// Spend the speified amount of currency, if possible based on the player's current amount of currency.
	/// </summary>
	/// <param name="amount"></param>
	/// <returns>True if currency can be spent successfully, False otherwise</returns>
	public static bool SpendCurrency(IPlayer player, int amount)
	{
		bool ret = false;
		if (playerCurrencies[player] >= amount)
		{
			playerCurrencies[player] -= amount;
			ret = true;
		}
		return ret;
	}

	/// <summary>
	/// Add the specified amount of currency to the player's current currency amount.
	/// </summary>
	/// <param name="amount"></param>
	public static void AddCurrency(IPlayer player, int amount)
	{
		playerCurrencies[player] += amount;
	}
}