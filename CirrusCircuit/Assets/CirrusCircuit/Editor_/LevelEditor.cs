using Castle.Core.Internal;
using Cirrus.Circuit.World;
using Cirrus.Circuit.World.Objects;
using Cirrus.UnityEditorExt;
using Cirrus;
//using Devdog.General.ThirdParty.UniLinq;
using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using Cirrus.Collections;
using System.Linq;

namespace Cirrus.Circuit.Editor
{
    public enum EditorMode
    {
        FreeCam,
        SetTileTemplate,
        RotateTile,
        SelectTile,
        ScrollLayer,
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
        public static readonly KeyCode[] SetTileTemplate = new KeyCode[] { KeyCode.F2 };
        public static readonly KeyCode[] RotateTile = new KeyCode[] { KeyCode.F3 };
        public static readonly KeyCode[] SelectTile = new KeyCode[] { KeyCode.F4 };
        public static readonly KeyCode[] ScrollLayer = new KeyCode[] { KeyCode.F5 };

        public static readonly KeyCode[] FreeCam_MacOS = new KeyCode[] { KeyCode.Alpha1 };
        public static readonly KeyCode[] SetTileTemplate_MacOS = new KeyCode[] { KeyCode.Alpha2 };
        public static readonly KeyCode[] RotateTile_MacOS = new KeyCode[] { KeyCode.Alpha3 };
        public static readonly KeyCode[] SelectTile_MacOS = new KeyCode[] { KeyCode.Alpha4 };
        public static readonly KeyCode[] ScrollLayer_MacOS = new KeyCode[] { KeyCode.Alpha5 };

    }

    public class LevelEditor : BaseSingleton<LevelEditor>
    {

#if UNITY_EDITOR_OSX

        [Header("Control+1 - Free Cam mode", order = -100)]
        [Header("Control+2 - Tile/Palette select", order = -100)]
        [Header("Control+3 - Rotate Tile", order = -100)]
        [Header("Control+4 - Scroll drawing layer", order = -100)]

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

                if (_level == null ||
                _level.gameObject == null)
                {
                    _level = FindObjectOfType<Level>();
                }
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
        public EditorMode Mode
        {
            get => _mode;
            set
            {
                _mode = value;
            }
        }

        public Tool _lastTool
            = Tool.None;

        [Header("Display")]
        [SerializeField]
        private bool _areDimensionsVisible = false;
        public bool AreDimensionsVisible => _areDimensionsVisible;
        [SerializeField]
        private Color _dimensionsColor = ColorUtils.LightBlue;
        [SerializeField]
        private Color _cursorColor = ColorUtils.LightBlue;
        [SerializeField]
        private Color _cursorSelectedColor = ColorUtils.LightGreen;
        [SerializeField]
        private Color _layerColor = ColorUtils.LightBlue;

        [Header("Info")]

        [SerializeField]
        private Cursor _cursor;

        [SerializeField]
        public Cursor Cursor {
            get 
            {
                if (_cursor == null)
                    _cursor = FindObjectOfType<Cursor>();

                return _cursor;
            }
        
        }

        [SerializeField]
        public Layer LayerX = new Layer();

        [SerializeField]
        public Layer LayerY = new Layer();

        [SerializeField]
        public Layer LayerZ = new Layer();


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

            if (Level == null || Level.gameObject == null) _level = FindObjectOfType<Level>();            

            EditorLibrary.Instance.DimensionsMaterial.color = _dimensionsColor;
            EditorLibrary.Instance.LayerMaterial.color = _layerColor;
            EditorLibrary.Instance.CursorDefaultMaterial.color = _cursorColor;
            EditorLibrary.Instance.CursorSelectedMaterial.color = _cursorSelectedColor;

            if (Level == null) return;

            if (Level.Dimensions != _dimensions)
            {
                Level._dimensions = _dimensions;
                EditorUtility.SetDirty(Level);
            }
        }

        [ExecuteInEditMode]
        public override void Awake()
        {
            base.Awake();

            _dimensions = _level.Dimensions;
            _dimensionsColor = EditorLibrary.Instance.DimensionsMaterial.color;
            _layerColor = EditorLibrary.Instance.LayerMaterial.color;
            _cursorColor = EditorLibrary.Instance.CursorDefaultMaterial.color;
            _cursorSelectedColor = EditorLibrary.Instance.CursorSelectedMaterial.color;
        }

        [ExecuteInEditMode]
        public override void Start()
        {
            base.Start();
        }

        [ExecuteInEditMode]
        public void Update()
        {
            //Vector2 mousePos = UnityEngine.Event.current.mousePosition;
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

        }
    }

    //#if UNITY_EDITOR
    [CustomEditor(typeof(LevelEditor))]
    public class LevelEditorCustom : UnityEditor.Editor
    {
        public static Delegate OnLevelSavedStaticHandler;

        private LevelEditor _editor;

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
            _editor.Cursor._selectedTileTemplate = selected;

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

                case EditorMode.SetTileTemplate:

                    // Change palete
                    if (Event.current.type == EventType.KeyDown)
                    {
                        if (Event.current.keyCode > KeyCode.Alpha0 &&
                            Event.current.keyCode <= KeyCode.Alpha9 &&
                            !Event.current.control)
                        {
                            int paletteIndex = Event.current.keyCode - KeyCode.Alpha0;
                            paletteIndex = IntegerUtils.Mod(paletteIndex - 1, 10);
                            _editor.Cursor.SelectedPaletteIndex = paletteIndex;

                            Event.current.Use();
                        }
                    }

                    // Change Tile
                    if (Event.current.type == EventType.ScrollWheel)
                    {
                        if (EventUtils.MouseWheelUp())
                        {
                            _editor.Cursor.SelectedTileIndex++;
                            Event.current.Use();
                        }
                        else if (EventUtils.MouseWheelDown())
                        {
                            _editor.Cursor.SelectedTileIndex--;
                            Event.current.Use();
                        }
                        
                    }

                    break;

                case EditorMode.RotateTile:

                    if (Event.current.type == EventType.ScrollWheel)
                    {
                        if (EventUtils.MouseWheelUp())
                        {
                            _editor.Cursor.RotationIndex++;
                            Event.current.Use();
                        }
                        else if (EventUtils.MouseWheelDown())
                        {
                            _editor.Cursor.RotationIndex--;
                            Event.current.Use();
                        }
                    }

                    break;

                case EditorMode.ScrollLayer:

                    // Change LayerMode
                    if (Event.current.type == EventType.KeyDown)
                    {
                        if (Event.current.keyCode > KeyCode.Alpha0 &&
                            Event.current.keyCode <= KeyCode.Alpha9 && 
                            !Event.current.control)
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
                            Event.current.Use();
                        }
                        else if (EventUtils.MouseWheelDown())
                        {
                            ScrollLayer(_editor.LayerMode, false);
                            InitPlane(_editor.LayerMode);
                            Event.current.Use();
                        }

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
                    else if (LevelEditorKeys.FreeCam.Contains(e.keyCode))
#elif UNITY_EDITOR_OSX
                   else if (e.control && LevelEditorKeys.FreeCam_MacOS.Contains(e.keyCode))
#endif
                    {
                        _editor.Mode = EditorMode.FreeCam;
                        Event.current.Use();
                    }
#if UNITY_EDITOR_WIN
                    else if (LevelEditorKeys.SetTileTemplate.Contains(e.keyCode))
#elif UNITY_EDITOR_OSX
                    else if (e.control && LevelEditorKeys.SetTileTemplate_MacOS.Contains(e.keyCode))                       
#endif
                    {
                        _editor.Mode = EditorMode.SetTileTemplate;
                        Event.current.Use();
                    }
#if UNITY_EDITOR_WIN
                    else if (LevelEditorKeys.RotateTile.Contains(e.keyCode))
#elif UNITY_EDITOR_OSX
                    else if (e.control && LevelEditorKeys.RotateTile_MacOS.Contains(e.keyCode))
#endif
                    {
                        _editor.Mode = EditorMode.RotateTile;
                        Event.current.Use();
                    }
#if UNITY_EDITOR_WIN
                    else if (LevelEditorKeys.ScrollLayer.Contains(e.keyCode))
#elif UNITY_EDITOR_OSX
                    else if (e.control && LevelEditorKeys.ScrollLayer_MacOS.Contains(e.keyCode))
#endif
                    {
                        _editor.Mode = EditorMode.ScrollLayer;
                        Event.current.Use();
                    }
#if UNITY_EDITOR_WIN
                    else if (LevelEditorKeys.SelectTile.Contains(e.keyCode))
#elif UNITY_EDITOR_OSX
                    else if (e.control && LevelEditorKeys.SelectTile_MacOS.Contains(e.keyCode))
#endif
                    {
                        _editor.Mode = EditorMode.SelectTile;
                        Event.current.Use();
                    }

                    break;
            }

        }

        //[DrawGizmo(GizmoType.Selected | GizmoType.Active)]
        //public void OnDrawGizmos()
        //{ 
        
        //}



        public void OnSceneGUI()
        {
            if (_editor.Level == null) return; 

            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

            HandleInputs();

            _editor._lastTool = Tools.current;
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
                _editor.Mode++;
                if (
                    (int)_editor._mode >=
                    EnumUtils.Size(typeof(EditorMode)))
                {
                    _editor.Mode = 0;
                }
            }


            GUILayout.EndArea();

            // Tile Buttons


            rect.height = screenRect.height / 8;
            rect.width = screenRect.width / 8;
            rect.position = new Vector2(screenRect.width /2, screenRect.height - 96);

            GUILayout.BeginArea(rect);

            if (GUILayout.Button(_editor.Cursor.SelectedPalette == null
                ? "?" :
                _editor.Cursor.SelectedPalette.name))
            {
                _editor.Cursor.SelectedPaletteIndex++;
            }

            if (GUILayout.Button(_editor.Cursor.SelectedTemplate == null
                ? "?" :
                _editor.Cursor.SelectedTemplate.name))
            {
                _editor.Cursor.SelectedTileIndex++;
            }


            GUILayout.EndArea();

#endregion

            //delete meshes of previous frame and draw new meshes
            EditorUtility.SetDirty(target);
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Break prefab"))
            {
                //_editor.Level.
                PrefabUtility.UnpackPrefabInstance(
                    _editor.Level.gameObject, 
                    PrefabUnpackMode.Completely, 
                    InteractionMode.AutomatedAction);
                
            }

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
                    foreach (
                        BaseObject obj in 
                        LevelEditor.Instance.Level.GetComponentsInChildren<BaseObject>())
                    {
                        if (obj == null) continue;
                        if (obj.gameObject.activeInHierarchy) continue;                        
                        obj.gameObject.DestroyImmediate();
                    }

                    PrefabUtility.SaveAsPrefabAssetAndConnect(
                        _editor.Level.gameObject, 
                        path, 
                        InteractionMode.AutomatedAction);

                    OnLevelSavedStaticHandler?.Invoke();
                }
            }
        }
    }

    //#endif

}