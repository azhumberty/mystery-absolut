using Godot;

namespace Game.Combat;

public partial class Stats : Node
{
    [Signal] public delegate void LevelUpEventHandler(int newLevel);
    [Signal] public delegate void XpGainedEventHandler(float currentXp, float requiredXp);

    [Export] public int Level { get; private set; } = 1;
    [Export] public float Experience { get; private set; } = 0f;
    [Export] public int SkillPoints { get; set; } = 0;

    // Base Attributes
    [Export] public int Strength { get; set; } = 10;
    [Export] public int Dexterity { get; set; } = 10;
    [Export] public int Intelligence { get; set; } = 10;

    public float ExperienceRequired => Level * 100f; // Simple curve

    public void AddXP(float amount)
    {
        Experience += amount;
        EmitSignal(SignalName.XpGained, Experience, ExperienceRequired);

        while (Experience >= ExperienceRequired)
        {
            Experience -= ExperienceRequired;
            Level++;
            SkillPoints++;
            EmitSignal(SignalName.LevelUp, Level);
        }
    }
}
