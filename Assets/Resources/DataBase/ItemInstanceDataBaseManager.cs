using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DataBase
{
    public class ItemInstanceDataBaseManager : MonoBehaviour
    {
        static public ItemInstanceDataBaseManager instance;
        //　アイテムデータベース
        [SerializeField]
        public ItemInstanceDataBase itemDataBase;
        // void Awake ()
        // {
        //     if (instance == null) {
            
        //         instance = this;     
        //     }
        //     else {
        //         Destroy (gameObject);
        //     }
        // }
        public void Create(){
            if (instance == null) {
            
                instance = this;         
            }
            else {
                Destroy (gameObject);
            }
        }
        //　名前でアイテムを取得
        public ItemInstanceData GetItem(int searchId)
        {
            return itemDataBase.GetItemLists().Find(id => id.GetId() == searchId);
        }
    }
}