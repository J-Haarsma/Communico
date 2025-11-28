using System.Collections.Generic;
using UnityEngine;

public class TeamData : MonoBehaviour
{
    public static TeamData Instance;

    public string teamName;
    public List<string> players = new List<string>();
    private Queue<string> instructorQueue = new Queue<string>();

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);   // persists through scene changes
    }

    public void SetPlayers(List<string> names)
    {
        players = names;
        instructorQueue.Clear();

        foreach (string p in players)
            instructorQueue.Enqueue(p);  // add order for rotation
    }

    public string GetNextInstructor()
    {
        if (instructorQueue.Count == 0)
            ResetQueue();

        return instructorQueue.Dequeue();
    }

    private void ResetQueue()
    {
        foreach (string p in players)
            instructorQueue.Enqueue(p);
    }
}