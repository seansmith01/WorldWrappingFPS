using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerNetwork : NetworkBehaviour
{
    [SerializeField] PlayerMovement playerMovement;
    // Start is called before the first frame update
    void Start()
    {
        if (IsOwner)
        {
            playerMovement.enabled = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
