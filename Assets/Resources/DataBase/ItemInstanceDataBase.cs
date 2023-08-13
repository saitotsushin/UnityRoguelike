using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DataBase{
    [CreateAssetMenu(fileName = "ItemInstanceDataBase", menuName="CreateItemInstanceDataBase")]
    public class ItemInstanceDataBase : ScriptableObject {
    
        [SerializeField]
        public List<ItemInstanceData> itemLists = new List<ItemInstanceData>();
    
        //　アイテムリストを返す
        public List<ItemInstanceData> GetItemLists() {
            return itemLists;
        }
    }
}
