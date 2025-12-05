using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;

public class Snake
{
    public enum E_MoveDir
    {
        left, up, right, down
    }

    public List<Vector2Int> snakeBodyList;
    
    private E_MoveDir _moveDir = E_MoveDir.right;
    private E_MoveDir _MoveDirBuffer = E_MoveDir.right;
    private Map _map;

    public Snake(Map map)
    {
        _map = map;

        snakeBodyList = new List<Vector2Int>(12);
        snakeBodyList.Add(new Vector2Int(2,4));
        snakeBodyList.Add(new Vector2Int(1,4));

        _map.UpdateSnakeHeadPos(snakeBodyList[0]);
        for(int i = 1; i < snakeBodyList.Count; i++)
        {
            _map.UpdateSnakeBodyPos(snakeBodyList[i]);
        }
    }

    public void TryChangeMoveDir(E_MoveDir moveDir)
    {
         if(moveDir == E_MoveDir.left && _moveDir != E_MoveDir.right)
        {
            _MoveDirBuffer = moveDir;
        }
        else if(moveDir == E_MoveDir.up && _moveDir != E_MoveDir.down)
        {
            _MoveDirBuffer = moveDir;
        }
        else if(moveDir == E_MoveDir.right && _moveDir != E_MoveDir.left)
        {
            _MoveDirBuffer = moveDir;
        }
        else if(moveDir == E_MoveDir.down && _moveDir != E_MoveDir.up)
        {
            _MoveDirBuffer = moveDir;
        }
        else
        {
            //TODO: delet this print info after complete this;
            Debug.Log($"cur move dir is 【{_MoveDirBuffer}】, can't change to the 【{moveDir}】");
        }
    }

    public void ChangeMoveDir(E_MoveDir moveDir)
    {
        if(moveDir == E_MoveDir.left && _moveDir != E_MoveDir.right)
        {
            _moveDir = moveDir;
        }
        else if(moveDir == E_MoveDir.up && _moveDir != E_MoveDir.down)
        {
            _moveDir = moveDir;
        }
        else if(moveDir == E_MoveDir.right && _moveDir != E_MoveDir.left)
        {
            _moveDir = moveDir;
        }
        else if(moveDir == E_MoveDir.down && _moveDir != E_MoveDir.up)
        {
            _moveDir = moveDir;
        }
        else
        {
            //TODO: delet this print info after complete this;
            Debug.Log($"cur move dir is 【{_moveDir}】, can't change to the 【{moveDir}】");
        }
    }

    private Vector2Int _oldTailPos;

    /// <summary>
    /// make the tail become longer
    /// </summary>
    public void Grow()
    {
        snakeBodyList.Add(_oldTailPos);
        //Update the tail pos to the map
        _map.UpdateSnakeBodyPos(_oldTailPos);
    }

    /// <summary>
    /// Update the 
    /// </summary>
    public void Move()
    {
        _moveDir = _MoveDirBuffer;

        //clear the end of the place
        _map.UpdateTheNullPos(snakeBodyList[snakeBodyList.Count - 1]);
        
        _oldTailPos = snakeBodyList[snakeBodyList.Count - 1];
        //update form tail to head(except head)
        for(int i = snakeBodyList.Count-1; i >= 1; i--)
        {
            snakeBodyList[i] = snakeBodyList[i-1];
        }


        Vector2Int newHeadPos = snakeBodyList[0];
        //move head first
        switch (_moveDir)
        {
            case E_MoveDir.left: 
                newHeadPos += Vector2Int.left;
                break;
            case E_MoveDir.up:
                newHeadPos -= Vector2Int.up;
                break;
            case E_MoveDir.right:
                newHeadPos += Vector2Int.right;
                break;
            case E_MoveDir.down:
                newHeadPos -= Vector2Int.down;
                break;
        }


        
        int collsionResult = _map.CollisionJudge(newHeadPos);
        if (collsionResult == 1)
        {   
            _map._isGameOver = true;
        }
        else if(collsionResult == 2)
        {
            Debug.Log("Eat the apple and grow");
            Grow();
            _map.GenerateNewApple();
        }

        snakeBodyList[0] = newHeadPos;
        //Update the head pos to the map
        _map.UpdateSnakeHeadPos(snakeBodyList[0]);
        //the first body place need update too
        _map.UpdateSnakeBodyPos(snakeBodyList[1]);
    }
}
