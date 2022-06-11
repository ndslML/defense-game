using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//적군의 풀링등을 담당하는 매니저
public class EnemyManager : MonoBehaviour
{
    public static EnemyManager instance { get; private set; }

    [SerializeField] GameObject[] enemyPrefab;
    Dictionary<string, Queue<GameObject>> EnemyPool = new Dictionary<string, Queue<GameObject>>();

    public int enemyPrefabNumber = 15;
    // Start is called before the first frame update
    void Awake()
    {
        var enemyKind = enemyPrefab.Length;
        for(int i = 0; i < enemyKind; i++)
        {
            EnemyPool.Add(enemyPrefab[i].name, new Queue<GameObject>());
            for (int j = 0; j < enemyPrefabNumber; j++)
            {
                MakeEnemy(enemyPrefab[i]);
            }
        }

        if (instance == null)
        {
            instance = this;

            return;
        }
        Destroy(gameObject);
    }

    // Update is called once per frame
    public void PathFinishEnemyDistory(float hp)
    {
        GameManager.instance.baseHitPointMinus(hp);
        GameManager.instance.IsPlayerDead();
    }



    void MakeEnemy(GameObject prefab)//실제로 만든다
    {
        if (EnemyPool.ContainsKey(prefab.name)) {
            var enemy = Instantiate(prefab);
            enemy.SetActive(false);
            EnemyPool[prefab.name].Enqueue(enemy);
        }
    }

    public GameObject AddEnemy(int ID,Transform enemySpawner)//딕셔너리 목록에서 디큐로 오브젝트를 뺸다
    {
        if (EnemyPool[enemyPrefab[ID].name].Count <= 0)
        {
            MakeEnemy(enemyPrefab[ID]);
            
        }
        var enemy = EnemyPool[enemyPrefab[ID].name].Dequeue();
        enemy.transform.position = enemySpawner.position;

        enemy.SetActive(true);
        return enemy;
    }

    public void DestoryEnemy(GameObject enemy,int ID)//오브젝트를 삭제하고 삭제한 오브젝트는 딕셔너리에 인큐로 넣는다
    {
        //enemy.transform.position = new Vector2(-100, -100);
        enemy.SetActive(false);
        EnemyPool[enemyPrefab[ID].name].Enqueue(enemy);

        WaveSystem.instance.DestoryEnemy();
    }
}
