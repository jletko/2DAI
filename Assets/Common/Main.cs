using System;
using System.Collections.Generic;
using System.Linq;

using Unity.MLAgents;

using UnityEditor;

using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Common
{
    public class Main : MonoBehaviour
    {
        private const float MaxTimeScale = 20;

        [FormerlySerializedAs("_mainCanvas")][SerializeField] private Canvas mainCanvas;
        [FormerlySerializedAs("_isMenuEnabledAfterStart")][SerializeField] private bool isMenuEnabledAfterStart = false;
        [FormerlySerializedAs("_timeScaleTextBox")][SerializeField] private Text timeScaleTextBox;
        [FormerlySerializedAs("_stepTextBox")][SerializeField] private Text stepTextBox;
        [FormerlySerializedAs("_timeTextBox")][SerializeField] private Text timeTextBox;
        [FormerlySerializedAs("_timeScaleSlider")][SerializeField] private Slider timeScaleSlider;
        [FormerlySerializedAs("_restartButton")][SerializeField] private Button restartButton;
        [FormerlySerializedAs("_trainingTimeScale")][SerializeField][Range(0, MaxTimeScale)] private float trainingTimeScale = 10;
        [FormerlySerializedAs("_gravity")][SerializeField] private Vector2 gravity = new Vector2(0, 0);
        [FormerlySerializedAs("_fixedTimestamp")][SerializeField] private float fixedTimestamp = 0.02f;

        private List<RefereeBase> _allReferees;
        private bool _isOneClick;
        private float _timerForDoubleClick;
        private int _fixedUpdatesCount;

        protected virtual void Start()
        {
            Physics2D.gravity = gravity;
            Time.fixedDeltaTime = fixedTimestamp;
            _allReferees = FindObjectsByType<RefereeBase>(FindObjectsSortMode.None).ToList();
            restartButton.gameObject.SetActive(_allReferees.Any());
            timeScaleSlider.value = Mathf.Log(1 + (IsCommunicatorOn ? trainingTimeScale : 1)) / Mathf.Log(MaxTimeScale + 1);
            _fixedUpdatesCount = 0;
#if UNITY_EDITOR
            EditorApplication.playModeStateChanged += ModeChanged;
#endif
            OnTimeScaleChanged();
            mainCanvas.enabled = isMenuEnabledAfterStart;
        }

        private void FixedUpdate()
        {
            _fixedUpdatesCount++;
            timeTextBox.text = $"Time: {SecondsToTime(Time.fixedTime)}";
            stepTextBox.text = $"Time step: {_fixedUpdatesCount}";
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(2))
            {
                mainCanvas.enabled = !mainCanvas.enabled;
            }

            if (IsCommunicatorOn)
            {
                Camera.main.orthographicSize -= 2 * Input.mouseScrollDelta.y;
            }
        }

        public void OnRestart()
        {
            _allReferees.ForEach(o => o.Restart());
        }

        public void OnQuit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
        }

        public void OnTimeScaleChanged()
        {
            float logValue = Mathf.Exp(Mathf.Log(MaxTimeScale + 1) * timeScaleSlider.value) - 1;
            Time.timeScale = logValue;
            timeScaleTextBox.text = $"Time scale: {logValue:F2}";
        }

        private bool IsCommunicatorOn => Academy.Instance.IsCommunicatorOn;

#if UNITY_EDITOR
        private void ModeChanged(PlayModeStateChange stateChange)
        {
            if (!EditorApplication.isPlayingOrWillChangePlaymode &&
                EditorApplication.isPlaying)
            {
                Debug.Log($"Exit fixed time: {SecondsToTime(Time.fixedTime + Time.fixedDeltaTime)}");
                Debug.Log($"Exit fixed delta time: {Time.fixedDeltaTime}");
                Debug.Log($"Exit fixed updates count: {_fixedUpdatesCount}");
                Debug.Log($"Exit step count: {Academy.Instance.StepCount}");
                Debug.Log($"Exit episode count: {Academy.Instance.EpisodeCount}");
            }
        }
#endif

        private string SecondsToTime(float seconds)
        {
            TimeSpan time = TimeSpan.FromSeconds(seconds);
            return time.ToString(@"hh\:mm\:ss");
        }
    }
}
