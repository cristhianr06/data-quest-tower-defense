using System;
using UnityEngine;

namespace FXEngine.SoundFX
{
    class SoundFXInstance
    {
        public AudioSource AudioSource;
        public SoundFXLifetime Lifetime;
        public float OriginalVolume;

        private Fader _fader = Fader.Default;
        private float _lifetime;

        public bool IsValid
        {
            get
            {
                if (Lifetime == null || AudioSource == null)
                {
                    return false;
                }

                // Check if the AudioSource is still linked to this instance
                return Lifetime.Instance == this;
            }
        }

        public void Stop()
        {
            if (IsValid)
            {
                AudioSource.Stop();
            }
        }

        public void Fade(float target, float duration, FadeDurationMode durationMode)
        {
            _fader.Fade(target, duration, durationMode);

            if (Mathf.Approximately(duration, 0f))
            {
                // Avoid latency on volume if the fade is instant
                AudioSource.volume = OriginalVolume * _fader.Current;

                if (Mathf.Approximately(_fader.Current, 0f))
                {
                    // Avoid latency on Stop() if the fade is instant
                    AudioSource.Stop();
                }
            }
        }

        public void FadeIn(float duration)
        {
            // If this is a fresh instance, then fade in starting from 0
            if (Mathf.Approximately(_lifetime, 0f))
            {
                _fader.Current = 0f;
            }

            Fade(1f, duration, FadeDurationMode.ConstantSpeed);
        }

        public void FadeOut(float duration)
        {
            Fade(0f, duration, FadeDurationMode.ConstantSpeed);
        }

        public void Update(float deltaTime)
        {
            if (IsValid == false)
            {
                return;
            }

            _lifetime += deltaTime;

            _fader.Update(deltaTime, out var shouldStopAudioSource);
            AudioSource.volume = OriginalVolume * _fader.Current;

            if (shouldStopAudioSource)
            {
                AudioSource.Stop();
            }
        }
    }

    struct Fader
    {
        public float Current
        {
            get => _current;
            set => _current = value;
        }

        private float _current;
        private float _target;
        private float _speed;

        public static readonly Fader Default = new Fader()
        {
            _current = 1f,
            _target = 1f,
            _speed = 0f,
        };

        public void Fade(float target, float duration, FadeDurationMode durationMode)
        {
            if (Mathf.Approximately(duration, 0f))
            {
                _current = target;
                _target = target;
                _speed = 0f;
            }
            else if (Mathf.Approximately(target, _current))
            {
                _current = target;
                _target = target;
                _speed = 0f;
            }
            else
            {
                _target = target;

                switch (durationMode)
                {
                    case FadeDurationMode.ConstantSpeed:
                    {
                        _speed = 1f / duration;
                        break;
                    }

                    case FadeDurationMode.ConstantTime:
                    {
                        var delta = Mathf.Abs(_target - _current);
                        _speed = delta / duration;
                        break;
                    }

                    default:
                        throw new ArgumentOutOfRangeException(nameof(durationMode), durationMode.ToString());
                }
            }
        }

        public void Update(float deltaTime, out bool shouldStopAudioSource)
        {
            shouldStopAudioSource = false;

            if (Mathf.Approximately(_current, _target))
            {
                return;
            }

            _current = Mathf.MoveTowards(_current, _target, _speed * deltaTime);
            _current = Mathf.Clamp01(_current);

            if (Mathf.Approximately(_current, 0f))
            {
                shouldStopAudioSource = true;
            }
        }
    }
}
