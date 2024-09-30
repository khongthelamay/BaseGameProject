using System;
using System.Collections;
using System.Collections.Generic;
using TW.Reactive.CustomComponent;
using TW.Utility.DesignPattern;
using UnityEngine;

public class QuestManager : Singleton<QuestManager>
{
    [field: SerializeField] public List<ReactiveValue<QuestSave>> questSaves { get; set; } = new();

    private void Start()
    {
        LoadData();
    }

    private void LoadData()
    {
        questSaves = InGameDataManager.Instance.InGameData.QuestData.questSaves;
    }

    public ReactiveValue<QuestSave> GetQuestSaveData(int questID) {
        for (int i = 0; i < questSaves.Count; i++)
        {
            if (questSaves[i].Value.id.Value == questID)
                return questSaves[i];
        }

        QuestSave newQuest = new();
        newQuest.id.Value = questID;
        newQuest.progress.Value = 0;
        newQuest.claimed.Value = false;

        ReactiveValue<QuestSave> newQuestSave = new(newQuest);

        questSaves.Add(newQuestSave);

        return newQuestSave;
    }

    public bool IsCanClaim(int questID) {
        for (int i = 0; i < questSaves.Count; i++)
        {
            if (questSaves[i].Value.id.Value == questID)
            {
                if (questSaves[i].Value.claimed.Value == true)
                    return false;
                return QuestGlobalConfig.Instance.IsDoneProgress(questSaves[i].Value);
            }
        }
        return false;
    }

    public void ClaimQuest(int questID) {
        for (int i = 0; i < questSaves.Count; i++)
        {
            if (questSaves[i].Value.id == questID)
                questSaves[i].Value.claimed.Value = true;
        }
        InGameDataManager.Instance.SaveData();
    }
}
