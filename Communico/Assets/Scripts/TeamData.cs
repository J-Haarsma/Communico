using System.Collections.Generic;
using UnityEngine;

public class TeamData : MonoBehaviour
{
    public static TeamData Instance;

    public string teamName;
    public List<string> players = new List<string>();

    // Tracks instructor usage count
    public Dictionary<string, int> instructorUsage = new Dictionary<string, int>();

    public string currentInstructor = null;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    // NEW — replaces SetPlayers()
    public void RegisterPlayerList(List<string> playerList)
    {
        players = new List<string>(playerList);
        instructorUsage.Clear();

        foreach (string p in players)
            instructorUsage[p] = 0;
    }

    public void SelectInstructor(string playerName)
    {
        currentInstructor = playerName;
    }

    public void ConfirmInstructor()
    {
        instructorUsage[currentInstructor]++;

        // Check if all have instructed once reset cycle
        bool allUsed = true;
        foreach (var p in instructorUsage)
            if (p.Value == 0) allUsed = false;

        if (allUsed)
            ResetInstructorCycle();
    }

    public void ResetInstructorCycle()
    {
        foreach (var key in players)
            instructorUsage[key] = 0;
    }
}
