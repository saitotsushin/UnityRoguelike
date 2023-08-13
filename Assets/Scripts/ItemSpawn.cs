using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataBase;

public class ItemSpawn : MonoBehaviour
{
    public static ItemSpawn instance; // インスタンスの定義
    public GameObject ItemPrefab;
    public Player _Player;
    public Transform parentLayer;
    public List<Item> ItemList;
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
    public void CreateItem(){
        DestroyChildAll(parentLayer);
        
        _Player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        List<Realm> _realms = Map.instance.realms;
        foreach(Realm _r in _realms){
            int CreateCount = UnityEngine.Random.Range(1,3);
            for(int i = 0; i < CreateCount; i++){
                SetItem(_r);
            }
        }
    }
    public void SetItem(Realm _r){
        int _x = UnityEngine.Random.Range(_r.RoomLeft, _r.RoomRight + 1);
        int _y = UnityEngine.Random.Range(_r.RoomTop, _r.RoomBottom + 1);
        if(!CheckSetPos(_x,_y)){
            return;
        }            
        Vector3 _Pos = new Vector3(_x, _y, 0);
        GameObject m_Item = Instantiate(ItemPrefab, _Pos, new Quaternion(),parentLayer);
        Item _Item = m_Item.GetComponent<Item>();
        _Item.Pos = new Vector2Int(_x,_y);
        _Item.roomId = _r.id;
        SetItemData(_Item, 1);
        ItemList.Add(_Item);
    }
    public bool StepOnItem(Vector2Int _Pos){
        bool IsStepOn = false;
        foreach(Item _item in ItemList){
            if(_item.Pos.x == _Pos.x && _item.Pos.y == _Pos.y){
                if(!_item.IsFired){
                    _item.Fire();
                    IsStepOn = true;
                    continue;
                }
            }
        }
        return IsStepOn;
    }
    public void SetItemData(Item _Item, int _ItemId){
        ItemInstanceData _ItemInstanceData = ItemInstanceDataBaseManager.instance.GetItem(_ItemId);
        _Item.ItemName = _ItemInstanceData.ItemName;
    }
    public bool CheckSetPos(int _x, int _y){
        List<Enemy> _enemyList = EnemyManager.instance.EnemyList;
        bool CanSet = true;
        if(_x == _Player.Pos.x && _y == _Player.Pos.y){
            CanSet = false;
        }
        
        foreach(Enemy _e in _enemyList){
            if(_x == _e.Pos.x && _y == _e.Pos.y){
                CanSet = false;
                continue;
            }
        }
        return CanSet;
    }
    private void DestroyChildAll(Transform root)
    {
        //自分の子供を全て調べる
        foreach (Transform child in root)
        {
            Destroy(child.gameObject);
        }
        ItemList = new List<Item>();
    }
}
