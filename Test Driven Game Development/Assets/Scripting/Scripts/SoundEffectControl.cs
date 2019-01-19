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




    public void PlayerHit()
    {
        playerSource.PlayOneShot(playerHit);
    }

    public void EnemyHit()
    {
        enemySource.PlayOneShot(enemyHit);
    }

    public void PlayerCharge()
    {
        playerSource.PlayOneShot(playerCharge);
    }

    public void EnemyCharge()
    {
        enemySource.PlayOneShot(enemyCharge);
    }

    public void PlayerDodged()
    {
        playerSource.PlayOneShot(dodged);
    }

    public void EnemyDodged()
    {
        enemySource.PlayOneShot(dodged);
    }

    public void Bomb()
    {
        playerSource.PlayOneShot(bomb);
    }

    public void Heal()
    {
        playerSource.PlayOneShot(heal);
    }

    public void Boost()
    {
        playerSource.PlayOneShot(boost);
    }

    public void EnemyDeath()
    {
        enemySource.PlayOneShot(enemyDeath);
    }

    public void ItemPickUp()
    {
        playerSource.PlayOneShot(itemPickUp);
    }

    public void KeyPickUp()
    {
        playerSource.PlayOneShot(keyPickUp);
    }

    public void ChestOpen()
    {
        playerSource.PlayOneShot(chestOpen);
    }

    public void Desaster()
    {
        playerSource.PlayOneShot(desaster);
    }

    public void Flee()
    {
        playerSource.PlayOneShot(flee);
    }

    public void FailFlee()
    {
        playerSource.PlayOneShot(failFlee);
    }

    public void Teleport()
    {
        playerSource.PlayOneShot(teleport);
    }
}
