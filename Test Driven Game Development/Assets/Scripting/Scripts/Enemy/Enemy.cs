using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public GameController GameCtr;
    public EnemyStatsClass stats;

	void Start ()
    {
        if (GameCtr == null)
        {
            GameCtr = FindObjectOfType<GameController>();
        }
        stats.SetUpEnemyStats(GameCtr);
	}
	
	void Update ()
    {
		
	}
}
