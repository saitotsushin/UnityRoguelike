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
    [SerializeField] private float _speed = 5.0f;
    private float distance = 1.0f;
    private Vector2 move;
    private Vector3 targetPos;
    public Vector2Int Pos = new Vector2Int(0, 0);
    public bool IsSetup = false;

    public DIRECTION dir =  DIRECTION.BOTTOM;

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
            Vector2Int _ToPos = GetPosFromDirction(dir);
            bool isDirectionToEnemy = EnemyManager.instance.IsDirectionToEnemy(Pos,_ToPos);
            Attack(Pos,_ToPos);
            // if(isDirectionToEnemy){
            //     Attack(_Pos,_ToPos);
            // }
        }
        if(isWall || isEnemy){
            return;
        }

        if (move != Vector2.zero && transform.position == targetPos)
        {
            targetPos += new Vector3(move.x, move.y, 0) * distance;
            Pos = new Vector2Int(Pos.x + (int)move.x, Pos.y + (int)move.y);
            // Vector2Int dirPos = new Vector2Int();
            if(move.x  < 0 && move.y == 0){ dir = DIRECTION.LEFT;}
            if(move.x  > 0 && move.y == 0){ dir = DIRECTION.RIGHT;}
            if(move.x == 0 && move.y  > 0){ dir = DIRECTION.TOP;}
            if(move.x == 0 && move.y  < 0){ dir = DIRECTION.BOTTOM;}
            if(move.x  > 0 && move.y  > 0){ dir = DIRECTION.RIGHT_TOP;}
            if(move.x  > 0 && move.y  < 0){ dir = DIRECTION.RIGHT_BOTTOM;} 
            if(move.x  < 0 && move.y  > 0){ dir = DIRECTION.LEFT_TOP;}
            if(move.x  < 0 && move.y  < 0){ dir = DIRECTION.LEFT_BOTTOM;}            
        }
        Move(targetPos);

    }
    public void Attack(Vector2Int _FromPos,Vector2Int _ToPos){
        FollowCamera.instance.RemoveFollowCamera();

        Debug.Log("_FromPos=" + _FromPos);
        Debug.Log("_ToPos=" + _ToPos);
        Vector2 AttackPos = new Vector2(
            _ToPos.x * 0.4f + _FromPos.x,
            _ToPos.y * 0.4f + _FromPos.y
        );
        Debug.Log("AttackPos=" + AttackPos);
        iTween.MoveTo(this.gameObject, iTween.Hash(
            "x", AttackPos.x,
            "y", AttackPos.y,
            "time", 0.1f,
            "delay", 0f,
            // "easeType", iTween.EaseType.linear,
            "oncomplete", "OnCompleteFrom"
        ));
    }
    public void OnCompleteFrom()
    {
        
        iTween.MoveTo(this.gameObject, iTween.Hash(
            "x", Pos.x,
            "y", Pos.y,
            "time", 0.1f,
            "delay", 0f,
            "oncomplete", "OnCompleteTo"
        ));  
        // iTween.Stop(this.gameObject, "move");

        // アニメーションが終わった時の処理
    }
    public void OnCompleteTo(){
        Debug.Log("アニメ終わり");
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
    private void Move(Vector3 targetPosition)
    {
        
        transform.position = Vector3.MoveTowards(transform.position, targetPosition,
            _speed * Time.deltaTime);
    }
    public void SetStart(Realm _r){
        int _x = UnityEngine.Random.Range(_r.RoomLeft, _r.RoomRight + 1);
        int _y = UnityEngine.Random.Range(_r.RoomTop, _r.RoomBottom + 1);
        transform.position = new Vector3(_x, _y, 0);
        Pos = new Vector2Int(_x, _y);
        SetUp();
    }
}
