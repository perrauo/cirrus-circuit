using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Cirrus.Circuit.World.Objects;
using Cirrus;
using UnityEditor;
using UnityEngine.Serialization;

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
        [FormerlySerializedAs("_dimension")]
        public Vector3Int _dimensions = new Vector3Int(20, 20, 20);

        public Vector3Int Dimensions => _dimensions;    
        public int Size => Dimensions.x * Dimensions.y * Dimensions.z;

        public const int MaxX = 25;
        public const int MaxY = 25;
        public const int MaxZ = 25;
        public const int MaxSize = MaxX * MaxY * MaxZ;

        [SerializeField]
        private BaseObject[] _objects;

        public static Level Instance => LevelSession.Instance.Level;

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

        #region Bounds

        public bool IsInsideBounds(Vector3Int pos)
        {
            return
                (pos.x >= 0 && pos.x < _dimensions.x &&
                pos.y >= 0 && pos.y < _dimensions.y &&
                pos.z >= 0 && pos.z < _dimensions.z);
        }

        public bool IsInsideBoundsX(int pos)
        {
            return pos >= 0 && pos < _dimensions.x;
        }

        public bool IsInsideBoundsY(int pos)
        {
            return pos >= 0 && pos < _dimensions.y;
        }

        public bool IsInsideBoundsZ(int pos)
        {
            return pos >= 0 && pos < _dimensions.z;
        }


        public bool IsInsideBoundsX(Vector3Int pos)
        {
            return pos.x >= 0 && pos.x < _dimensions.x;
        }

        public bool IsInsideBoundsY(Vector3Int pos)
        {
            return pos.y >= 0 && pos.y < _dimensions.y;
        }

        public bool IsInsideBoundsZ(Vector3Int pos)
        {
            return pos.z >= 0 && pos.z < _dimensions.z;
        }

        public Vector3Int GetOverflow(Vector3Int pos)
        {
            return _dimensions - pos;
        }

        #endregion


        public void Set(Vector3Int pos, BaseObject obj)
        {            
            int i = VectorUtils.ToIndex(pos, Dimensions.x, Dimensions.y);

            _objects[i] = obj;            
        }    

        public (Vector3, Vector3Int) RegisterObject(BaseObject obj)
        {
            Vector3Int pos = WorldToGrid(obj.Transform.position);

            int i = VectorUtils.ToIndex(pos, Dimensions.x, Dimensions.y);

            _objects[i] = obj;

            return (GridToWorld(pos), pos);
        }

        public bool RegisterObject(BaseObject obj, Vector3Int pos)
        {
            if (!IsInsideBounds(pos)) return false;

            int i = VectorUtils.ToIndex(pos, Dimensions.x, Dimensions.y);

            _objects[i] = obj;

            return true;
        }


        public bool Get(
            Vector3Int pos,
            out BaseObject obj)
        {
            obj = null;
            if (!IsInsideBounds(pos)) return false;
            

            int i = VectorUtils.ToIndex(pos, Dimensions.x, Dimensions.y);
            obj = _objects[i];

            return obj != null;
        }


        //public void Set(
        //    Vector3Int pos,
        //    BaseObject obj)
        //{
        //    int i = VectorUtils.ToIndex(pos, Dimensions.x, Dimensions.y);

        //    _objects[i] = obj;

        //}

        public void UpdateObjectReference()
        {
            _objects = new BaseObject[Dimensions.x * Dimensions.y * Dimensions.z];
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

                obj.InitState(ObjectState.LevelSelect, null);
            }
        } 
    }

    #region Editor
#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(Level))]
    public class LevelCustomInspector : UnityEditor.Editor
    {
        private Level _level;

        public virtual void OnEnable()
        {
            _level = serializedObject.targetObject as Level;

            var prop = serializedObject.FindProperty("_objects");
            
            if(prop != null) prop.isExpanded = false;
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
        }
    }

#endif

    #endregion
}