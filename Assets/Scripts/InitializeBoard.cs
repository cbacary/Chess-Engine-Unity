using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;

public class InitializeBoard : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField]
    private Color blackColor;
    [SerializeField]
    private Color whiteColor;

    [SerializeField]
    private GameObject square;

    [SerializeField]
    private string fenPerm;

    [SerializeField]
    private Transform canvas;

    [SerializeField]
    private GameObject piece;

    void Start()
    {
        fenPerm = fenPerm.Replace("/", "");
        float originalX = -10f;
        float originaly = 10f;
        bool black = false;
        float size = square.transform.localScale.x / square.GetComponent<SpriteRenderer>().sprite.bounds.size.x;
        float xPos = originalX;
        float yPos = originaly;
        Color colorForSquare = whiteColor;
        string fen = "" + fenPerm;
        int fenCount = 0;
        int count = 0;
        // For some reason I have to initialize, color, and render each thing separetly.
        // Initialize each square
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                PieceE pieceEnum;
                bool noPiece = false;
                bool noFenSub = false;
                if (Char.IsDigit(fen[fenCount]) && fen[fenCount] - '0' > 0)
                {
                    int f = fen[fenCount] - '0';
                    f--;
                    string strF = "" + f;
                    fen = fen.Substring(0, fenCount) + strF + fen.Substring(fenCount + 1, fen.Length - (fenCount + 1));
                    if (f <= 0) { fenCount++; }
                    noPiece = true;
                    noFenSub = true;
                }

                GameObject temp = Instantiate(square, new Vector2(xPos, yPos), Quaternion.identity, transform);
                temp.GetComponent<Square>().SetPosition((new string((char)(j + 97), 1) + "" + (7 - i + 1)));
                temp.GetComponent<Square>().index = count;
                if (!noPiece)
                {
                    char p = fen[fenCount];
                    if (Char.IsLower(p))
                    {
                        pieceEnum = PieceE.Black;
                    }
                    else
                    {
                        pieceEnum = PieceE.White;
                    }
                    p = Char.ToUpper(p);
                    if (p == 'P')
                        pieceEnum |= PieceE.Pawn;
                    else if (p == 'R')
                        pieceEnum |= PieceE.Rook;
                    else if (p == 'N')
                        pieceEnum |= PieceE.Knight;
                    else if (p == 'B')
                        pieceEnum |= PieceE.Bishop;
                    else if (p == 'Q')
                        pieceEnum |= PieceE.Queen;
                    else if (p == 'K')
                        pieceEnum |= PieceE.King;
                }
                else
                {
                    pieceEnum = PieceE.None;
                }
                temp.GetComponent<Square>().piece = pieceEnum;
                GameManager.instance.board.squaresList.Add(temp.GetComponent<Square>());
                xPos += size;
                if (!noFenSub)
                    fenCount++;
                count++;
            }

            xPos = originalX;
            yPos -= size;
            black = !black;
        }
        GameManager.instance.board.ConvertSquareListToArray();
        count = 0;
        // Color each square
        foreach (SpriteRenderer renderer in GetComponentsInChildren<SpriteRenderer>())
        {
            if (black)
                colorForSquare = blackColor;
            else
                colorForSquare = whiteColor;
            black = !black;
            renderer.color = new Color(colorForSquare.r, colorForSquare.g, colorForSquare.b);
            count++;
            if (count % 8 == 0) black = !black;
        }
        // Render piece images
        xPos = originalX;
        yPos = originaly;
        fen = "" + fenPerm;
        fenCount = 0;
        int pieceCount = 0;
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                PieceE pieceEnum;
                if (Char.IsDigit(fen[fenCount]) && fen[fenCount] - '0' > 0)
                {
                    int f = fen[fenCount] - '0';
                    f--;
                    string strF = "" + f;
                    fen = fen.Substring(0, fenCount) + strF + fen.Substring(fenCount + 1, fen.Length - (fenCount + 1));
                    if (f <= 0) { fenCount++; }
                    xPos += size;
                    pieceCount++;
                    continue;
                }
                // Debug.Log(fen[fenCount]);
                byte[] bytes = File.ReadAllBytes("./Assets/Resources/Pieces/" + (Char.IsUpper(fen[fenCount]) ? "White Pieces/" : "Black Pieces/") + fen[fenCount] + ".png");
                Texture2D texture = new Texture2D((int)size, (int)size, TextureFormat.RGB24, false);
                texture.filterMode = FilterMode.Trilinear;
                texture.LoadImage(bytes);
                texture.Apply();
                // Sprite sprite = Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0, 0), 1.0f);

                GameObject pieceSprite = Instantiate(piece, new Vector3(xPos, yPos, -1f), Quaternion.identity, canvas);
                pieceSprite.GetComponent<Piece>().piece = "" + fen[fenCount];
                char p = fen[fenCount];
                if (Char.IsLower(p))
                {
                    pieceEnum = PieceE.Black;
                }
                else
                {
                    pieceEnum = PieceE.White;
                }
                p = Char.ToUpper(p);
                if (p == 'P')
                    pieceEnum |= PieceE.Pawn;
                else if (p == 'R')
                    pieceEnum |= PieceE.Rook;
                else if (p == 'N')
                    pieceEnum |= PieceE.Knight;
                else if (p == 'B')
                    pieceEnum |= PieceE.Bishop;
                else if (p == 'Q')
                    pieceEnum |= PieceE.Queen;
                else if (p == 'K')
                    pieceEnum |= PieceE.King;
                pieceSprite.GetComponent<Piece>().pieceE = pieceEnum;
                pieceSprite.GetComponent<Piece>().currentIndex = pieceCount;
                GameManager.instance.board.squaresList[pieceCount].pieceObj = pieceSprite;
                pieceSprite.GetComponent<UnityEngine.UI.RawImage>().texture = texture;
                fenCount++;
                xPos += size;
                pieceCount++;
            }
            xPos = originalX;
            yPos -= size;
        }

    }
}
