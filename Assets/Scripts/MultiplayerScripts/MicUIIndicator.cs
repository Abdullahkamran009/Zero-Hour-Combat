using UnityEngine;
using UnityEngine.UI;
using Photon.Voice.Unity;

public class MicUIIndicator : MonoBehaviour
{
    public Recorder recorder;
    public Image micIcon;           // The UI icon to show/hide

    void Start()
    {
        if (recorder == null)
        {
            
            recorder = GetComponentInParent<Recorder>();
        }
    }

    void Update()
    {
        if (recorder != null && micIcon != null)
        {
            micIcon.enabled = recorder.IsCurrentlyTransmitting;
        }
    }
}
