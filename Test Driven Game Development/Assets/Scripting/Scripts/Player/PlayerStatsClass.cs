using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerStatsClass : FighterStatsClass
{
    private IPlayer playerAddition;

    private int currentPoints;

    #region Setup
    public PlayerStatsClass()
    {
        currentPoints = 0;
    }

    public override void SetUpStats(IPlayer player = null)
    {
        base.SetUpStats();
        SetPlayerAddition(player);
    }

    public void SetPlayerAddition(IPlayer addition)
    {
        this.playerAddition = addition;
    }
    #endregion Setup

    #region Points
    public int GetCurrentPoints()
    {
        return currentPoints;
    }

    public void ModifyPoints(int mod)
    {
        currentPoints += mod;
        if (currentPoints < 0)
        {
            currentPoints = 0;
        }
    }
    #endregion Points

    #region Attack
    public override int GetCurrentAttackDamage(bool attackAndReset = true)
    {
        int baseDamage = base.GetCurrentAttackDamage(attackAndReset);
        int bonusDamage = 0;

        if (playerAddition != null)
        {
            bonusDamage = playerAddition.GetAllDamageBonus();
        }

        return baseDamage + bonusDamage;
    }
    #endregion Attack

    #region Health
    public override void HandleDeath()
    {
        base.HandleDeath();
        // End game? 
    }
    #endregion Health
}
