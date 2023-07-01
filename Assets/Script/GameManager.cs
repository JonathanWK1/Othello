using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public TileState currTurn;

    public UnityEvent turnChanged;
    public UnityEvent gameOver;
    public Board board;

    int validateCount;
    private void Start()
    {

    }
    public void changeTurn()
    {
        currTurn = TileStateMethod.GetOppositeColor(currTurn);
        turnChanged.Invoke();
    }
    public void validateTurn()
    {
        if (board.emptyCount <= 0)
        {
            gameOver.Invoke();
            return;
        }
        else if (board.availCount <= 0)
        {
            if (validateCount == 0)
            {
                validateCount++;
                changeTurn();
            }
            else
            {
                gameOver.Invoke(); 
            }
        }
        else
        {
            validateCount = 0;
        }
    }

    public TileState getWinner()
    {
        if (board.whiteCount > board.blackCount)
        {
            return TileState.White;
        }
        else if (board.whiteCount < board.blackCount)
        {
            return TileState.Black;
        }
        else
        {
           return TileState.Avail;
        }
    }
}
