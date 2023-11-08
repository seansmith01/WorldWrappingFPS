using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLocalManager : MonoBehaviour
{
    GameManager gameManager;
    public int PlayerID;

    [Header("Player Meshes")]
    [SerializeField] MeshRenderer bodyMesh;
    [SerializeField] MeshRenderer headMesh;
    [Header("Player Materials")]
    [SerializeField] Material redMat;
    [SerializeField] Material blueMat;
    [SerializeField] Material greenMat;
    [SerializeField] Material yellowMat;
    [Header("Disable if not first player")]
    [SerializeField] AudioListener audioListener;
    [SerializeField] PlayerAudioHandler playerAudioHandler;
    [Header("Toggle Off When Dead")]
    [SerializeField] MeshRenderer[] meshes;
    [SerializeField] Collider[] colliders;
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] CameraController cameraController;
    [SerializeField] Rigidbody rb;
    private void Awake()
    {
        if (FindFirstObjectByType(typeof(GameManager)))
        {
            gameManager = FindFirstObjectByType<GameManager>();

            gameManager.CurrentPlayerCount++;

            PlayerID = gameManager.CurrentPlayerCount;
        }
        else // for online for now
        {
            PlayerID = 1;

        }


        ChangeMaterial();
        ChangeLayer();
    }

    private void ChangeMaterial()
    {
        Material newMat = null;
        switch (PlayerID)
        {
            case 0: Debug.LogError("No Players"); break;
            case 1: newMat = redMat; playerAudioHandler.IsFirstPlayerLocal = true; break;
            case 2: newMat = blueMat; audioListener.enabled = false; playerAudioHandler.IsFirstPlayerLocal = false; break;
            case 3: newMat = greenMat; audioListener.enabled = false; playerAudioHandler.IsFirstPlayerLocal = false; break;
            case 4: newMat = yellowMat; audioListener.enabled = false; playerAudioHandler.IsFirstPlayerLocal = false; break;
        }
        bodyMesh.material = newMat;
        headMesh.material = newMat;
    }

    private void ChangeLayer()
    {
        gameObject.layer = LayerMask.NameToLayer("Player" + PlayerID);
    }
    
    public void Die()
    {
        ToggleStuffOnDie(false);
        StartCoroutine(Respawn());
    }
    void ToggleStuffOnDie(bool b)
    {
        for (int i = 0; i < meshes.Length; i++)
        {
            meshes[i].enabled = b;
        }
        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].enabled = b;
        }
        playerMovement.enabled = b;
        cameraController.enabled = b;
        rb.isKinematic = !b;

    }
    IEnumerator Respawn()
    {
        transform.position = new Vector3(0,10,0);
        yield return new WaitForSeconds(2);
        transform.position = new Vector3(0,10,0);
        ToggleStuffOnDie(true);

    }
}
