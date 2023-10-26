using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLocalManager : MonoBehaviour
{
    GameManager gameManager;
    public int PlayerID;

    [SerializeField] MeshRenderer bodyMesh, headMesh;
    [SerializeField] Material redMat, blueMat, greenMat, yellowMat;
    private void Awake()
    {
        gameManager = FindFirstObjectByType<GameManager>();

        gameManager.CurrentPlayerCount++;

        PlayerID = gameManager.CurrentPlayerCount;

        ChangeMaterial();
        ChangeLayer();
    }

    private void ChangeMaterial()
    {
        Material newMat = null;
        switch (PlayerID)
        {
            case 0: Debug.LogError("No Players"); break;
            case 1: newMat = redMat; break;
            case 2: newMat = blueMat; break;
            case 3: newMat = greenMat; break;
            case 4: newMat = yellowMat; break;
        }
        bodyMesh.material = newMat;
        headMesh.material = newMat;
    }

    private void ChangeLayer()
    {
        gameObject.layer = LayerMask.NameToLayer("Player" + PlayerID);
    }
    [SerializeField] MeshRenderer[] meshes;
    [SerializeField] Collider[] colliders;
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] CameraController cameraController;
    [SerializeField] Rigidbody rigidbody;
    public void Die()
    {
        ToggleStuff(false);
        StartCoroutine(Respawn());
    }
    void ToggleStuff(bool b)
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
        rigidbody.isKinematic = !b;

    }
    IEnumerator Respawn()
    {
        transform.position = new Vector3(0,10,0);
        yield return new WaitForSeconds(2);
        transform.position = new Vector3(0,10,0);
        ToggleStuff(true);

    }
}
