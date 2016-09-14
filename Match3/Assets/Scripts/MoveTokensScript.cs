using UnityEngine;
using System.Collections;

public class MoveTokensScript : MonoBehaviour
{

    protected GameManagerScript gameManager; // A reference to the GameManagerScript component on this script's gameObject.
    protected MatchManagerScript matchManager; // A reference to the MatchManagerScript component on this script's gameObject.

    public bool move = false; //Determines whether or not the selected tokens are moving.

    public float lerpPercent; //Used to track how much the tokens have moved from their original positions, as a percentage between 0f and 1f.
    public float lerpSpeed; //How quickly the lerp percentage is increased.

    bool userSwap; //Have the user tokens been swapped or not??

    protected GameObject exchangeToken1; //A reference to the GameObject token that the user clicked first.
    GameObject exchangeToken2; //A reference to the GameObject token that the user clicked second.

    Vector2 exchangeGridPos1; //The position of exchangeToken1 in the scene.
    Vector2 exchangeGridPos2; //The position of exchangeToken2 in the scene.

    public virtual void Start()
    {
        gameManager = GetComponent<GameManagerScript>(); //Set the gameManager variable to the GameManagerScript component on the GameObject.
        matchManager = GetComponent<MatchManagerScript>(); //Set the matchManager variable to the MatchManagerScript component on the GameObject.
        lerpPercent = 0; //Reset the lerp percentage to 0 so that tokens don't move until the right time.
    }

    public virtual void Update()
    {

        if (move) //If the tokens are moving:
        {
            lerpPercent += lerpSpeed; //...make lerpPercent = lerpPercent + lerpSpeed.

            if (lerpPercent >= 1) //If the lerpPercentage exceeds 1f:
            {
                lerpPercent = 1; //...force it back to 1f.
            }

            if (exchangeToken1 != null) //If exchangeToken1 is a valid GameObject:
            {
                ExchangeTokens(); //...run the ExchangeTokens function.
            }
        }
    }

    /// <summary>
    /// This function resets the lerp percentage before setting up the tokens through SetupTokenExchange.
    /// </summary>
    public void SetupTokenMove()
    {
        move = true; //The tokens are now moving.
        lerpPercent = 0; //Reset the lerp percentage to 0 so that tokens don't move until the right time.
    }

    /// <summary>
    /// This function sets up the exchange of two tokens.
    /// </summary>
    /// <param name="token1">The first token you clicked on.</param>
    /// <param name="pos1">The location of token1 in the game scene.</param>
    /// <param name="token2">The second token you clicked on.</param>
    /// <param name="pos2">The location of token2 in the game scene.</param>
    /// <param name="reversable">Can this exchange be reversed? (Useful if the tokens don't match for a match 3.)</param>
	public void SetupTokenExchange(GameObject token1, Vector2 pos1,
                                   GameObject token2, Vector2 pos2, bool reversable)
    {
        SetupTokenMove(); //Run SetupTokenMove so that the tokens will start moving.

        exchangeToken1 = token1; //Set exchangeToken1 to the first token you clicked on.
        exchangeToken2 = token2; //Set exchangeToken2 to the second token you clicked on.

        exchangeGridPos1 = pos1; //Set exchangeGridPos1 to the position of the first token you clicked on.
        exchangeGridPos2 = pos2; //Set exchangeGridPos2 to the position of the second token you clicked on.


        this.userSwap = reversable; //Let the game know that the tokens have been swapped.
    }

    /// <summary>
    /// Exchange the two tokens that have been selected.
    /// </summary>
    public virtual void ExchangeTokens()
    {
        /*
        The next two lines set references to the locations we're using to swap.
        startPos is set to the position of wherever token1 is, WITHIN the game's grid position.
        endPos is set to the position of wherever token2 is, WITHIN the game's grid position.

        The positions are converted into grid positions by gameManager.GetWorldPositionFromGridPosition.
        */
        Vector3 startPos = gameManager.GetWorldPositionFromGridPosition((int)exchangeGridPos1.x, (int)exchangeGridPos1.y);
        Vector3 endPos = gameManager.GetWorldPositionFromGridPosition((int)exchangeGridPos2.x, (int)exchangeGridPos2.y);

        //		Vector3 movePos1 = Vector3.Lerp(startPos, endPos, lerpPercent);
        //		Vector3 movePos2 = Vector3.Lerp(endPos, startPos, lerpPercent);

        Vector3 movePos1 = SmoothLerp(startPos, endPos, lerpPercent);
        Vector3 movePos2 = SmoothLerp(endPos, startPos, lerpPercent);

        exchangeToken1.transform.position = movePos1;
        exchangeToken2.transform.position = movePos2;

        if (lerpPercent == 1)
        {
            gameManager.gridArray[(int)exchangeGridPos2.x, (int)exchangeGridPos2.y] = exchangeToken1;
            gameManager.gridArray[(int)exchangeGridPos1.x, (int)exchangeGridPos1.y] = exchangeToken2;

            if (!matchManager.GridHasMatch() && userSwap)
            {
                SetupTokenExchange(exchangeToken1, exchangeGridPos2, exchangeToken2, exchangeGridPos1, false);
            }
            else {
                exchangeToken1 = null;
                exchangeToken2 = null;
                move = false;
            }
        }
    }

    private Vector3 SmoothLerp(Vector3 startPos, Vector3 endPos, float lerpPercent)
    {
        return new Vector3(
            Mathf.SmoothStep(startPos.x, endPos.x, lerpPercent),
            Mathf.SmoothStep(startPos.y, endPos.y, lerpPercent),
            Mathf.SmoothStep(startPos.z, endPos.z, lerpPercent));
    }

    public virtual void MoveTokenToEmptyPos(int startGridX, int startGridY,
                                    int endGridX, int endGridY,
                                    GameObject token)
    {

        Vector3 startPos = gameManager.GetWorldPositionFromGridPosition(startGridX, startGridY);
        Vector3 endPos = gameManager.GetWorldPositionFromGridPosition(endGridX, endGridY);

        Vector3 pos = Vector3.Lerp(startPos, endPos, lerpPercent);

        token.transform.position = pos;

        if (lerpPercent == 1)
        {
            gameManager.gridArray[endGridX, endGridY] = token;
            gameManager.gridArray[startGridX, startGridY] = null;
        }
    }

    public virtual bool MoveTokensToFillEmptySpaces()
    {
        bool movedToken = false;

        for (int x = 0; x < gameManager.gridWidth; x++)
        {
            for (int y = 1; y < gameManager.gridHeight; y++)
            {
                if (gameManager.gridArray[x, y - 1] == null)
                {
                    for (int pos = y; pos < gameManager.gridHeight; pos++)
                    {
                        GameObject token = gameManager.gridArray[x, pos];
                        if (token != null)
                        {
                            MoveTokenToEmptyPos(x, pos, x, pos - 1, token);
                            movedToken = true;
                        }
                    }
                }
            }
        }

        if (lerpPercent == 1)
        {
            move = false;
        }

        return movedToken;
    }
}
