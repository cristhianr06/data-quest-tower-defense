using System;
using UnityEngine;

namespace FXEngine.SoundFX
{
    [ExecuteAlways]
    [AddComponentMenu("")]
    [RequireComponent(typeof(AudioSource))]
    class SoundFXLifetime : MonoBehaviour, IEditorPreviewComponent
    {
        [NonSerialized]
        public SoundFXManager Manager;

        [NonSerialized]
        public SoundFX Owner;

        [NonSerialized]
        public SoundFXInstance Instance;

        [NonSerialized]
        public int PlayCount;

        private AudioSource _audioSource;
        private int _frameCounter;

        private const int NumGraceFrames = 10;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        private void OnEnable()
        {
            EditorPreview.OnEnable(this);
        }

        private void OnDisable()
        {
            EditorPreview.OnDisable(this);
        }

        private void Update()
        {
            if (Application.isPlaying == false)
            {
                return;
            }

            DoUpdate(Time.deltaTime);
        }

        void IEditorPreviewComponent.EditorUpdate(float deltaTime)
        {
            DoUpdate(deltaTime);
        }

        private void DoUpdate(float deltaTime)
        {
            Instance?.Update(deltaTime);

            if (_audioSource.isPlaying == false)
            {
                _frameCounter++;

                if (_frameCounter >= NumGraceFrames)
                {
                    if (Owner != null)
                    {
                        Owner.Release(Instance);
                        Owner = null;
                        Instance = null;
                    }

                    if (Manager != null)
                    {
                        Manager.Release(_audioSource);
                        Manager = null;
                    }
                }
            }
        }

        public void Play(SoundFX soundFX, SoundFXInstance instance)
        {
            Owner = soundFX;
            Instance = instance;

            PlayCount++;
            _frameCounter = 0;

            instance.Lifetime = this;
        }
    }
}
