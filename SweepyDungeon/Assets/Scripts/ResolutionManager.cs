using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArcanaDungeon
{
    public class ResolutionManager : MonoBehaviour
    {
        

        // Update is called once per frame
        void Awake()
        {
            Screen.SetResolution(1920, 1080, true);
        }
    }
}