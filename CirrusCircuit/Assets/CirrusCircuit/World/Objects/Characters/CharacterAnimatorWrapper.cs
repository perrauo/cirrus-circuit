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
        Character_Triumphant=26048826,
        Character_Pushing=-1927405582,
        Character_Losing=1885568713,
        Character_Landing=1736525134,
        Character_Falling=2089116047,
    }
    public interface ICharacterAnimatorWrapper
    {
        float GetStateSpeed(CharacterAnimation state);
        void Play(CharacterAnimation animation, float normalizedTime);
        void Play(CharacterAnimation animation);
        float BaseLayerLayerWeight{set;}
    }
    public class CharacterAnimatorWrapper : ICharacterAnimatorWrapper
    {
        private Animator _animator;
        private Dictionary<CharacterAnimation,float> _stateSpeedValues = new Dictionary<CharacterAnimation,float>();
        public void Play(CharacterAnimation animation, float normalizedTime)
        {
            if(_animator != null)_animator.Play((int)animation, -1, normalizedTime);
        }
        public void Play(CharacterAnimation animation)
        {
            if(_animator != null)_animator.Play((int)animation);
        }
        public float BaseLayerLayerWeight{set { if(_animator != null) _animator.SetLayerWeight(0,value);} }
        public CharacterAnimatorWrapper(Animator animator)
        {
            _animator = animator;
            _stateSpeedValues.Add(CharacterAnimation.Character_Idle,1);
            _stateSpeedValues.Add(CharacterAnimation.Character_Intro,1);
            _stateSpeedValues.Add(CharacterAnimation.Character_Walking,1);
            _stateSpeedValues.Add(CharacterAnimation.Character_Triumphant,1);
            _stateSpeedValues.Add(CharacterAnimation.Character_Pushing,1);
            _stateSpeedValues.Add(CharacterAnimation.Character_Losing,1);
            _stateSpeedValues.Add(CharacterAnimation.Character_Landing,1);
            _stateSpeedValues.Add(CharacterAnimation.Character_Falling,1);
        }
        public float GetStateSpeed(CharacterAnimation state)
        {
            if(_stateSpeedValues.TryGetValue(state, out float res)) return res;
            return -1f;
        }
        public float GetClipLength(CharacterAnimation state)
        {
            return -1f;
        }
    }
}
