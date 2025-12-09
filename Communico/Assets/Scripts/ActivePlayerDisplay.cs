using UnityEngine;
using TMPro; // Using TextMeshPro

public class ActivePlayerDisplay : MonoBehaviour
{
    [Header("UI Reference")]
    public TMP_Text displayText;    // Assign your TMP_Text here

    [Header("Optional Prefix")]
    public string prefix = "Instructor: "; // Text before name

    void OnEnable()
    {
        UpdateDisplay();
    }

    public void UpdateDisplay()
    {
        if (displayText == null)
        {
            Debug.LogWarning($"ActivePlayerDisplay: No TMP_Text assigned on {gameObject.name}");
            return;
        }

        if (TeamData.Instance == null || string.IsNullOrEmpty(TeamData.Instance.currentInstructor))
        {
            displayText.text = prefix + "???"; // or leave empty
            return;
        }

        displayText.text = prefix + TeamData.Instance.currentInstructor;
    }
}

