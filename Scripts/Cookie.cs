using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Cookie : MonoBehaviour
{
    public int column; //x
    public int row; //y
    public int targetX;
    public int targetY;
    public int preColumn;
    public int preRow;

    private GridManager board;
    private GameObject ThisCookie;
    private GameObject otherCookie;
    private Vector2 firstTouchPos;
    private Vector2 finalTouchPos;
    private Vector2 tempPos;

    public float swipeAngle = 0f;
    public float swipeResist = 1f;
    public bool isMatched = false;

    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<GridManager>();
        targetX = (int)transform.position.x;
        targetY = (int)transform.position.y;

        column = targetX;
        row = targetY;

        preRow = row;
        preColumn = column;
    }

    // Update is called once per frame
    void Update()
    { 
        FindMatch();
        if (isMatched)
        {
            SpriteRenderer thisSprite = GetComponent<SpriteRenderer>();
            thisSprite.color = new Color(0f, 0f, 0f, 0.75f);
        }

        targetX = column;
        targetY = row;
        if (Mathf.Abs(targetX - transform.position.x) > 0.1)
        {
            //Hvis den ikke er tæt på så ryk imod i stedet
            tempPos = new Vector2(targetX, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, tempPos, 0.6f);
            if(board.allCookies[column,row] != this.gameObject)
            {
                board.allCookies[column, row] = this.gameObject;
            }
        }
        else
            {
            //Sæt positionen
            tempPos = new Vector2(targetX, transform.position.y);
            transform.position = tempPos;
            board.allCookies[column, row] = this.gameObject;
        }
        if (Mathf.Abs(targetY - transform.position.y) > 0.1)
        {
            //Hvis den ikke er tæt på så ryk imod i stedet
            tempPos = new Vector2(transform.position.x, targetY);
            transform.position = Vector2.Lerp(transform.position, tempPos, 0.6f);
            if (board.allCookies[column, row] != this.gameObject)
            {
                board.allCookies[column, row] = this.gameObject;
            }
        }
        else
        {
            //Sæt positionen
            tempPos = new Vector2(transform.position.x, targetY);
            transform.position = tempPos;
            board.allCookies[column, row] = this.gameObject;
        }
    }

    void OnMouseDown()
    {
        if (board.turnsLeft > 0)
        {
            AudioSource audio = gameObject.AddComponent<AudioSource>();
            AudioClip ClickClipDown = (AudioClip)Resources.Load("ClickDown");
            audio.PlayOneShot(ClickClipDown, 1.0f);
            firstTouchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }

    void OnMouseUp() //Definerer "finalTouchPos" udfra hvor musen er placeret iden for kameraet
    {
        if (board.turnsLeft > 0)
        {
            AudioSource audio1 = gameObject.AddComponent<AudioSource>();
            AudioClip ClickClipUp = (AudioClip)Resources.Load("ClickUp");
            audio1.PlayOneShot(ClickClipUp, 1.0f);
            finalTouchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            CalcAngle();
        }

    }

    void CalcAngle()
    {
        if(Mathf.Abs(finalTouchPos.y - firstTouchPos.y) > swipeResist || Mathf.Abs(finalTouchPos.x - firstTouchPos.x) > swipeResist)
        swipeAngle = Mathf.Atan2(finalTouchPos.y - firstTouchPos.y, finalTouchPos.x - firstTouchPos.x) * 180 / Mathf.PI;
        Move();
    }

    void Move() //Udfører skiftet.
    {
        if(swipeAngle > -45 && swipeAngle <= 45 && column < board.width - 1)
        {
            //Højre
            otherCookie = board.allCookies[column + 1, row];
            otherCookie.GetComponent<Cookie>().column -= 1;
            column += 1;
            board.turnsLeft -= 1;
        } 
        else if (swipeAngle > 45 && swipeAngle <= 135 && row < board.height - 1)
        {
            //Op
            otherCookie = board.allCookies[column , row + 1];
            otherCookie.GetComponent<Cookie>().row -= 1;
            row += 1;
            board.turnsLeft -= 1;
        } 
        else if ((swipeAngle > 135 || swipeAngle <= -135) && column > 0)
        {
            //Venstre
            otherCookie = board.allCookies[column - 1, row];
            otherCookie.GetComponent<Cookie>().column += 1;
            column -= 1;
            board.turnsLeft -= 1;
        } 
        else if (swipeAngle < -45 && swipeAngle >= -135 && row > 0)
        {
            //Ned
            otherCookie = board.allCookies[column , row - 1];
            otherCookie.GetComponent<Cookie>().row += 1;
            row -= 1;
            board.turnsLeft -= 1;
        }
        StartCoroutine(CheckMove());
    }

    public IEnumerator CheckMove() //Undo hvis de ikke matcher
    {
        yield return new WaitForSeconds(0.5f);
        if (otherCookie != null)
        {
            if(!isMatched && !otherCookie.GetComponent<Cookie>().isMatched)
            {
                otherCookie.GetComponent<Cookie>().row = row;
                otherCookie.GetComponent<Cookie>().column = column;
                row = preRow;
                column = preColumn;

            } else
            {
                board.DestroyCookies();
            }
        otherCookie = null;
        }
    }

    public void FindMatch() //Her finder vi de matchende cookies
    {
        if (column > 0 && column < board.width - 1)
        {
            GameObject leftCookie1 = board.allCookies[column - 1, row];
            GameObject rightCookie1 = board.allCookies[column + 1, row];
            if(leftCookie1 != null && rightCookie1 != null)
            {
                if (leftCookie1.tag == this.gameObject.tag && rightCookie1.tag == this.gameObject.tag)
                {
                    leftCookie1.GetComponent<Cookie>().isMatched = true;
                    rightCookie1.GetComponent<Cookie>().isMatched = true;
                    isMatched = true;
                }
            }
        }

        if (row > 0 && row < board.height - 1)
        {
            GameObject upperCookie1 = board.allCookies[column, row + 1];
            GameObject lowerCookie1 = board.allCookies[column, row - 1];
            if (upperCookie1 != null && lowerCookie1 != null)
            {
                if (upperCookie1.tag == this.gameObject.tag && lowerCookie1.tag == this.gameObject.tag)
                {
                    upperCookie1.GetComponent<Cookie>().isMatched = true;
                    lowerCookie1.GetComponent<Cookie>().isMatched = true;
                    isMatched = true;
                }
            }

        }
    }

}

