using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Map: MonoBehaviour
{
	public const int MAP_SIZE_X = 30;
	public const int MAP_SIZE_Y = 20;
    public int MIN_ROOM_SIZE = 4;
    public int MAX_ROOM_SIZE = 8;
    private GameObject floorPrefab;
	private GameObject wallPrefab;
    private int[,] map;

    private List<Pos> PosList = new List<Pos>();

    // Start is called before the first frame update
    void Start()
    {
        GenerateMap();
    }
    private void GenerateMap()
    {
        map = GenerateMap(MAP_SIZE_X,MAP_SIZE_Y);
		floorPrefab = Resources.Load("Prefabs/Floor") as GameObject;
		wallPrefab = Resources.Load("Prefabs/Wall") as GameObject;

		var floorList = new List<Vector3>();
		var wallList = new List<Vector3>();
        GameObject _obj = null;
        for (int y = 0; y < MAP_SIZE_Y; y++) {
			for (int x = 0; x < MAP_SIZE_X; x++) {
				if (map[x, y] == 1) {
					_obj = Instantiate(floorPrefab, new Vector3(x, y, 0), new Quaternion());
                    SpriteRenderer spriteRenderer = _obj.GetComponent<SpriteRenderer>();
                    spriteRenderer.color = Color.red; // 好みの色に変更する
				} else if (map[x, y] == 2) {
					_obj = Instantiate(floorPrefab, new Vector3(x, y, 0), new Quaternion());
                    SpriteRenderer spriteRenderer = _obj.GetComponent<SpriteRenderer>();
                    spriteRenderer.color = Color.blue; // 好みの色に変更する
				}else{
                    _obj = Instantiate(wallPrefab, new Vector3(x, y, 0), new Quaternion());
                }
			}
		}
    }
    public int[,] GenerateMap(int mapSizeX, int mapSizeY, int maxRoom = 1)
    {
        // this.mapSizeX = mapSizeX;
        // this.mapSizeY = mapSizeY;

        int[,] map = new int[mapSizeX, mapSizeY];

        
        System.Random random = new System.Random();
        int ROOM_X = random.Next(MIN_ROOM_SIZE, MAX_ROOM_SIZE + 1);
        int ROOM_Y = random.Next(MIN_ROOM_SIZE, MAX_ROOM_SIZE + 1);
        // bool isVertical = false;
        PosList.Add(new Pos(0,ROOM_X,0,ROOM_Y));
		foreach (Pos pos in PosList) {
			for (int x = pos.L_X; x <= pos.R_X; x++) {
				for (int y = pos.L_Y; y <= pos.R_Y; y++) {
					map[x, y] = 1;
				}
			}
		}        
        if(MAP_SIZE_Y - ROOM_Y >= MAX_ROOM_SIZE){
            int ROOM_X_2 = random.Next(ROOM_X + MIN_ROOM_SIZE, ROOM_X + MAX_ROOM_SIZE + 1);
            int ROOM_Y_2 = random.Next(ROOM_Y + MIN_ROOM_SIZE, ROOM_Y + MAX_ROOM_SIZE + 1);
            PosList.Add(new Pos(ROOM_X,ROOM_X_2,ROOM_Y,ROOM_Y_2));
            for (int x = ROOM_X; x <= ROOM_X_2; x++) {
                for (int y = ROOM_Y; y <= ROOM_Y_2; y++) {
                    map[x, y] = 2;
                }
            }
        }

        return map;
    }
}
public class Pos
{

    public int L_X = 0;
    public int R_X = 0;
    public int L_Y = 0;
    
    public int R_Y = 0;
    public Pos(int _L_X,int _R_X,int _L_Y,int _R_Y)
    {
        this.L_X = _L_X;
        this.R_X = _R_X;
        this.L_Y = _L_Y;
        this.R_Y = _R_Y;
    }
    //コンストラクタが呼び出し初期化
    public Pos() : this(0, 0, 0, 0) {}
}