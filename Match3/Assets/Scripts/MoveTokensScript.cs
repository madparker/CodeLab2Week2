using UnityEngine;
using System.Collections;

public class MoveTokensScript : MonoBehaviour
{

    protected GameManagerScript gameManager; // A reference to the GameManagerScript component on this script's gameObject.
    protected MatchManagerScript matchManager; // A reference to the MatchManagerScript component on this script's gameObject.

    public bool move = false; //Determines whether or not the selected icons can move.

    public float lerpPercent; //Used to track how much the icons have moved from their original positions, as a percentage between 0f and 1f.
    public float lerpSpeed; //How quickly the lerp percentage is increased.

    bool userSwap; //

    protected GameObject exchangeToken1;
    GameObject exchangeToken2;

    Vector2 exchangeGridPos1;
    Vector2 exchangeGridPos2;

    public virtual void Start()
    {
        gameManager = GetComponent<GameManagerScript>();
        matchManager = GetComponent<MatchManagerScript>();
        lerpPercent = 0;
    }

    public virtual void Update()
    {

        if (move)
        {
            lerpPercent += lerpSpeed;

            if (lerpPercent >= 1)
            {
                lerpPercent = 1;
            }

            if (exchangeToken1 != null)
            {
                ExchangeTokens();
            }
        }
    }

    /// <summary>
    /// This function resets the lerp percentage before setting up the icons through SetupTokenExchange.
    /// </summary>
    public void SetupTokenMove()
    {
        move = true;
        lerpPercent = 0;
    }

    /// <summary>
    /// This function sets up the exchange of two tokens.
    /// </summary>
    /// <param name="token1">The first token you clicked on.</param>
    /// <param name="pos1">The location of token1 in the game scene.</param>
    /// <param name="token2">The second token you clicked on.</param>
    /// <param name="pos2">The location of token2 in the game scene.</param>
    /// <param name="reversable">Can this exchange be reversed? (Useful if the icons don't match for a match 3.)</param>
	public void SetupTokenExchange(GameObject token1, Vector2 pos1,
                                   GameObject token2, Vector2 pos2, bool reversable)
    {
        SetupTokenMove();

        exchangeToken1 = token1;
        exchangeToken2 = token2;

        exchangeGridPos1 = pos1;
        exchangeGridPos2 = pos2;


        this.userSwap = reversable;
    }

    public virtual void ExchangeTokens()
    {

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
