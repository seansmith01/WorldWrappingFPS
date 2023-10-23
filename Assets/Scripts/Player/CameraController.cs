using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [SerializeField] Transform cameraHolder;
    [SerializeField] Camera playerCamera;
    PlayerMovement playerMovement;
    Camera MyCamera;
    //public Camera weaponCamera;
    [SerializeField] float sensX = 1f;
    [SerializeField] float sensY = 1f;
    float baseFov = 120f;
    float maxFov = 120f;
    float wallRunTilt = 15f;

    float wishTilt = 0;
    float curTilt = 0;
    public Vector2 currentLook;
    Vector2 sway = Vector3.zero;
    float fov;

    int playerNumber;

    PlayerInput playerInput;
    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerInput = GetComponent<PlayerInput>();
        curTilt = transform.localEulerAngles.z;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        playerNumber = GetComponent<PlayerLocalManager>().PlayerNumber;

        playerCamera.backgroundColor = RenderSettings.fogColor;


    }
    private void Update()
    {
        if(playerInput.actions["LowerSense"].WasPressedThisFrame()){
            sensX = 7;
            sensY = 7;
        }

    }
    void FixedUpdate()
    {
        RotateMainCamera();

        if (Input.GetKeyDown(KeyCode.N))
        {
            if(Cursor.lockState== CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
            Cursor.visible = !Cursor.visible;
        }
    }

    void RotateMainCamera()
    {
        

        Vector2 lookInput = playerInput.actions["Look"].ReadValue<Vector2>();
        lookInput.x *= sensX * Time.deltaTime;
        lookInput.y *= sensY * Time.deltaTime;

        transform.Rotate(new Vector3(0, lookInput.x, 0), Space.Self);


        cameraHolder.Rotate(new Vector3(-lookInput.y, 0, 0), Space.Self);
    }

    public void Punch(Vector2 dir)
    {
        sway += dir;
    }

    #region Setters
    public void SetTilt(float newVal)
    {
        wishTilt = newVal;
    }

    public void SetXSens(float newVal)
    {
        sensX = newVal;
    }

    public void SetYSens(float newVal)
    {
        sensY = newVal;
    }

    public void SetFov(float newVal)
    {
        baseFov = newVal;
    }
    #endregion
}