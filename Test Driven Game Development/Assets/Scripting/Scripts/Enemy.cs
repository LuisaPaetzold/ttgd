using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public EnemyStatsClass stats;

	void Start ()
    {
        stats.SetUpStats();
	}
	
	void Update ()
    {
		
	}
}

[Serializable]
public class EnemyStatsClass : FighterStatsClass
{
    #region Health
    public override void Die()
    {

    }
    #endregion Health
}
