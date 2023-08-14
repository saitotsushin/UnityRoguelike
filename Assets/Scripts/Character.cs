using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum DIRECTION
{
    TOP,
    LEFT,
    RIGHT,
    BOTTOM,
    LEFT_TOP,
    RIGHT_TOP,
    LEFT_BOTTOM,
    RIGHT_BOTTOM
}
public class Character : MonoBehaviour
{
    [SerializeField] protected float _speed = 5.0f;
    private float distance = 1.0f;
    protected Vector2 move;
    private Vector3 targetPos;
    public Vector2Int Pos = new Vector2Int(0, 0);
    public bool IsSetup = false;

    public DIRECTION dir =  DIRECTION.BOTTOM;
    public int roomId = 0;
    private int BaseHP = 1;
    private int BaseAK = 1;
    private int BaseDF = 1;
    public int HP = 1;
    public int AK = 1;
    public int DF = 1;
    public bool IsAlive = true;

    public List<Character> TargetEnemyList = new List<Character>();

    public bool IsInAction = false;
    // Start is called before the first frame update    
    public void SetUp(){
        targetPos = transform.position;
        IsSetup = true;
        HP = BaseHP;
        AK = BaseAK;
        DF = BaseDF;
        IsAlive = true;
        gameObject.SetActive (true);        
    }

    void Update()
    {

    }
    public void SetDirectionFromPos(Vector2Int Pos){
        if(move.x  < 0 && move.y == 0){ dir = DIRECTION.LEFT;}
        if(move.x  > 0 && move.y == 0){ dir = DIRECTION.RIGHT;}
        if(move.x == 0 && move.y  > 0){ dir = DIRECTION.TOP;}
        if(move.x == 0 && move.y  < 0){ dir = DIRECTION.BOTTOM;}
        if(move.x  > 0 && move.y  > 0){ dir = DIRECTION.RIGHT_TOP;}
        if(move.x  > 0 && move.y  < 0){ dir = DIRECTION.RIGHT_BOTTOM;} 
        if(move.x  < 0 && move.y  > 0){ dir = DIRECTION.LEFT_TOP;}
        if(move.x  < 0 && move.y  < 0){ dir = DIRECTION.LEFT_BOTTOM;}
    }
    public void Attack(Character _Enemy){
        if(!IsAlive){
            return;
        }
        TargetEnemyList.Add(_Enemy);

        Vector2Int _FromPos = Pos;
        Vector2Int _ToPos = _Enemy.Pos;

        FollowCamera.instance.RemoveFollowCamera();
        Vector2 DiffAttackPos = new Vector2(
            _ToPos.x - _FromPos.x,
            _ToPos.y - _FromPos.y
        );
        Vector2 AttackPos = new Vector2(
            DiffAttackPos.x * 0.4f + _FromPos.x,
            DiffAttackPos.y * 0.4f + _FromPos.y
        );
        iTween.MoveTo(this.gameObject, iTween.Hash(
            "x", AttackPos.x,
            "y", AttackPos.y,
            "time", 0.1f,
            "delay", 0f,
            "oncomplete", "OnCompleteAttackFrom"
        ));

        /*攻撃計算*/
        int Damage = _Enemy.DF - AK;
        if(Damage <= 0){
            Damage = 1;
        }
        _Enemy.HP -= Damage;
    }
    public void OnCompleteAttackFrom()
    {
        iTween.MoveTo(this.gameObject, iTween.Hash(
            "x", Pos.x,
            "y", Pos.y,
            "time", 0.1f,
            "delay", 0f,
            "oncomplete", "OnCompleteAttackTo"
        ));
        
    }
    public void OnCompleteAttackTo(){        
        FollowCamera.instance.SetCameta();
        if(TargetEnemyList.Count > 0){
            foreach(Character _e in TargetEnemyList){
                if(_e.HP <= 0){
                    _e.Dead();
                }
            }
        }
        if(GManager.instance.CurrentGameState == GameState.PlayerTurn){
            GManager.instance.SetCurrentState(GameState.EnemyBegin);
        }
        iTween.Stop(gameObject);

        IsInAction = false;
        TargetEnemyList = new List<Character>();
    }    
    public Vector2Int GetPosFromDirction(DIRECTION _dir){
        Vector2Int _CheckPos = new Vector2Int(0, 0);
        switch(_dir){
            case DIRECTION.LEFT:
                _CheckPos = new Vector2Int(-1, 0);
                break;
            case DIRECTION.RIGHT:
                _CheckPos = new Vector2Int(1, 0);
                break;
            case DIRECTION.TOP:
                _CheckPos = new Vector2Int(0, 1);
                break;
            case DIRECTION.BOTTOM:
                _CheckPos = new Vector2Int(0, -1);
                break;
            case DIRECTION.LEFT_BOTTOM:
                _CheckPos = new Vector2Int(-1, -1);
                break;
            case DIRECTION.RIGHT_BOTTOM:
                _CheckPos = new Vector2Int(1, -1);
                break;
            case DIRECTION.LEFT_TOP:
                _CheckPos = new Vector2Int(-1, 1);
                break;
            case DIRECTION.RIGHT_TOP:
                _CheckPos = new Vector2Int(1, 1);
                break;
        }
        return _CheckPos;
    }
    public void Move(Vector2Int targetPosition)
    {
        if(!IsAlive){
            return;
        }
        iTween.MoveTo(this.gameObject, iTween.Hash(
            "x", targetPosition.x,
            "y", targetPosition.y,
            "time", 0.1f,
            "delay", 0f,
            "oncomplete", "OnCompleteMove",
            "oncompletetarget", gameObject
        ));
        Pos.x = targetPosition.x;
        Pos.y = targetPosition.y;

    }
    public void Dead(){
        IsAlive = false;
        gameObject.SetActive (false);
    }
    protected virtual void OnCompleteMove(){
    }  
    public void UpdateRoomID(){
        List<Realm> _realms = Map.instance.realms;
        foreach(Realm _r in _realms){
            if(_r.RoomLeft <= Pos.x && _r.RoomRight >= Pos.x){
                if (_r.RoomTop <= Pos.y && _r.RoomBottom >= Pos.y)
                {
                    roomId = _r.id;
                }
            }
        }
        EnemyManager.instance.UpdateEnemyState();
    }
    public void SetStart(Realm _r){
        int _x = UnityEngine.Random.Range(_r.RoomLeft, _r.RoomRight + 1);
        int _y = UnityEngine.Random.Range(_r.RoomTop, _r.RoomBottom + 1);
        transform.position = new Vector3(_x, _y, 0);
        Pos = new Vector2Int(_x, _y);
        roomId = _r.id;
        SetUp();
    }
}
