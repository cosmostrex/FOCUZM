
public static class ChapelCarryOver
{
   
    public static int SelectedSlot = 1;

    public static void SetSlot(int slot1to9)
    {
        if (slot1to9 < 1) slot1to9 = 1;
        if (slot1to9 > 9) slot1to9 = 9;
        SelectedSlot = slot1to9;
    }
}
