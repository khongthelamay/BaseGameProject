using UnityEngine;
using UnityEngine.UI;

public class MainContentStreak : MainContent<StreakDataConfig>
{
    [SerializeField] ProgressBar progressBar;
    [SerializeField] Slider sliderStreak;
    Vector2 vectorTemp;

    public void ChangeCurrentProgress(float value)
    {
        progressBar.ChangeProgress(value);
    }

    public void SetPositionStreak() {
        sliderStreak.maxValue = slots[slots.Count - 1].slotData.streak;
        for (int i = 0; i < slots.Count; i++)
        {
            sliderStreak.value = slots[i].slotData.streak;
            vectorTemp = sliderStreak.handleRect.transform.localPosition;
            slots[i].GetComponent<RectTransform>().anchoredPosition = vectorTemp;
        }
    }
}
