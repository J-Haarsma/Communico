using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInputUI : MonoBehaviour
{
    public GameObject inputFieldPrefab;
    public Transform inputParent;
    public Button addPlayerButton;
    public Button continueButton;

    private List<InputField> fields = new List<InputField>();
    private int maxPlayers = 5;

    void Start()
    {
        AddPlayerField(); // always start with one
    }

    public void AddPlayerField()
    {
        if (fields.Count >= maxPlayers) return;

        GameObject newField = Instantiate(inputFieldPrefab, inputParent);
        InputField fieldComp = newField.GetComponent<InputField>();
        fields.Add(fieldComp);

        if (fields.Count >= maxPlayers)
            addPlayerButton.gameObject.SetActive(false);
    }

    public void SavePlayersAndContinue()
    {
        List<string> names = new List<string>();

        foreach (var f in fields)
            if (!string.IsNullOrWhiteSpace(f.text))
                names.Add(f.text);

        TeamData.Instance.SetPlayers(names);
        // then call next screen (you already know how)
    }
}
