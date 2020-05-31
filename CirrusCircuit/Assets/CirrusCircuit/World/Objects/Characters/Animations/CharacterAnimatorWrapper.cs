using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cirrus.Circuit.World.Objects.Characters
{
    public enum CharacterAnimation
    {
        Character_Idle=-1500613013,
        Character_Intro=-1379781672,
        Character_Walking=553373260,
        Character_Winning=-2118717581,
        Character_Pushing=-1927405582,
        Character_Losing=1885568713,
        Character_Landing=1736525134,
        Character_Falling=2089116047,
    }
    public interface ICharacterAnimatorWrapper
    {
        float GetStateSpeed(CharacterAnimation state);
        void Play(CharacterAnimation animation, float normalizedTime, bool reset=true);
        void Play(CharacterAnimation animation, bool reset=true);
        float BaseLayerLayerWeight{set;}
    }
    public class CharacterAnimatorWrapper : ICharacterAnimatorWrapper
    {
        private Animator _animator;
        private Dictionary<CharacterAnimation,float> _stateSpeedValues = new Dictionary<CharacterAnimation,float>();
        private Dictionary<CharacterAnimation,int> _stateLayerValues = new Dictionary<CharacterAnimation,int>();
        public void Play(CharacterAnimation animation, float normalizedTime, bool reset=true)
        {
            if (_animator == null) return; 
            if(!reset)
            {
                int layer = GetStateLayer(animation);
                var stateInfo = _animator.GetCurrentAnimatorStateInfo(layer);
                if((int)animation == stateInfo.GetHashCode())
                {
                    if (stateInfo.loop) return;
                    if (stateInfo.normalizedTime <= stateInfo.length) return;


                }
            }
            if(normalizedTime < 0) _animator.Play((int)animation);
            else _animator.Play((int)animation, -1, normalizedTime);
        }
        public void Play(CharacterAnimation animation, bool reset=true)
        {
            Play(animation, -1, reset);
        }
        public float BaseLayerLayerWeight{set { if(_animator != null) _animator.SetLayerWeight(0,value);} }
        public CharacterAnimatorWrapper(Animator animator)
        {
            if(animator==null)return;
            _animator = animator;
            _stateSpeedValues.Add(CharacterAnimation.Character_Idle,2);
            _stateLayerValues.Add(CharacterAnimation.Character_Idle,_animator.GetLayerIndex("Base Layer"));
            _stateSpeedValues.Add(CharacterAnimation.Character_Intro,1);
            _stateLayerValues.Add(CharacterAnimation.Character_Intro,_animator.GetLayerIndex("Base Layer"));
            _stateSpeedValues.Add(CharacterAnimation.Character_Walking,2);
            _stateLayerValues.Add(CharacterAnimation.Character_Walking,_animator.GetLayerIndex("Base Layer"));
            _stateSpeedValues.Add(CharacterAnimation.Character_Winning,1);
            _stateLayerValues.Add(CharacterAnimation.Character_Winning,_animator.GetLayerIndex("Base Layer"));
            _stateSpeedValues.Add(CharacterAnimation.Character_Pushing,1);
            _stateLayerValues.Add(CharacterAnimation.Character_Pushing,_animator.GetLayerIndex("Base Layer"));
            _stateSpeedValues.Add(CharacterAnimation.Character_Losing,1);
            _stateLayerValues.Add(CharacterAnimation.Character_Losing,_animator.GetLayerIndex("Base Layer"));
            _stateSpeedValues.Add(CharacterAnimation.Character_Landing,1);
            _stateLayerValues.Add(CharacterAnimation.Character_Landing,_animator.GetLayerIndex("Base Layer"));
            _stateSpeedValues.Add(CharacterAnimation.Character_Falling,1);
            _stateLayerValues.Add(CharacterAnimation.Character_Falling,_animator.GetLayerIndex("Base Layer"));
        }
        public float GetStateSpeed(CharacterAnimation state)
        {
            if(_stateSpeedValues.TryGetValue(state, out float res)) return res;
            return -1f;
        }
        public int GetStateLayer(CharacterAnimation state)
        {
            if(_stateLayerValues.TryGetValue(state, out int res)) return res;
            return -1;
        }
        public float GetClipLength(CharacterAnimation state)
        {
            return -1f;
        }
    }
}
