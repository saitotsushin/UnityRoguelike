using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using Item;

namespace DataBase
{
    [CreateAssetMenu(fileName = "ItemInstanceData", menuName="CreateItemInstanceData")]
    public class ItemInstanceData : ScriptableObject {
        //　アイテムID
        [SerializeField]
        public int Id;
        //　アイテムのアイコン
        [SerializeField]
        public Sprite TileSprite;
        //　アイテムのアイコン
        [SerializeField]
        public Sprite ItemFieldSprite;
        //　アイテムの名前
        [SerializeField]
        public string ItemName;
        //　アイテムの情報
        [SerializeField]
        public string Information;
        public int GetId() {
            return Id;
        }
    }
}