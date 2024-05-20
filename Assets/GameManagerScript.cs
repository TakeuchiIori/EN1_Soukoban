using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class NewBehaviourScript : MonoBehaviour
{
   // Start is called before the first frame update
    int[] map_2;
    // �ǉ�
    public GameObject playerPrefab;
    public GameObject boxPrefab;
    public GameObject goalPrefab;
    public GameObject clearText;
    public GameObject ParticlePrefab;
    int[,] map;          // ���x���f�U�C���p�̔z��
    GameObject[,] field; // �Q�[���Ǘ��p�̔z��

    bool IsCleard()
    {
        // Vector2Int�^�̉ϔz��̍쐬
        List<Vector2Int> goals = new List<Vector2Int>();
        for (int y = 0; y < map.GetLength(0); y++)
        {
            for (int x = 0; x < map.GetLength(1); x++)
            {
                // �i�[�ꏊ���ۂ��𔻒f
                if (map[y,x] == 3)
                {
                    // �i�[�ꏊ�̃C���f�b�N�X���T���Ă���
                    goals.Add(new Vector2Int(x,y));
                }
            }
        }

        for(int i = 0; i < goals.Count; i++)
        {
            GameObject f = field[goals[i].y,goals[i].x];
            if(f == null || f.tag != "Box")
            {
                // 1�ł���������������������B��
                return false;
            }
        }
        // �������B���łȂ���Ώ����B��
        return true;
    }

    Vector3 IndexToPosition(Vector2Int index)
    {
        return new Vector3(
           index.x - map.GetLength(1) / 2 + 0.5f,
           -index.y + map.GetLength(0) / 2,
            0);
    }

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
        map = new int[,] { // 3���S�[��
        { 0,0,0,0,3 },
        { 0,0,2,2,0 },
        { 0,0,1,0,0 },
        { 0,0,0,0,0 },
        { 3,0,0,0,0 },
        };
        // ��dfor���œ񎟌��z��̏����o��
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
        // �����N���A���Ă���
        if (IsCleard())
        {
            // �Q�[���I�u�W�F�N�g��SetActive���\�b�h���g���L����
            clearText.SetActive(true);
            Debug.Log("Clear");
        }
        else
        {
            clearText.SetActive(false);
        }

    }//Update

}
