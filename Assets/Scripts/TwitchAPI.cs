using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;
using System.Threading;
using System.Net.Sockets;
using System.IO;

public class TwitchAPI : MonoBehaviour
{
    public Text followerCountText;
    public Text viewerCountText;
    public Text subscriberCountText;
    public Text chatText;

    private string accessToken = "your_access_token";
    private string clientId = "your_client_id";
    private string channelId = "your_channel_id"; // You can retrieve this via the Twitch API
    private string username = "your_twitch_username";
    private TwitchChatClient chatClient;

    private async void Start()
    {
        await UpdateTwitchData();
        StartChatClient();
    }

    // Call Twitch API for Follower, Viewer, and Subscriber Counts
    private async Task UpdateTwitchData()
    {
        await GetFollowerCount();
        await GetViewerCount();
        await GetSubscriberCount();
    }

    // Get Follower Count
    private async Task GetFollowerCount()
    {
        string url = $"https://api.twitch.tv/helix/users/follows?to_id={channelId}";
        string response = await MakeTwitchRequest(url);
        JObject data = JObject.Parse(response);
        string followers = data["total"].ToString();
        followerCountText.text = $"Followers: {followers}";
    }

    // Get Viewer Count
    private async Task GetViewerCount()
    {
        string url = $"https://api.twitch.tv/helix/streams?user_id={channelId}";
        string response = await MakeTwitchRequest(url);
        JObject data = JObject.Parse(response);

        if (data["data"].HasValues)
        {
            string viewers = data["data"][0]["viewer_count"].ToString();
            viewerCountText.text = $"Viewers: {viewers}";
        }
        else
        {
            viewerCountText.text = "Viewers: 0"; // Not live
        }
    }

    // Get Subscriber Count (Requires Scope: channel:read:subscriptions)
    private async Task GetSubscriberCount()
    {
        string url = $"https://api.twitch.tv/helix/subscriptions?broadcaster_id={channelId}";
        string response = await MakeTwitchRequest(url);
        JObject data = JObject.Parse(response);
        string subscribers = data["total"].ToString();
        subscriberCountText.text = $"Subscribers: {subscribers}";
    }

    // Make a generic Twitch API request
    private async Task<string> MakeTwitchRequest(string url)
    {
        using (HttpClient client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
            client.DefaultRequestHeaders.Add("Client-Id", clientId);

            HttpResponseMessage response = await client.GetAsync(url);
            return await response.Content.ReadAsStringAsync();
        }
    }

    // Initialize Twitch Chat Client
    private void StartChatClient()
    {
        chatClient = new TwitchChatClient(username, accessToken);
        chatClient.Connect();
        chatClient.OnMessageReceived += DisplayChatMessage;
    }

    // Display chat messages in Unity UI
    private void DisplayChatMessage(string message)
    {
        chatText.text += message + "\n"; // Appending chat messages
    }
}

public class TwitchChatClient
{
    private string username;
    private string oauthToken;
    private TcpClient twitchClient;
    private StreamReader reader;
    private StreamWriter writer;

    public delegate void MessageReceivedHandler(string message);
    public event MessageReceivedHandler OnMessageReceived;

    public TwitchChatClient(string username, string oauthToken)
    {
        this.username = username;
        this.oauthToken = oauthToken;
    }

    // Connect to Twitch IRC
    public void Connect()
    {
        twitchClient = new TcpClient("irc.chat.twitch.tv", 6667);
        reader = new StreamReader(twitchClient.GetStream());
        writer = new StreamWriter(twitchClient.GetStream());

        writer.WriteLine($"PASS oauth:{oauthToken}");
        writer.WriteLine($"NICK {username}");
        writer.WriteLine($"JOIN #{username}");
        writer.Flush();

        // Start listening for chat messages in a separate thread
        Thread chatThread = new Thread(ReadChat);
        chatThread.Start();
    }

    // Read chat messages from Twitch IRC
    private void ReadChat()
    {
        while (twitchClient.Connected)
        {
            if (twitchClient.Available > 0)
            {
                string message = reader.ReadLine();

                if (message.Contains("PRIVMSG"))
                {
                    // Extract message
                    int idx = message.IndexOf(":", 1);
                    string chatMessage = message.Substring(idx + 1);
                    OnMessageReceived?.Invoke(chatMessage);
                }
            }
        }
    }
}
