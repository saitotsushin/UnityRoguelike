using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map_v2 : MonoBehaviour
{
    public Transform parentFloor;
    public Transform parentRealms;
    int mapSizeX = 40;
    int mapSizeY = 30;
    private GameObject floorPrefab;
	private GameObject wallPrefab;
    // public int[] realms;
    public int[,] map;
    public int setId = 0;
    public List<Realm> realms = new List<Realm>();
    // private int minRoomSizeX = 10;
    // private int minRoomSizeY = 10;
    private int testRealmsCount = 0;//後で消す
    private int minArea = 50;
    private int minWidth = 8;
    private int maxWidth = 0;
    private int minHeight = 8;
    private int maxHeight = 0;
    private int margin = 1;
    private int minRatio = 40;//分割した区画の何%を最小の部屋にするか
    private int roomMinusRange = 3;//
    // Start is called before the first frame update
    private string[] colorArray = new string[]
        {
            "#B73500",
            "#FF6464",
            "#FF0055",
            "#FF00B8",
            "#C200FF",
            "#5E00FF",
            "#001EFF",
            "#0088FF",
            "#00F8FF",
            "#00FF8E",
            "#C6FF00"
        }
    ;
    void Start()
    {
        map = new int[mapSizeX, mapSizeY];
        maxWidth = (int)mapSizeX / 3 * 2;
        maxHeight = (int)mapSizeY / 3 * 2;
        //初期値
        realms.Add(new Realm(0, 0, mapSizeX, mapSizeY));
        floorPrefab = Resources.Load("Prefabs/Floor") as GameObject;
        wallPrefab = Resources.Load("Prefabs/Wall") as GameObject;

        GenerateMap();
    }
    public void GenerateMap()
    {
        divideRoom();
        MakeRoom();
        MakeAdjacent();
        // CreatRoom();
    }
    public void divideRoom(){
        separateLargestRealm();
        Realm firstRealm = new Realm();
        Realm secondRealm = new Realm();
        firstRealm = realms[realms.Count - 2];
        secondRealm = realms[realms.Count - 1];

        GameObject _obj;
        SpriteRenderer spriteRenderer;
        // 新しいHSV値を設定する
        // float hue = 0f;        // 色相（0.0 - 1.0）
        // float saturation = 0.8f; // 彩度（0.0 - 1.0）
        // float value = 0.5f;      // 明度（0.0 - 1.0）
        testRealmsCount++;
        // hue = testRealmsCount;
        int test_creat_count = 0;
        string test_a1 = "";
        string test_a2 = "";
        for (int y = firstRealm.top; y < firstRealm.top + firstRealm.sizeY; y++){
            for (int x = firstRealm.left; x < firstRealm.left + firstRealm.sizeX; x++){
                _obj = Instantiate(wallPrefab, new Vector3(x, y, 0), new Quaternion(),parentRealms);      
                spriteRenderer = _obj.GetComponent<SpriteRenderer>();              
                // 新しい色を作成する
                Color newColor = ColorUtility.TryParseHtmlString(colorArray[testRealmsCount], out Color result) ? result : Color.white;
                spriteRenderer.color = newColor; // 好みの色に変更する
                test_creat_count++;
                test_a1 += "1,";
            }         
            test_a1 += "\n";       
        }
        testRealmsCount++;
        // hue = testRealmsCount;
        for (int y = secondRealm.top; y < secondRealm.top + secondRealm.sizeY; y++){
            for (int x = secondRealm.left; x < secondRealm.left + secondRealm.sizeX; x++){
                _obj = Instantiate(wallPrefab, new Vector3(x, y, 0), new Quaternion(),parentRealms);      
                spriteRenderer = _obj.GetComponent<SpriteRenderer>();
                // 新しい色を作成する
                Color newColor = ColorUtility.TryParseHtmlString(colorArray[testRealmsCount], out Color result) ? result : Color.white;
                spriteRenderer.color = newColor; // 好みの色に変更する   
                test_creat_count++;
                test_a2 += "2,";
            }   
            test_a2 += "\n";             
        }
        DebugRealms(realms);
    }
    private List<Realm> RemoveRealms(List<Realm> _realms, int _removeId = 0){
        List<Realm> new_realms = new List<Realm>();
        foreach(Realm _rect in _realms){
            if(_rect.id != _removeId){
                new_realms.Add(_rect);
            }
        }
        return new_realms;
    }
    private void DebugRealms(List<Realm> _realms){
        int[,] checkArr = new int[mapSizeY, mapSizeX];
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
    private void separateLargestRealm(){
        int index = 0;

        index = getIndexOfLargestRealm();
        

        // separateRealmByX(index);

        
        if (index >= 0) {
         
            Realm r = getRealmFromId(realms,index);
            if (r.sizeX >= r.sizeY) {
            
                separateRealmByX(index);
               
            } else {
               
                separateRealmByY(index);
                
            }
            
        }else{
            Debug.LogError("分割できませんでした");
        }
    }
    private bool separateRealmByX(int id) {
        int minWidthByArea = 0;
        int separateX;

        Realm r = getRealmFromId(realms,id);

        if (r.sizeY * (int)Math.Floor((double)r.sizeX / 2) < minArea) {
            Debug.LogError("分割できる領域が"+minArea+"未満です。サイズ="+r.sizeY * (int)Math.Floor((double)r.sizeX / 2));
            return false;
        }
        if((int)Math.Floor((double)r.sizeX / 2) <= minWidth){
            Debug.LogError("sizeXが小さすぎます。sizeX="+r.sizeX);
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
        Realm firstRoom = new Realm(r.left, r.top, separateX, r.sizeY);
        setId++;
        firstRoom.id = setId;
        Realm secondRoom = new Realm(r.left + separateX,r.top,r.sizeX - separateX, r.sizeY);
        setId++;
        secondRoom.id = setId;
        realms.Add(firstRoom);
        realms.Add(secondRoom);

        return true;
		
    }
    private bool separateRealmByY(int id) {
        int minHeightByArea = 0;
        int separateY;

        Realm r = getRealmFromId(realms,id);

        if (r.sizeY * (int)Math.Floor((double)r.sizeY / 2) < minArea) {
            Debug.LogError("分割できる領域が"+minArea+"未満です。サイズ="+r.sizeY * (int)Math.Floor((double)r.sizeX / 2));
            return false;
        }
        if((int)Math.Floor((double)r.sizeY / 2) <= minHeight){
            Debug.LogError("sizeYが小さすぎます。sizeY="+r.sizeY);
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
        Realm firstRoom = new Realm(r.left, r.top, r.sizeX, separateY);
        setId++;
        firstRoom.id = setId;
        Realm secondRoom = new Realm(r.left,r.top + separateY, r.sizeX, r.sizeY - separateY);
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
    private Realm getRealmFromId(List<Realm> _realms, int _id)
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
    public void MakeRoom(){
        for (int i = 0; i < realms.Count; i++){
            Realm r = realms[i];
            if (r.sizeX * r.sizeY < minArea)
            {
                r.setRoom(r.left + margin, r.top + margin, r.sizeX - margin * 2, r.sizeY - margin * 2);
            }else{

                System.Random random = new System.Random();
                int max_room_size = r.sizeX * r.sizeY;

                int rWidth = 0;
                int _minusWidth = (int)(random.Next(0, roomMinusRange + 1));
                rWidth = r.sizeX - _minusWidth;
                if(rWidth <= minWidth){
                    rWidth = minWidth - margin * 2;
                }else{
                    if(rWidth >= maxWidth){
                        rWidth = maxWidth - margin * 2;
                    }else{
                        rWidth = rWidth - margin * 2;
                    }
                }

                int _minusHeight = (int)(random.Next(0, roomMinusRange + 1));
                int rHeight = r.sizeY - _minusHeight;
                if(rHeight <= minHeight){
                    rHeight = minHeight - margin * 2;
                }else{
                    if (rHeight >= maxHeight)
                    {
                        rHeight = maxHeight - margin * 2;
                    }else{
                        rHeight = rHeight - margin * 2;
                    }
                }

                System.Random randomLeft = new System.Random();
                int StartLeft = (int)(randomLeft.Next(0, r.sizeX - margin * 2 - rWidth + 1)) + r.left + margin;

                System.Random randomTop = new System.Random();

                int StartTop = (int)(randomTop.Next(0, r.sizeY - margin * 2- rHeight + 1)) + r.top + margin;
                r.setRoom(StartLeft, StartTop, StartLeft + rWidth, StartTop + rHeight);

            }
        }
        CreatRoom();
    }
    public void MakeAdjacent(){
        for (int i = 0; i < realms.Count; i++)
        {
            Realm r = realms[i];
            r.adjacentIDs = getAdjacentRealmIdList(r.id);
        }
    }
    private void CreatRoom(){
        //一旦子要素は全て削除
        for (int i = parentFloor.childCount - 1; i >= 0; i--)
        {
            Transform child = parentFloor.GetChild(i);
            Destroy(child.gameObject);
        }   
        foreach(Realm _r in realms){
            for (int y = _r.RoomTop; y < _r.RoomBottom; y++){
                for (int x = _r.RoomLeft; x < _r.RoomRight; x++)
                {
                    GameObject _obj = Instantiate(wallPrefab, new Vector3(x, y, 0), new Quaternion(),parentFloor);
                }
            }
        }
        // spriteRenderer = _obj.GetComponent<SpriteRenderer>();              
        // // 新しい色を作成する
        // Color newColor = Color.HSVToRGB(hue, saturation, value);
        // spriteRenderer.color = newColor; // 好みの色に変更する
        // test_creat_count++;
        // test_a1 += "1,";       
    }
    /** その領域に接している他の領域のIDを全て取得する. 
    * 
    * @param	index	取得したい領域のインデックス
    * @return		    接している領域のIDを含む配列
    * 
    **/
    private List<int> getAdjacentRealmIdList(int index) {
        Debug.Log("getAdjacentRealmIdList");
        int i;        
        List<int> result = new List<int>();
        List<int> touchingRealms = getAdjacentRealmIndices(index);
        for (i = 0; i < touchingRealms.Count; i += 1) {

            result.Add(realms[touchingRealms[i]].id);
            Debug.Log("index="+index+"/id="+realms[touchingRealms[i]].id);

        }
        
        return result;

    }
    /** その領域に接している他の領域のインデックスを全て取得する. 
    * 
    * @param	index	接しているかどうかを調べたい領域のインデックス
    * @return			接している領域のインデックスを含む配列
    * 
    **/
    private List<int> getAdjacentRealmIndices(int index) {

        int i = 0;
        List<int> touchingRealms = new List<int>();
        Realm target;

        // touchingRealms = [];
        target = getRealmFromId(realms,index);

        for (i = 0; i < realms.Count; i += 1) {

            if (index != i) {

                if (isAdjacent(target, realms[i])) {
                    touchingRealms.Add(i);
                }

            }

        }

        return touchingRealms;

    }
     /** 領域Aと領域Bが接しているかどうかを調べる.
     * 
     * @param   realmA	領域A
     * @param   realmB	領域B
     * @return          領域A、Bが接しているか否かを示す真偽値
     * 
     **/
    private bool isAdjacent (Realm realmA, Realm realmB) {
        
        if ((realmA.left == realmB.right + 1) || (realmB.left == realmA.right + 1)) {

            if ((realmA.top <= realmB.bottom) && (realmA.top >= realmB.top)) {      return true; }
            if ((realmA.bottom >= realmB.top) && (realmA.bottom <= realmB.bottom)) {return true; }

        }

        if ((realmA.top == realmB.bottom + 1) || (realmB.top == realmA.bottom + 1)) {

            if ((realmA.left >= realmB.left) && (realmA.left <= realmB.right)) {    return true; }
            if ((realmA.right >= realmB.left) && (realmA.right <= realmB.right)) {	return true; }

        }

        return false;

    }
}
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
        this.RoomRight = _width;//-1にする？
        this.RoomBottom = _height;//-1にする？
    }
}