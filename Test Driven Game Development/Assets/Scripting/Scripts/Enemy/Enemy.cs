using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public GameController GameCtr;
    public EnemyStatsClass stats;
    public IUnityStaticService staticService;

    public GameObject healthBar;
    public GameObject turnTimeBar;

    public bool autoAttack = true;

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
            stats.AttackOpponent(GameCtr.player.stats);
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
}
