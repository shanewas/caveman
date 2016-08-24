﻿using System.Collections.Generic;
using Caveman.Bonuses;
using Caveman.CustomAnimation;
using Caveman.Setting;
using Caveman.Configs;
using Caveman.Weapons;
using UnityEngine;

namespace Caveman.Pools
{
    public class PoolsManager : MonoBehaviour
    {
        public static PoolsManager instance;

        public Transform container;

        public ObjectPool<EffectBase> SplashesStone { private set; get; }
        public ObjectPool<EffectBase> ImagesDeath { private set; get; }
        public ObjectPool<WeaponModelBase> Axes { private set; get; }
        public ObjectPool<WeaponModelBase> Stones { private set; get; }
        // todo miss prefab skull in resource
        public ObjectPool<WeaponModelBase> Skulls { private set; get; }
        public ObjectPool<BonusBase> BonusesSpeed { private set; get; }

        /// <summary>
        /// only for procedure generate 
        /// </summary>
        public readonly Dictionary<string, object> Pools = new Dictionary<string, object>();

        public void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
        }

        //todo also configs pools gameobjects from json
        public void PrepareAllPools(CurrentGameSettings settings)
        {
            var poolsConfig = settings.PoolsConfigs["sample"];

            var deathConfig = settings.ImagesConfigs["death"];
            var splahesConfig = settings.ImagesConfigs["stones_fragment"];
            var stonesConfig = settings.WeaponsConfigs["stone"];
            var axesConfig = settings.WeaponsConfigs["axe"];
            var skullsConfig = settings.WeaponsConfigs["skulls"];
            var bonusSpeedConfig = settings.BonusesConfigs["speed"];

            //ImagesDeath = PreparePool(Inst<EffectBase>(deathConfig.PrefabPath), deathConfig.Name, poolsConfig.ImagesOrdinary);
            //SplashesStone = PreparePool(Inst<EffectBase>(splahesConfig.PrefabPath), splahesConfig.Name, poolsConfig.ImagesPopular);
            //Skulls = PreparePool(Inst<WeaponModelBase>(skullsConfig.PrefabPath), skullsConfig.Name, poolsConfig.WeaponsOrdinary);
            Stones = PreparePool(Inst<WeaponModelBase>(stonesConfig.PrefabPath), stonesConfig.Name, poolsConfig.WeaponsPopular);
            Axes = PreparePool(Inst<WeaponModelBase>(axesConfig.PrefabPath), axesConfig.Name, poolsConfig.BonusesOrdinary);
            BonusesSpeed = PreparePool(Inst<BonusBase>(bonusSpeedConfig.PrefabPath), bonusSpeedConfig.Name, poolsConfig.BonusesOrdinary);

            Pools.Add(deathConfig.PrefabPath, ImagesDeath);
            Pools.Add(splahesConfig.PrefabPath, SplashesStone);
            Pools.Add(stonesConfig.PrefabPath, Stones);
            Pools.Add(axesConfig.PrefabPath, Axes);
            Pools.Add(bonusSpeedConfig.PrefabPath, BonusesSpeed);
        }

        private T Inst<T>(string prefabPath) where T : MonoBehaviour
        {           
            return Instantiate(Resources.Load(prefabPath, typeof (GameObject))) as T;
        }

        /// Used object pool pattern
        private ObjectPool<T> PreparePool<T>(T prefab, string nameContainer, int initialBufferSize)
            where T : MonoBehaviour
        {
            var containerSingle = new GameObject(nameContainer);
            containerSingle.transform.parent = container;

            var pool = containerSingle.AddComponent<ObjectPool<T>>();

            pool.CreatePool(prefab, initialBufferSize);
            for (var i = 0; i < initialBufferSize; i++)
            {
                var item = Instantiate(prefab);
                item.transform.SetParent(container.transform);
                pool.Store(item);
            }
            return pool;
        }

        /// <summary>
        /// When player pickup weapon another type 
        /// </summary>
        public ObjectPool<WeaponModelBase> SwitchPoolWeapons(WeaponConfig.Types type)
        {
            switch (type)
            {
                case WeaponConfig.Types.Stone:
                    return Stones;
                case WeaponConfig.Types.Skull:
                    return Skulls;
            }
            return null;
        }
    }
}
