using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {

    public static AudioManager instance;

    public AudioSource music;
    public AudioSource sfx;

    public float lowPitchRange = 0.95f;
    public float highPitchRange = 1.05f;

    private void Awake() {
        ConfigureLikeSingleton();
    }

    private void ConfigureLikeSingleton() {
        if (AudioManager.instance != null && AudioManager.instance != this) {
            Destroy(gameObject);
            return;
        }
        AudioManager.instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void PlaySingle(AudioClip clip, float pitch) {
        sfx.clip = clip;
        sfx.pitch = pitch;
        sfx.Play();
    }

    public void PlayRandomizeSFX(params AudioClip[] clips) {
        float randomPitch = Random.Range(lowPitchRange, highPitchRange);
        sfx.pitch = randomPitch;
        AudioClip clip = clips[Random.Range(0, clips.Length)];
        PlaySingle(clip, randomPitch);
    }
}
