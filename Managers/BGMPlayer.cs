using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 김도명_TEXTRPG
{
    public class BGMPlayer
    {
        private static BGMPlayer instance;

        private WaveOutEvent waveOutEvent;
        private AudioFileReader audioFileReader;
        public string music = "";
        private string filePath;
        private BGMPlayer() { }

        public static BGMPlayer Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new BGMPlayer();
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
            if (Path.Combine(GetProjectDirectory(), "Sounds", music) == filePath) return;

            filePath = Path.Combine(GetProjectDirectory(), "Sounds", music);
            Stop(); // 이미 플레이 중인 경우 중지

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
