using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    // Start is called before the first frame update
    GameManager gameManager;
    Canvas canvas;
    TileState currWin;
    public TextMeshProUGUI gameOverText;
    void Start()
    {
        canvas = GetComponent<Canvas>();
        canvas.enabled = false;
        gameManager = FindAnyObjectByType<GameManager>();
        gameManager.gameOver.AddListener(showUI);
    }

    void showUI()
    {
        gameOverText.text = setWinnerTxt();
        canvas.enabled = true;
    }
    string setWinnerTxt()
    {
        currWin = gameManager.getWinner();
        if (currWin == TileState.Avail)
        {
            return "Draw";
        }
        else if(currWin == TileState.White)
        {
            return "White Win";
        }
        else if(currWin == TileState.Black)
        {
            return "Black Win";
        }
        else
        {
            return "GAME OVER";
        }

    }
    // Update is called once per frame
    void Update()
    {
        if (canvas.enabled == true)
        {
            checkInput();
        }
    }
    void checkInput()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            SceneManager.LoadScene("Game");
        }
    }
}
