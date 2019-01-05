using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FighterStatsClass
{
    [Header ("Health")]
    public int MaxHealth = 100;
    public int currentHealth;
    public FighterState currentState;
    public Color green;
    public Color red;
    [Range(0, 1)]
    public float LastBreathThreshold = 0.1f;
    [Header ("Battle")]
    public int AttackDamage = 10;
    public float ChargeDamageBoost = 1;
    public int MaxAmountOfChargings = 3;
    public float oneTimeDamageBoost;
    public Dictionary<string, float> lastingDamageBoosts;
    [Range(0, 1)]
    public float DodgePropability = 0.1f;
    public GameObject dodged;
    public float DodgeDuration = 1;
    [Header ("Turn Time")]
    public float TurnTime = 3.0f;
    public float currentTurnTime = 0;




    #region Setup
    public FighterStatsClass()
    {
        SetUpStats();
    }

    public virtual void SetUpStats()
    {
        currentHealth = MaxHealth;
        currentState = FighterState.alive;
        lastingDamageBoosts = new Dictionary<string, float>();
    }
    #endregion Setup

    #region Health
    public int GetMaxHealth()
    {
        return MaxHealth;
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    public void SetHealthBar(GameObject healthBar)
    {
        if (healthBar != null)
        {
            float percentage = currentHealth * 1.0f / MaxHealth * 1.0f;
            healthBar.transform.localScale = new Vector3(percentage, healthBar.transform.localScale.y, healthBar.transform.localScale.z);

            Image img = healthBar.GetComponent<Image>();
            if (img != null)
            {
                if (currentState == FighterState.lastBreath)
                {
                    img.color = red;
                }
                else
                {
                    img.color = green;
                }
            }
        }
    }

    public void SetTurnTimeBar(GameObject turnTimeBar)
    {
        if (turnTimeBar != null)
        {
            float percentage = currentTurnTime * 1.0f / TurnTime * 1.0f;
            turnTimeBar.transform.localScale = new Vector3(percentage, turnTimeBar.transform.localScale.y, turnTimeBar.transform.localScale.z);
        }
    }

    public void ReceiveDamage(int damage)
    {
        if (currentState == FighterState.dead)
        {
            Debug.LogWarning("Fighter is already dead and can no longer receive any damage!");
            return;
        }

        if (damage > 0)
        {
            currentHealth -= damage;
            if (currentHealth <= 0)
            {
                HandleDeath();
            }
            else if (currentHealth <= MaxHealth * LastBreathThreshold)
            {
                currentState = FighterState.lastBreath;
            }
        }
        else
        {
            Debug.LogWarning("Fighter cannot receive negative damage. Health will not be modified.");
        }
    }

    public void GetHealedBy(int heal)
    {
        if (heal > 0)
        {
            currentHealth += heal;
            if (currentHealth > MaxHealth)
            {
                currentHealth = MaxHealth;
            }
            else if (currentHealth > MaxHealth * LastBreathThreshold)
            {
                currentState = FighterState.alive;
            }
        }
        else
        {
            Debug.LogWarning("Fighter cannot be healed by a negative amount. Health will not be modified.");
        }
    }

    public virtual void HandleDeath()
    {
        currentHealth = 0;
        currentState = FighterState.dead;
    }

    #endregion Health

    #region FighterState
    public FighterState GetCurrentFighterState()
    {
        return currentState;
    }

    public float GetLastBreathThreshold()
    {
        return LastBreathThreshold;
    }
    #endregion FighterState

    #region Attack
    public float GetDefaultAttackDamage()
    {
        return AttackDamage;
    }

    public float GetDodgePropability()
    {
        return DodgePropability;
    }

    public virtual int GetCurrentAttackDamage(bool attackAndReset = true)
    {
        float lastingBoosts = 0f;
        foreach (KeyValuePair<string, float> pair in lastingDamageBoosts)
        {
            lastingBoosts += pair.Value;
        }

        int damage = Mathf.FloorToInt(AttackDamage * (1 + lastingBoosts + oneTimeDamageBoost));
        if (attackAndReset)
        {
            oneTimeDamageBoost = 0;
        }
        return damage;
    }

    public bool CanAct()
    {
        return (currentTurnTime >= TurnTime);
    }

    public void UpdateTurnTime(float passedTime)
    {
        if (currentTurnTime < TurnTime)
        {
            currentTurnTime += passedTime;
        }
    }

    public virtual bool AttackOpponent(FighterStatsClass opponent, bool CanBeDodged = true, bool ignoreTurnTime = false)
    {
        if (!CanAct() && !ignoreTurnTime)
        {
            Debug.LogWarning("Tried to attack an opponent when not allowed to do that!");
            return false;
        }
        if (!ignoreTurnTime)
        {
            currentTurnTime = 0;
        }

        if (opponent == null)
        {
            Debug.LogWarning("Fighter tried to attack an opponent that's a nnullpointer. Can't attack non-existant opponents!");
        }
        else if (opponent.GetCurrentFighterState() == FighterState.dead)
        {
            Debug.LogWarning("Fighter tried to attack an opponent that already died. Can't attack dead opponents!");
        }
        else
        {
            if (CanBeDodged)
            {
                float dodgeRand = 1;
                dodgeRand = Random.value;
                if (dodgeRand >= opponent.GetDodgePropability())
                {
                    opponent.ReceiveDamage(GetCurrentAttackDamage());
                    return true;
                }
                else
                {
                    opponent.ShowDodge();
                    Debug.Log("Dodged!");
                    return false;
                }
            }
            else
            {
                opponent.ReceiveDamage(GetCurrentAttackDamage());
                return true;
            }
        }
        return false;
    }

    public virtual void ShowDodge()
    {
        Debug.LogError("ShowDodge() must be implemented inside the sub-class!");
    }


    #endregion Attack

    #region Boost
    public float GetChargeDamageBoost()
    {
        return ChargeDamageBoost;
    }

    public int GetMaxAmountOfChargings()
    {
        return MaxAmountOfChargings;
    }

    public int GetCurrentAmountOfChargings()
    {
        return Mathf.FloorToInt(oneTimeDamageBoost / ChargeDamageBoost);
    }

    public void AddLastingDamageBoost(string source, float mod)
    {
        if (lastingDamageBoosts.ContainsKey(source))
        {
            Debug.LogWarning("Fighter cannot receive multiple lasting damage boosts from the same source. Attacke damage will not be modified.");
        }
        lastingDamageBoosts.Add(source, mod);
    }

    public void RemoveLastingDamageBoost(string source)
    {
        if (!lastingDamageBoosts.ContainsKey(source))
        {
            Debug.LogWarning("Fighter cannot remove lasting damage boost of a source that never gave him a boost. Attacke damage will not be modified.");
        }
        lastingDamageBoosts.Remove(source);
    }

    public void UseChargeForDamageBoost(bool ignoreTurnTime = false)
    {
        if (!CanAct() && !ignoreTurnTime)
        {
            Debug.LogWarning("Tried to charge when not allowed to do that!");
            return;
        }
        if (oneTimeDamageBoost < MaxAmountOfChargings * ChargeDamageBoost)
        {
            if (!ignoreTurnTime)
            {
                currentTurnTime = 0;
            }
            
            oneTimeDamageBoost += ChargeDamageBoost;
        }
    }
    #endregion Boost
}

public enum FighterState
{
    alive, dead, lastBreath
}
