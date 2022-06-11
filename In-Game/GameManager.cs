using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//게임 자체를 총괄하는 매니저
public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }

    [SerializeField] int startCost = 1000;//시작금액
    [SerializeField] float currentCost = 1000;//현재금액
    [SerializeField] int startHitPoint = 1000;
    [SerializeField] float baseHitPoint = 1000;//기지 체력

    public bool isInfMode = false;
    int whatStage = 0;
    public int GetWhatStage => whatStage;
    public void SetWhatStage(int i)
    {
        whatStage = i;
    }

    float[] normalBestTime = new float[3];
    public float GetNormalBestTime(int i) => normalBestTime[i];
    public void SetNormalBestTime(float time,int stage)
    {
        normalBestTime[stage] = time;
    }

    int[] infBestWave = new int[3];
    public int GetInfBestWave(int i) => infBestWave[i];
    public void SetInfBestWave(int wave,int stage)
    {
        infBestWave[stage] = wave;
    }

    // Start is called before the first frame update
    void Awake()
    {
        if (null == instance)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            return;
        }

        Destroy(gameObject);
    }

    private void Start()
    {
        currentCost = startCost;
        baseHitPoint = startHitPoint;

        normalBestTime[0] = PlayerPrefs.GetFloat("Stage1NormalBestTime", 999.99f);
        infBestWave[0] = PlayerPrefs.GetInt("Stage1InfBestWave", 0);
        normalBestTime[1] = PlayerPrefs.GetFloat("Stage2NormalBestTime", 999.99f);
        infBestWave[1] = PlayerPrefs.GetInt("Stage2InfBestWave", 0);
        normalBestTime[2] = PlayerPrefs.GetFloat("Stage3NormalBestTime", 999.99f);
        infBestWave[2] = PlayerPrefs.GetInt("Stage3InfBestWave", 0);

    }

    public void IsPlayerDead()
    {
        if (baseHitPoint <= 0)
        {
            baseHitPoint = 0;

            UIManager.instance.GameFail();
        }
    }

    //cost 는 코스트의 증감과 정보를 담당한다.
    public void costPlus(float income)
    {
        currentCost += income;
    }

    public void costMinus(float outcome)
    {
        currentCost -= outcome;
    }

    public float getCost()
    {
        return currentCost;
    }

    public void baseHitPointMinus(float damage)
    {
        baseHitPoint -= damage;
    }

    public float getBaseHitPoint()
    {
        return baseHitPoint;
    }

    public void ReSetting()
    {
        currentCost = startCost;
        baseHitPoint = startHitPoint;
    }
    public void StageSetting(int life,int coin)
    {
        startCost = coin;
        startHitPoint = life;
    }
}
