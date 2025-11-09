using System;
using System.Collections.Generic;

[Serializable]
public class DialogueFile
{
    public string npcId;
    public List<DialogueNode> lines;
}

[Serializable]
public class DialogueNode
{
    public string id;
    public string speaker;         // "Yi Yi" หรือชื่อ NPC
    public string text_en;         // รองรับแท็ก <v-aux> <v> <n-pl> <qword>
    public string text_th;
    public DialogueChoice[] choices;
    public DialogueExpectInput expect_input; // ถ้ามี จุดตรวจประโยค
    public string on_correct_next;
    public string on_wrong_hint;   // ข้อความ hint สั้นๆ
    public bool end;               // จบบทสนทนา
}

[Serializable]
public class DialogueChoice
{
    public string text_en;
    public string next;            // id ของโหนดถัดไป
}

[Serializable]
public class DialogueExpectInput
{
    public string type;            // "sentence_check"
    public string answer;          // "Do you know where Sulupapa Cave is?"
}
