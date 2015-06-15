using UnityEngine;
using System.Collections;

public class JoinBtn : MonoBehaviour
{

    public UIInput usernameInput;

    // Use this for initialization 
    public void OnSubmit()
    {
        StartCoroutine(NetworkManager.Instance.Login(NGUIText.StripSymbols(usernameInput.value)));
        usernameInput.value = "";

        /*
         *  if (textList != null)
         {
             // It's a good idea to strip out all symbols as we don't want user input to alter colors, add new lines, etc
             string text = NGUIText.StripSymbols(mInput.value);

             if (!string.IsNullOrEmpty(text))
             {
                 textList.Add(text);
                 mInput.value = "";
                 mInput.isSelected = false;
             }
         }
         */
    }

    public void OnAutoChat()
    {
        NetworkManager.Instance.isAutoChat = true;
        OnSubmit();
    }
}
