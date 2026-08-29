
using UnityEngine;

public class AudioSourcePlayOnStart : MonoBehaviour
{
    private void Start()
    {
        AudioSource[] audioSrcs = GetComponents<AudioSource>();
    
        foreach(AudioSource src in audioSrcs)
        {
            src.SMPlay();
        }
    }
}