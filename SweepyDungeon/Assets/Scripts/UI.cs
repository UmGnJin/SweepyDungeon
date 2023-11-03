using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SweepyDungeon
{
    public class UI : MonoBehaviour
    {
        public Text stat;
        public Text gameOver;

        void Update()
        {
            stat.text = "HP : " + BoardManager.manager.player.hp + "\nCOIN : " + BoardManager.manager.player.coin;

            if (BoardManager.manager.gameOver)
                gameOver.gameObject.SetActive(true);
            else
                gameOver.gameObject.SetActive(false);
        }
    }
}