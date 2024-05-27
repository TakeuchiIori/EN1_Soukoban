using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Input;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{

    public GameObject playerPrefab;
    public GameObject boxPrefab;
    public GameObject goalPrefab;
    public GameObject clearText;
    public GameObject ParticlePrefab;
    public GameObject BlockPrefab;
    int[,] map;          // レベルデザイン用の配列
    GameObject[,] field; // ゲーム管理用の配列

    public class MoveRecord
    {
        public string tag;
        public Vector2Int moveFrom;
        public Vector2Int moveTo;

        public MoveRecord(string _tag, Vector2Int _moveFrom, Vector2Int _moveTo)
        {
            tag = _tag;
            moveFrom = _moveFrom;
            moveTo = _moveTo;
        }
    }
    List<MoveRecord> moveHistory = new List<MoveRecord>();
    int historyIndex = -1;

    bool IsCleard()
    {
        List<Vector2Int> goals = new List<Vector2Int>();
        for (int y = 0; y < map.GetLength(0); y++)
        {
            for (int x = 0; x < map.GetLength(1); x++)
            {
                if (map[y, x] == 3)
                {
                    goals.Add(new Vector2Int(x, y));
                }
            }
        }

        for (int i = 0; i < goals.Count; i++)
        {
            GameObject f = field[goals[i].y, goals[i].x];
            if (f == null || f.tag != "Box")
            {
                return false;
            }
        }
        return true;
    }

    Vector3 IndexToPosition(Vector2Int index)
    {
        return new Vector3(
           index.x - map.GetLength(1) / 2 + 0.5f,
           -index.y + map.GetLength(0) / 2,
            0);
    }

    Vector2Int GetPlayerIndex()
    {
        for (int y = 0; y < field.GetLength(0); y++)
        {
            for (int x = 0; x < field.GetLength(1); x++)
            {
                if (field[y, x] == null) { continue; }
                if (field[y, x].tag == "Player") { return new Vector2Int(x, y); }
            }
        }
        return new Vector2Int(-1, -1);
    }

    public bool MoveNumber(string tag, Vector2Int moveFrom, Vector2Int moveTo, bool isRecording = true)
{
   // Debug.Log($"MoveNumber called with tag={tag}, moveFrom={moveFrom}, moveTo={moveTo}, isRecording={isRecording}");

    if (moveTo.y < 0 || moveTo.y >= field.GetLength(0)) { return false; }
    if (moveTo.x < 0 || moveTo.x >= field.GetLength(1)) { return false; }
    if (map[moveTo.y, moveTo.x] == 4) { return false; }
    
    // ボックスの移動を記録するための MoveRecord オブジェクトを作成
   // MoveRecord moveRecord = new MoveRecord(tag, moveFrom, moveTo);

    // 目標位置が空であるか、押せるボックスがあるかを確認する
    if (field[moveTo.y, moveTo.x] == null || (field[moveTo.y, moveTo.x].tag == "Box" && MoveNumber("Box", moveTo, moveTo + (moveTo - moveFrom), false)))
    {
        if (field[moveTo.y, moveTo.x] != null && field[moveTo.y, moveTo.x].tag == "Box")
        {
            // 目標位置にボックスがある場合は再帰的にボックスを移動する
            bool success = MoveNumber("Box", moveTo, moveTo + (moveTo - moveFrom), false);
            if (!success) { return false; }
        }

        // 実際の移動処理
        Vector3 moveToPosition = IndexToPosition(new Vector2Int(moveTo.x, moveTo.y));
        field[moveFrom.y, moveFrom.x].GetComponent<Move>().MoveTo(moveToPosition);
        field[moveTo.y, moveTo.x] = field[moveFrom.y, moveFrom.x];
        field[moveFrom.y, moveFrom.x] = null;

        for (int i = 0; i < 10; ++i)
        {
            Instantiate(ParticlePrefab, IndexToPosition(moveFrom), Quaternion.identity);
        }

            // Undo/Redo用に移動履歴を記録
            if (isRecording)
            {
                // プレイヤーかボックスかに応じて移動履歴を作成する
                if (tag == "Player" || tag == "Box")
                {
                    moveHistory.Add(new MoveRecord(tag, moveFrom, moveTo));
                    historyIndex = moveHistory.Count - 1;
                }
            }

            return true;
    }
    return false; // 移動が失敗した場合
}


   

    private float clearTimer;
    void InitializeStage1()
    {
        map = new int[,] {
           { 4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4, },
            { 4,3,0,0,0,0,0,0,2,0,0,0,0,0,0,0,0,0,4, },
            { 4,0,0,2,0,0,0,0,2,0,0,0,0,0,0,0,0,0,4, },
            { 4,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,4, },
            { 4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,4, },
            { 4,3,0,0,0,0,0,0,0,3,0,0,0,0,0,0,0,0,4, },
            { 4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,4, },
            { 4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,4, },
            { 4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,4, },
            { 4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,4, },
            { 4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4, },
        };
    }
    void InitializeStage2()
    {
        map = new int[,] {
            { 4,4,4,4,4,4,4,4,4,4,4, },
            { 4,0,0,0,0,0,0,0,0,0,4, },
            { 4,0,3,3,0,0,0,0,0,0,4, },
            { 4,0,0,0,0,1,0,0,0,0,4, },
            { 4,0,0,0,0,0,0,0,2,0,4, },
            { 4,0,0,0,0,0,0,0,2,0,4, },
            { 4,0,0,0,0,2,0,0,0,0,4, },
            { 4,0,3,0,0,0,0,0,0,0,4, },
            { 4,0,0,0,0,0,0,0,0,0,4, },
            { 4,4,4,4,4,4,4,4,4,4,4, },
        };
    }

    private void InitializeObjects()
    {
        field = new GameObject[map.GetLength(0), map.GetLength(1)];
        for (int y = 0; y < map.GetLength(0); y++)
        {
            for (int x = 0; x < map.GetLength(1); x++)
            {
                if (map[y, x] == 1)
                {
                    field[y, x] = Instantiate(playerPrefab, IndexToPosition(new Vector2Int(x, y)), Quaternion.identity);
                }
                if (map[y, x] == 2)
                {
                    field[y, x] = Instantiate(boxPrefab, IndexToPosition(new Vector2Int(x, y)), Quaternion.identity);
                }
                if (map[y, x] == 3)
                {
                    Instantiate(goalPrefab, IndexToPosition(new Vector2Int(x, y)), Quaternion.identity);
                }
                if (map[y, x] == 4)
                {
                    Instantiate(BlockPrefab, IndexToPosition(new Vector2Int(x, y)), Quaternion.identity);
                }
            }
        }
    }
    void ResetGame()
    {
        // プレイヤーとボックスのオブジェクトを破棄し、再生成する
        foreach (GameObject obj in field)
        {
            if (obj != null && (obj.tag == "Player" || obj.tag == "Box"))
            {
                Destroy(obj);
            }
        }
        InitializeObjects();
    }
    // Undoメソッドの修正
   public void UndoMove()
{
    if (historyIndex >= 0)
    {
        MoveRecord record = moveHistory[historyIndex];
        if (record.tag == "Player" || record.tag == "Box")
        {
            MoveNumber(record.tag, record.moveTo, record.moveFrom, false);
        }
        historyIndex--;
    }
}



    // Redoメソッドの修正
    public void RedoMove()
    {
        if (historyIndex < moveHistory.Count - 1)
        {
            historyIndex++;
            MoveRecord record = moveHistory[historyIndex];
           // Debug.Log($"Redo: tag={record.tag}, moveFrom={record.moveFrom}, moveTo={record.moveTo}");
            if (record.tag == "Player")
            {
                MoveNumber("Player", record.moveFrom, record.moveTo, false);
            }
            else if (record.tag == "Box")
            {
                MoveNumber("Box", record.moveFrom, record.moveTo, false);
            }
        }
    }
    void Move()
    {
        if (!IsCleard())
        {
            Vector2Int playerIndex = GetPlayerIndex();

            if (Input.GetKeyDown(KeyCode.W))
            {
                MoveNumber("Player", playerIndex, playerIndex + new Vector2Int(0, -1));
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                MoveNumber("Player", playerIndex, playerIndex + new Vector2Int(0, 1));
            }

            if (Input.GetKeyDown(KeyCode.D))
            {
                MoveNumber("Player", playerIndex, playerIndex + new Vector2Int(1, 0));
            }

            if (Input.GetKeyDown(KeyCode.A))
            {
                MoveNumber("Player", playerIndex, playerIndex + new Vector2Int(-1, 0));
            }
            // UndoとRedoの入力処理
            if (Input.GetKeyDown(KeyCode.Z))
            {
                UndoMove();
            }

            if (Input.GetKeyDown(KeyCode.Y))
            {
                RedoMove();
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                ResetGame();
            }
        }

        if (IsCleard())
        {
            clearText.SetActive(true);
            StartCoroutine(WaitAndReset());
        }
        else
        {
            clearText.SetActive(false);
        }
        IEnumerator WaitAndReset()
        {
            yield return new WaitForSeconds(3f);
            ResetGame();
        }


    }
    void Start()
    {
        Screen.SetResolution(1280, 720, false);
        InitializeStage1();
        InitializeObjects();
    }

    void Update()
    {
        Move();

    }
 
}
