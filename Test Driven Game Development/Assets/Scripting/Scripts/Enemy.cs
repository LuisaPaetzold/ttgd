using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public EnemyStatsClass stats;
	// Use this for initialization
	void Start ()
    {
        stats = new EnemyStatsClass();
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}

[Serializable]
public class EnemyStatsClass : FighterStatsClass
{
    
}
