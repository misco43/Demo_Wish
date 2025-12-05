using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum E_GameState
{
    BeforeGameStart = 0,
    PlayFrontAnimtion = 1,
    GameNormal = 2,
    PlayOtherAnimation = 3,
    PlayEndAnimation = 4,
    GameEnd = 5,
}

public class GameMgr : SingletoMono<GameMgr>
{
    public BeforeGameStartState beforeGameStartState;
    public PlayFrontAnimationState playFrontAnimationState;
    public GameNormalState gameNormalState;
    public PlayOtherAnimationState playOtherAnimationState;
    public PlayEndAnimationState playEndAnimationState;
    public GameEndState gameEndState;

    private Dictionary<E_GameState, IState> _gameStateDic = new Dictionary<E_GameState, IState>();
    private IState _curState;
    public E_GameState _gameState;

    // Start is called before the first frame update
    void Start()
    {
        //registe all the state in here
        _gameStateDic.Add(E_GameState.BeforeGameStart, beforeGameStartState);
        _gameStateDic.Add(E_GameState.PlayFrontAnimtion, playFrontAnimationState);
        _gameStateDic.Add(E_GameState.GameNormal, gameNormalState);
        _gameStateDic.Add(E_GameState.PlayOtherAnimation, playOtherAnimationState);
        _gameStateDic.Add(E_GameState.PlayEndAnimation, playEndAnimationState);
        _gameStateDic.Add(E_GameState.GameEnd, gameEndState);

        //start the begin state
        ChangeState(_gameState);
    }

    // Update is called once per frame
    void Update()
    {
        if(!ReferenceEquals(_curState, null))
        {
            _curState.OnUpdate();
        }

    }

    public void ChangeState(E_GameState gameState)
    {
        if (!_gameStateDic.ContainsKey(gameState))
        {
            Debug.LogError($"this state : {gameState.ToString()} has not been registered in this fsm");
            return;
        }
        
        this._gameState = gameState;
        if(_curState != null)
        {
            _curState.OnExit();
        }
        _curState = _gameStateDic[gameState];
    }
}
