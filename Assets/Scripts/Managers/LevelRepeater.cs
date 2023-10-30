using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelRepeater : MonoBehaviour
{
    [SerializeField] bool doGPUInstancing;
    [SerializeField] bool frustrumCulling;
    GameObject level;
    public float RepeatAmount;
    public Vector3 RepeatSpacing;
    [SerializeField] private Transform boundsTriggers;
    [SerializeField] private float startDistanceMultiplier;
    [SerializeField] private float endDistanceMultiplier;
    // Start is called before the first frame update
    void Awake()
    {
        GameObject holder = new GameObject("LevelClonesHolder");
        //Color col = FindObjectOfType<Camera>().GetComponent<Camera>().backgroundColor; 
        //RenderSettings.fogColor = col;
        SetUpRaycastBounds();
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).gameObject.activeSelf)
            {
                level = transform.GetChild(i).gameObject;
            }
        }



        for (float x = -RepeatSpacing.x * RepeatAmount; x <= RepeatAmount * RepeatSpacing.x; x += RepeatSpacing.x)
        {
            for (float y = -RepeatSpacing.y * RepeatAmount; y <= RepeatAmount * RepeatSpacing.y; y += RepeatSpacing.y)
            {
                for (float z = -RepeatSpacing.z * RepeatAmount; z <= RepeatAmount * RepeatSpacing.z; z += RepeatSpacing.z)
                {
                    GameObject levelClone = Instantiate(level, new Vector3(x, y, z), Quaternion.identity);

                    if(x==0 && y==0 && z == 0)
                    {
                        //print("middle level")
                    }
                    else //disable colliders
                    {
                        foreach (Collider c in levelClone.GetComponentsInChildren<Collider>())
                        {
                            //c.enabled = false;
                        }
                    }
                    if (doGPUInstancing)
                    {
                        foreach (Renderer rend in levelClone.GetComponentsInChildren<Renderer>())
                        {
                            rend.sharedMaterial.enableInstancing = true;
                        }
                    }
                    
                    levelClone.name = "levelClone";
                    levelClone.transform.parent = holder.transform;
                    //if (frustrumCulling)
                    //{
                    //GameObject parent = new GameObject();
                    //parent.name = "parent";
                    //levelClone.transform.parent = parent.transform;
                    //parent.AddComponent<FrustrumCulling>();
                    //}
                    // dont delet colliders, they needed for grapple
                }
            }
        }
        level.SetActive(false);
        // transform.eulerAngles = new Vector3(0, 0, 45);
        float maxRepeatSpacing = GetMaxRepeatSpacing(RepeatSpacing.x, RepeatSpacing.y, RepeatSpacing.z);
        RenderSettings.fogStartDistance = maxRepeatSpacing * startDistanceMultiplier;
        RenderSettings.fogEndDistance = maxRepeatSpacing * endDistanceMultiplier;
    }

    private void Update()
    {
        //RenderSettings.fogStartDistance = RepeatSpacing * startDistanceMultiplier;
        //RenderSettings.fogEndDistance = RepeatSpacing * endDistanceMultiplier;


        //RenderSettings.fogDensity = repeatSpacing * (fogMultiplier * 0.01f);
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
    float GetMaxRepeatSpacing(float a, float b, float c)
    {
        // Compare a to b, and then compare the result to c to find the maximum value.
        float max = Mathf.Min(Mathf.Min(a, b), c);
        return max;
    }
    [SerializeField] Transform minusBoundsX;
    [SerializeField] Transform positiveBoundsX;
    [SerializeField] Transform minusBoundsY;
    [SerializeField] Transform positiveBoundsY;
    [SerializeField] Transform minusBoundsZ;
    [SerializeField] Transform positiveBoundsZ;
    void SetUpRaycastBounds()
    {
        float posX = (RepeatSpacing.x / 2) + 1;
        float posY = (RepeatSpacing.y / 2) + 1;
        float posZ = (RepeatSpacing.z / 2) + 1;
        minusBoundsX.position = new Vector3(-posX, 0, 0);
        positiveBoundsX.position = new Vector3(posX, 0, 0);
        minusBoundsY.position = new Vector3(0, -posY, 0);
        positiveBoundsY.position = new Vector3(0, posY, 0);
        minusBoundsZ.position = new Vector3(0, 0, -posZ);
        positiveBoundsZ.position = new Vector3(0, 0, posZ);
    }
}
