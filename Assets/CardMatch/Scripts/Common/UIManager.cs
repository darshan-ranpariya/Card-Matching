using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
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
    Coroutine timer;
    private void Awake()
    {
        inst = this;
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

    void ResetGame()
    {
        gameOverPanel.SetActive(false);
        currTime = totalTime;
        currScore = 0;
        Time.timeScale = 1;
        if(timer != null) StopCoroutine(timer);
    }


    #region ClickMethods
    public void StartBtnClick()
    {
        TurnOffAllPanels();
        gridPanel.gameObject.SetActive(true);
    }

    public void StartGame()
    {
        TurnOffAllPanels();
        currScoreTxt.text = currScore.ToString();
        gamePlayPanel.gameObject.SetActive(true);
        timer = StartCoroutine(StartTime());
    }

    public void PauseBtnClick()
    {
        StopCoroutine(timer);
        pausePanel.gameObject.SetActive(true);
        Time.timeScale = 0;
    }

    public void ResumeBtnClick()
    {
        Time.timeScale = 1;
        pausePanel.gameObject.SetActive(false);
        timer = StartCoroutine(StartTime());
    }

    public void HomeBtnClick()
    {
        ResetGame();
        if (pausePanel.gameObject.activeInHierarchy) pausePanel.gameObject.SetActive(false);
        TurnOffAllPanels();
        startPanel.gameObject.SetActive(true);
    }

    public void RestartBtnClick()
    {
        ResetGame();
        StartBtnClick();
    }

    #endregion

    IEnumerator StartTime()
    {
        while (currTime > 0)
        {
            int t = Mathf.CeilToInt(Mathf.Clamp(currTime, 0, totalTime));
            timeString.text = t.ToString();
            yield return new WaitForSeconds(1f);
            currTime -= 1;
        }
        gameOverPanel.SetActive(true);
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
        yield return new WaitForSeconds(delay);
        action?.Invoke();
    }
}
