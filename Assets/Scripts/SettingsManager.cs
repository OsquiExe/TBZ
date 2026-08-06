using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [Header("Referencias UI")]
    [SerializeField] private Slider volumeSlider;

    private void OnEnable()
    {

        float savedVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);

        if (volumeSlider != null)
        {
            volumeSlider.value = savedVolume;


            volumeSlider.onValueChanged.RemoveAllListeners();
            volumeSlider.onValueChanged.AddListener(ChangeVolume);
        }


        AudioListener.volume = savedVolume;
    }


    public void ChangeVolume(float newValue)
    {

        PlayerPrefs.SetFloat("MasterVolume", newValue);
        PlayerPrefs.Save();


        AudioListener.volume = newValue;
    }


    public void CloseSettings()
    {
        gameObject.SetActive(false);
    }
}