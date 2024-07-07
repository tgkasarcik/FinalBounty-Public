using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrencyManagerLegacy : MonoBehaviour
{
    [SerializeField] private int StartingCurrency;
    private int currency;

	private void Start()
	{
		currency = StartingCurrency;
	}

	/// <summary>
	/// Get the current amount of currency
	/// </summary>
	/// <returns></returns>
	public int GetCurrency()
	{
		return currency;
	}

	/// <summary>
	/// Spend the speified amount of currency, if possible based on the player's current amount of currency.
	/// </summary>
	/// <param name="amount"></param>
	/// <returns>True if currency can be spent successfully, False otherwise</returns>
	public bool SpendCurrency(int amount)
	{
		bool ret = false;
		if (currency >= amount)
		{
			currency -= amount;
			ret = true;
		}
		return ret;
	}

	/// <summary>
	/// Add the specified amount of currency to the player's current currency amount.
	/// </summary>
	/// <param name="amount"></param>
	public void AddCurrency(int amount)
	{
		currency += amount;
	}
}
