using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioReverbTrigger : MonoBehaviour
{
    [SerializeField] AudioReverbZone largeReverbZone;
    [SerializeField] AudioReverbZone smallReverrbZone;
    void Start()
    {
        largeReverbZone.enabled = true;
        smallReverrbZone.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponentInChildren<AudioListener>() != null)
        {
            InsideTrigger(true);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponentInChildren<AudioListener>() != null)
        {
            InsideTrigger(false);
        }
    }
    void InsideTrigger(bool isInside)
    {
        smallReverrbZone.enabled = isInside;
        largeReverbZone.enabled = !isInside;
    }
}
