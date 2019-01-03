using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player : MonoBehaviour, IPlayer
{
    [Header("Set Up")]
    public PlayerStatsClass stats;
    public PlayerInventoryClass inventory;
    public IUnityStaticService staticService;
    public GameController GameCtr;

    [Header("Battle UI")]
    public GameObject healthBar;
    public GameObject turnTimeBar;

    [Header("Gravity")]
    public float gravityValue = -0.1f;
    public CharacterController charContr;

    [Header("Particles")]
    public GameObject AttackParticle;
    public float AttackParticleLength = 1;
    public GameObject ChargeParticle;
    public float ChargeParticleLength = 1;
    public GameObject BombParticle;
    public float BombParticleLength = 1;

    void Start ()
    {
        if (GameCtr == null)
        {
            GameCtr = FindObjectOfType<GameController>();
        }

        if (stats == null)
        {
            stats = new PlayerStatsClass();
        }
        stats.SetUpPlayerStats(this);
        if (inventory == null)
        {
            inventory = new PlayerInventoryClass();
        }
        inventory.SetUpInventory(this);

        if (staticService == null)  // only setup staticServe anew if it's not there already (a playmode test might have set a substitute object here that we don't want to replace)
        {
            staticService = new UnityStaticService();
        }

        foreach(Item i in inventory.items)
        {
            if (i != null)
            {
                i.SetUpItem();
            }
        }

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

        if (charContr == null)
        {
            charContr = GetComponent<CharacterController>();
        }
        
    }

    void Update ()
    {
		// TMP
        if (Input.GetKeyDown(KeyCode.R))
        {
            stats.ReceiveDamage(10);
        }





        stats.SetHealthBar(healthBar);
        stats.SetTurnTimeBar(turnTimeBar);
        if (GameCtr != null && GameCtr.IsInBattle())
        {
            stats.UpdateTurnTime(staticService.GetDeltaTime());
        }
    }

    public void FixedUpdate()
    {
        #region Movement
        float horizontal = staticService.GetInputAxis("Horizontal");
        float vertical = (-1) * staticService.GetInputAxis("Vertical");

        if (charContr != null)
        {
            charContr.Move(stats.CalcMovement(horizontal, vertical, staticService.GetDeltaTime()));

            if (!charContr.isGrounded)
            {
                charContr.Move(new Vector3(0, gravityValue, 0));
            }
        }

        #endregion Movement
    }

    public void OnTriggerEnter(Collider other)
    {
        Enemy enemy = other.gameObject.GetComponent<Enemy>();
        if (enemy != null)
        {
            GameCtr.StartBattle(enemy);
        }

        ItemDrop itemDrop = other.gameObject.GetComponent<ItemDrop>();
        if (itemDrop != null)
        {
            itemDrop.PlayerCollectItem();
        }
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
        if (stats != null && stats.lastingDamageBoosts != null)
        {
            stats.lastingDamageBoosts.Clear();
        }

    }


    #region Implementation IPlayer

    public int GetAllDamageBonus()
    {
        int bonus = 0;

        if (inventory != null)
        {
            Weapon wpn = inventory.GetEquippedWeapon();
            if (wpn != null)
            {
                bonus += wpn.damage;
            }
        }

        return bonus;
    }

    public PlayerStatsClass GetPlayerStats()
    {
        return stats;
    }

    public PlayerInventoryClass GetPlayerInventory()
    {
        return inventory;
    }

    public IGameController GetGameController()
    {
        return GameCtr;
    }

    public float GetDistanceToPlayer(Vector3 pos)
    {
        return (this.transform.position - pos).magnitude;
    }

    #endregion Implementation IPlayer
}

public interface IPlayer
{
    int GetAllDamageBonus();
    PlayerStatsClass GetPlayerStats();
    PlayerInventoryClass GetPlayerInventory();
    IGameController GetGameController();
    float GetDistanceToPlayer(Vector3 pos);
}