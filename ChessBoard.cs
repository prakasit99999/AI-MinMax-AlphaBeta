using System;
using System.Collections.Generic;

public class ChessBoard
{
    // ======== Properties ========
    public int[,] Board { get; protected set; }
    public bool IsWhiteTurn { get; protected set; } // ตั้งค่า setter เป็น protected


    // ข้อมูลสำหรับการ Castling
    public bool WhiteKingMoved { get; private set; }
    public bool WhiteRookKingSideMoved { get; private set; }
    public bool WhiteRookQueenSideMoved { get; private set; }
    public bool BlackKingMoved { get; private set; }
    public bool BlackRookKingSideMoved { get; private set; }
    public bool BlackRookQueenSideMoved { get; private set; }

    // ข้อมูลสำหรับ En Passant
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
        // White Pieces
        Board[0, 0] = 4; // Rook
        Board[0, 1] = 2; // Knight
        Board[0, 2] = 3; // Bishop
        Board[0, 3] = 5; // Queen
        Board[0, 4] = 6; // King
        Board[0, 5] = 3; // Bishop
        Board[0, 6] = 2; // Knight
        Board[0, 7] = 4; // Rook

        // White Pawns
        for (int i = 0; i < 8; i++)
            Board[1, i] = 1;

        // Black Pieces
        Board[7, 0] = -4; // Rook
        Board[7, 1] = -2; // Knight
        Board[7, 2] = -3; // Bishop
        Board[7, 3] = -5; // Queen
        Board[7, 4] = -6; // King
        Board[7, 5] = -3; // Bishop
        Board[7, 6] = -2; // Knight
        Board[7, 7] = -4; // Rook

        // Black Pawns
        for (int i = 0; i < 8; i++)
            Board[6, i] = -1;
    }

    // ======== Make Move ========
    public void MakeMove(Move move)
    {
        int piece = Board[move.FromX, move.FromY];

        // บันทึกข้อมูล En Passant ก่อนทำการเดิน
        UpdateEnPassantTarget(move, piece);

        // เคลื่อนย้ายหมาก
        Board[move.ToX, move.ToY] = piece;
        Board[move.FromX, move.FromY] = 0;

        // อัพเดทสถานะ Castling
        UpdateCastlingRights(move, piece);

        // เปลี่ยนตาเล่น
        IsWhiteTurn = !IsWhiteTurn;
    }

    // ======== Clone Board ========
    public ChessBoard Clone()
    {
        ChessBoard newBoard = new ChessBoard();
        newBoard.Board = (int[,])Board.Clone();
        newBoard.IsWhiteTurn = IsWhiteTurn;
        newBoard.WhiteKingMoved = WhiteKingMoved;
        newBoard.WhiteRookKingSideMoved = WhiteRookKingSideMoved;
        newBoard.WhiteRookQueenSideMoved = WhiteRookQueenSideMoved;
        newBoard.BlackKingMoved = BlackKingMoved;
        newBoard.BlackRookKingSideMoved = BlackRookKingSideMoved;
        newBoard.BlackRookQueenSideMoved = BlackRookQueenSideMoved;
        newBoard.EnPassantTarget = EnPassantTarget;
        return newBoard;
    }

    // ======== Check Game Over ========
    public bool IsGameOver()
    {
        // ตรวจสอบการ Checkmate หรือ Stalemate
        List<Move> legalMoves = MoveGenerator.GenerateMoves(this);
        return legalMoves.Count == 0;
    }

    // ======== Check King Safety ========
    public bool IsInCheck(bool isWhite)
    {
        // หาตำแหน่ง King
        int king = isWhite ? 6 : -6;
        Square kingPos = FindKingPosition(king);

        // ตรวจสอบว่าตำแหน่ง King ถูกโจมตีหรือไม่
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
        // สร้างกระดานจำลองเพื่อตรวจสอบการโจมตี
        ChessBoard tempBoard = Clone();
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
        // เบี้ยเดิน 2 ช่อง
        if (Math.Abs(piece) == 1 && Math.Abs(move.ToX - move.FromX) == 2)
            EnPassantTarget = new Square((move.FromX + move.ToX) / 2, move.FromY);
        else
            EnPassantTarget = null;
    }

    private void UpdateCastlingRights(Move move, int piece)
    {
        int fromX = move.FromX;
        int fromY = move.FromY;

        // ตรวจสอบการเคลื่อนของ King หรือ Rook
        if (Math.Abs(piece) == 6) // King
        {
            if (piece == 6) // White King
                WhiteKingMoved = true;
            else // Black King
                BlackKingMoved = true;
        }
        else if (Math.Abs(piece) == 4) // Rook
        {
            if (fromX == 0 && fromY == 0) // White Queenside Rook
                WhiteRookQueenSideMoved = true;
            else if (fromX == 0 && fromY == 7) // White Kingside Rook
                WhiteRookKingSideMoved = true;
            else if (fromX == 7 && fromY == 0) // Black Queenside Rook
                BlackRookQueenSideMoved = true;
            else if (fromX == 7 && fromY == 7) // Black Kingside Rook
                BlackRookKingSideMoved = true;
        }
    }

    public void PrintBoard()
    {
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                Console.Write($"{Board[x, y],3}");
            }
            Console.WriteLine();
        }
        Console.WriteLine($"Turn: {(IsWhiteTurn ? "White" : "Black")}\n");
    }
}

// คลาส Square สำหรับเก็บตำแหน่ง X, Y
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
