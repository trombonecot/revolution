using UnityEngine;

public abstract class AbstractAction : MonoBehaviour
{
    public string actionName;

    public abstract void OnClick();
}
