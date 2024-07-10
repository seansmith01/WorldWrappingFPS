using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DuplicateController : MonoBehaviour
{
    public int PlayerNumber;
    public bool IsFirstItterationDuplicate;
    public Transform CameraHolder;
    public Transform GunTip;
    public OneShotAudioHolder OneShotAudioHolder;
    public AudioSource FallingAudioSource;

    [Header("Player Materials")]
    [SerializeField] Material redMat;
    [SerializeField] Material blueMat;
    [SerializeField] Material greenMat;
    [SerializeField] Material yellowMat;
    [Header("Player Meshes")]
    [SerializeField] MeshRenderer bodyMesh;
    [SerializeField] MeshRenderer headMesh;
    public void SetupColour()
    {
        Material newMat = null;
        switch (PlayerNumber)
        {
            case 0: Debug.LogError("No Players"); break;
            case 1: newMat = redMat; ; break;
            case 2: newMat = blueMat;  break;
            case 3: newMat = greenMat;  break;
            case 4: newMat = yellowMat; break;
        }
        bodyMesh.material = newMat;
        headMesh.material = newMat;
    }
}
