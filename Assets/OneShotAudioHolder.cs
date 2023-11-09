using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneShotAudioHolder : MonoBehaviour
{
    PlayerDuplicateManager duplicateManager;
    public bool IsLocalPlayer;
    [Header("Shooting")]
    [SerializeField] private AudioSource gunshotAudioSource;
    [SerializeField] private AudioClip[] gunshotClips;
    [Header("Footsteps")]
    [SerializeField] private AudioSource footstepAudioSource;    
    [SerializeField] private AudioClip[] footstepClips;
    [SerializeField] private float localMinFootstepVolume = 0.3f;
    [SerializeField] private float localMaxFootstepVolume = 0.6f;
    [SerializeField] private float nonlocalMinFootstepVolume = 1;
    [SerializeField] private float nonlocalMaxFootstepVolume = 1;
    [Header("Landing")]
    [SerializeField] private AudioClip landClip;
    [Header("Rotation")]
    [SerializeField] private AudioSource rotationAudioSource;
    [SerializeField] private AudioClip[] rotationClips;
    [SerializeField] private float minRotationVolume = 0.3f;
    [SerializeField] private float maxRotationVolume = 0.6f;
    public void PlayShootSound()
    {
        //for (int i = 0; i < duplicateManager.DuplicateControllers.Count; i++)
        //{
           // duplicateManager.du
        //}
        gunshotAudioSource.pitch=Random.Range(0.9f, 1.1f);
        gunshotAudioSource.PlayOneShot(gunshotClips[Random.Range(0, gunshotClips.Length)]);
        gunshotAudioSource.pitch = 1f;
    }
    public void PlayRotationSound()
    {
        AudioClip rotationSound = rotationClips[Random.Range(0, rotationClips.Length)];
        float rotationVolume = Random.Range(minRotationVolume, maxRotationVolume);
        //rotationAudioSource.pitch = Random.Range(-0.85f, 1.15f);
        rotationAudioSource.PlayOneShot(rotationSound, rotationVolume);
    }
    public void PlayLandSound(float velocityRatio)
    {
        //print(velocity);
        if(IsLocalPlayer)
        {
            footstepAudioSource.PlayOneShot(landClip, velocityRatio * 2f);
        }

    }
    public void PlayFootstepSound(bool isLocalPlayer)
    {
        AudioClip footstepSound = footstepClips[Random.Range(0, footstepClips.Length)];
        if (isLocalPlayer)
        {
            footstepAudioSource.spatialBlend = 0;
            float footstepVolume = Random.Range(localMinFootstepVolume, localMaxFootstepVolume);
            footstepAudioSource.PlayOneShot(footstepSound, footstepVolume);
        }
        else
        {
            footstepAudioSource.spatialBlend = 1;
            float footstepVolume = Random.Range(nonlocalMinFootstepVolume, nonlocalMaxFootstepVolume);
            footstepAudioSource.PlayOneShot(footstepSound, footstepVolume);
        }
        
    }
}
