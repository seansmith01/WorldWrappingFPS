using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudioHandler : MonoBehaviour
{
    private PlayerMovement playerMovement;
    private AudioSource audioSource;
    private Rigidbody rb;

    List<AudioSource> windAudioSources = new List<AudioSource>();
    [SerializeField] AudioSource freeFallAudioSource;
    [SerializeField] float radius;
    [SerializeField] int maxCollidersToListenTo;
    [SerializeField] LayerMask worldStaticMask;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        playerMovement = GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody>();

        GameObject pool = Instantiate(new GameObject("AudioSourcePool"));
        for (int i = 0; i < maxCollidersToListenTo; i++)
        {
            AudioSource newFreefallSound = Instantiate(freeFallAudioSource, pool.transform);
            windAudioSources.Add(newFreefallSound);
        }
    }

    // Initialize a priority queue to keep track of the closest hits.
    public List<Collider> closestColliders = new List<Collider>();

    // In your Update method:
    void Update()
    {
        Collider[] nearbyObjects = Physics.OverlapSphere(transform.position, radius, worldStaticMask);
        if (nearbyObjects.Length == 0)
        {
            return;
        }
        if (playerMovement.CurrentMoveState != PlayerMovement.MoveState.Flying)
        {
            MuteFallingSounds(true);
            return;
        }

        MuteFallingSounds(false);

        // Update the list of closest hits.
        closestColliders.Clear();
        for (int i = 0; i < nearbyObjects.Length; i++)
        {
            closestColliders.Add(nearbyObjects[i]);
        }

        //sort the colliders based on the distance from their closest point to the players position!!!!!!!!!!!!!!
        closestColliders.Sort((a, b) => 
        (a.ClosestPoint(transform.position) - transform.position).magnitude.CompareTo
        ((b.ClosestPoint(transform.position) - transform.position).magnitude));

        int numClosestHits = Mathf.Min(maxCollidersToListenTo, closestColliders.Count);

        //mute all wind sources
        for (int i = 0; i < windAudioSources.Count; i++)
        {
            windAudioSources[i].mute = true;
        }
        // Access the closest colliders if needed.
        for (int i = 0; i < numClosestHits; i++)
        {
            Vector3 closestPosOnCollider = closestColliders[i].ClosestPoint(transform.position);
            //unmute wind source since it's in use
            windAudioSources[i].mute = false;
            windAudioSources[i].transform.position = closestPosOnCollider;

            Debug.DrawLine(transform.position, closestPosOnCollider, Color.red);
        }

        
        //audioSource.pitch = Mathf.Lerp(0.5f, 1, veryclosestPoint / radius);
        
    }
    void MuteFallingSounds(bool isMute)
    {
        //audioSource.mute = isMute;
        for (int i = 0; i < windAudioSources.Count; i++)
        {
            windAudioSources[i].mute = isMute;
        }
    }
}
