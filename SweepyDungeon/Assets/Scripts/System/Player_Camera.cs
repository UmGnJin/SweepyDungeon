using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SweepyDungeon
{
    public class Player_Camera : MonoBehaviour
    {
        public GameObject Player;
        private void Start()
        {
            
        }
        void Update()
        {
            if (Player == null)
                Player = GameObject.FindWithTag("Player");
            else
                this.transform.position = new Vector3(Player.transform.position.x, Player.transform.position.y, -10);
        }
    }
}