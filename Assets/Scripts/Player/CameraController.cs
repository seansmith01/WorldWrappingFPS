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
        playerNumber = GetComponent<PlayerLocalManager>().PlayerID;

        playerCamera.backgroundColor = RenderSettings.fogColor;


    }
    private void Update()
    {
        if(playerInput.actions["LowerSense"].WasPressedThisFrame()){
            sensX = 10;
            sensY = 10;
        }

        RotateMainCamera();
    }
    void FixedUpdate()
    {

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
    float rotY;
    Vector2 lookInput;
    void RotateMainCamera()
    {
        lookInput = playerInput.actions["Look"].ReadValue<Vector2>();
        lookInput.x *= sensX * Time.deltaTime;
        transform.Rotate(new Vector3(0, lookInput.x, 0), Space.Self);

        rotY += lookInput.y * sensY * Time.deltaTime;
        rotY = Mathf.Clamp(rotY, -90f, 90f);

        cameraHolder.transform.localRotation = Quaternion.Euler(-rotY, 0f, 0f);

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