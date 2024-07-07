using PlanetGen;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/*
 
public class SolarSystemBase
{


    public GenPlanet CreatePlanet(int seed, Vector3 position, int index, float scale = -1.0f)
    {
        //create planet sphere with random scale and mass
        double mass = ((this.maxPlanetMass - this.minPlanetMass) * levelRandom.NextDouble() + this.minPlanetMass);

        //if no scale given then generate a random scale for planet
        if (scale == -1.0)
        {
            scale = (float)((this.maxPlanetSize - this.minPlanetSize) * levelRandom.NextDouble() + this.minPlanetSize);
        }

        //generate random rotation speed
        double planetRotSpeed = ((this.maxPlanetRotationSpeed - this.minPlanetRotationSpeed) * levelRandom.NextDouble() + this.minPlanetRotationSpeed);
        planetRotSpeed *= planetRotationSpeedModifier;

        //calculate oribital speed (https://www.physicsclassroom.com/class/circles/Lesson-4/Mathematics-of-Satellite-Motion#:~:text=As%20seen%20in%20the%20equation,the%20orbit%20affect%20orbital%20speed)
        double orbitSpeed = Math.Sqrt((GRAVITATIONAL_CONSTANT * sunMass) / Vector3.Distance(sunPos, position));
        orbitSpeed *= oribitSpeedModifier;

        GameObject prefab = generateRandomPrefab(PlanetPrefabs);

        //calc gavity https://emandpplabs.nscee.edu/cool/temporary/doors/space/planets/find/findFg.html
        double gravity = (GRAVITATIONAL_CONSTANT * mass) / Math.Pow((scale / 2), 2);


        //create genplanet object with parameters
        GenPlanet planet = new GenPlanet(prefab, this.enemySpawnObjects[index], this.enemySpawnPercentages[index], mass, planetRotSpeed, orbitSpeed, (scale / 2), scale, gravity, seed);

        return planet;
    }
}
*/