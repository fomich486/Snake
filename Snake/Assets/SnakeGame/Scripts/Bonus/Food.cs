namespace Gameplay
{
    public class Food : Bonus
    {
        protected override void BonusEffect()
        {
            Settings.Instance.Snake.AddNewElement();
        }
    }
}
