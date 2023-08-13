using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    public static Goal instance; // インスタンスの定義
    public Player _Player;
    public GameObject GoalPrefab;
    public Vector2Int Pos = new Vector2Int(0,0);
    public GameObject m_GoalPrefab;
    // Start is called before the first frame update
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
    public void CreateGoal()
    {
        m_GoalPrefab = Instantiate(GoalPrefab, new Vector3(0f, 0.0f, 0.0f), new Quaternion());
    }

    public void SetGoal(){
        _Player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        int roomId = GetRandomRoomId();
        List<Realm> _realms = Map.instance.realms;
        Realm _r = Map.instance.getRealmFromId(_realms,roomId);
        int _x = UnityEngine.Random.Range(_r.RoomLeft, _r.RoomRight + 1);
        int _y = UnityEngine.Random.Range(_r.RoomTop, _r.RoomBottom + 1);
        Vector3 _Pos = new Vector3(_x, _y, 0);
        Pos = new Vector2Int(_x, _y);
        m_GoalPrefab.transform.position = _Pos;
    }
    public void CheckGoal(){
        if(_Player.Pos.x == Pos.x && _Player.Pos.y == Pos.y){
            GManager.instance.NextStage();
        }
    }
    public int GetRandomRoomId(){
        List<Realm> _realms = Map.instance.realms;

        int id = 0;
        int count = 0;
        while(true){
            count++;
            int random = UnityEngine.Random.Range(0, _realms.Count);
            Realm _r = _realms[random];            
            if (_Player.roomId != _r.id)
            {
                id = _r.id;
                break;
            }
            if(count > 100){
                id = 0;
                break;
            }
        }
        return id;
    }
}
