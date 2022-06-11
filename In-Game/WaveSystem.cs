using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//웨이브를 담당한다.

public enum NormalOrInfinite
{
    Normal,
    Inf
}

[System.Serializable]
public struct Wave
{
    public float spawnTime;//현 웨이브 적 생성주기
    public int maxEnemyCount;//현 웨이브 적 등장 수

    public int minEnemyPrefab;//적 소환의 최소 번호수
    public int maxEnemyPrefab;//적 소환의 최대 번호수
}

public class WaveSystem : MonoBehaviour
{
    public static WaveSystem instance { get; private set; }

    [SerializeField] NormalOrInfinite normalOrInfinite;
    public NormalOrInfinite GetNOI => normalOrInfinite;

    [SerializeField] Wave[] waves;
    [SerializeField] EnemySpawner enemySpawner;
    int currentWaveIndex = -1;
    [SerializeField] float timer = 0;
    public float GetTimer => timer;

    [SerializeField] int NowEnemy = 0;
    [SerializeField] int TotalEnemy = 0;

    [SerializeField] GameObject arrow;

    public int currentWave => currentWaveIndex + 1;
    public int MaxWave => waves.Length;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;

            return;
        }

        Destroy(gameObject);
    }

    private void Start()
    {
        if (GameManager.instance.isInfMode == true)
        {
            normalOrInfinite = NormalOrInfinite.Inf;
        }

        if (normalOrInfinite == NormalOrInfinite.Inf)
        {
            waves = new Wave[999];

            for(int i = 0; i < 999; i++)
            {
                if (i == 0)
                {
                    waves[i].spawnTime = 1.0f;
                    waves[i].maxEnemyCount = 10;
                    waves[i].minEnemyPrefab = 0;
                    waves[i].maxEnemyPrefab = 0;
                }
                else if (i == 1)
                {
                    waves[i].spawnTime = 0.8f;
                    waves[i].maxEnemyCount = 20;
                    waves[i].minEnemyPrefab = 0;
                    waves[i].maxEnemyPrefab = 0;
                }
                else if (i == 2)
                {
                    waves[i].spawnTime = 1f;
                    waves[i].maxEnemyCount = 8;
                    waves[i].minEnemyPrefab = 1;
                    waves[i].maxEnemyPrefab = 1;
                }
                else if (i == 3)
                {
                    waves[i].spawnTime = 0.8f;
                    waves[i].maxEnemyCount = 20;
                    waves[i].minEnemyPrefab = 0;
                    waves[i].maxEnemyPrefab = 1;
                }
                else if (i == 4)
                {
                    waves[i].spawnTime = 1.2f;
                    waves[i].maxEnemyCount = 10;
                    waves[i].minEnemyPrefab = 2;
                    waves[i].maxEnemyPrefab = 2;
                }
                else if (i < 10)
                {
                    waves[i].spawnTime = Random.Range(0.8f, 1.5f);
                    waves[i].maxEnemyCount = Random.Range(12,18);
                    waves[i].minEnemyPrefab = 0;
                    waves[i].maxEnemyPrefab = 1;
                }
                else if (i < 15)
                {
                    waves[i].spawnTime = Random.Range(0.5f, 1.2f);
                    waves[i].maxEnemyCount = Random.Range(20, 30);
                    waves[i].minEnemyPrefab = 1;
                    waves[i].maxEnemyPrefab = 2;
                }
                else if (i < 30)
                {
                    waves[i].spawnTime = Random.Range(0.5f, 1.2f);
                    waves[i].maxEnemyCount = Random.Range(20, 30);
                    waves[i].minEnemyPrefab = 0;
                    waves[i].maxEnemyPrefab = 2;
                }
                else if(i < 50)
                {
                    waves[i].spawnTime = Random.Range(0.2f, 0.5f);
                    waves[i].maxEnemyCount = Random.Range(30, 100);
                    waves[i].minEnemyPrefab = 0;
                    waves[i].maxEnemyPrefab = 2;
                }
                else
                {
                    waves[i].spawnTime = Random.Range(0.05f, 0.2f);
                    waves[i].maxEnemyCount = Random.Range(50, 100);
                    waves[i].minEnemyPrefab = 0;
                    waves[i].maxEnemyPrefab = 2;
                }
            }
        }
        for (int i = 0; i < waves.Length; i++)
        {
            TotalEnemy += waves[i].maxEnemyCount;
        }
    }

    private void FixedUpdate()
    {
        timer -= Time.deltaTime;

        if (TotalEnemy <= 0 && currentWaveIndex < waves.Length && GameManager.instance.getBaseHitPoint() > 0) 
        {
            UIManager.instance.TimerOff();
            UIManager.instance.GameClear();
        }

        if (normalOrInfinite == NormalOrInfinite.Inf)
        {
            if (timer < 0 && currentWave > 0 && GameManager.instance.getBaseHitPoint() > 0)
            {
                StartWave();
            }
        }
    }

    public void StartWave()//버튼을 누르면 웨이브를 시작하게 한다.
    {
        if (NowEnemy == EnemySpawner.instance.spawnEnemyCount && currentWaveIndex < waves.Length - 1 && GameManager.instance.getBaseHitPoint() > 0)
        {
            if (currentWave < 30)
            {
                timer = 30;
            }
            else
            {
                timer = 100;
            }

            SoundManager.instance.ButtonSound(2);
            UIManager.instance.TimerOn();
            currentWaveIndex++;
            enemySpawner.StartWave(waves[currentWaveIndex]);
            NowEnemy = waves[currentWaveIndex].maxEnemyCount;
        }
    }

    public void TutorialStartWave()//버튼을 누르면 웨이브를 시작하게 한다.
    {
        if (NowEnemy == EnemySpawner.instance.spawnEnemyCount && currentWaveIndex < waves.Length - 1 && GameManager.instance.getBaseHitPoint() > 0)
        {
            if (currentWave < 30)
            {
                timer = 30;
            }
            else
            {
                timer = 100;
            }

            SoundManager.instance.ButtonSound(2);
            UIManager.instance.TimerOn();
            arrow.SetActive(false);
            currentWaveIndex++;
            enemySpawner.StartWave(waves[currentWaveIndex]);
            NowEnemy = waves[currentWaveIndex].maxEnemyCount;
        }
    }

    public void DestoryEnemy()
    {
        TotalEnemy--;
    }

    public int GetNowEnemy()
    {
        return NowEnemy;
    }
}
