
using System;
using UnityEngine;

public static class SoundExtension
{
    public static void SMPlay(this AudioSource audioSrc)
    {
        audioSrc.Play();

        Sound sound = new Sound(audioSrc);

        SoundManager.NotifySfxPlayed(sound);
    }

    public static void SMStop(this AudioSource audioSrc)
    {
        audioSrc.Stop();
    }
}