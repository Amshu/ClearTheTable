using System.Collections;
using System.Collections.Generic;

public enum GameState
{
    P1, 
    P2,
    Face,
    Deck,
    P1Pile,
    P2Pile,
    GameOver
}

public enum Combinations
{
    Blank,
    NotEnoughSelected,
    TooManySelected,
    Wrong,
    Placed,
    Pair,
    PairD, // Pair + Direct Hit
    PairC, // Pair + Clear The Table
    PairDC, // Pair + Direct Hit + Cleared The Table
    Add,
    AddD, // Additive + Direct Hit
    AddC, // Additive + Clear The Table
    AddDC // Additive + Direct Hit + Cleared The Table
}
