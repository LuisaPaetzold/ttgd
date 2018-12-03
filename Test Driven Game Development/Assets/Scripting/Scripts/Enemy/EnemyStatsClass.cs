using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EnemyStatsClass : FighterStatsClass
{
    public int PointsToGain = 10;

    internal IGameController GameCtr;

    #region Setup

    public void SetUpEnemy(IGameController ctr)
    {
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
}
