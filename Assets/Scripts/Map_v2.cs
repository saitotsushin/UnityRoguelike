using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map_v2 : MonoBehaviour
{
    public Transform parentFloor;
    public Transform parentRealms;
    public Transform parentRoad;
    public Transform parentFoolorInfo;
    int mapSizeX = 40;
    int mapSizeY = 30;
    private GameObject floorPrefab;
	private GameObject wallPrefab;
    private GameObject roadPrefab;
    private GameObject floorInfoPrefab;
    private GameObject roadAreaPrefab;
    // public int[] realms;
    public int[,] map;
    public int setId = 0;
    public List<Realm> realms = new List<Realm>();
    public List<connect> connects = new List<connect>();
    // private int minRoomSizeX = 10;
    // private int minRoomSizeY = 10;
    private int testRealmsCount = 0;//後で消す
    private int minArea = 40;
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
        roadPrefab = Resources.Load("Prefabs/Road") as GameObject;
        floorInfoPrefab = Resources.Load("Prefabs/FloorInfo") as GameObject;
        roadAreaPrefab = Resources.Load("Prefabs/RoadArea") as GameObject;

        GenerateMap();
        
    }
    public void GenerateMap()
    {
        divideRoom();
        
        // MakeAdjacent();
        
        // CreatRoom();
    }
    /** 接続を追加する
	*
    * @param	startRealm	始点の領域
	* @param	endRealm	終点の領域
	*
	**/
    public void addConnect (Realm fromRealm, Realm toRealm) {
        connect _connect = new connect();
        connects.Add(_connect);
        connects[connects.Count - 1].SetRealm(fromRealm, toRealm);
    }
    public void removeConnect(int _id){
        List<int> new_connectID = new List<int>();
        foreach(Realm _r in realms){
            for (int i = 0; i < _r.connectID.Count;i++){
                if(_r.connectID[i] != _id){
                    new_connectID.Add(_r.connectID[i]);
                }
            }
            _r.connectID = new_connectID;
        }
        List<connect> new_connects = new List<connect>();
        foreach(connect _c in connects){
            if(_c.fromId != _id && _c.toId != _id){
                new_connects.Add(_c);
            }
        }
        connects = new_connects;
    }
    public void MakeRoad(){
        //一旦子要素は全て削除
        for (int i = parentRoad.childCount - 1; i >= 0; i--)
        {
            Transform child = parentRoad.GetChild(i);
            Destroy(child.gameObject);
        }
        Debug.Log("connects.count=" + connects.Count);

        foreach (connect _c in connects)
        {
            drawConnect(_c);
        }

    }
    public void drawConnect (connect c) {
        GameObject _areaObj = Instantiate(roadAreaPrefab, new Vector3(0, 0, 0), new Quaternion(),parentRoad);
        Transform _area = _areaObj.transform;
        var i = 0;
        int dir = 0;
        int posX = 0;
        int posY = 0;
        int lim = 0;

        posX = c.startX;
        posY = c.startY;
        dir = c.direction;
        lim = c.middle;

        for (i = 0; i < lim; i += 1) {
            
            GameObject _obj = Instantiate(roadPrefab, new Vector3(posX, posY, 0), new Quaternion(),_area);
            _obj.GetComponent<BlockInfo>().SetPos(posX,posY);
            RoadInfo _m_obj = _areaObj.GetComponent<RoadInfo>();
            _m_obj.SetStatus(c);

            if (dir == 0) { posY -= 1; }
            if (dir == 1) { posX += 1; }
            if (dir == 2) { posY += 1; }
            if (dir == 3) { posX -= 1; }
        
        }
        
        if (dir % 2 == 0) {
            lim = Math.Abs(c.startX - c.endX) + 1;
            if (c.startX > c.endX) { dir = 3; }
            if (c.startX < c.endX) { dir = 1; }
        } else {
            lim = Math.Abs(c.startY - c.endY) + 1;
            if (c.startY > c.endY) { dir = 0; }
            if (c.startY < c.endY) { dir = 2; }
        }
        
        for (i = 0; i < lim; i += 1) {
            
            GameObject _obj = Instantiate(roadPrefab, new Vector3(posX, posY, 0), new Quaternion(),_area);
            _obj.GetComponent<BlockInfo>().SetPos(posX,posY);
            RoadInfo _m_obj = _areaObj.GetComponent<RoadInfo>();
            _m_obj.SetStatus(c);

            if (i != lim - 1) {
            
                if (dir == 0) { posY -= 1; }
                if (dir == 1) { posX += 1; }
                if (dir == 2) { posY += 1; }
                if (dir == 3) { posX -= 1; }

            }
        }
        
        dir = c.direction;
        if (dir % 2 == 0) {
            
            lim = Math.Abs(posY - c.endY);
            
        } else {
            
            lim = Math.Abs(posX - c.endX);
            
        }
        
        for (i = 0; i < lim; i += 1) {
            
            if (dir == 0) { posY -= 1; }
            if (dir == 1) { posX += 1; }
            if (dir == 2) { posY += 1; }
            if (dir == 3) { posX -= 1; }
            GameObject _obj = Instantiate(roadPrefab, new Vector3(posX, posY, 0), new Quaternion(),_area);
            _obj.GetComponent<BlockInfo>().SetPos(posX,posY);
            RoadInfo _m_obj = _areaObj.GetComponent<RoadInfo>();
            _m_obj.SetStatus(c);
        }
        
    }
    public void divideRoom(){
        separateLargestRealm();
        Realm firstRealm = new Realm();
        Realm secondRealm = new Realm();
        firstRealm = realms[realms.Count - 2];
        secondRealm = realms[realms.Count - 1];

        GameObject _obj;
        SpriteRenderer spriteRenderer;
        testRealmsCount++;
        int test_creat_count = 0;
        string test_a1 = "";
        string test_a2 = "";
        for (int y = firstRealm.top; y < firstRealm.top + firstRealm.sizeY; y++){
            for (int x = firstRealm.left; x < firstRealm.left + firstRealm.sizeX; x++){
                _obj = Instantiate(wallPrefab, new Vector3(x, y, 0), new Quaternion(),parentRealms);      
                _obj.GetComponent<BlockInfo>().SetPos(x,y);
                spriteRenderer = _obj.GetComponent<SpriteRenderer>();              
                // 新しい色を作成する
                Color newColor = ColorUtility.TryParseHtmlString(colorArray[testRealmsCount], out Color result) ? result : Color.white;
                spriteRenderer.color = newColor; // 好みの色に変更する
                test_creat_count++;
            }             
        }
        testRealmsCount++;
        // hue = testRealmsCount;
        for (int y = secondRealm.top; y < secondRealm.top + secondRealm.sizeY; y++){
            for (int x = secondRealm.left; x < secondRealm.left + secondRealm.sizeX; x++){
                _obj = Instantiate(wallPrefab, new Vector3(x, y, 0), new Quaternion(),parentRealms);      
                _obj.GetComponent<BlockInfo>().SetPos(x,y);
                spriteRenderer = _obj.GetComponent<SpriteRenderer>();
                // 新しい色を作成する
                Color newColor = ColorUtility.TryParseHtmlString(colorArray[testRealmsCount], out Color result) ? result : Color.white;
                spriteRenderer.color = newColor; // 好みの色に変更する   
                test_creat_count++;
            }           
        }
        MakeRoom();
        MakeAdjacent();
        
        foreach(Realm _r in realms){
            for (int i = 0; i < _r.adjacentIDs.Count;i++){
                int ToId = _r.adjacentIDs[i];
                addConnect(_r, getRealmFromId(realms,ToId));
            }
        }
        MakeRoad();
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
            randomNumber = UnityEngine.Random.Range(random_min, random_max + 1);
            separateX = minWidthByArea + randomNumber;            
        } else {
            random_max = r.sizeX - minWidth * 2;
            randomNumber = UnityEngine.Random.Range(random_min, random_max + 1);
            separateX = minWidth + randomNumber;
        }
        realms = RemoveRealms(realms,id);
        removeConnect(id);

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
            randomNumber = UnityEngine.Random.Range(random_min, random_max + 1);
            separateY = minHeightByArea + randomNumber;            
        } else {
            random_max = r.sizeY - minHeight * 2;
            randomNumber = UnityEngine.Random.Range(random_min, random_max + 1);
            separateY = minHeight + randomNumber;
        }
        realms = RemoveRealms(realms,id);
        removeConnect(id);
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
        //一旦子要素は全て削除
        for (int i = parentFoolorInfo.childCount - 1; i >= 0; i--)
        {
            Transform child = parentFoolorInfo.GetChild(i);
            Destroy(child.gameObject);
        }            
        int _SetLeft = 0;
        int _SetTop = 0;
        int _SetSizeX = 0;
        int _SetSizeY = 0;
        for (int i = 0; i < realms.Count; i++){
            Realm r = realms[i];
            if (r.sizeX * r.sizeY < minArea)
            {
                _SetLeft = r.left + 2;
                _SetTop = r.top + 2;
                _SetSizeX = r.sizeX - 4;
                _SetSizeY = r.sizeY - 4;                
                r.setRoom(_SetLeft, _SetTop, _SetSizeX, _SetSizeY);
                
            }else{

                int max_room_size = r.sizeX * r.sizeY;

                int rWidth = 0;
                int _minusWidth = (int)(UnityEngine.Random.Range(0, roomMinusRange + 1));
                rWidth = r.sizeX - _minusWidth - 4;
                if(rWidth <= minWidth){
                    rWidth = minWidth - 4;
                }

                int _minusHeight = (int)(UnityEngine.Random.Range(0, roomMinusRange + 1));
                int rHeight = r.sizeY - _minusHeight - 4;
                if(rHeight <= minHeight){
                    rHeight = minHeight - 4;
                }

                int StartLeft = (int)(UnityEngine.Random.Range(
                    r.left + 2,
                    r.left + 2 + ((r.sizeX - 4) - rWidth) + 1));

                int StartTop = (int)(UnityEngine.Random.Range(
                    r.top + 2,
                    r.top + 2 + ((r.sizeY - 4) - rHeight) + 1));

                _SetLeft = StartLeft;
                _SetTop = StartTop;
                _SetSizeX = rWidth;
                _SetSizeY = rHeight; 

                r.setRoom(StartLeft, StartTop, rWidth, rHeight);

            }
            GameObject _room_obj = Instantiate(floorInfoPrefab,
                new Vector3(
                    _SetLeft,
                    _SetTop,    
                    0
                ), new Quaternion(),parentFoolorInfo);
            // 幅を変更する新しい値を設定
            float newWidth = (float)_SetSizeX;
            float newHeight = (float)_SetSizeY;

            // 横幅を変更
            Vector3 newScale = _room_obj.transform.localScale;
            newScale.x = newWidth;
            newScale.y = newHeight;
            _room_obj.transform.localScale = newScale;
            _room_obj.transform.position = new Vector3(
                (float)(_SetLeft - 0.5) + newWidth / 2,
                (float)(_SetTop - 0.5) + newHeight / 2,
                0
            );
            FloorInfo _FloorInfo = _room_obj.GetComponent<FloorInfo>();
            _FloorInfo.SetStatus(
                r.id,
                r.RoomTop,
                r.RoomLeft,
                r.RoomRight,
                r.RoomBottom,
                r.roomSizeX,
                r.roomSizeY
            );

            // Transform childTransform = _room_obj.GetComponent<Transform>().transform.Find("FloorImage");

            // RectTransform rectTransform = childTransform.GetComponent<RectTransform>();
            // rectTransform.sizeDelta = new Vector2(newWidth, newHeight);

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
            for (int y = _r.RoomTop; y <= _r.RoomBottom; y++){
                for (int x = _r.RoomLeft; x <= _r.RoomRight; x++)
                {
                    GameObject _obj = Instantiate(wallPrefab, new Vector3(x, y, 0), new Quaternion(),parentFloor);
                    _obj.GetComponent<BlockInfo>().SetPos(x,y);
                }
            }
        } 
    }
    /** その領域に接している他の領域のIDを全て取得する. 
    * 
    * @param	index	取得したい領域のインデックス
    * @return		    接している領域のIDを含む配列
    * 
    **/
    private List<int> getAdjacentRealmIdList(int index) {
        int i;        
        List<int> result = new List<int>();
        List<int> touchingRealms = getAdjacentRealmIndices(index);
        for (i = 0; i < touchingRealms.Count; i += 1) {

            result.Add(realms[touchingRealms[i]].id);
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
                if (isAdjacent(target, realms[i])) {
                    touchingRealms.Add(i);
                }

            if (index != i) {


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
    /** その領域に接している他の領域から、ランダムで一つ選択し、その領域のインデックスを返す.
    * 
    * @param	index	領域のインデックス
    * @return			その領域に接している他の領域（複数ある場合にはランダムで１つ）のインデックス
    * 
    **/
    // public int randomChooseAdjacentRealmIndex(int index) {

    //     int r = 0;
    //     List<int> touchingRealms = getAdjacentRealmIndices(index);

    //     // touchingRealms = getAdjacentRealmIndices(index);
    //     r = random.Next(0, touchingRealms.Count + 1);        
    //     // r = Math.floor(xorshift.random() * touchingRealms.length);

    //     return touchingRealms[r];

    // }
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
/** 接続のクラス */
public class connect {

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
    //  directionは↓が0、→が1、↑が2、←が3。
    //  middleだけ進んだところで折れる
    
    /** 接続を追加する
	*
    * @param	startRealm	始点の領域
	* @param	endRealm	終点の領域
	*
	**/
    public void SetRealm(Realm startRealm, Realm endRealm) {

        int startX = 0;
        int startY = 0;
        int endX = 0;
        int endY = 0;
        int dir = 0;
        int middle = 0;
        
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
        
        // this.startX = startX;
        // this.startY = startY;
        // this.endX = endX;
        // this.endY = endY;
        // this.middle = middle;
        // this.direction = dir;
        
    }
    
};