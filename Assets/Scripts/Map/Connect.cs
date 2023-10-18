using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** 接続のクラス */
public class Connect {
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
    public bool isConnected = false;
    //  directionは↓が0、→が1、↑が2、←が3。
    //  middleだけ進んだところで折れる

    /** 接続を追加する
	*
    * @param	startRealm	始点の領域
	* @param	endRealm	終点の領域
	*
	**/
    public void SetRealm(Realm startRealm, Realm endRealm) {
        
        this.fromId = startRealm.id;
        this.toId = endRealm.id;

        //  接続元・接続先を記録
        //directionは↓が0、→が1、↑が2、←が3。
        //0：下、1:右、2:上、3：左

        //左に存在する
        if (startRealm.left == endRealm.right + 1) {
            this.startX = startRealm.RoomLeft - 1;// - 1?
            this.startY = startRealm.getRandomPointY();
            this.endX = endRealm.RoomRight+ 1;
            this.endY = endRealm.getRandomPointY();
            this.middle = startRealm.RoomLeft - startRealm.left - 1;
            this.direction = 3;
            return;
        }
		//右に存在する
        if (endRealm.left == startRealm.right + 1) {

            this.startX = startRealm.RoomRight + 1;//+1？
            this.startY = startRealm.getRandomPointY();
            this.endX = endRealm.RoomLeft - 1;
            this.endY = endRealm.getRandomPointY();
            this.middle = startRealm.right - startRealm.RoomRight;
            this.direction = 1;
            return;
        }
		//下に存在する
        if (startRealm.top == endRealm.bottom + 1) {
            this.startX = startRealm.getRandomPointX();
            this.startY = startRealm.RoomTop - 1;
            this.endX = endRealm.getRandomPointX();
            this.endY = endRealm.RoomBottom + 1;
            this.middle = startRealm.RoomTop - startRealm.top;
            this.direction = 0;
            return;
        }
		//上に存在する
        if (endRealm.top == startRealm.bottom + 1) {

            this.startX = startRealm.getRandomPointX();
            this.startY = startRealm.RoomBottom + 1;// + 1?
            this.endX = endRealm.getRandomPointX();
            this.endY = endRealm.RoomTop - 1;
            this.middle = startRealm.bottom - startRealm.RoomBottom - 1;
            this.direction = 2;
            return;
        }   
    }  
};
