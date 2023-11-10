using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [SerializeField] Transform cameraHolder;
    [SerializeField] Camera playerCamera;
    //public Camera weaponCamera;
    [SerializeField] float sensX = 1f;
    [SerializeField] float sensY = 1f;
    public Vector2 currentLook;

    PlayerInput playerInput;
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

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

        float x = lookInput.x * sensX * Time.deltaTime;
        transform.Rotate(new Vector3(0, x, 0), Space.Self);
    }
    float rotX, rotY;
    Vector2 lookInput;
    void RotateMainCamera()
    {
        lookInput = playerInput.actions["Look"].ReadValue<Vector2>();
        

        //rotX += lookInput.x * sensX * Time.deltaTime;
        //transform.rotation = Quaternion.Euler(transform.rotation.x, rotX, transform.rotation.z);


        rotY += lookInput.y * sensY * Time.deltaTime;
        rotY = Mathf.Clamp(rotY, -90f, 90f);

        cameraHolder.transform.localRotation = Quaternion.Euler(-rotY, 0f, 0f);

    }

}