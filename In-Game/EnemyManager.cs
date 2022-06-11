using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//������ Ǯ������ ����ϴ� �Ŵ���
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



    void MakeEnemy(GameObject prefab)//������ �����
    {
        if (EnemyPool.ContainsKey(prefab.name)) {
            var enemy = Instantiate(prefab);
            enemy.SetActive(false);
            EnemyPool[prefab.name].Enqueue(enemy);
        }
    }

    public GameObject AddEnemy(int ID,Transform enemySpawner)//��ųʸ� ��Ͽ��� ��ť�� ������Ʈ�� �A��
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

    public void DestoryEnemy(GameObject enemy,int ID)//������Ʈ�� �����ϰ� ������ ������Ʈ�� ��ųʸ��� ��ť�� �ִ´�
    {
        //enemy.transform.position = new Vector2(-100, -100);
        enemy.SetActive(false);
        EnemyPool[enemyPrefab[ID].name].Enqueue(enemy);

        WaveSystem.instance.DestoryEnemy();
    }
}
