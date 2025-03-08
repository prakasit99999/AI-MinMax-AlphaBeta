using System.Collections.Generic;

public static class MoveGenerator
{
    public static List<Move> GenerateMoves(ChessBoard board)
    {
        List<Move> moves = new List<Move>();
        int[,] currentBoard = board.Board;

        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                int piece = currentBoard[x, y];
                if (piece == 0) continue; // ช่องว่าง

                // ตรวจสอบว่าเป็นหมากของผู้เล่นปัจจุบัน
                bool isCurrentPlayerPiece = (board.IsWhiteTurn && piece > 0) || (!board.IsWhiteTurn && piece < 0);
                if (!isCurrentPlayerPiece) continue;

                // สร้างการเดินตามประเภทหมาก
                switch (Math.Abs(piece))
                {
                    case 1: // เบี้ย
                        GeneratePawnMoves(board, x, y, moves);
                        break;
                    case 2: // ม้า
                        GenerateKnightMoves(board, x, y, moves);
                        break;
                    case 3: // บิชอป
                        GenerateBishopMoves(board, x, y, moves);
                        break;
                    case 4: // เรือ
                        GenerateRookMoves(board, x, y, moves);
                        break;
                    case 5: // ควีน
                        GenerateQueenMoves(board, x, y, moves);
                        break;
                    case 6: // ราชา
                        GenerateKingMoves(board, x, y, moves);
                        break;
                }
            }
        }
        return moves;
    }

    // ===== ฟังก์ชันสร้างการเดินของหมากแต่ละประเภท =====
    private static void GeneratePawnMoves(ChessBoard board, int x, int y, List<Move> moves)
    {
        int direction = board.IsWhiteTurn ? 1 : -1;
        int startRow = board.IsWhiteTurn ? 1 : 6;

        // เดินหน้า 1 ช่อง
        int newX = x + direction;
        if (newX >= 0 && newX < 8 && board.Board[newX, y] == 0)
        {
            moves.Add(new Move(x, y, newX, y));
            // เดินหน้า 2 ช่อง (เฉพาะเบี้ยที่ยังไม่เคลื่อน)
            if (x == startRow && board.Board[newX + direction, y] == 0)
            {
                moves.Add(new Move(x, y, newX + direction, y));
            }
        }

        // โจมตีทแยง
        int[] captureY = { y - 1, y + 1 };
        foreach (int cy in captureY)
        {
            if (cy < 0 || cy >= 8) continue;
            int targetX = x + direction;
            if (targetX < 0 || targetX >= 8) continue;

            int targetPiece = board.Board[targetX, cy];
            if (targetPiece != 0 && ((board.IsWhiteTurn && targetPiece < 0) || (!board.IsWhiteTurn && targetPiece > 0)))
            {
                moves.Add(new Move(x, y, targetX, cy));
            }
        }
    }

    private static void GenerateKnightMoves(ChessBoard board, int x, int y, List<Move> moves)
    {
        int[,] knightMoves = {
            {2, 1}, {2, -1}, {-2, 1}, {-2, -1},
            {1, 2}, {1, -2}, {-1, 2}, {-1, -2}
        };

        for (int i = 0; i < knightMoves.GetLength(0); i++)
        {
            int newX = x + knightMoves[i, 0];
            int newY = y + knightMoves[i, 1];

            if (newX >= 0 && newX < 8 && newY >= 0 && newY < 8)
            {
                int targetPiece = board.Board[newX, newY];
                if (targetPiece == 0 || (board.IsWhiteTurn ? targetPiece < 0 : targetPiece > 0))
                {
                    moves.Add(new Move(x, y, newX, newY));
                }
            }
        }
    }

    private static void GenerateBishopMoves(ChessBoard board, int x, int y, List<Move> moves)
    {
        int[,] directions = { {1, 1}, {1, -1}, {-1, 1}, {-1, -1} };

        for (int d = 0; d < directions.GetLength(0); d++)
        {
            int dx = directions[d, 0];
            int dy = directions[d, 1];

            for (int step = 1; step < 8; step++)
            {
                int newX = x + dx * step;
                int newY = y + dy * step;

                if (newX < 0 || newX >= 8 || newY < 0 || newY >= 8) break;

                int targetPiece = board.Board[newX, newY];
                if (targetPiece == 0)
                {
                    moves.Add(new Move(x, y, newX, newY));
                }
                else
                {
                    if ((board.IsWhiteTurn && targetPiece < 0) || (!board.IsWhiteTurn && targetPiece > 0))
                    {
                        moves.Add(new Move(x, y, newX, newY));
                    }
                    break; // ถูกบล็อกโดยหมาก
                }
            }
        }
    }

    private static void GenerateRookMoves(ChessBoard board, int x, int y, List<Move> moves)
    {
        int[,] directions = { {1, 0}, {-1, 0}, {0, 1}, {0, -1} };

        for (int d = 0; d < directions.GetLength(0); d++)
        {
            int dx = directions[d, 0];
            int dy = directions[d, 1];

            for (int step = 1; step < 8; step++)
            {
                int newX = x + dx * step;
                int newY = y + dy * step;

                if (newX < 0 || newX >= 8 || newY < 0 || newY >= 8) break;

                int targetPiece = board.Board[newX, newY];
                if (targetPiece == 0)
                {
                    moves.Add(new Move(x, y, newX, newY));
                }
                else
                {
                    if ((board.IsWhiteTurn && targetPiece < 0) || (!board.IsWhiteTurn && targetPiece > 0))
                    {
                        moves.Add(new Move(x, y, newX, newY));
                    }
                    break;
                }
            }
        }
    }

    private static void GenerateQueenMoves(ChessBoard board, int x, int y, List<Move> moves)
    {
        GenerateBishopMoves(board, x, y, moves);
        GenerateRookMoves(board, x, y, moves);
    }

    private static void GenerateKingMoves(ChessBoard board, int x, int y, List<Move> moves)
    {
        int[,] kingMoves = {
            {1, 0}, {-1, 0}, {0, 1}, {0, -1},
            {1, 1}, {1, -1}, {-1, 1}, {-1, -1}
        };

        for (int i = 0; i < kingMoves.GetLength(0); i++)
        {
            int newX = x + kingMoves[i, 0];
            int newY = y + kingMoves[i, 1];

            if (newX >= 0 && newX < 8 && newY >= 0 && newY < 8)
            {
                int targetPiece = board.Board[newX, newY];
                if (targetPiece == 0 || (board.IsWhiteTurn ? targetPiece < 0 : targetPiece > 0))
                {
                    moves.Add(new Move(x, y, newX, newY));
                }
            }
        }
    }
}
