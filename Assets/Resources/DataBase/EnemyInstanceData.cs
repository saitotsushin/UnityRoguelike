using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DataBase
{
    [CreateAssetMenu(fileName = "EnemyInstanceData", menuName="CreateEnemyInstanceData")]
    public class EnemyInstanceData : ScriptableObject {
        //　アイテムID
        [SerializeField]
        public int Id;
        //　アイテムのアイコン
        [SerializeField]
        public Sprite TileSprite;
        //　アイテムの名前
        [SerializeField]
        public string EnemyName;
        [SerializeField]
        public int HP;
        [SerializeField]
        public int AK;
        [SerializeField]
        public int DF;
        //　アイテムの情報
        [SerializeField]
        public string Information;
        public int GetId() {
            return Id;
        }
    }
}