using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.HID;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class PlayerShooting : MonoBehaviour
{
    [SerializeField] LayerMask rotateToAbleObjectLayer;

    [Header("References")]
    [SerializeField] OneShotAudioHolder oneShotAudioHolder;
    [SerializeField] Transform camHolder, gunTip;

    [Header("Gun Settings")]
    [SerializeField] float gunRange;
    [SerializeField] float maxRotateDistance;
    [SerializeField] float maxGrappleDistance = 100f;

    [Header("Line Renderers")]
    [SerializeField] LineRenderer grappleLineRenderer;
    [SerializeField] LineRenderer shootLineRenderer;

    [Header("Grapple Settings")]
    [SerializeField] LayerMask whatIsGrappleable;
    private Vector3 grapplePoint;
    private Vector3 currentGrapplePosition;
    private SpringJoint joint;

    [Header("Player Components")]
    private PlayerDuplicateManager duplicateManager;
    private PlayerInput playerInput;
    private PlayerMovement playerMovement;
    private PlayerLocalManager playerLocalManager;
    private LevelRepeater levelRepeater;
    private int playerID;

    [Header("Shooting")]
    [SerializeField] GameObject bullet;
    [SerializeField] float lineRendererDuration;
    private LayerMask mask;
    [SerializeField] GameObject impactPointGO;

    void Awake()
    {
        playerInput= GetComponent<PlayerInput>();
        playerMovement = GetComponent<PlayerMovement>();
        duplicateManager = GetComponent<PlayerDuplicateManager>();
        playerLocalManager = GetComponent<PlayerLocalManager>();
        levelRepeater = FindFirstObjectByType<LevelRepeater>();
       
    }
    void Start()
    {
        playerID = playerLocalManager.PlayerID; // Must be in start, as it is set in awake
    }
   
    void Update()
    {
        ShootCheck();
        RotateCheck();
    }
    private float timeAtLastShot = 0f;
    [SerializeField] private float shootCooldown = 1f;
    void ShootCheck()
    {
        if (playerInput.actions["Fire"].WasPressedThisFrame() && Time.time - shootCooldown > timeAtLastShot)
        {
            FireRaycast(camHolder.position, gunRange, 0f);

            oneShotAudioHolder.SetupShootSound(playerLocalManager.IsFirstPlayerLocal);

            timeAtLastShot = Time.time;
        }
    }
    private void RotateCheck()
    {
        RaycastHit hit;
        #region UI
        //can rotate
        //if (Physics.Raycast(camHolder.position, camHolder.forward, out hit, maxRotateDistance, rotateToAbleObjectLayer))
        //{
        //    Vector3 floorSurface;
        //    floorSurface = hit.normal;
        //    //if (Vector3.Dot(transform.up, floorSurface) < Mathf.Cos(45 * Mathf.Deg2Rad))
        //    //{
        //        Debug.DrawRay(camHolder.position, camHolder.forward * maxRotateDistance, Color.red);

        //        // outterRing.color = Color.black;
        //        //innerRing.SetActive(true);
        //    //}
        //    //else
        //    //{
        //    //    Debug.DrawRay(camHolder.position, camHolder.forward * maxRotateDistance, Color.blue);

        //    //}
        //}
        //else
        //{
        //    Debug.DrawRay(camHolder.position, camHolder.forward * maxRotateDistance, Color.green);

        //}
        //if (Physics.Raycast(camHolder.position + (camHolder.forward * maxRotateDistance), camHolder.forward, out hit, maxGrappleDistance - maxRotateDistance, whatIsGrappleable))
        //{
        //    //outterRing.color = Color.white;
        //    //innerRing.SetActive(false);
        //}
        //else
        //{
        //   // outterRing.color=Color.black;
        //   // innerRing.SetActive(false);
        //}
        #endregion
        if (playerInput.actions["RotateToSurface"].WasPressedThisFrame())
        {
            //rotate
            if (Physics.Raycast(camHolder.position, camHolder.forward, out hit, maxRotateDistance, rotateToAbleObjectLayer))
            {
                Vector3 floorSurface;
                floorSurface = hit.normal;
                //check is not the same surface as one standing on
                if (Vector3.Dot(transform.up, floorSurface) < Mathf.Cos(45 * Mathf.Deg2Rad))
                {
                    playerMovement.ChangeRotation(floorSurface, hit.point);
                }
            }
        }
    }

    void FireRaycast(Vector3 startRayPos, float rayRange, float totalRayDistance)
    {
        // Get the spacing values for repeated objects
        float repeatSpacingX = levelRepeater.RepeatSpacing.x;
        float repeatSpacingY = levelRepeater.RepeatSpacing.y;
        float repeatSpacingZ = levelRepeater.RepeatSpacing.z;

        // Initialize a RaycastHit variable
        RaycastHit hit;

        // Create a layer mask to control what the ray interacts with
        mask = 1 << gameObject.layer; // Excludes the player's own layer from the raycast
        mask = ~mask;

        // Perform a raycast from the startRayPos in the camera's forward direction
        if (Physics.Raycast(startRayPos, camHolder.forward, out hit, rayRange, mask))
        {
            if (hit.transform.TryGetComponent<PlayerShooting>(out PlayerShooting hitPlayer))
            {
                // Handle when the ray hits a player
                Debug.Log("Player Hit: " + hit.transform.gameObject.name);
                KillOtherPlayer(hitPlayer);
                totalRayDistance += hit.distance;
                DrawLasersAndImpactParticle(camHolder.forward * totalRayDistance, true , hit.normal);
            }
            else if (hit.transform.CompareTag("BoundsTrigger"))
            {
                // Handle when the ray hits a bounds trigger
                print("Player Hit bound");
                print(hit.point);

                // Calculate the new position for the next ray
                Vector3 newRayOffset = Vector3.zero;
                float remainingDist = rayRange - Vector3.Distance(hit.point, startRayPos);

                switch (hit.transform.name)
                {
                    case "-BoundsX": newRayOffset = new Vector3(repeatSpacingX, 0, 0); print("-x"); break;
                    case "BoundsX": newRayOffset = new Vector3(-repeatSpacingX, 0, 0); print("x"); break;
                    case "-BoundsY": newRayOffset = new Vector3(0, repeatSpacingY, 0); print("-y"); break;
                    case "BoundsY": newRayOffset = new Vector3(0, -repeatSpacingY, 0); print("y"); break;
                    case "-BoundsZ": newRayOffset = new Vector3(0, 0, repeatSpacingZ); print("-z"); break;
                    case "BoundsZ": newRayOffset = new Vector3(0, 0, -repeatSpacingZ); print("z"); break;
                }

                Vector3 newRayPos = hit.point + newRayOffset;
                Debug.DrawLine(startRayPos, hit.point, Color.green, 5f);

                float newTotalRayDistance = totalRayDistance + hit.distance;
                // Recursively fire a new ray from the adjusted position
                FireRaycast(newRayPos, remainingDist, newTotalRayDistance);
            }
            else
            {
                // Handle when the ray hits a wall
                
                totalRayDistance += hit.distance;
                DrawLasersAndImpactParticle(camHolder.forward * totalRayDistance, true, hit.normal);
                Debug.DrawLine(startRayPos, hit.point, Color.green, 5f);
                Debug.Log("Hit a wall: " + hit.transform.gameObject.name);
                Debug.Log("Hit distance: " + hit.distance);
            }
        }
        else
        {
            // Handle when the ray doesn't hit anything
            Debug.DrawRay(startRayPos, camHolder.forward * rayRange, Color.green, 5f);
            // Ray missed everything so it's laser range is the max possible
            DrawLasersAndImpactParticle(camHolder.forward * gunRange, false, Vector3.zero);
        }
    }


    void DrawLasersAndImpactParticle(Vector3 hitOffset, bool hitSomething, Vector3 hitNormal)
    {
        LineRenderer shootLineRender = Instantiate(shootLineRenderer);
        shootLineRender.transform.parent = transform;

        shootLineRender.SetPosition(0, gunTip.position);
        shootLineRender.SetPosition(1, camHolder.position + hitOffset);

        StartCoroutine(DestroyLineRenderer(shootLineRender, lineRendererDuration)); // destroy after x secs

        if (hitSomething)
        {
            GameObject ImpactInstance = Instantiate(impactPointGO, camHolder.position + hitOffset + (hitNormal / 2f), Quaternion.identity);
            Destroy(ImpactInstance, 0.5f);
        }
        
        for (int i = 0; i < duplicateManager.DuplicateControllers.Count; i++)
        {
            LineRenderer dupShootLineRender = Instantiate(shootLineRenderer);
        
            Transform dupGunTip = duplicateManager.DuplicateControllers[i].GunTip;
            Transform dupCamHolder = duplicateManager.DuplicateControllers[i].CameraHolder;
            dupShootLineRender.SetPosition(0, dupGunTip.position);
            dupShootLineRender.SetPosition(1, dupCamHolder.position + hitOffset);
            StartCoroutine(DestroyLineRenderer(dupShootLineRender, lineRendererDuration)); // destroy after x secs

            if (hitSomething)
            {
                GameObject dupImpactInstance = Instantiate(impactPointGO, dupCamHolder.position + hitOffset, Quaternion.identity);
                Destroy(dupImpactInstance, 0.5f);
            }

                
        }
    }
    //void AAAAAGetRaycastHit()
    //{
    //    // bool playerHitSomething = false;
    //    // bool duphitSomething = false;
    //    //bool hasHitPlayer = false;
    //    //hitFromDuplicate = false;

    //    //diagonalDistance = Mathf.Sqrt(gunRange * gunRange + gunRange * gunRange);

    //    //// Calculate the direction based on the angle in degrees.
    //    //float radians = transform.eulerAngles.y * Mathf.Deg2Rad;
    //    //Vector3 direction = new Vector3(Mathf.Cos(radians), 0.0f, Mathf.Sin(radians));

    //    //// Debug.DrawLine is used to visualize the ray in the Unity editor.
    //    //Debug.DrawLine(camHolder.position, camHolder.position + direction * 100, Color.green, 2.0f);
    //    Debug.DrawRay(camHolder.position + (-camHolder.forward * 100), camHolder.forward * gunRange, Color.green, 5f);

    //    RaycastHit[] playerHits = Physics.RaycastAll(camHolder.position, camHolder.forward, gunRange);
    //    System.Array.Sort(playerHits, (a, b) => a.distance.CompareTo(b.distance));

    //    foreach (RaycastHit playerHit in playerHits)
    //    {
    //        //doesn't work for headshots currently so head collider is removed
    //        if (playerHit.transform.TryGetComponent<PlayerShooting>(out PlayerShooting hitPlayer))
    //        {
    //            // If player hits self, ignore 
    //            if (hitPlayer == this)
    //            {
    //                print("Player Hit self");
    //                continue; // contine for loop (check next ray)
    //            }
    //            else
    //            {
    //                print("Player Hit Other Player");
    //                // hasHitPlayer = true;
    //                KillOtherPlayer(hitPlayer);
    //                DrawLasers(gunTip.position, playerHit.point);
    //                return;
    //            }
    //        }
    //        else
    //        {
    //            print("Player Hit wall");
    //            DrawLasers(gunTip.position, playerHit.point);

    //            return;
    //        }
    //    }

    //    foreach (DuplicateController duplicate in duplicateManager.DuplicateControllers)
    //    {
    //        Transform dupCamHolder = duplicate.CameraHolder;
    //        Transform dupGunTip = duplicate.GunTip;
    //        RaycastHit[] duplicateHits = Physics.RaycastAll(dupCamHolder.position, camHolder.forward, gunRange);
    //        System.Array.Sort(duplicateHits, (a, b) => a.distance.CompareTo(b.distance));

    //        foreach (RaycastHit duplicateHit in duplicateHits)
    //        {
    //            //hitFromDuplicate = true;
    //            if (duplicateHit.transform.TryGetComponent<PlayerShooting>(out PlayerShooting hitPlayer))
    //            {
    //                if(hitPlayer == this)
    //                {
    //                    KillOtherPlayer(hitPlayer);
    //                    print("Dup Hit self");
    //                }
    //                else
    //                {
    //                    print("Dup Hit OtherPlayer");
    //                    //hasHitPlayer = true;
    //                    KillOtherPlayer(hitPlayer);
    //                    DrawLasers(dupGunTip.position, duplicateHit.point);

    //                }
    //                return;
    //            }
    //            else
    //            {
    //                DrawLasers(dupGunTip.position, duplicateHit.point);
    //                print("Dup Hit something");
    //                return;
    //            }
    //        }
    //    }
    //    DrawLasers(gunTip.position, gunTip.position + (gunTip.forward*gunRange));

    //}

    private static void KillOtherPlayer(PlayerShooting hitPlayer)
    {
        // hitPlayer.transform.position = Vector3.zero;
        hitPlayer.GetComponent<PlayerLocalManager>().Die();
    }

    

    IEnumerator DestroyLineRenderer(LineRenderer lr, float duration)
    {
        lr.widthMultiplier = 1;
        float time = 0f;

        while (time < 1)
        {
            time += Time.deltaTime / duration;
            lr.widthMultiplier = Mathf.Lerp(1, 0, time);
            yield return null;
        }
        Destroy(lr.gameObject);
    }
    //Called after Update
    void LateUpdate()
    {
        DrawRope();
    }

    /// <summary>
    /// Call whenever we want to start a grapple
    /// </summary>
    void StartGrapple(RaycastHit hit)
    {
        grapplePoint = hit.point;
        joint = gameObject.AddComponent<SpringJoint>();
        joint.autoConfigureConnectedAnchor = false;
        joint.connectedAnchor = grapplePoint;

        float distanceFromPoint = Vector3.Distance(transform.position, grapplePoint);

        //The distance grapple will try to keep from grapple point. 
        joint.maxDistance = distanceFromPoint * 0.5f;
        joint.minDistance = distanceFromPoint * 0.25f;

        //Adjust these values to fit your game.
        joint.spring = 4.5f;
        joint.damper = 7f;
        joint.massScale = 4.5f;

        grappleLineRenderer.positionCount = 2;
        currentGrapplePosition = gunTip.position;
    }


    /// <summary>
    /// Call whenever we want to stop a grapple
    /// </summary>
    void StopGrapple()
    {
        grappleLineRenderer.positionCount = 0;
        Destroy(joint);
    }


    void DrawRope()
    {
        //If not grappling, don't draw rope
        if (!joint) return;

        currentGrapplePosition = Vector3.Lerp(currentGrapplePosition, grapplePoint, Time.deltaTime * 8f);

        grappleLineRenderer.SetPosition(0, gunTip.position);
        grappleLineRenderer.SetPosition(1, currentGrapplePosition);
    }

    public bool IsGrappling()
    {
        return joint != null;
    }

    public Vector3 GetGrapplePoint()
    {
        return grapplePoint;
    }

    public void SetGrapplingPoint(Vector3 newGrapplePoint)
    {
        Vector3 diff = newGrapplePoint - grapplePoint;

        grapplePoint = newGrapplePoint;
        joint.connectedAnchor = grapplePoint;

        currentGrapplePosition += diff;
    }

}
