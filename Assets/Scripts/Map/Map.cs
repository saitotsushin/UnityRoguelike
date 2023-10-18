using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    public static Map instance; // インスタンスの定義
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
    
    public int[,] floorMap;
    public int setId = 0;
    public List<Realm> realms = new List<Realm>();
    public List<Connect> connects = new List<Connect>();

    private int testRealmsCount = 0;//後で消す
    private int minArea = 60;
    private int minWidth = 8;
    private int maxWidth = 0;
    private int minHeight = 5;
    private int maxHeight = 5;
    private int minRoomWidth = 4;
    private int minRoomHeight = 4;    
    private int roomMinusRange = 3;
    private int separateSize = 8;

    private MapUtil _MapUtil;
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
        floorPrefab = Resources.Load("Prefabs/Floor") as GameObject;
        wallPrefab = Resources.Load("Prefabs/Wall") as GameObject;
        roadPrefab = Resources.Load("Prefabs/Road") as GameObject;
        floorInfoPrefab = Resources.Load("Prefabs/FloorInfo") as GameObject;
        roadAreaPrefab = Resources.Load("Prefabs/RoadArea") as GameObject;
        _MapUtil = new MapUtil();
    }
    public void ResetMap(){
        realms = new List<Realm>();
        floorMap = new int[mapSizeY, mapSizeX];
        maxWidth = (int)mapSizeX / 3 * 2;
        maxHeight = (int)mapSizeY / 3 * 2;
        //初期値
        realms.Add(new Realm(0, 0, mapSizeX, mapSizeY));

        floorMap = IntMap(floorMap);
        
    }
    public void GenerateMap()
    {
        ResetMap();
        
        bool isGenarateMap = false;
        for (int i = 0; i < separateSize; i++)
        {
            isGenarateMap = divideRoom();
            if(!isGenarateMap){
                break;
            }            
        }

        MakeRoom();

        RemoveMinRoom();

        CreatRoom();

        CheckLoad();

        MakeAdjacent();
        
        foreach(Realm _r in realms){
            for (int i = 0; i < _r.adjacentIDs.Count;i++){
                int ToId = _r.adjacentIDs[i];
                addConnect(_r, _MapUtil.getRealmFromId(realms,ToId));
            }
        }
        MakeRoad();
        MapDebug.instance.DebugMap(floorMap);
        MapDebug.instance.DebugRealms(realms,mapSizeX,mapSizeY);        

    }
    private void CheckLoad(){
        List<int> connectIdList = new List<int>();
        List<int> notConnectIdList = new List<int>();
        foreach (Realm _r in realms)
        {
            foreach (Connect _c in connects)
            {
                if(_c.fromId == _r.id || _c.toId == _r.id){
                    connectIdList.Add(_r.id);
                }
            }
            if(connectIdList.Count == 0){
                notConnectIdList.Add(_r.id);
            }       
        }
        if (connectIdList.Count > 0)
        {
            for(int i = 0; i < connectIdList.Count; i++){
                Debug.Log("取り残させれたid=" + connectIdList[i]);
            }
        }
    }
    private void RemoveMinRoom(){
        List<Realm> newRealms = new List<Realm>();
        List<Connect> newConnect = new List<Connect>();
        List<int> RemoveIds = new List<int>();
        foreach(Realm _r in realms){
            if(_r.roomSizeX >= minRoomWidth && _r.roomSizeY >= minRoomHeight){
                newRealms.Add(_r);
            }else{
                // newRealms.Add(_r);
                RemoveIds.Add(_r.id);
            }
        }
        foreach (Connect _c in connects)
        {
            bool ContainFromId = RemoveIds.Contains(_c.fromId);
            bool ContainToId = RemoveIds.Contains(_c.toId);
            if(!ContainFromId && !ContainToId){
                newConnect.Add(_c);
            }
        }

        realms = newRealms;
        connects = newConnect;

    }
    /** 接続を追加する
	*
    * @param	startRealm	始点の領域
	* @param	endRealm	終点の領域
	*
	**/
    public void addConnect (Realm fromRealm, Realm toRealm) {
        Connect _connect = new Connect();
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
        List<Connect> new_connects = new List<Connect>();
        foreach(Connect _c in connects){
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

        foreach (Connect _c in connects)
        {
            bool IsAlreadyConnected = checkAlreadyConnected(_c);
            drawConnect(_c);
        }

    }
    public Realm GetRandomRealm(){
        int maxCount = realms.Count;
        int randomNumber = UnityEngine.Random.Range(0, maxCount);
        return realms[randomNumber];
    }
    public bool checkAlreadyConnected(Connect c){
        bool isConnected = false;
        foreach (Connect _c in connects)
        {
            if(_c.fromId == c.toId && _c.toId == c.fromId){
                if(_c.isConnected){
                    c.isConnected = true;
                }
            }
        }
        return isConnected;
    }
    public void drawConnect (Connect c) {
        if(c.isConnected){
            return;
        }
        GameObject _areaObj = Instantiate(roadAreaPrefab, new Vector3(0, 0, 0), new Quaternion(),parentRoad);
        Transform _area = _areaObj.transform;

        c.isConnected = true;//接続済み

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
            floorMap[posY, posX] = 1;
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
            floorMap[posY, posX] = 1;
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
            floorMap[posY, posX] = 1;
            RoadInfo _m_obj = _areaObj.GetComponent<RoadInfo>();
            _m_obj.SetStatus(c);
        }
        
    }
    public bool divideRoom(){
        bool isDivide = separateLargestRealm();
        if (!isDivide)
        {
            return false;
        };
        Realm firstRealm = new Realm();
        Realm secondRealm = new Realm();
        firstRealm = realms[realms.Count - 2];
        secondRealm = realms[realms.Count - 1];

        GameObject _obj;
        SpriteRenderer spriteRenderer;
        testRealmsCount++;
        int test_creat_count = 0;

        for (int y = firstRealm.top; y < firstRealm.top + firstRealm.sizeY; y++){
            for (int x = firstRealm.left; x < firstRealm.left + firstRealm.sizeX; x++){
                _obj = Instantiate(wallPrefab, new Vector3(x, y, 0), new Quaternion(),parentRealms);      
                _obj.GetComponent<BlockInfo>().SetPos(x,y);
                test_creat_count++;
            }             
        }
        testRealmsCount++;

        for (int y = secondRealm.top; y < secondRealm.top + secondRealm.sizeY; y++){
            for (int x = secondRealm.left; x < secondRealm.left + secondRealm.sizeX; x++){
                _obj = Instantiate(wallPrefab, new Vector3(x, y, 0), new Quaternion(),parentRealms);      
                _obj.GetComponent<BlockInfo>().SetPos(x,y);  
                test_creat_count++;
            }           
        }
        return isDivide;
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

    public int[,] IntMap(int[,] _map)
    {
        int[,] newMap = new int[mapSizeY, mapSizeX];
        for (int y = 0; y < _map.GetLength(0); y++)
        {
            for (int x = 0; x < _map.GetLength(1); x++)
            {
                _map[y, x] = 0;
            }
        }
        return newMap;
    }
    private bool separateLargestRealm(){
        int index = 0;

        index = _MapUtil.getIndexOfLargestRealm(realms);

        bool isSeparate = false;

        if (index >= 0) {
         
            Realm r = _MapUtil.getRealmFromId(realms,index);
            if (r.sizeX >= r.sizeY) {
            
                isSeparate = separateRealmByX(index);
               
            } else {
               
                isSeparate = separateRealmByY(index);
                
            }
            
        }else{
            isSeparate = false;
            Debug.LogError("分割できませんでした");
        }
        return isSeparate;
    }
    private bool separateRealmByX(int id) {
        int minWidthByArea = 0;
        int separateX;

        Realm r = _MapUtil.getRealmFromId(realms,id);

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

        Realm r = _MapUtil.getRealmFromId(realms,id);

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
            if ((r.sizeX - 4 ) * (r.sizeY - 4) < minArea)
            {
                _SetLeft = r.left + 2;
                _SetTop = r.top + 2;
                _SetSizeX = r.sizeX - 4;
                _SetSizeY = r.sizeY - 4;                
                r.setRoom(_SetLeft, _SetTop, _SetSizeX, _SetSizeY);
                
            }else{

                int max_room_size = (r.sizeX - 4) * (r.sizeY - 4);

                int rWidth = 0;
                int _minusWidth = (int)(UnityEngine.Random.Range(0, roomMinusRange + 1));
                rWidth = r.sizeX - _minusWidth - 4;
                if(rWidth <= minWidth){
                    rWidth = minWidth;
                }
                if(r.sizeX - 4 <= rWidth){
                    rWidth = r.sizeX - 4;
                }

                int _minusHeight = (int)(UnityEngine.Random.Range(0, roomMinusRange + 1));
                int rHeight = r.sizeY - _minusHeight - 4;
                if(rHeight <= minHeight){
                    rHeight = minHeight;
                }
                if(r.sizeY - 4 <= rHeight){
                    rHeight = r.sizeY - 4;
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

        }
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
                    floorMap[y, x] = 1;

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
        target = _MapUtil.getRealmFromId(realms,index);

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
    public bool CheckWall(Vector2Int _NextPos,Vector2Int _NowPos){
        if(floorMap[_NowPos.y,_NextPos.x] == 0 ||
           floorMap[_NextPos.y,_NowPos.x] == 0 ||
           floorMap[_NextPos.y,_NextPos.x] == 0){
            return true;
        }
        return false;
    }
}