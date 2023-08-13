using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
     public static FollowCamera instance; // インスタンスの定義
    // Player player;
    GameObject playerObjct;
    Transform playerTransform;
    bool isFollow = true;
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
    public void SetCameta(){
        playerObjct = GameObject.FindGameObjectWithTag("Player");
        playerTransform = playerObjct.transform;
        isFollow = true;
    }
    public void RemoveFollowCamera(){
        isFollow = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(!isFollow){
            return;
        }
        transform.position = new Vector3(playerTransform.position.x, playerTransform.position.y, transform.position.z);
    }
}
