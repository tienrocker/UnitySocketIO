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
    public string SocketID;
    public GameObject LoginPanel;
    public GameObject ChatPanel;
    public UILabel ChatContent;

    public bool isAutoChat = false;
    private string UserName;

    public List<string> DictionaryQuote = new List<string>();

    public IEnumerator Start()
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

        while (!socket.IsConnected)
            yield return new WaitForSeconds(0.05f);

        socket.On("open", OnOpen);
        socket.On("message", OnMessage);
        socket.On("error", OnError);
        socket.On("close", OnCLose);

        LoginPanel.SetActive(true);
        ChatPanel.SetActive(false);
    }

    public void Login(string username)
    {
        JSONObject user = new JSONObject();
        if (isAutoChat)
            username = GenerateName(Random.Range(4, 7));

        user.AddField("username", username);
        socket.Emit("login", user);
        UserName = username;

        if (isAutoChat)
            StartCoroutine("AutoChat");
    }

    public void Chat(string text)
    {
        JSONObject content = new JSONObject();
        content.AddField("text", text);
        socket.Emit("chat", content);
    }

    public IEnumerator AutoChat()
    {
        System.Random rnd = new System.Random();
        JSONObject content = new JSONObject();
        content.AddField("text", DictionaryQuote[rnd.Next(DictionaryQuote.Count)]);
        socket.Emit("chat", content);

        yield return new WaitForSeconds(0.5f);
        StartCoroutine("AutoChat");
    }

    void OnMessage(SocketIOEvent e)
    {
        ProcessMessage(e);
    }

    void ProcessMessage(SocketIOEvent e)
    {
        Result result = new Result(new JSONObject(e.data.ToString()));

        if (result.log != null)
            Debug.LogFormat("[SocketIO-{0}] {1}", result.messagetype.ToString(), result.log);

        if (result.messagetype == Result.messageType.LOGIN)
        {
            SocketID = result.from;
            LoginPanel.SetActive(false);
            ChatPanel.SetActive(true);
            ChatContent.text = string.Format("[009900]{0} joined[-]\n{1}", result.content, ChatContent.text);
        }

        if (result.messagetype == Result.messageType.CHAT)
        {
            if (UserName == "Admin")
            {
                ChatContent.text = string.Format("[99ff00]{0}:[-] {1}\n{2}", result.from, result.log, ChatContent.text);
            }
            else
            {
                ChatContent.text = string.Format("[99ff00]{0}:[-] {1}\n{2}", result.from, result.content, ChatContent.text);
            }

        }
    }

    #region default function
    void OnOpen(SocketIOEvent e)
    {
        Debug.Log("[SocketIO-OnOpen] Error received: " + e.name + " " + e.data);
        if (UserName != null)
            Login(UserName);
    }

    void OnError(SocketIOEvent e)
    {
        Debug.Log("[SocketIO-OnError] Error received: " + e.name + " " + e.data);
    }

    void OnCLose(SocketIOEvent e)
    {
        Debug.Log("[SocketIO-OnCLose] Error received: " + e.name + " " + e.data);
        ChatContent.text = string.Format("[FF0000]Disconnected[-]\n{2}");
    }
    #endregion default function

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

class Result
{
    public enum messageType { ERROR, CONNECT, LOGIN, NOTIFY, CHAT };
    public messageType messagetype = messageType.ERROR;
    public string from;
    public string content;
    public string log;

    public Result(JSONObject obj)
    {
        messagetype = (messageType)obj.GetField("messagetype").n;
        from = obj.GetField("from").str;
        content = obj.GetField("content").str;
        log = obj.GetField("log").str;
    }

    void accessData(JSONObject obj)
    {
        switch (obj.type)
        {
            case JSONObject.Type.OBJECT:
                for (int i = 0; i < obj.list.Count; i++)
                {
                    string key = (string)obj.keys[i];
                    JSONObject j = (JSONObject)obj.list[i];
                    Debug.Log(key);
                    accessData(j);
                }
                break;
            case JSONObject.Type.ARRAY:
                foreach (JSONObject j in obj.list)
                {
                    accessData(j);
                }
                break;
            case JSONObject.Type.STRING:
                Debug.Log(obj.str);
                break;
            case JSONObject.Type.NUMBER:
                Debug.Log(obj.n);
                break;
            case JSONObject.Type.BOOL:
                Debug.Log(obj.b);
                break;
            case JSONObject.Type.NULL:
                Debug.Log("NULL");
                break;

        }
    }
}