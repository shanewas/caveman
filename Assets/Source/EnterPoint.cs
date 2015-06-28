﻿using System;
using System.Collections;
using Caveman.Level;
using Caveman.Players;
using Caveman.Setting;
using Caveman.Utils;
using Caveman.Weapons;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

namespace Caveman
{
    public class EnterPoint : MonoBehaviour
    {
        private const string PrefabPlayer = "player";
        private const string PrefabBot = "bot";
        private const string PrefabText = "Text";

        public CNAbstractController movementJoystick;
        public Transform prefabPauseBtn;
        public Transform prefabSkull;
        public Transform prefabStoneFlagmentInc;
        public Transform prefabStone;
        public Transform prefabDeathImage;

        public GameObject windowPause;

        private readonly string[] names = { "Kiracosyan", "IkillU", "skaska", "loser", "yohoho", "shpuntik" };

        public SmoothCamera smoothCamera;
        public Transform containerStonesPool;
        public Transform containerStoneSplashPool;
        public Transform containerSkullsPool;
        public Transform containerDeathImagesPool;
        public Transform containerPlayerPool;
        public Transform result;
        public Text roundTime;
        public Text weapons;
        public Text killed;
       
        private Player humanPlayer;
        private Random r;
        private bool flagEnd;

        private ObjectPool poolStones;
        private ObjectPool poolSkulls;
        private ObjectPool poolStonesSplash;
        private ObjectPool poolDeathImage;
        private PlayerPool poolPlayers;

        public void Start()
        {
            r = new Random();
            Player.idCounter = 0;
            
            poolStonesSplash = CreatePool(Settings.PoolCountSplashStones, containerStoneSplashPool, prefabStoneFlagmentInc, null);
            poolStones = CreatePool(Settings.PoolCountStones, containerStonesPool, prefabStone, InitStoneModel);
            poolSkulls = CreatePool(Settings.PoolCountSkulls, containerSkullsPool, prefabSkull, InitSkullModel);
            poolDeathImage = CreatePool(Settings.PoolCountDeathImages, containerDeathImagesPool, prefabDeathImage, null);

            poolStones.RelatedPool += () => poolStonesSplash;

            poolPlayers = containerPlayerPool.GetComponent<PlayerPool>();
            poolPlayers.Init(Settings.BotsCount + 1);
            CreatePlayer(new Player("Zabiyakin"), false);
            for (var i = 0; i < Settings.BotsCount; i++)
            {
                CreatePlayer(new Player(names[i]), true);
            }

            PutWeapons();

            humanPlayer.WeaponsCountChanged += WeaponsCountChanged;
            humanPlayer.KillsCountChanged += KillsCountChanged;

            Invoke("PutWeapons", Settings.TimeRespawnWeapon);
        }

        public void PauseGame()
        {
            Time.timeScale = 0.000001f;
            windowPause.SetActive(true);
            //TODO stop all animations
        }

        public void Update()
        {
            var remainTime = Settings.RoundTime - Math.Floor(Time.timeSinceLevelLoad);
            int m = Convert.ToInt32(remainTime / 60);
            int s = Convert.ToInt32(remainTime % 60);
            if (m < 60)
            {
                m = 0;
            }
                       
            //var displayTime = remainTime > 60 ? "1 : " + (remainTime - 60) : remainTime.ToString();
            var displayTime = m + ":" + s;
            roundTime.text = displayTime;
            if (remainTime < 0 && !flagEnd)
            {
                flagEnd = true; 
                result.gameObject.SetActive(true);
                StartCoroutine(DisplayResult());
            }
        }

        private void PutWeapons()
        {
            for (var i = 0; i < Settings.InitialLyingWeapons; i++)
            {
                PutWeapon(poolStones);
            }
            for (var i = 0; i < Settings.CountLyingSkulls; i++)
            {
                PutWeapon(poolSkulls);
            }
            Invoke("PutWeapons", Settings.TimeRespawnWeapon);
        }

        private void PutWeapon(ObjectPool pool)
        {
            var weapon = pool.New();
            StartCoroutine(FadeIn(weapon.GetComponent<SpriteRenderer>()));
            weapon.transform.position = new Vector2(r.Next(-Settings.Br, Settings.Br),
                r.Next(-Settings.Br, Settings.Br));
        }

        private ObjectPool CreatePool(int initialBufferSize, Transform container, Transform prefab, Action<GameObject, ObjectPool> init)
        {
            var pool = container.GetComponent<ObjectPool>();
            pool.CreatePool(prefab, initialBufferSize);
            for (var i = 0; i < initialBufferSize; i++)
            {
                var item = Instantiate(prefab);
                if (init != null)
                {
                    init(item.gameObject, pool);
                }
                item.SetParent(container);
                pool.Store(item.transform);
            }
            return pool;
        }

        private void InitSkullModel(GameObject item, ObjectPool pool)
        {
            item.GetComponent<SkullModel>().SetPool(pool);
        }

        private void InitStoneModel(GameObject item, ObjectPool pool)
        {
            var model = item.GetComponent<StoneModel>();
            model.SetPool(pool);
            model.SetPoolSplash(poolStonesSplash);
        }

        private IEnumerator DisplayResult()
        {
            yield return new WaitForSeconds(1f);
            Time.timeScale = 0.00001f;

            var namePlayer = result.GetChild(1);
            var kills = result.GetChild(2);
            var deaths = result.GetChild(3);
            var axisY = -20;
            const int deltaY = 30;
            for (var i = 0; i < Settings.BotsCount + 1; i++)
            {
                Write(poolPlayers.GetName(i), namePlayer, axisY);
                Write(poolPlayers.GetKills(i), kills, axisY);
                Write(poolPlayers.GetDeaths(i), deaths, axisY);
                axisY -= deltaY;
            }
        }

        private void WeaponsCountChanged(int count)
        {
            weapons.text = count.ToString();
        }

        private void KillsCountChanged(int count)
        {
            killed.text = count.ToString();
        }

        private void Write(string value, Transform parent, int axisY)
        {
            var goName = Instantiate(Resources.Load(PrefabText, typeof (GameObject))) as GameObject;
            goName.transform.SetParent(parent);
            goName.transform.localPosition = new Vector2(-2, axisY);
            goName.transform.rotation = Quaternion.identity;
            goName.GetComponent<Text>().text = value;
        }

        private void CreatePlayer(Player player, bool isAiPlayer)
        {
            BasePlayerModel playerModel;
            if (isAiPlayer)
            {
                var prefab = Instantiate(Resources.Load(PrefabBot, typeof(GameObject))) as GameObject;
                playerModel = prefab.GetComponent<AiPlayerModel>();
                var ai = (AiPlayerModel) playerModel;
                ai.InitAi(player,
                new Vector2(r.Next(-Settings.Br, Settings.Br), r.Next(-Settings.Br, Settings.Br)), r, poolPlayers, containerStonesPool);
            }
            else
            {
                var prefabPlayer = Instantiate(Resources.Load(PrefabPlayer, typeof(GameObject))) as GameObject;
                humanPlayer = player;
                playerModel = prefabPlayer.GetComponent<PlayerModel>();
                smoothCamera.target = prefabPlayer.transform;
                playerModel.Init(player, Vector2.zero, r, poolPlayers);
                var pl = (PlayerModel) playerModel;
                pl.SetJoystick(movementJoystick);
            }
            poolPlayers.Add(player.id, playerModel);
            playerModel.transform.SetParent(containerPlayerPool);
            playerModel.Respawn += player1 => StartCoroutine(RespawnPlayer(player));
            playerModel.Death += DeathAnimate;
            playerModel.ChangedWeapons += ChangedWeapons;
        }

        private ObjectPool ChangedWeapons(WeaponType weaponType)
        {
            switch (weaponType)
            {
                case WeaponType.Stone:
                    return poolStones;
                case  WeaponType.Skull:
                    return poolSkulls;
            }
            return null;
        }

        private IEnumerator RespawnPlayer(Player player)
        {
            yield return new WaitForSeconds(Settings.TimeRespawnPlayer);
            poolPlayers.New(player.id).RandomPosition();
        }

        private void DeathAnimate(Vector2 position)
        {
            var deathImage = poolDeathImage.New();
            deathImage.position = position;
            var sprite = deathImage.GetComponent<SpriteRenderer>();
            if (sprite)
            {
                StartCoroutine(FadeOut(sprite));
            }
        }

        private IEnumerator FadeOut(SpriteRenderer spriteRenderer)
        {
            for (var i = 1f; i > 0; i -= 0.1f)
            {
                var c = spriteRenderer.color;
                c.a = i;
                spriteRenderer.color = c;
                yield return null;
            }
            poolDeathImage.Store(spriteRenderer.transform);
        }

        private IEnumerator FadeIn(SpriteRenderer spriteRenderer)
        {
            var color = spriteRenderer.color;
            color.a = 0;
            spriteRenderer.color = color;
            for (var i = 0f; i < 1; i += 0.1f)
            {
                if (spriteRenderer)
                {
                    var c = spriteRenderer.color;
                    c.a = i;
                    spriteRenderer.color = c;
                }
                yield return null;
            }
        }
    }
}
