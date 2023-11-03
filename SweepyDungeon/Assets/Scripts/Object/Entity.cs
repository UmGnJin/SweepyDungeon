using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SweepyDungeon
{
    public class Entity : MonoBehaviour
    {
        public int x;
        public int y;

        public void Init(int x, int y)
        {
            this.x = x; 
            this.y = y; 
        }

        public void Delete()
        {
            BoardManager.manager.board[x, y, 1] = Object.EMPTY;
            BoardManager.manager.entities[x, y] = null;
            Destroy(this.gameObject);
        }
    }
}