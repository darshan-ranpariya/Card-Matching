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
        
        return matchScore;
    }

    public void EndCombo()
    {
        if (!isInCombo) return;

        uiService.UpdateScore(comboScore);
        
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
    }

    private void ShowComboFeedback(int comboCount, int matchScore)
    {
        if (comboCount == 1)
        {
            AudioManager.inst?.PlayMatch();
            uiService.ShowComboText("", false);
        }
        else
        {
            AudioManager.inst?.PlayCombo();
            uiService.ShowComboText($"COMBO x {comboCount} !", true);
        }
    }
}
