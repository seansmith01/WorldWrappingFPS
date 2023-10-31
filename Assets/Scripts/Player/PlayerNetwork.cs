using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;

public class PlayerNetwork : NetworkBehaviour
{
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] CameraController cameraController;
    [SerializeField] GameObject camHolder;
    [SerializeField] Transform headTransform;
    [SerializeField] PlayerInput playerInput;

    private readonly NetworkVariable<Vector3> netPos = new NetworkVariable<Vector3>(writePerm:NetworkVariableWritePermission.Owner);
    private readonly NetworkVariable<Quaternion> netRot = new NetworkVariable<Quaternion>(writePerm:NetworkVariableWritePermission.Owner);
    private readonly NetworkVariable<Quaternion> headRot = new NetworkVariable<Quaternion>(writePerm:NetworkVariableWritePermission.Owner);
    // Start is called before the first frame update
    void Start()
    {
        if (IsOwner)
        {
            playerInput.enabled = true;
            playerMovement.enabled = true;
            cameraController.enabled = true;
            camHolder.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (IsOwner)
        {
            netPos.Value = transform.position;
            netRot.Value = transform.rotation;
            headRot.Value = headTransform.rotation;
        }
        else
        {
            transform.position = netPos.Value;
            transform.rotation = netRot.Value;
            headTransform.rotation = headRot.Value;
        }
    }
}
