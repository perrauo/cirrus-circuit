using UnityEngine;
using System.Collections;
using UnityEditor;
using Cirrus;
using System.Linq;
using Cirrus.Circuit.World.Objects;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using Cirrus.UnityEditorExt;
using Castle.Core.Internal;
using UnityEngine.InputSystem.LowLevel;
using Cirrus.Collections;
using Cirrus.Circuit.World;
using System;
using Object = UnityEngine.Object;

using Event = UnityEngine.Event;

namespace Cirrus.Circuit.Editor
{
    public enum BrushMode
    {
        Terrain,
        Object,
        Select,
        Erase,
    }

    public static class CursorUtils
    {
        public const int MaxUndos = 40;

        public const float SelectRaycastMaxDistance = 120f;

    }


    public class Cursor : MonoBehaviour
    {
        [Header("R-Click - Draw", order = -100)]
        [Header("L-Click - Erase", order = -100)]
        [Header("Ctrl - Disable tile overwrite", order = -100)]

        public int _size = 1;

        [SerializeField]
        public List<Object> _tiles;
        
        [SerializeField]
        public Vector3Int _position = Vector3Int.zero;

        [SerializeField]
        private List<GameObject> _previews = new List<GameObject>();

        public Material _cursorMaterial = null;

        public Material Material 
        {
            set => _cursorMaterial = value;

            get {
                return _cursorMaterial == null ?
                    EditorLibrary.Instance.CursorDefaultMaterial :
                    _cursorMaterial;
            }
        }

        [SerializeField]
        private GameObject _preview = null;

        [SerializeField]
        public int _selectedTemplateIndex = 0;

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

        public float RotationAngle => _rotationIndex.IndexToAngle();   

        public int SelectedTileIndex
        {
            get => _selectedTemplateIndex;
            set
            {
                if (_tiles.Count == 0)
                {
                    _selectedTemplateIndex = 0;
                    return;
                }

                _selectedTemplateIndex = value;
                _selectedTemplateIndex = IntegerUtils.Wrap(
                    _selectedTemplateIndex, 0,
                   _tiles.Count);

                if (_selectedTemplateIndex >= 0) _selectedTileTemplate = _tiles[_selectedTemplateIndex];

                if (SelectedTemplate == null) return;

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

                if (EditorLibrary.Instance.Palettes.Length == 0)
                {
                    _selectedPaletteIndex = 0;
                    return;
                }

                _selectedPaletteIndex = value;
                _selectedPaletteIndex = IntegerUtils.Wrap(
                    _selectedPaletteIndex, 0,
                    EditorLibrary.Instance.Palettes.Length);

                _tiles = EditorLibrary.Instance.Palettes[_selectedPaletteIndex].Tiles.Select(x => (Object)x).ToList();

                UpdatePreview();
            }
        }

        public Palette SelectedPalette =>
            _selectedPaletteIndex >= EditorLibrary.Instance.Palettes.Length || _selectedPaletteIndex < 0 ?
                null :
                EditorLibrary.Instance.Palettes[_selectedPaletteIndex];

        [SerializeField]
        public Object _selectedTileTemplate;
        public BaseObject SelectedTemplate => _selectedTileTemplate == null ?
            null :
                _selectedTileTemplate is GameObject ?
          
                    (_selectedTileTemplate as GameObject).GetComponent<BaseObject>() :
                    (BaseObject)_selectedTileTemplate;

        [SerializeField]
        private BaseObject _selectedTile = null;

        [SerializeField]
        public Vector3Int _selectedStartPosition = Vector3Int.zero;

        [SerializeField]
        public int _selectedStartRotation = -1;

        public BaseObject SelectedTile
        {
            get => _selectedTile;
            set {
                _selectedTile = value;

                if (_selectedTile == null) return;

                _selectedStartPosition =
                    LevelEditor.Instance.Level.WorldToGrid(
                    _selectedTile.transform.position);

                _selectedStartRotation = _selectedTile.RotationIndex;


            }
        }

        public void UpdateCursor()
        {
            if (LevelEditor
                .Instance
                .Level == null) return;


            if (SelectedTile != null)
            {
                SelectedTile.transform.position =
                    LevelEditor
                    .Instance
                    .Level
                    .GridToWorld(_position);
            }

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
            if (SelectedTemplate == null) return;

            var template = SelectedTemplate.gameObject;

            if (template == null) return;

            _preview = template.Create(transform);
            if (_preview.TryGetComponent(out BaseObject obj))
            {
                obj.RotationIndex = RotationIndex;
            }

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

        public void DoErase(
            EditorAction action, 
            BaseObject target)
        {
            if (target.SelectedTemplate != null)
            {
                action.Erased.Add(new EditorObjectDescription
                {
                    Position = _position,
                    Rotation = RotationIndex,
                    Template = target.SelectedTemplate
                });

                target.gameObject.DestroyImmediate();
            }
            else
            {
                action.Erased.Add(new EditorObjectDescription
                {
                    Position = _position,
                    Rotation = target.Forward.DirectionToIndex(),
                    Template = target
                });
                target.gameObject.SetActive(false);
            }

            LevelEditor.Instance.Level.Set(_position, null);
        }

        public void DoDraw(EditorAction action, BaseObject target)
        {
            target.RotationIndex = RotationIndex;
            target._gridPosition = _position;
            target.SelectedTemplate = SelectedTemplate;
            target.Register(LevelEditor.Instance.Level, _position);
            action.Added.Add(new EditorObjectDescription
            {
                Position = _position,
                Rotation = RotationIndex,
                Template = SelectedTemplate
            });
        }

        public void DoMoveDraw(EditorAction action, BaseObject target)
        {
            target.RotationIndex = target.RotationIndex;
            target._gridPosition = _position;                                  
            target.Register(LevelEditor.Instance.Level, _position);
            action.Added.Add(new EditorObjectDescription
            {
                Position = _position,
                Rotation = RotationIndex,
                Template = target.SelectedTemplate == null ? target : target.SelectedTemplate
            });
        }

        public void DoMoveErase(EditorAction action, BaseObject target)
        {
            action.Erased.Add(new EditorObjectDescription
            {
                Position = _selectedStartPosition,
                Rotation = _selectedStartRotation,
                Template = target.SelectedTemplate == null ? target : target.SelectedTemplate
            });

            if(target.SelectedTemplate == null) target.gameObject.SetActive(false); 
            
            else target.gameObject.DestroyImmediate();

            LevelEditor.Instance.Level.Set(_selectedStartPosition, null);
        }

        public bool Draw(out EditorAction action)
        {
            action = new EditorAction
            {
                Type = ActionType.Draw,
                Position = _position,
                SelectedTile = SelectedTemplate
            };

            if (SelectedTemplate == null) return false;

            if (LevelEditor.Instance.Level.Get(
                _position,
                out BaseObject other))
            {
                DoErase(action, other);
            }

            DoDraw(
                action, 
                SelectedTemplate.Create(LevelEditor.Instance.Level.transform));

            EditorUtility.SetDirty(LevelEditor.Instance.Level);

            return true;
        }

        public bool Erase(out EditorAction action)
        {
            action = new EditorAction
            {
                Type = ActionType.Erase,
                Position = _position,
                SelectedTile = SelectedTemplate
            };

            if (SelectedTemplate == null) return false;


            if (LevelEditor.Instance.Level.Get(_position, out BaseObject other))
            {                             
                DoErase(action, other);
                EditorUtility.SetDirty(LevelEditor.Instance.Level);
                return true;
            }

            return false;
        }

        public bool Move(out EditorAction action)
        {
            action = null;

            if (SelectedTile == null) return false;

            action = new EditorAction
            {
                Type = ActionType.Draw,
                Position = _position,
            };
            
            DoMoveDraw(
                action,
                SelectedTile.Create(LevelEditor.Instance.Level.transform));

            DoMoveErase(action, SelectedTile);

            EditorUtility.SetDirty(LevelEditor.Instance.Level);

            return true;
        }





        #endregion
    }


    //#if UNITY_EDITOR
    [CustomEditor(typeof(Cursor))]
    public class CursorEditorCustom : UnityEditor.Editor
    {
        public bool _drawHeld = false;

        public bool _eraseHeld = false;

        public bool _controlHeld = false;
        
        private Cursor _cursor;

        private Mesh _cursorMesh;

        public CircularBuffer<EditorAction> _undos = new CircularBuffer<EditorAction>(CursorUtils.MaxUndos, true);

        public CircularBuffer<EditorAction> _redos = new CircularBuffer<EditorAction>(CursorUtils.MaxUndos, true);

        public EditorAction _lastAction;

        public int CellSize => Level.CellSize;

        [ExecuteInEditMode]
        public void OnEnable()
        {
            _undos.Clear();
            _redos.Clear();

            _cursor = serializedObject.targetObject as Cursor;

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

        public void Select(BaseObject tile)
        {
            _cursor.SelectedTile = tile;
            _cursor.Material = EditorLibrary.Instance.CursorSelectedMaterial;
            LevelEditor.Instance.Mode = EditorMode.ScrollLayer;
        }

        public void Unselect()
        {
            _cursor.SelectedTile = null;
            _cursor.Material = null;
        }

        public IEnumerable<MeshFilter> MeshFilters
        {
            get
            {
                var list = new List<MeshFilter>();
                foreach (var obj in LevelEditor.Instance.Level.NonNullObjects)
                {
                    if (obj == null) continue;
                    var filters = obj.GetComponentsInChildren<MeshFilter>();
                    list.AddRange(filters);
                }

                return list;
            }
        }

        public void OnSceneGUI()
        {
            HandleInputs();

            switch (LevelEditor.Instance.Mode)
            {
                case EditorMode.FreeCam:
                    _cursor.ClearPreview();
                    break;

                case EditorMode.SelectTile:
                    _cursor.ClearPreview();
                    break;

                case EditorMode.ScrollLayer:
                case EditorMode.RotateTile:
                    if(_cursor.SelectedTile != null) _cursor.ClearPreview();
                    break;

                case EditorMode.SetTileTemplate:
                    // Change currently selected tile
                    break;
            }


            var ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

            if (
                LevelEditor.Instance.Mode == EditorMode.SelectTile &&
                _cursor.SelectedTile == null)
            {

                if (Event.current.type == EventType.MouseDown &&
                    Event.current.button == 0)
                {                    

                    if (EditorHandles.Raycast(MeshFilters, ray, out GameObject hit))
                    {
                        if (hit == null) return;                        

                        var obj = hit.transform.gameObject.GetComponentInParent<BaseObject>();
                        if (obj != null)
                        {
                            Select(obj);
                        }
                    }

                    Event.current.Use();
                }
            }
            else
            {
                if (LevelEditor.Instance.SelectedLayer.Plane.Raycast(
                    ray,
                    out float enter))
                {
                    //Get the point that is clicked
                    Vector3 hitPoint = ray.GetPoint(enter);

                    //Move your cube GameObject to the point where you clicked
                    _cursor._position = LevelEditor.Instance.Level.WorldToGrid(hitPoint);
                    _cursor._position.Clamp(
                        Vector3Int.zero,
                        LevelEditor.Instance.Level.Dimensions - new Vector3Int(1, 1, 1));

                    Graphics.DrawMesh(
                        _cursorMesh,
                        //_editor.Level.transform.position +
                        LevelEditor.Instance.Level.GridToWorld(_cursor._position),
                        Quaternion.identity,
                        _cursor.Material,
                        0
                        );
                }
               
            }


            _cursor.UpdateCursor();

            //delete meshes of previous frame and draw new meshes
            EditorUtility.SetDirty(target);
        }

        #region Actions

        public void Draw()
        {
            if (_cursor.Draw(out EditorAction action))
            {
                if (action.Equals(_lastAction)) return;

                _lastAction = action;
                _redos.Clear();
                _undos.Push(action);
            }
        }

        public void Erase()
        {
            if (_cursor.Erase(out EditorAction action))
            {
                if (action.Equals(_lastAction)) return;

                _lastAction = action;
                _redos.Clear();
                _undos.Push(action);
            }
        }

        public void Move()
        {
            if (_cursor.Move(out EditorAction action))
            {
                if (action.Equals(_lastAction)) return;

                _lastAction = action;
                _redos.Clear();
                _undos.Push(action);
            }
        }

        public void RestoreSelectedMove()
        {
            // Do not commit action

            //if (_cursor.RestoreMove(out EditorAction action))
            //{
            //    if (action.Equals(_lastAction)) return;

            //    _lastAction = action;
            //    _redos.Clear();
            //    _undos.Push(action);
            //}
        }


        public bool Redo(EditorAction action)
        {
            switch (action.Type)
            {
                case ActionType.Draw:

                    foreach (var erased in action.Erased)
                    {
                        if (erased == null) continue;
                        if (erased.Template == null) continue;
                        if (LevelEditor.Instance.Level.Get(erased.Position, out BaseObject del))
                        {
                            del.gameObject.DestroyImmediate();
                            LevelEditor.Instance.Level.Set(erased.Position, null);
                        }
                    }

                    foreach (var added in action.Added)
                    {
                        if (added == null) continue;

                        if (LevelEditor.Instance.Level.Get(added.Position, out BaseObject del))
                        {
                            del.gameObject.DestroyImmediate();
                            LevelEditor.Instance.Level.Set(added.Position, null);
                        }

                        var obj = added.Template.Create(LevelEditor.Instance.Level.transform);
                        obj.gameObject.SetActive(true);
                        obj.SelectedTemplate = added.Template;
                        obj.RotationIndex = added.Rotation;
                        if (obj.Register(LevelEditor.Instance.Level, added.Position))
                        {
                            LevelEditor.Instance.Level.Set(added.Position, obj);
                        }
                    }

                    break;

                case ActionType.Erase:
                    foreach (var erased in action.Erased)
                    {                        
                        if (erased == null) continue;

                        if (LevelEditor.Instance.Level.Get(erased.Position, out BaseObject del))
                        {
                            del.gameObject.DestroyImmediate();
                            LevelEditor.Instance.Level.Set(erased.Position, null);
                        }

                        var obj = erased.Template.Create(LevelEditor.Instance.Level.transform);
                        obj.gameObject.SetActive(true);
                        obj.SelectedTemplate = erased.Template;
                        if (obj.Register(LevelEditor.Instance.Level, erased.Position))
                        {
                            LevelEditor.Instance.Level.Set(erased.Position, obj);
                        }                        
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



        public bool Undo(EditorAction action)
        {
            switch (action.Type)
            {
                case ActionType.Draw:

                    foreach (var added in action.Added)
                    {
                        if (added == null) continue;
                        //if (added.Item2 == null) continue;

                        if (LevelEditor.Instance.Level.Get(added.Position, out BaseObject obj))
                        {
                            obj.gameObject.DestroyImmediate();
                            LevelEditor.Instance.Level.Set(added.Position, null);
                        }
                    }

                    foreach (var erased in action.Erased)
                    {
                        if (erased == null) continue;
                        //if (erased.Item2 == null) continue;

                        var obj = erased.Template.Create(LevelEditor.Instance.Level.transform);
                        obj.gameObject.SetActive(true);
                        obj.SelectedTemplate = erased.Template;
                        obj.RotationIndex = erased.Rotation;
                        obj.Register(LevelEditor.Instance.Level, erased.Position);
                        LevelEditor.Instance.Level.Set(erased.Position, obj);
                    }

                    break;

                case ActionType.Erase:
                    foreach (var erased in action.Erased)
                    {
                        if (erased == null) continue;

                        if (LevelEditor.Instance.Level.Get(erased.Position, out BaseObject del))
                        {
                            del.gameObject.DestroyImmediate();
                            LevelEditor.Instance.Level.Set(erased.Position, null);
                        }

                        var obj = erased.Template.Create(LevelEditor.Instance.Level.transform);
                        obj.gameObject.SetActive(true);
                        obj.SelectedTemplate = erased.Template;
                        obj.RotationIndex = erased.Rotation;
                        obj.Register(LevelEditor.Instance.Level, erased.Position);
                        LevelEditor.Instance.Level.Set(erased.Position, obj);
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

        #endregion


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
                    else if (e.keyCode == KeyCode.Escape)
                    {
                        Unselect();
                        e.Use();
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
                        if (_cursor.SelectedTile != null)
                        {
                            Move();
                            Unselect();
                            LevelEditor.Instance.Mode = EditorMode.SelectTile;
                            e.Use();
                        }
                        else if (LevelEditor.Instance.Mode != EditorMode.SelectTile)
                        {
                            _drawHeld = true;
                            _eraseHeld = false;
                            e.Use();
                        }
                    }
                    else if (e.button == 1)
                    {
      
                        if (_cursor.SelectedTile != null)
                        {
                            RestoreSelectedMove();
                            e.Use();
                        }
                        else if (LevelEditor.Instance.Mode != EditorMode.SelectTile)
                        {
                            _drawHeld = false;
                            _eraseHeld = true;
                            e.Use();
                        }
                    }

                    break;


                case EventType.MouseUp:


                    if (e.button == 0) _drawHeld = false;

                    else if (e.button == 1) _eraseHeld = false;


                    e.Use();

                    break;

                default:
                    _cursor.InitPreview();
                    break;

            }

            #region Using brush, draw and erase

            // Draw
            if (_drawHeld) Draw();

            // Erase
            if (_eraseHeld) Erase();

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

            if (_cursor.SelectedPalette == null) return;

            _cursor.SelectedPaletteIndex = EditorGUILayout.Popup(
                _cursor.SelectedPaletteIndex,
                EditorLibrary.Instance.Palettes.Select(x => x.name).ToArray());

            _cursor.SelectedTileIndex = EditorGUILayout.Popup(
                _cursor.SelectedTileIndex,
                _cursor.SelectedPalette.Tiles.Select(x => x.name).ToArray());
        }
    }

        //#endif
}