using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{

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
    protected override void OnCompleteMove(){
        UpdateRoomID();
        GManager.instance.SetCurrentState(GameState.EnemyBegin);
        Goal.instance.CheckGoal();
        bool StepOnItem = ItemSpawn.instance.StepOnItem(Pos);

        iTween.Stop(gameObject);
    }  
}
