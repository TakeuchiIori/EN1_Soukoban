using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Input;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    int[] map_2;
    public GameObject playerPrefab;
    public GameObject boxPrefab;
    public GameObject goalPrefab;
    public GameObject clearText;
    public GameObject ParticlePrefab;
    public GameObject BlockPrefab;
    int[,] map;          // レベルデザイン用の配列
    GameObject[,] field; // ゲーム管理用の配列

    private UndoManager undoManager = new UndoManager(); // UndoManagerのインスタンス化

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
        Debug.Log($"MoveNumber called with tag={tag}, moveFrom={moveFrom}, moveTo={moveTo}, isRecording={isRecording}");

        if (moveTo.y < 0 || moveTo.y >= field.GetLength(0)) { return false; }
        if (moveTo.x < 0 || moveTo.x >= field.GetLength(1)) { return false; }
        if (map[moveTo.y, moveTo.x] == 4) { return false; }
        if (field[moveTo.y, moveTo.x] != null && field[moveTo.y, moveTo.x].tag == "Box")
        {
            Vector2Int velocity = moveTo - moveFrom;
            bool success = MoveNumber(tag, moveTo, moveTo + velocity, false);
            if (!success) { return false; }
        }

        Vector3 moveToPosition = IndexToPosition(new Vector2Int(moveTo.x, moveTo.y));
        field[moveFrom.y, moveFrom.x].GetComponent<Move>().MoveTo(moveToPosition);
        field[moveTo.y, moveTo.x] = field[moveFrom.y, moveFrom.x];
        field[moveFrom.y, moveFrom.x] = null;

        for (int i = 0; i < 10; ++i)
        {
            Instantiate(ParticlePrefab, IndexToPosition(moveFrom), Quaternion.identity);
        }

        if (isRecording)
        {
            undoManager.ExecuteCommand(new MoveCommand(field, tag, moveFrom, moveTo, this));
        }

        return true;
    }

    void ResetGame()
    {
        foreach (GameObject obj in field)
        {
            Destroy(obj);
        }
        Start();
    }

    private float clearTimer;
    void InitializeStage1()
    {
        map = new int[,] {
            { 4,4,4,4,4,4,4,4,4,4,4, },
            { 4,3,0,0,0,0,0,0,2,0,4, },
            { 4,0,0,2,0,0,0,0,2,0,4, },
            { 4,0,0,0,0,1,0,0,0,0,4, },
            { 4,0,0,0,0,0,0,0,0,0,4, },
            { 4,3,0,0,0,0,0,0,0,3,4, },
            { 4,0,0,0,0,0,0,0,0,0,4, },
            { 4,0,0,0,0,0,0,0,0,0,4, },
            { 4,0,0,0,0,0,0,0,0,0,4, },
            { 4,4,4,4,4,4,4,4,4,4,4, },
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

    void Start()
    {
        Screen.SetResolution(1280, 720, false);
        InitializeStage1();
        InitializeObjects();
    }

    void Update()
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

            if (Input.GetKeyDown(KeyCode.Z)) // Undo機能のキー
            {
                Debug.Log("Undo key pressed");
                undoManager.Undo();
            }


            if (Input.GetKeyDown(KeyCode.Y)) // Redo機能のキー
            {
                Debug.Log("Redo key pressed");
                undoManager.Redo();
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                ResetGame();
            }
        }

        if (IsCleard())
        {
            clearTimer += 1.0f / 60.0f;
            clearText.SetActive(true);
            if (clearTimer >= 20.0f)
            {
                ResetGame();
                clearTimer = 0.0f;
            }
        }
        else
        {
            clearText.SetActive(false);
        }
    }
}
public interface ICommand
{
    void Execute();
    void UnExecute();
}
public class MoveCommand : ICommand
{
    private GameObject[,] field;
    private Vector2Int moveFrom;
    private Vector2Int moveTo;
    private string tag;
    private NewBehaviourScript script;

    public MoveCommand(GameObject[,] field, string tag, Vector2Int moveFrom, Vector2Int moveTo, NewBehaviourScript script)
    {
        this.field = field;
        this.tag = tag;
        this.moveFrom = moveFrom;
        this.moveTo = moveTo;
        this.script = script;
    }

    public void Execute()
    {
        Debug.Log($"Executing MoveCommand: moveFrom={moveFrom} moveTo={moveTo}");
        script.MoveNumber(tag, moveFrom, moveTo, false); // false to prevent recording this move
    }

    public void UnExecute()
    {
        Debug.Log($"UnExecuting MoveCommand: moveFrom={moveFrom} moveTo={moveTo}");
        script.MoveNumber(tag, moveTo, moveFrom, false); // false to prevent recording this move
    }
}
public class UndoManager
{
    private Stack<ICommand> undoStack = new Stack<ICommand>();
    private Stack<ICommand> redoStack = new Stack<ICommand>();

    public void ExecuteCommand(ICommand command)
    {
        Debug.Log("Executing command");
        command.Execute();
        undoStack.Push(command);
        redoStack.Clear();
    }

    public void Undo()
    {
        if (undoStack.Count > 0)
        {
            Debug.Log("Undo operation");
            ICommand command = undoStack.Pop();
            command.UnExecute();
            redoStack.Push(command);
        }
        else
        {
            Debug.Log("Nothing to undo");
        }
    }

    public void Redo()
    {
        if (redoStack.Count > 0)
        {
            Debug.Log("Redo operation");
            ICommand command = redoStack.Pop();
            command.Execute();
            undoStack.Push(command);
        }
        else
        {
            Debug.Log("Nothing to redo");
        }
    }
}