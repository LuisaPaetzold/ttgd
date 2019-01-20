using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffectControl : MonoBehaviour
{
    public AudioSource playerSource;
    public AudioSource enemySource;

    public AudioClip playerHit;
    public AudioClip enemyHit;
    public AudioClip playerCharge;
    public AudioClip enemyCharge;
    public AudioClip bomb;
    public AudioClip heal;
    public AudioClip boost;
    public AudioClip enemyDeath;
    public AudioClip itemPickUp;
    public AudioClip keyPickUp;
    public AudioClip chestOpen;
    public AudioClip desaster;
    public AudioClip flee;
    public AudioClip failFlee;
    public AudioClip teleport;
    public AudioClip dodged;
    public AudioClip gameOver;



    public void PlayerHit()
    {
        playerSource.clip = playerHit;
        playerSource.Play();
    }

    public void EnemyHit()
    {
        enemySource.clip = enemyHit;
        enemySource.Play();
    }

    public void PlayerCharge()
    {
        playerSource.clip = playerCharge;
        playerSource.Play();
    }

    public void EnemyCharge()
    {
        enemySource.clip = enemyCharge;
        enemySource.Play();
    }

    public void PlayerDodged()
    {
        playerSource.clip = dodged;
        playerSource.Play();
    }

    public void EnemyDodged()
    {
        enemySource.clip = dodged;
        enemySource.Play();
    }

    public void Bomb()
    {
        playerSource.clip = bomb;
        playerSource.Play();
    }

    public void Heal()
    {
        playerSource.clip = heal;
        playerSource.Play();
    }

    public void Boost()
    {
        playerSource.clip = boost;
        playerSource.Play();
    }

    public void EnemyDeath()
    {
        enemySource.clip = enemyDeath;
        enemySource.Play();
    }

    public void ItemPickUp()
    {
        playerSource.clip = itemPickUp;
        playerSource.Play();
    }

    public void KeyPickUp()
    {
        playerSource.clip = keyPickUp;
        playerSource.Play();
    }

    public void ChestOpen()
    {
        playerSource.clip = chestOpen;
        playerSource.Play();
    }

    public void Desaster()
    {
        playerSource.clip = desaster;
        playerSource.Play();
    }

    public void Flee()
    {
        playerSource.clip = flee;
        playerSource.Play();
    }

    public void FailFlee()
    {
        playerSource.clip = failFlee;
        playerSource.Play();
    }

    public void Teleport()
    {
        playerSource.clip = teleport;
        playerSource.Play();
    }

    public void GameOver()
    {
        playerSource.clip = gameOver;
        playerSource.Play();
    }
}
