using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using PlayerObject;

public class HealthBar : MonoBehaviour
{

    [SerializeField] public UnityEngine.UI.Slider healthSlider;
    [SerializeField] public GameObject victoryScreen;

    private void Awake()
    {
        BossHealth.bossHealth += ChangeHealth;
        BossHealth.bossVictory += DisplayVictoryScreen;
    }

    private void OnDisable()
    {
        BossHealth.bossHealth -= ChangeHealth;
        BossHealth.bossVictory -= DisplayVictoryScreen;
    }

    private void ChangeHealth(float health)
    {
        healthSlider.value = health;
    }

    private void DisplayVictoryScreen(bool victory)
    {
        victoryScreen.SetActive(victory);
        if(victory)
        {
            //give players money on win
            foreach(IPlayer player in PlayerManager.players)
            {
                CurrencyManager.AddCurrency(player, 250);
            }
        }
    }



}
