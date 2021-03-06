using System;
using System.Collections.Generic;
using Caveman.Bonuses;
using Caveman.CustomAnimation;
using Caveman.DevSetting;
using Caveman.Weapons;
using Caveman.Weapons.Melee;
using JetBrains.Annotations;
using Caveman.Weapons.Ranged;
using UnityEngine;

namespace Caveman.Pools
{
    public class PoolsManager : MonoBehaviour
    {
        [SerializeField] private Transform containerStones;
        [SerializeField] private Transform containerSplashStones;
        [SerializeField] private Transform containerSkulls;
        [SerializeField] private Transform containerImagesDeath;
        [SerializeField] private Transform containerBonusesSpeed;
        [SerializeField] private Transform containerSword;

        public ObjectPool<ImageBase> ImagesDeath { private set; get; }
        public ObjectPool<BonusBase> BonusesSpeed { private set; get; }
        public ObjectPool<WeaponModelBase> Stones { private set; get; }
        public ObjectPool<WeaponModelBase> Sword { private set; get; }

        // todo miss prefab skull in resource
        private ObjectPool<WeaponModelBase> Skulls { set; get; }
        private ObjectPool<WeaponModelBase> Axes { set; get; }
        private ObjectPool<ImageBase> SplashesStone { set; get; }

        [HideInInspector] public readonly Dictionary<string, object> Pools = new Dictionary<string, object>();

        public Transform ContainerStones => containerStones;

        private bool multiplayer;

        public void InitializationPools(GameConfigs settings, bool isMultiplayer)
        {
            multiplayer = isMultiplayer;
            var poolsConfig = settings.Pool["sample"];

            var deathConfig = settings.Image["death"];
            var splahesConfig = settings.Image["stones_fragment"];
            var stonesConfig = settings.Weapon["stone"];
            //var axesConfig = settings.Weapon["axe"];
            //var skullsConfig = settings.Weapon["skulls"];

            var bonusSpeedConfig = settings.Bonus["speed"];
            var swordConfig = settings.Weapon["sword"];


            ImagesDeath = PreparePool(containerImagesDeath, Inst<ImageBase>(deathConfig.PrefabPath),
                poolsConfig.ImagesOrdinary, null);
            SplashesStone = PreparePool(containerSplashStones, Inst<ImageBase>(splahesConfig.PrefabPath),
                poolsConfig.ImagesPopular, null);
            //Skulls = PreparePool(containerSkulls, Inst<WeaponModelBase>(skullsConfig.PrefabPath), poolsConfig.WeaponsOrdinary, InitializationPoolAxe, isMultiplayer);
            Stones = PreparePool(containerStones, Inst<WeaponModelBase>(stonesConfig.PrefabPath),
                poolsConfig.WeaponsPopular, InitializationPoolStone);
            //Axes = PreparePool(Inst<WeaponModelBase>(axesConfig.PrefabPath), poolsConfig.BonusesOrdinary);
            BonusesSpeed = PreparePool(containerBonusesSpeed, Inst<BonusBase>(bonusSpeedConfig.PrefabPath),
                poolsConfig.BonusesOrdinary,
                InitializationPoolBonus);
                print("start pool sword");
            Sword = PreparePool(containerSword, Inst<WeaponModelBase>(swordConfig.PrefabPath), poolsConfig.WeaponsRare,
                InitializationPoolSword);
            print(Sword);

            Stones.RelatedPool += () => SplashesStone;

            // for map 
            Pools.Add(stonesConfig.PrefabPath, Stones);
            //Pools.Add(axesConfig.PrefabPath, Axes);
            Pools.Add(bonusSpeedConfig.PrefabPath, BonusesSpeed);
            Pools.Add(swordConfig.PrefabPath, Sword);
        }

        private T Inst<T>(string prefabPath) where T : MonoBehaviour
        {
            return Instantiate(Resources.Load(prefabPath, typeof(T))) as T;
        }

        private ObjectPool<T> PreparePool<T>(Transform container, T prefab, int initialBufferSize, [CanBeNull] Action<GameObject, ObjectPool<T>> init)
            where T : MonoBehaviour
        {
            var pool = container.GetComponent<ObjectPool<T>>();
            pool.Initialization(prefab, initialBufferSize, multiplayer);

            for (var i = 0; i < initialBufferSize; i++)
            {
                AddItem(pool, init, container, Instantiate(prefab));
            }
            AddItem(pool, init, container, prefab);
            return pool;
        }

        private void AddItem<T>(ObjectPool<T> pool, [CanBeNull] Action<GameObject, ObjectPool<T>> init, Transform container, T item) where T : MonoBehaviour
        {
            init?.Invoke(item.gameObject, pool);
            item.transform.SetParent(container.transform);
            pool.Store(item);
        }

        /// <summary>
        /// Each model assigned reference on objectpool
        /// </summary>
        private void InitializationPoolBonus(GameObject item, ObjectPool<BonusBase> pool)
        {
            item.GetComponent<BonusBase>().InitializationPool(pool);
        }

        private void InitializationPoolSword(GameObject item, ObjectPool<WeaponModelBase> pool)
        {
            item.GetComponent<SwordModel>().InitializationPool(pool);
        }

        private void InitializationPoolAxe(GameObject item, ObjectPool<WeaponModelBase> pool)
        {
            item.GetComponent<AxeModel>().InitializationPool(pool);
        }

        private void InitializationPoolStone(GameObject item, ObjectPool<WeaponModelBase> pool)
        {
            var model = item.GetComponent<StoneModel>();
            model.InitializationPool(pool);
            model.InitializationPoolSplashesStone(SplashesStone);
        }
    }
}
