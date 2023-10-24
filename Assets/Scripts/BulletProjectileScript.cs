using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletProjectileScript : MonoBehaviour
{
    LevelRepeater levelRepeater;
    [SerializeField] GameObject meshToCopy;
    [SerializeField] float speed;
    [SerializeField] float timeTillDestory;
    bool firedForXTime;
    void Start()
    {
        levelRepeater = FindFirstObjectByType<LevelRepeater>();

        
        Destroy(gameObject, timeTillDestory);
        float repeatSpacing = levelRepeater.RepeatSpacing;
        //float repeatAmount = levelRepeater.repeatAmount - 1; // one less of the worl repeats
        float repeatAmount = levelRepeater.RepeatAmount; // one less of the worl repeats
        //spawn dups
        for (float x = -repeatSpacing * repeatAmount; x <= repeatAmount * repeatSpacing; x += repeatSpacing)
        {
            for (float y = -repeatSpacing * repeatAmount; y <= repeatAmount * repeatSpacing; y += repeatSpacing)
            {
                for (float z = -repeatSpacing * repeatAmount; z <= repeatAmount * repeatSpacing; z += repeatSpacing)
                {
                    SpawnDupPrefab(new Vector3(x, y, z));
                }
            }
        }
    }
    void SpawnDupPrefab(Vector3 dupOffset)
    {
        GameObject meshDup = Instantiate(meshToCopy, transform.position + dupOffset, transform.rotation);
        meshDup.transform.parent = transform;
    }
    float timeSinceShot;
    private void Update()
    {
        timeSinceShot += Time.deltaTime;
        if (timeSinceShot > 0.25f)
        {
            firedForXTime = true;
        }
        transform.position += transform.forward * Time.deltaTime * speed;
        WrapCheck();
    }
    void WrapCheck()
    {
        float boundsMax = levelRepeater.RepeatSpacing / 2f;
        if (transform.position.x > boundsMax)
        {
            WrapTo(new Vector3(-boundsMax, transform.position.y, transform.position.z));
        }
        if (transform.position.x < -boundsMax)
        {
            WrapTo(new Vector3(boundsMax, transform.position.y, transform.position.z));
        }

        if (transform.position.z > boundsMax)
        {
            WrapTo(new Vector3(transform.position.x, transform.position.y, -boundsMax));
        }
        if (transform.position.z < -boundsMax)
        {
            WrapTo(new Vector3(transform.position.x, transform.position.y, boundsMax));
        }

        if (transform.position.y > boundsMax)
        {
            WrapTo(new Vector3(transform.position.x, -boundsMax, transform.position.z));
        }
        if (transform.position.y < -boundsMax)
        {
            WrapTo(new Vector3(transform.position.x, boundsMax, transform.position.z));
        }

    }
    void WrapTo(Vector3 newPos)
    {

        foreach (ParticleSystem t in GetComponentsInChildren<ParticleSystem>()) //actually seems to work?!!!
        {
            var l = t.main;
            l.simulationSpace = ParticleSystemSimulationSpace.Local;
        }
        transform.position = newPos;
        foreach (ParticleSystem t in GetComponentsInChildren<ParticleSystem>())
        {
            var l = t.main;
            l.simulationSpace = ParticleSystemSimulationSpace.World;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (firedForXTime)
        {
            if (transform.parent == null)
            {
                Destroy(gameObject);

            }
            Destroy(transform.root.gameObject);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (firedForXTime)
        {
            if(transform.parent == null)
            {
                Destroy(gameObject);

            }
            Destroy(transform.root.gameObject);

        }

    }
}
