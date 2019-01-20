using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicControl : MonoBehaviour
{
    public IUnityStaticService staticService;

    public AudioSource source;

    public AudioClip normalBgMusic;
    public AudioClip battleMusic;
    public AudioClip suspenseBgMusic;

    public float IntroFadeDuration = 3;
    public float OutroFadeDuration = 3;

    [Range(0,1)]
    public float normalMusicVolume = 0.8f;
    [Range(0,1)]
    public float battleMusicVolume = 0.8f;
    [Range(0,1)]
    public float suspenseMusicVolume = 1;


    void Start ()
    {
        if (staticService == null)  // only setup staticServe anew if it's not there already (a playmode test might have set a substitute object here that we don't want to replace)
        {
            staticService = new UnityStaticService();
        }


        source = GetComponent<AudioSource>();
        
        if (source != null)
        {
            source.clip = normalBgMusic;
            source.volume = normalMusicVolume;
            source.loop = true;

            StartCoroutine(StartGame());
        }

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
            source.volume = battleMusicVolume;
            source.Play();
        }
    }

    public void EndBattle()
    {
        if (source != null)
        {
            source.Stop();
            source.clip = normalBgMusic;
            source.volume = normalMusicVolume;
            source.Play();
        }
    }

    public void EnterBasement()
    {
        if (source != null)
        {
            source.Stop();
            source.clip = suspenseBgMusic;
            source.volume = suspenseMusicVolume;
            source.Play();
        }
    }

    public void LeaveBasement()
    {
        if (source != null)
        {
            source.Stop();
            source.clip = normalBgMusic;
            source.volume = normalMusicVolume;
            source.Play();
        }
    }

    private IEnumerator StartGame()
    {
        float startVolume = source.volume;
        source.Play();


        source.volume = 0;
        while (source.volume < startVolume)
        {
            source.volume += startVolume * staticService.GetDeltaTime() / IntroFadeDuration;

            yield return null;
        }

        source.volume = startVolume;
    }

    private IEnumerator EndGame()
    {
        float startVolume = source.volume;

        while (source.volume > 0)
        {
            source.volume -= startVolume * staticService.GetDeltaTime() / OutroFadeDuration;

            yield return null;
        }

        source.Stop();
        source.volume = startVolume;
    }

    public void InvokeGameEnd()
    {
        StartCoroutine(EndGame());
    }

    public void GameOver()
    {
        source.Stop();
    }
}
