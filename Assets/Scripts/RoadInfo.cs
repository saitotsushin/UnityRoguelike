using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadInfo : MonoBehaviour
{
    public int id = 0;
    public int fromId = 0;
    public int toId = 0;
    public int startX = 0;
    public int startY = 0;
    public int middle = 0;
    // public int dir = 0;
    public int endX = 0;
    public int endY = 0;
    public int direction = 0;
    public void SetStatus(connect _c){
        this.id     = _c.id;
        this.fromId = _c.fromId;
        this.toId   = _c.toId;
        this.startX = _c.startX;
        this.startY = _c.startY;
        this.middle = _c.middle;
        this.endX   = _c.endX;
        this.endY   = _c.endY;
        this.direction = _c.direction;
    }
}
