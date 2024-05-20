using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class NewBehaviourScript : MonoBehaviour
{
   // Start is called before the first frame update
    int[] map_2;
    // 追加
    public GameObject playerPrefab;
    public GameObject boxPrefab;
    public GameObject goalPrefab;
    public GameObject clearText;
    public GameObject ParticlePrefab;
    int[,] map;          // レベルデザイン用の配列
    GameObject[,] field; // ゲーム管理用の配列

    bool IsCleard()
    {
        // Vector2Int型の可変配列の作成
        List<Vector2Int> goals = new List<Vector2Int>();
        for (int y = 0; y < map.GetLength(0); y++)
        {
            for (int x = 0; x < map.GetLength(1); x++)
            {
                // 格納場所か否かを判断
                if (map[y,x] == 3)
                {
                    // 格納場所のインデックスを控えておく
                    goals.Add(new Vector2Int(x,y));
                }
            }
        }

        for(int i = 0; i < goals.Count; i++)
        {
            GameObject f = field[goals[i].y,goals[i].x];
            if(f == null || f.tag != "Box")
            {
                // 1つでも箱が無かったら条件未達成
                return false;
            }
        }
        // 条件未達成でなければ条件達成
        return true;
    }

    Vector3 IndexToPosition(Vector2Int index)
    {
        return new Vector3(
           index.x - map.GetLength(1) / 2 + 0.5f,
           -index.y + map.GetLength(0) / 2,
            0);
    }

    Vector2Int GetPlayerIndex()// 1が格納されているIndexを取得する処理
    {
        for (int y = 0; y < field.GetLength(0); y++)
        {
           for(int x = 0;x < field.GetLength(1); x++)
            {
                if (field[y,x] == null) { continue; }
                if (field[y,x].tag == "Player") { return new Vector2Int(x, y); }
            }
        }
        return new Vector2Int(-1,-1);
    }

    bool MoveNumber(string tag, Vector2Int moveFrom, Vector2Int moveTo)//配列外条件 
    {
        // 縦軸横軸の配列外参照していないか
        if (moveTo.y < 0 || moveTo.y >= field.GetLength(0)) { return false; }
        if (moveTo.x < 0 || moveTo.x >= field.GetLength(1)) { return false; }
        // Boxタグを持っていたら再起処理
        if (field[moveTo.y,moveTo.x] != null && field[moveTo.y,moveTo.x].tag == "Box")
        {
            Vector2Int velocity = moveTo - moveFrom;
            bool success = MoveNumber(tag, moveTo, moveTo + velocity);
            // 箱の移動に失敗したらプレイヤーも失敗
            if (!success) { return false; }
            }
        // GameObjectの座標(Position)を移動させてからインデックスの入れ替え
        Vector3 moveToPosition =
           IndexToPosition(new Vector2Int(moveTo.x, moveTo.y));
        field[moveFrom.y, moveFrom.x].GetComponent<Move>().MoveTo(moveToPosition );
        field[moveTo.y, moveTo.x] = field[moveFrom.y, moveFrom.x];
        field[moveFrom.y, moveFrom.x] = null;
        for (int i = 0; i < 10; ++i)
        {
            Instantiate(
            ParticlePrefab,
            IndexToPosition(moveFrom),
            Quaternion.identity
        );
        }
            return true;
    }



    void Start()
    {
        Screen.SetResolution(1280, 720, false);
        map = new int[,] { // 3をゴール
        { 0,0,0,0,3 },
        { 0,0,2,2,0 },
        { 0,0,1,0,0 },
        { 0,0,0,0,0 },
        { 3,0,0,0,0 },
        };
        // 二重for文で二次元配列の情報を出力
        field = new GameObject
       [
       map.GetLength(0),
       map.GetLength(1)
       ];
        for (int y = 0; y < map.GetLength(0); y++)
        {
            for (int x = 0; x < map.GetLength(1); x++)
            {
                if (map[y, x] == 1)
                {
                    field[y, x] = Instantiate(
                        playerPrefab,
                       IndexToPosition(new Vector2Int(x,y)),
                        Quaternion.identity
                        );
                }
                if (map[y, x] == 2)
                {
                    field[y, x] = Instantiate(
                        boxPrefab,
                         IndexToPosition(new Vector2Int(x, y)),
                        Quaternion.identity
                        );
                } 
                if(map[y, x] == 3)
                {
                        Instantiate(
                        goalPrefab,
                        IndexToPosition(new Vector2Int(x, y)),
                        Quaternion.identity
                        );
                }
                
            }
        }

 

    }

    void Update()
    {

        if (Input.GetKeyUp(KeyCode.W))
        {
            Vector2Int playerIndex = GetPlayerIndex();
            MoveNumber("Player", 
                playerIndex, 
                playerIndex + new Vector2Int(0,-1)
                );
        }

        if (Input.GetKeyUp(KeyCode.S))
        {
            Vector2Int playerIndex = GetPlayerIndex();
            MoveNumber("Player", 
                playerIndex,
                playerIndex + new Vector2Int(0, 1)
                );
        }

        if (Input.GetKeyUp(KeyCode.D))
        {
            Vector2Int playerIndex = GetPlayerIndex();
            MoveNumber("Player",
               playerIndex,
               playerIndex + new Vector2Int(1, 0)
               );
        }

        if (Input.GetKeyUp(KeyCode.A))
        {
            Vector2Int playerIndex = GetPlayerIndex();
            MoveNumber("Player",
                playerIndex,
                playerIndex + new Vector2Int(-1, 0)
                );

        }
        // もしクリアしてたら
        if (IsCleard())
        {
            // ゲームオブジェクトのSetActiveメソッドを使い有効化
            clearText.SetActive(true);
            Debug.Log("Clear");
        }
        else
        {
            clearText.SetActive(false);
        }

    }//Update

}
