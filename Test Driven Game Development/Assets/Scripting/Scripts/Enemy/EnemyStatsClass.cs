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

    #endregion Attack
}
