﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EnemyStatsClass : FighterStatsClass
{
    public int PointsToGain = 10;
    internal DoorControl lockedDoor;

    private IGameController GameCtr;

    #region Setup

    public void SetUpEnemyStats(IGameController ctr)
    {
        base.SetUpStats();
        GameCtr = ctr;
    }

    #endregion Setup

    #region Health
    public override void HandleDeath()
    {
        base.HandleDeath();
        if (GameCtr != null)
        {
            PlayerStatsClass player = GameCtr.GetPlayerStats();
            if (player != null)
            {
                player.ModifyPoints(PointsToGain);
            }
            if (GameCtr.GetCurrentEnemies() != null)
            {
                Enemy enemy = GameCtr.GetCurrentEnemies()[0];
                GameCtr.HandleDeath(enemy.transform, enemy.DeathParticle, enemy.DeathParticleLength, new Vector3(0, 0, 0));
                SoundEffectControl sfx = GameCtr.GetSFXControl();
                if (sfx != null)
                {
                    sfx.EnemyDeath();
                }

                if (lockedDoor != null)
                {
                    ItemDrop key = lockedDoor.OnEnemyDied();
                    if (key != null)
                    {
                        ItemDrop droppedKey = GameObject.Instantiate(key, enemy.transform.position, enemy.transform.rotation);
                        droppedKey.door = lockedDoor;
                    }
                    else
                    {
                        enemy.DropRandomItem();
                    }
                }
                else
                {
                    enemy.DropRandomItem();
                }
            }
        }
    }
    #endregion Health

    #region Attack

    public override void ShowDodge()
    {
        if (GameCtr != null)
        {
            GameCtr.ReactToDodge(dodged, DodgeDuration);

            SoundEffectControl sfx = GameCtr.GetSFXControl();
            if (sfx != null)
            {
                sfx.EnemyDodged();
            }
        }
    }

    public override bool AttackOpponent(FighterStatsClass opponent, bool CanBeDodged = true, bool ignoreTurnTime = false)
    {
        if (GameCtr != null)
        {
            if (!GameCtr.TakesPartInCurrentBattle(this))
            {
                Debug.LogError("Enemy that is not part of the current battle tried to attack the player!");
            }
        }
        
        return base.AttackOpponent(opponent, CanBeDodged, ignoreTurnTime);
    }

    #endregion Attack
}
