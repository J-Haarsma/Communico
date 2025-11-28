using UnityEngine;
using UnityEngine.UI;

public class InstructorSelectUI : MonoBehaviour
{
    public GameObject buttonPrefab;
    public Transform buttonParent;

    void OnEnable()
    {
        CreateInstructorButtons();
    }

    void CreateInstructorButtons()
    {
        foreach (Transform child in buttonParent)
            Destroy(child.gameObject);

        foreach (var player in TeamData.Instance.players)
        {
            GameObject newButton = Instantiate(buttonPrefab, buttonParent);
            newButton.GetComponentInChildren<Text>().text = player;

            newButton.GetComponent<Button>().onClick.AddListener(() =>
            {
                string instructor = TeamData.Instance.GetNextInstructor();
                Debug.Log("Selected Instructor: " + instructor);

                // CALL PAGE MANAGER TO GO NEXT
                PageSwitchManager.Instance.ShowNext();
            });
        }
    }
}
