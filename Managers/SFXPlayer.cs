using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 김도명_TEXTRPG
{
    public class SFXPlayer
    {
        private static SFXPlayer instance;

        private WaveOutEvent waveOutEvent;
        private AudioFileReader audioFileReader;
        public string music = "";

        private SFXPlayer() { }

        public static SFXPlayer Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SFXPlayer();
                }
                return instance;
            }
        }
        public bool IsPlaying
        {
            get { return waveOutEvent != null && waveOutEvent.PlaybackState == PlaybackState.Playing; }
        }

        public async Task PlayAsync(float volume = 1.0f)
        {
            Stop(); // 이미 플레이 중인 경우 중지

            string filePath = Path.Combine(GetProjectDirectory(), "Sounds", music);
            audioFileReader = new AudioFileReader(filePath);
            waveOutEvent = new WaveOutEvent();
            waveOutEvent.Init(audioFileReader);
            // PlaybackStopped 이벤트를 활용하여 음악이 끝났는지 확인
            var playbackStoppedTaskCompletionSource = new TaskCompletionSource<bool>();
            waveOutEvent.PlaybackStopped += (sender, args) =>
            {
                playbackStoppedTaskCompletionSource.TrySetResult(true);
            };
            waveOutEvent.Volume = volume; // 볼륨 조절
            waveOutEvent.Play();

            // 비동기로 음악이 끝날 때까지 기다림
            await playbackStoppedTaskCompletionSource.Task;
        }

        public void Stop()
        {
            if (waveOutEvent != null)
            {
                waveOutEvent.Stop();
                waveOutEvent.Dispose();
                waveOutEvent = null;
            }

            if (audioFileReader != null)
            {
                audioFileReader.Dispose();
                audioFileReader = null;
            }
        }

        private string GetProjectDirectory()
        {
            return Directory.GetParent(Environment.CurrentDirectory).Parent.FullName;
        }
    }
}
