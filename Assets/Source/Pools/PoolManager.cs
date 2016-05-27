﻿using Caveman.Bonuses;
using Caveman.CustomAnimation;
using Caveman.Setting;
using Caveman.Configs;
using Caveman.Weapons;
using UnityEngine;

namespace Caveman.Pools
{
    public class PoolManager : MonoBehaviour
    {
        public static PoolManager instance;

        public AxeModel prefabAxe;
        public StoneModel prefabStone;
        public StoneSplash prefabStoneFlagmentInc;
        public EffectBase prefabDeathImage;
        public SpeedBonus prefabBonusSpeed;

        public Transform containerStones;
        public Transform containerSplashStones;
        public Transform containerSkulls;
        public Transform containerDeathImages;
        //public Transform containerPlayers;
        public Transform containerBonusesSpeed;

        public ObjectPool<EffectBase> SplashesStone { private set; get; }
        public ObjectPool<EffectBase> ImagesDeath { private set; get; }
        public ObjectPool<EffectBase> Axes { private set; get; }
        public ObjectPool<WeaponModelBase> Stones { private set; get; }
        public ObjectPool<WeaponModelBase> Skulls { private set; get; }
        public ObjectPool<BonusBase> BonusesSpeed { private set; get; }

        public void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
        }

        public void PrepareAllPools(CurrentGameSettings currentSettings)
        {
            ImagesDeath = PreparePool<EffectBase>(, containerDeathImages, prefabDeathImage);
            SplashesStone = PreparePool<EffectBase>()
        }

        /// Used object pool pattern
        private ObjectPool<T> PreparePool<T>(int initialBufferSize, Transform container, T prefab) where T : MonoBehaviour
        {
            var pool = container.GetComponent<ObjectPool<T>>();
            pool.CreatePool(prefab, initialBufferSize);
            for (var i = 0; i < initialBufferSize; i++)
            {
                var item = Instantiate(prefab);

                item.transform.SetParent(container);
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
