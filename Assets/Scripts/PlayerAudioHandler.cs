using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class PlayerAudioHandler : MonoBehaviour
{
    private PlayerMovement playerMovement;
    private AudioSource fallingAudioSource;
    [SerializeField] private OneShotAudioHolder oneShotAudioHolder;

    List<AudioSource> windAudioSources = new List<AudioSource>();
    [SerializeField] AudioSource freeFallAudioSource;
    [SerializeField] float radius;
    [SerializeField] int maxCollidersToListenTo;
    [SerializeField] LayerMask worldStaticMask;

    [SerializeField] LayerMask mask;
    // Initialize a priority queue to keep track of the closest hits.
    private List<Vector3> closestPoints = new List<Vector3>();
    [SerializeField] private float volumeLerpSpeed;

    [Header("Footsteps")]
    [SerializeField] private float minTimeBetweenFootsteps = 0.3f;
    [SerializeField] private float maxTimeBetweenFootsteps = 0.6f;
    private float timeSinceLastFootstep;
    private void Start()
    {
        fallingAudioSource = GetComponent<AudioSource>();
        playerMovement = GetComponent<PlayerMovement>();

        GameObject pool = Instantiate(new GameObject("AudioSourcePool"));
        for (int i = 0; i < maxCollidersToListenTo; i++)
        {
            AudioSource newFreefallSound = Instantiate(freeFallAudioSource, pool.transform);
            //newFreefallSound.gameObject.SetActive(false);
            windAudioSources.Add(newFreefallSound);
        }
    }

    

    // In your Update method:
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            transform.position = new Vector3(9, 17, 30);
            GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
        Vector3 playerRelativeVelocity = playerMovement.GetRelativeVelocity();
        MuteWindSources();
        float fallVolumeRatio = Mathf.Lerp(0, 0.25f, playerRelativeVelocity.y / playerMovement.MaxFallSpeed);
        fallingAudioSource.volume = Mathf.Lerp(fallingAudioSource.volume, fallVolumeRatio, volumeLerpSpeed * Time.deltaTime);

        if (playerMovement.IsGrounded)
        {
            GroundedSounds();
            return;
        }
        //NOT GROUNDED
        #region MultipleRaycastMethod
        //GetClosestPointOnColliders();
        //if(closestPoints.Count == 0)
        //{
        //    return;
        //}
        //for (int i = 0; i < closestPoints.Count; i++)
        //{
        //    if (i == windAudioSources.Count)
        //        break;
        //    windAudioSources[i].gameObject.SetActive(true);
        //    float windVolumeRatio = Mathf.Lerp(0, 1f, playerRelativeVelocity.y / playerMovement.MaxFallSpeed);
        //    windAudioSources[i].volume = windVolumeRatio;
        //    windAudioSources[i].transform.position = closestPoints[i];
        //    Debug.DrawLine(transform.position, closestPoints[i], Color.red);
        //}
        //return;
        #endregion
        //Sphere overlap method
        Collider[] overLappingColliders = Physics.OverlapSphere(transform.position, radius, worldStaticMask);
        if (overLappingColliders.Length == 0)
        {
            return;
        }

        // Update the list of closest hits.
        //colliders.Clear();
        for (int i = 0; i < overLappingColliders.Length; i++)
        {
            
            Vector3 closestPosOnCollider = overLappingColliders[i].ClosestPoint(transform.position);
            if (i == windAudioSources.Count)
                break;
            //if (!isWithinHeightRange(closestPosOnCollider))
            //{
            //    break;
            //}
            windAudioSources[i].gameObject.SetActive(true);
            float windVolumeRatio = Mathf.Lerp(0, 1f, playerRelativeVelocity.y / playerMovement.MaxFallSpeed);
            windAudioSources[i].volume = windVolumeRatio;
            //windAudioSources[i].volume = Mathf.Lerp(fallingAudioSource.volume, windVolumeRatio, volumeLerpSpeed * Time.deltaTime);
            windAudioSources[i].transform.position = closestPosOnCollider;
            Debug.DrawLine(transform.position, closestPosOnCollider, Color.red);
            //colliders.Add(overLappingColliders[i]);
        }

        //sort the colliders based on the distance from their closest point to the players position!!!!!!!!!!!!!!
        //closestColliders.Sort((a, b) => 
        //(a.ClosestPoint(transform.position) - transform.position).magnitude.CompareTo
        //((b.ClosestPoint(transform.position) - transform.position).magnitude));

        //int numClosestHits = Mathf.Min(maxCollidersToListenTo, colliders.Count);

        // Access the closest colliders if needed.
        //for (int i = 0; i < numClosestHits; i++)
        //{
        //    Vector3 closestPosOnCollider = colliders[i].ClosestPoint(transform.position);
        //    //unmute wind source since it's in use
        //   // windAudioSources[i].mute = false;
        //    windAudioSources[i].transform.position = closestPosOnCollider;

        //}


        //audioSource.pitch = Mathf.Lerp(0.5f, 1, veryclosestPoint / radius);

    }
    bool isWithinHeightRange(Vector3 closestPoint)
    {
        // Calculate the relative direction
        Vector3 relativeDir = closestPoint - transform.position;

        // Transform the relative direction into the player's local space
        Vector3 localRelativeDir = transform.InverseTransformPoint(relativeDir);

        // Check if the absolute Y component is within the vertical range
        if (Mathf.Abs(localRelativeDir.y) <= 5f)
        {
            return true;
        }
        return false;
    }
    void GetClosestPointOnColliders()
    {
        closestPoints.Clear();
        int rayCount = 8; // Number of rays in the 360-degree arc
        // Calculate the angle between each ray
        float angleStep = 360f / rayCount;
        //Raycast method
        for (int i = 0; i < rayCount; i++)
        {
            // Calculate the angle for the current ray
            float angle = i * angleStep;

            // Convert the angle to a direction vector
            Vector3 rayDirection = Quaternion.Euler(0, angle, 0) * transform.forward;

            // Perform the raycast
            RaycastHit hit;
            Debug.DrawRay(transform.position, rayDirection * radius, Color.green);
            if (Physics.Raycast(transform.position, rayDirection, out hit, radius, worldStaticMask))
            {
                Vector3 closestPosOnCollider = hit.collider.ClosestPoint(transform.position);
                closestPoints.Add(closestPosOnCollider);
            }
        }
    }
    private void MuteWindSources()
    {
        for (int i = 0; i < maxCollidersToListenTo; i++)
        {
            windAudioSources[i].volume = 0f;
            //windAudioSources[i].gameObject.SetActive(false);
        }
    }

    void GroundedSounds()
    {
        Vector3 playerRelativeVelocity = playerMovement.GetRelativeVelocity();
        if (playerRelativeVelocity.magnitude > 5f)
        {
            // Check if enough time has passed to play the next footstep sound
            if (Time.time - timeSinceLastFootstep >= Random.Range(minTimeBetweenFootsteps, maxTimeBetweenFootsteps))
            {
                oneShotAudioHolder.PlayFootstepSound();

                timeSinceLastFootstep = Time.time; // Update the time since the last footstep sound
            }
        }
    }
}
