using System.Collections;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    [SerializeField] GameObject gameManagerObject;

    private States currentGameState = States.GameNeutral;
    private ItemSpawner itemSpawner;
    private ScoreSystem scoreSystem;

    


    // On every item pick up, add some to the multiplier
    // Then, using the multiplier, determine how many employees are spawned at a time
    // as well as determine how long the timer is
    private int gameDifficultyMultiplier;
    private float deadlineBetweenItemPickup;
    private float currentTimeLeft;
    private int currentSpawnedEmployees;
    

    void Start()
    {
        this.itemSpawner = gameManagerObject.GetComponent<ItemSpawner>();
        this.scoreSystem = gameManagerObject.GetComponent<ScoreSystem>();
        this.currentTimeLeft = 45f; // Start with 45 seconds on your timer
        this.deadlineBetweenItemPickup = 45f;
    }



    void Update()
    {
        Debug.Log("Time left: " +  currentTimeLeft);

        if (Input.GetKeyUp(KeyCode.Q) && currentGameState != States.GameRunning)
        {
            Debug.Log("GAME STARTED");

            currentGameState = States.GameRunning;
            itemSpawner.ChooseNextShelf();
        }

        if (currentGameState == States.GameRunning) 
        {
            if (currentTimeLeft < 0f)
            {
                currentGameState = States.GameEnd;

                Debug.Log("-------------- Game ended -----------------");

                // HANDLE GAME END HERE
            }

            //
            if (currentTimeLeft > 0f)
            {
                currentTimeLeft -= Time.deltaTime;
            }

        }
    }

    public void ConsumeItem()
    {
        scoreSystem.currentScore += 1;

        Debug.Log("Current score is:" + scoreSystem.currentScore);

        // Handle Consuming effects

        itemSpawner.ChooseNextShelf();

        // Reset timer OR add more to time
        currentTimeLeft = deadlineBetweenItemPickup;

    }

    private void IncreaseGameDifficulty()
    {
        // Max difficulty is 30;
        int currScore = scoreSystem.currentScore;
        if (currentTimeLeft >= 15)
        {
            deadlineBetweenItemPickup--;
            gameDifficultyMultiplier++;
        }
    }

}

public enum States 
{
    GameEnd,
    GameRunning,
    GameNeutral
}
