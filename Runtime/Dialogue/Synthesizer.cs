using System.Collections;
using UnityEngine;

public class Synthesizer : MonoBehaviour
{
    public double frequency;
    private double incement;
    private double phase;
    public double sampling_frequency;
    public float gain;

    private void OnAudioFilterRead(float[] data, int channels)
    {
        incement = frequency * 2.0f * Mathf.PI / sampling_frequency;

        for (int i = 0; i < data.Length; i+= channels)
        {
            phase += incement;
            data[i] = (float)(gain * Mathf.Sin((float)phase));

            if (channels == 2)
            {
                data[i + 1] = data[i];
            }

            if (phase > (Mathf.PI * 2))
            {
                phase -= Mathf.PI * 2;
            }
        }
    }

    public IEnumerator PlaySoundCoroutine(float time, double frequencyParameter, float volume)
    {
        frequency = frequencyParameter;
        gain = volume;
        yield return new WaitForSeconds(time);
        gain = 0;
    }
}
