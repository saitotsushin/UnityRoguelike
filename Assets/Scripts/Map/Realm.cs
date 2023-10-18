using System;
using System.Collections;
using System.Collections.Generic;
public class Realm
{

    public int sizeX= 0;
    public int sizeY= 0;
    public int left = 0;
    public int top = 0;
    public int right = 0;
    public int bottom = 0;
    public int id = 0;
    public int RoomLeft = 0;
    public int RoomRight = 0;
    public int RoomTop = 0;
    public int RoomBottom = 0;
    public int roomSizeX = 0;
    public int roomSizeY = 0;
    public List<int> connectID = new List<int>();//接続ID
    public List<int> adjacentIDs = new List<int>();//隣接ID

    public Realm(int _left, int _top, int _sizeX,int _sizeY)
    {
        this.left = _left;
        this.top = _top;
        this.sizeX = _sizeX;
        this.sizeY = _sizeY;
		this.right = _left + _sizeX - 1;
		this.bottom = _top + _sizeY - 1;        
        
    }
    //コンストラクタが呼び出し初期化
    public Realm() : this(0, 0, 0, 0) {}
    public void setRoom(int _left, int _top, int _width, int _height){
        this.RoomLeft = _left;
        this.RoomTop = _top;
        this.RoomRight = _left + _width - 1;//-1にする？
        this.RoomBottom = _top + _height - 1;//-1にする？
		this.roomSizeX = _width;
        this.roomSizeY = _height;       
    }
	/** 部屋の通路を開くため、右端・左端を除くY座標の範囲からランダムに１つの値を取得する.
	 *  
	 * @return	右端・左端を除くX座標の範囲から選ばれたランダムの値
	 *
	 **/
	public int getRandomPointX() {
        int left = this.RoomLeft + 1;
        int right = this.RoomLeft + this.roomSizeX - 2;
        return UnityEngine.Random.Range(left,right + 1);		
	}
	
	/** 部屋の通路を開くため、上端・下端を除くY座標の範囲からランダムに１つの値を取得する.
	 *  
	 * @return	上端・下端を除くY座標の範囲から選ばれたランダムの値
	 *
	 **/
	public int getRandomPointY() {
        int top = this.RoomTop + 1;
        int bottom = this.RoomTop + this.roomSizeY - 2;
        return UnityEngine.Random.Range(top, bottom + 1);
    }
}