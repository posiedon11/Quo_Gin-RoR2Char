using Quo_Gin.Componenets;
using Quo_Gin.SkillStates;
using R2API;
using RoR2;
using RoR2.Projectile;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Networking;

namespace Quo_Gin.Modules
{
    internal static class Projectiles
    {
        internal static GameObject bombPrefab;
        internal static GameObject solarGrenadePrefab;
        internal static GameObject solarGrenadePulsePrefab;
        internal static GameObject wellOfRadiancePrefab;
        internal static GameObject sunShotPrefab;
        internal static void RegisterProjectiles()
        {
            CreateBomb();
            createWellOfRadiance();
            createSolarPulse();
            createSolarGrenade();


            AddProjectile(wellOfRadiancePrefab);
            AddProjectile(bombPrefab);
            AddProjectile(solarGrenadePrefab);
            AddProjectile(solarGrenadePulsePrefab);

        }

        internal static void AddProjectile(GameObject projectileToAdd)
        {
            Content.AddProjectilePrefab(projectileToAdd);
        }

        private static void createWellOfRadiance()
        {
            wellOfRadiancePrefab = CloneProjectilePrefab("SporeGrenadeProjectileDotZone", "WellOfradianceZone");
            UnityEngine.Object.Destroy(wellOfRadiancePrefab.GetComponent<ProjectileDotZone>());
            wellOfRadiancePrefab.AddComponent<DestroyOnTimer>().duration = WellOfRadiance.wellDuration;
            
           // wellOfRadiancePrefab.transform.localScale = Vector3.one * WellOfRadiance.landingRadius;
            float range = WellOfRadiance.landingRadius;
            float temp = 0f;
            range = Mathf.SmoothDamp(wellOfRadiancePrefab.transform.localScale.x, range, ref temp, .2f);
            wellOfRadiancePrefab.transform.localScale = Vector3.one * range * 2;


            BuffWard wellBuff = wellOfRadiancePrefab.AddComponent<BuffWard>();
            wellBuff.radius = WellOfRadiance.landingRadius;
            wellBuff.interval = .25f;
            wellBuff.buffDef = Buffs.wellOfRadianceBuff;
            wellBuff.buffDuration = WellOfRadiance.wellBuffduration;
            wellBuff.expires = true;
            wellBuff.expireDuration = WellOfRadiance.wellDuration;
            wellBuff.floorWard= true;
            wellBuff.animateRadius = false;

            HealingWard wellHeal = wellOfRadiancePrefab.AddComponent<HealingWard>();
            wellHeal.radius = WellOfRadiance.landingRadius;
            wellHeal.interval = WellOfRadiance.healingPulseBaseFrequency / WellOfRadiance.healingPulseSpeedStat;
            wellHeal.healFraction = WellOfRadiance.healingCoefficient;
            wellHeal.floorWard = true;
            //wellHeal.rangeIndicator.localScale = new Vector3(range, range, range);

            //GameObject wellEffect = CloneProjectilePrefab("SporeGrenadeProjectileDotZone", "WellOfRadianceEffect");
            //wellEffect.transform.parent = wellOfRadiancePrefab.transform;
            //wellEffect.transform.localPosition = Vector3.zero;
            //wellEffect.transform.localScale = Vector3.one * WellOfRadiance.landingRadius;

            //UnityEngine.Object.Destroy(wellOfRadiancePrefab.transform.GetChild(0).gameObject);


        }
        private static void createSolarGrenade()
        {
            Log.Message("Creating Solar grenade Proj");
            Projectiles.solarGrenadePrefab = CloneProjectilePrefab("CommandoGrenadeProjectile", "QuoQinSolarGrenadeProjectile");

            ProjectileController solarGrenadeController = solarGrenadePrefab.GetComponent<ProjectileController>();
            solarGrenadePrefab.GetComponent<TeamFilter>().teamIndex = TeamIndex.Player;
            solarGrenadeController.ghostPrefab = CreateGhostPrefab("HenryBombGhost");
           // UnityEngine.Object.Destroy(solarGrenadePulsePrefab.GetComponent<ProjectileDotZone>());

            ProjectileDamage solardamage = solarGrenadePrefab.GetComponent<ProjectileDamage>();
            InitializeProjectilDamage(solardamage);

            


            //if (Assets.mainAssetBundle.LoadAsset<GameObject>("SolarGrenadeGhost") != null) solarGrenadeController.ghostPrefab = CreateGhostPrefab("SolarGrenadeGhost");
            solarGrenadeController.startSound = "";


            ProjectileImpactExplosion solarGrenadeExplosion = solarGrenadePrefab.GetComponent<ProjectileImpactExplosion>();
             InitializeImpactExplosion(solarGrenadeExplosion);

            solarGrenadePrefab.AddComponent<DamageAPI.ModdedDamageTypeHolderComponent>().Add(Quo_GinPlugin.ScorchMark);

            //solarGrenadeExplosion.destroyOnEnemy = true;
            //solarGrenadeExplosion.destroyOnWorld = true;
            solarGrenadeExplosion.impactEffect = Assets.bombExplosionEffect;
            solarGrenadeExplosion.timerAfterImpact = true;
            solarGrenadeExplosion.lifetime = 12f;
            solarGrenadeExplosion.blastRadius = SolarGrenade.damageRadius;
            solarGrenadeExplosion.lifetimeAfterImpact = 0f;
            solarGrenadeExplosion.fireChildren = true;
            solarGrenadeExplosion.childrenCount = 1;
            solarGrenadeExplosion.childrenProjectilePrefab = Projectiles.solarGrenadePulsePrefab;
            solarGrenadeExplosion.childrenDamageCoefficient = 1f;
            solarGrenadeExplosion.falloffModel = BlastAttack.FalloffModel.None;

            
            


        }   
        private static void createSolarPulse()
        {
            Log.Message("Creating Solar grenade Pulse Proj");
            Projectiles.solarGrenadePulsePrefab = CloneProjectilePrefab("SporeGrenadeProjectileDotZone", "QuoQinSolarGrenadePulse");
            UnityEngine.Object.Destroy(solarGrenadePulsePrefab.GetComponent<ProjectileDotZone>());
            UnityEngine.Object.Destroy(solarGrenadePulsePrefab.transform.GetChild(0).gameObject);
            solarGrenadePulsePrefab.AddComponent<DamageAPI.ModdedDamageTypeHolderComponent>().Add(Quo_GinPlugin.ScorchMark);
            solarGrenadePulsePrefab.AddComponent<SolarGrenadePulseController>();;
            //ProjectileImpactExplosion explosion = solarGrenadePulsePrefab.GetComponent<ProjectileImpactExplosion>();
            //InitializeImpactExplosion(explosion);
            //solarGrenadePulsePrefab.GetComponent<Rigidbody>().velocity = Vector3.zero;
            solarGrenadePulsePrefab.AddComponent<DestroyOnTimer>().duration = SolarGrenade.duration;
            //solarGrenadePulsePrefab.transform.parent = solarGrenadePulsePrefab.transform;b);


        }
        private static void CreateBomb()
        {
            bombPrefab = CloneProjectilePrefab("CommandoGrenadeProjectile", "HenryBombProjectile");

            ProjectileImpactExplosion bombImpactExplosion = bombPrefab.GetComponent<ProjectileImpactExplosion>();
            InitializeImpactExplosion(bombImpactExplosion);

            bombImpactExplosion.blastRadius = 16f;
            bombImpactExplosion.destroyOnEnemy = true;
            bombImpactExplosion.lifetime = 12f;
            bombImpactExplosion.impactEffect = Assets.bombExplosionEffect;
            //bombImpactExplosion.lifetimeExpiredSound = Modules.Assets.CreateNetworkSoundEventDef("HenryBombExplosion");
            bombImpactExplosion.timerAfterImpact = true;
            bombImpactExplosion.lifetimeAfterImpact = 0.1f;



            ProjectileController bombController = bombPrefab.GetComponent<ProjectileController>();
            if (Assets.mainAssetBundle.LoadAsset<GameObject>("HenryBombGhost") != null) bombController.ghostPrefab = CreateGhostPrefab("HenryBombGhost");
            bombController.startSound = "";


        }


        private static void InitializeProjectilDamage(ProjectileDamage projectileDamage) 
        {
            projectileDamage.crit = false;
            projectileDamage.damage = 0f;
            projectileDamage.damageColorIndex = DamageColorIndex.Default;
            projectileDamage.damageType = DamageType.Generic;
            projectileDamage.force = 0f;
        }
        private static void InitializeImpactExplosion(ProjectileImpactExplosion projectileImpactExplosion)
        {
            projectileImpactExplosion.blastDamageCoefficient = 1f;
            projectileImpactExplosion.blastProcCoefficient = 1f;
            projectileImpactExplosion.blastRadius = 1f;
            projectileImpactExplosion.bonusBlastForce = Vector3.zero;
            projectileImpactExplosion.childrenCount = 0;
            projectileImpactExplosion.childrenDamageCoefficient = 0f;
            projectileImpactExplosion.childrenProjectilePrefab = null;
            projectileImpactExplosion.destroyOnEnemy = false;
            projectileImpactExplosion.destroyOnWorld = false;
            projectileImpactExplosion.falloffModel = BlastAttack.FalloffModel.None;
            projectileImpactExplosion.fireChildren = false;
            projectileImpactExplosion.impactEffect = null;
            projectileImpactExplosion.lifetime = 0f;
            projectileImpactExplosion.lifetimeAfterImpact = 0f;
            projectileImpactExplosion.lifetimeRandomOffset = 0f;
            projectileImpactExplosion.offsetForLifetimeExpiredSound = 0f;
            projectileImpactExplosion.timerAfterImpact = false;
            projectileImpactExplosion.bonusBlastForce = new Vector3(0f, -500f, 0f);

            projectileImpactExplosion.GetComponent<ProjectileDamage>().damageType = DamageType.Generic;
        }

        private static GameObject CreateGhostPrefab(string ghostName)
        {
            GameObject ghostPrefab = Assets.mainAssetBundle.LoadAsset<GameObject>(ghostName);
            if (!ghostPrefab.GetComponent<NetworkIdentity>()) ghostPrefab.AddComponent<NetworkIdentity>();
            if (!ghostPrefab.GetComponent<ProjectileGhostController>()) ghostPrefab.AddComponent<ProjectileGhostController>();

            Assets.ConvertAllRenderersToHopooShader(ghostPrefab);

            return ghostPrefab;
        }

        private static GameObject CloneProjectilePrefab(string prefabName, string newPrefabName)
        {
            GameObject newPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/Projectiles/" + prefabName).InstantiateClone(newPrefabName);
            return newPrefab;
        }
    }
}