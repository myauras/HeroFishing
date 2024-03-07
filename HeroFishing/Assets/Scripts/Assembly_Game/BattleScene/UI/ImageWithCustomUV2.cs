using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageWithCustomUV2 : Image
{
    [SerializeField]
    private Vector2[] _uvs2;

    protected override void Start() {
        base.Start();
        if (!canvas.additionalShaderChannels.HasFlag(AdditionalCanvasShaderChannels.TexCoord1)) {
            canvas.additionalShaderChannels |= AdditionalCanvasShaderChannels.TexCoord1;
        }
    }

    protected override void OnPopulateMesh(VertexHelper vh) {
        base.OnPopulateMesh(vh);
        if (_uvs2?.Length != 4) return;

        var vertex = new UIVertex();
        for (int i = 0; i < 4; i++) {
            vh.PopulateUIVertex(ref vertex, i);
            vertex.uv1 = _uvs2[i];
            vh.SetUIVertex(vertex, i);
        }
    }
}
