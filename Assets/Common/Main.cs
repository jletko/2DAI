﻿using MLAgents;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Common
{
    public class Main : MonoBehaviour
    {
        private const float MaxTimeScale = 20;

        [SerializeField] private Canvas _mainCanvas;
        [SerializeField] private Text _timeScaleTextBox;
        [SerializeField] private Text _stepTextBox;
        [SerializeField] private Text _timeTextBox;
        [SerializeField] private Slider _timeScaleSlider;
        [SerializeField] private Button _restartButton;
        [SerializeField] [Range(0, MaxTimeScale)] private float _trainingTimeScale = MaxTimeScale;
        [SerializeField] private Vector2 _gravity = new Vector2(0, 0);
        [SerializeField] private float _fixedTimestamp = 0.02f;

        private List<Referee> _allReferees;
        private bool _isOneClick;
        private float _timerForDoubleClick;
        private int _fixedUpdatesCount;

        protected virtual void Start()
        {
            Physics2D.gravity = _gravity;
            Time.fixedDeltaTime = _fixedTimestamp;
            _allReferees = FindObjectsOfType<Referee>().ToList();
            _restartButton.gameObject.SetActive(_allReferees.Any());
            _timeScaleSlider.value = Mathf.Log(1 + (IsCommunicatorOn ? _trainingTimeScale : 1)) / Mathf.Log(MaxTimeScale + 1);
            _fixedUpdatesCount = 0;
#if UNITY_EDITOR
            EditorApplication.playModeStateChanged += ModeChanged;
#endif
            OnTimeScaleChanged();
        }

        private void FixedUpdate()
        {
            _fixedUpdatesCount++;
            _timeTextBox.text = $"Time: {SecondsToTime(Time.fixedTime)}";
            _stepTextBox.text = $"Time step: {_fixedUpdatesCount}";
        }

        private void Update()
        {
            CheckDoubleClick();

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
            Application.Quit();
        }

        public void OnTimeScaleChanged()
        {
            float logValue = Mathf.Exp(Mathf.Log(MaxTimeScale + 1) * _timeScaleSlider.value) - 1;
            Time.timeScale = logValue;
            _timeScaleTextBox.text = $"Time scale: {logValue:F2}";
        }

        private bool IsCommunicatorOn => Academy.Instance.IsCommunicatorOn;

        private void CheckDoubleClick()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (!_isOneClick) // first click no previous clicks
                {
                    _isOneClick = true;

                    // save the current time
                    _timerForDoubleClick = Time.time;

                    // do one click things
                }
                else
                {
                    _isOneClick = false; // found a double click, now reset

                    // do double click things
                    OnDoubleClick();
                }
            }
            if (_isOneClick)
            {
                // if the time now is delay seconds more than when the first click started. 
                if ((Time.time - _timerForDoubleClick) > 0.75f * Time.timeScale)
                {
                    //basically if thats true its been too long and we want to reset so the next click is simply a single click and not a double click.
                    _isOneClick = false;
                }
            }
        }

        private void OnDoubleClick()
        {
            _mainCanvas.enabled = !_mainCanvas.enabled;
        }
#if UNITY_EDITOR
        private void ModeChanged(PlayModeStateChange stateChange)
        {
            if (!EditorApplication.isPlayingOrWillChangePlaymode &&
                EditorApplication.isPlaying)
            {
                Debug.Log($"Exit fixed time: {SecondsToTime(Time.fixedTime + Time.fixedDeltaTime)}");
                Debug.Log($"Exit fixed delta time: {Time.fixedDeltaTime}");
                Debug.Log($"Exit fixed updates count: {_fixedUpdatesCount}");
                Debug.Log($"Exit step count: {Academy.Instance.GetStepCount()}");
                Debug.Log($"Exit episode count: {Academy.Instance.GetEpisodeCount()}");
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