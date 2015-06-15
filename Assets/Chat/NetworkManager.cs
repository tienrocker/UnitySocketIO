using UnityEngine;
using System.Collections;
using SocketIO;
using System.Collections.Generic;

public class NetworkManager : MonoBehaviour
{

    #region create instance
    public static NetworkManager Instance;

    private void Awake()
    {
        Instance = this;
    }
    #endregion

    // Use this for initialization
    public SocketIOComponent socket;
    public bool isLogingIn = false;
    public GameObject LoginPanel;
    public GameObject ChatPanel;
    public UITextList ChatContent;

    public bool isAutoChat = false;
    public string UserName;

    public List<string> DictionaryQuote = new List<string>();

    public void Start()
    {
        DictionaryQuote.Add("All great movements are popular movements. They are the volcanic eruptions of human passions and emotions, stirred into activity by the ruthless Goddess of Distress or by the torch of the spoken word cast into the midst of the people.");
        DictionaryQuote.Add("Great liars are also great magicians.");
        DictionaryQuote.Add("Hate is more lasting than dislike.");
        DictionaryQuote.Add("Humanitarianism is the expression of stupidity and cowardice.");
        DictionaryQuote.Add("It is not truth that matters, but victory.");
        DictionaryQuote.Add("Make the lie big, make it simple, keep saying it, and eventually they will believe it.");
        DictionaryQuote.Add("I use emotion for the many and reserve reason for the few.");
        DictionaryQuote.Add("Those who want to live, let them fight, and those who do not want to fight in this world of eternal struggle do not deserve to live.");
        DictionaryQuote.Add("The victor will never be asked if he told the truth.");
        DictionaryQuote.Add("The art of leadership... consists in consolidating the attention of the people against a single adversary and taking care that nothing will split up that attention.");
        DictionaryQuote.Add("It is always more difficult to fight against faith than against knowledge.");

        LoginPanel.SetActive(true);
        ChatPanel.SetActive(false);
    }

    public IEnumerator Login(string username)
    {
        if (isLogingIn) yield return null;
        isLogingIn = true;
        socket.autoConnect = false;

        if (!socket.IsConnected) socket.Connect();
        while (!socket.IsConnected) yield return new WaitForSeconds(0.05f);

        socket.On("open", (SocketIOEvent e) => { Debug.Log("[OnOpen] received: " + e.name + " " + e.data); });
        socket.On("error", (SocketIOEvent e) => { Debug.Log("[OnError] Error received: " + e.name + " " + e.data); });
        socket.On("close", (SocketIOEvent e) => { Debug.Log("[OnClose]: " + e.name + " " + e.data); });

        yield return new WaitForSeconds(0.05f);

        JSONObject user = new JSONObject();
        if (isAutoChat) username = GenerateName(Random.Range(4, 7));

        user.AddField("username", username);
        socket.Emit("login", user, OnLogin);
        UserName = username;

        if (isAutoChat)
            StartCoroutine("AutoChat");
    }

    public void Chat(string text)
    {
        JSONObject content = new JSONObject();
        content.AddField("text", text);
        socket.Emit("chat", content, OnChat);
    }

    public IEnumerator AutoChat()
    {
        Chat(DictionaryQuote[Random.Range(0, DictionaryQuote.Count)]);
        yield return new WaitForSeconds(0.5f);
        StartCoroutine("AutoChat");
    }

    void OnLogin(JSONObject e)
    {
        if (e == true)
        {
            LoginPanel.SetActive(false);
            ChatPanel.SetActive(true);

            socket.On("join", OnJoin);
            socket.On("message", OnMessage);
        }
        else
        {
            Logout();
        }
    }

    void OnChat(JSONObject e)
    {
        if (e == true) { Debug.Log("Message sent"); } else { Debug.Log("Message can\'t send"); }
    }

    void OnMessage(SocketIOEvent e)
    {
        ChatContent.Add(new ChatResult(new JSONObject(e.data.ToString())).ToString());
    }

    void OnJoin(SocketIOEvent e)
    {
        ChatContent.Add(new ChatResult(new JSONObject(e.data.ToString())).ToString());
    }

    public void Logout()
    {
        // close socket
        socket.Close();

        LoginPanel.SetActive(true);
        ChatPanel.SetActive(false);
        UserName = null;
    }

    public static string GenerateName(int len)
    {
        System.Random r = new System.Random();
        string[] consonants = { "b", "c", "d", "f", "g", "h", "j", "k", "l", "m", "l", "n", "p", "q", "r", "s", "sh", "zh", "t", "v", "w", "x" };
        string[] vowels = { "a", "e", "i", "o", "u", "ae", "y" };
        string Name = "";
        Name += consonants[r.Next(consonants.Length)].ToUpper();
        Name += vowels[r.Next(vowels.Length)];
        int b = 2; //b tells how many times a new letter has been added. It's 2 right now because the first two letters are already in the name.
        while (b < len)
        {
            Name += consonants[r.Next(consonants.Length)];
            b++;
            Name += vowels[r.Next(vowels.Length)];
            b++;
        }

        return Name;
    }
}