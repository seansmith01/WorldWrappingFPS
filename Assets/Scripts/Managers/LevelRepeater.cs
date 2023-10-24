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
    public float RepeatSpacing;
    [SerializeField] private float startDistanceMultiplier;
    [SerializeField] private float endDistanceMultiplier;
    // Start is called before the first frame update
    void Awake()
    {
        GameObject holder = new GameObject("LevelClonesHolder");
        //Color col = FindObjectOfType<Camera>().GetComponent<Camera>().backgroundColor; 
        //RenderSettings.fogColor = col;
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).gameObject.activeSelf)
            {
                level = transform.GetChild(i).gameObject;
            }
        }


        for (float x = -RepeatSpacing * RepeatAmount; x <= RepeatAmount * RepeatSpacing; x += RepeatSpacing)
        {
            for (float y = -RepeatSpacing * RepeatAmount; y <= RepeatAmount * RepeatSpacing; y += RepeatSpacing)
            {
                for (float z = -RepeatSpacing * RepeatAmount; z <= RepeatAmount * RepeatSpacing; z += RepeatSpacing)
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

        RenderSettings.fogStartDistance = RepeatSpacing * startDistanceMultiplier;
        RenderSettings.fogEndDistance = RepeatSpacing * endDistanceMultiplier;
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
}
