using Cirrus.Circuit.World;
using Cirrus.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Cirrus.Circuit.World.Editor
{
    public enum CursorMode
    {
        Terrain,
        Object,
        Select,
        Erase,
    }
    public enum LayerMode
    {
        X,
        Y,
        Z
    }

    [Serializable]
    public class Layer
    {
        [SerializeField]
        public int Index = 0;

        [SerializeField]
        public Plane Plane = new Plane();
    }

    public class LevelEditor : MonoBehaviour
    {
        [Header("Level")]
        [SerializeField]
        private Level _level;

        [SerializeField]
        public Level Level => _level;

        public Vector3Int _dimensions;

        [Header("Display")]
        [SerializeField]
        private bool _areDimensionsVisible = false;
        public bool AreDimensionsVisible => _areDimensionsVisible;
        [SerializeField]
        private Color _dimensionsColor = ColorUtils.LightBlue;
        [SerializeField]
        private Color _cursorColor = ColorUtils.LightBlue;
        [SerializeField]
        private Color _layerColor = ColorUtils.LightBlue;

        [Header("Modes")]
        [SerializeField]
        public CursorMode _cursorMode;
        public CursorMode CursorMode => _cursorMode;


        [SerializeField]
        public LayerMode _layerMode = LayerMode.Y;
        public LayerMode LayerMode => _layerMode;

        [Header("Info")]
        [SerializeField]
        public Layer XLayer = new Layer();

        [SerializeField]
        public Layer YLayer = new Layer();

        [SerializeField]
        public Layer ZLayer = new Layer();

        public Layer SelectedLayer
        {
            get
            {
                switch (_layerMode)
                {
                    case LayerMode.X: return XLayer;
                    case LayerMode.Y: return YLayer;
                    case LayerMode.Z: return ZLayer;
                    default: return YLayer;
                }
            }
        }

        public virtual void OnValidate()
        {

            if (_level == null) _level = FindObjectOfType<Level>();

            EditorLibrary.Instance.DimensionsMaterial.color = _dimensionsColor;
            EditorLibrary.Instance.LayerMaterial.color = _layerColor;
            EditorLibrary.Instance.CursorMaterial.color = _cursorColor;

            if (_level.Dimensions != _dimensions)
            {
                _level._dimensions = _dimensions;
                EditorUtility.SetDirty(_level);
            }
        }

        public void Awake()
        {
            _dimensions = _level.Dimensions;
            _dimensionsColor = EditorLibrary.Instance.DimensionsMaterial.color;
            _layerColor = EditorLibrary.Instance.LayerMaterial.color;
            _cursorColor = EditorLibrary.Instance.CursorMaterial.color;
        }

        public void Start()
        {

        }

        [ExecuteInEditMode]
        public void Update()
        {
            //Vector2 mousePos = Event.current.mousePosition;
            //mousePos.y = Camera.current.pixelHeight - mousePos.y;

            //Vector3 worldPosition = Camera.current.ScreenToWorldPoint(mousePos);
            //worldPosition.z = 0;

            //_cursor.Position = _level.WorldToGrid(worldPosition);
        }

        public void Save()
        {

        }

        [SerializeField]
        public Vector3 _debugPosition = Vector3.zero;

        public Ray _debugRay;


        [ExecuteInEditMode]
        public void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(_debugPosition, 0.1f);
         
            Gizmos.DrawLine(_debugRay.origin, _debugRay.origin + _debugRay.direction * 100);
            Gizmos.DrawRay(_debugRay.origin, _debugRay.direction * 100);

        }

    }

//#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(LevelEditor))]
    public class LevelEditorCustomInspector : UnityEditor.Editor
    {
        private LevelEditor _editor;

        private Mesh _cursorMesh;

        private Plane _plane;

        public int CellSize => Level.CellSize;

        public virtual void OnEnable()
        {
            _editor = serializedObject.targetObject as LevelEditor;

            _cursorMesh = GraphicsUtils.CreateCube(
            CellSize,
            CellSize,
            CellSize);

            //int id = GUIUtility.GetControlID(FocusType.Passive);
            //HandleUtility.AddDefaultControl(id);

        }

        public virtual void OnDisable()
        {
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Keyboard));
        }

        public void InitPlane(LayerMode mode)
        {
            Vector3 planePosition;

            if (mode == LayerMode.X)
            {
                planePosition = _editor.Level.GridToWorld(
                    new Vector3Int(_editor.XLayer.Index, 0, 0));

                _editor.XLayer.Plane = new Plane(
                    new Vector3(1, 0, 0),
                    planePosition);
            }
            else if (mode == LayerMode.Y)
            {
                planePosition = _editor.Level.GridToWorld(
                    new Vector3Int(0, _editor.YLayer.Index, 0));

                _editor.YLayer.Plane = new Plane(
                    new Vector3(0, 1, 0),
                    planePosition);
            }
            else if (mode == LayerMode.Z)
            {
                planePosition = _editor.Level.GridToWorld(
                    new Vector3Int(0, 0, _editor.ZLayer.Index));

                _editor.ZLayer.Plane = new Plane(
                    new Vector3(0, 0, 1),
                    planePosition);
            }
        }

        public void ScrollLayer(LayerMode mode, bool up)
        {
            if (mode == LayerMode.X)
            {
                _editor.XLayer.Index = up ? _editor.XLayer.Index + 1 : _editor.XLayer.Index - 1;
                if (_editor.XLayer.Index >= _editor._dimensions.x) _editor.XLayer.Index = _editor._dimensions.x - 1;
                else if (_editor.XLayer.Index < 0) _editor.XLayer.Index = 0;

            }
            else if (mode == LayerMode.Y)
            {
                _editor.YLayer.Index = up ? _editor.YLayer.Index + 1 : _editor.YLayer.Index - 1;
                if (_editor.YLayer.Index >= _editor._dimensions.y) _editor.YLayer.Index = _editor._dimensions.y - 1;
                else if (_editor.YLayer.Index < 0) _editor.YLayer.Index = 0;
            }
            else if (mode == LayerMode.Z)
            {
                _editor.ZLayer.Index = up ? _editor.ZLayer.Index + 1 : _editor.ZLayer.Index - 1;
                if (_editor.ZLayer.Index >= _editor._dimensions.z) _editor.ZLayer.Index = _editor._dimensions.z - 1;
                else if (_editor.ZLayer.Index < 0) _editor.ZLayer.Index = 0;
            }

        }


        public void HandleInputs()
        {
            Event e = Event.current;
            switch (e.type)
            {
                case EventType.KeyDown:
                    {
                        if (
                            Event.current.keyCode == KeyCode.Plus ||
                            Event.current.keyCode == KeyCode.KeypadPlus
                            )
                        {
                            ScrollLayer(_editor.LayerMode, true);
                            InitPlane(_editor.LayerMode);
                        }
                        else if (
                            Event.current.keyCode == KeyCode.Minus ||
                            Event.current.keyCode == KeyCode.KeypadMinus
                            )
                        {
                            ScrollLayer(_editor.LayerMode, false);
                            InitPlane(_editor.LayerMode);
                        }

                        break;
                    }
            }

        }

        public void OnSceneGUI()
        {
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

            HandleInputs();

            Mesh mesh;

            #region Dimensions

            if (_editor.AreDimensionsVisible)
            {
                mesh = GraphicsUtils.CreateCube(
                    _editor.Level.Dimensions.x * CellSize,
                    _editor.Level.Dimensions.y * CellSize,
                    _editor.Level.Dimensions.z * CellSize);

                Graphics.DrawMesh(
                    mesh,
                    _editor.Level.transform.position +

                    new Vector3(1, 0, 0) * (_editor.Level.Dimensions.x * CellSize) / 2 -
                    new Vector3(1, 0, 0) * CellSize / 2 +

                    new Vector3(0, 1, 0) * (_editor.Level.Dimensions.y * CellSize) / 2 -
                    new Vector3(0, 1, 0) * CellSize / 2 +

                    new Vector3(0, 0, 1) * (_editor.Level.Dimensions.z * CellSize) / 2 -
                    new Vector3(0, 0, 1) * CellSize / 2 +

                    _editor.Level.Offset * CellSize,

                    Quaternion.identity,
                    EditorLibrary.Instance.DimensionsMaterial,
                    0
                    );
            }

            #endregion

            #region Layer

            switch (_editor.LayerMode)
            {
                case LayerMode.Y:
                    mesh = GraphicsUtils.CreateCube(
                        _editor.Level.Dimensions.x * CellSize,
                        CellSize,
                        _editor.Level.Dimensions.z * CellSize);

                    Graphics.DrawMesh(
                        mesh,
                        _editor.Level.transform.position +

                        new Vector3(1, 0, 0) * (_editor.Level.Dimensions.x * CellSize) / 2 -
                        new Vector3(1, 0, 0) * CellSize / 2 +

                        new Vector3(0, 1, 0) * (_editor.YLayer.Index * CellSize) +

                        new Vector3(0, 0, 1) * (_editor.Level.Dimensions.z * CellSize) / 2 -
                        new Vector3(0, 0, 1) * CellSize / 2 +

                        _editor.Level.Offset * CellSize,

                        Quaternion.identity,
                        EditorLibrary.Instance.LayerMaterial,
                        0
                        );
                    break;
            }

            #endregion


            #region Cursor            

            var cam = SceneView.currentDrawingSceneView.camera;
      
            var ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

            if (_editor.SelectedLayer.Plane.Raycast(
                ray,
                out float enter))
            {
                //Get the point that is clicked
                Vector3 hitPoint = ray.GetPoint(enter);
                _editor._debugPosition = hitPoint;

                //Move your cube GameObject to the point where you clicked
                Vector3Int gridPos = _editor.Level.WorldToGrid(hitPoint);
                gridPos.Clamp(Vector3Int.zero, _editor.Level.Dimensions);

                Graphics.DrawMesh(
                    _cursorMesh,
                    //_editor.Level.transform.position +
                    _editor.Level.GridToWorld(gridPos),
                    Quaternion.identity,
                    EditorLibrary.Instance.CursorMaterial,
                    0
                    );
            }


            #endregion


            //delete meshes of previous frame and draw new meshes
            EditorUtility.SetDirty(target);
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            //Called whenever the inspector is drawn for this object.
            //DrawDefaultInspector();

            //_selectedIndex = EditorGUILayout.Popup(
            //    _selectedIndex,
            //    Enum.GetNames(typeof(EditorOption)));

            if (GUILayout.Button("Save"))
            {
                _editor.Save();
            }
        }
    }

//#endif

}