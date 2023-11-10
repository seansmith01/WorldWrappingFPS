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
    [SerializeField] private float minFootstepVolume = 0.3f;
    [SerializeField] private float maxFootstepVolume = 0.6f;
    [Header("Landing")]
    [SerializeField] private AudioClip landClip;
    [Header("Rotation")]
    [SerializeField] private AudioSource rotationAudioSource;
    [SerializeField] private AudioClip[] rotationClips;
    [SerializeField] private float minRotationVolume = 0.3f;
    [SerializeField] private float maxRotationVolume = 0.6f;

    public void Start()
    {
        if (transform.root.GetComponent<PlayerDuplicateManager>() != null)
        {
            duplicateManager = transform.root.GetComponent<PlayerDuplicateManager>();
        }
    }
    public void SetupShootSound()
    {
        if (duplicateManager==null)
        {
            Debug.LogWarning("duplicate manager not found");
            return;
        }
        PlayShootSound();
        for (int i = 0; i < duplicateManager.DuplicateControllers.Count; i++)
        {
            duplicateManager.DuplicateControllers[i].OneShotAudioHolder.PlayShootSound();
        }

    }
    public void PlayShootSound()
    {
        gunshotAudioSource.pitch = Random.Range(0.9f, 1.1f);
        gunshotAudioSource.PlayOneShot(gunshotClips[Random.Range(0, gunshotClips.Length)]);
        gunshotAudioSource.pitch = 1f;
    }

    public void InitializeRotationSound()
    {
        PlayRotationSound();
        for (int i = 0; i < duplicateManager.DuplicateControllers.Count; i++)
        {
            duplicateManager.DuplicateControllers[i].OneShotAudioHolder.PlayRotationSound();
        }
    }
    void PlayRotationSound()
    {
        AudioClip rotationSound = rotationClips[Random.Range(0, rotationClips.Length)];
        float rotationVolume = Random.Range(minRotationVolume, maxRotationVolume);
        //rotationAudioSource.pitch = Random.Range(-0.85f, 1.15f);
        rotationAudioSource.PlayOneShot(rotationSound, rotationVolume);
    }
    public void InitializeLandSound(float velocityRatio)
    {
        float vol = velocityRatio;
        PlayLandSound(vol);
        for (int i = 0; i < duplicateManager.DuplicateControllers.Count; i++)
        {
            duplicateManager.DuplicateControllers[i].OneShotAudioHolder.PlayLandSound(vol);
        }
    }
    void PlayLandSound(float volume)
    {
        footstepAudioSource.PlayOneShot(landClip, volume);        
    }
    public void InitializeFootstepSound()
    {
        if (duplicateManager == null)
        {
            Debug.LogWarning("duplicate manager not found");
            return;
        }
        PlayFootstepSound();
        for (int i = 0; i < duplicateManager.DuplicateControllers.Count; i++)
        {
            duplicateManager.DuplicateControllers[i].OneShotAudioHolder.PlayFootstepSound();
        }
    }
    void PlayFootstepSound()
    {
        AudioClip footstepSound = footstepClips[Random.Range(0, footstepClips.Length)];
        float footstepVolume = Random.Range(minFootstepVolume, maxFootstepVolume);
        footstepAudioSource.PlayOneShot(footstepSound, footstepVolume);
    }

}
