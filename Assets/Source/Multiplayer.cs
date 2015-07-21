﻿using Caveman.Players;
using Caveman.Utils;
using UnityEngine;

namespace Caveman.Network
{
    public class Multiplayer : EnterPoint, IServerListener
    {
        public PlayerModelBase prefabServerPlayer;

        public const float WidthMapServer = 1350;
        public const float HeigthMapServer = 1350;

        public override void Start()
        {
            serverConnection = new ServerConnection {ServerListener = this};
            serverConnection.StartSession(SystemInfo.deviceUniqueIdentifier, SystemInfo.deviceName);
            base.Start();
            serverConnection.SendRespawn(poolPlayers[IdHostPlayer].transform.position);
            poolPlayers.SetPrefab(prefabServerPlayer);
        }

        public void Update()
        {
            serverConnection.Update();
        }

        public void WeaponAddedReceived(Vector2 point)
        {
            poolStones.New(point).transform.position = UnityExtensions.ConvectorCoordinate(point); Debug.Log("stone added : " + point);
        }

        public void BonusAddedReceived(Vector2 point)
        {
            print(string.Format("BonusAddedReceived {0}", point));
        }

        public void PlayerDeadResceived(string playerId, Vector2 point)
        {
            //todo нужна команды умереть 
            print(string.Format("PlayerDeadResceived {0}", point));
        }

        public void WeaponRemovedReceived(Vector2 point)
        {
            poolStones.Store(point); print(string.Format("WeaponRemovedReceived {0}", point));
        }

        public void MoveReceived(string playerId, Vector2 point)
        {
            print(string.Format("MoveReceived {0} by playerId {1}", point, playerId));
        }

        public void LoginReceived(string playerId)
        {
            print(string.Format("LoginReceived {0} by playerId", playerId));
        }

        public void PickWeaponReceived(string playerId, Vector2 point)
        {
            //todo только один тип - камни 
            poolPlayers[playerId].PickupWeapon(poolStones[UnityExtensions.GenerateKey(point)]); print(string.Format("PickWeaponReceived {0} by playerId {1}", point, playerId));
        }

        public void PickBonusReceived(string playerId, Vector2 point)
        {
            poolPlayers[playerId].PickupBonus(poolBonusesSpeed[UnityExtensions.GenerateKey(point)]); print(string.Format("PickBonusReceived {0} by playerId {1}", point, playerId));
        }

        public void UseWeaponReceived(string playerId, Vector2 point)
        {
            poolPlayers[playerId].Throw(point); Debug.Log(string.Format("UseWeaponReceived {0} by playerId {1}", point, playerId));
        }

        public void RespawnReceived(string playerId, Vector2 point)
        {
            //todo playerId null
            var hack = "123";
            poolPlayers.New(hack).transform.position = point; Debug.Log(string.Format("RespawnReceived {0} by playerId {1}", point, playerId));
        }

        public void OnDestroy()
        {
            serverConnection.StopSession();
        }
    }
}
