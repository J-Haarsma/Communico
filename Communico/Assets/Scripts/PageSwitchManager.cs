using System.Collections.Generic;
using UnityEngine;

public class PageSwitchManager : MonoBehaviour
{
    public static PageSwitchManager Instance { get; private set; }

    [Header("Canvas containing all panels")]
    public Canvas parentCanvas;

    [Header("Transition")]
    public float fadeDuration = 0.25f;

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

        // Auto-collect panels
        pages.Clear();
        foreach (Transform child in parentCanvas.transform)
        {
            pages.Add(child.gameObject);

            if (child.GetComponent<CanvasGroup>() == null)
                child.gameObject.AddComponent<CanvasGroup>();

            child.gameObject.SetActive(false);
        }

        if (pages.Count > 0)
            ShowPage(0, true);
    }

    public void ShowPage(GameObject page)
    {
        int idx = pages.IndexOf(page);
        if (idx >= 0)
            ShowPage(idx);
        else
            Debug.LogWarning("PageSwitchManager: Page not found in canvas children");
    }

    public void ShowPage(int index, bool instant = false)
    {
        if (index < 0 || index >= pages.Count)
            return;

        if (currentIndex >= 0 && currentIndex < pages.Count)
            pages[currentIndex].SetActive(false);

        pages[index].SetActive(true);

        currentIndex = index;
    }

    public void ShowNext()
    {
        int next = currentIndex + 1;
        if (next < pages.Count)
            ShowPage(next);
        else
            Debug.Log("PageSwitchManager: End of pages");
    }

    public void ShowPrevious()
    {
        int prev = currentIndex - 1;
        if (prev >= 0)
        {
            ShowPage(prev);
        }
        else
        {
            Debug.Log("PageSwitchManager: Already at first panel");
        }
    }
}
