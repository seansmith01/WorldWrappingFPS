using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.HID;
using UnityEngine.UI;

public class PlayerShooting : MonoBehaviour
{
    [SerializeField] LayerMask rotateToAbleObjectLayer;
    LevelRepeater levelRepeater;
   // [SerializeField] GameObject innerRing;
   // [SerializeField] Image outterRing;
    [SerializeField] Transform camHolder, gunTip;
    [SerializeField] float gunRange;

    [Header("Line Renderers")]
    [SerializeField] LineRenderer grappleLineRenderer;
    [SerializeField] LineRenderer shootLineRenderer;

    private Vector3 grapplePoint;
    private Vector3 currentGrapplePosition;
    public LayerMask whatIsGrappleable;
    [SerializeField] float maxRotateDistance;
    [SerializeField] float maxGrappleDistance = 100f;
    private SpringJoint joint;

    PlayerDuplicateManager duplicateManager;
    PlayerInput playerInput;
    PlayerMovement playerMovement;
    int playerNumber;
    // Start is called before the first frame update
    void Awake()
    {
        playerInput= GetComponent<PlayerInput>();
        playerMovement = GetComponent<PlayerMovement>();
        duplicateManager = GetComponent<PlayerDuplicateManager>();
        levelRepeater = FindFirstObjectByType<LevelRepeater>();
    }
    private void Start()
    {
        playerNumber = GetComponent<PlayerLocalManager>().PlayerNumber;
    }
   
    // Update is called once per frame
    void Update()
    {
        ShootCheck();

        RaycastHit hit;
        #region UI
        //can rotate
        if (Physics.Raycast(camHolder.position, camHolder.forward, out hit, maxRotateDistance, rotateToAbleObjectLayer))
        {
            Vector3 floorSurface;
            floorSurface = hit.normal;
            //if (Vector3.Dot(transform.up, floorSurface) < Mathf.Cos(45 * Mathf.Deg2Rad))
            //{
                Debug.DrawRay(camHolder.position, camHolder.forward * maxRotateDistance, Color.red);

                // outterRing.color = Color.black;
                //innerRing.SetActive(true);
            //}
            //else
            //{
            //    Debug.DrawRay(camHolder.position, camHolder.forward * maxRotateDistance, Color.blue);

            //}
        }
        else
        {
            Debug.DrawRay(camHolder.position, camHolder.forward * maxRotateDistance, Color.green);

        }
        if (Physics.Raycast(camHolder.position + (camHolder.forward * maxRotateDistance), camHolder.forward, out hit, maxGrappleDistance - maxRotateDistance, whatIsGrappleable))
        {
            //outterRing.color = Color.white;
            //innerRing.SetActive(false);
        }
        else
        {
           // outterRing.color=Color.black;
           // innerRing.SetActive(false);
        }
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
                print(hit.normal);
            }
            //else if (Physics.Raycast(camHolder.position + (camHolder.forward*maxRotateDistance), camHolder.forward, out hit, maxGrappleDistance - maxRotateDistance, whatIsGrappleable))
            //{
            //    StartGrapple(hit);
            //}            
        }
        //else if (Input.GetMouseButtonUp(1) && IsGrappling())
        //{
        //    StopGrapple();
        //}
    }
    [SerializeField] GameObject bullet;
    void ShootCheck()
    {
        if (playerInput.actions["Fire"].WasPressedThisFrame())
        {
            //Instantiate(bullet, gunTip.position, camHolder.rotation);
            //return;
            RaycastHit hit = new RaycastHit();
            bool hitFromDuplicate = false;
            bool hitOtherPlayer = false;
            GetRaycastHit();

            //DrawLasers(hit);
            //
            //
            //if (hit.collider == null)
            //{
            //    print("miss");
            //    return;
            //}
            //if(hitOtherPlayer)
            //{
            //    print("kill;");
            //}
            // Hit something

            
        }

    }
    void DrawLasers(Vector3 gunTipPos, Vector3 endPoint)
    {
        LineRenderer shootLineRender = Instantiate(shootLineRenderer);
        shootLineRender.transform.parent = transform;
        StartCoroutine(DestroyLineRenderer(shootLineRender, 2f)); // destroy after x secs
        

        shootLineRender.endWidth = 0.1f;
        shootLineRender.startWidth = 0.1f;

        float shotDistance = (endPoint - gunTipPos).magnitude;

        Vector3 endOffset = ((endPoint - gunTipPos).normalized * shotDistance);

        shootLineRender.SetPosition(0, gunTip.position);
        shootLineRender.SetPosition(1, gunTip.position + endOffset);

        for (int i = 0; i < duplicateManager.DuplicateControllers.Count; i++)
        {
            LineRenderer dupShootLineRender = Instantiate(shootLineRenderer);
            dupShootLineRender.transform.parent = transform;
            StartCoroutine(DestroyLineRenderer(dupShootLineRender, 2f)); // destroy after x secs
        
        
            dupShootLineRender.endWidth = 0.1f;
            dupShootLineRender.startWidth = 0.1f;
        
            Vector3 dupGunTip = duplicateManager.DuplicateControllers[i].GunTip.position;
            dupShootLineRender.SetPosition(0, dupGunTip);
            dupShootLineRender.SetPosition(1, dupGunTip + endOffset);
        }

    }
    void CheckHit(RaycastHit hit)
    {
        //body shot
        //if (hit.collider.GetComponent<PlayerLocalManager>() != null)
        //{
        //    if (hit.collider.GetComponent<PlayerLocalManager>().PlayerNumber != playerNumber)
        //    {
        //        FindObjectOfType<GameManager>().UpdateScore(playerNumber);
        //    }
        //}
        ////headshot
        //if (hit.collider.GetComponentInParent<PlayerLocalManager>() != null)
        //{
        //    if (hit.collider.GetComponentInParent<PlayerLocalManager>().PlayerNumber != playerNumber)
        //    {
        //        FindObjectOfType<GameManager>().UpdateScore(playerNumber);
        //    }
        //}
        ////dupshot body
        //if (hit.collider.GetComponent<DuplicateController>() != null)
        //{
        //    if (hit.collider.GetComponent<DuplicateController>().PlayerNumber != playerNumber)
        //    {
        //        FindObjectOfType<GameManager>().UpdateScore(playerNumber);
        //    }
        //}
        ////hed dup
        //if (hit.collider.GetComponentInParent<DuplicateController>() != null)
        //{
        //    if (hit.collider.GetComponentInParent<DuplicateController>().PlayerNumber != playerNumber)
        //    {
        //        FindObjectOfType<GameManager>().UpdateScore(playerNumber);
        //    }
        //}

    }
    void GetRaycastHit()
    {
       // bool playerHitSomething = false;
       // bool duphitSomething = false;
        //bool hasHitPlayer = false;
        //hitFromDuplicate = false;

        RaycastHit[] playerHits = Physics.RaycastAll(camHolder.position, camHolder.forward, gunRange);
        System.Array.Sort(playerHits, (a, b) => a.distance.CompareTo(b.distance));

        foreach (RaycastHit playerHit in playerHits)
        {
            //doesn't work for headshots currently so head collider is removed
            if (playerHit.transform.TryGetComponent<PlayerShooting>(out PlayerShooting hitPlayer))
            {
                // If player hits self, ignore 
                if (hitPlayer == this)
                {
                    print("Player Hit self");
                    continue; // contine for loop (check next ray)
                }
                else
                {
                    print("Player Hit Other Player");
                    // hasHitPlayer = true;
                    KillOtherPlayer(hitPlayer);
                    DrawLasers(gunTip.position, playerHit.point);
                    return;
                }
            }
            else
            {
                print("Player Hit wall");
                DrawLasers(gunTip.position, playerHit.point);

                return;
            }
        }

        foreach (DuplicateController duplicate in duplicateManager.DuplicateControllers)
        {
            Transform dupCamHolder = duplicate.CameraHolder;
            Transform dupGunTip = duplicate.GunTip;
            RaycastHit[] duplicateHits = Physics.RaycastAll(dupCamHolder.position, camHolder.forward, gunRange);
            System.Array.Sort(duplicateHits, (a, b) => a.distance.CompareTo(b.distance));

            foreach (RaycastHit duplicateHit in duplicateHits)
            {
                //hitFromDuplicate = true;
                if (duplicateHit.transform.TryGetComponent<PlayerShooting>(out PlayerShooting hitPlayer))
                {
                    if(hitPlayer == this)
                    {
                        KillOtherPlayer(hitPlayer);
                        print("Dup Hit self");
                    }
                    else
                    {
                        print("Dup Hit OtherPlayer");
                        //hasHitPlayer = true;
                        KillOtherPlayer(hitPlayer);
                        DrawLasers(dupGunTip.position, duplicateHit.point);

                    }
                    return;
                }
                else
                {
                    DrawLasers(dupGunTip.position, duplicateHit.point);
                    print("Dup Hit something");
                    return;
                }
            }
        }
        DrawLasers(gunTip.position, gunTip.position + (gunTip.forward*gunRange));

    }

    private static void KillOtherPlayer(PlayerShooting hitPlayer)
    {
        for (int i = 0; i < hitPlayer.GetComponent<PlayerDuplicateManager>().DuplicateControllers.Count; i++)
        {
            Destroy(hitPlayer.GetComponent<PlayerDuplicateManager>().DuplicateControllers[i].gameObject);
        }
        Destroy(hitPlayer.gameObject);
    }

    void DrawLaser()
    {
        
    }

    IEnumerator DestroyLineRenderer(LineRenderer lr, float time)
    {
        yield return new WaitForSeconds(time);
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
