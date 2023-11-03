using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SweepyDungeon
{
    public class Coin : Entity
    {
        int value;

        public void SetValue(int v)
        {
            this.value = v;
        }

        public int GetValue()
        {
            return this.value;
        }

    }
}