 using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

	public float levelStartDelay = 2f;
    public float turnDelay = .1f;
    public static GameManager instance = null;
    public BoardManager boardScript;
    public int playerFoodPoints = 100;
    [HideInInspector]
    public bool playersTurn = true;

	private Text levelText;
	private GameObject levelImage;
    private int level = 3;
    private List<Enemy> enemies;
    private bool enemiesMoving;
	private bool doingSetup;

	// Use this for initialization
	void Awake () {

        if (instance == null)
        {
            instance = this;
        }else if (instance != this)
        {
            Destroy(gameObject);
        }
        enemies = new List<Enemy>();
        boardScript = GetComponent<BoardManager>();
        InitGame();

	}

	private void OnlevelWasLoaded(int index){
		level++;
		InitGame();
	}

    public void GameOver()
    {
		levelText.text = "After " + level + " days, you starved.";
		levelImage.SetActive (true);
        enabled = false;
    }

    void InitGame()
    {
		doingSetup = true;
		levelImage = GameObject.Find ("LevelImage");
		levelText = GameObject.Find ("LevelText").GetComponent<Text> ();
		levelText.text = "Day " + level;
		Invoke ("HideLevelImage", levelStartDelay);

        enemies.Clear();
        boardScript.SetupScene(level);
    }

	private void HideLevelImage(){
		levelImage.SetActive (false);
		doingSetup = false;
	}

	// Update is called once per frame
	void Update () {
        if (playersTurn || enemiesMoving)
            return;

        StartCoroutine(MoveEnemies());
	}

    public void AddEnemyToList(Enemy script)
    {
        enemies.Add(script);
    }

    IEnumerator MoveEnemies()
    {
        enemiesMoving = true;
        yield return new WaitForSeconds(turnDelay);

        if (enemies.Count == 0)
        {
            //Wait for turnDelay seconds between moves, replaces delay caused by enemies moving when there are none.
            yield return new WaitForSeconds(turnDelay);
        }

        for (int i = 0; i < enemies.Count; i++)
        {
            //Call the MoveEnemy function of Enemy at index i in the enemies List.
            enemies[i].MoveEnemy();

            //Wait for Enemy's moveTime before moving next Enemy, 
            yield return new WaitForSeconds(enemies[i].moveTime);
        }
        //Once Enemies are done moving, set playersTurn to true so player can move.
        playersTurn = true;

        //Enemies are done moving, set enemiesMoving to false.
        enemiesMoving = false;
    }
}
