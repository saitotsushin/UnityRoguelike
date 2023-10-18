using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpBar : MonoBehaviour
{
    public static HpBar instance; // インスタンスの定義
    public Text TextHP;
    private int MaxHP;
    public GameObject HPsprite;
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
    public void SetStatus(int _HP){
        TextHP.text = _HP.ToString();
        MaxHP = _HP;
    }
    public void CalcStatus(){

        Player _Player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        float ParHP = (float)_Player.HP / _Player.BaseHP;

        TextHP.text = _Player.HP.ToString();

        Vector3 newScale = new Vector3(ParHP,1f,1f);
        RectTransform rectTransform = HPsprite.GetComponent<RectTransform>();

        // スケールを変更
        rectTransform.localScale = newScale;
    }
}
