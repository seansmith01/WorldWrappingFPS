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

    public void Start()
    {
        if (transform.root.GetComponent<PlayerDuplicateManager>() != null)
        {
            duplicateManager = transform.root.GetComponent<PlayerDuplicateManager>();
        }
    }
    public void SetupShootSound(bool isLocalPlayer)
    {
        if (duplicateManager==null)
        {
            Debug.LogWarning("duplicate manager not found");
            return;
        }
        if (isLocalPlayer)
        {
            for (int i = 0; i < duplicateManager.DuplicateControllers.Count; i++)
            {
                duplicateManager.DuplicateControllers[i].OneShotAudioHolder.PlayShootSound();
            }
            gunshotAudioSource.spatialBlend = 0;
            PlayShootSound(); // can play at volume for local player
        }
        else
        {
            for (int i = 0; i < duplicateManager.DuplicateControllers.Count; i++)
            {
                duplicateManager.DuplicateControllers[i].OneShotAudioHolder.PlayShootSound();
            }
            gunshotAudioSource.spatialBlend = 1;
            PlayShootSound();
        }
    }
    public void PlayShootSound()
    {
        gunshotAudioSource.pitch = Random.Range(0.9f, 1.1f);
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
    public void SetupFootstepSound(bool isLocalPlayer)
    {
        if (duplicateManager == null)
        {
            Debug.LogWarning("duplicate manager not found");
            return;
        }
        if (isLocalPlayer)
        {
            for (int i = 0; i < duplicateManager.DuplicateControllers.Count; i++)
            {
                duplicateManager.DuplicateControllers[i].OneShotAudioHolder.PlayFootstepSound();
            }
            footstepAudioSource.spatialBlend = 0;
            PlayFootstepSound(); // can play at volume for local player
        }
        else
        {
            for (int i = 0; i < duplicateManager.DuplicateControllers.Count; i++)
            {
                duplicateManager.DuplicateControllers[i].OneShotAudioHolder.PlayFootstepSound();
            }
            footstepAudioSource.spatialBlend = 1;
            PlayFootstepSound();
        }
    }
    void PlayFootstepSound()
    {
        AudioClip footstepSound = footstepClips[Random.Range(0, footstepClips.Length)];
        float footstepVolume = Random.Range(localMinFootstepVolume, localMaxFootstepVolume);
        footstepAudioSource.PlayOneShot(footstepSound, footstepVolume);
    }

}
