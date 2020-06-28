using System.IO;
using UnityEngine;


public class Scoreboard : MonoBehaviour
{
    [SerializeField] private int maxScoreboardEntries = 10;                                             // 스코어보드엔트리 크기
    [SerializeField] private Transform highsocresHolderTransform = null;                                // 하이스코어보드 위치
    [SerializeField] private GameObject scoreboardEntryObject = null;                                   // 스코어보드엔트리 오브젝트

    public string SavePath = null;                                                                      // 저장위치

    private void Start()
    {
        ScoreboardSaveData savedScores = GetSavedScores();

        UpdateUI(savedScores);

        SaveScores(savedScores);
    }


    private ScoreboardSaveData GetSavedScores()
    {
        if (!File.Exists(SavePath))
        {
            File.Create(SavePath).Dispose();
            return new ScoreboardSaveData();
        } // 파일이 존재하지 않으면 새 파일 생성 및 스코어보드 반환

        using (StreamReader stream = new StreamReader(SavePath))
        {
            string json = stream.ReadToEnd();

            return JsonUtility.FromJson<ScoreboardSaveData>(json) != null ? JsonUtility.FromJson<ScoreboardSaveData>(json) : new ScoreboardSaveData();
        } //파일이 존재하면 파일 내 스코어보드 반환
    }

    private void SaveScores(ScoreboardSaveData scoreboardSaveData)
    {
        using (StreamWriter stream = new StreamWriter(SavePath))
        {
            string json = JsonUtility.ToJson(scoreboardSaveData, true);
            stream.Write(json);
        }
    }

    private void UpdateUI(ScoreboardSaveData savedScores)
    {
        foreach (Transform child in highsocresHolderTransform)
        {
            Destroy(child.gameObject);
        }

        foreach (ScoreboardEntryData highsocre in savedScores.highscores)
        {
            Instantiate(scoreboardEntryObject, highsocresHolderTransform).GetComponent<ScoreboardEntryUI>().Init(highsocre);
        }
    }

    public void AddEntry(ScoreboardEntryData scoreboardEntryData)
    {
        ScoreboardSaveData savedScores = GetSavedScores();

        bool scoreAdded = false;

        for (int i = 0; i < savedScores.highscores.Count; i++)
        {
            if (scoreboardEntryData.entryScore.CompareTo(savedScores.highscores[i].entryScore) < 0)
            {
                savedScores.highscores.Insert(i, scoreboardEntryData);
                scoreAdded = true;
                break;
            }
        }

        if (!scoreAdded && savedScores.highscores.Count < maxScoreboardEntries)
        {
            savedScores.highscores.Add(scoreboardEntryData);
        }
        if (savedScores.highscores.Count > maxScoreboardEntries)
        {
            savedScores.highscores.RemoveRange(maxScoreboardEntries, savedScores.highscores.Count - maxScoreboardEntries);
        }

        UpdateUI(savedScores);

        SaveScores(savedScores);
    }

    public void ShowEntry()
    {
        ScoreboardSaveData savedScores = GetSavedScores();

        UpdateUI(savedScores);
    }
}
