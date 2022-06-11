using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

enum MainOrSelect
{
    Main,
    Select
}

public class MainUIManager : MonoBehaviour
{
    public static MainUIManager instance;

    [SerializeField] MainOrSelect mainOrSelect = MainOrSelect.Main;
    [SerializeField] string GameStartScene = "Stage 1";

    [SerializeField] GameObject mapPreview;
    [SerializeField] Image mapLocal;
    [SerializeField] Sprite[] mapSprite;

    [SerializeField] int[] stageLife;
    [SerializeField] int[] stageCoin;

    [SerializeField] Text stageLifeText;
    [SerializeField] Text stageCoinText;

    [SerializeField] Text highScore;

    bool isInfMode = false;

    [SerializeField] AudioSource world1SelectSound;
    [SerializeField] AudioSource modeSelectSound;

    [SerializeField] LoadingScreen loading;
    [SerializeField] LoadingScreen loadMap;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;

            return;
        }
        Destroy(gameObject);
    }

    public void GoStageSet()
    {
        loadMap.AnimStart();
        SoundManager.instance.ButtonSound(1);

        Invoke("GoStageSelect", 1.3f);
    }

    void GoStageSelect()
    {
        SceneManager.LoadScene(GameStartScene);
    }

    public void MapPreviewOn(Toggle toggle)
    {
        if (toggle.isOn)
        {
            mapPreview.SetActive(true);
            SoundManager.instance.ButtonSound(0);
            switch (toggle.name)
            {
                case "Stage 1 Button":
                    mapLocal.sprite = mapSprite[0];
                    GameManager.instance.StageSetting(stageLife[0], stageCoin[0]);
                    GameManager.instance.SetWhatStage(0);
                    stageLifeText.text = string.Format("{0}", stageLife[0]);
                    stageCoinText.text = string.Format("{0}", stageCoin[0]);
                    highScore.text = string.Format("Normal Best Time : {0:N2}\nInf Best Wave : {1}", GameManager.instance.GetNormalBestTime(GameManager.instance.GetWhatStage), GameManager.instance.GetInfBestWave(GameManager.instance.GetWhatStage));
                    GameStartScene = "Stage 1";

                    break;
                case "Stage 2 Button":
                    mapLocal.sprite = mapSprite[1];
                    GameManager.instance.StageSetting(stageLife[1], stageCoin[1]);
                    GameManager.instance.SetWhatStage(1);
                    stageLifeText.text = string.Format("{0}", stageLife[1]);
                    stageCoinText.text = string.Format("{0}", stageCoin[1]);
                    highScore.text = string.Format("Normal Best Time : {0:N2}\nInf Best Wave : {1}", GameManager.instance.GetNormalBestTime(GameManager.instance.GetWhatStage), GameManager.instance.GetInfBestWave(GameManager.instance.GetWhatStage));
                    GameStartScene = "Stage 2";

                    break;
                case "Stage 3 Button":
                    mapLocal.sprite = mapSprite[2];
                    GameManager.instance.StageSetting(stageLife[2], stageCoin[2]);
                    GameManager.instance.SetWhatStage(2);
                    stageLifeText.text = string.Format("{0}", stageLife[2]);
                    stageCoinText.text = string.Format("{0}", stageCoin[2]);
                    highScore.text = string.Format("Normal Best Time : {0:N2}\nInf Best Wave : {1}", GameManager.instance.GetNormalBestTime(GameManager.instance.GetWhatStage), GameManager.instance.GetInfBestWave(GameManager.instance.GetWhatStage));
                    GameStartScene = "Stage 3";

                    break;
            }
        }
        else
        {
            mapPreview.SetActive(false);
        }
    }

    public void EnterNormalMode()
    {
        SoundManager.instance.ButtonSound(1);
        GameManager.instance.isInfMode = false;
        loading.AnimStart();

        Invoke("LoadingLevel", 1.3f);
    }
    public void EnterInfiniteMode()
    {
        SoundManager.instance.ButtonSound(1);
        GameManager.instance.isInfMode = true;
        loading.AnimStart();

        //StartCoroutine(LoadingLevel(1.3f));
        Invoke("LoadingLevel", 1.3f);
    }
    public void ReturnMain()
    {
        SoundManager.instance.ButtonSound(3);
        SceneManager.LoadScene("Main Menu");
    }

    public void EnterTuto()
    {
        SoundManager.instance.ButtonSound(1);
        loading.AnimStart();
        Invoke("EnterTutorial", 1.3f);
    }

    void EnterTutorial()
    {
        GameManager.instance.StageSetting(3000, 10000);
        GameManager.instance.ReSetting();
        GameManager.instance.SetWhatStage(-1);
        SceneManager.LoadScene("Tutorial");
    }


    public void ExitGame()
    {
        Application.Quit();
    }

    //IEnumerator LoadingLevel(float time)
    //{
    //    yield return new WaitForSeconds(time);
    //    //yield return null;
    //
    //    int a = 0;
    //    //LoadingLevel();
    //    GameManager.instance.ReSetting();
    //    SceneManager.LoadScene(GameStartScene);
    //}
    void LoadingLevel()
    {
        GameManager.instance.ReSetting();
        SceneManager.LoadScene(GameStartScene);

    }
}
