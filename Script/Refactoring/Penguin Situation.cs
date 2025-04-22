///Penguin Logical Value
public static class PenguinSituation
{
    //Player contorller Logical Value
    public static bool isPenguinController;     //Æë±ÏÀÇ Á¶ÀÛ °ü¸®
    public static bool isMoveing;
    public static bool isJump;
    public static bool isSlide;                 
    public static bool isSwiming;
    public static bool isAction;
    public static bool isLife;

    //Equipment Logical Value
    public static bool isPlayFishing;
    public static bool isMining;
    public static bool isAttack;
    public enum isGround
    {
        defualt = 0,
        Snow,
        Water
    }
}

public class CharacterMeshId
{
    public const string rockhopper = nameof(rockhopper);
    public const string emperor = nameof(emperor);
    public const string magellanic = nameof(magellanic);
    public const string gentoo = nameof(gentoo);
    public const string king = nameof(king);
    public const string adelie = nameof(adelie);
}

public class AnimationId
{
    public const string aniIsAxe = nameof(aniIsAxe);
    public const string aniIsPunch = nameof(aniIsPunch);
    public const string aniIsSword = nameof(aniIsSword);
    public const string aniIsSpoon = nameof(aniIsSpoon);
    public const string aniIsMining = nameof(aniIsMining);
    public const string aniIsFarming = nameof(aniIsFarming);
    public const string aniIsSlingshot = nameof(aniIsSlingshot);
    public const string aniReSpawn = nameof(aniReSpawn);
    public const string aniIsFishing = nameof(aniIsFishing);
    public const string aniIsEndFishing = nameof(aniIsEndFishing);

    public const string aniIsMove = nameof(aniIsMove);
    public const string aniJump_01 = nameof(aniJump_01);
    public const string aniJump_02 = nameof(aniJump_02);
    public const string aniIsSliding = nameof(aniIsSliding);
    public const string aniIsSwimming = nameof(aniIsSwimming);
    public const string aniIsDeath = nameof(aniIsDeath);
}