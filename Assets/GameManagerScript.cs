using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
   // Start is called before the first frame update
    int[] map_2;
    // 追加
    public GameObject playerPrefab;
    int[,] map;          // レベルデザイン用の配列
    GameObject[,] field; // ゲーム管理用の配列

    //void PrintAarray()//Indexなどを調べる処理
    //{
    //    string debugText = "";
    //    for (int i = 0; i < map_2.Length; i++)
    //    {
    //        debugText += map_2[i].ToString() + ",";
    //    }
    //    Debug.Log(debugText);
    //}

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
        field[moveFrom.y, moveFrom.x].transform.position =
            new Vector3(moveTo.x, field.GetLength(0) - moveTo.y, 0);
        field[moveTo.y, moveTo.x] = field[moveFrom.y, moveFrom.x];
        field[moveFrom.y, moveFrom.x] = null;
        return true;
    }



    void Start()
    {
        map = new int[,] {
        { 0,0,0,0,0 },
        { 0,0,0,0,0 },
        { 0,0,1,0,0 },
        };
        // 二重for文で二次元配列の情報を出力
        field = new GameObject
        [
        map.GetLength(0),
        map.GetLength(1)
        ];
        for(int y = 0; y <  map.GetLength(0); y++)
        {
            for(int x = 0;x <  map.GetLength(1); x++)
            {
                field[y, x] = Instantiate(
                    playerPrefab,
                    new Vector3(
                    x - map.GetLength(1)/2, 
                    - y + map.GetLength(0)/2, 
                    0),
                    Quaternion.identity
                    );
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
     
    }

}
