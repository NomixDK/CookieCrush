using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridManager : MonoBehaviour
{
    public int height;
    public int width;
    public int StartingTurns;
    public int turnsLeft;
    public int currentScore;
    public GameObject tilePrefab;
    public GameObject[] cookies;
    private BackgroundTile[,] allTiles;
    public GameObject[,] allCookies;
    public Text textValue;
    public Text textValue2;

    // Start is called before the first frame update
    void Start()
    {
        allTiles = new BackgroundTile[width, height];
        allCookies = new GameObject[width, height];
        StartUp();
        turnsLeft = StartingTurns;

    }


    private void StartUp() //
    {
        for (int i = 0; i < width; i++) //Flusher igennem hele "Griddet mens at den laver Objekter der bliver defineret i "Game Object""
        {
            for (int i1 = 0; i1 < height; i1++)
            {
                Vector2 tempPosition = new Vector2(i, i1);
                GameObject backgroundTile = Instantiate(tilePrefab, tempPosition, Quaternion.identity) as GameObject;
                backgroundTile.transform.parent = this.transform;
                backgroundTile.name = "( " + i + ", " + i1 + " )"; //Navngiver "tiles" til position

                    int _cookie = Random.Range(0, cookies.Length);
                    int maxIterations = 0;

                        while(MatchesAt(i , i1, cookies[_cookie]) && maxIterations < 100)
                        {
                        _cookie = Random.Range(0, cookies.Length);
                        maxIterations++;
                        }

                        maxIterations = 0;

                        GameObject cookie = Instantiate(cookies[_cookie], tempPosition, Quaternion.identity);
                        cookie.transform.parent = this.transform; //Childer til "BackgroundTile"
                        cookie.name = "C " + "( " + i + ", " + i1 + " )";
                        allCookies[i, i1] = cookie;


            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        textValue.text = turnsLeft.ToString();
        textValue2.text = currentScore.ToString();
    }

   private void DestroyCookiesCord (int column, int row) //Sub funktion 
    {
        if (allCookies[column,row].GetComponent<Cookie>().isMatched)
            {
                Destroy(allCookies[column, row]);
                allCookies[column, row] = null;
            }
    }

    public void DestroyCookies () //Sub funktion 
    {
        AudioSource audio3 = gameObject.AddComponent<AudioSource>();
        AudioClip Pop = (AudioClip)Resources.Load("pop");

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if(allCookies[i, j] != null)
                {
                    DestroyCookiesCord(i, j);
                    currentScore += 2;
                    audio3.PlayOneShot(Pop, 1.0f);
                }
            }
        }
        StartCoroutine(Collapse());
    }

    private bool MatchesAt(int column, int row, GameObject piece) {
        if (column > 1 && row > 1)
        {
            if(allCookies[column -1, row].tag == piece.tag && allCookies[column -2, row].tag == piece.tag)
            {
                return true;
            }
            if (allCookies[column, row -1].tag == piece.tag && allCookies[column, row -2].tag == piece.tag)
            {
                return true;
            }

        } else if(column <= 1 || row <= 1)
        {
            if(row > 1)
            {
                if (allCookies[column, row - 1].tag == piece.tag && allCookies[column, row - 2].tag == piece.tag)
                {
                    return true;
                } if(column > 1)
                {
                    if (allCookies[column - 1, row].tag == piece.tag && allCookies[column - 2, row].tag == piece.tag)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private IEnumerator Collapse()
    {
        int nullCount = 0;
        for(int i = 0; i < width; i++) {
            for(int j = 0; j < height; j++)
            {
                if(allCookies[i,j] == null)
                {
                    nullCount++;
                } else if (nullCount > 0)
                {
                    allCookies[i, j].GetComponent<Cookie>().row -= nullCount;
                    allCookies[i, j] = null;
                }
            }
            nullCount = 0;
        }
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(Fillboard());
    }

    private void BoardRefill()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if(allCookies[i, j] == null)
                {
                    Vector2 tempPosition = new Vector2(i, j);
                    int cookie_ = Random.Range(0, cookies.Length);
                    GameObject piece = Instantiate(cookies[cookie_], tempPosition, Quaternion.identity);
                    allCookies[i, j] = piece;
                }
            }
        }
    }

    private bool MatchesOnBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++) 
            {
                if (allCookies[i, j] != null)
                {
                    if(allCookies[i, j].GetComponent<Cookie>().isMatched)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private IEnumerator Fillboard()
    {
        BoardRefill();
        yield return new WaitForSeconds(0.5f);

        while(MatchesOnBoard())
        {
            yield return new WaitForSeconds(0.5f);
            DestroyCookies();
        }
    }
}

