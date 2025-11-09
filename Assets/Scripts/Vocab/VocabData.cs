using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class VocabSet
{
    public string mob_id;
    public int time_limit_sec = 20;
    public int win_requirement = 3;
    public List<VocabCard> cards;
    public RewardDef reward;
}
[Serializable]
public class VocabCard
{
    public string id;
    public string word;
    public string pos;
    public string th;
    public string imagePath; // Resources path
    public string audioPath; // Resources path
}
