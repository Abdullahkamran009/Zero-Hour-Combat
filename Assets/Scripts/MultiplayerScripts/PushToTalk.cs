using UnityEngine;
using Photon.Voice.Unity;

public class PushToTalk : MonoBehaviour
{
    public Recorder recorder;
    public KeyCode pushToTalkKey = KeyCode.V;

    void Update()
    {
        if (recorder == null)
            return;

        // Transmit only when key is held
        recorder.TransmitEnabled = Input.GetKey(pushToTalkKey);
    }
}
