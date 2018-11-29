using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public EnemyStatsClass stats;

	void Start ()
    {
        //stats = new EnemyStatsClass();
        stats.SetUpStats();
	}
	
	void Update ()
    {
		
	}
}

[Serializable]
public class EnemyStatsClass : FighterStatsClass
{
    
}
