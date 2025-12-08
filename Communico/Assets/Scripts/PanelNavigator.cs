using UnityEngine;
using UnityEngine.UI;

public class PanelNavigator : MonoBehaviour
{
    [Header("Next panel settings")]
    public GameObject nextPanel;
    public bool autoAdvance = false;
    public float autoDelay = 3f;

    [Header("Button advance")]
    public Button advanceButton;

    [Header("Back button settings")]
    public Button backButton;

    void OnEnable()
    {
        if (autoAdvance)
            Invoke(nameof(GoNext), autoDelay);

        if (advanceButton != null)
            advanceButton.onClick.AddListener(GoNext);

        if (backButton != null)
            backButton.onClick.AddListener(GoBack);
    }

    void OnDisable()
    {
        if (advanceButton != null)
            advanceButton.onClick.RemoveListener(GoNext);

        if (backButton != null)
            backButton.onClick.RemoveListener(GoBack);

        CancelInvoke();
    }

    // public void GoNext()
    // {
    //     if (nextPanel != null)
    //         PageSwitchManager.Instance.ShowPage(nextPanel);
    //     else
    //         PageSwitchManager.Instance.ShowNext();
    // }

    public void GoNext()
{
    if (nextPanel != null)
    {
        Debug.Log($"[PanelNavigator] {gameObject.name} GoNext → explicit nextPanel: {nextPanel.name}");
        PageSwitchManager.Instance.ShowPage(nextPanel);
    }
    else
    {
        Debug.Log($"[PanelNavigator] {gameObject.name} GoNext → using ShowNext()");
        PageSwitchManager.Instance.ShowNext();
    }
}


    public void GoBack()
    {
        PageSwitchManager.Instance.ShowPrevious();
    }
}
