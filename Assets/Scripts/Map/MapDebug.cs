using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDebug : MonoBehaviour
{
    public static MapDebug instance; // インスタンスの定義
    void Awake()
    {
        // シングルトンの呪文
        if (instance == null)
        {
            // 自身をインスタンスとする
            instance = this;
        }
        else
        {
            // インスタンスが複数存在しないように、既に存在していたら自身を消去する
            Destroy(gameObject);
        }
    }

    public void DebugMap(int[,] _map)
    {
        string logs = "";
        for (int y = 0; y < _map.GetLength(0); y++)
        {
            for (int x = 0; x < _map.GetLength(1); x++)
            {
                logs += _map[y, x]+",";
            }
            logs += "\n";
        }
        Debug.Log(logs);
    }
    public void DebugRealms(List<Realm> _realms,int _mapSizeX,int _mapSizeY){
        int[,] checkArr = new int[_mapSizeY, _mapSizeX];
        for (int y = 0; y < checkArr.GetLength(0); y++)
        {
            for (int x = 0; x < checkArr.GetLength(1); x++){
                checkArr[y, x] = 0;
            }
        }
        foreach(Realm r in _realms){
            for (int y = r.top; y < r.top + r.sizeY; y++){
                for (int x = r.left; x < r.left + r.sizeX; x++)
                {
                    checkArr[y, x] = r.id;
                }
            }
        }
        string logs = "";
        for (int y = 0; y < checkArr.GetLength(0); y++)
        {
            for (int x = 0; x < checkArr.GetLength(1); x++){
                logs += checkArr[y, x]+",";
            }
            logs += "\n";
        }
        Debug.Log(logs);
    }
}
