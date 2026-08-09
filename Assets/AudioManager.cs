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
