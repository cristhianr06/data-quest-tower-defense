using UnityEditor;
using UnityEngine;
using VrBand.Assets._model_explorer_vr.Code.Tools;

namespace VrBand.Assets._model_explorer_vr.Code.Editor
{
    public class ShowHideCollidersEditor : EditorWindow
    {
        private GameObject _selectedObject;
        private Color _selectedColor;
        private bool _isGizmoVisible;
        private bool _isSelected;
        private Texture2D _myTexture;
        private GUIStyle _showButtonStyle;
        private GUIStyle _hideButtonStyle;
        private GUIStyle _boxStyle;

        [MenuItem("Tools/Show Hide Colliders")]
        private static void CreateWindow()
        {
            //Crea la ventana de un tamaño predeterminado
            var window = GetWindow<ShowHideCollidersEditor>("Show Hide Colliders");
            window.minSize = new Vector2(300, 120);
            window.maxSize = new Vector2(300.1f, 120.1f);
        }

        private void OnEnable()
        {
            //Establece un colo inicial para "_selectedColor"
            SetConfigInitial();
            _myTexture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Art/Sprites/warning.png");
            //EditorApplication.update += Repaint;
            Selection.selectionChanged += OnSelectionChanged;
        }

        private void OnDisable()
        {
            if (!_selectedObject) return;
            Selection.selectionChanged -= OnSelectionChanged;
        }

        private void OnSelectionChanged()
        {
            //Actualiza la ventana cada vez que se selecciona un gameObject
            SetConfigInitial();
            Repaint();
        }

        private void OnGUI()
        {
            //Estilos
            #region StyleButtons

            _showButtonStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = 12,
                normal =
                {
                    textColor = Color.white,
                },
                padding = new RectOffset(0, 0, 5, 5)
            };
            _hideButtonStyle = new GUIStyle(GUI.skin.button)
            {
                normal =
                {
                    textColor = Color.white,
                },
                padding = new RectOffset(0, 0, 5, 5)
            };

            _boxStyle = new GUIStyle(GUI.skin.box)
            {
                padding = new RectOffset(10, 10, 10, 10)
            };
            #endregion

            //Verifica si un gameObject fue selecionado
            if (_selectedObject)
            {
                var selectedObjCollider = _selectedObject.GetComponent<Collider>();
                //identifica el tipo de collider y si es valido muesta el boton "Show Collider"
                if (selectedObjCollider.GetType() == typeof(BoxCollider) || selectedObjCollider.GetType() == typeof(SphereCollider))
                {
                    //Verifica si hay Gizmo dibujado, la variable se actualiza en SetConfigInitial()
                    if (_isGizmoVisible)
                    {
                        //Dibuja los botones en ventana de acuerdo a las condiciones
                        GUILayout.Space(10);
                        GUILayout.BeginHorizontal();
                        GUILayout.Space(25);
                        if (GUILayout.Button("Hide Collider", _hideButtonStyle, GUILayout.Width(250), GUILayout.Height(30)))
                        {
                            RemoveVisualizerCollider();
                            SetConfigInitial();
                        }
                        GUILayout.Space(25);
                        GUILayout.EndHorizontal();

                        GUILayout.Space(10);

                        GUILayout.BeginHorizontal();
                        GUILayout.Space(25);
                        _selectedColor = EditorGUILayout.ColorField("", _selectedColor, GUILayout.Width(250), GUILayout.Height(30));
                        UpdateColor();
                        GUILayout.Space(25);
                        GUILayout.EndHorizontal();
                    }
                    else if (_isSelected && !_isGizmoVisible)
                    {
                        GUILayout.Space(10);
                        GUILayout.BeginHorizontal();
                        GUILayout.Space(25);
                        if (GUILayout.Button("Show Collider", _showButtonStyle, GUILayout.Width(250), GUILayout.Height(30)))
                        {
                            AddVisualizerCollider();
                            SetConfigInitial();
                        }
                        GUILayout.Space(25);
                        GUILayout.EndHorizontal();
                    }
                }
                else
                {
                    GUILayout.Space(10);
                    GUILayout.BeginVertical();
                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    if (_myTexture)
                    {
                        GUILayout.Label(_myTexture, GUILayout.Width(32), GUILayout.Height(32));
                    }
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();

                    GUILayout.Space(10);

                    GUILayout.BeginHorizontal();
                    GUILayout.Box(new GUIContent(
                        "The selected game object must have a BoxCollider or a SphereCollider in order to display colliding Gizmos.",
                        "El objeto de juego seleccionado debe tener un BoxCollider o un SphereCollider para poder mostrar Gizmos colisionadores."), _boxStyle);
                    GUILayout.EndHorizontal();
                    GUILayout.Space(10);
                    GUILayout.EndVertical();
                }
            }
            else
            {
                //Estilos de caja
                GUILayout.Space(10);
                GUILayout.BeginVertical();
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (_myTexture)
                {
                    GUILayout.Label(_myTexture, GUILayout.Width(32), GUILayout.Height(32));
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.Space(10);

                GUILayout.BeginHorizontal();
                GUILayout.Box(new GUIContent(
                    "Select one or more game objects in scene with at least one BoxCollider or SphereCollider component, to display collider gizmos.",
                    "Selecciona uno o más objetos de juego en escena con al menos un componente BoxCollider o SphereCollider, para mostrar gizmos de colisionador."));
                GUILayout.EndHorizontal();
                GUILayout.Space(10);
                GUILayout.EndVertical();
            }
        }

        //Inicializa la configuracion de la ventana
        private void SetConfigInitial()
        {
            _selectedObject = Selection.activeGameObject;
            if (_selectedObject)
            {
                _isSelected = true;
            }
            else
            {
                _isSelected = false;
                return;
            }
            var isVisualizerCollider = _selectedObject.GetComponent<GizmoVisualizeColliders>();
            if (isVisualizerCollider)
            {
                var isColor = isVisualizerCollider.ColorGizmos;
                _selectedColor = isColor;
                _isGizmoVisible = true;
            }
            else
            {
                _isGizmoVisible = false;
                _selectedColor = Color.cyan;
            }
        }

        //Actualiza el color del Gizmo
        private void UpdateColor()
        {
            var selectedObjects = Selection.gameObjects;

            foreach (var selectedObject in selectedObjects)
            {
                var isVisualizerCollider = selectedObject.GetComponent<GizmoVisualizeColliders>();
                if (!isVisualizerCollider) return;
                isVisualizerCollider.ColorGizmos = _selectedColor;
            }
        }

        //Method that adds the “VisualizeColliders” script as a component to the selected objects.
        private void AddVisualizerCollider()
        {
            var selectedObjects = Selection.gameObjects;

            foreach (var selectedObject in selectedObjects)
            {
                var boxCollider = selectedObject.GetComponent<BoxCollider>();
                var sphereCollider = selectedObject.GetComponent<SphereCollider>();
                var meshCollider = selectedObject.GetComponent<MeshCollider>();
                var capsuleCollider = selectedObject.GetComponent<CapsuleCollider>();
                if (!boxCollider && !sphereCollider && !meshCollider && capsuleCollider)
                {
                    Debug.LogWarning(
                        $"The collider is not of the box or sphere type. Current collider: {selectedObject.GetComponent<Collider>().GetType()}");
                    Debug.Log($"Collider display not added to a {selectedObject.name}");
                }
                else
                {
                    if (!boxCollider && !sphereCollider) return;
                    var visualizer = selectedObject.GetComponent<GizmoVisualizeColliders>();
                    if (visualizer) return;
                    var gizmoVisualizeColliders = selectedObject.AddComponent<GizmoVisualizeColliders>();
                    gizmoVisualizeColliders.ColorGizmos = _selectedColor;
                    Debug.Log($"Collider display added from {selectedObject.name}");
                }
            }
        }

        //Method that removes the “VisualizeColliders” script as a component to the selected objects.
        private static void RemoveVisualizerCollider()
        {
            var selectedObjects = Selection.gameObjects;

            foreach (var selectedObject in selectedObjects)
            {
                var isVisualizerCollider = selectedObject.GetComponent<GizmoVisualizeColliders>();
                if (!isVisualizerCollider) return;
                DestroyImmediate(isVisualizerCollider);
                Debug.Log($"Collider display removed from {selectedObject.name}");
            }
        }
    }
}