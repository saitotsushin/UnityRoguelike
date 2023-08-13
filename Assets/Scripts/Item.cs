using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public Vector2Int Pos = new Vector2Int(0, 0);
    public int roomId = 0;
    public bool IsFired = false;
    public string ItemName;

    public void Fire(){
        IsFired = true;
        gameObject.SetActive(false);
        Debug.Log(ItemName + "を拾いました");
    }
}
