using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] int playerCount;
    [SerializeField] Transform spawnPointHolder;
    [SerializeField] GameObject playerPrefab;

    GameObject newPlayer1, newPlayer2;
    void Awake()
    {
            newPlayer1 = Instantiate(playerPrefab, spawnPointHolder.GetChild(0).position, spawnPointHolder.GetChild(0).rotation);
            newPlayer1.GetComponent<PlayerLocalManager>().PlayerNumber = 1;
        
        //if(playerCount == 2)
        //{
        //    newPlayer1 = Instantiate(playerPrefab1, spawnPointHolder.GetChild(0).position, spawnPointHolder.GetChild(0).rotation);

        //    newPlayer1.GetComponentInChildren<Camera>().rect = new Rect(GetCameraRect(0).x, GetCameraRect(0).y, 1, 1);

        //    newPlayer1.GetComponent<PlayerLocalManager>().PlayerNumber = 1;


        //    newPlayer2 = Instantiate(playerPrefab2, spawnPointHolder.GetChild(1).position, spawnPointHolder.GetChild(1).rotation);

        //    newPlayer2.GetComponentInChildren<Camera>().rect = new Rect(GetCameraRect(1).x, GetCameraRect(1).y, 1, 1);

        //    newPlayer2.GetComponent<PlayerLocalManager>().PlayerNumber = 2;
        //}
        
    }
    Vector2 GetCameraRect(int i)
    {
        if (i == 0)
            return new Vector2(-0.5f, 0);
        if (i == 1)
            return new Vector2(0.5f, 0);

        Debug.LogError("Error");
        return new Vector2(0.5f, 1);

    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(0);
        }
    }
    float player1Score, player2Score;
    [SerializeField] TextMeshProUGUI score1, score2;
    public void UpdateScore(int playerNumber)
    {
        if (playerNumber == 1)
        {
            player1Score += 0.03703703704f;
            score1.text = player1Score.ToString("F0");

            newPlayer2.transform.position = spawnPointHolder.GetChild(Random.Range(0,spawnPointHolder.childCount)).position;

        }
        if (playerNumber == 2)
        {
            player2Score += 0.03703703704f;
            score2.text = player2Score.ToString("F0");

            newPlayer1.transform.position = spawnPointHolder.GetChild(Random.Range(0,spawnPointHolder.childCount)).position;
        }
    }
}
