using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class ChatResult
{
    private string[] color = new string[12]{
    "e21400", "91580f", "f8a700", "f78b00",
    "58dc00", "287b00", "a8f07a", "4ae8c4",
    "3b88eb", "3824aa", "a700ff", "d300e7"
  };

    public string colorhex;
    public string username;
    public string message;

    public ChatResult(JSONObject obj)
    {
        username = obj.GetField("username").str;
        message = obj.GetField("message").str;
        colorhex = getUsernameColor(username);
    }

    public override string ToString()
    {
        return string.Format("[{0}]{1}:[-] {2}", colorhex, username, message);
    }

    public string getUsernameColor(string username)
    {
        // Compute hash code
        var hash = 7;
        for (var i = 0; i < username.Length; i++)
        {
            hash = username[i] + (hash << 5) - hash;
        }
        // Calculate color
        var index = Math.Abs(hash % color.Length);
        return color[index];
    }
}