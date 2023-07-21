using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // public float moveSpeed = 1f;
    // public float inputInterval = 0.5f; // キー入力の間隔
    // private float lastInputTime; // 最後にキー入力があった時刻
    // private Rigidbody rb;
    [SerializeField] private float _speed = 5.0f;
    private float distance = 1.0f;
    private Vector2 move;
    private Vector3 targetPos;
    public Vector2Int Pos = new Vector2Int(0, 0);
    public bool IsSetup = false;
    // Start is called before the first frame update
    void Start()
    {
        // lastInputTime = Time.time; // ゲーム開始時点の時刻を記録
        // rb = GetComponent<Rigidbody>();
    }
    public void CreatePlayer(){

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
        if(isWall){
            return;
        }

        if (move != Vector2.zero && transform.position == targetPos)
        {
            targetPos += new Vector3(move.x, move.y, 0) * distance;
            Pos = new Vector2Int(Pos.x + (int)move.x, Pos.y + (int)move.y);
        }
        Move(targetPos);

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
