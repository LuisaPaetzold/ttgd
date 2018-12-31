using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EnemyStatsClass : FighterStatsClass
{
    public int PointsToGain = 10;

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
            }
            
        }
    }
    #endregion Health

    #region Attack

    public override void ShowDodge()
    {
        if (GameCtr != null)
        {
            GameCtr.ReactToDodge(dodged);
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
