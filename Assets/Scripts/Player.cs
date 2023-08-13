using System;
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
public class Player : MonoBehaviour
{
    [SerializeField] protected float _speed = 5.0f;
    private float distance = 1.0f;
    private Vector2 move;
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

    public List<Player> TargetEnemyList = new List<Player>();

    public bool IsInAction = false;

    void Start()
    {
    }
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
        if(!IsSetup || !IsAlive){
            return;
        }
        if(GManager.instance.CurrentGameState != GameState.KeyInput ){
            return;
        }
        if (Input.GetKeyDown(KeyCode.W)){move.y = 1f;}
        if (Input.GetKeyDown(KeyCode.S)){move.y = -1f;}
        if (Input.GetKeyDown(KeyCode.A)){move.x = -1f;}
        if (Input.GetKeyDown(KeyCode.D)){move.x = 1f;}

        move.x = Input.GetAxisRaw("Horizontal");
        move.y = Input.GetAxisRaw("Vertical");

        Vector2Int checkTargetPos = new Vector2Int(Pos.x + (int)move.x, Pos.y + (int)move.y);

        bool isWall = Map.instance.CheckWall(checkTargetPos,Pos);
        bool isEnemy = EnemyManager.instance.IsExistEnemy(checkTargetPos);

        if (Input.GetKeyDown(KeyCode.E))
        {
            GManager.instance.SetCurrentState(GameState.PlayerTurn);
            Vector2Int _ToPos = GetPosFromDirction(dir);
            var result = EnemyManager.instance.GetDirectionToEnemy(Pos,_ToPos);
            if(result.IsExist){
                Attack(result.TargetEnemy);
                return;
            }
        }

        if (move != Vector2.zero)
        {
            // Vector2Int SetPos = new Vector2Int(0, 0);
            if(!isWall && !isEnemy){
                GManager.instance.SetCurrentState(GameState.PlayerTurn);
                Pos = new Vector2Int(Pos.x + (int)move.x, Pos.y + (int)move.y);
            }
            Vector2Int SetPos = new Vector2Int((int)move.x, (int)move.y);

            SetDirectionFromPos(SetPos);
            
        }
        if (isWall || isEnemy)
        {
            return;
        }
        if(GManager.instance.CurrentGameState == GameState.PlayerTurn){
            Move(checkTargetPos);
        }
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
    public void Attack(Player _Enemy){
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
            foreach(Player _e in TargetEnemyList){
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
        TargetEnemyList = new List<Player>();
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
            // "easeType", iTween.EaseType.linear,
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
    public void OnCompleteMove(){
        if(gameObject.tag == "Player"){
            UpdateRoomID();
            GManager.instance.SetCurrentState(GameState.EnemyBegin);
            Goal.instance.CheckGoal();
        }
        else{
            IsInAction = false;
        }        
        iTween.Stop(gameObject);
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
