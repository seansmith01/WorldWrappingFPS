using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class PlayerAudioHandler : MonoBehaviour
{
    private PlayerMovement playerMovement;
    private AudioSource fallingAudioSource;

    List<AudioSource> windAudioSources = new List<AudioSource>();
    [SerializeField] AudioSource freeFallAudioSource;
    [SerializeField] float radius;
    [SerializeField] int maxCollidersToListenTo;
    [SerializeField] LayerMask worldStaticMask;

    [SerializeField] LayerMask mask;
    // Initialize a priority queue to keep track of the closest hits.
    public List<Collider> closestColliders = new List<Collider>();
    [SerializeField] private float volumeLerpSpeed;
    private void Start()
    {
        fallingAudioSource = GetComponent<AudioSource>();
        playerMovement = GetComponent<PlayerMovement>();

        GameObject pool = Instantiate(new GameObject("AudioSourcePool"));
        for (int i = 0; i < maxCollidersToListenTo; i++)
        {
            AudioSource newFreefallSound = Instantiate(freeFallAudioSource, pool.transform);
            windAudioSources.Add(newFreefallSound);
        }
    }

    

    // In your Update method:
    void Update()
    {
        Collider[] nearbyObjects = Physics.OverlapSphere(transform.position, radius, worldStaticMask);
        if (nearbyObjects.Length == 0)
        {
            return;
        }
        Vector3 playerRelativeVelocity = playerMovement.GetRelativeVelocity();

        float fallVolumeRatio = Mathf.Lerp(0, 0.25f, playerRelativeVelocity.y / playerMovement.MaxFallSpeed);

        fallingAudioSource.volume = Mathf.Lerp(fallingAudioSource.volume, fallVolumeRatio, volumeLerpSpeed * Time.deltaTime);




        for (int i = 0; i < windAudioSources.Count; i++)
        {
            windAudioSources[i].volume = Mathf.Lerp(windAudioSources[i].volume, fallVolumeRatio * 4f, volumeLerpSpeed * Time.deltaTime);
        }
        // Update the list of closest hits.
        closestColliders.Clear();
        for (int i = 0; i < nearbyObjects.Length; i++)
        {
            closestColliders.Add(nearbyObjects[i]);
            //if (nearbyObjects[i].GetComponent<LevelAudioScript>().inUse == false)
            //{
            //nearbyObjects[i].GetComponent<LevelAudioScript>().inUse = true;
            //closestColliders.Add(nearbyObjects[i]);

            //}
        }

        //sort the colliders based on the distance from their closest point to the players position!!!!!!!!!!!!!!
        closestColliders.Sort((a, b) => 
        (a.ClosestPoint(transform.position) - transform.position).magnitude.CompareTo
        ((b.ClosestPoint(transform.position) - transform.position).magnitude));

        int numClosestHits = Mathf.Min(maxCollidersToListenTo, closestColliders.Count);

        //mute all wind sources
        for (int i = 0; i < windAudioSources.Count; i++)
        {
           // windAudioSources[i].mute = true;
        }
        // Access the closest colliders if needed.
        for (int i = 0; i < numClosestHits; i++)
        {
            Vector3 closestPosOnCollider = closestColliders[i].ClosestPoint(transform.position);
            //unmute wind source since it's in use
           // windAudioSources[i].mute = false;
            windAudioSources[i].transform.position = closestPosOnCollider;

            Debug.DrawLine(transform.position, closestPosOnCollider, Color.red);
        }

        
        //audioSource.pitch = Mathf.Lerp(0.5f, 1, veryclosestPoint / radius);
        
    }
    void MuteFallingSounds(bool isMute)
    {
        //audioSource.mute = isMute;
        
    }
}
