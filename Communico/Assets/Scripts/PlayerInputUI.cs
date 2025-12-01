using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PlayerInputUI : MonoBehaviour
{
    [Header("UI References")]
    public InputField teamNameField;
    public Transform playerInputParent;
    public GameObject playerInputPrefab;

    public Button addPlayerButton;
    public Button continueButton;

    private List<InputField> playerFields = new List<InputField>();
    private int maxPlayers = 5;

    void Start()
    {
        continueButton.interactable = false;
        AddPlayerInput(); // first field by default
        addPlayerButton.onClick.AddListener(AddPlayerInput);
    }

    void Update()
    {
        ValidateContinue();
    }

    void AddPlayerInput()
    {
        if (playerFields.Count >= maxPlayers) return;

        GameObject newField = Instantiate(playerInputPrefab, playerInputParent);
        InputField input = newField.GetComponent<InputField>();

        playerFields.Add(input);

        if (playerFields.Count >= maxPlayers)
            addPlayerButton.gameObject.SetActive(false);
    }

    void ValidateContinue()
    {
        // Team name must exist
        if (string.IsNullOrWhiteSpace(teamNameField.text)) { continueButton.interactable = false; return; }

        // Collect filled names
        List<string> validPlayers = new List<string>();
        foreach (var f in playerFields)
            if (!string.IsNullOrWhiteSpace(f.text))
                validPlayers.Add(f.text);

        // Must have at least 3 names
        continueButton.interactable = validPlayers.Count >= 3;
    }

    public void ConfirmTeamAndContinue()
    {
        List<string> finalNames = new List<string>();
        foreach (var f in playerFields)
            if (!string.IsNullOrWhiteSpace(f.text))
                finalNames.Add(f.text);

        TeamData.Instance.teamName = teamNameField.text;
        TeamData.Instance.RegisterPlayerList(finalNames); // important!

        PageSwitchManager.Instance.ShowNext(); // now InstructorSelectUI opens
    }
}
