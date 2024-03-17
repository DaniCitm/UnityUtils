using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Adapted from deepnightlibs (Haxe code)
public class ISquasheable : MonoBehaviour
{
    private float squashX = 1f;  //Transform X squash & stretch scaling, which automatically comes back to 1 after a few frames.
    private float squashY = 1f;  //Transform Y squash & stretch scaling, which automatically comes back to 1 after a few frames.

    void LateUpdate()
    {
        var scale = transform.localScale;
        scale.x = transform.localScale.x * squashX;
        scale.y = transform.localScale.y * squashY;
        transform.localScale = scale;
        
        squashX += (1 - squashX) * Mathf.Min(1, 0.2f);
        squashY += (1 - squashY) * Mathf.Min(1, 0.2f);
    }

    // Briefly squash sprite on X (Y changes accordingly). "1.0" means no distorsion.
    public void SetSquashX(float scaleX)
    {
        squashX = scaleX;
        squashY = 2 - scaleX;
    }

    // Briefly squash sprite on Y (X changes accordingly). "1.0" means no distorsion.
    public void setSquashY(float scaleY)
    {
        squashX = 2 - scaleY;
        squashY = scaleY;
    }
}
