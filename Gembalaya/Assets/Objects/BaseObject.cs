//using Cirrus.DH.Actions;
//using Cirrus.DH.Objects.Characters;
//using Cirrus.DH.Objects.Characters.Controls;
using System.Collections.Generic;
using UnityEngine;
using Cirrus.Tags;
//using Cirrus.DH.Conditions;
//using Cirrus.DH.Objects.Actions;

namespace Cirrus.Gembalaya.Objects
{
    public delegate void OnObjectCollided(BaseObject other);

    public abstract class BaseObject : MonoBehaviour
    {
        public string Name
        {
            get
            {
                if (transform.parent == null)
                    return "<Unknown>";
                else return transform.parent.name;
            }
        }

        [SerializeField]
        private List<Tag> _tags;

        public IEnumerable<Tag> Tags
        {
            get
            {
                return _tags;
            }
        }


        [SerializeField]
        private List<Object> _lists;


        [SerializeField]
        public ObjectPhysic Physic;




        [HideInInspector]
        public Vector3 StartPosition;


        public OnObjectCollided OnObjectCollidedHandler;

        [SerializeField]
        private UnityEngine.AI.NavMeshModifierVolume _navMeshModifierVolume;
        public UnityEngine.AI.NavMeshModifierVolume NavMeshModifierVolume { get { return _navMeshModifierVolume; } }

        protected virtual void Awake()
        {
            StartPosition = transform.position;
            foreach (IList<BaseObject> list in _lists)
            {
                if (list != null)
                    list.Add(this);
            }
        }


        // TODO: remove cap by player speed for push (suppor
        public virtual void OnObjectCollision(BaseObject other)
        {
            // calculate force vector
            var force = transform.position - other.transform.position;
            // normalize force vector to get direction only and trim magnitude
            force.Normalize();

            Physic.MoveVelocity += force * other.Physic.PushCoefficient * other.Physic.Mass;

            OnObjectCollidedHandler?.Invoke(other);
        }


        public virtual void OnValidate()
        {
            if (Physic == null)
            {
                Physic = GetComponent<ObjectPhysic>();
            }
        }
    }
}
