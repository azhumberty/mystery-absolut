using Godot;

namespace Game.World;

public partial class DungeonGenerator : Node2D
{
    [Export] public int NumRooms = 5;
    [Export] public PackedScene EnemyScene;
    [Export] public PackedScene BossScene;

    public override void _Ready()
    {
        // Simple procedural linear generation for prototype
        Vector2 currentPos = Vector2.Zero;
        for (int i = 0; i < NumRooms; i++)
        {
            var room = new Polygon2D();
            room.Color = new Color(0.2f, 0.2f, 0.3f);
            room.Polygon = new Vector2[]
            {
                new Vector2(-300, -300), new Vector2(300, -300), 
                new Vector2(300, 300), new Vector2(-300, 300)
            };
            room.Position = currentPos;
            AddChild(room);

            if (i > 0)
            {
                // Connect with previous room
                var corridor = new Polygon2D();
                corridor.Color = new Color(0.15f, 0.15f, 0.25f);
                corridor.Polygon = new Vector2[]
                {
                    new Vector2(-50, -300), new Vector2(50, -300), 
                    new Vector2(50, 300), new Vector2(-50, 300)
                };
                corridor.Position = currentPos - new Vector2(400, 0); // Assuming horizontal connection
                // But rooms are spaced by 800, so corridor needs logic, let's keep it simple:
            }

            if (i == NumRooms - 1)
            {
                if (BossScene != null)
                {
                    var boss = BossScene.Instantiate<Node2D>();
                    boss.Position = currentPos;
                    AddChild(boss);
                }
            }
            else if (i > 0)
            {
                if (EnemyScene != null)
                {
                    var enemy = EnemyScene.Instantiate<Node2D>();
                    enemy.Position = currentPos;
                    AddChild(enemy);
                }
            }

            currentPos += new Vector2(800, 0);
        }
    }
}
