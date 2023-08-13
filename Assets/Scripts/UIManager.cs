using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance; // インスタンスの定義
    public GameObject UI_Gameover;
    public GameObject UI_NextStage;
    public Text UI_StageCount;
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
    public void Init(int _NowStageCount){
        UI_Gameover.SetActive(false);
        UI_NextStage.SetActive(true);
        UI_StageCount.text = _NowStageCount.ToString();
        ShowStageCount();
    }
    public void GameOver(){
        UI_Gameover.SetActive(true);
    }
    public void ShowStageCount(){
        StartCoroutine(DelayMethod(1.0f, () => {
            UI_NextStage.SetActive(false);
        }));
    }
    private IEnumerator DelayMethod(float waitTime, Action action)
    {
        yield return new WaitForSeconds(waitTime);
        action();
    }
}
