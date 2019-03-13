namespace Gameplay
{
    public class Reverse : Bonus
    {
        protected override void BonusEffect()
        {
            Settings.Instance.Snake.Translate();
            Settings.Instance.InputActions.ReverseChangeDirection();
            Settings.Instance.Snake.MoveReverse();
        }
    }
}
