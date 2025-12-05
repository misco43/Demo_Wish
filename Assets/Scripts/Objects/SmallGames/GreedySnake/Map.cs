using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Map : MonoBehaviour
{
    public RectTransform imgGameBKRect;
    public GameObject snakeGameTile;
    public GameObject snakeHead;
    public GameObject snakeBody;
    public GameObject apple;

    public int[ , ] grid = new int[9, 8];  //0 means null ; 1 means snake ; 2 means apple; 3 means head

    private Snake _snake;
    public bool _isGameOver = false;

    //private Dictionary<int, List<GameObject>> _pool = new Dictionary<int, List<GameObject>>();
    private List<GameObject> _objList;
    private float _timer = -1f;
    public float UpdateInterval = 1f;

    private void OnEnable()
    {
        _objList = new List<GameObject>();

        //1.Generate the snake in random place
        _snake = new Snake(this);

        //2.Generate the apple in random place
        GenerateNewApple();
        UpdateTheView();
    }

    private void Update()
    {
        if (_isGameOver)
        {
            return;
        }

        //Get Input
        if(Keyboard.current != null)
        {
            if (Keyboard.current.aKey.wasPressedThisFrame)
            {
                _snake.TryChangeMoveDir(Snake.E_MoveDir.left);
            }
            else if (Keyboard.current.wKey.wasPressedThisFrame)
            {
                _snake.TryChangeMoveDir(Snake.E_MoveDir.up);
            }
            else if (Keyboard.current.dKey.wasPressedThisFrame)
            {
                _snake.TryChangeMoveDir(Snake.E_MoveDir.right);
            }
            else if (Keyboard.current.sKey.wasPressedThisFrame)
            {
                _snake.TryChangeMoveDir(Snake.E_MoveDir.down);
            }
        }
        else
        {
            Debug.LogWarning("Keyboard is null!");
        }

        
        if(_timer < UpdateInterval)
        {
            _timer += Time.deltaTime;
            return;
        }
        else
        {
            _timer = 0f;
        }

        //Update the logic
        //Snake Move and update it's position
        _snake.Move();

        //Do the collsion judge after the move
        // int collsionResult = CollisionJudge();
        // if (collsionResult == 1)
        // {   
        //     _isGameOver = true;
        // }
        // else if(collsionResult == 2)
        // {
        //     Debug.Log("Eat the apple and grow");
        //     _snake.Grow();
        //     GenerateNewApple();
        // }

        //Update the view with the grid data
        UpdateTheView();
    }

    private void OnDisable()
    {
        _snake = null;
        //Clear the objList
        for(int i = 0; i < _objList.Count; i++)
        {
            Destroy(_objList[i]);
        }
        _objList.Clear();

        //clear the grids
        for(int i = 0; i < 9; i++)
        {
            for(int j = 0; j < 8; j++)
            {
                grid[i, j] = 0;
            }
        }

    }

    public void GenerateNewApple()
    {
        int row = Random.Range(0, 9);
        int col = Random.Range(0, 8);

        while(grid[row, col] != 0)
        {
            row = Random.Range(0, 9);
            col = Random.Range(0, 8);
        }

        grid[row, col] = 2;
    }

    public void UpdateTheNullPos(Vector2Int pos)
    {
        if(pos.y < 0 || pos.y >= 9 || pos.x < 0 || pos.x >= 8)
        {
            Debug.LogWarning($"the pos you pass in {pos} is out of bound");
            return;    
        }

        grid[pos.y, pos.x] = 0;
    }
    public void UpdateSnakeBodyPos(Vector2Int pos)
    {
        if(pos.y < 0 || pos.y >= 9 || pos.x < 0 || pos.x >= 8)
        {
            Debug.LogWarning($"the pos you pass in {pos} is out of bound");
            return;    
        }

        grid[pos.y, pos.x] = 1;
    }


    public void UpdateSnakeHeadPos(Vector2Int pos)
    {
        if(pos.y < 0 || pos.y >= 9 || pos.x < 0 || pos.x >= 8)
        {
            Debug.LogWarning($"the pos you pass in {pos} is out of bound");
            return;    
        }

        grid[pos.y, pos.x] = 3; 
    } 

    /// <summary>
    /// Judge if the snake head is collsion with something
    /// </summary>
    /// <param name="pos">snake head pos</param>
    /// <returns>1 is body or wall; 2 is apple; 0 is nothing</returns>
    public int CollisionJudge(Vector2Int pos)
    {
        int row = pos.y;
        int col = pos.x;

        if(row < 0 || row >= 9 || col < 0 || col >= 8)
        {
            return 1;
        }
        else if(grid[row, col] == 1)
        {
            return 1;
        }
        else if(grid[row, col] == 2)
        {
            return 2;
        }

        return 0;
    }

    public void UpdateTheView()
    {
        //Clear all the exist view obj
        for(int i = 0; i < _objList.Count; i++)
        {
            DestroyImmediate(_objList[i]);
        }
        _objList.Clear();

        for(int i = 0; i < 9; i++)
        {
            for(int j = 0; j < 8; j++)
            {
                GameObject obj = null;
                int curObj = grid[i, j];
                if(curObj == 1)
                {
                    obj = GameObject.Instantiate(snakeBody, imgGameBKRect);
                }
                else if(curObj == 2)
                {
                    obj = GameObject.Instantiate(apple, imgGameBKRect);
                }
                else if(curObj == 3)
                {
                    obj = GameObject.Instantiate(snakeHead, imgGameBKRect);
                }

                if(obj != null)
                {
                    (obj.transform as RectTransform).anchoredPosition = new Vector2((float)j/10f, -(float)i/10f);
                    _objList.Add(obj);
                }
                
            }
        }
    }
}
