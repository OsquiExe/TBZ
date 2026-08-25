using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManagerMenu : MonoBehaviour
{
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource sfxSource; 

    public AudioClip backgroundMenuMusic;
    public AudioClip touchButtonSFX;
    // Start is called before the first frame update
    void Start()
    {
        musicSource.clip = backgroundMenuMusic;
        musicSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }
}
