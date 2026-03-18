using UnityEngine;

public class ComboService
{
    private int currentComboCount = 0;
    private int comboScore = 0;
    private bool isInCombo = false;

    private readonly IUIService uiService;

    public ComboService(IUIService uiService)
    {
        this.uiService = uiService;
    }

    public int HandleMatch()
    {
        if (!isInCombo)
        {
            StartCombo();
        }

        currentComboCount++;

        int baseScore = 1;
        int comboBonus = currentComboCount > 1 ? (currentComboCount - 1) : 0;
        int matchScore = baseScore + comboBonus;

        comboScore += matchScore;

        ShowComboFeedback(currentComboCount, matchScore);

        Debug.Log($"Match! Combo: {currentComboCount}, Match Score: {matchScore}, Total Combo Score: {comboScore}");

        return matchScore;
    }

    public void EndCombo()
    {
        if (!isInCombo) return;

        uiService.UpdateScore(comboScore);

        Debug.Log($"Combo Ended! Total pairs: {currentComboCount}, Final score awarded: {comboScore}");

        isInCombo = false;
        currentComboCount = 0;
        comboScore = 0;
        uiService.ShowComboText("", false);
    }

    private void StartCombo()
    {
        isInCombo = true;
        currentComboCount = 0;
        comboScore = 0;
        Debug.Log("Combo Started!");
    }

    private void ShowComboFeedback(int comboCount, int matchScore)
    {
        if (comboCount == 1)
        {
            Debug.Log("First match!");
            AudioManager.inst?.PlayMatch();
            uiService.ShowComboText("", false);
        }
        else
        {
            Debug.Log($"COMBO x{comboCount}! +{matchScore} points");
            AudioManager.inst?.PlayCombo();
            uiService.ShowComboText($"COMBO x {comboCount} !", true);
        }
    }
}
