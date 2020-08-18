using UnityEngine;
using UnityEngine.UI;

namespace SeganX
{
    [ExecuteInEditMode]
    public class UiResourceParticle : MaskableGraphic
    {
        private struct Quad
        {
            public float age;
            public Vector3 position;
        }

        [SerializeField] private Sprite sprite = null;
        [SerializeField] private float size = 1;
        [SerializeField] private Gradient colorOverLife = new Gradient();
        [SerializeField] private AnimationCurve sizeOverLife = new AnimationCurve(new Keyframe[] { new Keyframe(0, 0), new Keyframe(0.5f, 1), new Keyframe(1, 0) });

#if UNITY_EDITOR
        [Header("Editor Mode:")]
#endif
        [SerializeField] private float life = 2;
        [SerializeField] private Vector2[] points = new Vector2[] { new Vector2(-50, -50), new Vector2(50, 50) };

        private Quad[] quads = new Quad[128];

        public override Texture mainTexture => sprite ? sprite.texture : base.mainTexture;

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();

            var uv = sprite ? UnityEngine.Sprites.DataUtility.GetOuterUV(sprite) : Vector4.zero;
            var sw = sprite ? sprite.rect.width * size * 0.5f : 50 * size;
            var sh = sprite ? sprite.rect.height * size * 0.5f : 50 * size;

            var quad = new UIVertex[4];

            for (int i = 0; i < quads.Length; i++)
            {
                if (quads[i].age < 0 || quads[i].age > 1) continue;

                var qcolor = colorOverLife.Evaluate(quads[i].age) * color;
                var qsize = sizeOverLife.Evaluate(quads[i].age);
                var qw = sw * qsize;
                var qh = sh * qsize;


                quad[0] = UIVertex.simpleVert;
                quad[0].color = qcolor;
                quad[0].position = new Vector3(-qw, -qh, 0) + quads[i].position;
                quad[0].uv0 = new Vector2(uv.x, uv.y);

                quad[1] = UIVertex.simpleVert;
                quad[1].color = qcolor;
                quad[1].position = new Vector3(-qw, qh, 0) + quads[i].position;
                quad[1].uv0 = new Vector2(uv.x, uv.w);

                quad[2] = UIVertex.simpleVert;
                quad[2].color = qcolor;
                quad[2].position = new Vector3(qw, qh, 0) + quads[i].position;
                quad[2].uv0 = new Vector2(uv.z, uv.w);

                quad[3] = UIVertex.simpleVert;
                quad[3].color = qcolor;
                quad[3].position = new Vector3(qw, -qh, 0) + quads[i].position;
                quad[3].uv0 = new Vector2(uv.z, uv.y);

                vh.AddUIVertexQuad(quad);
            }
        }

        private void Update()
        {
            bool dirty = false;
            for (int i = 0; i < quads.Length; i++)
            {
                if (quads[i].age > 1) continue;
                dirty = true;

                quads[i].age += Time.unscaledDeltaTime / life;
                quads[i].position = Vector2.Lerp(points[0], points[1], quads[i].age);
            }

            if (dirty) SetAllDirty();
        }

#if UNITY_EDITOR
        void LateUpdate()
        {
            if (!Application.isPlaying)
            {
                SetAllDirty();
            }
        }

        protected override void OnValidate()
        {
            base.OnValidate();

            for (int i = 0; i < quads.Length; i++)
            {
                quads[i].age = -i * 0.5f;
            }
        }
#endif
    }
}