using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager instance; // インスタンスの定義
    public GameObject EnemyPrefab;

    public List<Enemy> EnemyList;

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
        List<Realm> _realms = Map.instance.realms;
        foreach(Realm _r in _realms){
            int _x = UnityEngine.Random.Range(_r.RoomLeft, _r.RoomRight + 1);
            int _y = UnityEngine.Random.Range(_r.RoomTop, _r.RoomBottom + 1);
            Vector3 _Pos = new Vector3(_x, _y, 0);
            GameObject m_Enemy = Instantiate(EnemyPrefab, _Pos, new Quaternion());
            Enemy _Enemy = m_Enemy.GetComponent<Enemy>();
            _Enemy.Pos = new Vector2Int(_x,_y);
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
    public bool IsDirectionToEnemy(Vector2Int _Pos,Vector2Int _DirectionPos){
        Vector2Int _CheckPos = new Vector2Int(_Pos.x + _DirectionPos.x, _Pos.x + _DirectionPos.y);
        foreach(Enemy _e in EnemyList){
            if(_e.Pos.x == _CheckPos.x && _e.Pos.y == _CheckPos.y){
                return true;
            }
        }        
        return false;
    }
}
