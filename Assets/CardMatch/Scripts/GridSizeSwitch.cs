using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GridSizeSwitch : MonoBehaviour
{
    public Button btn;
    TMP_Text text;
    public int row, column;

    private void OnValidate()
    {
        btn = GetComponent<Button>();
        text = GetComponentInChildren<TMP_Text>();
        text.text = row + " x " + column;
    }

    private void OnEnable()
    {
        btn.onClick.AddListener(BtnClick);
    }

    private void OnDisable()
    {
        btn.onClick.RemoveAllListeners();
    }

    private void BtnClick()
    {
        ItemsHandler.inst.OnGridSwitchOn(row, column);
        UIManager.inst.StartGame();
    }
}
