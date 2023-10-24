using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLocalManager : MonoBehaviour
{
    GameManager gameManager;
    public int PlayerNumber;

    [SerializeField] MeshRenderer bodyMesh, headMesh;
    [SerializeField] Material redMat, blueMat, greenMat, yellowMat;
    private void Awake()
    {
        gameManager = FindFirstObjectByType<GameManager>();

        gameManager.CurrentPlayerCount++;

        switch(gameManager.CurrentPlayerCount)
        {
            case 0: Debug.LogError("No Players"); break;
            case 1: ChangeMaterial(redMat); break;
            case 2: ChangeMaterial(blueMat); break;
            case 3: ChangeMaterial(greenMat); break;
            case 4: ChangeMaterial(yellowMat); break;
        }
    }
    private void ChangeMaterial(Material newMat)
    {
        bodyMesh.material = newMat;
        headMesh.material = newMat;
    }
}
