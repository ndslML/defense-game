using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//UI 를 담당하는 매니저
public class UIManager : MonoBehaviour
{
    public static UIManager instance { get; private set; }

    [SerializeField] GameObject HPBarPrefab;
    [SerializeField] Transform HPBarPanel;
    Dictionary<string, Queue<GameObject>> HPBarPool = new Dictionary<string, Queue<GameObject>>();
    public int BarPoolNumber = 20;

    [SerializeField] Text cost;
    [SerializeField] Text hitPoint;
    [SerializeField] Text waveText;

    [SerializeField] GameObject upgrade;

    [SerializeField] Text pierceLevel;
    [SerializeField] Text pierceCost;
    [SerializeField] Text rangeLevel;
    [SerializeField] Text rangeCost;
    [SerializeField] Text damageLevel;
    [SerializeField] Text damageCost;
    [SerializeField] Text SPLevel;
    [SerializeField] Text SPCost;

    [SerializeField] Text SellCost;

    [SerializeField] GameObject range;
    [SerializeField] SpriteRenderer rangeColor;

    [SerializeField] WaveSystem waveSystem;
    [SerializeField] Image waveFill;

    [SerializeField] GameObject gameClear;
    [SerializeField] Text ClearTime;

    [SerializeField] GameObject gameFail;
    [SerializeField] Text failWave;

    [SerializeField] GameObject pauseMenu;

    [SerializeField] LoadingScreen loading;
    [SerializeField] LoadingScreen reLoad;

    bool isTimerOn = false;
    float time = 0;

    bool isDouble = false;

    // Start is called before the first frame update
    void Awake()
    {
        HPBarPool.Add(HPBarPrefab.name, new Queue<GameObject>());
        for(int i = 0; i < BarPoolNumber; i++)
        {
            MakeBar(HPBarPrefab);
        }

        CheckTime();

        if (null == instance)
        {
            instance = this;

            return;
        }

        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {//현재 가진 재화와 체력을 보여준다.
        if (isTimerOn)
        {
            time += Time.deltaTime;
        }
  
        cost.text = string.Format("{0}", GameManager.instance.getCost());
        hitPoint.text = string.Format("{0}", (int)GameManager.instance.getBaseHitPoint());
        switch (WaveSystem.instance.GetNOI)
        {
            case NormalOrInfinite.Normal:
                waveText.text = string.Format("Time : {0}\nWave {1}/{2}", string.Format("{0:N2}", time), waveSystem.currentWave, waveSystem.MaxWave);
                break;
            default:
                waveText.text = string.Format("Time : {0}\nWave {1}/{2}", string.Format("{0:N2}", Mathf.Max(WaveSystem.instance.GetTimer, 0)), waveSystem.currentWave, waveSystem.MaxWave);
                break;
        }

        if (WaveSystem.instance.currentWave > WaveSystem.instance.MaxWave-1)
        {
            waveFill.fillAmount = 0;
        }
        else if (WaveSystem.instance.currentWave > 0)
        {
            waveFill.fillAmount = EnemySpawner.instance.spawnEnemyCount / (float)WaveSystem.instance.GetNowEnemy();
        }
        

    }

    public void UpgradeOff()//업그레이드창 닫기
    {
        upgrade.SetActive(false);
    }
    public void UpgradeOn(Tower tower)//레벨을 기초로 업그레이드창 토글
    {//레벨 외 다른 정보도 받아야 함
        upgrade.SetActive(true);
        pierceLevel.text = string.Format("Pierce\nLV. {0}", string.Format(tower.GetPierceLevel.ToString()));
        pierceCost.text = string.Format("Cost\n{0}", string.Format(tower.GetPierceCost.ToString()));
        rangeLevel.text = string.Format("range\nLV. {0}", string.Format(tower.GetRangeLevel.ToString()));
        rangeCost.text = string.Format("Cost\n{0}", string.Format(tower.GetRangeCost.ToString()));
        damageLevel.text = string.Format("damage\nLV. {0}", string.Format(tower.GetdamageLevel.ToString()));
        damageCost.text = string.Format("Cost\n{0}", string.Format(tower.GetdamageCost.ToString()));
        SPLevel.text = string.Format("SPecial\nLV. {0}", string.Format(tower.GetSPLevel.ToString()));
        SPCost.text = string.Format("Cost\n{0}", string.Format(tower.GetSPCost.ToString()));
        SellCost.text = string.Format("Sell:{0}", string.Format((tower.GetTotalTowerCost*0.7f).ToString()));
    }

    public void RangeOff()//사거리표시 off
    {
        range.SetActive(false);
    }
    public void RangeOn(float towerRange)//사거리 표시를 튼다
    {
        range.SetActive(true);
        range.transform.localScale = new Vector3(towerRange * 2, towerRange * 2, 0);
    }
    public void RangeOn(float towerRange, Vector3 towerPos)//이미 존재하는 타워의 사거리표시
    {
        range.SetActive(true);
        range.transform.localScale = new Vector3(towerRange * 2, towerRange * 2, 0);
        range.transform.position = towerPos;
    }
    public void RangeMove()//배치할때 사거리를 움직이기 위한 용도
    {
        range.transform.position = GetMousePos();
    }
    public void RangeRed()//배치불가지역
    {
        rangeColor.color = new Color(1, 0.3f, 0.3f, 0.4f);
    }
    public void RangeBlack()//배치가능지역
    {
        rangeColor.color = new Color(0, 0, 0, 0.4f);
    }

    Vector3 GetMousePos()
    {//마우스 위치를 알음
        var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        return mousePos;
    }

    public void MakeBar(GameObject prefab)
    {
        if (HPBarPool.ContainsKey(prefab.name))
        {
            var HPBar = Instantiate(prefab, HPBarPanel);
            HPBar.SetActive(false);
            HPBarPool[prefab.name].Enqueue(HPBar);
        }
    }

    public GameObject AddBar()
    {
        if (HPBarPool[HPBarPrefab.name].Count <= 0)
        {
            MakeBar(HPBarPrefab);
        }
        var HPBar = HPBarPool[HPBarPrefab.name].Dequeue();
        HPBar.SetActive(true);

        return HPBar;
    }

    public void DestoryBar(GameObject HPBar)
    {
        HPBar.SetActive(false);
        HPBarPool[HPBarPrefab.name].Enqueue(HPBar);
    }

    public void TimerOn()
    {
        isTimerOn = true;
    }
    public void TimerOff()
    {
        isTimerOn = false;
    }

    public void GameClear()
    {
        gameClear.SetActive(true);
        ClearTime.text = string.Format("Clear Normal Mode!\n\nTime : {0}", string.Format("{0:N2}", time));
        if(GameManager.instance.GetWhatStage != -1)
        {
            if(time < GameManager.instance.GetNormalBestTime(GameManager.instance.GetWhatStage))
            {
                GameManager.instance.SetNormalBestTime(time, GameManager.instance.GetWhatStage);
                switch (GameManager.instance.GetWhatStage)
                {
                    case 0:
                        PlayerPrefs.SetFloat("Stage1NormalBestTime", time);
                        break;
                    case 1:
                        PlayerPrefs.SetFloat("Stage2NormalBestTime", time);
                        break;
                    case 2:
                        PlayerPrefs.SetFloat("Stage3NormalBestTime", time);
                        break;
                }
            }
        }

    }
    public void GameFail()
    {
        gameFail.SetActive(true);
        TimerOff();
        failWave.text = string.Format("Game Set\n\nWave : {0}", string.Format("{0}", WaveSystem.instance.currentWave));

        if (GameManager.instance.GetWhatStage != -1)
        {
            if (GameManager.instance.isInfMode)
            {
                if (WaveSystem.instance.currentWave > GameManager.instance.GetInfBestWave(GameManager.instance.GetWhatStage))
                {
                    GameManager.instance.SetInfBestWave(WaveSystem.instance.currentWave, GameManager.instance.GetWhatStage);
                    switch (GameManager.instance.GetWhatStage)
                    {
                        case 0:
                            PlayerPrefs.SetInt("Stage1InfBestWave", WaveSystem.instance.currentWave);
                            break;
                        case 1:
                            PlayerPrefs.SetInt("Stage2InfBestWave", WaveSystem.instance.currentWave);
                            break;
                        case 2:
                            PlayerPrefs.SetInt("Stage3InfBestWave", WaveSystem.instance.currentWave);
                            break;
                    }
                }
            }
        }

    }

    public void RetMenu()
    {
        loading.AnimStart();
        SoundManager.instance.ButtonSound(1);
        Time.timeScale = 1;
        Invoke("ReturnMenu", 1.3f);
    }

    void ReturnMenu()
    {
        SceneManager.LoadScene("Stage Select");
    }

    public void ReRetry()
    {
        Time.timeScale = 1;
        reLoad.AnimStart();
        SoundManager.instance.ButtonSound(1);
        Invoke("Retry", 1.3f);
    }
    void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        GameManager.instance.ReSetting();
    }

    public void PauseGame()
    {
        SoundManager.instance.ButtonSound(3);
        pauseMenu.SetActive(true);
        Time.timeScale = 0;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
    }
    public void DoubleTime(Toggle toggle)
    {
        SoundManager.instance.ButtonSound(3);
        if (toggle.isOn)
        {
            isDouble = true;
            CheckTime();
        }
        else
        {
            isDouble = false;
            CheckTime();
        }
    }
    public void CheckTime()
    {
        if (isDouble)
        {
            Time.timeScale = 2;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
        }
        else
        {
            Time.timeScale = 1;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
        }
    }
    public void ResumeGame()
    {
        SoundManager.instance.ButtonSound(2);
        pauseMenu.SetActive(false);
        CheckTime();
    }
}
