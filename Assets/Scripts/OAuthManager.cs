using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class OAuthManager : MonoBehaviour
{
    public string clientId = "your_client_id";
    public string redirectUri = "http://localhost";
    public string[] scopes = { "user:read:email", "chat:read", "channel:read:subscriptions" }; // Add more scopes if needed
    public Text accessTokenText;

    private string accessToken = "";

    void Start()
    {
        // Create the URL for OAuth2
        string scopeString = string.Join("+", scopes);
        string authUrl = $"https://id.twitch.tv/oauth2/authorize?response_type=token&client_id={clientId}&redirect_uri={redirectUri}&scope={scopeString}";

        // Open the URL in the browser for the user to authenticate
        Application.OpenURL(authUrl);
    }

    // This method will be called once the user gets redirected back with the token
    public void HandleRedirectUrl(string url)
    {
        // Check if the URL contains the access token
        if (url.Contains("access_token="))
        {
            // Extract the access token from the URL
            string[] parts = url.Split('#');
            if (parts.Length > 1)
            {
                string[] parameters = parts[1].Split('&');
                foreach (string param in parameters)
                {
                    if (param.StartsWith("access_token="))
                    {
                        accessToken = param.Split('=')[1];
                        accessTokenText.text = "Access Token: " + accessToken;
                        break;
                    }
                }
            }
        }
        else
        {
            Debug.LogError("Access token not found in the URL.");
        }
    }

    public string GetAccessToken()
    {
        return accessToken;
    }
}
