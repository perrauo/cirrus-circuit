using UnityEngine;
using System.Collections;
using UnityEditor;
using Cirrus.Utils;
using System.Linq;
using Cirrus.Circuit.World.Objects;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using Cirrus.Editor;
using Castle.Core.Internal;
using UnityEngine.InputSystem.LowLevel;
using Cirrus.Collections;
using Cirrus.Circuit.World;

namespace Cirrus.Circuit.Editor
{
    public enum BrushMode
    {
        Terrain,
        Object,
        Select,
        Erase,
    }

    public class Brush : MonoBehaviour
    {
        [Header("R-Click - Draw", order = -100)]
        [Header("L-Click - Erase", order = -100)]
        [Header("Ctrl - Disable tile overwrite", order = -100)]

        public int _size = 1;

        [SerializeField]
        public List<Object> _tiles;
        
        [SerializeField]
        public List<Palette> _palettes;

        [SerializeField]
        public Vector3Int _position = Vector3Int.zero;

        [SerializeField]
        public EditorAction _lastAction;

        [SerializeField]
        private List<GameObject> _previews = new List<GameObject>();

        [SerializeField]
        private GameObject _preview = null;

        [SerializeField]
        public int _selectedTileIndex = 0;

        public int _rotationIndex = 0;        
        public int RotationIndex
        {
            get => _rotationIndex;
            set
            {
                _rotationIndex = IntegerUtils.Wrap(value, 0, 4);
                UpdatePreview();
            }
        }

        public float Rotation
        {
            get
            {
                switch (RotationIndex)
                {
                    case 0: return 0;
                    case 1: return 90;
                    case 2: return 180;
                    case 3: return 270;
                    default: return 0;
                }
            
            }
        }

        public int SelectedTileIndex
        {
            get => _selectedTileIndex;
            set
            {
                if (_tiles.Count == 0)
                {
                    _selectedTileIndex = 0;
                    return;
                }

                _selectedTileIndex = value;
                _selectedTileIndex = IntegerUtils.Wrap(
                    _selectedTileIndex, 0,
                   _tiles.Count);

                if (_selectedTileIndex >= 0) _selectedTileObject = _tiles[_selectedTileIndex];

                if (SelectedTile == null) return;

                UpdatePreview();
            }
        }

        [SerializeField]
        public int _selectedPaletteIndex = 0;
        public int SelectedPaletteIndex
        {
            get => _selectedPaletteIndex;
            set
            {

                if (_palettes.Count == 0)
                {
                    _selectedPaletteIndex = 0;
                    return;
                }

                _selectedPaletteIndex = value;
                _selectedPaletteIndex = IntegerUtils.Wrap(
                    _selectedPaletteIndex, 0,
                    _palettes.Count);

                _tiles = _palettes[_selectedPaletteIndex].Tiles.Select(x => (Object)x).ToList();

                UpdatePreview();
            }
        }

        public Palette SelectedPalette
        {
            get
            {
                if (_selectedPaletteIndex >= _palettes.Count || _selectedPaletteIndex < 0) return null;
                return _palettes[_selectedPaletteIndex];
            }
        }

        [SerializeField]
        public Object _selectedTileObject;
        public BaseObject SelectedTile => _selectedTileObject == null ?
            null :
                _selectedTileObject is GameObject ?
          
                    (_selectedTileObject as GameObject).GetComponent<BaseObject>() :
                    (BaseObject)_selectedTileObject;


        public void Update()
        {
            if (_preview == null) return;

            _preview.transform.position =
                LevelEditor
                .Instance
                .Level
                .GridToWorld(_position);

        }

        public void OnValidate()
        {
            //if (_tiles.IsNullOrEmpty())
            //{
            //    _tiles = AssetDatabaseUtils
            //        .FindObjectsOfType<Object>()
            //        .Where(x =>
            //            x is IEditorTile ||
            //            (
            //                x is GameObject &&
            //                ((GameObject)x).TryGetComponent(out IEditorTile component) &&
            //                component.IsAvailableInEditor
            //            ))
            //        .ToList();
            //}
        }

        #region Brush Preview

        public void InitPreview()
        {
            if (_preview == null)
            {
                CreatePreview();
            }
        }

        public void UpdatePreview()
        {
            ClearPreview();
            CreatePreview();
        }

        public void CreatePreview()
        {
            var template = SelectedTile.gameObject;

            if (template == null) return;

            _preview = template.Create(transform);
            _preview.transform.Rotate(Vector3.up, Rotation);
            _previews.Add(_preview);
        }

        public void ClearPreview()
        {
            if (_previews.Count != 0)
            {
                foreach (var prev in _previews)
                {
                    prev.DestroyImmediate();
                }

                _previews.Clear();
            }

            _preview = null;
        }

        #endregion

        #region Draw/Erase

        public bool Draw(out EditorAction action)
        {
            action = new EditorAction
            {
                Type = ActionType.Draw,
                Position = _position,
                SelectedTile = SelectedTile
            };

            if (SelectedTile == null) return false;

            if (_lastAction.Equals(action)) return false;

            if (LevelEditor.Instance.Level.Get(
                _position,
                out BaseObject other))
            {
                action.Erased.Add(other);
                other.gameObject.SetActive(false);
            }

            var tile = Instantiate(SelectedTile.gameObject, LevelEditor.Instance.Level.transform);
            tile.transform.Rotate(Vector3.up, Rotation);

            tile.transform.position = LevelEditor.Instance.Level.GridToWorld(_position);

            if (tile.TryGetComponent(out BaseObject obj))
            {
                obj._gridPosition = _position;
                obj.Register(LevelEditor.Instance.Level);
                action.Added.Add(obj);

                _lastAction = action;

                EditorUtility.SetDirty(LevelEditor.Instance.Level);

                return true;
            }

            return false;
        }

        public bool Erase(out EditorAction action)
        {
            action = new EditorAction
            {
                Type = ActionType.Erase,
                Position = _position,
                SelectedTile = SelectedTile
            };

            if (SelectedTile == null) return false;

            if (_lastAction.Equals(action)) return false;

            if (LevelEditor.Instance.Level.Get(_position, out BaseObject other))
            {                
                other.gameObject.SetActive(false);

                LevelEditor.Instance.Level.Set(_position, null);

                action.Erased.Add(other);
                _lastAction = action;

                EditorUtility.SetDirty(LevelEditor.Instance.Level);
                return true;
            }

            return false;
        }





        #endregion
    }


    //#if UNITY_EDITOR
    [CustomEditor(typeof(Brush))]
    public class BrushCustomInspector : UnityEditor.Editor
    {
        public bool _drawHeld = false;

        public bool _eraseHeld = false;

        public bool _controlHeld = false;

        //public EditorAction _heldAction;
        
        private const int MaxUndos = 40;

        private Brush _brush;

        private Mesh _cursorMesh;

        public CircularBuffer<EditorAction> _undos = new CircularBuffer<EditorAction>(MaxUndos, true);

        public CircularBuffer<EditorAction> _redos = new CircularBuffer<EditorAction>(MaxUndos, true);


        public int CellSize => Level.CellSize;

        [ExecuteInEditMode]
        public void OnEnable()
        {
            _undos.Clear();
            _redos.Clear();

            _brush = serializedObject.targetObject as Brush;

            LevelEditorCustom.OnLevelSavedStaticHandler = null;
            LevelEditorCustom.OnLevelSavedStaticHandler += OnLevelSaved;

            _cursorMesh = GraphicsUtils.CreateCube(
                CellSize,
                CellSize,
                CellSize);           
        }

        public void OnDisable()
        {
            LevelEditorCustom.OnLevelSavedStaticHandler -= OnLevelSaved;
        }

        public void OnLevelSaved()
        {
            _undos.Clear();
            _redos.Clear();
        }


        public void OnSceneGUI()
        {
            HandleInputs();

            if(LevelEditor.Instance.Mode == EditorMode.FreeCam) _brush.ClearPreview();   

            // Draw Cursor

            var cam = SceneView.currentDrawingSceneView.camera;      

            var ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

            if (LevelEditor.Instance.SelectedLayer.Plane.Raycast(
                ray,
                out float enter))
            {
                //Get the point that is clicked
                Vector3 hitPoint = ray.GetPoint(enter);

                //Move your cube GameObject to the point where you clicked
                _brush._position = LevelEditor.Instance.Level.WorldToGrid(hitPoint);
                _brush._position.Clamp(
                    Vector3Int.zero,
                    LevelEditor.Instance.Level.Dimensions - new Vector3Int(1, 1, 1));

                Graphics.DrawMesh(
                    _cursorMesh,
                    //_editor.Level.transform.position +
                    LevelEditor.Instance.Level.GridToWorld(_brush._position),
                    Quaternion.identity,
                    EditorLibrary.Instance.CursorMaterial,
                    0
                    );
            }

            _brush.Update();

            //delete meshes of previous frame and draw new meshes
            EditorUtility.SetDirty(target);
        }

        public void ClearRedos()
        {
            foreach (var redo in _redos)
            {
                if (redo == null) continue;

                foreach (var added in redo.Added)
                {
                    if (added == null) continue;
                    if (added.gameObject == null) continue;

                    DestroyImmediate(added.gameObject);
                }
            }

            _redos.Clear();
        }


        public bool Undo(EditorAction action)
        {
            switch (action.Type)
            {
                case ActionType.Draw:
                    foreach (var erased in action.Erased)
                    {
                        if (erased == null) continue;
                        erased.gameObject.SetActive(true);
                        LevelEditor.Instance.Level.Set(erased._gridPosition, erased);
                    }

                    foreach (var added in action.Added)
                    {
                        if (added == null) continue;
                        added.gameObject.SetActive(false);
                    }

                    break;

                case ActionType.Erase:
                    foreach (var erased in action.Erased)
                    {
                        if (erased == null) continue;
                        erased.gameObject.SetActive(true);
                        LevelEditor.Instance.Level.Set(erased._gridPosition, erased);
                    }

                    //foreach (var added in action.Added)
                    //{
                    //    if (added == null) continue;
                    //    added.gameObject.SetActive(false);
                    //}



                    break;
                case ActionType.Fill:

                    break;

                case ActionType.Move:
                    break;

                default: return false;
            
            }



            return true;
        }


        public bool Redo(EditorAction action)
        {
            switch (action.Type)
            {
                case ActionType.Draw:
                    foreach (var erased in action.Added)
                    {
                        if (erased == null) continue;
                        erased.gameObject.SetActive(true);
                        LevelEditor.Instance.Level.Set(erased._gridPosition, erased);
                    }

                    foreach (var added in action.Erased)
                    {
                        if (added == null) continue;
                        added.gameObject.SetActive(false);
                    }

                    break;

                case ActionType.Erase:
                    foreach (var erased in action.Erased)
                    {
                        if (erased == null) continue;
                        erased.gameObject.SetActive(true);
                        LevelEditor.Instance.Level.Set(erased._gridPosition, null);
                    }


                    break;
                case ActionType.Fill:

                    break;

                case ActionType.Move:
                    break;

                default: return false;

            }



            return true;
        }



        public void HandleInputs()
        {
            Event e = Event.current;

            bool ctrlZPressed = false;
            bool ctrlYPressed = false;

            if (LevelEditor.Instance.Mode == EditorMode.FreeCam)
            {
                _drawHeld = false;
                _eraseHeld = false;
                return;

            }

            switch (e.type)
            {
                case EventType.KeyDown:

                    if (
                        e.keyCode == KeyCode.LeftControl || 
                        e.keyCode == KeyCode.RightControl)
                    {
                        _controlHeld = true;
                        e.Use();
                    }
                    else if (e.keyCode == KeyCode.Z)                        
                    {
                        if (_controlHeld)
                        {
                            ctrlZPressed = true;
                            e.Use();
                        }
                    }
                    else if (e.keyCode == KeyCode.Y)
                    {
                        if (_controlHeld)
                        {
                            ctrlYPressed = true;
                            e.Use();
                        }
                    }

                    break;

                case EventType.KeyUp:

                    if (
                        e.keyCode == KeyCode.LeftControl ||
                        e.keyCode == KeyCode.RightControl)
                    {
                        _controlHeld = false;
                        e.Use();
                    }

                    break;


                case EventType.MouseDown:

                    if (e.button == 0)
                    {
                        _drawHeld = true;
                        _eraseHeld = false;
                        e.Use();
                    }
                    else if (e.button == 1)
                    {
                        _drawHeld = false;
                        _eraseHeld = true;
                        e.Use();
                    }

                    break;


                case EventType.MouseUp:


                    if (e.button == 0)
                    {
                        _drawHeld = false;
                    }
                    else if (e.button == 1)
                    {
                        _eraseHeld = false;
                    }

                    e.Use();

                    break;


                default:
                    _brush.InitPreview();
                    break;

            }


            #region Using brush, draw and erase

            // Draw
            if (_drawHeld)
            {
                if (_brush.Draw(out EditorAction action))
                {
                    _redos.Clear();
                    _undos.Push(action);
                }
            }

            // Erase
            if (_eraseHeld)
            {
                if (_brush.Erase(out EditorAction action))
                {
                    _redos.Clear();
                    _undos.Push(action);
                }
            }


            if (ctrlZPressed)
            {
                if (_undos.PopBack(out EditorAction action))
                {
                    Undo(action);
                    _redos.Push(action);
                }
            }
            else if (ctrlYPressed)
            {
                if (_redos.PopBack(out EditorAction action))
                {
                    Redo(action);
                    _undos.Push(action);
                }
            }

        }

        #endregion

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (_brush.SelectedPalette == null) return;

            _brush.SelectedPaletteIndex = EditorGUILayout.Popup(
                _brush.SelectedPaletteIndex,
                _brush._palettes.Select(x => x.name).ToArray());

            _brush.SelectedTileIndex = EditorGUILayout.Popup(
                _brush.SelectedTileIndex,
                _brush.SelectedPalette.Tiles.Select(x => x.name).ToArray());
        }
    }

        //#endif
}