using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    private static readonly KeyCode[] SUPPORTED_KEYS = new KeyCode[] {
        KeyCode.A, KeyCode.B, KeyCode.C, KeyCode.D, KeyCode.E, KeyCode.F,
        KeyCode.G, KeyCode.H, KeyCode.I, KeyCode.J, KeyCode.K, KeyCode.L,
        KeyCode.M, KeyCode.N, KeyCode.O, KeyCode.P, KeyCode.Q, KeyCode.R,
        KeyCode.S, KeyCode.T, KeyCode.U, KeyCode.V, KeyCode.W, KeyCode.X,
        KeyCode.Y, KeyCode.Z,
    };
    
    private Row[] rows;

    private string[] solutions;
    private string[] validWords;
    private string word;

    private int rowIndex;
    private int columnIndex;

    [Header("States")]
    public Tile.State emptyState;
    public Tile.State occupiedState;
    public Tile.State correctState;
    public Tile.State wrongSpotState;
    public Tile.State incorrectState;

    private void Awake()
    {
        rows = GetComponentsInChildren<Row>();
    }

    private void Start()
    {
        LoadData(); 
        SetRandomWord();    
    }

    private void LoadData()
    {
        TextAsset textFile = Resources.Load("official_wordle_all") as TextAsset;
        validWords = textFile.text.Split("\n");

        textFile = Resources.Load("official_wordle_common") as TextAsset;
        solutions = textFile.text.Split("\n") ;
    }

    private void SetRandomWord()
    {
        word = solutions[Random.Range(0, solutions.Length)];
        word = word.ToLower().Trim();
    }

    // Update is called once per frame
    void Update()
    {
        Row currentRow = rows[rowIndex];

        if(Input.GetKeyDown(KeyCode.Backspace))
        {
            columnIndex = Mathf.Max(columnIndex - 1, 0);
            currentRow.tiles[columnIndex].SetLetter('\0'); //single quotes for character and \0 for null character
            currentRow.tiles[columnIndex].SetState(emptyState);
        }
        else if(columnIndex >= currentRow.tiles.Length)
        {
            if(Input.GetKeyDown(KeyCode.Return))
            {
                SubmitRow(currentRow);
            }
        }
        else
        {
            for(int i = 0;i < SUPPORTED_KEYS.Length;i++)
            {
                if(Input.GetKeyDown(SUPPORTED_KEYS[i]))
                {
                    currentRow.tiles[columnIndex].SetLetter((char)SUPPORTED_KEYS[i]);
                    currentRow.tiles[columnIndex].SetState(occupiedState);
                    columnIndex++;
                    break;
                }
            }
        }    
    }

    private void SubmitRow(Row row)
    {
        string remaining = word;

        for(int i = 0; i < row.tiles.Length; i++)
        {
            Tile tile = row.tiles[i];

            if(tile.letter == word[i])
            {
                tile.SetState(correctState);

                remaining = remaining.Remove(i, 1);
                remaining = remaining.Insert(i, " ");
            }
            else if(!word.Contains(tile.letter))
            {
                tile.SetState(incorrectState);
            }
        }

        for(int i = 0; i < row.tiles.Length; i++)
        {
            Tile tile = row.tiles[i];

            if(tile.state != correctState && tile.state != incorrectState)
            {
                if(remaining.Contains(tile.letter))
                {
                    tile.SetState(wrongSpotState);

                    int index = remaining.IndexOf(tile.letter);
                    remaining = remaining.Remove(index, 1);
                    remaining = remaining.Insert(index, " ");
                }
                else
                {
                    tile.SetState(incorrectState);
                }
            }
            
        }


        /*
        for(int i=0;i < row.tiles.Length;i++)
        {
            Tile tile = row.tiles[i];
            
            if(tile.letter == word[i])
            {
                tile.SetState(correctState);
            }
            else if(word.Contains(tile.letter)) //letter in wrong spot
            {
                tile.SetState(wrongSpotState);
            }
            else //complete incorrect
            {
                tile.SetState(incorrectState);
            }
        }
        */

        rowIndex++;
        columnIndex = 0;

        if(rowIndex >= rows.Length)
        {
            enabled = false;
        }
    }

}
