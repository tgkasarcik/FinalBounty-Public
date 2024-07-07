using PlanetGen;
using PlayerObject;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class RedDamagePlayer : MonoBehaviour
{
    private Color damagedColor = new Color(1f, 0f, 0f, 1f);

    private Material gameObjectMaterial;
    private Color originalColor;
    private float originalRimPower;

    private float damagedColorTime = .5f;
    private float damagedColorTimer = 0f;

    private bool isURPShader;

    void OnEnable()
    {
        Renderer gameObjectRenderer = GetComponent<Renderer>();
        gameObjectMaterial = gameObjectRenderer.material;
        if(gameObjectMaterial.HasProperty("_Color"))
        {
            //is a urp shader
            isURPShader = true;
            originalColor = gameObjectMaterial.color;
        }
        else
        {
            //assumes its the polugon spaceship shader
            isURPShader = false;
            originalRimPower = gameObjectMaterial.GetFloat("_RimPower");
        }
        
        damagedColor = new Color(1f, 0f, 0f, 1f);
    }

    public void TurnRed()
    {
        StopAllCoroutines();
        if(isURPShader)
        {
            StartCoroutine(ChangeColor());
        }
        else
        {
            StartCoroutine(ChangeRimColor());
        }
    }

    //makes object turn red and back again progressively over a timer amount
    private IEnumerator ChangeColor()
    {
        float TimeForEachSection = damagedColorTime / 2;
        gameObjectMaterial.color = damagedColor;
        damagedColorTimer = 0;
        while (damagedColorTimer < TimeForEachSection)
        {
            gameObjectMaterial.color = Color.Lerp(damagedColor, originalColor, damagedColorTimer);
            damagedColorTimer += Time.deltaTime;
            yield return null;
        }
        damagedColorTimer = 0;
        gameObjectMaterial.color = originalColor;
    }

    //makes object turn red and back again progressively over a timer amount
    //this method is used for objects with the polygon shader b/c it doesnt have full body color
    private IEnumerator ChangeRimColor()
    {
        float TimeForEachSection = damagedColorTime / 2;
        float rimPowerScale = 1 / TimeForEachSection;
        gameObjectMaterial.SetFloat("_RimPower", 0);

        damagedColorTimer = 0;
        while (damagedColorTimer < TimeForEachSection)
        {
            gameObjectMaterial.SetFloat("_RimPower", 0 + (damagedColorTimer * rimPowerScale * originalRimPower));
            damagedColorTimer += Time.deltaTime;
            yield return null;
        }
        gameObjectMaterial.SetFloat("_RimPower", originalRimPower);
    }
}
