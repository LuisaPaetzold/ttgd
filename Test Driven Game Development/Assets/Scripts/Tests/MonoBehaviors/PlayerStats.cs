using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour 
{

    PlayerStatsClass stats;

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}


public class PlayerStatsClass : FighterStatsClass
{
    private int currentPoints;

    public PlayerStatsClass()
    {
        currentPoints = 0;
    }

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
}