using UnityEngine;
using System.Collections;

public class GravityBodyParticle : MonoBehaviour
{

    Gravity planet;
    ParticleSystem partSystem;
    ParticleSystem.ForceOverLifetimeModule forceSystem;
    public Vector3 normal;
    [SerializeField] public GameObject planetObj;

    //Should rotate effects if the object should orient itself with the planet
    private bool shouldRotate = true;
    public bool ShouldRotate
    {
        get { return shouldRotate; }
        set { shouldRotate = value; }
    }
    void Start()
    {
        planet = planetObj.GetComponent<Gravity>();
        partSystem = GetComponent<ParticleSystem>();
        forceSystem = partSystem.forceOverLifetime;
        forceSystem.enabled = true;
        forceSystem.space = ParticleSystemSimulationSpace.World;

        normal = planet.normal;
        
    }

    void LateUpdate()
    {
        // Allow this body to be influenced by planet's gravity
        normal = planetObj.transform.position - this.transform.position;
        normal.Normalize();
        Vector3 gravityForce = -normal * (planet.gravity * 1000/planet.distanceToSurface);
        //-planet.gravity *
        forceSystem.x = gravityForce.x;
        forceSystem.y = gravityForce.y;
        forceSystem.z = gravityForce.z;
        // planet.OrbitParticle(this.gameObject, true, false);
        // normal = planet.normal;

    }

    public void SetPlanetObj(GameObject planet)
    {
        planetObj = planet;
        normal = planet.GetComponent<Gravity>().normal;
    }
}