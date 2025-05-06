using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class LapTimer : MonoBehaviour
{
    public TMP_Text lapText;
    public int maxLaps = 3;

    private float raceStartTime;
    private int currentLap = 0;
    private List<float> lapTimes = new List<float>();
    private bool finished = false;

    void Start()
    {
        raceStartTime = Time.time;
        UpdateLapDisplay();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (finished || !other.CompareTag("FinishLine"))
            return;

        float currentTime = Time.time - raceStartTime;
        currentLap++;
        lapTimes.Add(currentTime);

        UpdateLapDisplay();

        if (currentLap >= maxLaps)
        {
            finished = true;
            lapText.text += "\n🏁 Finished!";
        }
    }

    void UpdateLapDisplay()
    {
        lapText.text = "";

        for (int i = 0; i < lapTimes.Count; i++)
        {
            string timeFormatted = FormatTime(lapTimes[i]);
            lapText.text += $"Lap {i + 1}: {timeFormatted}\n";
        }
    }

    string FormatTime(float t)
    {
        int minutes = Mathf.FloorToInt(t / 60f);
        float seconds = t % 60f;
        return $"{minutes:00}:{seconds:00.00}";
    }
}
