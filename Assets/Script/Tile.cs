using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

public class Tile : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] private Sprite[] stateSpriteList;
    public TileState currState { get; private set; }

    public UnityEvent<Board.PositionOnBoard> onClick;
    public UnityEvent<GameObject> stateChanged;

    public SpriteRenderer stateSpriteRenderer;
    public Board.PositionOnBoard pos;
    void Start()
    {
        //currState = TileState.UnAvail;
        setState(currState);
    }

    public void setState(TileState state)
    {
        currState = state;
        stateSpriteRenderer.sprite = stateSpriteList[((int)currState)];
        stateChanged.Invoke(gameObject);
    }

    public void flipTile()
    {
        currState = TileStateMethod.GetOppositeColor(currState);
        stateSpriteRenderer.sprite = stateSpriteList[((int)currState)];

    }

    private void OnMouseDown()
    {
        if (currState != TileState.Avail)
        {
            return;
        }        
        onClick.Invoke(pos);
    }
}
