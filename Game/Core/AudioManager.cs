using Godot;

namespace Game.Core;

public partial class AudioManager : Node
{
    private AudioStreamPlayer _attackSound;
    private AudioStreamPlayer _magicSound;
    private AudioStreamPlayer _lootSound;
    
    // Very simple synth audio generation since we don't have .wav/.ogg assets
    public override void _Ready()
    {
        _attackSound = new AudioStreamPlayer { Stream = GenerateBeep(440.0f, 0.1f) };
        AddChild(_attackSound);
        
        _magicSound = new AudioStreamPlayer { Stream = GenerateBeep(660.0f, 0.2f) };
        AddChild(_magicSound);
        
        _lootSound = new AudioStreamPlayer { Stream = GenerateBeep(880.0f, 0.15f) };
        AddChild(_lootSound);
    }
    
    public void PlayAttack() => _attackSound.Play();
    public void PlayMagic() => _magicSound.Play();
    public void PlayLoot() => _lootSound.Play();

    private AudioStreamWav GenerateBeep(float frequency, float duration)
    {
        int sampleRate = 44100;
        int numSamples = (int)(sampleRate * duration);
        var data = new byte[numSamples * 2]; // 16-bit
        
        for (int i = 0; i < numSamples; i++)
        {
            float t = (float)i / sampleRate;
            // Sine wave
            float sample = Mathf.Sin(t * frequency * Mathf.Tau);
            short sample16 = (short)(sample * 32767f * 0.2f); // Volume at 20%
            
            data[i * 2] = (byte)(sample16 & 0xFF);
            data[i * 2 + 1] = (byte)((sample16 >> 8) & 0xFF);
        }
        
        return new AudioStreamWav
        {
            Data = data,
            Format = AudioStreamWav.FormatEnum.Format16Bits,
            MixRate = sampleRate,
            Stereo = false
        };
    }
}
