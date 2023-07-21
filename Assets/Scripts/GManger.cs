using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GManger : MonoBehaviour
{
    public Map _Map;
    public GameObject PlayerPrefab;
    public Player _Player;
    // Start is called before the first frame update
    void Start()
    {
        _Map.GenerateMap();
        GameObject _player = Instantiate(PlayerPrefab, new Vector3(0, 0, 0), new Quaternion());
        _Player = _player.GetComponent<Player>();
        Realm _StartRealm = _Map.GetRandomRealm();
        _Player.SetStart(_StartRealm);

    }

    // Update is called once per frame
    void Update()
    {

    }
}
