using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//적 소환을 담당한다.

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner instance { get; private set; }

    [SerializeField] GameObject enemyHPSliderPrefab;
    [SerializeField] RectTransform canvasTransform;

    [SerializeField] int ID;
    [SerializeField] float spawnTime;
    [SerializeField] Transform[] wayPoint;

    public int spawnEnemyCount = 0;

    Wave currentWave;

    // Start is called before the first frame update

    private void Start()
    {
        if (instance == null)
        {
            instance = this;

            return;
        }
        Destroy(gameObject);
    }

    public void StartWave(Wave wave)
    {
        currentWave = wave;

        StartCoroutine("SpawnEnemy");
    }
    //웨이브가 시작돠면 적을 소환한다. 현재 웨이브 수도 갱신한다.

    IEnumerator SpawnEnemy()
    {
        spawnEnemyCount = 0;//현재 소환한 적 수

        while (spawnEnemyCount < currentWave.maxEnemyCount)//웨이브 소환 적수보다 작으면 
        {
            int enemyIndex = Random.Range(currentWave.minEnemyPrefab, currentWave.maxEnemyPrefab + 1);
            var spawnEnemy = EnemyManager.instance.AddEnemy(enemyIndex, transform);
            var enemyPathFinder = spawnEnemy.GetComponent<Enemy>();
            //나올 적을 최소 ID 와 최대 ID 사이에서 결정해서 소환한다.

            SpawnEnemyHPSlider(spawnEnemy);

            enemyPathFinder.Setup(wayPoint);
            //적은 지정된 점을 향해 나아간다.
            spawnEnemyCount++;

            yield return new WaitForSeconds(currentWave.spawnTime);
            //웨이브의 소환 주기만큼 기다린다.
        }

    }

    void SpawnEnemyHPSlider(GameObject enemy)
    {
        GameObject sliderClone = UIManager.instance.AddBar();

        //sliderClone.transform.SetParent(canvasTransform);

        sliderClone.transform.localScale = Vector3.one;

        if (enemy)
        {
            sliderClone.GetComponent<HPSliderPositionAuto>().Setup(enemy.transform, canvasTransform, enemy.GetComponent<Enemy>());

            sliderClone.GetComponent<EnemyHPViewer>().Setup(enemy.GetComponent<Enemy>());
        }

    }
}
