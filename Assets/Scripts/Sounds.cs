using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sounds : MonoBehaviour
{
    public AudioSource deathAudioSource;
    public AudioSource deathPitchUpAudioSource;
    public AudioSource deathPitchDownAudioSource;

    public AudioSource doorCloseAudioSource;
    public AudioSource doorClosePitchUpAudioSource;
    public AudioSource doorClosePitchDownAudioSource;

    public AudioSource doorOpenAudioSource;
    public AudioSource doorOpenPitchUpAudioSource;
    public AudioSource doorOpenPitchDownAudioSource;

    public AudioSource impactAudioSource;
    public AudioSource impactPitchUpAudioSource;
    public AudioSource impactPitchDownAudioSource;

    public AudioSource sacrificeAudioSource;
    public AudioSource sacrificePitchUpAudioSource;
    public AudioSource sacrificePitchDownAudioSource;

    public AudioSource shootAudioSource;
    public AudioSource shootPitchUpAudioSource;
    public AudioSource shootPitchDownAudioSource;

    public List<AudioSource> deathAudioSources = new List<AudioSource>();
    public List<AudioSource> doorCloseAudioSources = new List<AudioSource>();
    public List<AudioSource> doorOpenAudioSources = new List<AudioSource>();
    public List<AudioSource> impactAudioSources = new List<AudioSource>();
    public List<AudioSource> sacrificeAudioSources = new List<AudioSource>();
    public List<AudioSource> shootAudioSources = new List<AudioSource>();

    void Awake()
    {
        deathAudioSources.Add(deathAudioSource);
        deathAudioSources.Add(deathPitchUpAudioSource);
        deathAudioSources.Add(deathPitchDownAudioSource);

        doorCloseAudioSources.Add(doorCloseAudioSource);
        doorCloseAudioSources.Add(doorClosePitchUpAudioSource);
        doorCloseAudioSources.Add(doorClosePitchDownAudioSource);

        doorOpenAudioSources.Add(doorOpenAudioSource);
        doorOpenAudioSources.Add(doorOpenPitchUpAudioSource);
        doorOpenAudioSources.Add(doorOpenPitchDownAudioSource);

        impactAudioSources.Add(impactAudioSource);
        impactAudioSources.Add(impactPitchUpAudioSource);
        impactAudioSources.Add(impactPitchDownAudioSource);

        sacrificeAudioSources.Add(sacrificeAudioSource);
        sacrificeAudioSources.Add(sacrificePitchUpAudioSource);
        sacrificeAudioSources.Add(sacrificePitchDownAudioSource);

        shootAudioSources.Add(shootAudioSource);
        shootAudioSources.Add(shootPitchUpAudioSource);
        shootAudioSources.Add(shootPitchDownAudioSource);
    }
}
