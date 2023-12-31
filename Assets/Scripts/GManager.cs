using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataBase;
public enum GameState
{
    // 開始
    KeyInput, // キー入力待ち＝プレイヤーターン開始
    PlayerTurn, //プレイヤーの行動中
    EnemyBegin, // エネミーターン開始
    EnemyTurn, //エネミーの行動中
    TurnEnd,   // ターン終了→KeyInputへ変遷
    GameOver
}
public class GManager : MonoBehaviour
{
    public static GManager instance; // インスタンスの定義
    public Map _Map;
    public GameObject PlayerPrefab;
    public Player _Player;
    public GameState CurrentGameState; //現在のゲーム状態
    public Transform parentLayer;
    public ItemInstanceDataBaseManager _ItemInstanceDataBaseManager;
    public EnemyInstanceDataBaseManager _EnemyInstanceDataBaseManager;
    [SerializeField]
    private int NowStageCount = 0;
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
    void Start()
    {
        GameObject _player = Instantiate(PlayerPrefab, new Vector3(0, 0, 0), new Quaternion(),parentLayer);
        _Player = _player.GetComponent<Player>();
        Goal.instance.CreateGoal();
        _ItemInstanceDataBaseManager.Create();
        _EnemyInstanceDataBaseManager.Create();
        HpBar.instance.SetStatus(_Player.HP);
        NextStage();

    }
    public void NextStage(){
        NowStageCount++;
        _Map.GenerateMap();
        Realm _StartRealm = _Map.GetRandomRealm();
        _Player.SetStart(_StartRealm);
        FollowCamera.instance.SetCameta();
        EnemyManager.instance.CreateEnemy();
        Goal.instance.SetGoal();
        ItemSpawn.instance.CreateItem();
        UIManager.instance.Init(NowStageCount);
        OnGameStateChanged(GameState.KeyInput);
    }

    //現在のゲームステータスを変更する関数　外部及び内部から
    public void SetCurrentState(GameState state)
    {
        CurrentGameState = state;
        OnGameStateChanged(CurrentGameState);
    }
    void OnGameStateChanged(GameState state)
    {
        switch (state)
        {
            case GameState.KeyInput:
                break;

            case GameState.PlayerTurn:
                break;

            case GameState.EnemyBegin:
                EnemyManager.instance.EnemyActions();
                SetCurrentState(GameState.EnemyTurn);
                break;

            case GameState.EnemyTurn:
                break;

            case GameState.TurnEnd:
                if(!_Player.IsAlive){
                    GManager.instance.SetCurrentState(GameState.GameOver);
                }else{
                    SetCurrentState(GameState.KeyInput);
                }
                break;
            case GameState.GameOver:
                UIManager.instance.GameOver();
                NowStageCount = 0;
                SetCurrentState(GameState.KeyInput);
                break;
        }
    }
}
