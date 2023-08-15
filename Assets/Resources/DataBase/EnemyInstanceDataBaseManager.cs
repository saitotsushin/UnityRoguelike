using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DataBase
{
    public class EnemyInstanceDataBaseManager : MonoBehaviour
    {
        static public EnemyInstanceDataBaseManager instance;
        //　アイテムデータベース
        [SerializeField]
        public EnemyInstanceDataBase enemyDataBase;
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
        public EnemyInstanceData GetItem(int searchId)
        {
            return enemyDataBase.GetItemLists().Find(id => id.GetId() == searchId);
        }
    }
}
