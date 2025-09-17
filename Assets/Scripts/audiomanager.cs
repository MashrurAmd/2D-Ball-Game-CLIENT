using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class audiomanager : MonoBehaviour
{
    public static audiomanager Instance; // ✅ global access

    [Header("---------------------Audio Sources--------------")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource SFX;

    [Header("---------------------Audio Clips--------------")]
    public AudioClip background;
    public AudioClip correctColor;
    public AudioClip wrongColor;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (musicSource != null && background != null)
        {
            musicSource.clip = background;
            musicSource.loop = true;   // 🔁 loop background music
            musicSource.Play();
        }
    }

    // 🔊 Correct match sound
    public void PlayCorrect()
    {
        if (SFX != null && correctColor != null)
            SFX.PlayOneShot(correctColor);
    }

    // 🔊 Wrong match sound
    public void PlayWrong()
    {
        if (SFX != null && wrongColor != null)
            SFX.PlayOneShot(wrongColor);
    }
}
