using System;
using System.Collections.Generic;
using System.Text;
using System.Linq; 


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

    // Draw Conditions
    public List<string> PositionHistory { get; } = new List<string>();
    public int FiftyMoveCounter { get; private set; }



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
        int targetPiece = Board[move.ToX, move.ToY];

        // ======== อัปเดตกฎ 50 การเดิน ========
        if (Math.Abs(piece) == 1 || targetPiece != 0)
            FiftyMoveCounter = 0;
        else
            FiftyMoveCounter++;

        // ======== บันทึกประวัติกระดาน ========
        string currentPosition = SerializeBoard();
        PositionHistory.Add(currentPosition);

        // ======== อัปเดตสถานะ King และ Rook เมื่อเคลื่อนที่ ========
        if (Math.Abs(piece) == 6) // King เคลื่อนที่
        {
            if (piece > 0) WhiteKingMoved = true;
            else BlackKingMoved = true;
        }
        else if (Math.Abs(piece) == 4) // Rook เคลื่อนที่
        {
            // ตรวจสอบตำแหน่งเริ่มต้นของ Rook
            if (piece > 0) // White Rook
            {
                if (move.FromX == 7 && move.FromY == 7)
                    WhiteRookKingSideMoved = true;
                else if (move.FromX == 7 && move.FromY == 0)
                    WhiteRookQueenSideMoved = true;
            }
            else // Black Rook
            {
                if (move.FromX == 0 && move.FromY == 7)
                    BlackRookKingSideMoved = true;
                else if (move.FromX == 0 && move.FromY == 0)
                    BlackRookQueenSideMoved = true;
            }
        }

        // ======== จัดการการเดินพิเศษ ========
        UpdateEnPassantTarget(move, piece);
        HandleCastling(move, piece); // ตรวจสอบการ Castling
        HandleEnPassantCapture(move, piece);

        // ======== ย้ายหมาก ========
        Board[move.ToX, move.ToY] = piece;
        Board[move.FromX, move.FromY] = 0;

        // ======== การ Promote เบี้ย ========
        if (move.PromotionPiece != 0)
            Board[move.ToX, move.ToY] = move.PromotionPiece;

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


        // ตรวจสอบให้แน่ใจว่า King อยู่ในกระดาน
        if (newBoard.Board.Cast<int>().All(p => Math.Abs(p) != 6))
            throw new InvalidOperationException("Invalid board state: King is missing!");


        return newBoard;
    }

    // เพิ่มเมธอด Clone พิเศษสำหรับการตรวจสอบการโจมตี
    public ChessBoard CloneWithTurn(bool isWhiteTurn)
    {
        ChessBoard clone = this.Clone();
        clone.IsWhiteTurn = isWhiteTurn; // อนุญาตให้ตั้งค่าในคลาสตัวเอง
        return clone;
    }

    // ======== Check Game Over ========
    public bool IsGameOver()
    {
        return IsCheckmate() || IsDraw();
    }

    public bool IsCheckmate()
    {
        return IsInCheck(IsWhiteTurn) && MoveGenerator.GenerateMoves(this).Count == 0;
    }

    // ======== Check Draw ========
    public bool IsDraw()
    {
        // Stalemate
        if (!IsInCheck(IsWhiteTurn) && MoveGenerator.GenerateMoves(this).Count == 0)
            return true;

        // Threefold Repetition
        string currentPos = SerializeBoard();
        if (PositionHistory.FindAll(p => p == currentPos).Count >= 3)
            return true;

        // Fifty Move Rule
        if (FiftyMoveCounter >= 100)
            return true;

        // Insufficient Material
        return HasInsufficientMaterial();
    }

    // ======== Check King Safety ========
    public bool IsInCheck(bool isWhite)
    {
        int kingValue = isWhite ? 6 : -6;
        Square kingPos = FindKingPosition(kingValue);

        // หากไม่พบ King ให้รีเทิร์น false เพื่อหลีกเลี่ยง Exception
        if (kingPos.X == -1 || kingPos.Y == -1)
            return false;

        return IsSquareUnderAttack(kingPos, !isWhite);
    }

    // ======== Helper Methods ========
    private bool HasInsufficientMaterial()
    {
        int totalPieces = 0;
        bool hasPawnOrMajorPiece = false;

        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                int piece = Math.Abs(Board[x, y]);
                if (piece == 0) continue;

                totalPieces++;
                if (piece == 1 || piece == 4 || piece == 5)
                    hasPawnOrMajorPiece = true;
            }
        }

        return totalPieces <= 4 && !hasPawnOrMajorPiece;
    }

    private Square FindKingPosition(int kingValue)
    {
        if(kingValue != 6 && kingValue != -6)
            throw new ArgumentException("Invalid king value!");
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

    // ======== Special Move Handlers ========
    private void HandleCastling(Move move, int piece)
    {
        if (Math.Abs(piece) == 6 && Math.Abs(move.FromY - move.ToY) == 2)
        {
            bool isWhite = piece > 0;
            bool isValid = isWhite ?
                (move.ToY == 6 && !WhiteKingMoved && !WhiteRookKingSideMoved) :
                (move.ToY == 2 && !BlackKingMoved && !BlackRookQueenSideMoved);

            //if (!isValid)
            //{
            //    throw new InvalidOperationException("Invalid castling attempt detected!");
            //}

            // ย้าย Rook
            int rookFromY = move.ToY == 6 ? 7 : 0;
            int rookToY = move.ToY == 6 ? 5 : 3;

            Board[move.FromX, rookToY] = Board[move.FromX, rookFromY];
            Board[move.FromX, rookFromY] = 0;

            // อัปเดตสถานะ
            if (isWhite)
            {
                WhiteKingMoved = true;
                if (rookFromY == 7) WhiteRookKingSideMoved = true;
                if (rookFromY == 0) WhiteRookQueenSideMoved = true;
            }
            else
            {
                BlackKingMoved = true;
                if (rookFromY == 7) BlackRookKingSideMoved = true;
                if (rookFromY == 0) BlackRookQueenSideMoved = true;
            }
        }
    }

    private void UpdateEnPassantTarget(Move move, int piece)
    {
        if (Math.Abs(piece) == 1 && Math.Abs(move.ToX - move.FromX) == 2)
            EnPassantTarget = new Square((move.FromX + move.ToX) / 2, move.FromY);
        else
            EnPassantTarget = null;
    }

    private void HandleEnPassantCapture(Move move, int piece)
    {
        if (Math.Abs(piece) == 1 && EnPassantTarget.HasValue &&
            move.ToX == EnPassantTarget.Value.X && move.ToY == EnPassantTarget.Value.Y)
        {
            int captureX = IsWhiteTurn ? move.ToX + 1 : move.ToX - 1;
            Board[captureX, move.ToY] = 0;
        }
    }

    public bool IsSquareUnderAttack(Square square, bool byWhite)
    {
        try
        {
            int attackerColor = byWhite ? 1 : -1;

            // 1. ตรวจสอบ Knight
            int[,] knightMoves = { { 2, 1 }, { 2, -1 }, { -2, 1 }, { -2, -1 }, { 1, 2 }, { 1, -2 }, { -1, 2 }, { -1, -2 } };
            for (int i = 0; i < knightMoves.GetLength(0); i++)
            {
                int dx = knightMoves[i, 0];
                int dy = knightMoves[i, 1];
                int x = square.X + dx;
                int y = square.Y + dy;
                if (x >= 0 && x < 8 && y >= 0 && y < 8)
                {
                    int piece = Board[x, y];
                    if (piece == 2 * attackerColor) return true;
                }
            }

            // 2. ตรวจสอบ Pawn
            int pawnDir = byWhite ? -1 : 1;
            int[] pawnCaptureY = { square.Y - 1, square.Y + 1 };
            foreach (int y in pawnCaptureY)
            {
                int x = square.X + pawnDir;
                if (x >= 0 && x < 8 && y >= 0 && y < 8)
                {
                    int piece = Board[x, y];
                    if (piece == 1 * attackerColor) return true;
                }
            }

            // 3. ตรวจสอบ King
            int[,] kingMoves = { { 1, 0 }, { -1, 0 }, { 0, 1 }, { 0, -1 }, { 1, 1 }, { 1, -1 }, { -1, 1 }, { -1, -1 } };
            for (int i = 0; i < kingMoves.GetLength(0); i++)
            {
                int dx = kingMoves[i, 0];
                int dy = kingMoves[i, 1];
                int x = square.X + dx;
                int y = square.Y + dy;
                if (x >= 0 && x < 8 && y >= 0 && y < 8)
                {
                    int piece = Board[x, y];
                    if (piece == 6 * attackerColor) return true;
                }
            }

            // 4. ตรวจสอบแนวตรง (Rook/Queen)
            if (CheckLineAttack(square, new int[,] { { 1, 0 }, { -1, 0 }, { 0, 1 }, { 0, -1 } }, attackerColor, new[] { 4, 5 }))
                return true;

            // 5. ตรวจสอบแนวทแยง (Bishop/Queen)
            if (CheckLineAttack(square, new int[,] { { 1, 1 }, { 1, -1 }, { -1, 1 }, { -1, -1 } }, attackerColor, new[] { 3, 5 }))
                return true;

            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in IsSquareUnderAttack: {ex.Message}");
            return false;
        }
    }
    private bool CheckLineAttack(Square square, int[,] directions, int attackerColor, int[] validPieces)
    {
        try
        {
            for (int d = 0; d < directions.GetLength(0); d++)
            {
                int dx = directions[d, 0];
                int dy = directions[d, 1];
                for (int step = 1; step < 8; step++)
                {
                    int x = square.X + dx * step;
                    int y = square.Y + dy * step;
                    if (x < 0 || x >= 8 || y < 0 || y >= 8) break;

                    int piece = Board[x, y];
                    if (piece != 0)
                    {
                        if ((piece * attackerColor > 0) && validPieces.Contains(Math.Abs(piece)))
                            return true;
                        break;
                    }
                }
            }
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in CheckLineAttack: {ex.Message}");
            return false;
        }
    }

    public string SerializeBoard()
    {
        StringBuilder sb = new StringBuilder();
        try
        {
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    sb.Append(Board[x, y]); // เพิ่มค่าหมากในตำแหน่ง [x,y]
                }
                sb.Append(IsWhiteTurn ? '1' : '0'); // เพิ่มสถานะตาเล่น (ต่อแถว)
            }
            return sb.ToString();
        }
        catch (Exception ex) // แก้ไขการสะกด Exception
        {
            Console.WriteLine($"เกิดข้อผิดพลาดในการสร้างบอร์ด: {ex.Message}");
            return ""; // คืนค่าสตริงว่างหากเกิดข้อผิดพลาด
        }
    }

    // ======== Display Board ========
    public void PrintBoard()
    {
        Dictionary<int, string> symbols = new Dictionary<int, string>
        {
            { 1, "♙" }, { -1, "♟" }, { 2, "♘" }, { -2, "♞" },
            { 3, "♗" }, { -3, "♝" }, { 4, "♖" }, { -4, "♜" },
            { 5, "♕" }, { -5, "♛" }, { 6, "♔" }, { -6, "♚" },
            { 0, "·" }
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