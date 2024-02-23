using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class CustomOutline : BaseMeshEffect {
    [SerializeField]
    private Color _outlineColor;
    public Color OutlineColor {
        get => _outlineColor;
        set {
            _outlineColor = value;
            graphic.SetVerticesDirty();
        }
    }
    [SerializeField]
    private int _outlineWidth;
    public int OutlineWidth {
        get => _outlineWidth;
        set {
            _outlineWidth = value;
            graphic.SetVerticesDirty();
        }
    }

    private Canvas _canvas;

    private List<UIVertex> _vertexCache = new List<UIVertex>();

    const string MaterialPath = "Assets/Resource/ShaderGraph/UI_Outline.mat";

    protected override void Awake() {
        base.Awake();
        if (graphic != null) {
            if (graphic == null || graphic.material.shader.name != "UI/Outline") {
                LoadOutlineMat();
            }
        }
        RefreshOutline();
    }

    protected override void Start() {
        base.Start();

        graphic.SetVerticesDirty();
    }

#if UNITY_EDITOR
    protected override void OnValidate() {
        base.OnValidate();
        if (graphic != null) {
            if (graphic == null || graphic.material.shader.name != "UI/Outline") {
                LoadOutlineMat();
            }
        }
        //RefreshOutline();
    }
#endif
    private void LoadOutlineMat() {
#if UNITY_EDITOR
        var mat = UnityEditor.AssetDatabase.LoadAssetAtPath<Material>(MaterialPath);
        if (mat != null) graphic.material = mat;
        else
            Debug.LogError("outline mat not found");
#else
        var shader = Shader.Find("UI/Outline");
        graphic.material = new Material(shader);
#endif
    }

    public void RefreshOutline() {        
        if (graphic.canvas != null) {
            var v1 = graphic.canvas.additionalShaderChannels;
            var v2 = AdditionalCanvasShaderChannels.TexCoord1;
            if ((v1 & v2) != v2)
                graphic.canvas.additionalShaderChannels |= v2;

            v2 = AdditionalCanvasShaderChannels.TexCoord2;
            if ((v1 & v2) != v2)
                graphic.canvas.additionalShaderChannels |= v2;

            v2 = AdditionalCanvasShaderChannels.TexCoord3;
            if ((v1 & v2) != v2)
                graphic.canvas.additionalShaderChannels |= v2;

            v2 = AdditionalCanvasShaderChannels.Tangent;
            if ((v1 & v2) != v2)
                graphic.canvas.additionalShaderChannels |= v2;

            v2 = AdditionalCanvasShaderChannels.Normal;
            if ((v1 & v2) != v2)
                graphic.canvas.additionalShaderChannels |= v2;
        }
    }

    public override void ModifyMesh(VertexHelper vh) {
        vh.GetUIVertexStream(_vertexCache);
        ApplyOutline();
        vh.Clear();
        vh.AddUIVertexTriangleStream(_vertexCache);
        _vertexCache.Clear();
    }

    private void ApplyOutline() {
        for (int i = 0, count = _vertexCache.Count - 3; i <= count; i += 3) {
            var v1 = _vertexCache[i];
            var v2 = _vertexCache[i + 1];
            var v3 = _vertexCache[i + 2];

            var minX = Min(v1.position.x, v2.position.x, v3.position.x);
            var minY = Min(v1.position.y, v2.position.y, v3.position.y);
            var maxX = Max(v1.position.x, v2.position.x, v3.position.x);
            var maxY = Max(v1.position.y, v2.position.y, v3.position.y);
            var posCenter = new Vector2(minX + maxX, minY + maxY) * 0.5f;

            Vector2 triX, triY, uvX, uvY;
            Vector2 pos1 = v1.position;
            Vector2 pos2 = v2.position;
            Vector2 pos3 = v3.position;

            if (Mathf.Abs(Vector2.Dot((pos2 - pos1).normalized, Vector2.right)) > Mathf.Abs(Vector2.Dot((pos3 - pos2).normalized, Vector2.right))) {
                triX = pos2 - pos1;
                triY = pos3 - pos2;
                uvX = v2.uv0 - v1.uv0;
                uvY = v3.uv0 - v2.uv0;
            }
            else {
                triX = pos3 - pos2;
                triY = pos2 - pos1;
                uvX = v3.uv0 - v2.uv0;
                uvY = v2.uv0 + v1.uv0;
            }

            var uvMin = Min(v1.uv0, v2.uv0, v3.uv0);
            var uvMax = Max(v1.uv0, v2.uv0, v3.uv0);

            var col_rg = new Vector2(_outlineColor.r, _outlineColor.g);
            var col_ba = new Vector4(0, 0, _outlineColor.b, _outlineColor.a);
            var normal = new Vector3(0, 0, _outlineWidth);

            v1 = SetNewPosAndUV(v1, _outlineWidth, posCenter, triX, triY, uvX, uvY, uvMin, uvMax);
            v1.uv3 = col_rg;
            v1.tangent = col_ba;
            v1.normal = normal;

            v2 = SetNewPosAndUV(v2, _outlineWidth, posCenter, triX, triY, uvX, uvY, uvMin, uvMax);
            v2.uv3 = col_rg;
            v2.tangent = col_ba;
            v2.normal = normal;

            v3 = SetNewPosAndUV(v3, _outlineWidth, posCenter, triX, triY, uvX, uvY, uvMin, uvMax);
            v3.uv3 = col_rg;
            v3.tangent = col_ba;
            v3.normal = normal;

            _vertexCache[i] = v1;
            _vertexCache[i + 1] = v2;
            _vertexCache[i + 2] = v3;
        }
    }

    private static UIVertex SetNewPosAndUV(UIVertex vertex, int width, Vector2 pPosCenter, Vector2 triangleX, Vector2 triangleY,
        Vector2 uvX, Vector2 uvY, Vector2 uvMin, Vector2 uvMax) {
        var pos = vertex.position;
        var posXOffset = pos.x > pPosCenter.x ? width : -width;
        var posYOffset = pos.y > pPosCenter.y ? width : -width;
        pos.x += posXOffset;
        pos.y += posYOffset;
        vertex.position = pos;
        Vector4 uv = vertex.uv0;
        uv += (Vector4)uvX / triangleX.magnitude * posXOffset * (Vector2.Dot(triangleX, Vector2.right) > 0 ? 1 : -1);
        uv += (Vector4)uvY / triangleY.magnitude * posYOffset * (Vector2.Dot(triangleY, Vector2.up) > 0 ? 1 : -1);
        vertex.uv0 = uv;

        vertex.uv1 = uvMin;
        vertex.uv2 = uvMax;
        return vertex;
    }

    private static float Min(float a, float b, float c) {
        return Mathf.Min(Mathf.Min(a, b), c);
    }

    private static float Max(float a, float b, float c) {
        return Mathf.Max(Mathf.Max(a, b), c);
    }

    private static Vector2 Min(Vector2 a, Vector2 b, Vector2 c) {
        return new Vector2(Min(a.x, b.x, c.x), Min(a.y, b.y, c.y));
    }

    private static Vector2 Max(Vector2 a, Vector2 b, Vector2 c) {
        return new Vector2(Max(a.x, b.x, c.x), Max(a.y, b.y, c.y));
    }
}
