using DG.Tweening.Plugins.Options;
using DG.Tweening.Core;
using VibePack.Utility;
using DG.Tweening;
using UnityEngine;
using System;

namespace VibePack.UI
{
    [Serializable]
    public class TransitionSettings
    {
        public bool alpha;
        public Optional<Vector3> direction;
        public Optional<Vector3> scale;
        public float duration = 1;
    }

    public class TransitionDirector
    {
        readonly Sequence sequence;
        readonly bool opening;

        public TransitionDirector(bool opening)
        {
            this.opening = opening;
            sequence = DOTween.Sequence();
        }

        public TransitionDirector(bool opening, TransitionSettings settings, SpriteRenderer spriteRenderer, Transform transform)
        {
            this.opening = opening;
            sequence = DOTween.Sequence();

            bool isFading = settings.alpha;
            bool isMoving = settings.direction;
            bool isScaling = settings.scale;

            if (!isFading && !isMoving && !isScaling)
                return;

            float duration = settings.duration;

            if (isFading)
                Fade(spriteRenderer, opening ? 1 : 0, duration);
            else
            {
                if (opening)
                {
                    Color col = spriteRenderer.color;
                    col.a = 1;
                    spriteRenderer.color = col;
                }
                else
                    OnComplete(() =>
                    {
                        Color col = spriteRenderer.color;
                        col.a = 0;
                        spriteRenderer.color = col;
                    });
            }

            if (isMoving)
                Move(transform, settings.direction, duration);

            if (isScaling)
            {
                transform.localScale = Vector3.zero;
                Scale(transform, settings.scale, duration);
            }
        }

        public TransitionDirector(bool opening, TransitionSettings settings, CanvasGroup canvasGroup, Transform transform)
        {
            this.opening = opening;
            sequence = DOTween.Sequence();

            bool isFading = settings.alpha;
            bool isMoving = settings.direction;
            bool isScaling = settings.scale;

            if (!isFading && !isMoving && !isScaling)
                return;

            float duration = settings.duration;

            if (isFading)
                Fade(canvasGroup, opening ? 1: 0, duration);
            else
            {
                if (opening)
                    canvasGroup.alpha = 1;
                else
                    OnComplete(() => canvasGroup.alpha = 0);
            }

            if (isMoving)
                Move(transform, settings.direction, duration);

            if (isScaling)
            {
                transform.localScale = Vector3.zero;
                Scale(transform, settings.scale, duration);
            }
        }

        private void Fade(CanvasGroup canvasGroup, float targetAlpha, float duration) => sequence.Join(canvasGroup.DOFade(targetAlpha, duration));

        private void Fade(SpriteRenderer spriteRenderer, float targetAlpha, float duration) => sequence.Join(spriteRenderer.DOFade(targetAlpha, duration));

        private void Move(Transform transform, Vector3 direction, float duration)
        {
            Vector3 originalPosition = transform.localPosition;
            TweenerCore<Vector3, Vector3, VectorOptions> moveTween = transform.DOLocalMove(transform.localPosition + direction, duration);

            if (opening)
                moveTween.From();

            sequence.Join(moveTween);
            OnComplete(() => transform.localPosition = originalPosition);
        }

        private void Scale(Transform transform, Vector3 scale, float duration)
        {
            TweenerCore<Vector3, Vector3, VectorOptions> scaleTween = transform.DOScale(scale, duration);

            if (!opening)
                scaleTween.From();

            sequence.Join(scaleTween);
        }

        public void OnComplete(TweenCallback callback) => sequence.onComplete += callback;

        public YieldInstruction Build() => sequence.WaitForCompletion();
    }
}
