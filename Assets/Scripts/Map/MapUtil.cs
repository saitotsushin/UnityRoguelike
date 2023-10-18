using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapUtil
{
    /** 最大面積の領域のインデックスを取得. */
    public int getIndexOfLargestRealm(List<Realm> realms) {

        int i;
        int maxIndex,
            area,
            maxArea;

        Realm r;

        maxIndex = 0;
        area = 0;
        maxArea = 0;

        if (realms.Count == 0) { return -1; }
        
        for (i = 0; i < realms.Count; i += 1) {

            r = realms[i];
            area = r.sizeX * r.sizeY;

            if (area > maxArea) {

                maxArea = area;
                maxIndex = r.id;

            }

        }

        return maxIndex;

    }
    public Realm getRealmFromId(List<Realm> _realms, int _id)
    {
        // int GetId = 0;
        Realm _RectBox = null;
        for (int i = 0; i < _realms.Count; i += 1)
        {
            if(_realms[i].id == _id){
                _RectBox = _realms[i];
            }
        }
        return _RectBox;
    }
}
