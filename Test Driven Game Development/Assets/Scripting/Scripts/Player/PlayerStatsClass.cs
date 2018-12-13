using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerStatsClass : FighterStatsClass
{
    private IPlayer playerAddition;

    public int currentPoints;
    public float playerSpeed = 2.5f;

    #region Setup
    public PlayerStatsClass()
    {
        currentPoints = 0;
    }

    public void SetUpPlayerStats(IPlayer player)
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

    public override void ShowDodge()
    {
        IGameController gameCtr = playerAddition.GetGameController();
        if (gameCtr != null)
        {
            gameCtr.ReactToDodge(dodged);
        }
    }

    #endregion Attack

    #region Health
    public override void HandleDeath()
    {
        base.HandleDeath();
        // End game? 
    }
    #endregion Health

    #region Movement

    public Vector3 CalcMovement(float hor, float vert, float deltaTime)
    {
        float moveX;
        float moveZ;
        if (playerAddition != null && playerAddition.GetGameController() != null &&  playerAddition.GetGameController().IsInBattle() == true)
        {
            moveX = 0;
            moveZ = 0;
        }
        else
        {
            moveX = vert * playerSpeed * deltaTime;
            moveZ = hor * playerSpeed * deltaTime;
        }
     
        return new Vector3(moveX, 0, moveZ);
    }

    #endregion Movement

}
