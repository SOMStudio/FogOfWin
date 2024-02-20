using UnityEngine;

public class GameManager : MonoSingletone
{
    [Header("Data")]
    [SerializeField] private GameLogic gameLogic;

    public GameLogic GameLogic => gameLogic;
}
