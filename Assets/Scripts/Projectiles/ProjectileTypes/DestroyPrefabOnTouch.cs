using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyPrefabOnTouch : MonoBehaviour
{
    [SerializeField] public bool piercing = false;
    [SerializeField] public bool spectral = false;
    [SerializeField] public string TagToCheck;

    private List<string> tagsToDestroyOn = new List<string> { "Obsticle", "Planet", "Sun", "Boss" };

    private void OnTriggerEnter(Collider collision)
    {
        //Wont destroy when the player rolls
        if(!(collision.gameObject.tag == "Player" && PlayerManager.FindPlayerByObject(collision.gameObject).rolling))
        {
            if (collision.gameObject.tag == TagToCheck)
            {
                //destroy projectile and damage enem
                if (!piercing)
                {
                    Destroy(gameObject);
                }
            }
            else if (!spectral && tagsToDestroyOn.Contains(collision.gameObject.tag))
            {
                Destroy(gameObject);
            }
        }

    }
}
