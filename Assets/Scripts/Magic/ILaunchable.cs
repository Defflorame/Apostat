namespace Game.Magic
{
    /// <summary>
    /// То, что можно подбросить вверх заклинанием (JumpEffectData). Интерфейс нужен, чтобы
    /// Magic не зависела от PlayerMovement: Player уже знает про Magic, а не наоборот.
    /// </summary>
    public interface ILaunchable
    {
        void Launch(float height);
    }
}
