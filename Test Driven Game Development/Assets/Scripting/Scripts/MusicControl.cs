using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicControl : MonoBehaviour
{

    // to test:
    // game starts with normal music
    // battle music starts in battle
    // battle end goes back to normal music
    // suspense music after teleporting down
    // normal music after teleporting back up
    // All music is looping
    // music fades out on game end

    public AudioSource source;

    public AudioClip normalBgMusic;
    public AudioClip battleMusic;
    public AudioClip suspenseBgMusic;

    void Start ()
    {
        source = GetComponent<AudioSource>();
	}
	
	void Update ()
    {
		
	}

    public void StartBattle()
    {
        if (source != null)
        {
            source.Stop();
            source.clip = battleMusic;
            source.Play();
        }
    }

    public void EndBattle()
    {
        if (source != null)
        {
            source.Stop();
            source.clip = normalBgMusic;
            source.Play();
        }
    }

    public void EnterBasement()
    {
        if (source != null)
        {
            source.Stop();
            source.clip = suspenseBgMusic;
            source.Play();
        }
    }

    public void LeaveBasement()
    {
        if (source != null)
        {
            source.Stop();
            source.clip = normalBgMusic;
            source.Play();
        }
    }
}
