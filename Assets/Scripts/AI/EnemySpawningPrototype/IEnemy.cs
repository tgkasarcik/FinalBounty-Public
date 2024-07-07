using PlanetGen;
using PlayerObject;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public abstract class IEnemy : MonoBehaviour
{
    public float Health;
    public float impactDamage = 10f;
    public int moneyOnDrop = 5;
    public bool onFire {get; private set;}
    public float fireDamageAmount;
    private IPlayer playersFireDamage;
    private float flameTimer = 0f;
    private float flameTime = 1f;
    private int flameTicks = 5;

    private bool isURPShader;
    private bool isColorChangeRunning;

    private Coroutine changeColorCoroutine;
    private Coroutine changeRimColorCoroutine;

    private float damagedColorTime = .5f;
    private float damagedColorTimer = 0f;

    public GameObject ExplosionFX;

    private Material gameObjectMaterial;
    private Color originalColor;
    private Color damagedColor;
    private float originalRimPower;

    public Cell StartCell;
    public GenPlanet MyPlanet;
    private AudioManager _audioManager;

    public virtual void Awake()
    {
        Health = 10f;
        onFire = false;
        fireDamageAmount = .5f;
        isColorChangeRunning = false;
        moneyOnDrop = 10;
        ExplosionFX = (GameObject)Resources.Load("Prefabs/Particles/FX_Explosion");
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
    public virtual void OnEnable()
    {
        //add to enemy counter
        EnemyCounter.AddEnemyCount();
        _audioManager = FindObjectOfType<AudioManager>();
    }
    public virtual void OnDisable()
    {
        //subtract from enemy counter
        EnemyCounter.MinusEnemyCount();
    }

    //makes enemy turn red and back again progressively over a timer amount
    private IEnumerator ChangeColor()
    {
        float TimeForEachSection = damagedColorTime / 2;
        //Color colorRightNow = gameObjectMaterial.color;
        // while (damagedColorTimer < TimeForEachSection)
        // {
        //     gameObjectMaterial.color = Color.Lerp(colorRightNow, damagedColor, damagedColorTimer);
        //     damagedColorTimer += Time.deltaTime;
        //     yield return null;
        // }
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

    //makes enemy turn red and back again progressively over a timer amount
    //used for enemies with the polygon shader b/c it doesnt have full body color
    private IEnumerator ChangeRimColor()
    {
        isColorChangeRunning = true;
        //float currRimPower = gameObjectMaterial.GetFloat("_RimPower");
        float TimeForEachSection = damagedColorTime / 2;
        float rimPowerScale = 1 / TimeForEachSection;
        // while (damagedColorTimer < TimeForEachSection / 2)
        // {
        //     gameObjectMaterial.SetFloat("_RimPower", currRimPower - (damagedColorTimer * rimPowerScale * 2 * currRimPower));
        //     damagedColorTimer += Time.deltaTime;
        //     yield return null;
        // }
        gameObjectMaterial.SetFloat("_RimPower", 0);

        damagedColorTimer = 0;
        while (damagedColorTimer < TimeForEachSection)
        {
            gameObjectMaterial.SetFloat("_RimPower", 0 + (damagedColorTimer * rimPowerScale * originalRimPower));
            damagedColorTimer += Time.deltaTime;
            yield return null;
        }
        gameObjectMaterial.SetFloat("_RimPower", originalRimPower);
        isColorChangeRunning = false;
    }

    public virtual void Wander()
    {

    }

    public virtual void SetStartCell(Cell cell)
    {
        this.StartCell = cell;
    }


    public virtual void SetStartPlanet(GenPlanet planet)
    {
        this.MyPlanet = planet;
    }

    /*
     * Chase a player. Will automatically fire when close
     * 
     */
    public virtual void Chase(IPlayer player)
    {

    }

    public virtual void TakeDamage(float damageAmount, IPlayer player)
    {
        this.Health -= damageAmount;
        if (this.Health < 0)
        {
            //gives player money if the enemy object is destroyed
            CurrencyManager.AddCurrency(player, moneyOnDrop);
            this.DestroySequence();
        }
        else
        {

            if (isURPShader)
            {
                if(this.changeColorCoroutine != null)
                {
                    StopCoroutine(this.changeColorCoroutine);
                }
                this.changeColorCoroutine = StartCoroutine(ChangeColor());
            }
            else
            {
                if(this.changeRimColorCoroutine != null)
                {
                    StopCoroutine(this.changeRimColorCoroutine);
                }
                this.changeRimColorCoroutine = StartCoroutine(ChangeRimColor());
            }
        }
    }

    public virtual void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Enemy") {
            Physics.IgnoreCollision(other.collider, GetComponent<Collider>());
        }
        if (other.gameObject.tag == "Player")
        {
            IPlayer tempPlayer = PlayerManager.FindPlayerByObject(other.gameObject);
            tempPlayer.TakeDamage(impactDamage);
            DestroySequence();
        } 
    }

    public virtual void DestroySequence() 
    {
        Destroy(gameObject);
        _audioManager.PlayRandomExplosion();
        Instantiate(ExplosionFX, transform.position, transform.rotation);
    }

    public virtual void Update()
    {
        //ticks health for fire damage if on fire
        TakeFireDamage();

    }

    public virtual void LightOnFire(float fireDamageAmount, IPlayer playerWhoLitOnFire)
    {
        onFire = true;
        flameTicks = 5;
        this.fireDamageAmount = fireDamageAmount;
        this.playersFireDamage = playerWhoLitOnFire;
    }

    public void TakeFireDamage()
    {
        if(onFire)
        {
            flameTimer += Time.deltaTime;
            if(flameTimer >= flameTime)
            {
                TakeDamage(this.fireDamageAmount, playersFireDamage);
                flameTimer -= flameTime;
                flameTicks--;
            }

            if(flameTicks <= 0)
            {
                onFire = false;
                int childCount = this.transform.childCount;
                for(int i = 0; i < childCount; i++)
                {
                    GameObject child = this.transform.GetChild(i).gameObject;
                    if(child.GetComponent<ParticleSystem>() != null)
                    {
                        Destroy(child); 
                    }
                }
            }
        }
    }

}
