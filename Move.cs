public class Move
{
    // ======== Properties ========
    public int FromX { get; }      // ตำแหน่งแถวเริ่มต้น
    public int FromY { get; }      // ตำแหน่งคอลัมน์เริ่มต้น
    public int ToX { get; }        // ตำแหน่งแถวปลายทาง
    public int ToY { get; }        // ตำแหน่งคอลัมน์ปลายทาง
    public int PromotionPiece { get; set; } // ชนิดหมากเมื่อ Promote (0 = ไม่ Promote)

    // ======== Constructor ========
    public Move(int fromX, int fromY, int toX, int toY, int promotionPiece = 0)
    {
        FromX = fromX;
        FromY = fromY;
        ToX = toX;
        ToY = toY;
        PromotionPiece = promotionPiece;
    }

    // ======== Override ToString() ========
    public override string ToString()
    {
        return $"({FromX}, {FromY}) -> ({ToX}, {ToY})" +
               (PromotionPiece != 0 ? $" [Promote to {PromotionPiece}]" : "");
    }
}