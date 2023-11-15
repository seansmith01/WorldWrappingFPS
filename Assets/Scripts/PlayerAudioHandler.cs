using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class PlayerAudioHandler : MonoBehaviour
{
    private PlayerMovement playerMovement;
    private PlayerLocalManager playerLocalManager;
    private PlayerDuplicateManager duplicateManager;
    [Header("Misc")]
    [SerializeField] private OneShotAudioHolder oneShotAudioHolder;
    [SerializeField] LayerMask mask;
    [Header("CollidersFallingAudio")]
    List<AudioSource> collisionWindAudioSources = new List<AudioSource>();
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
    [Header("Footsteps")]
    [SerializeField] private float minTimeBetweenFootsteps = 0.3f;
    [SerializeField] private float maxTimeBetweenFootsteps = 0.6f;
    private float timeSinceLastFootstep;
    private void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerLocalManager = GetComponent<PlayerLocalManager>();
        duplicateManager = GetComponent<PlayerDuplicateManager>();

        if (playerLocalManager.IsFirstPlayerLocal)
        {
            GameObject pool = Instantiate(new GameObject("AudioSourcePool"));
            for (int i = 0; i < maxCollidersToListenTo; i++)
            {
                AudioSource newFreefallSound = Instantiate(colliderFreeFallAudioSource, pool.transform);
                //newFreefallSound.gameObject.SetActive(false);
                collisionWindAudioSources.Add(newFreefallSound);
            }

            playerFallingAudioSource.spatialBlend = 1.0f;
        }
        else
        {
            playerFallingAudioSource.spatialBlend = 1.0f;
        }

    }


    [SerializeField] List<Vector3> closestPointsOnColliders = new List<Vector3>();
    // In your Update method:
    void Update()
    {
        Vector3 playerRelativeVelocity = playerMovement.GetRelativeVelocity();

        // Handle audio for all players
        if (playerMovement.IsGrounded)
        {
            FootstepSounds();

        }
        ManageFallingSounds(playerRelativeVelocity.y);


        if (!playerLocalManager.IsFirstPlayerLocal)
        {
            return;
        }
        // Handle audio only for player 1

        // Mute all the colliders auido source, will unmute if sphere collider registers and assigns them
        MuteColliderWindSources();
        // Return if grounded
        if (playerMovement.IsGrounded)
        {
            return;
        }

        HandleNearbyCollidersWindAudio(playerRelativeVelocity);
    }

    private void HandleNearbyCollidersWindAudio(Vector3 playerRelativeVelocity)
    {
        // Cast an overlap sphere over objects in the world static mask
        Collider[] overLappingColliders = Physics.OverlapSphere(transform.position, radius, worldStaticMask);
        // If no colliders are in the sphere, return
        if (overLappingColliders.Length == 0)
        {
            return;
        }
        // The the list of closest points on the colliders
        closestPointsOnColliders.Clear();
        // Cycle through each collider in the sphere cast
        for (int i = 0; i < overLappingColliders.Length; i++)
        {
            // Get the closest point on the collider relative to the player
            Vector3 closestPointOnCollider = overLappingColliders[i].ClosestPoint(transform.position);
            // Add the closest point to the list of vectors
            closestPointsOnColliders.Add(closestPointOnCollider);
        }
        // The program will now move the instantiated audio sources to the position of these closest point vectors
        // But first, it will scale their volumes based on a series of varibles
        // These variables will be scaled to be between 0 and 1 and all multiplied together to get the final volume for the audio source
        // * How far the player is away from the ground (0 when the player is closest to ground, 1 when equal or further than set value e.g 15 units)
        // * How fast the player is falling (0 when not falling, 1 when falling at max speed)
        // * How far the player is away from the collider (0 when distance is equal to radius of sphere (furthest away), 1 when at closest possible distance)
        // * How far the player vertical Y position is from the collider's 
        //   (0 when the player's Y position is as far as it can be from the collider's, 1 when player's Y position is equal to the closest point's Y position)


        // Set a float to be between 0 and 1 depending on how close player is to ground
        float downwardsRayRange = 15f;
        float distToGroundRatio;

        RaycastHit downwardsHit;
        // Cast a ray in the player's local down direction
        if (Physics.Raycast(transform.position, -transform.up, out downwardsHit, downwardsRayRange, worldStaticMask))
        {
            // If the ray hits, set the ratio value to be between 0 and 1 depending on the ray's distance divided by its max potential distance
            distToGroundRatio = Mathf.Lerp(0, 1, downwardsHit.distance / downwardsRayRange);
        }
        else
        {
            //If the ray misses, the player is high in the air therefore the audio sources on the colliders should play full volume
            distToGroundRatio = 1f;
        }
        // Set a float to be between 0 and 1 depening on the players current falling speed divided by their potential max speed
        // If they are falling at max speed, this value will be 1
        float playerYVelocityRatio = Mathf.Lerp(0, 1f, playerRelativeVelocity.y / playerMovement.MaxFallSpeed);
        float targetVolume = playerYVelocityRatio * distToGroundRatio;
        //sort the colliders based on the distance from their closest point to the players position
        //closestPointsOnColliders.Sort((a, b) => (a - transform.position).magnitude.CompareTo((b - transform.position).magnitude));

        // Update the list of closest hits
        for (int i = 0; i < closestPointsOnColliders.Count; i++)
        {

            Vector3 closestPosOnCollider = closestPointsOnColliders[i];
            if (i == collisionWindAudioSources.Count)
                break;
            // Get the difference in the Y positions between the player and the closest point relative to the player's rotation
            float relativeYDifference = transform.InverseTransformPoint(transform.position).y - transform.InverseTransformPoint(closestPosOnCollider).y;
            // Set a float to be between 1 and 0 depending on the relative Y difference divided by the radius (the max possible difference)
            float relativeYDifferenceRatio = Mathf.Lerp(1f, 0f, Mathf.Abs(relativeYDifference) / radius);

            // Set a float to be between 0 and 1 depening on the distance between the player and the closest point
            float distanceFromPlayer = Vector3.Distance(transform.position, closestPosOnCollider);
            float distanceRatio = Mathf.Lerp(1f, 0f, distanceFromPlayer / radius);


            //collisionWindAudioSources[i].GetComponent<test>().distanceFromPlayerY = relativeYDifference;
            //collisionWindAudioSources[i].GetComponent<test>().RelativeYDifferenceRatio = relativeYDifferenceRatio;
            //collisionWindAudioSources[i].GetComponent<test>().distanceRatio = distanceRatio;

            // Set the audio source target volume to be all the ratio's multiplied together
            float newTargetVolume = targetVolume * relativeYDifferenceRatio * distanceRatio;
            //collisionWindAudioSources[i].GetComponent<test>().targetVol = newTargetVolume;

            // Lerp the volume so there's not any major sudden changes in audio volumes. mainly for when walking off a platform, audio can go up too quick
            collisionWindAudioSources[i].volume = Mathf.Lerp(collisionWindAudioSources[i].volume, newTargetVolume, 100f * Time.deltaTime);
            // Set the position of the audio source to be the closest point
            collisionWindAudioSources[i].transform.position = closestPosOnCollider;
            //collisionWindAudioSources[i].pitch= 1;


            Debug.DrawLine(transform.position, closestPosOnCollider, Color.red);
            //colliders.Add(overLappingColliders[i]);
        }
    }

    private void MuteColliderWindSources()
    {
        for (int i = 0; i < maxCollidersToListenTo; i++)
        {
            collisionWindAudioSources[i].volume = 0f;
        }
    }

    void ManageFallingSounds(float relVelocityY)
    {
        float fallVolumeRatio = Mathf.Lerp(0, 0.3f, relVelocityY / playerMovement.MaxFallSpeed);
        //print(fallVolumeRatio);
        float newVolume = Mathf.Lerp(playerFallingAudioSource.volume, fallVolumeRatio, playerFallVolumeLerpSpeed * Time.deltaTime);
        if (duplicateManager == null)
        {
            Debug.LogWarning("duplicate manager not found");
            return;
        }
        playerFallingAudioSource.volume = newVolume;
        for (int i = 0; i < duplicateManager.DuplicateControllers.Count; i++)
        {
            duplicateManager.DuplicateControllers[i].FallingAudioSource.volume = newVolume;
        }

    }
    void FootstepSounds()
    {
        Vector3 playerRelativeVelocity = playerMovement.GetRelativeVelocity();
        if (playerRelativeVelocity.magnitude > 5f)
        {
            // Check if enough time has passed to play the next footstep sound
            if (Time.time - timeSinceLastFootstep >= Random.Range(minTimeBetweenFootsteps, maxTimeBetweenFootsteps))
            {
                oneShotAudioHolder.InitializeFootstepSound();
                

                timeSinceLastFootstep = Time.time; // Update the time since the last footstep sound
            }
        }
    }
}
