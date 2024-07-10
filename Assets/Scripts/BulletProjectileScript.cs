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

        //Destroy(gameObject, timeTillDestory);
        float repeatSpacing = levelRepeater.RepeatSpacing.x; //temp
        //float repeatAmount = levelRepeater.repeatAmount - 1; // one less of the worl repeats
        float repeatAmount = levelRepeater.RepeatAmount; // one less of the worl repeats
        //spawn dups
        for (float x = -repeatSpacing * repeatAmount; x <= repeatAmount * repeatSpacing; x += repeatSpacing)
        {
            for (float y = -repeatSpacing * repeatAmount; y <= repeatAmount * repeatSpacing; y += repeatSpacing)
            {
                for (float z = -repeatSpacing * repeatAmount; z <= repeatAmount * repeatSpacing; z += repeatSpacing)
                {
                   // SpawnDupPrefab(new Vector3(x, y, z));
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
    [SerializeField] Vector3 lastPosition;
    [SerializeField] Vector3 fakeVelocity;
    private void Update()
    {
        timeSinceShot += Time.deltaTime;
        if (timeSinceShot > 0.25f)
        {
            firedForXTime = true;
        }
        transform.position += transform.forward * Time.deltaTime * speed;
        WrapCheck();


        var currentPosition = transform.position;
        if (currentPosition != lastPosition)
        {
            fakeVelocity = (transform.position - lastPosition) / Time.deltaTime;
            //Do something
        }

        lastPosition = currentPosition;

    }
    void WrapCheck()
    {
        float boundsMaxX = levelRepeater.RepeatSpacing.x / 2f;
        float boundsMaxY = levelRepeater.RepeatSpacing.y / 2f;
        float boundsMaxZ = levelRepeater.RepeatSpacing.z / 2f;
        if (transform.position.x > boundsMaxX)
        {
            WrapTo(new Vector3(-boundsMaxX, transform.position.y, transform.position.z));
        }
        if (transform.position.x < -boundsMaxX)
        {
            WrapTo(new Vector3(boundsMaxX, transform.position.y, transform.position.z));
        }

        if (transform.position.y > boundsMaxY)
        {
            WrapTo(new Vector3(transform.position.x, -boundsMaxY, transform.position.z));
        }
        if (transform.position.y < -boundsMaxY)
        {
            WrapTo(new Vector3(transform.position.x, boundsMaxY, transform.position.z));
        }

        if (transform.position.z > boundsMaxZ)
        {
            WrapTo(new Vector3(transform.position.x, transform.position.y, -boundsMaxZ));
        }
        if (transform.position.z < -boundsMaxZ)
        {
            WrapTo(new Vector3(transform.position.x, transform.position.y, boundsMaxZ));
        }
    }
    void WrapTo(Vector3 newPos)
    {

        transform.position = newPos;
        return;
        foreach (ParticleSystem t in GetComponentsInChildren<ParticleSystem>()) //actually seems to work?!!!
        {
            var l = t.main;
            l.simulationSpace = ParticleSystemSimulationSpace.Local;
        }
        foreach (ParticleSystem t in GetComponentsInChildren<ParticleSystem>())
        {
            var l = t.main;
            l.simulationSpace = ParticleSystemSimulationSpace.World;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //if (firedForXTime)
        //{
        //    if (transform.parent == null)
        //    {
        //        Destroy(gameObject);
        //
        //    }
        //    Destroy(transform.root.gameObject);
        //}
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            //rb.GetComponent<PlayerMovement>().SetRelativeVelocity(fakeVelocity);
            //print(rb.gameObject);
            rb.velocity = fakeVelocity;
            //rb.velocity = fakeVelocity;
        }
    }
    private void OnCollisionStay(Collision collision)
    {
        if(collision.collider.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            //rb.GetComponent<PlayerMovement>().SetRelativeVelocity(fakeVelocity);
            print(rb.gameObject);
            rb.AddForce(fakeVelocity /4f, ForceMode.VelocityChange);
            //rb.velocity = fakeVelocity;
        }
        //if (firedForXTime)
        //{
        //    if(transform.parent == null)
        //    {
        //        Destroy(gameObject);
        //
        //    }
        //    Destroy(transform.root.gameObject);
        //
        //}

    }
}
