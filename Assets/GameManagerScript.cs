using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    // Start is called before the first frame update
    int[] map;

    void PrintAarray()//Indexなどを調べる処理
    {
        string debugText = "";
        for (int i = 0; i < map.Length; i++)
        {
            debugText += map[i].ToString() + ",";
        }
        Debug.Log(debugText);
    }

    int GetPlayerIndex()//1が格納されているIndexを取得する処理
    {
        for(int i = 0; i < map.Length;i++)
        {
            if (map[i] == 1)
            {
                return i;
            }
        }
        return -1;
    }

    bool MoveNumber(int number,int moveFrom,int moveTo)//配列外条件 
    {
        if(moveTo < 0 || moveTo >= map.Length) { return false; }
        //移動先に2(箱)が居たら...
        if (map[moveTo] == 2)
        {
            //どの方向へ移動するか算出
            int velocity = moveTo - moveFrom;

            bool success = MoveNumber(2, moveTo, moveTo + velocity);
            //箱の移動に失敗したらプレイヤーも失敗
            if (!success) { return false; }
        }
        //プレイヤー・箱かかわらずの移動処理
        map[moveTo] = number;
        map[moveFrom] = 0;
        return true;
    }

   void Start()
    {
        map = new int[] { 0, 0, 0, 0, 0, 2, 1, 0, 0 };
     
    }

    // Update is called once per frame
    void Update()
    {
     
        if(Input.GetKeyUp(KeyCode.RightArrow)) 
        {
            int playerIndex = GetPlayerIndex();

            MoveNumber(1, playerIndex, playerIndex + 1);
            PrintAarray();

        }

        if (Input.GetKeyUp(KeyCode.LeftArrow))
        {
            int playerIndex = GetPlayerIndex();

            MoveNumber(1, playerIndex, playerIndex - 1);

            PrintAarray();

        }
    }

}
