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

namespace Cirrus.Circuit.World.Editor
{
    public interface IEditorTile
    {
        bool IsAvailableInEditor { get; }

        GameObject GetPreview(Level level, Vector3Int position);

        void Draw(Level level, Vector3Int position);        
    }

    public enum BrushMode
    {
        Terrain,
        Object,
        Select,
        Erase,
    }

    public class Brush : MonoBehaviour
    {
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
        private GameObject _preview = null;

        [SerializeField]
        public int _selectedTileIndex = 0;

        public void UpdatePreview()
        {
            if (_preview != null)
            {
                _preview.DestroyImmediate();
                _preview = null;
            }

            var template = SelectedTile.GetPreview(
                    LevelEditor.Instance.Level,
                    _potition);

            if (template == null) return;

            _preview = template.Create(transform);
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
            set {

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
            get {
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

        public void Use(Level level)
        {
            // TODO if draw
            if (SelectedTile == null) return;

            SelectedTile.Draw(level, _potition);
        }


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
            #region Cursor            

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


            #endregion

            _brush.Update();

            //delete meshes of previous frame and draw new meshes
            EditorUtility.SetDirty(target);
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