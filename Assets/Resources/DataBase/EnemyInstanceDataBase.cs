using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DataBase{
    [CreateAssetMenu(fileName = "EnemyInstanceDataBase", menuName="CreateEnemyInstanceDataBase")]
    public class EnemyInstanceDataBase : ScriptableObject {
    
        [SerializeField]
        public List<EnemyInstanceData> enemyLists = new List<EnemyInstanceData>();
    
        //　アイテムリストを返す
        public List<EnemyInstanceData> GetItemLists() {
            return enemyLists;
        }
    }
}
