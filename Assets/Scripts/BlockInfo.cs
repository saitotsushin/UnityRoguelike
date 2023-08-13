using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockInfo : MonoBehaviour
{
    public int X = 0;
    public int Y = 0;
    public void SetPos(int _X,int _Y){
        X = _X;
        Y = _Y;
    }
}
