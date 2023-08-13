using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager instance; // インスタンスの定義
    public GameObject EnemyPrefab;

    public Player _Player;

    public List<Enemy> EnemyList;
    public Transform parentLayer;

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


    // Update is called once per frame
    void Update()
    {
        if(GManager.instance.CurrentGameState == GameState.EnemyTurn){
            bool IsAction = IsActions();
            if(!IsAction){
                EndAction();            
            }
        }
    }
    private bool IsActions(){
        bool CheckEndAction = false;
        foreach (Enemy _e in EnemyList)
        {
            if(_e.IsAlive){
                CheckEndAction = _e.IsInAction;
                if(CheckEndAction){
                    break;
                }
            }
        }
        return CheckEndAction;
    }
    public void CreateEnemy(){
        DestroyChildAll(parentLayer);        
        _Player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        
        List<Realm> _realms = Map.instance.realms;
        foreach(Realm _r in _realms){
            int _x = UnityEngine.Random.Range(_r.RoomLeft, _r.RoomRight + 1);
            int _y = UnityEngine.Random.Range(_r.RoomTop, _r.RoomBottom + 1);
            if(_x == _Player.Pos.x && _y == _Player.Pos.y){
                continue;
            }
            
            Vector3 _Pos = new Vector3(_x, _y, 0);
            GameObject m_Enemy = Instantiate(EnemyPrefab, _Pos, new Quaternion(),parentLayer);
            Enemy _Enemy = m_Enemy.GetComponent<Enemy>();
            _Enemy.Pos = new Vector2Int(_x,_y);
            _Enemy.roomId = _r.id;
            _Enemy.IsSetup = true;
            EnemyList.Add(_Enemy);
        }
    }
    private void DestroyChildAll(Transform root)
    {
        //自分の子供を全て調べる
        foreach (Transform child in root)
        {
            //自分の子供をDestroyする
            if(child.gameObject.tag == "Player"){
                continue;
            }
            Destroy(child.gameObject);
        }
        EnemyList = new List<Enemy>();
    }
    public bool IsExistEnemy(Vector2Int _NextPos){
        foreach(Enemy _e in EnemyList){
            if(_e.IsAlive){
                if(_e.Pos.x == _NextPos.x && _e.Pos.y == _NextPos.y){
                    return true;
                }
            }
        }
        return false;        
    }
    public (Enemy TargetEnemy, bool IsExist) GetDirectionToEnemy(Vector2Int _Pos,Vector2Int _DirectionPos){
        Enemy _Enemy = new Enemy();
        bool _IsExist = false;
        Vector2Int _CheckPos = new Vector2Int(_Pos.x + _DirectionPos.x, _Pos.y + _DirectionPos.y);
        foreach(Enemy _e in EnemyList){
            if(_e.Pos.x == _CheckPos.x && _e.Pos.y == _CheckPos.y){
                _Enemy = _e;
                _IsExist = true;
            }
        }        
        return (_Enemy, _IsExist);
    }

    public void UpdateEnemyState(){
        foreach (Enemy _e in EnemyList)
        {
            if(_e.roomId == _Player.roomId){
                _e.isChase = true;
            }
        }
    }
    public void EnemyActions(){        
        foreach (Enemy _e in EnemyList)
        {
            if(_e.IsAlive){
                _e.Action();
            }
        }
    }
    public void EndAction(){
        GManager.instance.SetCurrentState(GameState.TurnEnd);
    }

}
