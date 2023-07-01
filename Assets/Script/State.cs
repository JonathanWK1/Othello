using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileState
{
    UnAvail,
    Avail,
    White,
    Black
};

public static class TileStateMethod
{
    public static TileState GetOppositeColor(TileState state)
    {
        if (state == TileState.White)
        {
            return TileState.Black;
        }
        else if (state == TileState.Black)
        {
            return TileState.White;
        }
        return TileState.UnAvail;
    }

}