using Kudoshi.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// Mostly used for oneshot sounds.
/// Warning: Not recommended to use it for loopable objects. The pooling will cause the sound to be cut off when reusing the sound emitter
/// </summary>
public partial class SoundManager : Singleton<SoundManager>
{
    public static event Action<Sound> OnSoundSfxPlayed;
    public static event Action<Sound> OnBGMPlayed;
    public static event Action<Sound> OnDialoguePlayed;
    public static void NotifySfxPlayed(Sound sound) => OnSoundSfxPlayed?.Invoke(sound);
    public static void NotifyBGMPlayed(Sound sound) => OnBGMPlayed?.Invoke(sound);
    public static void NotifyDialoguePlayed(Sound sound) => OnDialoguePlayed?.Invoke(sound);

    [SerializeField] private SO_SoundRepository SoundRepoSO;
    [SerializeField] private Pool<SoundEmitter> m_SoundEmitterPool;
    [SerializeField] private string m_TestSoundName;
    [SerializeField] private AudioSource m_BGMAudioSource;
    [SerializeField] private AudioSource m_DialogueAudioSource;
    [SerializeField] private AudioMixer m_AudioMixer;

    private Dictionary<string, Sound> m_OneShotAudioDict;
    private Dictionary<string, Sound> m_BGMAudioDict;
    private Dictionary<string, Sound> m_DialogueAudioDict;
    private List<SoundEmitter> m_SoundEmitterList = new List<SoundEmitter>();

    private Coroutine _bgmStartLoopCr;

    private void Awake()
    {
        // Removed parenting. Trusting that applicaton controller will persist this object across scenes.

        this.m_OneShotAudioDict = new Dictionary<string, Sound>();
        for (int s = 0; s < this.SoundRepoSO.SoundList.Length; s++)
        {
            Sound sound = this.SoundRepoSO.SoundList[s];
            this.m_OneShotAudioDict.Add(sound.SoundName, sound);
        }

        this.m_BGMAudioDict = new Dictionary<string, Sound>();
        for (int i = 0; i < this.SoundRepoSO.BGMList.Length; i++)
        {
            Sound sound = this.SoundRepoSO.BGMList[i];
            sound.SoundGroup = SoundGroup.Music;
            this.m_BGMAudioDict.Add(sound.SoundName, sound);
        }

        InitializeSoundEmitters();

        DontDestroyOnLoad(gameObject);
    }

    private void InitializeSoundEmitters()
    {
        GameObject soundEmitterParent = new GameObject("SoundEmitter Parent");
        soundEmitterParent.transform.parent = transform;

        m_SoundEmitterPool.Initialize(soundEmitterParent.transform);

        foreach(SoundEmitter emitter in m_SoundEmitterPool.Objects)
        {
            emitter.Initialize(m_AudioMixer);
        }
    }

    #region Context Menu

    [ContextMenu("Test Sound fx")]
    private void TestSound()
    {
        PlaySound(m_TestSoundName, transform);
    }

    [ContextMenu("Test Stop Sound")]
    private void TestStopSound()
    {
        StopOneShotBySoundName(m_TestSoundName);
    }

    [ContextMenu("Test Music")]
    private void TestMusic()
    {
        PlayBGMusic("bgm_testmusic");
    }

    #endregion

    /// <summary>
    /// Sound Emitters register itself to sound manager to get its entity ID
    /// Allows for tracking and managing of sound emitters
    /// </summary>
    /// <param name="soundEmitter"></param>
    /// <returns>Entity ID</returns>
    public int SubscribeSoundEmitter(SoundEmitter soundEmitter)
    {
        m_SoundEmitterList.Add(soundEmitter);
        return m_SoundEmitterList.Count - 1;
    }

    public void LoadDialogueVoicelines(string levelID)
    {
        this.m_DialogueAudioDict = new Dictionary<string, Sound>();
        AudioClip[] dialogues = Resources.LoadAll<AudioClip>(this.SoundRepoSO.DialoguePath + "/Lv"+levelID);

        foreach(AudioClip dialogue in dialogues) 
        {
            Sound sound = new Sound();

            sound.Clip = dialogue;
            sound.SoundName = dialogue.name;
            sound.SoundGroup = SoundGroup.Dialogue;

            this.m_DialogueAudioDict.Add(dialogue.name, sound);
        }
    }


    #region Play Sounds

    public int PlaySound(SoundVariationizer variationizer, Transform attachedObj = null, SoundRuntimeModifier? runtimeModifier = null)
    {
        SoundRuntimeModifier modifier = runtimeModifier ?? new SoundRuntimeModifier();
        modifier.Pitch = variationizer.Pitch;

        return DoPlaySound(variationizer.SoundName, attachedObj, modifier);
    }

    /// <summary>
    /// Plays the specified sound using PlayOneShot
    /// </summary>
    /// <param name="soundName"></param>
    public int PlaySound(string soundName, Transform attachedObj = null, SoundRuntimeModifier? runtimeModifier = null)
    {
        return DoPlaySound(soundName, attachedObj, runtimeModifier);
    }

    private int DoPlaySound(string soundName, Transform attachedObject, SoundRuntimeModifier? runtimeModifier)
    {
        Sound soundToPlay;
        if (this.m_OneShotAudioDict.TryGetValue(soundName, out soundToPlay))
        {
            SoundEmitter soundEmitter = m_SoundEmitterPool.GetNextObject();

            if (attachedObject != null)
                soundEmitter.PlaySound(soundToPlay, attachedObject, runtimeModifier);
            else
                soundEmitter.PlaySound(soundToPlay, transform, runtimeModifier);

            //Debug.Log("[SoundSystem] Playing sfx: " + soundName);

            return soundEmitter.GetSoundEntityID();
        }
        else
        {
            Debug.LogWarning("Sound: " + soundName + " not found!");
            return -1;
        }
    }


    public int PlayAtLocation(SoundVariationizer variationizer, Vector3 location, SoundRuntimeModifier? runtimeModifier = null)
    {
        SoundRuntimeModifier modifier = runtimeModifier ?? new SoundRuntimeModifier();
        modifier.Pitch = variationizer.Pitch;

        return DoPlayAtLocation(variationizer.SoundName, location, runtimeModifier);
    }
    public int PlayAtLocation(string soundName, Vector3 location, SoundRuntimeModifier? runtimeModifier = null)
    {
        return DoPlayAtLocation(soundName, location, runtimeModifier);
    }

    private int DoPlayAtLocation(string soundName, Vector3 location, SoundRuntimeModifier? runtimeModifier)
    {
        Sound soundToPlay;
        if (this.m_OneShotAudioDict.TryGetValue(soundName, out soundToPlay))
        {
            SoundEmitter soundEmitter = m_SoundEmitterPool.GetNextObject();
            soundEmitter.transform.position = location;
            soundEmitter.PlaySound(soundToPlay, null, runtimeModifier);

            return soundEmitter.GetSoundEntityID();
        }
        else
        {
            Debug.LogWarning("Sound: " + soundName + " not found!");
            return -1;
        }
    }

    #endregion

    #region Stop Sounds

    /// <summary>
    /// Stops the first existing found sound. 
    /// Warning: If multiple instances of the same sound are playing, only the first instance found will be stopped.
    /// </summary>
    /// <param name="soundName"></param>
    public void StopOneShotBySoundName(string soundName)
    {
        if (!m_OneShotAudioDict.TryGetValue(soundName, out Sound soundToStop))
        {
            Debug.LogWarning($"Sound: {soundName} not found! Cannot stop playback.");
            return;
        }

        // Iterate through active SoundEmitters in the pool
        foreach (SoundEmitter soundEmitter in m_SoundEmitterPool.GetActiveObjects())
        {
            if (soundEmitter.IsPlayingSound(soundToStop))
            {
                soundEmitter.StopSound();
                Debug.Log($"Stopped sound: {soundName}");
                return; // Stop once the sound is found
            }
        }

        Debug.LogWarning($"Sound: {soundName} is not currently playing!");
    }

    public void StopOneShotByEntityID(int entityID)
    {
        if (entityID < 0 || entityID >= m_SoundEmitterList.Count)
        {
            Debug.LogWarning($"Entity ID: {entityID} is out of range! Cannot stop playback.");
            return;
        }

        m_SoundEmitterList[entityID].StopSound();
    }

    #endregion

    #region Play BGM Music

    public void PlayBGMusic(string musicName)
    {
        Sound soundToPlay;
        if (this.m_BGMAudioDict.TryGetValue(musicName, out soundToPlay))
        {
            m_BGMAudioSource.Stop();
            soundToPlay.ApplyTo(m_BGMAudioSource);
            m_BGMAudioSource.Play();

            NotifyBGMPlayed(soundToPlay);

            if (_bgmStartLoopCr != null)
            {
                StopCoroutine(_bgmStartLoopCr);
            }

            _bgmStartLoopCr = StartCoroutine(BgmStartLoopCr(musicName, soundToPlay.Clip));
        }
        else
        {
            Debug.LogWarning("Music: " + musicName + " not found!");
            return;
        }
    }

    public void StopBGMMusic()
    {
        m_BGMAudioSource.Stop();
    }

    #endregion

    #region Play Dialogue

    public void PlayDialogue(string dialogueName)
    {
        Sound soundToPlay;
        if (this.m_DialogueAudioDict.TryGetValue(dialogueName, out soundToPlay))
        {
            m_DialogueAudioSource.Stop();
            soundToPlay.ApplyTo(m_DialogueAudioSource);
            m_DialogueAudioSource.Play();
            NotifyDialoguePlayed(soundToPlay);
        }
        else
        {
            Debug.LogWarning("Dialogue: " + dialogueName + " not found!");
            return;
        }
    }

    public void StopDialogue()
    {
        m_DialogueAudioSource.Stop();
    }

    public AudioClip GetCurrentDialogueClip()
    {
        return m_DialogueAudioSource.clip;
    }

    #endregion

    #region Control Mixer Groups

    // Used for fade in and out 
    private Dictionary<SoundGroup, SoundFade> m_SoundFadeDict = new Dictionary<SoundGroup, SoundFade>();
    public void FadeIn(SoundGroup soundGroup, float duration)
    {
        // We always assume that fade in MUST be played AFTER fade out
        if (!m_SoundFadeDict.ContainsKey(soundGroup)) return;

        // Kill of previous fade out tween if have yet to finish
        if (m_SoundFadeDict[soundGroup].Coroutine != null)
        {
            StopCoroutine(m_SoundFadeDict[soundGroup].Coroutine);
        }

        string soundGroupSetting = soundGroup.ToString() + "Volume";
        m_AudioMixer.GetFloat(soundGroupSetting, out float currentVolume);

        m_SoundFadeDict[soundGroup].Coroutine = StartCoroutine(Cr_FadeAudioRoutine(soundGroupSetting, currentVolume, m_SoundFadeDict[soundGroup].OriginalVolume, duration, CompleteFade));

        void CompleteFade()
        {
            m_SoundFadeDict.Remove(soundGroup);
        }
    }

    public void FadeOut(SoundGroup soundGroup, float duration)
    {
        // Always assume fade out is done first and cannot be done during fade in
        if (m_SoundFadeDict.ContainsKey(soundGroup) && m_SoundFadeDict[soundGroup].Coroutine != null) return;

        string soundGroupSetting = soundGroup.ToString() + "Volume";
        m_AudioMixer.GetFloat(soundGroupSetting, out float currentVolume);

        // Register original volume
        if (m_SoundFadeDict.ContainsKey(soundGroup))
        {
            m_SoundFadeDict[soundGroup].OriginalVolume = currentVolume;
        }
        else
        {
            SoundFade soundFade = new SoundFade();
            soundFade.OriginalVolume = currentVolume;
            m_SoundFadeDict.Add(soundGroup, soundFade);
        }

        m_SoundFadeDict[soundGroup].Coroutine = StartCoroutine(Cr_FadeAudioRoutine(soundGroupSetting, currentVolume, -80f, duration, null));
    }

    private IEnumerator Cr_FadeAudioRoutine(string soundSetting, float start, float target, float duration, Action onComplete)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime; // important for UI/audio systems

            float value = Mathf.Lerp(start, target, elapsed / duration);
            m_AudioMixer.SetFloat(soundSetting, value);

            yield return null;
        }

        m_AudioMixer.SetFloat(soundSetting, target);

        onComplete?.Invoke();
    }
    #endregion

    /// <summary>
    /// Ensure that the looping bgm has suffix ending with _loop
    /// </summary>
    /// <param name="musicName"></param>
    /// <returns></returns>
    private IEnumerator BgmStartLoopCr(string musicName, AudioResource clip)
    {
        float openLength = (clip as AudioClip).length;

        yield return new WaitForSecondsRealtime(openLength);

        Sound soundToPlay;
        if (this.m_BGMAudioDict.TryGetValue(musicName + "_loop", out soundToPlay))
        {
            m_BGMAudioSource.Stop();
            m_BGMAudioSource.resource = soundToPlay.Clip;
            soundToPlay.SoundSetting.ApplyTo(m_BGMAudioSource);
            m_BGMAudioSource.Play();

            NotifyBGMPlayed(soundToPlay);
        }
        else
        {
            Debug.LogWarning("Music Loop: " + musicName + " not found!");
            yield break;
        }
    }

    // Currently not in use
    private AudioMixerGroup GetAudioMixerGroup(string groupName)
    {
        AudioMixerGroup[] groups = m_AudioMixer.FindMatchingGroups(groupName);

        if (groups.Length > 0)
            return groups[0];

        return null;
    }

    /// <summary>
    /// Safely retrieves a raw AudioResource clip directly from the repository by its key string.
    /// </summary>
    public AudioResource GetRawAudioClip(string soundName)
    {
        // Try to pull from the faster dictionary cache if it has already been initialized
        if (m_OneShotAudioDict != null && m_OneShotAudioDict.TryGetValue(soundName, out Sound cachedSound))
        {
            return cachedSound.Clip;
        }

        // Fallback: search the raw ScriptableObject configuration list directly
        if (SoundRepoSO != null && SoundRepoSO.SoundList != null)
        {
            foreach (var sound in SoundRepoSO.SoundList)
            {
                if (sound.SoundName == soundName)
                {
                    return sound.Clip;
                }
            }
        }

        Debug.LogWarning($"[SoundManager] Audio clip with name '{soundName}' could not be found.");
        return null;
    }

    public Sound GetSound(string soundName)
    {
        Sound soundToPlay;
        if (this.m_OneShotAudioDict.TryGetValue(soundName, out soundToPlay))
        {
            return soundToPlay;
        }
        else
        {
            Debug.LogWarning("Sound: " + soundName + " not found!");
            return null;
        }
    }
}
