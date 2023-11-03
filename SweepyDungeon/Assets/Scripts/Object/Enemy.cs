using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SweepyDungeon
{
    public class Enemy : Entity
    {
        int hp;

        public void SetHP(int hp)
        {
            this.hp = hp;   
        }

        public int GetHP()
        {
            return this.hp;
        }
    }
}
