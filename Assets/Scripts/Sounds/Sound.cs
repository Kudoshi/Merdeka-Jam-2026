using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.Audio;

public enum SoundGroup
{
    UI, Dialogue, Music, SFX, Environment, Fire, Interactables, Ambience, Gameplay, Feedback, Player
, Runtime1, Runtime2, Runtime3
}

[System.Serializable]
public class Sound
{
    public string SoundName;
    public AudioResource Clip;
    public SoundGroup SoundGroup;

    [HideInInspector] public Transform AttachedParent;

    public SoundSetting SoundSetting = new SoundSetting();

    public Sound()
    {
    }

    public Sound(AudioSource audioSrc)
    {
        this.SoundName = audioSrc.clip != null ? audioSrc.clip.name : "Unknown";
        this.Clip = audioSrc.clip;

        if (audioSrc.outputAudioMixerGroup != null)
            this.SoundGroup = (SoundGroup)Enum.Parse(typeof(SoundGroup), audioSrc.outputAudioMixerGroup.name);
        else
            this.SoundGroup = SoundGroup.SFX; // Fallback default

        this.AttachedParent = audioSrc.transform;
        this.SoundSetting = new SoundSetting(audioSrc);
    }

    public void ApplyTo(AudioSource audioSrc)
    {
        audioSrc.resource = this.Clip;

        this.SoundSetting.ApplyTo(audioSrc);
    }
}

// Container used for sound fading
public class SoundFade
{
    public Coroutine Coroutine;
    public float OriginalVolume;
}

public struct SoundVariationizer
{

    public float Pitch;
    public float MinInclusiveRandomID;
    public float MaxExclusiveRandomID;
    public string SoundName;

    public SoundVariationizer(string soundName, float pitchVariation, int minInclusiveRandomID, int maxExclusiveRandomID)
    {
        Pitch = UnityEngine.Random.Range(1 - pitchVariation, 1 + pitchVariation);

        MinInclusiveRandomID = minInclusiveRandomID;
        MaxExclusiveRandomID = maxExclusiveRandomID;

        SoundName = soundName + UnityEngine.Random.Range(minInclusiveRandomID, maxExclusiveRandomID);
    }

    public SoundVariationizer(string soundName, float pitchVariation)
    {
        Pitch = UnityEngine.Random.Range(1 - pitchVariation, 1 + pitchVariation);

        MinInclusiveRandomID = -1f;
        MaxExclusiveRandomID = -1f;

        SoundName = soundName;
    }

    public SoundVariationizer(string soundName, int minInclusiveRandomID, int maxExclusiveRandomID)
    {
        Pitch = 1f;

        MinInclusiveRandomID = minInclusiveRandomID;
        MaxExclusiveRandomID = maxExclusiveRandomID;

        SoundName = soundName + UnityEngine.Random.Range(minInclusiveRandomID, maxExclusiveRandomID);
    }

}

[System.Serializable]
public class SoundSetting
{
    // Stereo & 3D Sound
    public GeneralAudioSettings GeneralAudioSettings = new GeneralAudioSettings();
    public BypassSettings BypassSettings = new BypassSettings();
    public StereoSettings StereoSettings = new StereoSettings();
    public RolloffSettings RolloffSettings = new RolloffSettings();

    public SoundSetting()
    {
        GeneralAudioSettings = new GeneralAudioSettings();
        BypassSettings = new BypassSettings();
        StereoSettings = new StereoSettings();
        RolloffSettings = new RolloffSettings();
    }

    public SoundSetting(AudioSource audioSource)
    {
        GeneralAudioSettings = new GeneralAudioSettings(audioSource);
        BypassSettings = new BypassSettings(audioSource);
        StereoSettings = new StereoSettings(audioSource);
        RolloffSettings = new RolloffSettings(audioSource);
    }

    /// <summary>
    /// Applies the sound settings to a given AudioSource.
    /// </summary>
    public void ApplyTo(AudioSource audioSource)
    {
        GeneralAudioSettings.ApplyTo(audioSource);
        BypassSettings.ApplyTo(audioSource);
        StereoSettings.ApplyTo(audioSource);
        RolloffSettings.ApplyTo(audioSource);
    }


    /// <summary>
    /// Copies settings from an existing AudioSource.
    /// </summary>
    public void CopyFrom(AudioSource audioSource)
    {
        GeneralAudioSettings.CopyFrom(audioSource);
        BypassSettings.CopyFrom(audioSource);
        StereoSettings.CopyFrom(audioSource);
        RolloffSettings.CopyFrom(audioSource);
    }
}

[System.Serializable]
public class GeneralAudioSettings
{
    // Basic settings
    [Range(0, 1)] public float Volume = 1;
    [Range(0.1f, 3)] public float Pitch = 1;
    public bool Loop = false;
    [Range(0, 256)] public int Priority = 128; // Default Unity priority (0 = highest, 256 = lowest)

    public GeneralAudioSettings() { }

    public GeneralAudioSettings(AudioSource audioSource)
    {
        CopyFrom(audioSource);
    }
    public GeneralAudioSettings(float volume, float pitch, bool loop, int priority)
    {
        Volume = volume;
        Pitch = pitch;
        Loop = loop;
        Priority = priority;
    }

    public void ApplyTo(AudioSource audioSource)
    {
        audioSource.volume = Volume;
        audioSource.pitch = Pitch;
        audioSource.loop = Loop;
        audioSource.priority = Priority;
    }

    public void CopyFrom(AudioSource audioSource)
    {
        Volume = audioSource.volume;
        Pitch = audioSource.pitch;
        Loop = audioSource.loop;
        Priority = audioSource.priority;
    }

}

[System.Serializable]
public class StereoSettings
{
    [Range(-1, 1)] public float PanStereo = 0f;
    [Range(0, 1)] public float SpatialBlend = 0f;
    [Range(0, 1.1f)] public float ReverbZoneMix = 1f;

    public StereoSettings() { }

    public StereoSettings(AudioSource audioSource)
    {
        CopyFrom(audioSource);
    }
    public StereoSettings(float panStereo, float spatialBlend, float reverbZoneMix)
    {
        PanStereo = panStereo;
        SpatialBlend = spatialBlend;
        ReverbZoneMix = reverbZoneMix;
    }

    public void ApplyTo(AudioSource audioSource)
    {
        audioSource.panStereo = PanStereo;
        audioSource.spatialBlend = SpatialBlend;
        audioSource.reverbZoneMix = ReverbZoneMix;
    }

    public void CopyFrom(AudioSource audioSource)
    {
        PanStereo = audioSource.panStereo;
        ReverbZoneMix = audioSource.reverbZoneMix;
        SpatialBlend = audioSource.spatialBlend;
    }
}

[System.Serializable]
public class RolloffSettings
{
    [Range(0, 5)] public float DopplerLevel = 1f;
    [Range(0, 360)] public float Spread = 0f;
    public AudioRolloffMode RolloffMode = AudioRolloffMode.Logarithmic;
    public float MinDistance = 1f;
    public float MaxDistance = 500f;

    public RolloffSettings() { }

    public RolloffSettings(AudioSource audioSource)
    {
        CopyFrom(audioSource);
    }
    public RolloffSettings(float dopplerLevel, float spread, AudioRolloffMode rolloffMode, float minDistance, float maxDistance)
    {
        DopplerLevel = dopplerLevel;
        Spread = spread;
        RolloffMode = rolloffMode;
        MinDistance = minDistance;
        MaxDistance = maxDistance;
    }

    public void ApplyTo(AudioSource audioSource)
    {
        audioSource.dopplerLevel = DopplerLevel;
        audioSource.spread = Spread;
        audioSource.rolloffMode = RolloffMode;
        audioSource.minDistance = MinDistance;
        audioSource.maxDistance = MaxDistance;
    }

    public void CopyFrom(AudioSource audioSource)
    {
        DopplerLevel = audioSource.dopplerLevel;
        Spread = audioSource.spread;
        RolloffMode = audioSource.rolloffMode;
        MinDistance = audioSource.minDistance;
        MaxDistance = audioSource.maxDistance;
    }
}

[System.Serializable]
public class BypassSettings
{
    public bool BypassEffects = false;
    public bool BypassListenerEffects = false;
    public bool BypassReverbZones = false;

    public BypassSettings() { }

    public BypassSettings(AudioSource audioSource)
    {
        CopyFrom(audioSource);
    }
    public BypassSettings(bool bypassEffects, bool bypassListenerEffects, bool bypassReverbZones)
    {
        BypassEffects = bypassEffects;
        BypassListenerEffects = bypassListenerEffects;
        BypassReverbZones = bypassReverbZones;
    }

    public void ApplyTo(AudioSource audioSource)
    {
        audioSource.bypassEffects = BypassEffects;
        audioSource.bypassListenerEffects = BypassListenerEffects;
        audioSource.bypassReverbZones = BypassReverbZones;
    }

    public void CopyFrom(AudioSource audioSource)
    {
        BypassEffects = audioSource.bypassEffects;
        BypassListenerEffects = audioSource.bypassListenerEffects;
        BypassReverbZones = audioSource.bypassReverbZones;
    }
}
