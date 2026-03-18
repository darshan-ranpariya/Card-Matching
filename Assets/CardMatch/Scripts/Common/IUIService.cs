using System;

public interface IUIService
{
    void UpdateScore(int score);
    void ShowComboText(string text, bool isActive);
    void DelayedAction(Action action, float delay);
    void ShowWinPanel();
}
