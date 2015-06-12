using UnityEngine;
using System.Collections;

public class ChatBtn : MonoBehaviour
{

    public UIInput chatInput;

    // Use this for initialization 
    public void OnSubmit()
    {
        NetworkManager.Instance.Chat(NGUIText.StripSymbols(chatInput.value));
        chatInput.value = "";
    }
}
