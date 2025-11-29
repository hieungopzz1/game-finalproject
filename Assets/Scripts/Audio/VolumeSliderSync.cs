using UnityEngine;
using UnityEngine.UI;

public class VolumeSliderSync : MonoBehaviour
{
    public Slider slider;
    public bool isMusic = true;

    void Start()
    {
        if (slider == null) slider = GetComponent<Slider>();

        if (isMusic)
        {
            float savedVol = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
            slider.value = savedVol;
            Debug.Log("Loaded Music Volume: " + savedVol);
        }
        else
        {
            float savedVol = PlayerPrefs.GetFloat("SFXVolume", 1f);
            slider.value = savedVol;
        }
    }
}