using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ACTION
{
    WAIT,
    MOVE,
    ATTACK
}
public class Enemy : Player
{
    public Player _Player;
    public ACTION ActionState = ACTION.WAIT;
    public bool isChase = false;
    void Start()
    {
        _Player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        
    }
    void Update()
    {
        if(!IsSetup){
            return;
        }
    }
    public void Action(){
        // Vector2Int CheckPos = new Vector2Int(_Player.Pos.x - Pos.x,_Player.Pos.y - Pos.y);
        var result = GetCanAttackPlayer(_Player.Pos);
        Vector2Int _ToPos = GetPosFromDirction(result.PlayerDirection);

        if(result.CanAttack){
            Attack(_Player);
        }else if (isChase)
        {
            Vector2Int MovePos = ChasePlayer();
            if (MovePos.x != _Player.Pos.x || MovePos.y != _Player.Pos.y)
            {
                Move(MovePos);
            }
        }else{
            Wait();
        }
    }
    public void Wait(){
        EnemyManager.instance.EndAction();
    }
    public (bool CanAttack ,DIRECTION PlayerDirection) GetCanAttackPlayer(Vector2Int _PlayerPos)
    {
        if(Pos.x - 1 <= _PlayerPos.x && _PlayerPos.x <= Pos.x + 1 &&
           Pos.y - 1 <= _PlayerPos.y && _PlayerPos.y <= Pos.y + 1 
        ){
            Vector2Int CheckPos = new Vector2Int(_Player.Pos.x - Pos.x,_Player.Pos.y - Pos.y);
            // Vector2Int _PlayerPos =
            DIRECTION _dir = GetPosToDirection(CheckPos);
            return (true,_dir);
        }
        return (false,dir);
    }

    public Vector2Int ChasePlayer(){
        Vector2Int move = new Vector2Int(_Player.Pos.x - Pos.x, _Player.Pos.y - Pos.y);

        dir = GetPosToDirection(move);

        Vector2Int _ToPos = GetPosFromDirction(dir);

        Vector2Int MovePos = new Vector2Int(Pos.x + _ToPos.x, Pos.y + _ToPos.y);
        return MovePos;
    }
    public DIRECTION GetPosToDirection(Vector2Int _Pos){
        DIRECTION _dir = DIRECTION.LEFT;
        if(_Pos.x  < 0 && _Pos.y == 0){ _dir = DIRECTION.LEFT;}
        if(_Pos.x  > 0 && _Pos.y == 0){ _dir = DIRECTION.RIGHT;}
        if(_Pos.x == 0 && _Pos.y  > 0){ _dir = DIRECTION.TOP;}
        if(_Pos.x == 0 && _Pos.y  < 0){ _dir = DIRECTION.BOTTOM;}
        if(_Pos.x  > 0 && _Pos.y  > 0){ _dir = DIRECTION.RIGHT_TOP;}
        if(_Pos.x  > 0 && _Pos.y  < 0){ _dir = DIRECTION.RIGHT_BOTTOM;} 
        if(_Pos.x  < 0 && _Pos.y  > 0){ _dir = DIRECTION.LEFT_TOP;}
        if(_Pos.x  < 0 && _Pos.y  < 0){ _dir = DIRECTION.LEFT_BOTTOM;}
        return _dir;
    }
}
