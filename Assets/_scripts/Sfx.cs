using UnityEngine;

// Central one-shot player. No scene setup: creates its own AudioSource on first use.
// Null clips are silently skipped, so every sound slot in the game is optional.
public static class Sfx
{
    private static AudioSource _source;

    public static void Play(AudioClip clip, float volume = 1f)
    {
        if (clip == null) return;

        if (_source == null)
        {
            var go = new GameObject("Sfx (auto)");
            Object.DontDestroyOnLoad(go);
            _source = go.AddComponent<AudioSource>();
        }

        _source.PlayOneShot(clip, volume);   // one-shots overlap; nothing ever cuts off
    }
}
