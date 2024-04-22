using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
   // Start is called before the first frame update
    int[] map_2;
    // �ǉ�
    public GameObject playerPrefab;
    int[,] map;          // ���x���f�U�C���p�̔z��
    GameObject[,] field; // �Q�[���Ǘ��p�̔z��

    //void PrintAarray()//Index�Ȃǂ𒲂ׂ鏈��
    //{
    //    string debugText = "";
    //    for (int i = 0; i < map_2.Length; i++)
    //    {
    //        debugText += map_2[i].ToString() + ",";
    //    }
    //    Debug.Log(debugText);
    //}

   Vector2Int GetPlayerIndex()// 1���i�[����Ă���Index���擾���鏈��
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

    bool MoveNumber(string tag, Vector2Int moveFrom, Vector2Int moveTo)//�z��O���� 
    {
        // �c�������̔z��O�Q�Ƃ��Ă��Ȃ���
        if (moveTo.y < 0 || moveTo.y >= field.GetLength(0)) { return false; }
        if (moveTo.x < 0 || moveTo.x >= field.GetLength(1)) { return false; }
        // Box�^�O�������Ă�����ċN����
        if (field[moveTo.y,moveTo.x] != null && field[moveTo.y,moveTo.x].tag == "Box")
        {
            Vector2Int velocity = moveTo - moveFrom;
            bool success = MoveNumber(tag, moveTo, moveTo + velocity);
            // ���̈ړ��Ɏ��s������v���C���[�����s
            if (!success) { return false; }
        }
        // GameObject�̍��W(Position)���ړ������Ă���C���f�b�N�X�̓���ւ�
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
        // ��dfor���œ񎟌��z��̏����o��
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
