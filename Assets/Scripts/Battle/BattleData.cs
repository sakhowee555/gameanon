using System;
using System.Collections.Generic;

[Serializable]
public class BattleDef
{
    public string enemy;
    public int enemyHP = 5;
    public int playerHP = 3;
    public int timePerTurn = 15;
    public List<BattleTurn> turns;
    public BattleReward reward;
}
[Serializable]
public class BattleTurn
{
    public string type;        // "scramble" | "choice" | "fill" | "drag_fill"
    public string question;    // สำหรับ choice
    public List<string> tiles; // สำหรับ scramble
    public string answer;      // สำหรับ scramble/fill (เช็คแบบ normalize)
    public string template;    // สำหรับ fill/drag_fill (แสดงประโยคมีช่องว่าง)
    public List<string> answers; // คำตอบที่ต้องเติมตามลำดับ
    public List<string> options;  // สำหรับ choice
    public int answerIndex;       // สำหรับ choice
    public List<string> bank;     // สำหรับ drag_fill (word bank)
    public string hint;
}
[Serializable]
public class BattleReward
{
    public int xp = 0;
}
