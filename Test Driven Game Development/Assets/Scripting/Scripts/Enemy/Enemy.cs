using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Set Up")]
    public GameController GameCtr;
    public EnemyStatsClass stats;
    public IUnityStaticService staticService;

    [Header("Battle UI")]
    public GameObject healthBar;
    public GameObject turnTimeBar;

    [Header("Battle Behavior")]
    public bool autoAttack = true;
    [Range(0,1)]
    public float AttackProbability = 0.7f;
    public List<ItemDrop> DroppableItems;
    public bool playerCanFlee = true;
    [Range(0, 1)]
    public float playerFleeProbability = 0.2f;

    [Header("Particles")]
    public GameObject AttackParticle;
    public float AttackParticleLength = 1;
    public GameObject ChargeParticle;
    public float ChargeParticleLength = 1;
    public GameObject DeathParticle;
    public float DeathParticleLength = 1;

    void Start()
    {
        if (GameCtr == null)
        {
            GameCtr = FindObjectOfType<GameController>();
        }

        if (stats == null)
        {
            stats = new EnemyStatsClass();
        }
        stats.SetUpEnemyStats(GameCtr);

        if (healthBar != null)
        {
            healthBar.transform.parent.gameObject.SetActive(false);
        }
        if (turnTimeBar != null)
        {
            turnTimeBar.transform.parent.gameObject.SetActive(false);
        }
        if (stats.dodged != null)
        {
            stats.dodged.SetActive(false);
        }
        if (staticService == null)  // only setup staticServe anew if it's not there already (a playmode test might have set a substitute object here that we don't want to replace)
        {
            staticService = new UnityStaticService();
        }
    }

    void Update()
    {
        stats.SetHealthBar(healthBar);
        stats.SetTurnTimeBar(turnTimeBar);
        if (GameCtr != null && GameCtr.IsInBattle())
        {
            stats.UpdateTurnTime(staticService.GetDeltaTime());
        }

        if (GameCtr != null
            && GameCtr.IsInBattle()
            && autoAttack
            && stats.CanAct()
            && GameCtr.player != null
            && GameCtr.player.stats.GetCurrentFighterState() != FighterState.dead
            && GameCtr.TakesPartInCurrentBattle(this))
        {
            ChooseRandomBattleActionAndAct();
        }
    }

    public bool IsAlive()
    {
        return stats.GetCurrentFighterState() != FighterState.dead;
    }

    public void OnStartBattle()
    {
        if (healthBar != null)
        {
            healthBar.transform.parent.gameObject.SetActive(true);
        }
        if (turnTimeBar != null)
        {
            turnTimeBar.transform.parent.gameObject.SetActive(true);
        }
        stats.currentTurnTime = 0;
    }

    public void OnEndBattle()
    {
        if (healthBar != null)
        {
            healthBar.transform.parent.gameObject.SetActive(false);
        }
        if (turnTimeBar != null)
        {
            turnTimeBar.transform.parent.gameObject.SetActive(false);
        }
    }

    public void ChooseRandomBattleActionAndAct(bool CanBeDodged = true, bool IgnoreTurnTime = false)
    {
        float randValue = UnityEngine.Random.value;

        if (randValue < AttackProbability || stats.GetCurrentAmountOfChargings() == stats.GetMaxAmountOfChargings())
        {
            bool attackLanded = stats.AttackOpponent(GameCtr.player.stats, CanBeDodged, IgnoreTurnTime);
            if (attackLanded)
            {
                GameCtr.HandleLandedAttack(GameCtr.player.transform, AttackParticle, AttackParticleLength);
            }
        }
        else
        {
            if (stats.GetCurrentAmountOfChargings() < stats.GetMaxAmountOfChargings())
            {
                stats.UseChargeForDamageBoost();
                GameCtr.HandleCharging(transform, ChargeParticle, ChargeParticleLength, new Vector3(0, 0.23f, 0));
            }
        }
    }

    public void DropRandomItem()
    {
        if (DroppableItems != null && DroppableItems.Count > 0)
        {
            int rand = UnityEngine.Random.Range(0, DroppableItems.Count);
            GameObject.Instantiate(DroppableItems[rand], this.transform.position, this.transform.rotation);
        }
    }
}
