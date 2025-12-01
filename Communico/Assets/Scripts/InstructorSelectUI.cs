using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class InstructorSelectUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject playerButtonPrefab;      // prefab with Button + TMP_Text
    public Transform playerButtonParent;
    public Button continueButton;

    private Dictionary<string, Button> buttonMap = new Dictionary<string, Button>();
    private string selectedPlayer = null;

    void OnEnable()
    {
        if (continueButton != null)
            continueButton.interactable = false;

        GeneratePlayerButtons();
    }

    void OnDisable()
    {
        // cleanup listeners
        foreach (var kvp in buttonMap)
            if (kvp.Value != null) kvp.Value.onClick.RemoveAllListeners();

        buttonMap.Clear();
        selectedPlayer = null;
    }

    void GeneratePlayerButtons()
    {
        // Clear old buttons
        foreach (Transform child in playerButtonParent)
            Destroy(child.gameObject);

        buttonMap.Clear();

        // Defensive: ensure players exist
        if (TeamData.Instance == null || TeamData.Instance.players == null || TeamData.Instance.players.Count == 0)
            return;

        foreach (string player in TeamData.Instance.players)
        {
            GameObject btnObj = Instantiate(playerButtonPrefab, playerButtonParent);
            Button btn = btnObj.GetComponent<Button>();
            TMP_Text txt = btnObj.GetComponentInChildren<TMP_Text>();

            if (txt != null)
                txt.text = player; // set the TMP text to the player's name

            buttonMap[player] = btn;

            // disable if player has already been instructor in current cycle
            bool usedBefore = TeamData.Instance.instructorUsage.ContainsKey(player) &&
                              TeamData.Instance.instructorUsage[player] > 0;

            btn.interactable = !usedBefore;

            // closure capture
            string capturedPlayer = player;
            btn.onClick.AddListener(() => SelectPlayer(capturedPlayer));
        }
    }

    void SelectPlayer(string player)
    {
        // Deselect previous selection
        if (selectedPlayer != null && buttonMap.ContainsKey(selectedPlayer))
            Highlight(buttonMap[selectedPlayer], false);

        selectedPlayer = player;
        TeamData.Instance.SelectInstructor(player);

        if (buttonMap.ContainsKey(player))
            Highlight(buttonMap[player], true);

        if (continueButton != null)
            continueButton.interactable = true;
    }

    // Call this from Continue button OnClick
    public void ConfirmInstructorAndContinue()
    {
        if (TeamData.Instance == null)
            return;

        if (string.IsNullOrEmpty(TeamData.Instance.currentInstructor))
            return;

        TeamData.Instance.ConfirmInstructor();
        PageSwitchManager.Instance.ShowNext();
    }

    void Highlight(Button btn, bool active)
    {
        if (btn == null) return;

        ColorBlock colors = btn.colors;
        colors.normalColor = active ? new Color(1f, 0.75f, 0.2f) : Color.white; // gold highlight
        btn.colors = colors;
    }
}


