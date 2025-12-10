using UnityEngine;

public class PanelAudioManager : MonoBehaviour
{
    [System.Serializable]
    public class AudioPanelGroup
    {
        public string name;
        public AudioSource audioSource;
        public GameObject[] panelsToWatch;
    }

    public AudioPanelGroup[] audioGroups;

    void Update()
    {
        foreach (var group in audioGroups)
        {
            bool shouldPlay = false;

            // Check if any of this group's panels are active
            foreach (GameObject panel in group.panelsToWatch)
            {
                if (panel != null && panel.activeInHierarchy)
                {
                    shouldPlay = true;
                    break;
                }
            }

            // Play or stop the audio source
            if (shouldPlay)
            {
                if (!group.audioSource.isPlaying)
                    group.audioSource.Play();
            }
            else
            {
                if (group.audioSource.isPlaying)
                    group.audioSource.Stop();
            }
        }
    }
}
