using UnityEngine;

public class Action : AbstractAction
{
    void Start()
    {
        actionName = "Generic action";
    }
    public override void OnClick()
    {
        Debug.Log("Implement me");
    }
}
