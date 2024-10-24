using System.Collections;
using System.Collections.Generic;
using TW.Utility.DesignPattern;
using UnityEngine;

public class RecruitManager : Singleton<RecruitManager>
{
    public List<RecruitReward> recruitRewards = new();
    public int currentTurn;

    public void InitData(int recruitTotal) {

        recruitRewards.Clear();

        RecruitReward newRecruitReward = new();
        newRecruitReward.type = RecruitRewardType.Hero;
        newRecruitReward.amount = recruitTotal;

        recruitRewards.Add(newRecruitReward);
        currentTurn++;
    }
}
