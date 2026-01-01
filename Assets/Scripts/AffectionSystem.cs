
using UnityEngine;

public class AffectionSystem
{
    public int affection = 20;

    public bool ApplyDelta(int delta)
    {
        affection += delta;
        Debug.Log($"好感度变化：{delta}，当前好感度：{affection}");
        return false;
    }
}
