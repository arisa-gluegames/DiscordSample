using System;
using System.Collections;
using UnityEngine;

namespace GlueGames.Utilities
{
    public class Timer
    {
        public TimeSpan Duration => _duration;

        private readonly TimeSpan _duration;
        private readonly Action _callback;
        private readonly bool _isCountdown;
        private bool _isRunning;
        private TimeSpan _elapsedTime;

        public event Action<int> PerSecondEvent;
        public event Action<float, float> PerTickEvent;

        public string GetFormattedTime(string format = null)
        {
            if (string.IsNullOrEmpty(format))
            {
                return GetDefaultStringFormat();
            }
            else
            {
                return _elapsedTime.ToString(format);
            }
        }

        public string GetFormattedSeconds(string format = null)
        {
            return _elapsedTime.TotalSeconds.ToString(format);
        }

        public Timer(TimeSpan timeSpan, System.Action callback, bool countDown = false)
        {
            _duration = timeSpan;
            _callback = callback;
            _isRunning = false;
            _isCountdown = countDown;
            _elapsedTime = _isCountdown ? _duration : TimeSpan.Zero;
        }

        public Timer(int hours, int minutes, int seconds, Action callback, bool countDown = false)
        {
            _duration = new TimeSpan(hours, minutes, seconds);
            _callback = callback;
            _isRunning = false;
            _isCountdown = countDown;
            _elapsedTime = _isCountdown ? _duration : TimeSpan.Zero;
        }

        public void Start()
        {
            if (_isRunning)
            {
                return;
            }
            _isRunning = true;
            CoroutineHandler.StartStaticCoroutine(UpdateTimer());
        }

        public void Stop()
        {
            if (_isRunning)
            {
                _isRunning = false;
                if (CoroutineHandler.Instance)
                {
                    CoroutineHandler.StopStaticCoroutine(UpdateTimer());
                }
            }
        }

        public void Reset()
        {
            _elapsedTime = _isCountdown ? _duration : TimeSpan.Zero;
        }

        private IEnumerator UpdateTimer()
        {
            int currentSecond = 0;
            int lastSecond = 0;
            float totalTime = (float)_duration.TotalSeconds;

            while (_isRunning)
            {
                if (_isCountdown)
                {
                    _elapsedTime -= TimeSpan.FromSeconds(Time.deltaTime);
                    if (_elapsedTime.TotalSeconds <= 0f)
                    {
                        _callback?.Invoke();
                        Stop();
                    }
                }
                else
                {
                    _elapsedTime += TimeSpan.FromSeconds(Time.deltaTime);
                    if (_elapsedTime >= _duration)
                    {
                        _callback?.Invoke();
                        Stop();
                    }
                }
                PerTickEvent?.Invoke((float)_elapsedTime.TotalSeconds, totalTime);
                currentSecond = (int)_elapsedTime.TotalSeconds;
                // Check if a new second has started
                if (currentSecond != lastSecond)
                {
                    // Update the last second
                    lastSecond = currentSecond;

                    // Invoke the event every second and pass the total seconds elapsed
                    PerSecondEvent?.Invoke(currentSecond);
                }
                yield return null;
            }
        }

        private string GetDefaultStringFormat()
        {
            if (_elapsedTime.Hours >= 1)
            {
                return _elapsedTime.ToString(Commons.HourMinuteFormat);
            }
            else if (_elapsedTime.Minutes >= 1)
            {
                return _elapsedTime.ToString(Commons.MinuteSecondsFormat);
            }
            else
            {
                return _elapsedTime.ToString(Commons.SecondsFormat);
            }
        }
    }

}
