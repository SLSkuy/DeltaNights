using System;
using UIFramework.UIAnimation;
using UnityEngine;

namespace UIFramework
{
    /// <summary>
    /// 渐入动画
    /// </summary>
    public class FadeAni : AnimComponent
    {
        [SerializeField] private float fadeDuration = 0.5f;
        [SerializeField] private bool fadeOut = false;

        private CanvasGroup _canvasGroup;
        private float _timer;
        private Action _currentAction;
        private Transform _currentTarget;

        private float _startValue;
        private float _endValue;

        private bool _shouldAnimate;

        public override void Animate(Transform target, Action callWhenFinished)
        {
            if (_currentAction != null)
            {
                _canvasGroup.alpha = _endValue;
                _currentAction();
            }

            _canvasGroup = target.GetComponent<CanvasGroup>();
            if (_canvasGroup == null)
            {
                _canvasGroup = target.gameObject.AddComponent<CanvasGroup>();
            }

            if (fadeOut)
            {
                _startValue = 1f;
                _endValue = 0f;
            }
            else
            {
                _startValue = 0f;
                _endValue = 1f;
            }

            _currentAction = callWhenFinished;
            _timer = fadeDuration;

            _canvasGroup.alpha = _startValue;
            _shouldAnimate = true;
        }

        private void Update()
        {
            if (!_shouldAnimate) return;

            if (_timer > 0f)
            {
                _timer -= Time.deltaTime;
                _canvasGroup.alpha = Mathf.Lerp(_startValue, _endValue, 1 - (_timer / fadeDuration));
            }
            else
            {
                _canvasGroup.alpha = _endValue;

                if (_currentAction != null)
                {
                    _currentAction();
                }

                _currentAction = null;
                _shouldAnimate = false;
            }
        }
    }
}