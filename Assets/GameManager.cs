using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Card CardPrefab;

    public List<Card> LocalCards, Hand;

    public Transform HandPosition;

    public CardScriptableObject[] ScriptableObjects;

    public string userID, userPass, userEmail;

    public Text User;

    public bool LoggedIn;

    public void PrepareGame()
    {
        LocalCards = new List<Card>();
        Hand = new List<Card>();
        foreach (var item in ScriptableObjects)
        {
            Card c = Instantiate(CardPrefab);
            c.init(item);
            c.name = item.name;
            LocalCards.Add(c);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        LoggedIn = false;
        PrepareGame();
    }

    public void SetLogin(string ID)
    {
        userID = ID;
    }
    public void SetPassword(string Pass)
    {
        userPass = Pass;
    }

    public void SetEmail(string Email)
    {
        userEmail = Email;
    }

    public void Register()
    {
        if (!string.IsNullOrEmpty(userID) && !string.IsNullOrEmpty(userPass))
        {

            var request = new RegisterPlayFabUserRequest
            {
                Username = userID,
                Password = userPass,
                RequireBothUsernameAndEmail = false,
                TitleId = PlayFabSettings.staticSettings.TitleId

            };

            PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnError);
        }
    }



    public void Login()
    {
        if (!string.IsNullOrEmpty(userPass))
        {
            if (!string.IsNullOrEmpty(userID))
            {
                var request = new LoginWithPlayFabRequest
                {
                    Username = userID,
                    Password = userPass

                };

                PlayFabClientAPI.LoginWithPlayFab(request, OnLoginSuccess, OnError);
            }
            else if (!string.IsNullOrEmpty(userEmail))
            {
                var request = new LoginWithEmailAddressRequest
                {
                    Email = userEmail,
                    Password = userPass

                };

                PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnError);

            }
        }
    }

    public void SendCardData()
    {
        Dictionary<string, string> Data = new Dictionary<string, string>();
        String Numbers = "";
        foreach (Card item in Hand)
        {
            Numbers += item.Template.Number + ",";
        }

        Data.Add("CardNumber", Numbers);

        var request = new UpdateUserDataRequest
        {
            Data = Data
            

        };

        PlayFabClientAPI.UpdateUserData(request, OnSendSucceed, OnError);
    }

    public void GetCardData()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), OnDataRecieved, OnError);

    }

    private void OnDataRecieved(GetUserDataResult obj)
    {
        Debug.Log("Data successfully recieved!");

        if (obj.Data.ContainsKey("CardNumber"))
        {
            string[] cards = obj.Data["CardNumber"].Value.Split(',');
            ResetCards();

            foreach (var item in cards)
            {
                if (!String.IsNullOrEmpty(item))
                {
                    int i = int.Parse(item);
                    Card c = LocalCards.Find(o => o.Template.Number == i);
                    c.transform.localPosition = HandPosition.localPosition + Vector3.right * Hand.Count * 2f;
                    Hand.Add(c);
                }
            }

            foreach (var item in Hand)
            {
                LocalCards.Remove(item);
            }
        }
    }

    private void OnSendSucceed(UpdateUserDataResult obj)
    {
        Debug.Log("Data successfully sent!");
    }

    public void NewHand()
    {
        ResetCards();

        while (Hand.Count != 7)
        {
            int i = UnityEngine.Random.Range(0, LocalCards.Count);
            Card c = LocalCards[i];
            c.transform.localPosition = HandPosition.localPosition + Vector3.right * Hand.Count * 2f;
            LocalCards.RemoveAt(i);
            Hand.Add(c);
        }

    }

    private void ResetCards()
    {
        for (int i = Hand.Count - 1; i >= 0; i--)
        {
            Card c = Hand[i];
            c.transform.localPosition = new Vector3(-1000, -1000, 0);
            Hand.RemoveAt(i);
            LocalCards.Add(c);
        }
    }

    private void OnError(PlayFabError obj)
    {
        Debug.LogError(obj.Error + ": " + obj.ErrorMessage);
    }

    private void OnRegisterSuccess(RegisterPlayFabUserResult obj)
    {
        LoggedIn = true;
        Debug.Log("User " + userID + " registered Successfully!");
    }

    private void OnLoginSuccess(LoginResult result)
    {
        LoggedIn = true;
        User.text = result.PlayFabId;
        Debug.Log("User " + userID + " logged in Successfully!");
    }

    private void OnLoginFailure(PlayFabError error)
    {
        LoggedIn = false;
        Debug.LogWarning("Something went wrong with your first API call.  :(");
        Debug.LogError("Here's some debug information:");
        Debug.LogError(error.GenerateErrorReport());
    }
}
