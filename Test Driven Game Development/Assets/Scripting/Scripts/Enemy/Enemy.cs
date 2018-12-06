using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public GameController GameCtr;
    public EnemyStatsClass stats;

    public GameObject healthBar;

    void Start ()
    {
        if (GameCtr == null)
        {
            GameCtr = FindObjectOfType<GameController>();
        }
        stats.SetUpEnemyStats(GameCtr);
        healthBar.transform.parent.gameObject.SetActive(false);

    }

    void Update ()
    {
        stats.SetHealthBar(healthBar);
	}

    public bool IsAlive()
    {
        return stats.GetCurrentFighterState() != FighterState.dead;
    }

    public void OnStartBattle()
    {
        healthBar.transform.parent.gameObject.SetActive(true);
    }
}
