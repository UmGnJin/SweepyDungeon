using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = System.Random;

namespace SweepyDungeon
{
    public class BoardManager : MonoBehaviour
    {
        Random random = new Random();

        public GameObject[] Tiles;// Ÿ�� prefab �ҷ�����
        public GameObject[] Mobs;
        public GameObject[] Items;
        public GameObject[] Players;
        public GameObject[] Numbers;

        public static BoardManager manager;

        public GameObject playerObject;
        public Player player;
        public int[,,] board;
        public Entity[,] entities;
        public double enemySpawnRate = 0.2;
        public double itemSpawnRate = 0.2;
        public bool gameOver = false;

        // Start is called before the first frame update
        void Awake()
        {
            // ������Ʈ�� �̱���ȭ�Ͽ� 1���� BoardManager���� �����ϵ��� ����.
            if (manager == null)
            {
                manager = this;
                DontDestroyOnLoad(this);
            }
            else if (manager != this)
                Destroy(this.gameObject);
            
            //������ �ε�
            Tiles = Resources.LoadAll<GameObject>("prefabs/Tiles");
            Mobs = Resources.LoadAll<GameObject>("prefabs/Enemies");
            Items = Resources.LoadAll<GameObject>("prefabs/Objects");
            Players = Resources.LoadAll<GameObject>("prefabs/Player");
            Numbers = Resources.LoadAll<GameObject>("prefabs/Numbers");

            //
            SetBoard();
            PrintBoard();
            player.CheckObjects();
        }
        private void FixedUpdate()
        {
            if (gameOver && Input.GetKeyDown(KeyCode.Space))
            {
                NextLevel();
                gameOver = false;
            }
        }
        public void SetBoard()
        {
            board = new int[7, 7, 3];//ũ�⿡ �°� Ÿ�� ���� ����. ���� ���簢�� ����.
                                     //3���� ���̾�� �ٴ�, ������Ʈ
            entities = new Entity[7, 7];

            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    board[i, j, 0] = Terrain.GROUND;
                    board[i, j, 1] = Object.EMPTY;
                }
            }//�ٴ�, ������Ʈ, ����ã�� UI �ʱ�ȭ. 

            board[3, 3, 1] = Object.PLAYER;
            if (playerObject == null || player == null)
            {
                playerObject = Instantiate(Players[0], new Vector2(3, 3), Quaternion.identity) as GameObject;
                player = playerObject.GetComponent<Player>();
            }
            else
            {
                player.x = 3;
                player.y = 3;
                player.transform.position = new Vector2(3, 3);
            }
            //�÷��̾� ����

            switch (random.Next(0, 4))
            { 
                case 0:
                    board[0, 0, 0] = Terrain.DOOR;
                    board[0, 0, 1] = Object.DOOR;
                    break;
                case 1:
                    board[6, 0, 0] = Terrain.DOOR;
                    board[6, 0, 1] = Object.DOOR;
                    break;
                case 2:
                    board[0, 6, 0] = Terrain.DOOR;
                    board[0, 6, 1] = Object.DOOR;
                    break;
                case 3:
                    board[6, 6, 0] = Terrain.DOOR;
                    board[6, 6, 1] = Object.DOOR;
                    break;
            }
            //�� ����

            int enemySpawnCount = 10;
            int itemSpawnCount = 10;
            
            while (enemySpawnCount > 0)
            {
                int count = 0;
                for (int i = 0; i < 7; i++)
                {
                    for (int j = 0; j < 7; j++)
                    {
                        if (board[i, j, 1] == Object.EMPTY)
                        {
                            count++;
                        }
                    }
                }
                int num = random.Next(0, count);
                for (int i = 0; i < 7; i++)
                {
                    for (int j = 0; j < 7; j++)
                    {
                        if (board[i, j, 1] == Object.EMPTY)
                        {
                            if(num == 0)
                            {
                                GameObject enemyObject = Instantiate(Mobs[random.Next(0, 6)], new Vector2(i, j), Quaternion.identity) as GameObject;
                                board[i, j, 1] = Object.ENEMY;
                                entities[i, j] = enemyObject.GetComponent<Enemy>();
                                entities[i, j].Init(i, j);
                                enemyObject.GetComponent<Enemy>().SetHP(1);
                                enemySpawnCount--;
                                num--;
                                break;
                            }
                            num--;
                        }
                    }
                }
            }
            //�� ����

            while (itemSpawnCount > 0)
            {
                int count = 0;
                for (int i = 0; i < 7; i++)
                {
                    for (int j = 0; j < 7; j++)
                    {
                        if (board[i, j, 1] == Object.EMPTY)
                        {
                            count++;
                        }
                    }
                }
                int num = random.Next(0, count);
                for (int i = 0; i < 7; i++)
                {
                    for (int j = 0; j < 7; j++)
                    {
                        if (board[i, j, 1] == Object.EMPTY)
                        {
                            if (num == 0)
                            {
                                if (itemSpawnCount % 2 == 0)
                                {
                                    GameObject enemyObject = Instantiate(Items[0], new Vector2(i, j), Quaternion.identity) as GameObject;
                                    board[i, j, 1] = Object.COIN;
                                    entities[i, j] = enemyObject.GetComponent<Coin>();
                                    entities[i, j].Init(i, j);
                                    enemyObject.GetComponent<Coin>().SetValue(random.Next(1, 3));
                                }
                                else
                                {
                                    GameObject enemyObject = Instantiate(Items[1], new Vector2(i, j), Quaternion.identity) as GameObject;
                                    board[i, j, 1] = Object.HEART;
                                    entities[i, j] = enemyObject.GetComponent<Heart>();
                                    entities[i, j].Init(i, j);
                                }
                                itemSpawnCount--;
                                num--;
                                break;
                            }
                            num--;
                        }
                    }
                }
            }
            //������ ����
        }

        public void PrintBoard()
        {
            for(int i = 0; i < board.GetLength(0) ; i++)
            {
                for(int j = 0; j < board.GetLength(1) ; j++)
                {
                    GameObject tileObject;
                    int tile = board[i, j, 0];
                    switch (tile)
                    {
                        case Terrain.EMPTY:
                            continue;
                        case Terrain.GROUND:
                            tileObject = Tiles[Array.FindIndex(Tiles, t => t.name == "FloorTile")];
                            break;
                        case Terrain.WALL:
                            tileObject = Tiles[Array.FindIndex(Tiles, t => t.name == "WallTile")];
                            break;
                        case Terrain.STAIRS_UP:
                            tileObject = Tiles[Array.FindIndex(Tiles, t => t.name == "Upstairs")];
                            break;
                        case Terrain.STAIRS_DOWN:
                            tileObject = Tiles[Array.FindIndex(Tiles, t => t.name == "Downstairs")];
                            break;
                        case Terrain.DOOR:
                            tileObject = Tiles[Array.FindIndex(Tiles, t => t.name == "DoorTile")];
                            break;;
                        default:
                            continue;
                    }

                    GameObject newTile = Instantiate(tileObject, new Vector2(i, j), Quaternion.identity) as GameObject;
                    newTile.transform.SetParent(this.transform, false);
                }
            }
        }

        public Entity FindEntity(int x, int y)
        {
            return entities[x, y];
        }

        public void NextLevel()
        {
            foreach (Entity entity in entities)
            {
                if(entity != null)
                    entity.Delete();
            }
            foreach (Transform child in manager.transform)
                Destroy(child.gameObject);
            SetBoard();
            PrintBoard();
            player.CheckObjects();
        }
    }
}