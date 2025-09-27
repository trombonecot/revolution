using UnityEngine;

public static class RangeCircleUtils
{
    private const float circleThickness = 0.02f;

    public static Material rangeMat;


    /// <summary>
    /// Shows or creates a move range circle around a token.
    /// Returns the circle GameObject (caller should keep a reference to hide later).
    /// </summary>
    public static GameObject ShowRangeCircle(Token token, float radius)
    {
        if (rangeMat == null)
        {
            // load your custom material once
            rangeMat = Resources.Load<Material>("Materials/RangeCircleMat");
            if (rangeMat == null)
            {
                Debug.LogError("RangeCircleUtils: Could not load material at Resources/Materials/RangeCircleMat.mat");
            }
        }

        if (token == null) return null;

        GameObject circle = token.transform.Find("MoveRangeCircle")?.gameObject;

        if (circle == null)
        {
            circle = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            circle.name = "MoveRangeCircle";
            circle.transform.SetParent(token.transform, worldPositionStays: false);

            // Disable collisions
            var col = circle.GetComponent<Collider>();
            if (col != null) col.enabled = false;

            var mr = circle.GetComponent<MeshRenderer>();
            mr.material = rangeMat;

            //mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            //mr.receiveShadows = false;
        }

        // Position: bottom of token’s bounds
        Renderer[] rends = token.GetComponentsInChildren<Renderer>();
        if (rends.Length > 0)
        {
            Bounds b = rends[0].bounds;
            for (int i = 1; i < rends.Length; i++) b.Encapsulate(rends[i].bounds);

            float bottomWorldY = b.min.y;
            float localYOffset = bottomWorldY - token.transform.position.y + 0.01f - 0.5f;

            circle.transform.localPosition = new Vector3(0f, localYOffset, 0f);
        }
        else
        {
            circle.transform.localPosition = new Vector3(0f, -0.5f + 0.01f, 0f);
        }

        circle.transform.localRotation = Quaternion.identity;

        // Adjust local scale → ensure world radius = radius
        Vector3 parentLossy = token.transform.lossyScale;
        float px = parentLossy.x == 0f ? 1f : parentLossy.x;
        float pz = parentLossy.z == 0f ? 1f : parentLossy.z;
        float py = parentLossy.y == 0f ? 1f : parentLossy.y;

        float localScaleX = (radius * 2f) / px;
        float localScaleZ = (radius * 2f) / pz;
        float localScaleY = (circleThickness) / py;

        circle.transform.localScale = new Vector3(localScaleX, localScaleY, localScaleZ);
        circle.SetActive(true);

        return circle;
    }

    /// <summary>
    /// Hides the circle if it exists.
    /// </summary>
    public static void HideRangeCircle(GameObject circle)
    {
        if (circle != null)
            circle.SetActive(false);
    }
}
