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

    public class Status
    {
        public int GuideTileCount = 0;
    }

    public abstract class BaseObject : MonoBehaviour
    {
        [SerializeField]
        protected GameObject _visual;

        [SerializeField]
        protected Collider _collider;

        [SerializeField]
        protected float _stepDistance = 2f;

        [SerializeField]
        protected float _stepSpeed = 0.6f;

        protected Vector3 _targetPosition;

        protected float _targetScale = 1;

        protected float _scaleSpeeed = 0.6f;

        public bool _busy = false;

        protected virtual void Awake()
        {
            _targetPosition = transform.position;
        }

        public virtual bool TryEnter()
        {
            _busy = true;
            _collider.enabled = false;
            _targetScale = 0;
            return true;
        }

        public virtual bool TryMove(Vector3 step, Status stat=null, BaseObject incoming = null)
        {
            if
                (!_busy &&
                Utils.Vectors.CloseEnough(transform.position, _targetPosition))
            {
                if (
                    Physics.Raycast(
                        _targetPosition + Vector3.up,
                        step,
                        out RaycastHit hit,
                        Levels.Level.CubeSize / 2))
                {
                    BaseObject obj = hit.collider.GetComponent<BaseObject>();

                    if (obj != null)
                    {
                        if (obj.TryMove(step, stat, this))
                        {
                            stat.GuideTileCount++;
                            _targetPosition = _targetPosition + step;
                            return true;
                        }
                    }
                }
                else
                {
                    _targetPosition = _targetPosition + step;
                    return true;
                }
            }
            else
            {
                // TODO handle guide count
            }

            return false;
        }

        public string Name
        {
            get
            {
                if (transform.parent == null)
                    return "<Unknown>";
                else return transform.parent.name;
            }
        }

        public virtual void FixedUpdate()
        {
            transform.position = Vector3.Lerp(transform.position, _targetPosition, _stepSpeed);

            float scale = Mathf.Lerp(transform.localScale.x, _targetScale, _scaleSpeeed);
            transform.localScale = new Vector3(scale, scale, scale);
        }
    }
}
