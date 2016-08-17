﻿using System.Collections;
using Caveman.Configs;
using Caveman.Players;
using Caveman.Pools;
using Caveman.Weapons;
using UnityEngine;

namespace Caveman.Bonuses
{
    public class BonusBase : ASupportPool<BonusBase>
    {
        protected float preValue;
        protected ObjectPool<BonusBase> pool;

        public BonusConfig Config { protected set; get; } 

        public void OnTriggerEnter2D(Collider2D other)
        {
            if (Time.time < 1) return; // todo подумать. 
            if (other.gameObject.GetComponent<PlayerModelBase>())
            {
                Effect(other.gameObject.GetComponent<PlayerModelBase>());
            }
            if (other.gameObject.GetComponent<WeaponModelBase>())
            {
                pool.Store(this);
            }
        }

        public override void SetPool(ObjectPool<BonusBase> item)
        {
            pool = item;
        }

        public virtual void Effect(PlayerModelBase playerModel)
        {
            // HACK: trigger methods calling before Start
            if (playerModel.PlayerCore == null)
                return;
            if (Config == null)
                return;
            playerModel.PlayerCore.ActivatedBonus(Config.Type, Config.Duration);
            //todo внедрить систему событий
            playerModel.bonusBase = this;
            transform.position = new Vector3(200, 200, 200);
            StartCoroutine(UnEffect(playerModel));
        }

        protected virtual IEnumerator UnEffect(PlayerModelBase playerModel)
        {
            yield return null;
        }
    }
}