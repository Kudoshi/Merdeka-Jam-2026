using UnityEngine;

public struct SoundRuntimeModifier
{
    public float? Pitch;
    public float? Volume;
    public bool? Loop;
    public int? Priority;
    public float? PanStereo;
    public float? SpatialBlend;
    public float? ReverbZoneMix;
    public float? DopplerLevel;
    public float? Spread;
    public AudioRolloffMode? RolloffMode;
    public float? MinDistance;
    public float? MaxDistance;
    public bool? BypassEffects;
    public bool? BypassListenerEffects;
    public bool? BypassReverbZones;

    // Extra settings
    public float? Delay;

    public SoundRuntimeModifier WithPitch(float v) { Pitch = v; return this; }
    public SoundRuntimeModifier WithVolume(float v) { Volume = v; return this; }
    public SoundRuntimeModifier WithLoop(bool v) { Loop = v; return this; }
    public SoundRuntimeModifier WithPriority(int v) { Priority = v; return this; }
    public SoundRuntimeModifier WithPanStereo(float v) { PanStereo = v; return this; }
    public SoundRuntimeModifier WithSpatialBlend(float v) { SpatialBlend = v; return this; }
    public SoundRuntimeModifier WithReverbZoneMix(float v) { ReverbZoneMix = v; return this; }
    public SoundRuntimeModifier WithDopplerLevel(float v) { DopplerLevel = v; return this; }
    public SoundRuntimeModifier WithSpread(float v) { Spread = v; return this; }
    public SoundRuntimeModifier WithRolloffMode(AudioRolloffMode v) { RolloffMode = v; return this; }
    public SoundRuntimeModifier WithMinDistance(float v) { MinDistance = v; return this; }
    public SoundRuntimeModifier WithMaxDistance(float v) { MaxDistance = v; return this; }
    public SoundRuntimeModifier WithBypassEffects(bool v) { BypassEffects = v; return this; }
    public SoundRuntimeModifier WithBypassListenerEffects(bool v) { BypassListenerEffects = v; return this; }
    public SoundRuntimeModifier WithBypassReverbZones(bool v) { BypassReverbZones = v; return this; }
    public SoundRuntimeModifier WithDelay(float v) { Delay = v; return this; }

    public void ApplyTo(AudioSource src)
    {
        if (Pitch.HasValue) src.pitch = Pitch.Value;
        if (Volume.HasValue) src.volume = Volume.Value;
        if (Loop.HasValue) src.loop = Loop.Value;
        if (Priority.HasValue) src.priority = Priority.Value;
        if (PanStereo.HasValue) src.panStereo = PanStereo.Value;
        if (SpatialBlend.HasValue) src.spatialBlend = SpatialBlend.Value;
        if (ReverbZoneMix.HasValue) src.reverbZoneMix = ReverbZoneMix.Value;
        if (DopplerLevel.HasValue) src.dopplerLevel = DopplerLevel.Value;
        if (Spread.HasValue) src.spread = Spread.Value;
        if (RolloffMode.HasValue) src.rolloffMode = RolloffMode.Value;
        if (MinDistance.HasValue) src.minDistance = MinDistance.Value;
        if (MaxDistance.HasValue) src.maxDistance = MaxDistance.Value;
        if (BypassEffects.HasValue) src.bypassEffects = BypassEffects.Value;
        if (BypassListenerEffects.HasValue) src.bypassListenerEffects = BypassListenerEffects.Value;
        if (BypassReverbZones.HasValue) src.bypassReverbZones = BypassReverbZones.Value;
    }
}