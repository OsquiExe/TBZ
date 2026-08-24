using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
   [SerializeField] AudioSource musicSource;
   [SerializeField] AudioSource SFXSource;

    public AudioClip backgroundLevel1;
    public AudioClip dashSFX;
    public AudioClip punchSFX;
    public AudioClip missileSFX;
    public AudioClip laserSFX;
    public AudioClip bombDecoysSFX;
    public AudioClip boomerangSFX;
    public AudioClip fireBallSFX;
    public AudioClip snakesSFX;
    public AudioClip shockwaveSFX;
    public AudioClip spikesSFX;
    public AudioClip healSFX;
    public AudioClip bombSFX;
    public AudioClip coinSFX;
    public AudioClip blackholeSFX;


    public List<AudioClip> Sfxs;

    public enum AudioClipNames {
           backgroundLevel1
    , dashSFX
    , punchSFX
    , missileSFX
    , laserSFX
    , bombDecoysSFX
    , boomerangSFX
    , fireBallSFX
    , snakesSFX
    , shockwaveSFX
    , spikesSFX
    , healSFX
    , bombSFX
    , coinSFX
    , blackholeSFX
}

   void Start()
   {
       musicSource.clip = backgroundLevel1;
       musicSource.Play();

        //Automatizar despues del evento
        Sfxs.Add(backgroundLevel1);
        Sfxs.Add(dashSFX);
        Sfxs.Add(punchSFX);
        Sfxs.Add(missileSFX);
        Sfxs.Add(laserSFX);
        Sfxs.Add(bombDecoysSFX);
        Sfxs.Add(boomerangSFX);
        Sfxs.Add(fireBallSFX);
        Sfxs.Add(snakesSFX);
        Sfxs.Add(shockwaveSFX);
        Sfxs.Add(spikesSFX);
        Sfxs.Add(healSFX);
        Sfxs.Add(bombSFX);
        Sfxs.Add(coinSFX);
        Sfxs.Add(blackholeSFX);
    }

   public void PlaySFX(AudioClip clip)
   {
       SFXSource.PlayOneShot(clip);
}

    public void PlaySFX(AudioClipNames clip) {
        SFXSource.PlayOneShot(Sfxs[(int)clip]);
    }
}
