using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour, IUIService
{
    public static UIManager inst;
    [Header("Panels")]
    public GameObject startPanel;
    public GameObject gridPanel;
    public GameObject gamePlayPanel;
    public GameObject gameOverPanel;
    public GameObject pausePanel;
    public GameObject winPanel;
    [Header("UI Elements")]
    public int currScore;
    public float totalTime;
    internal float currTime;
    public TMP_Text currScoreTxt;
    public TMP_Text comboTxt;
    [Header("Others")]
    public TMP_Text timeString;

    private Coroutine timer;
    private WaitForSeconds oneSecondWait;
    private readonly Dictionary<float, WaitForSeconds> waitCache = new Dictionary<float, WaitForSeconds>();

    private void Awake()
    {
        inst = this;
        oneSecondWait = new WaitForSeconds(1f);
    }

    private void OnEnable()
    {
        currScore = 0;
    }

    void Start()
    {
        if(PlayerPrefs.GetInt(Utility.StageDataSavePrefKey, 0) != 0)
        {
            DelayedAction(ItemsHandler.inst.LoadGameSavedData, Time.deltaTime * 2);
        }
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            if (gamePlayPanel.activeInHierarchy && !pausePanel.activeInHierarchy)
            {
                ItemsHandler.inst.SaveStageData();
            }
        }
    }

    private void OnApplicationQuit()
    {
        if (gamePlayPanel.activeInHierarchy && !pausePanel.activeInHierarchy)
        {
            ItemsHandler.inst.SaveStageData();
        }
    }

    private void ResetGame()
    {
        gameOverPanel.SetActive(false);
        currTime = totalTime;
        currScore = 0;
        Time.timeScale = 1;
        if (timer != null)
        {
            StopCoroutine(timer);
            timer = null;
        }
    }


    #region ClickMethods
    public void StartBtnClick()
    {
        AudioManager.inst?.PlayButtonClick();
        TurnOffAllPanels();
        gridPanel.gameObject.SetActive(true);
    }

    public void StartGame()
    {
        AudioManager.inst?.PlayButtonClick();
        TurnOffAllPanels();
        currScoreTxt.text = currScore.ToString();
        gamePlayPanel.gameObject.SetActive(true);
        timer = StartCoroutine(StartTime());
    }

    public void PauseBtnClick()
    {
        AudioManager.inst?.PlayButtonClick();
        if (timer != null)
        {
            StopCoroutine(timer);
            timer = null;
        }
        pausePanel.SetActive(true);
        Time.timeScale = 0;
    }

    public void ResumeBtnClick()
    {
        AudioManager.inst?.PlayButtonClick();
        Time.timeScale = 1;
        pausePanel.SetActive(false);
        timer = StartCoroutine(StartTime());
    }

    public void HomeBtnClick()
    {
        AudioManager.inst?.PlayButtonClick();
        ResetGame();
        if (pausePanel.gameObject.activeInHierarchy) pausePanel.gameObject.SetActive(false);
        TurnOffAllPanels();
        startPanel.gameObject.SetActive(true);
    }

    public void RestartBtnClick()
    {
        AudioManager.inst?.PlayButtonClick();
        ResetGame();
        StartBtnClick();
    }

    #endregion

    private IEnumerator StartTime()
    {
        while (currTime > 0)
        {
            int t = Mathf.CeilToInt(Mathf.Clamp(currTime, 0, totalTime));
            timeString.text = t.ToString();
            yield return oneSecondWait;
            currTime -= 1;
        }
        AudioManager.inst?.PlayGameOver();
        gameOverPanel.SetActive(true);
        timer = null;
    }

    public void UpdateScore(int score)
    {
        currScore += score;
        currScore = currScore >= 0 ? currScore : 0;
        currScoreTxt.text = currScore.ToString();
    }

    public void TurnOffAllPanels()
    {
        startPanel.SetActive(false);
        gridPanel.SetActive(false);
        gamePlayPanel.SetActive(false);
        gameOverPanel.SetActive(false);
        pausePanel.SetActive(false);
        winPanel.SetActive(false);
    }

    public void DelayedAction(Action action, float delay)
    {
        StartCoroutine(InvokeAfterDelay(action, delay));
    }

    private IEnumerator InvokeAfterDelay(Action action, float delay)
    {
        if (!waitCache.TryGetValue(delay, out WaitForSeconds wait))
        {
            wait = new WaitForSeconds(delay);
            waitCache[delay] = wait;
        }

        yield return wait;
        action?.Invoke();
    }

    public void ShowComboText(string text, bool isActive)
    {
        comboTxt.text = text;
        comboTxt.gameObject.SetActive(isActive);
    }

    public void ShowWinPanel()
    {
        DelayedAction(() =>
        {
            TurnOffAllPanels();
            winPanel.gameObject.SetActive(true);
        }, 0.5f);
    }
}
