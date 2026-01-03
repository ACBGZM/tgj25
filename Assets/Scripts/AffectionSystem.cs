
using UnityEngine;

public enum ConversationResult : uint
{
    None,
    Good,
    Bad,
}

public class AffectionSystem
{
    public int PlayerLevel { get; private set; } = 3;
    public int NPCLevel { get; private set; } = 3;

    private HandDistanceConfigSO _distanceConfig;

    public void Init(HandDistanceConfigSO config)
    {
        _distanceConfig = config;
        PlayerLevel = 3;
        NPCLevel = 3;
    }

    public int ResolveNPCTurn()
    {
        DistanceProbability prob = _distanceConfig.GetProbability(NPCLevel);
        float roll = Random.Range(0.0f, 1.0f);

        if (roll < prob.approach)
        {
            TryMoveNPC(+1);
            return 1;
        }
        else
        {
            TryMoveNPC(-1);
            return -1;
        }
    }

    public int ResolvePlayerTurn(bool approach)
    {
        int delta = approach ? +1 : -1;

        if (PlayerLevel == 5 && delta > 0
            || PlayerLevel == 1 && delta < 0)
        {
            return 0;
        }

        PlayerLevel = Mathf.Clamp(PlayerLevel + delta, 1, 5);
        return delta;
    }

    public ConversationResult CheckConversationEnded()
    {
        if (PlayerLevel == 1 || NPCLevel == 1)
        {
            return ConversationResult.Bad;
        }

        if (PlayerLevel == 5 && NPCLevel == 5)
        {
            return ConversationResult.Good;
        }

        return ConversationResult.None;
    }

    private void TryMoveNPC(int delta)
    {
        if (NPCLevel == 5
            || NPCLevel == 1 && delta < 0)
        {
            return;
        }

        NPCLevel = Mathf.Clamp(NPCLevel + delta, 1, 5);
    }
}
