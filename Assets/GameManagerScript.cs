using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    // Start is called before the first frame update
    int[] map;

    void PrintAarray()//Index�Ȃǂ𒲂ׂ鏈��
    {
        string debugText = "";
        for (int i = 0; i < map.Length; i++)
        {
            debugText += map[i].ToString() + ",";
        }
        Debug.Log(debugText);
    }

    int GetPlayerIndex()//1���i�[����Ă���Index���擾���鏈��
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

    bool MoveNumber(int number,int moveFrom,int moveTo)//�z��O���� 
    {
        if(moveTo < 0 || moveTo >= map.Length) { return false; }
        //�ړ����2(��)��������...
        if (map[moveTo] == 2)
        {
            //�ǂ̕����ֈړ����邩�Z�o
            int velocity = moveTo - moveFrom;

            bool success = MoveNumber(2, moveTo, moveTo + velocity);
            //���̈ړ��Ɏ��s������v���C���[�����s
            if (!success) { return false; }
        }
        //�v���C���[�E��������炸�̈ړ�����
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
