using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SweepyDungeon
{
    public class BoardManager : MonoBehaviour
    {
        public int[,,] board;
        public int boardSize;
        public double enemySpawnRate = 0.2;
        public double itemSpawnRate = 0.2;


        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void SetBoard(int size)
        {
            board = new int[size, size, 3];//ũ�⿡ �°� Ÿ�� ���� ����. ���� ���簢�� ����.
                                           //3���� ���̾�� �ٴ�, ������Ʈ, ����ã�� UI(����)

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    board[i, j, 0] = Terrain.GROUND;
                    board[i, j, 1] = -1;
                    board[i, j, 2] = 0;
                }
            }//�ٴ�, ������Ʈ, ����ã�� UI �ʱ�ȭ. 

            int enemySpawnCount = (int)(size * size * enemySpawnRate);
            int itemSpawnCount = (int)(size * size * itemSpawnRate);
            boardSize = size;
            //
        }

        public void SetUINumber()
        {
            for (int i = 0; i < boardSize; i++)
            {
                for (int j = 0; j < boardSize; j++)
                {
                    /*if(i != 0)
                    {
                        if()
                        {

                        }
                    }
                    if(i + 1 = boardSize)
                    {

                    }*/

                }
            }
        }
    }
}