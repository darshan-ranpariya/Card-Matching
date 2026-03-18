using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GridSizeSwitch : MonoBehaviour
{
    public Button btn;
    public int row, column;

    private TMP_Text text;
    private IGameManager gameManager;
    private UIManager uiManager;

    private void OnValidate()
    {
        btn = GetComponent<Button>();
        text = GetComponentInChildren<TMP_Text>();
        if (text != null)
        {
            text.text = row + " x " + column;
        }
    }

    private void Start()
    {
        gameManager = ItemsHandler.inst;
        uiManager = UIManager.inst;
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
        if (gameManager != null)
        {
            ItemsHandler.inst.OnGridSwitchOn(row, column);
        }

        if (uiManager != null)
        {
            uiManager.StartGame();
        }
    }
}
