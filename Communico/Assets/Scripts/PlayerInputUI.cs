using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;   // << nodig voor nieuwe Input System
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

        AddPlayerInput(); // eerste veld standaard
        addPlayerButton.onClick.AddListener(AddPlayerInput);
    }

    void Update()
    {
        HandleTabAddPlayer();   // << toegevoegd (Input System versie)
        ValidateContinue();
    }

    // ADD PLAYER INPUT FIELD
    void AddPlayerInput()
    {
        if (playerFields.Count >= maxPlayers)
            return;

        GameObject newFieldObj = Instantiate(playerInputPrefab, playerInputParent);
        InputField input = newFieldObj.GetComponent<InputField>();

        playerFields.Add(input);

        // hide add button if max reached
        if (playerFields.Count >= maxPlayers)
            addPlayerButton.gameObject.SetActive(false);

        // Auto-focus het nieuwe veld
        EventSystem.current.SetSelectedGameObject(newFieldObj);
        input.ActivateInputField();
    }

    // TAB HANDLING (Input System)
    void HandleTabAddPlayer()
    {
        if (Keyboard.current == null)
            return;

        // Kijk of Tab deze frame ingedrukt werd
        if (Keyboard.current.tabKey.wasPressedThisFrame)
        {
            GameObject selected = EventSystem.current.currentSelectedGameObject;

            // Tab werkt alleen als je in een bestaand inputfield zit
            if (selected != null && selected.GetComponent<InputField>() != null)
            {
                AddPlayerInput();
            }
        }
    }

    // CONTINUE BUTTON VALIDATION
    void ValidateContinue()
    {
        // Teamname verplicht
        if (string.IsNullOrWhiteSpace(teamNameField.text))
        {
            continueButton.interactable = false;
            return;
        }

        // Verzamel ingevulde namen
        List<string> validPlayers = new List<string>();
        foreach (var f in playerFields)
            if (!string.IsNullOrWhiteSpace(f.text))
                validPlayers.Add(f.text);

        // Minimaal 3 spelers
        continueButton.interactable = validPlayers.Count >= 3;
    }

    // SAVE & CONTINUE
    public void ConfirmTeamAndContinue()
    {
        List<string> finalNames = new List<string>();
        foreach (var f in playerFields)
            if (!string.IsNullOrWhiteSpace(f.text))
                finalNames.Add(f.text);

        TeamData.Instance.teamName = teamNameField.text;
        TeamData.Instance.RegisterPlayerList(finalNames);

        PageSwitchManager.Instance.ShowNext();
    }
}

