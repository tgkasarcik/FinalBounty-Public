using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrades;
using PlayerObject;


public class TouchUpgradeHandler : MonoBehaviour
{

    [SerializeField] public IUpgrade upgrade;
    
    private AudioManager _audioManager;

    void Awake()
    {
        _audioManager = FindObjectOfType<AudioManager>();
    }

    private void OnTriggerEnter(Collider collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            IPlayer player = PlayerManager.FindPlayerByObject(collision.gameObject);
            player.inventory.AddUpgrade(upgrade);
            HUDNotifications.MakeNotification("Collected " + upgrade.name + "!");
            _audioManager.PlayItemPickup();
            //destroys waypoint first
            Waypoint waypointOnObj = this.gameObject.GetComponent<Waypoint>();
            if(waypointOnObj != null)
            {
                Destroy(waypointOnObj);
            }

            Destroy(this.gameObject);

            if(player.onPlanet)
            {
                ProgressManager.GetInstance().SetPlanetCleared();
                PlanetRoomManager.ExitPlanet();
            }
        }
    }
}
