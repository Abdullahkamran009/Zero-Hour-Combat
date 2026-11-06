using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using System.Collections;
using UnityEngine.EventSystems;



public class ChatManager : MonoBehaviourPun
{
    public TMP_InputField chatInputField;
    public Button sendButton;
    public GameObject messagePrefab;
    public Transform messageContent;
    public ScrollRect scrollRect;
    public int maxMessages = 30;
    public GameObject chatUI;
    public static bool isTyping = false;

    private List<GameObject> messageList = new List<GameObject>();

    void Start()
    {
        chatUI.SetActive(false);
        sendButton.onClick.AddListener(SendChat);
        chatInputField.onSubmit.AddListener(delegate { SubmitByEnter(); });
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M) && !chatInputField.isFocused)
        {
            chatUI.SetActive(!chatUI.activeSelf);
            isTyping = chatUI.activeSelf;

            if (isTyping)
            {
                // Unlock and show cursor
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;

                EventSystem.current.SetSelectedGameObject(chatInputField.gameObject);
                chatInputField.OnPointerClick(new PointerEventData(EventSystem.current));
            }
            else
            {
                // Lock and hide cursor
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;

                EventSystem.current.SetSelectedGameObject(null);
            }
        }
    }




    public void SendChat()
    {
        Debug.Log("SendChat() called");

        if (!string.IsNullOrWhiteSpace(chatInputField.text))
        {
            photonView.RPC("ReceiveMessage", RpcTarget.All, PhotonNetwork.NickName, chatInputField.text);
            chatInputField.text = "";
        }
    }

    [PunRPC]
    void ReceiveMessage(string sender, string message, PhotonMessageInfo info)
    {
        Debug.Log($"[RECEIVED] {sender}: {message}");

        // Auto-show chat UI for everyone except the sender
        if (!chatUI.activeSelf && !info.Sender.IsLocal)
        {
            chatUI.SetActive(true);
            StartCoroutine(HideChatUIAfterDelay());
        }

        GameObject newMessage = Instantiate(messagePrefab, messageContent);
        newMessage.GetComponent<TMP_Text>().text = $"<b>{sender}:</b> {message}";
        messageList.Add(newMessage);

        if (messageList.Count > maxMessages)
        {
            Destroy(messageList[0]);
            messageList.RemoveAt(0);
        }

        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0f;
    }

    public void SubmitByEnter()
    {
        SendChat();

        isTyping = false;
        chatUI.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        EventSystem.current.SetSelectedGameObject(null);
    }

    IEnumerator HideChatUIAfterDelay()
    {
        yield return new WaitForSeconds(4f); // Show chat briefly
        if (!isTyping)
        {
            chatUI.SetActive(false);
        }
    }

}
