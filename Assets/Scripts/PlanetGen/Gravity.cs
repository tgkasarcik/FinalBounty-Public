using UnityEngine;

//https://github.com/SebLague/Spherical-Gravity/blob/master/GravityAttractor.cs
public class Gravity : MonoBehaviour
{
    public float gravity = -9.81f;

    public float buffer = 0.01f;

    public Vector3 normal;
    public float distanceToSurface;
    private float currentDistance;
    private float planetSize;

    [SerializeField] public float distance = 10f;
    private float repulsiveStrength = 1000f;
    bool insideAtmosphere = false;

    //Need a better way of calculating the size
    private void Start()
    {
        planetSize = (GetComponent<MeshRenderer>().bounds.size.x / 2);
        distanceToSurface =  planetSize + distance;
        //Debug.Log(temp);
    }

    private void Update()
    {
        distanceToSurface = planetSize + distance;
    }

    //Adds gravitational force. Could be used for simple gravity sim
    public void AddGravitationalForce(Vector3 normal, Rigidbody body)
    {
        //Adds gravitational force
        body.AddForce(normal * -gravity, ForceMode.Acceleration);
    }

    //Adds gravitational force. Could be used for simple gravity sim
    public void AddGravitationalForce(Vector3 normal, ParticleSystem.ForceOverLifetimeModule forceSystem)
    {
        //Adds gravitational force
        Vector3 gravityForce = normal * -gravity;
        AddForceToParticle(gravityForce, forceSystem);
    }

    private void AddForceToParticle(Vector3 force, ParticleSystem.ForceOverLifetimeModule forceSystem)
    {
        forceSystem.x = new ParticleSystem.MinMaxCurve(force.x);
        forceSystem.y = new ParticleSystem.MinMaxCurve(force.y);
        forceSystem.z = new ParticleSystem.MinMaxCurve(force.z);
    }

    //Makes an object orbit a sphere
    //Close Orbit determines how accurate the altiude above the planet is
    public void Orbit(GameObject obj, bool closeOrbit, bool shouldRotate)
    {
        Rigidbody body = obj.GetComponent<Rigidbody>();

        //Gets the normal of the planet
        normal = obj.transform.position - transform.position;
        normal.Normalize();
        
        //Distance from the planet
        currentDistance = (obj.transform.position - transform.position).magnitude;
        
        AddGravitationalForce(-normal, body);

        //Determines if the object is in the atmosphere
        insideAtmosphere = currentDistance < distanceToSurface ? true : false;
        if(insideAtmosphere)
        {
            PushAwayFromSurface(body);
        }
        else
        {
            if(closeOrbit)
            {
                PushTowardsSurface(body);
            }
        }

        //Zeros out the velocity in the normal direction to smooth out orbiting
        if(closeOrbit)
        {
            if (Mathf.Abs(currentDistance - distanceToSurface) > buffer)
            { 
                Vector3 velocityForce = Vector3.Project(body.velocity, normal);
                body.velocity = body.velocity - velocityForce;
            }
        }
        
        //Orients the object if it should be
        if (shouldRotate)
        {
            RotateObject(normal, obj.transform);
        }
        
            
    }

    //for particle orbits
    public void OrbitParticle(GameObject obj, bool closeOrbit, bool shouldRotate)
    {
        ParticleSystem partSystem = obj.GetComponent<ParticleSystem>();
        ParticleSystem.ForceOverLifetimeModule forceSystem = partSystem.forceOverLifetime;
        var velocity = partSystem.velocityOverLifetime;

        //Gets the normal of the planet
        normal = obj.transform.position - transform.position;
        normal.Normalize();
        
        //Distance from the planet
        currentDistance = (obj.transform.position - transform.position).magnitude;
        
        AddGravitationalForce(-normal, forceSystem);

        //Determines if the object is in the atmosphere
        // insideAtmosphere = currentDistance < distanceToSurface ? true : false;
        // if(insideAtmosphere)
        // {
        //     PushAwayFromSurface(forceSystem, velocity);
        // }
        // else
        // {
        //     if(closeOrbit)
        //     {
        //         PushTowardsSurface(forceSystem);
        //     }
        // }

        // //Zeros out the velocity in the normal direction to smooth out orbiting
        // if(closeOrbit)
        // {
        //     if (Mathf.Abs(currentDistance - distanceToSurface) > buffer)
        //     { 
        //         Vector3 vel = new Vector3(velocity.x.constant, velocity.y.constant, velocity.z.constant);
        //         Vector3 velocityForce = Vector3.Project(vel, normal);
        //         Vector3 newVel = vel - velocityForce;
        //         velocity.x = newVel.x;
        //         velocity.y = newVel.y;
        //         velocity.z = newVel.z;
        //     }
        // }
        
        // //Orients the object if it should be
        // if (shouldRotate)
        // {
        //     RotateObject(normal, obj.transform);
        // }
        
            
    }

    //Pushes body away from the planet to make it look like its orbiting
    private void PushAwayFromSurface(Rigidbody rb)
    {
        //Gets the velocity vector of the body relative to the normal
        Vector3 velocityForce = Vector3.Project(rb.velocity, -normal);

        //Creates a repulsive force based on how close the object is to the surface
        float distanceGoal = Mathf.Clamp(distanceToSurface - currentDistance, 0, 1);
        //Avoids wobbling
        if (distanceGoal < 1)
        {
            distanceGoal = 0;
        }

        Vector3 repulsiveForce = normal.normalized * (repulsiveStrength * distanceGoal);

        //Adds the force of gravity in the other direction to cancel out the planets gravity
        rb.AddForce(normal * -gravity, ForceMode.Acceleration);
        //Adds a force relative to the bodys veloctiy in the normal so it doesn't crash
        rb.AddForce(-velocityForce + repulsiveForce);

    }

    //Pushes body away from the planet to make it look like its orbiting
    private void PushAwayFromSurface(ParticleSystem.ForceOverLifetimeModule forceSystem, ParticleSystem.VelocityOverLifetimeModule vel)
    {
        Vector3 velocity = new Vector3(vel.x.constant, vel.y.constant, vel.z.constant);
        //Gets the velocity vector of the body relative to the normal
        Vector3 velocityForce = Vector3.Project(velocity, -normal);

        //Creates a repulsive force based on how close the object is to the surface
        float distanceGoal = Mathf.Clamp(distanceToSurface - currentDistance, 0, 1);
        //Avoids wobbling
        if (distanceGoal < 1)
        {
            distanceGoal = 0;
        }

        Vector3 repulsiveForce = normal.normalized * (repulsiveStrength * distanceGoal);
        

        //Adds the force of gravity in the other direction to cancel out the planets gravity
        AddForceToParticle(normal * -gravity, forceSystem);
        //Adds a force relative to the bodys veloctiy in the normal so it doesn't crash
        AddForceToParticle(-velocityForce + repulsiveForce, forceSystem);

    }

    //Keeps the Object from gainingg escape velocity by making the velocity component
    //on the normal equal to 0
    //Caution - Has a probability of breaking if the speed is too high
    private void PushTowardsSurface(Rigidbody rb)
    { 
        float multiplier = Mathf.Abs(distanceToSurface - currentDistance);
        rb.AddForce(normal * gravity * multiplier, ForceMode.VelocityChange);     
    }

        //Rotates the object to face the normal
        private void RotateObject(Vector3 normal, Transform trans)
    {
        //Gets the rotation of the transforms up vector to be tangent of the sphere
        var upRotation = Quaternion.FromToRotation(trans.up, normal);
        trans.rotation = upRotation * trans.rotation;
    }

    //Keeps the Object from gainingg escape velocity by making the velocity component
    //on the normal equal to 0
    //Caution - Has a probability of breaking if the speed is too high
    private void PushTowardsSurface(ParticleSystem.ForceOverLifetimeModule forceSystem)
    { 
        float multiplier = Mathf.Abs(distanceToSurface - currentDistance);
        AddForceToParticle(normal * gravity * multiplier, forceSystem);
    }

}
