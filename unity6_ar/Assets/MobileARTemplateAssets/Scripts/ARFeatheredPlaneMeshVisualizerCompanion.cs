using UnityEngine.XR.Interaction.Toolkit.Utilities.Tweenables.Primitives;

namespace UnityEngine.XR.Interaction.Toolkit.Samples.ARStarterAssets
{
    /// <summary>
    /// Unity의 AR Foundation 환경에서 사용되는 AR 평면(Plane)의 시각적 효과를 조정하는 컴포넌트
    /// ARFeatheredPlane의 투명도(알파 값)를 부드럽게 페이드 인/아웃시키는 데 사용
    /// Performs additional visual operations on the ARFeatheredPlane mesh, such as animated alpha fading.
    /// </summary>
    [RequireComponent(typeof(MeshRenderer))]  ///이 컴포넌트를 사용하는 GameObject에 MeshRenderer 컴포넌트가 필수
    public class ARFeatheredPlaneMeshVisualizerCompanion : MonoBehaviour
    {
        [Tooltip("Renderer component on the ARFeatheredPlane prefab. Used to fetch the material to fade in/out.")]
        [SerializeField]
        Renderer m_PlaneRenderer;

        /// <summary>
        /// The Renderer component on the ARFeatheredPlane prefab. Used to fetch the material to fade in/out.
        /// </summary>
        public Renderer planeRenderer
        {
            get => m_PlaneRenderer;
            set => m_PlaneRenderer = value;
        }

        [Tooltip("Fade in/out speed multiplier applied during the alpha tweening. The lower the value, the slower it works. A value of 1 is full speed (1 second).")]
        [Range(0.1f, 1.0f)]
        [SerializeField]
        float m_FadeSpeed = 1f;

        /// <summary>
        /// Fade in/out speed multiplier applied during the alpha tweening.
        /// The lower the value, the slower it works. A value of 1 is full speed (1 second).
        /// </summary>
        public float fadeSpeed
        {
            get => m_FadeSpeed;
            set => m_FadeSpeed = value;
        }

        int m_ShaderAlphaPropertyID;
        float m_SurfaceVisualAlpha = 1f;
        float m_TweenProgress;
        Material m_PlaneMaterial;

#pragma warning disable CS0618 // Type or member is obsolete -- affordance system to be replaced in a future XRI version
        readonly FloatTweenableVariable m_AlphaTweenableVariable = new FloatTweenableVariable();
#pragma warning restore CS0618

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        void Awake()
        {
            m_ShaderAlphaPropertyID = Shader.PropertyToID("_PlaneAlpha");
            m_PlaneMaterial = m_PlaneRenderer.material;
            visualizeSurfaces = true;
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        void OnDestroy()
        {
            m_AlphaTweenableVariable.Dispose();
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        void Update()
        {
            m_AlphaTweenableVariable.HandleTween(m_TweenProgress);
            m_TweenProgress += Time.unscaledDeltaTime * m_FadeSpeed;
            m_SurfaceVisualAlpha = m_AlphaTweenableVariable.Value;
            m_PlaneMaterial.SetFloat(m_ShaderAlphaPropertyID, m_SurfaceVisualAlpha);
        }

        /// <summary>
        /// Show plane surfaces if true, hide plane surfaces if false
        /// </summary>
        public bool visualizeSurfaces
        {
            set
            {
                m_TweenProgress = 0f;
                m_AlphaTweenableVariable.target = value ? 1f : 0f;
                m_AlphaTweenableVariable.HandleTween(0f);
            }
        }
    }
}
