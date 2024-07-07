using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class GravityBody : MonoBehaviour
{

    Gravity planet;
    Rigidbody rbody;
    public Vector3 normal;
    public float distanceFromSurface;
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
        rbody = GetComponent<Rigidbody>();

        normal = planet.normal;
        distanceFromSurface = planet.distanceToSurface;

        // Disable rigidbody gravity and rotation as this is simulated in GravityAttractor script
        rbody.useGravity = false;
        rbody.constraints = RigidbodyConstraints.FreezeRotation;
        
    }

    void LateUpdate()
    {
        // Allow this body to be influenced by planet's gravity
        planet.Orbit(this.gameObject, true, shouldRotate);
        normal = planet.normal;
        distanceFromSurface = planet.distanceToSurface;
    }

    public void SetPlanetObj(GameObject planet)
    {
        planetObj = planet;
        normal = planet.GetComponent<Gravity>().normal;
    }
}