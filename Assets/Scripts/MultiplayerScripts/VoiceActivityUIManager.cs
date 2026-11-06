using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Voice.PUN;

public class VoiceActivityUIManager : MonoBehaviour
{
    public GameObject speakerRowPrefab;
    public Transform panelParent;

    private Dictionary<int, GameObject> activeRows = new Dictionary<int, GameObject>();

    void Update()
    {
        foreach (PhotonVoiceView voiceView in FindObjectsOfType<PhotonVoiceView>())
        {
            PhotonView view = voiceView.GetComponent<PhotonView>();
            if (view == null || view.IsMine)
                continue;

            int actorNumber = view.OwnerActorNr;
            bool isSpeaking = voiceView.IsSpeaking;

            if (isSpeaking && !activeRows.ContainsKey(actorNumber))
            {
                GameObject row = Instantiate(speakerRowPrefab, panelParent);
                row.GetComponentInChildren<TMP_Text>().text = view.Owner.NickName;
                row.SetActive(true);
                activeRows[actorNumber] = row;
            }
            else if (!isSpeaking && activeRows.ContainsKey(actorNumber))
            {
                Destroy(activeRows[actorNumber]);
                activeRows.Remove(actorNumber);
            }
        }
    }
}
