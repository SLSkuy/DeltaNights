using System;
using System.Collections;
using UIFramework.UIAnimation;
using UnityEngine;

namespace UIFramework.Examples
{
    /// <summary>
    /// 单个UI界面具体动画实现
    /// </summary>
    public class AnimationView : AnimComponent
    {
        [SerializeField] private AnimationClip clip = null;
        [SerializeField] private bool playReverse = false;

        private Action _previousCallbackWhenFinished;
        
        public override void Animate(Transform target, Action callWhenFinished) {
            FinishPrevious();
            var targetAnimation = target.GetComponent<Animation>();
            if (targetAnimation == null) {
                Debug.LogError("[LegacyAnimationScreenTransition] No Animation component in " + target);
                if (callWhenFinished != null) {
                    callWhenFinished();
                }

                return;
            }

            targetAnimation.clip = clip;
            StartCoroutine(PlayAnimationRoutine(targetAnimation, callWhenFinished));
        }

        private IEnumerator PlayAnimationRoutine(Animation targetAnimation, Action callWhenFinished) {
            _previousCallbackWhenFinished = callWhenFinished;
            foreach (AnimationState state in targetAnimation) {
                state.time = playReverse ? state.clip.length : 0f;
                state.speed = playReverse ? -1f : 1f;
            }

            targetAnimation.Play(PlayMode.StopAll);
            yield return new WaitForSeconds(targetAnimation.clip.length);
            FinishPrevious();
        }
        
        private void FinishPrevious() {
            if (_previousCallbackWhenFinished != null) {
                _previousCallbackWhenFinished();
                _previousCallbackWhenFinished = null;
            }

            StopAllCoroutines();
        }
    }
}