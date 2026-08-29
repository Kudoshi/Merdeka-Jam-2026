using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class SoundEmitter : MonoBehaviour
{
    private static int EXECUTE_EVERY_N_FRAMES = 5;

    private AudioSource m_AudioSource;
    private Transform m_AttachedObj;
    private Sound m_CurrentSound;
    private int m_SoundEntityID;
    private Coroutine m_DelayCr;
    private AudioMixer m_AudioMixer;
    
    private void Awake()
    {
        m_AudioSource = GetComponent<AudioSource>();
        m_SoundEntityID = SoundManager.Instance.SubscribeSoundEmitter(this);
    }

    public void Initialize(AudioMixer mixer)
    {
        m_AudioMixer = mixer;
    }

    /// <summary>
    /// Play sound and have the sound follow the attached object
    /// 
    /// If attached object is null, the sound will play at the sound emitter's position
    /// You can pass in special effects to overwrite the sound settings. If not, it will use the sound's default settings
    /// </summary>
    /// <param name="soundToPlay"></param>
    /// <param name="attachObj">Object sound to follow</param>
    public void PlaySound(Sound soundToPlay, Transform attachObj, SoundRuntimeModifier? runtimeModifier)
    {
        float playDelay = 0;
        m_AttachedObj = attachObj;
        m_CurrentSound = soundToPlay;

        AssignAudioSourceToGroup(soundToPlay.SoundGroup.ToString());
        soundToPlay.ApplyTo(m_AudioSource);

        if (runtimeModifier != null)
        {
            runtimeModifier.Value.ApplyTo(m_AudioSource);
            
            if (runtimeModifier.Value.Delay.HasValue) playDelay = runtimeModifier.Value.Delay.Value;
        }

       
        if (m_DelayCr != null)
        {
            StopCoroutine(m_DelayCr);
        }

        m_DelayCr = StartCoroutine(PlayCr(playDelay, soundToPlay));
        
        
    }
    
    public void StopSound()
    {
        if (m_DelayCr != null)
        {
            StopCoroutine(m_DelayCr);
        }

        m_AudioSource.Stop();
        m_CurrentSound = null;
    }

    public bool IsPlayingSound(Sound sound)
    {
        return m_AudioSource.isPlaying && m_CurrentSound == sound;
    }

    public int GetSoundEntityID()
    {
        return m_SoundEntityID;
    }

    private void ResetAttachedObject()
    {
        m_AudioSource.Stop();
        m_AttachedObj = null;
        m_CurrentSound = null;
    }

    private void Update()
    {
        // Restricts update to every N frames
        if (Time.frameCount % EXECUTE_EVERY_N_FRAMES == 0)
        {
            if (m_AttachedObj != null)
            {
                transform.position = m_AttachedObj.position;
            }
        }
        
    }

    private IEnumerator PlayCr(float delay, Sound soundToPlay)
    {
        yield return new WaitForSeconds(delay);

        m_AudioSource.Play();
        SoundManager.NotifySfxPlayed(m_CurrentSound);
        
        if (soundToPlay.Clip is AudioClip)
        {
            yield return new WaitForSeconds((soundToPlay.Clip as AudioClip).length + 0.01f);
        }
        else
        {
            while (m_AudioSource.isPlaying)
            {
                yield return new WaitForSeconds(0.25f);
            }
        }

        ResetAttachedObject();
    }

    private void AssignAudioSourceToGroup(string groupName)
    {
        // Find the group by name
        AudioMixerGroup[] groups = m_AudioMixer.FindMatchingGroups(groupName);

        if (groups.Length > 0)
        {
            m_AudioSource.outputAudioMixerGroup = groups[0];
        }
        else
        {
            Debug.LogWarning($"AudioMixerGroup '{groupName}' not found in mixer '{m_AudioMixer.name}'");
        }
    }
}

