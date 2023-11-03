using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SweepyDungeon;

namespace SweepyDungeon
{
    public enum Direction
    {
        UP = 0, 
        DOWN = 1,
        LEFT = 2,
        RIGHT = 3
    }


    public class Player : Entity
    {
        public GameObject numberObject;

        public int hp;
        public int coin;
        public int nearbyObjects;
        public bool nearbyEnemies;


        private void Awake()
        {
            this.hp = 3;
            this.coin = 0;
            this.x = 3;
            this.y = 3;
            this.nearbyEnemies = false;
            this.nearbyObjects = 0;
        }
        //초기 정보 입력

        private void FixedUpdate()
        {
            if (Input.GetKeyDown(KeyCode.W))
                Move(Direction.UP);
            if(Input.GetKeyDown(KeyCode.A))
                Move(Direction.LEFT);
            if (Input.GetKeyDown(KeyCode.S))
                Move(Direction.DOWN);
            if (Input.GetKeyDown(KeyCode.D))
                Move(Direction.RIGHT);
            //이동 입력 인식
        }
        //캐릭터 입력


        public void Combat(Enemy enemy)
        {
            if (this.hp > enemy.GetHP())
            {
                this.hp -= enemy.GetHP();
                enemy.Delete();
            }
            else
                Die();
        }

        public void GetCoin(Coin c)
        {
            this.coin += c.GetValue();
            c.Delete();
        }

        public void GetHeart(Heart h)
        {
            this.hp++;
            h.Delete();
        }
        //상호작용 기능 3종


        public void Move(Direction d)
        {
            string obj = "";
            switch (d)
            {   
                case Direction.UP:
                    obj = CheckTile(x, y + 1);
                    if (obj == "invalid") return;
                    else if (obj == "enemy") Combat((Enemy)BoardManager.manager.FindEntity(x, y + 1));
                    else if (obj == "coin") GetCoin((Coin)BoardManager.manager.FindEntity(x, y + 1));
                    else if (obj == "heart") GetHeart((Heart)BoardManager.manager.FindEntity(x, y + 1));
                    else if (obj == "door") { BoardManager.manager.NextLevel(); return; }
                    y += 1;
                    break;
                case Direction.DOWN:
                    obj = CheckTile(x, y - 1);
                    if (obj == "invalid") return;
                    else if (obj == "enemy") Combat((Enemy)BoardManager.manager.FindEntity(x, y - 1));
                    else if (obj == "coin") GetCoin((Coin)BoardManager.manager.FindEntity(x, y - 1));
                    else if (obj == "heart") GetHeart((Heart)BoardManager.manager.FindEntity(x, y - 1));
                    else if (obj == "door") { BoardManager.manager.NextLevel(); return; }
                    y -= 1;
                    break;
                case Direction.LEFT:
                    obj = CheckTile(x - 1, y);
                    if (obj == "invalid") return;
                    else if (obj == "enemy") Combat((Enemy)BoardManager.manager.FindEntity(x - 1, y));
                    else if (obj == "coin") GetCoin((Coin)BoardManager.manager.FindEntity(x - 1, y));
                    else if (obj == "heart") GetHeart((Heart)BoardManager.manager.FindEntity(x - 1, y));
                    else if (obj == "door") { BoardManager.manager.NextLevel(); return; }
                    x -= 1;
                    break;
                case Direction.RIGHT:
                    obj = CheckTile(x + 1, y);
                    if (obj == "invalid") return;
                    else if (obj == "enemy") Combat((Enemy)BoardManager.manager.FindEntity(x + 1, y));
                    else if (obj == "coin") GetCoin((Coin)BoardManager.manager.FindEntity(x + 1, y));
                    else if (obj == "heart") GetHeart((Heart)BoardManager.manager.FindEntity(x + 1, y));
                    else if (obj == "door") { BoardManager.manager.NextLevel(); return; }
                    x += 1;
                    break;
                default:
                    break;
            }
            this.transform.position = new Vector2(x, y);
            CheckObjects(); 
            //Debug.Log(x + ", " + y);
        }

        public string CheckTile(int x, int y)
        {
            if (x < 0 || y < 0 || x >= BoardManager.manager.board.GetLength(0) || y >= BoardManager.manager.board.GetLength(1))
                return "invalid";
            switch (BoardManager.manager.board[x, y, 1])
            {
                case Object.EMPTY:
                    return "empty";
                case Object.ENEMY:
                    return "enemy";
                case Object.COIN:
                    return "coin";
                case Object.HEART:
                    return "heart";
                case Object.DOOR:
                    return "door";
                default:
                    return "null";
            }
        }
        //이동 관련 기능 2종

        public void CheckObjects()
        {
            string[] obj = new string[8];
            nearbyEnemies = false;
            nearbyObjects = 0;

            obj[0] = CheckTile(x - 1, y - 1);
            obj[1] = CheckTile(x - 1, y);
            obj[2] = CheckTile(x - 1, y + 1);
            obj[3] = CheckTile(x, y - 1);
            obj[4] = CheckTile(x, y + 1);
            obj[5] = CheckTile(x + 1, y - 1);
            obj[6] = CheckTile(x + 1, y);
            obj[7] = CheckTile(x + 1, y + 1);
            
            foreach(string s in obj)
            {
                if (s == "enemy")
                {
                    nearbyEnemies = true;
                    nearbyObjects++;
                }
                else if (s == "coin" || s == "heart")
                    nearbyObjects++;
            }
            Destroy(numberObject);
            numberObject = Instantiate(BoardManager.manager.Numbers[nearbyObjects])as GameObject;
            numberObject.transform.parent = transform;
            numberObject.transform.position = new Vector2(this.x, this.y + 1);
            if(nearbyEnemies)
                numberObject.GetComponent<SpriteRenderer>().color = Color.red;

        }

        public void Die()
        {
            BoardManager.manager.gameOver = true;
            Destroy(this.gameObject);
        }

    }
}