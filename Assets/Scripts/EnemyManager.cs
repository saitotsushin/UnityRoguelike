using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager instance; // インスタンスの定義
    public GameObject EnemyPrefab;

    public Player _Player;

    public List<Enemy> EnemyList;
    public int ActionCount = 0;
    // デリゲートの定義
    public delegate void MyFunctionDelegate();

    public List<MyFunctionDelegate> functionList = new List<MyFunctionDelegate>();
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
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void CreateEnemy(){
        _Player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        List<Realm> _realms = Map.instance.realms;
        foreach(Realm _r in _realms){
            int _x = UnityEngine.Random.Range(_r.RoomLeft, _r.RoomRight + 1);
            int _y = UnityEngine.Random.Range(_r.RoomTop, _r.RoomBottom + 1);
            if(_x == _Player.Pos.x && _y == _Player.Pos.y){
                continue;
            }
            Vector3 _Pos = new Vector3(_x, _y, 0);
            GameObject m_Enemy = Instantiate(EnemyPrefab, _Pos, new Quaternion());
            Enemy _Enemy = m_Enemy.GetComponent<Enemy>();
            _Enemy.Pos = new Vector2Int(_x,_y);
            _Enemy.roomId = _r.id;
            _Enemy.IsSetup = true;
            EnemyList.Add(_Enemy);
        }
    }
    public bool IsExistEnemy(Vector2Int _NextPos){
        foreach(Enemy _e in EnemyList){
            if(_e.Pos.x == _NextPos.x && _e.Pos.y == _NextPos.y){
                return true;
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
        // デリゲートのリストを作成し、関数を追加
        functionList = new List<MyFunctionDelegate>();
        
        foreach (Enemy _e in EnemyList)
        {
            functionList.Add(_e.Action);
        } 
        if(functionList.Count == 0){
            GManger.instance.SetCurrentState(GameState.TurnEnd);
        }
    }
    public void ExecActions(){
        // リスト内の関数を順に実行
        foreach (var func in functionList) {
            func();
        }
    }
    public void EndAction(){
        ActionCount++;
        if(functionList.Count == ActionCount){
            ActionCount = 0;
            functionList = new List<MyFunctionDelegate>();
            GManger.instance.SetCurrentState(GameState.TurnEnd);
        }
    }
}
