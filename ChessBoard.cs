using System;
using System.Collections.Generic;

public class ChessBoard
{
    // ======== Properties ========
    public int[,] Board { get; protected set; }
    public bool IsWhiteTurn { get; protected set; }

    // Castling Flags
    public bool WhiteKingMoved { get; private set; }
    public bool WhiteRookKingSideMoved { get; private set; }
    public bool WhiteRookQueenSideMoved { get; private set; }
    public bool BlackKingMoved { get; private set; }
    public bool BlackRookKingSideMoved { get; private set; }
    public bool BlackRookQueenSideMoved { get; private set; }

    // En Passant
    public Square? EnPassantTarget { get; private set; }

    // ======== Constructor ========
    public ChessBoard()
    {
        Board = new int[8, 8];
        IsWhiteTurn = true;
        InitializeBoard();
    }

    // ======== Initialize Board ========
    private void InitializeBoard()
    {
        // White Pieces (แถว 1 และ 2 ใน chess notation)
        Board[7, 0] = 4; // Rook (a1)
        Board[7, 1] = 2; // Knight (b1)
        Board[7, 2] = 3; // Bishop (c1)
        Board[7, 3] = 5; // Queen (d1)
        Board[7, 4] = 6; // King (e1)
        Board[7, 5] = 3; // Bishop (f1)
        Board[7, 6] = 2; // Knight (g1)
        Board[7, 7] = 4; // Rook (h1)

        // White Pawns (แถว 2 ใน chess notation)
        for (int i = 0; i < 8; i++)
            Board[6, i] = 1;

        // Black Pieces (แถว 8 และ 7 ใน chess notation)
        Board[0, 0] = -4; // Rook (a8)
        Board[0, 1] = -2; // Knight (b8)
        Board[0, 2] = -3; // Bishop (c8)
        Board[0, 3] = -5; // Queen (d8)
        Board[0, 4] = -6; // King (e8)
        Board[0, 5] = -3; // Bishop (f8)
        Board[0, 6] = -2; // Knight (g8)
        Board[0, 7] = -4; // Rook (h8)

        // Black Pawns (แถว 7 ใน chess notation)
        for (int i = 0; i < 8; i++)
            Board[1, i] = -1;
    }

    // ======== Make Move ========
    public void MakeMove(Move move)
    {
        int piece = Board[move.FromX, move.FromY];

        // Handle En Passant
        UpdateEnPassantTarget(move, piece);

        // Handle Castling
        HandleCastling(move, piece);

        // Move the piece
        Board[move.ToX, move.ToY] = piece;
        Board[move.FromX, move.FromY] = 0;

        // Handle Promotion
        if (move.PromotionPiece != 0)
            Board[move.ToX, move.ToY] = move.PromotionPiece;

        // Switch turn
        IsWhiteTurn = !IsWhiteTurn;
    }

    // ======== Clone Board ========
    public ChessBoard Clone()
    {
        ChessBoard newBoard = new ChessBoard();
        newBoard.Board = (int[,])this.Board.Clone(); // Deep copy of the board array
        newBoard.IsWhiteTurn = this.IsWhiteTurn;

        // Copy castling flags
        newBoard.WhiteKingMoved = this.WhiteKingMoved;
        newBoard.WhiteRookKingSideMoved = this.WhiteRookKingSideMoved;
        newBoard.WhiteRookQueenSideMoved = this.WhiteRookQueenSideMoved;
        newBoard.BlackKingMoved = this.BlackKingMoved;
        newBoard.BlackRookKingSideMoved = this.BlackRookKingSideMoved;
        newBoard.BlackRookQueenSideMoved = this.BlackRookQueenSideMoved;

        // Copy En Passant target
        newBoard.EnPassantTarget = this.EnPassantTarget;

        return newBoard;
    }

    // ======== Check Game Over ========
    public bool IsGameOver()
    {
        List<Move> legalMoves = MoveGenerator.GenerateMoves(this);
        return legalMoves.Count == 0;
    }

    // ======== Check King Safety ========
    public bool IsInCheck(bool isWhite)
    {
        int king = isWhite ? 6 : -6;
        Square kingPos = FindKingPosition(king);
        return IsSquareUnderAttack(kingPos, !isWhite);
    }

    // ======== Helper Methods ========
    private Square FindKingPosition(int kingValue)
    {
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                if (Board[x, y] == kingValue)
                    return new Square(x, y);
            }
        }
        throw new Exception("King not found!");
    }

    private bool IsSquareUnderAttack(Square square, bool byWhite)
    {
        ChessBoard tempBoard = this.Clone();
        tempBoard.IsWhiteTurn = byWhite;
        List<Move> attacks = MoveGenerator.GenerateMoves(tempBoard);

        foreach (Move move in attacks)
        {
            if (move.ToX == square.X && move.ToY == square.Y)
                return true;
        }
        return false;
    }

    private void UpdateEnPassantTarget(Move move, int piece)
    {
        if (Math.Abs(piece) == 1 && Math.Abs(move.ToX - move.FromX) == 2)
            EnPassantTarget = new Square((move.FromX + move.ToX) / 2, move.FromY);
        else
            EnPassantTarget = null;
    }

    private void HandleCastling(Move move, int piece)
    {
        if (Math.Abs(piece) == 6) // King move
        {
            if (piece == 6)
                WhiteKingMoved = true;
            else
                BlackKingMoved = true;
        }
        else if (Math.Abs(piece) == 4) // Rook move
        {
            if (move.FromX == 0 && move.FromY == 0)
                WhiteRookQueenSideMoved = true;
            else if (move.FromX == 0 && move.FromY == 7)
                WhiteRookKingSideMoved = true;
            else if (move.FromX == 7 && move.FromY == 0)
                BlackRookQueenSideMoved = true;
            else if (move.FromX == 7 && move.FromY == 7)
                BlackRookKingSideMoved = true;
        }
    }

    // ======== Display Board ========
    public void PrintBoard()
    {
        Dictionary<int, string> symbols = new Dictionary<int, string>
        {
            { 1, "♙" }, { -1, "♟" }, // เบี้ย
            { 2, "♘" }, { -2, "♞" }, // ม้า
            { 3, "♗" }, { -3, "♝" }, // บิชอป
            { 4, "♖" }, { -4, "♜" }, // เรือ
            { 5, "♕" }, { -5, "♛" }, // ควีน
            { 6, "♔" }, { -6, "♚" }, // ราชา
            { 0, "·" }  // ช่องว่าง
        };
        Console.WriteLine("  a  b  c  d  e  f  g  h");
        for (int x = 0; x < 8; x++)
        {
            Console.Write($"{8 - x} ");
            for (int y = 0; y < 8; y++)
                Console.Write($"{symbols[Board[x, y]]} ");
            Console.WriteLine($"{8 - x}");
        }
        Console.WriteLine("  a  b  c  d  e  f  g  h");
        Console.WriteLine($"ตาเล่น: {(IsWhiteTurn ? "ขาว" : "ดำ")}\n");
    }
}



public struct Square
{
    public int X { get; }
    public int Y { get; }

    public Square(int x, int y)
    {
        X = x;
        Y = y;
    }
}