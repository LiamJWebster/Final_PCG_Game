using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlaySound : MonoBehaviour
{
    public AudioClip selectSound;
    public AudioClip SliderDrag;

    AudioSource sound;
    // Start is called before the first frame update
    void Start()
    {
        sound = GetComponent<AudioSource>();        
    }

    public void PlaySelectSound()
    {
        sound.PlayOneShot(selectSound);
    }

    public void PlaySliderDragSound()
    {
        sound.PlayOneShot(SliderDrag);
    }

    public void PlayTest(Button button)
    {
        if (button.isActiveAndEnabled) 
        {
            sound.PlayOneShot(selectSound);
        }
    }

}
