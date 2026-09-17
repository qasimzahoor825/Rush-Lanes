using UnityEngine;

namespace RushLanes
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        AudioSource _sfx;
        AudioSource _music;
        AudioClip _coin, _jump, _slide, _hit, _power, _click, _over, _whoosh;

        public void Init()
        {
            Instance = this;
            _sfx = gameObject.AddComponent<AudioSource>();
            _sfx.playOnAwake = false;
            _music = gameObject.AddComponent<AudioSource>();
            _music.loop = true;
            _music.playOnAwake = false;
            _music.volume = 0.18f;

            _coin = Tone(880, 0.09f, 0.35f);
            _jump = Sweep(220, 520, 0.14f, 0.28f);
            _slide = Sweep(420, 160, 0.12f, 0.22f);
            _hit = Noise(0.22f, 0.4f);
            _power = Sweep(300, 900, 0.22f, 0.3f);
            _click = Tone(640, 0.05f, 0.25f);
            _over = Sweep(360, 90, 0.45f, 0.4f);
            _whoosh = Sweep(180, 80, 0.2f, 0.2f);
            _music.clip = LoopBed();
        }

        public void PlayMusic(bool on)
        {
            if (!SaveSystem.Data.soundOn || !on)
            {
                _music.Stop();
                return;
            }

            if (!_music.isPlaying)
                _music.Play();
        }

        public void Coin() => Play(_coin, 0.55f);
        public void Jump() => Play(_jump, 0.45f);
        public void Slide() => Play(_slide, 0.4f);
        public void Hit() => Play(_hit, 0.7f);
        public void Power() => Play(_power, 0.55f);
        public void Click() => Play(_click, 0.4f);
        public void GameOver() => Play(_over, 0.7f);
        public void Whoosh() => Play(_whoosh, 0.3f);

        void Play(AudioClip clip, float volume)
        {
            if (!SaveSystem.Data.soundOn || clip == null)
                return;
            _sfx.PlayOneShot(clip, volume);
        }

        static AudioClip Tone(float freq, float duration, float volume)
        {
            int sampleRate = 44100;
            int samples = Mathf.CeilToInt(sampleRate * duration);
            var data = new float[samples];
            for (int i = 0; i < samples; i++)
            {
                float t = i / (float)sampleRate;
                float env = 1f - t / duration;
                data[i] = Mathf.Sin(2f * Mathf.PI * freq * t) * env * volume;
            }
            var clip = AudioClip.Create("tone", samples, 1, sampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }

        static AudioClip Sweep(float from, float to, float duration, float volume)
        {
            int sampleRate = 44100;
            int samples = Mathf.CeilToInt(sampleRate * duration);
            var data = new float[samples];
            for (int i = 0; i < samples; i++)
            {
                float n = i / (float)samples;
                float freq = Mathf.Lerp(from, to, n);
                float t = i / (float)sampleRate;
                float env = Mathf.Sin(n * Mathf.PI);
                data[i] = Mathf.Sin(2f * Mathf.PI * freq * t) * env * volume;
            }
            var clip = AudioClip.Create("sweep", samples, 1, sampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }

        static AudioClip Noise(float duration, float volume)
        {
            int sampleRate = 44100;
            int samples = Mathf.CeilToInt(sampleRate * duration);
            var data = new float[samples];
            for (int i = 0; i < samples; i++)
            {
                float n = i / (float)samples;
                data[i] = (Random.value * 2f - 1f) * (1f - n) * volume;
            }
            var clip = AudioClip.Create("noise", samples, 1, sampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }

        static AudioClip LoopBed()
        {
            int sampleRate = 44100;
            int samples = sampleRate * 4;
            var data = new float[samples];
            for (int i = 0; i < samples; i++)
            {
                float t = i / (float)sampleRate;
                data[i] =
                    Mathf.Sin(2f * Mathf.PI * 110f * t) * 0.12f +
                    Mathf.Sin(2f * Mathf.PI * 165f * t) * 0.07f +
                    Mathf.Sin(2f * Mathf.PI * 220f * t) * 0.04f;
            }
            var clip = AudioClip.Create("bed", samples, 1, sampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }
    }
}
