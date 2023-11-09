
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class PlayerDuplicateManager : MonoBehaviour
{
    [SerializeField] 
    private GameObject duplicatePrefab;
    [SerializeField] 
    private Transform cameraHolder;

    public  List<DuplicateController> DuplicateControllers = new List<DuplicateController>();
    private List<Vector3> offsets = new List<Vector3>();
    private GameObject duplicatesHolder;
    private PlayerLocalManager playerLocalManager;
    public void Start()
    {
        playerLocalManager = GetComponent<PlayerLocalManager>();
        duplicatesHolder = Instantiate(new GameObject("DuplicatesHolder"));

        LevelRepeater levelRepeater = FindFirstObjectByType<LevelRepeater>();
        float repeatSpacingX = levelRepeater.RepeatSpacing.x;
        float repeatSpacingY = levelRepeater.RepeatSpacing.y;
        float repeatSpacingZ = levelRepeater.RepeatSpacing.z;
        //float repeatAmount = levelRepeater.RepeatAmount - 1; // one less of the worl repeats
        float repeatAmount = 1; // one less of the worl repeats
        //spawn dups
        for (float x = -repeatSpacingX * repeatAmount; x <= repeatAmount * repeatSpacingX; x += repeatSpacingX)
        {
            for (float y = -repeatSpacingY * repeatAmount; y <= repeatAmount * repeatSpacingY; y += repeatSpacingY)
            {
                for (float z = -repeatSpacingZ * repeatAmount; z <= repeatAmount * repeatSpacingZ; z += repeatSpacingZ)
                {
                    //bool firstItterationDuplicate = false;
                    //if ((x == repeatSpacing || x == -repeatSpacing) && (y == repeatSpacing || y == -repeatSpacing) && (z == repeatSpacing || z == -repeatSpacing))
                    //{
                    //    firstItterationDuplicate = true;
                    //}
                    SpawnDupPrefab(new Vector3(x, y, z));                     
                }
            }
        }

    }
    
    private void Update()
    {
        for (int j = 0; j < DuplicateControllers.Count; j++)
        {
            if (DuplicateControllers[j] != null)
            {
                DuplicateControllers[j].transform.position = transform.position + offsets[j];
                DuplicateControllers[j].transform.rotation = transform.rotation;
                DuplicateControllers[j].CameraHolder.rotation = cameraHolder.rotation;
            }
        }
        
    }
    void SpawnDupPrefab(Vector3 dupOffset)
    {
        // If duplicate is in centre (where player is)
        if (dupOffset == Vector3.zero)
            return;
        GameObject meshDup = Instantiate(duplicatePrefab, transform.position + dupOffset, transform.rotation);
        meshDup.layer = LayerMask.NameToLayer("Player" + GetComponent<PlayerLocalManager>().PlayerID);
        DuplicateController duplicateController = meshDup.GetComponent<DuplicateController>();
        duplicateController.PlayerNumber = playerLocalManager.PlayerID;
        meshDup.transform.parent = duplicatesHolder.transform;
        duplicateController.SetupColour();

        DuplicateControllers.Add(duplicateController);

        offsets.Add(dupOffset);

    }
    public void WrapTo(Vector3 newPos)
    {
        for (int i = 0; i < DuplicateControllers.Count; i++)
        {
            DuplicateControllers[i].transform.position = newPos;
        }
    }

}
