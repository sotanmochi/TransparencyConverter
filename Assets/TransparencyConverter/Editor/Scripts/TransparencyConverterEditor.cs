using UnityEditor;
using UnityEngine;

namespace TransparencyConverter.Editor
{
    public class TransparencyConverterEditor : EditorWindow
    {
        private static readonly string _shaderName = "Transparency Converter/Unlit Color";

        private bool _initialize;
        private PreviewRenderUtility _previewRenderer;

        private float _transparency = 1.0f;
        private float _transparencySRGB = 1.0f;

        private Color _backgroundColor = Color.white;
        private Color _foregroundColor = Color.black;

        private Material _imageMaterial;

        [MenuItem("Window/Transparency Converter")]
        public static void Init()
        {
            var window = EditorWindow.GetWindow<TransparencyConverterEditor>("Transparency Converter");
            window.minSize = new Vector2(280, 280);
        }

        public void OnDisable()
        {
            _initialize = false;
            _previewRenderer.Cleanup();
        }

        public void OnGUI()
        {
            if (!_initialize)
            {
                _previewRenderer = new PreviewRenderUtility();
                _initialize = true;
            }

            EditorGUILayout.LabelField($"<Input>", EditorStyles.boldLabel);

            _transparencySRGB = EditorGUILayout.Slider("Transparency (sRGB)", _transparencySRGB, 0, 1);

            var transparencyLinear = TransparencyConverter.SRGBToLinear(_transparencySRGB);
            _transparency = PlayerSettings.colorSpace is ColorSpace.Gamma ? _transparencySRGB : transparencyLinear;

            _imageMaterial = EditorGUILayout.ObjectField("Image Material", _imageMaterial, typeof(Material), true) as Material;
            _foregroundColor = EditorGUILayout.ColorField("Foreground Color", _foregroundColor);
            _backgroundColor = EditorGUILayout.ColorField("Background Color", _backgroundColor);

            _foregroundColor.a = _transparency;

            EditorGUILayout.LabelField($"<Output>", EditorStyles.boldLabel);
            EditorGUILayout.FloatField($"Transparency ({PlayerSettings.colorSpace})", _transparency);
            EditorGUILayout.LabelField($"Preview");

            DrawPreview();
        }

        private void DrawPreview()
        {
            var heightOffset = 180;
            var size = position.width;

            var rect = new Rect(0, heightOffset, size, size);
            _previewRenderer.BeginStaticPreview(rect);

            // Camera settings
            _previewRenderer.camera.farClipPlane = 100;
            _previewRenderer.camera.transform.position = new Vector3(0, 50, 0);
            _previewRenderer.camera.transform.rotation = Quaternion.Euler(90, 180, 0);
            _previewRenderer.camera.clearFlags = CameraClearFlags.SolidColor;
            // _previewRenderer.camera.backgroundColor = _backgroundColor;

            // Add game object
            var foregroundPlane = GameObject.CreatePrimitive(PrimitiveType.Plane);

            var foregroundMaterial = new Material(Shader.Find(_shaderName));
            foregroundMaterial.color = _foregroundColor;

            foregroundPlane.GetComponent<MeshRenderer>().material = foregroundMaterial;
            _previewRenderer.AddSingleGO(foregroundPlane);

            // Add game object
            var backgroundPlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
            backgroundPlane.transform.position = new Vector3(0, -25, 0);
            backgroundPlane.transform.localScale = new Vector3(2, 2, 2);

            var backgroundMaterial = new Material(Shader.Find(_shaderName));
            backgroundMaterial.color = _backgroundColor;

            backgroundPlane.GetComponent<MeshRenderer>().material = backgroundMaterial;
            _previewRenderer.AddSingleGO(backgroundPlane);

            // Add game object
            var imagePlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
            imagePlane.transform.position = new Vector3(0, -20, 0);

            if (_imageMaterial != null)
            {
                imagePlane.GetComponent<MeshRenderer>().material = _imageMaterial;
                _previewRenderer.AddSingleGO(imagePlane);
            }

            // Rendering
            _previewRenderer.camera.Render();
            GUI.DrawTexture(rect, _previewRenderer.EndStaticPreview());

            // Delete game objects
            GameObject.DestroyImmediate(foregroundPlane);
            GameObject.DestroyImmediate(backgroundPlane);
            GameObject.DestroyImmediate(imagePlane);
        }
    }
}