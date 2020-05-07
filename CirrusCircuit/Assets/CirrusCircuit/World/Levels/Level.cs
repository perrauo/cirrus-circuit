using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Cirrus.Circuit.World.Objects;
using Cirrus.Utils;
using System;

using System.Threading;
using Cirrus.Utils;
using Cirrus.Circuit.Networking;
using UnityEditor;

namespace Cirrus.Circuit.World
{
    public class Level : MonoBehaviour
    {
        private const int FallTrials = 100;

        public enum Rule
        {
            Timeout,
            RequiredGemsCollected,
        }

        public const int CellSize = 2;

        [SerializeField]
        private Vector3Int _offset = new Vector3Int(2, 2, 2);

        public Vector3Int Offset => _offset;

        [SerializeField]
        private Vector3Int _dimension = new Vector3Int(20, 20, 20);

        public Vector3Int Dimension => _dimension;    
        public int Size => Dimension.x * Dimension.y * Dimension.z;

        public const int MaxX = 25;
        public const int MaxY = 25;
        public const int MaxZ = 25;
        public const int MaxSize = MaxX * MaxY * MaxZ;

        [SerializeField]
        private BaseObject[] _objects;

        public IEnumerable<BaseObject> Objects => _objects;

        [SerializeField]
        private string _name;

        public string Name => _name;        

        [SerializeField]
        public float DistanceLevelSelection = 35;

        [SerializeField]
        public float CameraSize = 10;

        public Vector3 TargetPosition;

        [SerializeField]
        public float _positionSpeed = 0.4f;

        [SerializeField]
        private float _randomDropRainTime = 2f;
        public float RandomDropRainTime => _randomDropRainTime;

        //[SerializeField]
        //private float _randomDropSpawnTime = 2f;
        //public float RandomDropSpawnTime => _randomDropSpawnTime;

        public void OnValidate()
        {
            _name = gameObject.name.Substring(gameObject.name.IndexOf('.') + 1);
            _name = _name.Replace('.', ' ');
        }

        public void FixedUpdate()
        {
            transform.position = Vector3.Lerp(
                transform.position, 
                TargetPosition, 
                _positionSpeed);
        }

        public void Awake()
        {            
            
        }

        public Vector3Int WorldToGrid(Vector3 pos)
        {
            return
                new Vector3Int(
                    Mathf.RoundToInt(pos.x / CellSize) - _offset.x,
                    Mathf.RoundToInt(pos.y / CellSize) - _offset.y,
                    Mathf.RoundToInt(pos.z / CellSize) - _offset.z);
        }

        public Vector3 GridToWorld(Vector3Int pos)
        {
            return
                new Vector3(
                (pos.x + _offset.x) * CellSize,
                (pos.y + _offset.y) * CellSize,
                (pos.z + _offset.z) * CellSize);
        }

        public bool IsWithinBounds(Vector3Int pos)
        {
            return
                (pos.x >= 0 && pos.x < _dimension.x &&
                pos.y >= 0 && pos.y < _dimension.y &&
                pos.z >= 0 && pos.z < _dimension.z);
        }

        public bool IsWithinBoundsX(int pos)
        {
            return pos >= 0 && pos < _dimension.x;
        }

        public bool IsWithinBoundsY(int pos)
        {
            return pos >= 0 && pos < _dimension.y;
        }

        public bool IsWithinBoundsZ(int pos)
        {
            return pos >= 0 && pos < _dimension.z;
        }    

        public Vector3Int GetOverflow(Vector3Int pos)
        {
            return _dimension - pos;
        }

        public void Set(Vector3Int pos, BaseObject obj)
        {            
            int i = VectorUtils.ToIndex(pos, Dimension.x, Dimension.y);

            _objects[i] = obj;            
        }    

        public (Vector3, Vector3Int) RegisterObject(BaseObject obj)
        {
            Vector3Int pos = WorldToGrid(obj.Transform.position);

            int i = VectorUtils.ToIndex(pos, Dimension.x, Dimension.y);

            _objects[i] = obj;

            return (GridToWorld(pos), pos);
        }

        public void UpdateObjectReference()
        {
            _objects = new BaseObject[Dimension.x * Dimension.y * Dimension.z];
            foreach (var obj in gameObject.GetComponentsInChildren<BaseObject>(true))
            {
                if (obj == null) continue;
                obj.Register(this);
            }

#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
#endif
        }

        public void OnLevelSelect()
        {
            foreach (BaseObject obj in _objects)
            {
                if (obj == null)
                    continue;

                obj.TrySetState(BaseObject.State.LevelSelect);
            }
        }

        [HideInInspector]
        [SerializeField]
        public bool _levelDimensionsVisible = false;
    }

    #region Editor
#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(Level))]
    public class LevelEditor : UnityEditor.Editor
    {
        private Level _level;

        public virtual void OnEnable()
        {
            _level = serializedObject.targetObject as Level;

            var prop = serializedObject.FindProperty("_objects");
            
            if(prop != null) prop.isExpanded = false;
        }

        public void OnSceneGUI()
        {

            if (!_level._levelDimensionsVisible) return;

            int cellSize = Level.CellSize;


            var mesh = GraphicsUtils.CreateCube(
                _level.Dimension.x * cellSize,
                _level.Dimension.y * cellSize,
                _level.Dimension.z * cellSize);

            Graphics.DrawMesh(
                mesh,
                _level.transform.position +

                new Vector3(1, 0, 0) * (_level.Dimension.x * cellSize) / 2 -
                new Vector3(1, 0, 0) * cellSize / 2 +

                new Vector3(0, 1, 0) * (_level.Dimension.y * cellSize) / 2 -
                new Vector3(0, 1, 0) * cellSize / 2 +

                new Vector3(0, 0, 1) * (_level.Dimension.z * cellSize) / 2 -
                new Vector3(0, 0, 1) * cellSize / 2 +
                
                _level.Offset * cellSize,                                

                Quaternion.identity,
                LevelLibrary.Instance.EditorMaterial,
                0
                );

            //delete meshes of previous frame and draw new meshes
            EditorUtility.SetDirty(target);
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            //Called whenever the inspector is drawn for this object.
            //DrawDefaultInspector();

            if (GUILayout.Button("Update object references"))
            {
                _level.UpdateObjectReference();
            }

            if (GUILayout.Button("Show/Hide Level Dimensions"))
            {
                _level._levelDimensionsVisible = !_level._levelDimensionsVisible;
                EditorUtility.SetDirty(target);
            }
        }
    }

#endif

    #endregion
}