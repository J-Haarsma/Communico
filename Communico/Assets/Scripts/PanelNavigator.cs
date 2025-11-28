using UnityEngine;
using UnityEngine.UI;

public class PanelNavigator : MonoBehaviour
{
    [Header("Next panel settings")]
    public GameObject nextPanel;     // optional – manually chosen next screen
    public bool autoAdvance = false;
    public float autoDelay = 3f;

    [Header("Button advance")]
    public Button advanceButton;   // optional button to go next

    void OnEnable()
    {
        if (autoAdvance)
            Invoke(nameof(GoNext), autoDelay);

        if (advanceButton != null)
            advanceButton.onClick.AddListener(GoNext);
    }

    void OnDisable()
    {
        if (advanceButton != null)
            advanceButton.onClick.RemoveListener(GoNext);

        CancelInvoke();
    }

    public void GoNext()
    {
        if (nextPanel != null)
            PageSwitchManager.Instance.ShowPage(nextPanel);
        else
            PageSwitchManager.Instance.ShowNext();
    }
}
