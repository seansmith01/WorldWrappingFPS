using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrustrumCulling : MonoBehaviour
{
    Camera cam;
    Bounds boundingBox;
    Plane[] planes;

    Bounds[] corners = new Bounds[8];
    private void Start()
    {
        cam = Camera.main;
        foreach (Renderer renderer in GetComponentsInChildren<Renderer>())
        {
            boundingBox.Encapsulate(renderer.bounds.min);
            boundingBox.Encapsulate(renderer.bounds.max);
        }
        Vector3 size = boundingBox.size;
        corners[0] = new Bounds(boundingBox.min, Vector3.zero);
        corners[1] = new Bounds(boundingBox.min + new Vector3(size.x, 0, 0), Vector3.zero);
        corners[2] = new Bounds(boundingBox.min + new Vector3(size.x, size.y, 0), Vector3.zero);
        corners[3] = new Bounds(boundingBox.min + new Vector3(size.x, 0, size.z), Vector3.zero);
        corners[4] = new Bounds(boundingBox.min + new Vector3(0, size.y, 0), Vector3.zero);
        corners[5] = new Bounds(boundingBox.min + new Vector3(0, size.y, size.z), Vector3.zero);
        corners[6] = new Bounds(boundingBox.min + new Vector3(0, 0, size.z), Vector3.zero);
        corners[7] = new Bounds(boundingBox.min + new Vector3(size.x, size.y, size.z), Vector3.zero);
    }
    void Update()
    {
        bool shouldDisable = true;
        planes = GeometryUtility.CalculateFrustumPlanes(cam);

        for (int i = 0; i < 8; i++)
        {
            if (GeometryUtility.TestPlanesAABB(planes, corners[i]))
            {
                shouldDisable = false;
                break;
            }
        }
        transform.GetChild(0).gameObject.SetActive(!shouldDisable);

    }
}
