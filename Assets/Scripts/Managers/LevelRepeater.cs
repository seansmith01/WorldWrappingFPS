using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelRepeater : MonoBehaviour
{
    [SerializeField] bool frustrumCulling;
    GameObject level;
    public float repeatAmount;
    public float repeatSpacing;
    [SerializeField] float fogMultiplier;
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


        for (float x = -repeatSpacing * repeatAmount; x <= repeatAmount * repeatSpacing; x += repeatSpacing)
        {
            for (float y = -repeatSpacing * repeatAmount; y <= repeatAmount * repeatSpacing; y += repeatSpacing)
            {
                for (float z = -repeatSpacing * repeatAmount; z <= repeatAmount * repeatSpacing; z += repeatSpacing)
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
                            c.enabled = false;
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
    }
    [SerializeField] private float startDistanceMultiplier;
    [SerializeField] private float endDistanceMultiplier;
    private void Update()
    {
        RenderSettings.fogStartDistance = repeatSpacing * startDistanceMultiplier;
        RenderSettings.fogEndDistance = repeatSpacing * endDistanceMultiplier;
        

        //RenderSettings.fogDensity = repeatSpacing * (fogMultiplier * 0.01f);
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
