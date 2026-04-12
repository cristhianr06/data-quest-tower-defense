using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace FXEngine.SoundFX
{
    /// <summary>
    /// Manages a pool of <a href="https://docs.unity3d.com/ScriptReference/AudioSource.html">AudioSource</a> used by <see cref="SoundFX"/>.
    /// </summary>
    [ExecuteAlways]
    [AddComponentMenu("SoundFX/Sound FX Manager")]
    [EmbeddedIcon(Icons.SoundFXIconGUID)]
    public class SoundFXManager : MonoBehaviour
    {
        public enum CapacityType
        {
            /// <summary>
            /// The pool can grow as needed.
            /// </summary>
            Flexible,

            // Recycle, // This scenario is already well handled by the built-in AudioSource priority system

            /// <summary>
            /// The total amount of active AudioSource instances created by this pool cannot exceed <see cref="SoundFXManager.Capacity"/>.
            /// No new sounds are played while the capacity is exceeded.
            /// </summary>
            Strict,
        }

        [SerializeField]
        private int _preLoadCount = 50;

        /// <summary>
        /// The amount of instances to pre-load on Awake.
        /// </summary>
        public int PreLoadCount
        {
            get => _preLoadCount;
            set => _preLoadCount = Mathf.Max(value, 0);
        }

        [SerializeField]
        private CapacityType _capacityMode = CapacityType.Flexible;

        /// <summary>
        /// The type of capacity of the pool.
        /// </summary>
        public CapacityType CapacityMode
        {
            get => _capacityMode;
            set => _capacityMode = value;
        }

        [SerializeField]
        private int _capacity = 200;

        /// <summary>
        /// The maximum count of active AudioSources to use when <see cref="CapacityMode"/> is <see cref="CapacityType">CapacityType.Strict</see>.
        /// </summary>
        public int Capacity
        {
            get => _capacity;
            set => _capacity = Mathf.Max(value, 0);
        }

#pragma warning disable CS0414
        [HideInInspector]
        [SerializeField]
        private int _version = 0;
#pragma warning restore CS0414

        private static SoundFXManager _instance;

        /// <summary>
        /// Returns the currently loaded instance of the manager.
        /// </summary>
        /// <remarks>Returns null if no instance exists.</remarks>
        public static SoundFXManager Instance => _instance;

        /// <summary>
        /// Returns the currently loaded instance of the manager or throws an exception if no instance exists.
        /// </summary>
        /// <exception cref="InvalidOperationException">No instance exists.</exception>
        public static SoundFXManager InstanceChecked
        {
            get
            {
                if (_instance == null)
                {
                    throw new InvalidOperationException($"No instance of {nameof(SoundFXManager)} was found in the scene.");
                }

                return _instance;
            }
        }

        /// <summary>
        /// The total number of active and inactive AudioSources that were created by the pool.
        /// </summary>
        public int CountAll => _pool?.CountAll ?? 0;

        /// <summary>
        /// The total number of AudioSources removed from the pool.
        /// </summary>
        public int CountActive => _pool?.CountActive ?? 0;

        /// <summary>
        /// The total number of AudioSources contained in the pool.
        /// </summary>
        public int CountInactive => _pool?.CountInactive ?? 0;

        private ObjectPool<AudioSource> _pool;
        private int _poolInstanceCounter;

        internal static event Action<SoundFXManager> OnPoolUpdated;

        private static readonly List<AudioSource> _temp = new List<AudioSource>(100);

        protected virtual void OnValidate()
        {
            _preLoadCount = Mathf.Max(_preLoadCount, 0);
            _capacity = Mathf.Max(_capacity, 0);
        }

        private void Initialize()
        {
            _instance = this;

            _pool = new ObjectPool<AudioSource>(CreatePoolEntry, OnGetPoolEntry, OnReleasePoolEntry, OnDestroyPoolEntry);
            _poolInstanceCounter = 0;

            if (Application.isPlaying)
            {
                _temp.Clear();
                for (var i = 0; i < _preLoadCount; i++)
                {
                    var audioSource = _pool.Get();
                    _temp.Add(audioSource);
                }
                foreach (var audioSource in _temp)
                {
                    _pool.Release(audioSource);
                }
                _temp.Clear();
            }
        }

        protected virtual void Awake()
        {
            Initialize();
        }

        protected virtual void OnEnable()
        {
            // Handles domain reload
            if (Application.isPlaying == false)
            {
                Initialize();
            }
        }

        protected virtual void OnDisable()
        {
            // Handles domain reload
            if (Application.isPlaying == false)
            {
                _pool.Clear();
            }
        }

        /// <summary>
        /// Get an AudioSource from the pool.
        /// </summary>
        /// <returns>True if an instance could be found. (This is influenced by <see cref="CapacityMode"/>).</returns>
        public bool TryGet(out AudioSource audioSource)
        {
            if (_capacityMode == CapacityType.Strict)
            {
                if (_pool.CountActive >= _capacity)
                {
                    Debug.LogWarning($"Maximum {nameof(SoundFXManager)} capacity reached ({_pool.CountActive}) and {nameof(CapacityMode)} is {nameof(CapacityType.Strict)}. Skipping sound playback.", this);
                    audioSource = default;
                    return false;
                }
            }

            audioSource = _pool.Get();
            return true;
        }

        /// <summary>
        /// Returns the AudioSource back to the pool.
        /// </summary>
        /// <param name="audioSource">The instance to return.</param>
        public void Release(AudioSource audioSource)
        {
            _pool.Release(audioSource);
        }

        /// <summary>
        /// Removes all AudioSources from the pool and destroys them.
        /// </summary>
        public void Clear()
        {
            _pool.Clear();
        }

        /// <summary>
        /// Method called to create a new GameObject holding an AudioSource for the pool.
        /// Override this to modify the instance.
        /// </summary>
        protected virtual AudioSource CreatePoolEntry()
        {
            var go = new GameObject
            {
                name = $"Pooled SoundFX {_poolInstanceCounter++}",
                transform =
                {
                    parent = transform
                }
            };
            go.SetActive(false);

            if (Application.isPlaying == false)
            {
                go.hideFlags = HideFlags.DontSave;
            }

            var audioSource = go.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;

            var soundFXLifetime = go.AddComponent<SoundFXLifetime>();
            soundFXLifetime.Manager = this;

            go.AddComponent<SoundFXSpatializer>();

            return audioSource;
        }

        /// <summary>
        /// Method called when the instance is taken from the pool.
        /// Override this to extend the logic of setting up and activating the instance.
        /// </summary>
        protected virtual void OnGetPoolEntry(AudioSource audioSource)
        {
            audioSource.gameObject.SetActive(true);

            var soundFXLifetime = audioSource.GetComponent<SoundFXLifetime>();
            soundFXLifetime.Manager = this;

            NotifyPoolChanged();
        }

        /// <summary>
        /// Method called when the instance is returned to the pool.
        /// Override this to extend the logic of cleaning up and deactivating the instance.
        /// </summary>
        protected virtual void OnReleasePoolEntry(AudioSource audioSource)
        {
            audioSource.Stop();
            audioSource.gameObject.SetActive(false);

            // Clear state that can interfere with the next session

            audioSource.playOnAwake           = false;
            audioSource.loop                  = false;
            audioSource.mute                  = false;

            var spatializer = audioSource.GetComponent<SoundFXSpatializer>();
            spatializer.SetFollowTarget(null);

            NotifyPoolChanged();
        }

        /// <summary>
        /// Method called when the pool is cleared.
        /// </summary>
        protected virtual void OnDestroyPoolEntry(AudioSource audioSource)
        {
            if (audioSource != null)
            {
                if (Application.isPlaying)
                {
                    Destroy(audioSource.gameObject);
                }
                else
                {
                    DestroyImmediate(audioSource.gameObject);
                }
            }
        }

        protected void NotifyPoolChanged()
        {
            OnPoolUpdated?.Invoke(this);
        }
    }
}
