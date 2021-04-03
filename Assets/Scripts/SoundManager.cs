using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public static class SoundManager
{
    public enum Sound
    {
        BirdJump,
        Score,
        Lose,
        ButtonClick
    }

    public static void PlaySound(Sound sound)
    {
        GameObject gameObj = new GameObject("Sound", typeof(AudioSource));
        AudioSource audioSrc =  gameObj.GetComponent<AudioSource>();
        audioSrc.PlayOneShot(GameAssets.GetInstance().audioClips.FirstOrDefault(a=>a.sound == sound).audioClip);
    }

    public static void AddButtonSound(this Button button)
    {
        button.onClick.AddListener(() => {
            PlaySound(Sound.ButtonClick);
        });
    }
}
