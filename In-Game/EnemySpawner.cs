using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�� ��ȯ�� ����Ѵ�.

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
    //���̺갡 ���۵¸� ���� ��ȯ�Ѵ�. ���� ���̺� ���� �����Ѵ�.

    IEnumerator SpawnEnemy()
    {
        spawnEnemyCount = 0;//���� ��ȯ�� �� ��

        while (spawnEnemyCount < currentWave.maxEnemyCount)//���̺� ��ȯ �������� ������ 
        {
            int enemyIndex = Random.Range(currentWave.minEnemyPrefab, currentWave.maxEnemyPrefab + 1);
            var spawnEnemy = EnemyManager.instance.AddEnemy(enemyIndex, transform);
            var enemyPathFinder = spawnEnemy.GetComponent<Enemy>();
            //���� ���� �ּ� ID �� �ִ� ID ���̿��� �����ؼ� ��ȯ�Ѵ�.

            SpawnEnemyHPSlider(spawnEnemy);

            enemyPathFinder.Setup(wayPoint);
            //���� ������ ���� ���� ���ư���.
            spawnEnemyCount++;

            yield return new WaitForSeconds(currentWave.spawnTime);
            //���̺��� ��ȯ �ֱ⸸ŭ ��ٸ���.
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
