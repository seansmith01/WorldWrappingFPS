using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneShotAudioHolder : MonoBehaviour
{
    [Header("Shooting")]
    [SerializeField] private AudioSource gunshotAudioSource;
    [SerializeField] private AudioClip[] gunshotClips;
    [Header("Footsteps")]
    [SerializeField] private AudioSource footstepAudioSource;    
    [SerializeField] private AudioClip[] footstepClips;
    [SerializeField] private float minFootstepVolume = 0.3f;
    [SerializeField] private float maxFootstepVolume = 0.6f;
    [Header("Rotation")]
    [SerializeField] private AudioSource rotationAudioSource;
    [SerializeField] private AudioClip[] rotationClips;
    [SerializeField] private float minRotationVolume = 0.3f;
    [SerializeField] private float maxRotationVolume = 0.6f;
    public void PlayShootSound()
    {
        gunshotAudioSource.pitch=Random.Range(0.9f, 1.1f);
        gunshotAudioSource.PlayOneShot(gunshotClips[Random.Range(0, gunshotClips.Length)]);
        gunshotAudioSource.pitch = 1f;
    }
    public void PlayRotationSound()
    {
        AudioClip rotationSound = rotationClips[Random.Range(0, rotationClips.Length)];
        float rotationVolume = Random.Range(minFootstepVolume, maxFootstepVolume);
        //rotationAudioSource.pitch = Random.Range(-0.85f, 1.15f);
        rotationAudioSource.PlayOneShot(rotationSound, rotationVolume);
    }
    public void PlayFootstepSound()
    {
        AudioClip footstepSound = footstepClips[Random.Range(0, footstepClips.Length)];
        float footstepVolume = Random.Range(minFootstepVolume, maxFootstepVolume);
        footstepAudioSource.PlayOneShot(footstepSound, footstepVolume);
    }
}
