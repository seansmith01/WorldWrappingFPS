using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class PlayerAudioHandler : MonoBehaviour
{
    private PlayerMovement playerMovement;
    private PlayerLocalManager playerLocalManager;
    [Header("Misc")]
    [SerializeField] private OneShotAudioHolder oneShotAudioHolder;
    [SerializeField] LayerMask mask;
    [Header("CollidersFallingAudio")]
    List<AudioSource> windAudioSources = new List<AudioSource>();
    [SerializeField] AudioSource colliderFreeFallAudioSource;
    [SerializeField] float radius;
    [SerializeField] int maxCollidersToListenTo;
    [SerializeField] LayerMask worldStaticMask;

    // Initialize a priority queue to keep track of the closest hits.
    private List<Vector3> closestPoints = new List<Vector3>();

    [Header("PlayerFallingWind")]
    [SerializeField] private AudioSource playerFallingAudioSource;
    [SerializeField] private float playerMinFallingWindVolume;
    [SerializeField] private float playerMaxFallingWindVolume;
    [SerializeField] private float otherPlayerMinFallingWindVolume;
    [SerializeField] private float otherPlayerMaxFallingWindVolume;
    [SerializeField] private float playerFallVolumeLerpSpeed = 2f;
    private float currentMinFallingWindVolume;
    private float currentMaxFallingWindVolume;
    [Header("Footsteps")]
    [SerializeField] private float minTimeBetweenFootsteps = 0.3f;
    [SerializeField] private float maxTimeBetweenFootsteps = 0.6f;
    private float timeSinceLastFootstep;
    private float timeSinceLastLand;
    private void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerLocalManager = GetComponent<PlayerLocalManager>();

        if (playerLocalManager.IsFirstPlayerLocal)
        {
            GameObject pool = Instantiate(new GameObject("AudioSourcePool"));
            for (int i = 0; i < maxCollidersToListenTo; i++)
            {
                AudioSource newFreefallSound = Instantiate(colliderFreeFallAudioSource, pool.transform);
                //newFreefallSound.gameObject.SetActive(false);
                windAudioSources.Add(newFreefallSound);
            }

            playerFallingAudioSource.spatialBlend = 0.0f;
            currentMinFallingWindVolume = playerMinFallingWindVolume;
            currentMaxFallingWindVolume = playerMaxFallingWindVolume;
        }
        else
        {
            playerFallingAudioSource.spatialBlend = 1.0f;
            //audioSource.enabled = false;
            currentMinFallingWindVolume = otherPlayerMinFallingWindVolume;
            currentMaxFallingWindVolume = otherPlayerMaxFallingWindVolume;
        }

    }


    [SerializeField] List<Vector3> closestPointsOnColliders = new List<Vector3>();
    // In your Update method:
    void Update()
    {
        Vector3 playerRelativeVelocity = playerMovement.GetRelativeVelocity();

        if (playerMovement.IsGrounded)
        {
            FootstepSounds();
            playerFallingAudioSource.volume = 0f;

        }
        else
        {
            InAirSounds(playerRelativeVelocity.y);
        }
        

        if (!playerLocalManager.IsFirstPlayerLocal)
        {
            return;
        }

        //Code from here only applies to the first player in splitscreeen

        if (Input.GetKeyUp(KeyCode.K))
        {
            if(Time.timeScale == 0)
            {
                Time.timeScale = 1;

            }
            else
            {
                Time.timeScale = 0;
            }
            //transform.position = new Vector3(9, 17, 30);
            GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
        

        if (playerMovement.IsGrounded)
        {
            MuteWindSources();
            return;

        }
            MuteWindSources();
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
        closestPointsOnColliders.Clear();
        for (int i = 0; i < overLappingColliders.Length; i++)
        {
            closestPointsOnColliders.Add(overLappingColliders[i].ClosestPoint(transform.position));
        }
        float downwardsRayRange = 15f;
        float distToGroundRatio = 1f; // set to one so if in air and ray below is hitting nothing, the windsources shuld play full volume
        RaycastHit downwardsHit;
        
        if(Physics.Raycast(transform.position, -transform.up, out downwardsHit, downwardsRayRange, worldStaticMask))
        {
            distToGroundRatio = Mathf.Lerp(0, 1, downwardsHit.distance / downwardsRayRange);

            //distToGroundMultiplier = Mathf.Lerp(distToGroundMultiplier, distToGroundRatio, 150f * Time.deltaTime);
        }

        float playerYVelocityRatio = Mathf.Lerp(0, 1f, playerRelativeVelocity.y / playerMovement.MaxFallSpeed);
        float targetVolume = playerYVelocityRatio * distToGroundRatio;
        //print(targetVolume);
        //sort the colliders based on the distance from their closest point to the players position!!!!!!!!!!!!!!
        closestPointsOnColliders.Sort((a, b) => (a - transform.position).magnitude.CompareTo((b - transform.position).magnitude));
        // Update the list of closest hits
        for (int i = 0; i < closestPointsOnColliders.Count; i++)
        {
            
            Vector3 closestPosOnCollider = closestPointsOnColliders[i];
            if (i == windAudioSources.Count)
                break;
            //if (!isWithinHeightRange(closestPosOnCollider))
            //{
            //    break;
            //}
            windAudioSources[i].gameObject.SetActive(true);
            float relativeYDifference = transform.InverseTransformPoint(transform.position).y - transform.InverseTransformPoint(closestPosOnCollider).y;

            float relativeYDifferenceRatio = Mathf.Lerp(1f, 0f, Mathf.Abs(relativeYDifference) / (radius));

            float distanceFromPlayer = Vector3.Distance(transform.position, closestPosOnCollider);
            float distanceRatio = Mathf.Lerp(1f, 0f, distanceFromPlayer / radius);
            windAudioSources[i].GetComponent<test>().distanceFromPlayerY = relativeYDifference;
            windAudioSources[i].GetComponent<test>().RelativeYDifferenceRatio = relativeYDifferenceRatio;
            windAudioSources[i].GetComponent<test>().distanceRatio = distanceRatio;

            float newTargetVolume = targetVolume * relativeYDifferenceRatio * distanceRatio;
            if (windAudioSources[i].GetComponent<test>().debug)
            {

            }
            windAudioSources[i].GetComponent<test>().targetVol = newTargetVolume;

            windAudioSources[i].volume =  Mathf.Lerp(windAudioSources[i].volume, newTargetVolume, 100f * Time.deltaTime);
            //windAudioSources[i].volume = newTargetVolume;

            windAudioSources[i].transform.position = closestPosOnCollider;
            //Get the distance between the  co-oridinates of  the player and the closest point on the collider that the audio source is attached to
            

            
            Debug.DrawLine(transform.position, closestPosOnCollider, Color.red);
            //colliders.Add(overLappingColliders[i]);
        }
        

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

    void InAirSounds(float relVelocityY)
    {
        float fallVolumeRatio = Mathf.Lerp(currentMinFallingWindVolume, currentMaxFallingWindVolume, relVelocityY / playerMovement.MaxFallSpeed);
        playerFallingAudioSource.volume = Mathf.Lerp(playerFallingAudioSource.volume, fallVolumeRatio, playerFallVolumeLerpSpeed * Time.deltaTime);
    }
    void FootstepSounds()
    {
        Vector3 playerRelativeVelocity = playerMovement.GetRelativeVelocity();
        if (playerRelativeVelocity.magnitude > 5f)
        {
            // Check if enough time has passed to play the next footstep sound
            if (Time.time - timeSinceLastFootstep >= Random.Range(minTimeBetweenFootsteps, maxTimeBetweenFootsteps))
            {
                oneShotAudioHolder.SetupFootstepSound(playerLocalManager.IsFirstPlayerLocal);
                

                timeSinceLastFootstep = Time.time; // Update the time since the last footstep sound
            }
        }
    }
}
