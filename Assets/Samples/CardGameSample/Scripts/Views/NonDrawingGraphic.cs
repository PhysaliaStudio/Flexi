using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A concrete subclass of the Unity UI `Graphic` class that just skips drawing.
/// Useful for providing a raycast target without actually drawing anything.
/// </summary>
/// <remarks>
/// Reference: http://answers.unity.com/answers/1157876/view.html
/// </remarks>
[RequireComponent(typeof(CanvasRenderer))]
public sealed class NonDrawingGraphic : Graphic
{
    public override void SetMaterialDirty()
    {
        return;
    }

    public override void SetVerticesDirty()
    {
        return;
    }

    /// <remarks>
    /// Probably not necessary since the chain of calls `Rebuild()`->`UpdateGeometry()`->`DoMeshGeneration()`->`OnPopulateMesh()` won't happen; so here really just as a fail-safe.
    /// </remarks>
    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
        return;
    }
}
