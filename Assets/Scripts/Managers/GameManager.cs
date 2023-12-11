using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public int CurrentPlayerCount;
    [SerializeField] AudioSource music;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            foreach(PlayerShooting ps in FindObjectsByType<PlayerShooting>(FindObjectsSortMode.None))
            {
                ps.gameObject.SetActive(true);
            }
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            music.enabled = !music.enabled;
        }
    }
}
