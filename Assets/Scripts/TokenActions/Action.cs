using UnityEngine;

public class Action : AbstractAction
{
    public HabilitiesEnum activeHability;
    public HabilitiesEnum targetHability;
    void Start()
    {
        actionName = "Generic action";
    }
    public override void OnActivate(Token active, Token target)
    {
        Debug.Log("Implement me");
    }
}
