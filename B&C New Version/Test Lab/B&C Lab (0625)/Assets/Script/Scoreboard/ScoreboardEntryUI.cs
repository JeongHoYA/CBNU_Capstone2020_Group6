using TMPro;
using UnityEngine;

namespace HoYA.Scoreboards
{
    public class ScoreboardEntryUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI entryNameText = null;
        [SerializeField] private TextMeshProUGUI entryScoreText = null;

        public void Init(ScoreboardEntryData scoreboardEntryData)
        {
            entryNameText.text = scoreboardEntryData.entryName.ToString();
            entryScoreText.text = scoreboardEntryData.entryScore.ToString();
        }
    }
}
