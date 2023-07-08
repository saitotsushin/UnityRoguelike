using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map_v2 : MonoBehaviour
{
    int mapSizeX = 40;
    int mapSizeY = 30;
    private GameObject floorPrefab;
	private GameObject wallPrefab;
    // public int[] realms;
    public int[,] map;
    public int setId = 0;
    public List<RectBox> realms = new List<RectBox>();
    private int minRoomSizeX = 10;
    private int minRoomSizeY = 10;
    private float testRealmsCount = 0;//後で消す
    private int minArea = 10;
    private int minWidth = 5;
    private int minHeight = 5;
    // Start is called before the first frame update
    void Start()
    {
        map = new int[mapSizeX, mapSizeY];
        //初期値
        realms.Add(new RectBox(0, 0, mapSizeX, mapSizeY));
        floorPrefab = Resources.Load("Prefabs/Floor") as GameObject;
        wallPrefab = Resources.Load("Prefabs/Wall") as GameObject;

        GenerateMap();
    }
    private void GenerateMap()
    {
        divideRoom();
    }
    public void divideRoom(){
        separateLargestRealm();
        RectBox firstRealm = new RectBox();
        RectBox secondRealm = new RectBox();
        firstRealm = realms[realms.Count - 2];
        secondRealm = realms[realms.Count - 1];

        GameObject _obj;
        SpriteRenderer spriteRenderer;
        // 新しいHSV値を設定する
        float hue = 0f;        // 色相（0.0 - 1.0）
        float saturation = 0.8f; // 彩度（0.0 - 1.0）
        float value = 0.5f;      // 明度（0.0 - 1.0）
        testRealmsCount += 0.1f;
        hue = testRealmsCount;
        int test_creat_count = 0;
        string test_a1 = "";
        string test_a2 = "";
        for (int y = firstRealm.top; y < firstRealm.top + firstRealm.sizeY; y++){
            for (int x = firstRealm.left; x < firstRealm.left + firstRealm.sizeX; x++){
                _obj = Instantiate(wallPrefab, new Vector3(x, y, 0), new Quaternion());      
                spriteRenderer = _obj.GetComponent<SpriteRenderer>();              
                // 新しい色を作成する
                Color newColor = Color.HSVToRGB(hue, saturation, value);
                spriteRenderer.color = newColor; // 好みの色に変更する
                test_creat_count++;
                test_a1 += "1,";
            }         
            test_a1 += "\n";       
        }
        testRealmsCount += 0.1f;
        hue = testRealmsCount;
        for (int y = secondRealm.top; y < secondRealm.top + secondRealm.sizeY; y++){
            for (int x = secondRealm.left; x < secondRealm.left + secondRealm.sizeX; x++){
                _obj = Instantiate(wallPrefab, new Vector3(x, y, 0), new Quaternion());      
                spriteRenderer = _obj.GetComponent<SpriteRenderer>();
                // 新しい色を作成する
                Color newColor = Color.HSVToRGB(hue, saturation, value);
                spriteRenderer.color = newColor; // 好みの色に変更する   
                test_creat_count++;
                test_a2 += "2,";
            }   
            test_a2 += "\n";             
        }
        DebugRealms(realms);
        Debug.Log("分割数=" + realms.Count);    
    }
    private List<RectBox> RemoveRealms(List<RectBox> _realms, int _removeId = 0){
        List<RectBox> new_realms = new List<RectBox>();
        foreach(RectBox _rect in _realms){
            if(_rect.id != _removeId){
                new_realms.Add(_rect);
            }
        }
        return new_realms;
    }
    private void DebugRealms(List<RectBox> _realms){
        int[,] checkArr = new int[mapSizeY, mapSizeX];
        for (int y = 0; y < checkArr.GetLength(0); y++)
        {
            for (int x = 0; x < checkArr.GetLength(1); x++){
                checkArr[y, x] = 0;
            }
        }
        foreach(RectBox r in _realms){
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
    private void separateLargestRealm(){
        int index = 0;

        index = getIndexOfLargestRealm();
        

        // separateRealmByX(index);

        
        if (index >= 0) {
         
            RectBox r = getRealmFromId(realms,index);
            Debug.Log("r.sizeX=" + r.sizeX + "/r.sizeY=" + r.sizeY);
            if (r.sizeX >= r.sizeY) {
            
                separateRealmByX(index);
               
            } else {
               
                separateRealmByY(index);
                
            }
            
        }
    }
    private bool separateRealmByX(int id) {
        int minWidthByArea = 0;
        int separateX;

        RectBox r = getRealmFromId(realms,id);

        if (r.sizeY * (int)Math.Floor((double)r.sizeX / 2) < minArea) {
            Debug.Log("分割できる領域が"+minArea+"未満です。サイズ="+r.sizeY * (int)Math.Floor((double)r.sizeX / 2));
            return false;
        }
        if((int)Math.Floor((double)r.sizeX / 2) <= minWidth){
            Debug.Log("sizeXが小さすぎます。sizeX="+r.sizeX);
            return false;
        }

        minWidthByArea = (int)Math.Floor((double)minArea / r.sizeY);
        separateX = 0;

        int random_min = 0;
        int random_max = 0;
        int randomNumber = 0;

        if (minWidthByArea > minWidth) {
            random_max = r.sizeX - minWidthByArea * 2;
            System.Random random = new System.Random();
            randomNumber = random.Next(random_min, random_max + 1);
            separateX = minWidthByArea + randomNumber;            
        } else {
            random_max = r.sizeX - minWidth * 2;
            System.Random random = new System.Random();
            randomNumber = random.Next(random_min, random_max + 1);
            separateX = minWidth + randomNumber;
        }
        realms = RemoveRealms(realms,id);
        //	分割位置を決定
        RectBox firstRoom = new RectBox(r.left, r.top, separateX, r.sizeY);
        setId++;
        firstRoom.id = setId;
        RectBox secondRoom = new RectBox(r.left + separateX,r.top,r.sizeX - separateX, r.sizeY);
        setId++;
        secondRoom.id = setId;
        realms.Add(firstRoom);
        realms.Add(secondRoom);

        return true;
		
    }
    private bool separateRealmByY(int id) {
        int minHeightByArea = 0;
        int separateY;

        RectBox r = getRealmFromId(realms,id);

        if (r.sizeY * (int)Math.Floor((double)r.sizeX / 2) < minArea) {
            Debug.Log("分割できる領域が"+minArea+"未満です。サイズ="+r.sizeY * (int)Math.Floor((double)r.sizeX / 2));
            return false;
        }
        if((int)Math.Floor((double)r.sizeX / 2) <= minHeight){
            Debug.Log("sizeYが小さすぎます。sizeY="+r.sizeY);
            return false;
        }

        minHeightByArea = (int)Math.Floor((double)minArea / r.sizeX);
        separateY = 0;

        int random_min = 0;
        int random_max = 0;
        int randomNumber = 0;

        if (minHeightByArea > minHeight) {
            random_max = r.sizeY - minHeightByArea * 2;
            System.Random random = new System.Random();
            randomNumber = random.Next(random_min, random_max + 1);
            separateY = minHeightByArea + randomNumber;            
        } else {
            random_max = r.sizeY - minHeight * 2;
            System.Random random = new System.Random();
            randomNumber = random.Next(random_min, random_max + 1);
            separateY = minHeight + randomNumber;
        }
        realms = RemoveRealms(realms,id);
        //	分割位置を決定
        RectBox firstRoom = new RectBox(r.left, r.top, r.sizeX, separateY);
        setId++;
        firstRoom.id = setId;
        RectBox secondRoom = new RectBox(r.left,r.top + separateY, r.sizeX, r.sizeY - separateY);
        setId++;
        secondRoom.id = setId;
        realms.Add(firstRoom);
        realms.Add(secondRoom);

        return true;
		
    }
    /** 最大面積の領域のインデックスを取得. */
    private int getIndexOfLargestRealm() {

        int i;
        int maxIndex,
            area,
            maxArea;

        RectBox r;

        maxIndex = 0;
        area = 0;
        maxArea = 0;

        if (realms.Count == 0) { return 0; }
        
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
    private RectBox getRealmFromId(List<RectBox> _realms, int _id)
    {
        // int GetId = 0;
        RectBox _RectBox = null;
        for (int i = 0; i < _realms.Count; i += 1)
        {
            if(_realms[i].id == _id){
                _RectBox = _realms[i];
            }
        }
        return _RectBox;
    }

}
public class RectBox
{

    public int sizeX= 0;
    public int sizeY= 0;
    public int left = 0;
    public int top = 0;
    public int id = 0;

    // public int R_Y = 0;
    public RectBox(int _left, int _top, int _sizeX,int _sizeY)
    {
        this.left = _left;
        this.top = _top;
        this.sizeX = _sizeX;
        this.sizeY = _sizeY;
        
    }
    //コンストラクタが呼び出し初期化
    public RectBox() : this(0, 0, 0, 0) {}
}