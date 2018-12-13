using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player : MonoBehaviour, IPlayer
{
    public PlayerStatsClass stats;
    public PlayerInventoryClass inventory;
    public IUnityStaticService staticService;
    public GameController GameCtr;

    public GameObject healthBar;
    public GameObject turnTimeBar;

    [Header("Gravity")]
    public float verticalVelocity = 0;
    public float gravity = 0.1f;
    public bool isGrounded = true;

    void Start ()
    {
        if (GameCtr == null)
        {
            GameCtr = FindObjectOfType<GameController>();
        }

        stats.SetUpPlayerStats(this);
        inventory.SetUpInventory(this);
        if (staticService == null)  // only setup staticServe anew if it's not there already (a playmode test might have set a substitute object here that we don't want to replace)
        {
            staticService = new UnityStaticService();
        }

        foreach(Item i in inventory.items)
        {
            i.SetUpItem();
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
        
    }

    void Update ()
    {
		// TMP
        if (Input.GetKeyDown(KeyCode.C))
        {
            stats.UseChargeForDamageBoost();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            stats.ReceiveDamage(10);
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            inventory.UseItem(0);
        }



        #region Movement
        float horizontal = staticService.GetInputAxisRaw("Horizontal");
        float vertical = (-1) * staticService.GetInputAxisRaw("Vertical");
        /*RaycastHit groundCheck;
        //isGrounded = Physics.Raycast(this.transform.position + new Vector3(0, 0.5f, 0), Vector3.down, out groundCheck, 0.5f + 0.1f);
        if (transform.position.y < 0.1f)
        {
            isGrounded = true;
        }
        if (isGrounded)
        {
            verticalVelocity = 0;
            //verticalVelocity = gravity * Time.deltaTime;
        }
        
        verticalVelocity += gravity * Time.deltaTime;
        if (transform.position.y > 0.1f)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y - 0.01f, transform.position.z);
        }*/

        transform.position += stats.CalcMovement(horizontal, vertical, staticService.GetDeltaTime());
        
        #endregion Movement

        stats.SetHealthBar(healthBar);
        stats.SetTurnTimeBar(turnTimeBar);
        if (GameCtr.IsInBattle())
        {
            stats.UpdateTurnTime(staticService.GetDeltaTime());
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        Enemy enemy = other.gameObject.GetComponent<Enemy>();
        if (enemy != null)
        {
            GameCtr.StartBattle(enemy);
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

    #endregion Implementation IPlayer
}

public interface IPlayer
{
    int GetAllDamageBonus();
    PlayerStatsClass GetPlayerStats();
    PlayerInventoryClass GetPlayerInventory();
    IGameController GetGameController();
}