using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorInfo : MonoBehaviour
{
    public int id = 0;
    public int top = 0;
    public int left = 0;
    public int right = 0;
    public int bottom = 0;
    public int width = 0;
    public int height = 0;
    public void SetStatus(
        int _id,
        int _top,
        int _left,
        int _right,
        int _bottom,
        int _width,
        int _height
    ){
        id = _id;
        top = _top;
        left = _left;
        right = _right;
        bottom = _bottom;
        width = _width;
        height = _height;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
