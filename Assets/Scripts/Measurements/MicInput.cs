using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MicInput : MonoBehaviour
{
    public string[] Devices;
    public int DeviceArrayId = 0;
    public int SampleRate = 48000;

    // Start is called before the first frame update
    void Start()
    {
        if(Microphone.devices.Length <= 0)
        {
            Debug.LogWarning("Microphone not connected!");
        }
        else
        {
            Devices = Microphone.devices;
        }
    }

    private void Update()
    {

        //if (Microphone.devices.Length <= 0)
        //{
        //}
        //else
        //{
        //    Devices = Microphone.devices;
        //}
    }

    public void StartListening()
    {
        if(Microphone.devices.Length > 0)
        {
            var playback = GetComponent<AudioSource>();
            var microphone = Microphone.devices[DeviceArrayId];
            playback.clip = Microphone.Start(microphone, true, 10, SampleRate);
            playback.loop = true;
            while (!(Microphone.GetPosition(Microphone.devices[DeviceArrayId]) > 7168)) { };
            playback.Play();
            Debug.Log($"Listening started to {Microphone.devices[DeviceArrayId]}");
        }
    }

    public void StopListening()
    {
        var playback = GetComponent<AudioSource>();
        if(playback.isPlaying )
        {
            playback.Stop();
            Debug.Log("Listening stopped");
        }
    }
}
