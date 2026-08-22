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

   void Start()
   {
       musicSource.clip = backgroundLevel1;
       musicSource.Play();
   }

   public void PlaySFX(AudioClip clip)
   {
       SFXSource.PlayOneShot(clip);
   }
}
