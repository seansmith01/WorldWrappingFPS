using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public int CurrentPlayerCount;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            foreach(PlayerShooting ps in FindObjectsByType<PlayerShooting>(FindObjectsSortMode.None))
            {
                ps.gameObject.SetActive(true);
            }
        }
    }
}
