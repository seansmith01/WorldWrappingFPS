using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;


public class GameManager : MonoBehaviour
{
    private List<PlayerInput> players = new List<PlayerInput>();
    PlayerInputManager playerInputManager;
    
    void Awake()
    {
        playerInputManager = FindFirstObjectByType<PlayerInputManager>();
        //playerInputManager.onPlayerJoined += AddPlayer;
        for (int i = 0; i < 2; i++)
        {
            playerInputManager.JoinPlayer(i, i);

        }
    }
    void AddPlayer(PlayerInput player)
    {
        players.Add(player);

        player.GetComponent<PlayerLocalManager>().PlayerNumber = players.Count - 1;

    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(0);
        }
    }
}
