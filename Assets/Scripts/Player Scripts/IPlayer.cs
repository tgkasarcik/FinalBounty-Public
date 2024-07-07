using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrades;

namespace PlayerObject
{
    public interface IPlayer
    {
        static event EventHandler GameOverEvent;

        GameObject prefab {get;set;}

        string name {get;set;}

        IUpgradeInventory inventory {get;set;}

        //gun ammo
        int rocketAmmo {get;set;}
        int maxRocketAmmo {get;set;}

        int beamAmmo {get;set;}
        int maxBeamAmmo {get;set;}

        bool haveBeam {get;set;}

        //player stats
        int lives{get;set;}
        float Health{get;set;}
        float movementSpeed {get;set;}
        float currentHealth{get;set;}
        float playerSize{get;set;}
        bool onFire {get;set;}

        float spaceMovementSpeed { get;set;}

        //projectile stats
        float damageOut {get;set;}
        float damageOutPercent {get;set;}
        float projectileSize {get;set;}
        float fireRate {get;set;}
        float fireRatePercent {get;set;}
        float projectileTime {get;set;}
        int shotNumberOutputted {get;set;}
        int projectileFanCount {get;set;}
        float shootFOV {get;set;}
        float projectileSpeed {get;set;}

        //different shot modifiers
        bool flameShot {get;set;}
        float flameDamage {get;set;}
        bool homingShot {get;set;}
        bool gravityShot {get;set;}
        float sensingDistance {get;set;}
        bool pierceShot {get;set;}
        bool spectralShot {get;set;}
        bool controledShot {get;set;}
       bool lightningShot {get;set;}


        //misc

        public bool invunerable { get; set; }
        public bool rolling { get; set; }
        bool onPlanet {get;set;}
        bool bossBattle {get;set;}

        PlayerProjectiles projectiles {get;set;}

        GameObject playerObj {get;set;}
        MovementWithAbilities playerMovement {get;set;}
        List<GameObject> projectilePrefabs {get;set;}

        GameObject CreatePlayerObj(Vector3 position, Quaternion rotation);
        void InitPlayerAttributes();
        void SetOnPlanet(bool result);

        void TakeDamage(float damage);
    }
}
