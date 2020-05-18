using Castle.Core.Internal;
using Cirrus.Circuit.World;
using Cirrus.Circuit.World.Objects;
using Cirrus.Editor;
using Cirrus.Utils;
using Devdog.General.ThirdParty.UniLinq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Cirrus.Circuit.World.Editor
{
    public enum EditorMode
    {
        FreeCam,
        SelectTile,
        RotateTile,
        ScrollLayer
    }

    public enum LayerMode
    {
        X = 0,
        Y = 1,
        Z = 2
    }

    [Serializable]
    public class Layer
    {
        [SerializeField]
        public int Index = 0;

        [SerializeField]
        public Plane Plane = new Plane();
    }

    public class LevelEditorKeys
    {
        public static readonly KeyCode[] FreeCam = new KeyCode[] { KeyCode.F1 };
        public static readonly KeyCode[] TileSelect = new KeyCode[] { KeyCode.F2 };
        public static readonly KeyCode[] RotateTile = new KeyCode[] { KeyCode.F3 };
        public static readonly KeyCode[] ScrollLayer = new KeyCode[] { KeyCode.F4 };

        public static readonly KeyCode FreeCamMacOS = KeyCode.Alpha1;
        public static readonly KeyCode TileSelectMacOS = KeyCode.Alpha2;
        public static readonly KeyCode RotateTileMacOS = KeyCode.Alpha3;
        public static readonly KeyCode ScrollLayerMacOS = KeyCode.Alpha4;

        //public static readonly KeyCode[] ScrollLayer = new KeyCode[] { KeyCode.Keypad0 };
    }

    public class LevelEditor : BaseSingleton<LevelEditor>
    {

#if UNITY_EDITOR_OSX

        [Header("Shift+1 - Free Cam mode", order = -100)]
        [Header("Shift+2 - Tile/Palette select", order = -100)]
        [Header("Shift+3 - Rotate Tile", order = -100)]
        [Header("Shift+4 - Scroll drawing layer", order = -100)]

#elif UNITY_EDITOR_WIN
        [Header("F1 - Free Cam mode", order = -100)]
        [Header("F2 - Tile/Palette select", order = -100)]
        [Header("F3 - Rotate Tile", order = -100)]
        [Header("F4 - Scroll drawing layer", order = -100)]

#endif


        [Space(height: 32, order = -99)]
        [Header("Level")]
        [SerializeField]
        private Level _level;

        [SerializeField]
        public Level Level {
            get {

                if (_level == null || _level.gameObject == null) _level = FindObjectOfType<Level>();
                if (_level.gameObject == null) _level = null; 
                return _level;
            }
        
        }
        public Vector3Int _dimensions;

        [Header("Modes")]
        [SerializeField]
        public LayerMode _layerMode = LayerMode.Y;
        public LayerMode LayerMode => _layerMode;

        [SerializeField]
        public EditorMode _mode = EditorMode.FreeCam;
        public EditorMode Mode => _mode;

        public Tool LastTool = Tool.None;

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

        [Header("Info")]
        [SerializeField]
        public Layer LayerX = new Layer();

        [SerializeField]
        public Layer LayerY = new Layer();

        [SerializeField]
        public Layer LayerZ = new Layer();

        [SerializeField]
        public Vector3 _debugPosition = Vector3.zero;

        [SerializeField]
        public Brush _brush;

        public Layer SelectedLayer
        {
            get
            {
                switch (_layerMode)
                {
                    case LayerMode.X: return LayerX;
                    case LayerMode.Y: return LayerY;
                    case LayerMode.Z: return LayerZ;
                    default: return LayerY;
                }
            }
        }

        public override void OnValidate()
        {
            base.OnValidate();

            if (_level == null || _level.gameObject == null) _level = FindObjectOfType<Level>();

            EditorLibrary.Instance.DimensionsMaterial.color = _dimensionsColor;
            EditorLibrary.Instance.LayerMaterial.color = _layerColor;
            EditorLibrary.Instance.CursorMaterial.color = _cursorColor;

            if (_level.Dimensions != _dimensions)
            {
                _level._dimensions = _dimensions;
                EditorUtility.SetDirty(_level);
            }
        }

        [ExecuteInEditMode]
        public override void Awake()
        {
            base.Awake();

            _dimensions = _level.Dimensions;
            _dimensionsColor = EditorLibrary.Instance.DimensionsMaterial.color;
            _layerColor = EditorLibrary.Instance.LayerMaterial.color;
            _cursorColor = EditorLibrary.Instance.CursorMaterial.color;
        }

        [ExecuteInEditMode]
        public override void Start()
        {
            base.Start();
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

        [ExecuteInEditMode]
        public void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(_debugPosition, 0.1f);

            //Gizmos.DrawLine(_debugRay.origin, _debugRay.origin + _debugRay.direction * 100);
            //Gizmos.DrawRay(_debugRay.origin, _debugRay.direction * 100);

        }
    }

    //#if UNITY_EDITOR
    [CustomEditor(typeof(LevelEditor))]
    public class LevelEditorCustomInspector : UnityEditor.Editor
    {
        private LevelEditor _editor;

        private Mesh _cursorMesh;

        private Plane _plane;

        public int CellSize => Level.CellSize;

        public virtual void OnEnable()
        {
            _editor = serializedObject.targetObject as LevelEditor;

            Selection.selectionChanged += OnSelectionChange;

            ScrollLayer(_editor.LayerMode, true);
            InitPlane(_editor.LayerMode);

        }


        public virtual void OnDisable()
        {
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Keyboard));
        }

        public void OnSelectionChange()
        {
            Object selected = Selection.objects.FirstOrDefault();
            if (selected == null) return;
            if (!(selected is BaseObject)) return;
            _editor._brush._selectedTileObject = selected;

        }

        public void InitPlane(LayerMode mode)
        {
            Vector3 planePosition;

            if (mode == LayerMode.X)
            {
                planePosition = _editor.Level.GridToWorld(
                    new Vector3Int(_editor.LayerX.Index, 0, 0));

                _editor.LayerX.Plane = new Plane(
                    new Vector3(1, 0, 0),
                    planePosition);
            }
            else if (mode == LayerMode.Y)
            {
                planePosition = _editor.Level.GridToWorld(
                    new Vector3Int(0, _editor.LayerY.Index, 0));

                _editor.LayerY.Plane = new Plane(
                    new Vector3(0, 1, 0),
                    planePosition);
            }
            else if (mode == LayerMode.Z)
            {
                planePosition = _editor.Level.GridToWorld(
                    new Vector3Int(0, 0, _editor.LayerZ.Index));

                _editor.LayerZ.Plane = new Plane(
                    new Vector3(0, 0, 1),
                    planePosition);
            }
        }

        public void ScrollLayer(LayerMode mode, bool up)
        {
            if (mode == LayerMode.X)
            {
                _editor.LayerX.Index = up ? _editor.LayerX.Index + 1 : _editor.LayerX.Index - 1;
                if (_editor.LayerX.Index >= _editor._dimensions.x) _editor.LayerX.Index = _editor._dimensions.x - 1;
                else if (_editor.LayerX.Index < 0) _editor.LayerX.Index = 0;

            }
            else if (mode == LayerMode.Y)
            {
                _editor.LayerY.Index = up ? _editor.LayerY.Index + 1 : _editor.LayerY.Index - 1;
                if (_editor.LayerY.Index >= _editor._dimensions.y) _editor.LayerY.Index = _editor._dimensions.y - 1;
                else if (_editor.LayerY.Index < 0) _editor.LayerY.Index = 0;
            }
            else if (mode == LayerMode.Z)
            {
                _editor.LayerZ.Index = up ? _editor.LayerZ.Index + 1 : _editor.LayerZ.Index - 1;
                if (_editor.LayerZ.Index >= _editor._dimensions.z) _editor.LayerZ.Index = _editor._dimensions.z - 1;
                else if (_editor.LayerZ.Index < 0) _editor.LayerZ.Index = 0;
            }

        }


        public void HandleInputs()
        {
            // Consume inputs
            switch (_editor.Mode)
            {
                case EditorMode.FreeCam:
                    break;

                case EditorMode.SelectTile:

                    // Change palete
                    if (Event.current.type == EventType.KeyDown)
                    {
                        if (Event.current.keyCode > KeyCode.Alpha0 &&
                            Event.current.keyCode <= KeyCode.Alpha9)
                        {
                            int paletteIndex = Event.current.keyCode - KeyCode.Alpha0;
                            paletteIndex = IntegerUtils.Mod(paletteIndex - 1, 10);
                            _editor._brush.SelectedPaletteIndex = paletteIndex;

                            Event.current.Use();
                        }
                    }

                    // Change Tile
                    if (Event.current.type == EventType.ScrollWheel)
                    {
                        if (EventUtils.MouseWheelUp()) _editor._brush.SelectedTileIndex++;
                        else if (EventUtils.MouseWheelDown()) _editor._brush.SelectedTileIndex--;

                        Event.current.Use();
                    }

                    break;

                case EditorMode.RotateTile:

                    if (Event.current.type == EventType.ScrollWheel)
                    {
                        if (EventUtils.MouseWheelUp()) _editor._brush.RotationIndex++;
                        else if (EventUtils.MouseWheelDown()) _editor._brush.RotationIndex--;

                        Event.current.Use();
                    }

                    break;

                case EditorMode.ScrollLayer:

                    // Change LayerMode
                    if (Event.current.type == EventType.KeyDown)
                    {
                        if (Event.current.keyCode > KeyCode.Alpha0 &&
                            Event.current.keyCode <= KeyCode.Alpha9)
                        {
                            int keyIndex = Event.current.keyCode - KeyCode.Alpha0;
                            _editor._layerMode = (LayerMode)IntegerUtils.Mod(keyIndex - 1, 3);

                            Event.current.Use();
                        }
                    }

                    if (Event.current.type == EventType.ScrollWheel)
                    {
                        if (EventUtils.MouseWheelUp())
                        {
                            ScrollLayer(_editor.LayerMode, true);
                            InitPlane(_editor.LayerMode);
                        }
                        else if (EventUtils.MouseWheelDown())
                        {
                            ScrollLayer(_editor.LayerMode, false);
                            InitPlane(_editor.LayerMode);
                        }

                        Event.current.Use();
                    }
                    break;

            }


            Event e = Event.current;
            switch (e.type)
            {
                case EventType.KeyDown:
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
#if UNITY_EDITOR_WIN
                    else if (LevelEditorKeys.FreeCam.Contains(Event.current.keyCode))
#elif UNITY_EDITOR_OSX
                    else if (e.control && e.keyCode == KeyCode.Alpha1)
#endif
                        _editor._mode = EditorMode.FreeCam;
#if UNITY_EDITOR_WIN
                    else if (LevelEditorKeys.TileSelect.Contains(Event.current.keyCode))
#elif UNITY_EDITOR_OSX
                    else if (e.control && e.keyCode == KeyCode.Alpha2)
#endif
                        _editor._mode = EditorMode.SelectTile;
#if UNITY_EDITOR_WIN
                    else if (LevelEditorKeys.RotateTile.Contains(Event.current.keyCode))
#elif UNITY_EDITOR_OSX
                    else if (e.control && e.keyCode == KeyCode.Alpha3)
#endif
                        _editor._mode = EditorMode.RotateTile;
#if UNITY_EDITOR_WIN
                    else if (LevelEditorKeys.ScrollLayer.Contains(Event.current.keyCode))
#elif UNITY_EDITOR_OSX
                    else if (e.control && e.keyCode == KeyCode.Alpha3)
#endif
                        _editor._mode = EditorMode.ScrollLayer;

                    break;

            }

        }

        public void OnSceneGUI()
        {
            if (_editor.Level == null) return; 

            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

            HandleInputs();

            _editor.LastTool = Tools.current;
            Tools.current = Tool.None;

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
                case LayerMode.X:
                    mesh = GraphicsUtils.CreateCube(
                        CellSize,
                        _editor.Level.Dimensions.y * CellSize,
                        _editor.Level.Dimensions.z * CellSize);

                    Graphics.DrawMesh(
                        mesh,
                        _editor.Level.transform.position +

                        new Vector3(1, 0, 0) * (_editor.LayerX.Index * CellSize) +

                        new Vector3(0, 1, 0) * (_editor.Level.Dimensions.y * CellSize) / 2 -
                        new Vector3(0, 1, 0) * CellSize / 2 +

                        new Vector3(0, 0, 1) * (_editor.Level.Dimensions.z * CellSize) / 2 -
                        new Vector3(0, 0, 1) * CellSize / 2 +

                        _editor.Level.Offset * CellSize,

                        Quaternion.identity,
                        EditorLibrary.Instance.LayerMaterial,
                        0
                        );
                    break;

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

                        new Vector3(0, 1, 0) * (_editor.LayerY.Index * CellSize) +

                        new Vector3(0, 0, 1) * (_editor.Level.Dimensions.z * CellSize) / 2 -
                        new Vector3(0, 0, 1) * CellSize / 2 +

                        _editor.Level.Offset * CellSize,

                        Quaternion.identity,
                        EditorLibrary.Instance.LayerMaterial,
                        0
                        );
                    break;

                case LayerMode.Z:
                    mesh = GraphicsUtils.CreateCube(
                        _editor.Level.Dimensions.x * CellSize,
                        _editor.Level.Dimensions.y * CellSize,
                        CellSize);

                    Graphics.DrawMesh(
                        mesh,
                        _editor.Level.transform.position +

                        new Vector3(1, 0, 0) * (_editor.Level.Dimensions.x * CellSize) / 2 -
                        new Vector3(1, 0, 0) * CellSize / 2 +

                        new Vector3(0, 1, 0) * (_editor.Level.Dimensions.y * CellSize) / 2 -
                        new Vector3(0, 1, 0) * CellSize / 2 +

                        new Vector3(0, 0, 1) * (_editor.LayerZ.Index * CellSize) +

                        _editor.Level.Offset * CellSize,

                        Quaternion.identity,
                        EditorLibrary.Instance.LayerMaterial,
                        0
                        );
                    break;
            }

#endregion

#region GUI

            Rect screenRect =
                SceneView.currentDrawingSceneView.position;
            Rect rect = new Rect();

            rect.height = screenRect.height / 8;
            rect.width = screenRect.width / 8;
            rect.position = new Vector2(screenRect.width / 2, 16);

            // Mode Button
            GUILayout.BeginArea(rect);

            if (GUILayout.Button(
                ObjectNames.NicifyVariableName(
                Enum.GetName(typeof(EditorMode), _editor.Mode))))
            {
                _editor._mode++;
                if (
                    (int)_editor._mode >=
                    EnumUtils.Size(typeof(EditorMode)))
                {
                    _editor._mode = 0;
                }
            }


            GUILayout.EndArea();

            // Tile Buttons


            rect.height = screenRect.height / 8;
            rect.width = screenRect.width / 8;
            rect.position = new Vector2(screenRect.width / 2, screenRect.height - 96);

            GUILayout.BeginArea(rect);

            if (GUILayout.Button(_editor._brush.SelectedPalette == null
                ? "?" :
                _editor._brush.SelectedPalette.name))
            {
                _editor._brush.SelectedPaletteIndex++;
            }

            if (GUILayout.Button(_editor._brush.SelectedTile == null
                ? "?" :
                _editor._brush.SelectedTile.name))
            {
                _editor._brush.SelectedTileIndex++;
            }


            GUILayout.EndArea();

#endregion

            //delete meshes of previous frame and draw new meshes
            EditorUtility.SetDirty(target);
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Save level as prefab"))
            {
                _editor.Save();
                var path = EditorUtility.SaveFilePanel(
                    "Save Level as Prefab",
                    "",
                    _editor.Level.name + ".prefab",
                    "prefab");

                if (path.Length != 0)
                {
                    PrefabUtility.SaveAsPrefabAssetAndConnect(
                        _editor.Level.gameObject, 
                        path, 
                        InteractionMode.AutomatedAction);
                }

            }
        }
    }

    //#endif

}