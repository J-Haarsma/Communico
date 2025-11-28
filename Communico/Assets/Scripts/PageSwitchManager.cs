using System.Collections.Generic;
using UnityEngine;

public class PageSwitchManager : MonoBehaviour
{
    public static PageSwitchManager Instance { get; private set; }

    [Header("Canvas containing all panels")]
    public Canvas parentCanvas; // Assign your Canvas here

    [Header("Transition")]
    public float fadeDuration = 0.25f;  // optional fade time

    private List<GameObject> pages = new List<GameObject>();
    private int currentIndex = -1;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (parentCanvas == null)
        {
            Debug.LogError("PageSwitchManager: Parent Canvas not assigned!");
            return;
        }

        // Automatically collect all direct child panels
        pages.Clear();
        foreach (Transform child in parentCanvas.transform)
        {
            if (child.gameObject.activeInHierarchy || child.gameObject.GetComponent<CanvasGroup>() != null || child.gameObject.GetComponent<PanelNavigator>() != null)
            {
                pages.Add(child.gameObject);

                // Ensure CanvasGroup exists for future fade/interaction handling
                if (child.GetComponent<CanvasGroup>() == null)
                    child.gameObject.AddComponent<CanvasGroup>();

                // Initially disable all panels
                child.gameObject.SetActive(false);
            }
        }

        // Show first panel automatically
        if (pages.Count > 0)
            ShowPage(0, true);
    }

    public void ShowPage(GameObject page)
    {
        int idx = pages.IndexOf(page);
        if (idx >= 0) ShowPage(idx);
        else Debug.LogWarning("PageSwitchManager: Page not found in canvas children");
    }

    public void ShowPage(int index, bool instant = false)
    {
        if (index < 0 || index >= pages.Count) return;

        // Hide current panel
        if (currentIndex >= 0 && currentIndex < pages.Count)
        {
            var from = pages[currentIndex];
            if (from != null)
                from.SetActive(false);
        }

        // Show new panel
        var to = pages[index];
        if (to != null)
            to.SetActive(true);

        currentIndex = index;
    }

    public void ShowNext()
    {
        int next = currentIndex + 1;
        if (next < pages.Count)
            ShowPage(next);
        else
            Debug.Log("PageSwitchManager: Reached end of panels list");
    }
}
