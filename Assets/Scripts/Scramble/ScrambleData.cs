using System;
using System.Collections.Generic;

[Serializable]
public class ScrambleDef
{
    public string scramble_id;
    public string title;
    public string instruction_en;
    public List<string> tiles;
    public string answer;
    public string hint;
    public RewardDef reward;
    public int time_limit_sec = 0; // 0 = no timer
}

[Serializable]
public class RewardDef
{
    public int grammar = 0;
}
