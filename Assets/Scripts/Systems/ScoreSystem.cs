using LootLocker.Requests;
using System.Collections;
using UnityEngine;

public class ScoreSystem : MonoBehaviour
{

    private const int LEADERBOARD_ID = 15145;
    private const string LEADERBOARD_KEY = "test_key";

    void Start()
    {
        StartCoroutine(LoginPlayerAsGuest());
    }

    
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    StartCoroutine(SubmitScore(200));
        //}
        //if (Input.GetKeyDown(KeyCode.S))
        //{
        //    StartCoroutine(GetSpecifiedScoresFromLeaderboard(50));
        //}
    }


    private IEnumerator GetSpecifiedScoresFromLeaderboard(int count)
    {
        bool done = false;
        LootLockerSDKManager.GetScoreList(LEADERBOARD_KEY, count, 0, (response) =>
        {
            if (response.statusCode == 200)
            {
                Debug.Log("Successful");
               
                for (int i = 0; i < response.items.Length; i++)
                {
                    Debug.Log(response.items[i].ToString());
                }

                done = true;
            }
            else
            {
                Debug.Log("failed: " + response.Error);
                done = true;
            }
        });
        yield return new WaitWhile(() => done == false);
    }

    private IEnumerator LoginPlayerAsGuest()
    {
        bool done = false;
        LootLockerSDKManager.StartGuestSession((response) =>
        {
            if (response.success)
            {
                Debug.Log("Player was logged in");
                PlayerPrefs.SetString("PlayerID", response.player_id.ToString());

                done = true;
            }
            else
            {
                Debug.Log("Could not start session");
                done = true;
            }
        });
        yield return new WaitWhile(() => done == false);
        
    }


    [System.Obsolete]
    public IEnumerator SubmitScore(int score)
    {
        bool done = false;
        LootLockerSDKManager.SubmitScore(PlayerPrefs.GetString("PlayerID"), score, LEADERBOARD_ID, (response) =>
        {
            if (response.statusCode == 200)
            {
                Debug.Log("Successful");
            }
            else
            {
                Debug.Log("failed: " + response.Error);
            }
        });
        yield return new WaitWhile(() => done == false);
    }
}
