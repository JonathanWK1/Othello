using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    // Start is called before the first frame update
    public TextMeshProUGUI whiteCount;
    public TextMeshProUGUI blackCount;

    GameManager gameManager;
    void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        gameManager.board.countChanged.AddListener(setCount);

    }

    void setCount()
    {
        whiteCount.text = gameManager.board.whiteCount.ToString();
        blackCount.text = gameManager.board.blackCount.ToString();
    }

    
}
