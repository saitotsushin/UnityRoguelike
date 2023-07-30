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
    // public bool IsActed = false;

    void Start()
    {
    }
    public void SetUp(){
        targetPos = transform.position;
        IsSetup = true;
    }

    void Update()
    {
        if(!IsSetup){
            return;
        }
        if(GManger.instance.CurrentGameState != GameState.KeyInput ){
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
            GManger.instance.SetCurrentState(GameState.PlayerTurn);
            Vector2Int _ToPos = GetPosFromDirction(dir);
            bool isDirectionToEnemy = EnemyManager.instance.IsDirectionToEnemy(Pos,_ToPos);
            Attack(Pos,_ToPos);
        }
        if(isWall || isEnemy){
            return;
        }

        if (move != Vector2.zero)
        {
            GManger.instance.SetCurrentState(GameState.PlayerTurn);

            // targetPos += new Vector3(move.x, move.y, 0) * distance;
            Pos = new Vector2Int(Pos.x + (int)move.x, Pos.y + (int)move.y);
            
            if(move.x  < 0 && move.y == 0){ dir = DIRECTION.LEFT;}
            if(move.x  > 0 && move.y == 0){ dir = DIRECTION.RIGHT;}
            if(move.x == 0 && move.y  > 0){ dir = DIRECTION.TOP;}
            if(move.x == 0 && move.y  < 0){ dir = DIRECTION.BOTTOM;}
            if(move.x  > 0 && move.y  > 0){ dir = DIRECTION.RIGHT_TOP;}
            if(move.x  > 0 && move.y  < 0){ dir = DIRECTION.RIGHT_BOTTOM;} 
            if(move.x  < 0 && move.y  > 0){ dir = DIRECTION.LEFT_TOP;}
            if(move.x  < 0 && move.y  < 0){ dir = DIRECTION.LEFT_BOTTOM;}
        }
        if(GManger.instance.CurrentGameState == GameState.PlayerTurn){
            Move(checkTargetPos);
        }
    }
    public void Attack(Vector2Int _FromPos,Vector2Int _ToPos){
        Debug.Log(gameObject.name+"...Attack");
        FollowCamera.instance.RemoveFollowCamera();
        Vector2 AttackPos = new Vector2(
            _ToPos.x * 0.4f + _FromPos.x,
            _ToPos.y * 0.4f + _FromPos.y
        );
        iTween.MoveTo(this.gameObject, iTween.Hash(
            "x", AttackPos.x,
            "y", AttackPos.y,
            "time", 0.1f,
            "delay", 0f,
            // "easeType", iTween.EaseType.linear,
            "oncomplete", "OnCompleteAttackFrom"
        ));
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
        if(GManger.instance.CurrentGameState == GameState.PlayerTurn){
            GManger.instance.SetCurrentState(GameState.EnemyBegin);
        }
        if(GManger.instance.CurrentGameState == GameState.EnemyTurn){
            EnemyManager.instance.EndAction();
        }
    }
    public void OnCompleteAttackTo(){
        FollowCamera.instance.SetCameta();
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
        Debug.Log(gameObject.name+"...Move="+targetPosition);
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
    public void OnCompleteMove(){
        if(GManger.instance.CurrentGameState == GameState.PlayerTurn){
            UpdateRoomID();
            GManger.instance.SetCurrentState(GameState.EnemyBegin);
        }
        if(GManger.instance.CurrentGameState == GameState.EnemyTurn){
            EnemyManager.instance.EndAction();
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
