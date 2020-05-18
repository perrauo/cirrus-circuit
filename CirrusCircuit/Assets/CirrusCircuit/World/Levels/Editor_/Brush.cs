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

namespace Cirrus.Circuit.World.Editor
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
        public BrushMode _brushMode;
        public BrushMode BrushMode => _brushMode;

        [SerializeField]
        public Vector3Int _potition = Vector3Int.zero;

        [SerializeField]
        private List<GameObject> _previews = new List<GameObject>();

        [SerializeField]
        private GameObject _preview = null;

        [SerializeField]
        public bool _drawHeld = false;

        [SerializeField]
        public bool _eraseHeld = false;

        [SerializeField]
        public bool _tileOverwriteHeld = false;

        [SerializeField]
        public int _selectedTileIndex = 0;

        public int _rotationIndex = 0;        
        public int RotationIndex
        {
            get => _rotationIndex;
            set
            {
                _rotationIndex = IntegerUtils.Wrap(value, 0, 3);
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
                .GridToWorld(_potition);

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

        public void Draw()
        {
            // TODO if draw
            if (SelectedTile == null) return;

            bool result = LevelEditor.Instance.Level.Get(
                _potition, 
                out BaseObject other);

            if (!result || _tileOverwriteHeld)
            {
                if (result) other.gameObject.DestroyImmediate();

                var tile = Instantiate(SelectedTile.gameObject, LevelEditor.Instance.Level.transform);
                tile.transform.Rotate(Vector3.up, Rotation);

                tile.transform.position = LevelEditor.Instance.Level.GridToWorld(_potition);

                if (tile.TryGetComponent(out BaseObject obj))
                {
                    obj._gridPosition = _potition;
                    obj.Register(LevelEditor.Instance.Level);
                }

                EditorUtility.SetDirty(LevelEditor.Instance.Level);
            }
        }


        public void Erase()
        {
            // TODO if draw            

            bool result = LevelEditor.Instance.Level.Get(_potition, out BaseObject other);

            if (result)
            {
                other.gameObject.DestroyImmediate();
                LevelEditor.Instance.Level.Set(_potition, null);

                EditorUtility.SetDirty(LevelEditor.Instance.Level);
            }


        }

        #endregion
    }


    //#if UNITY_EDITOR
    [CustomEditor(typeof(Brush))]
    public class BrushCustomInspector : UnityEditor.Editor
    {
        private Brush _brush;

        private Mesh _cursorMesh;

        public int CellSize => Level.CellSize;

        public void OnEnable()
        {
            _brush = serializedObject.targetObject as Brush;

            _cursorMesh = GraphicsUtils.CreateCube(
                CellSize,
                CellSize,
                CellSize);
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
                _brush._potition = LevelEditor.Instance.Level.WorldToGrid(hitPoint);
                _brush._potition.Clamp(
                    Vector3Int.zero,
                    LevelEditor.Instance.Level.Dimensions - new Vector3Int(1, 1, 1));

                Graphics.DrawMesh(
                    _cursorMesh,
                    //_editor.Level.transform.position +
                    LevelEditor.Instance.Level.GridToWorld(_brush._potition),
                    Quaternion.identity,
                    EditorLibrary.Instance.CursorMaterial,
                    0
                    );
            }

            _brush.Update();

            //delete meshes of previous frame and draw new meshes
            EditorUtility.SetDirty(target);
        }

        

        public void HandleInputs()
        {
            Event e = Event.current;

            if (LevelEditor.Instance.Mode == EditorMode.FreeCam)
            {
                _brush._drawHeld = false;
                _brush._eraseHeld = false;
                _brush._tileOverwriteHeld = false;
                return;

            }

            switch (e.type)
            {
                case EventType.KeyDown:

                    if (e.keyCode == KeyCode.LeftControl || e.keyCode == KeyCode.RightControl)
                    {
                        _brush._tileOverwriteHeld = true;
                        e.Use();
                    }

                    break;

                case EventType.KeyUp:
       
                    if (e.keyCode == KeyCode.LeftControl || e.keyCode == KeyCode.RightControl)
                    {
                        _brush._tileOverwriteHeld = false;
                        e.Use();
                    }

                    break;


                case EventType.MouseDown:

                    if (e.button == 0)
                    {
                        _brush._drawHeld = true;
                        _brush._eraseHeld = false;
                    }
                    else if (e.button == 1)
                    {
                        _brush._drawHeld = false;
                        _brush._eraseHeld = true;
                    }

                    e.Use();

                    break;


                case EventType.MouseUp:


                    if (e.button == 0)
                    {
                        _brush._drawHeld = false;
                    }
                    else if (e.button == 1)
                    {
                        _brush._eraseHeld = false;
                    }

                    e.Use();

                    break;


                default:
                    _brush.InitPreview();
                    break;

            }

            if (_brush._drawHeld) _brush.Draw();
            else if (_brush._eraseHeld) _brush.Erase();


        }

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