using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneShotAudioHolder : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] gunshotClips;
    public void PlayShootSound()
    {
        audioSource.pitch=Random.Range(0.9f, 1.1f);
        audioSource.PlayOneShot(gunshotClips[Random.Range(0, gunshotClips.Length)]);
        audioSource.pitch = 1f;
    }
}
