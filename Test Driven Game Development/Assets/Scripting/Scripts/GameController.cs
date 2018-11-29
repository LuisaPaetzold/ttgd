using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour, IGameController
{
    private PlayerStatsClass playerStats;
    private List<EnemyStatsClass> currentEnemies = new List<EnemyStatsClass>();

    void Start ()
    {
        Player player = FindObjectOfType<Player>();

        playerStats = player.stats;

        //TODO: Test in Play mode, dass nur ein Spieler da ist?
	}
	
	void Update ()
    {
		
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

    #endregion Implementation IGameController
}

public interface IGameController
{
    PlayerStatsClass GetPlayerStats();
    List<EnemyStatsClass> GetCurrentEnemiesStats();
}
