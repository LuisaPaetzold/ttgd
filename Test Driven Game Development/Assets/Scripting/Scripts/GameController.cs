using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour, IGameController
{
    private PlayerStatsClass playerStats;
    private List<EnemyStatsClass> currentEnemies = new List<EnemyStatsClass>();
    private bool isInBattle = false;

    void Start ()
    {
        Player player = FindObjectOfType<Player>();

        playerStats = player.stats;

        //TODO: Test in Play mode, dass nur ein Spieler da ist?
	}
	
	void Update ()
    {
		
	}

    public void StartBattle()
    {
        Debug.Log("Start Battle!");
        isInBattle = true;
    }


    #region Implementation IGameController

    public PlayerStatsClass GetPlayerStats()
    {
        return playerStats;
    }

    public List<EnemyStatsClass> GetCurrentEnemiesStats()
    {
        return currentEnemies;
    }

    public bool IsInBattle()
    {
        return isInBattle;
    }

    #endregion Implementation IGameController
}

public interface IGameController
{
    PlayerStatsClass GetPlayerStats();
    List<EnemyStatsClass> GetCurrentEnemiesStats();
    bool IsInBattle();
}
