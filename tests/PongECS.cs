using Szark.ECS;
using Szark.Graphics;
using Szark.Input;
using Szark.Math;
using Szark;

namespace Example
{
    /// <summary>
    /// This is a generic representation of 
    /// the padding and the ball.
    /// </summary>
    public struct Quad : IComponent
    {
        public Vec2 Position;
        public Vec2 Size;
    }

    /// <summary>
    /// This is the velocity of the ball.
    /// </summary>
    public struct Velocity : IComponent
    {
        public Vec2 Value;
    }

    // This will be attached to the player paddle.
    public struct PlayerTag : ITag { }

    // This will be attached to the enemy paddle.
    public struct EnemyTag : ITag { }

    /// <summary>
    /// This renders a square on the screen for 
    /// entities that have a Quad component.
    /// </summary>
    public class QuadRenderer : ComponentSystem<Pong>
    {
        public override void Execute(Canvas canvas, float deltaTime)
        {
            Entities.Query().ForEach((ref Quad quad) =>
            {
                canvas.FillRectangle(
                    quad.Position,
                    (int)quad.Size.X,
                    (int)quad.Size.Y,
                    Color.White
                );
            });
        }
    }

    /// <summary>
    /// This moves the player entity with keyboard input.
    /// </summary>
    public class PlayerMover : ComponentSystem<Pong>
    {
        public override void Execute(Canvas canvas, float deltaTime)
        {
            var keyboard = Game.Keyboard;
            var yInput = keyboard[Key.S, Input.Hold] ? 1 :
                keyboard[Key.W, Input.Hold] ? -1 : 0;

            var maxHeight = Game.ScreenHeight;

            Entities
                .Query()
                .WithTag<PlayerTag>()
                .ForEach((ref Quad quad) =>
            {
                quad.Position = new Vec2()
                {
                    X = quad.Position.X,
                    Y = Mathf.Clamp(quad.Position.Y + yInput, 0,
                        maxHeight - quad.Size.Y)
                };
            });
        }
    }

    /// <summary>
    /// This moves the enemy paddle toward the ball.
    /// </summary>
    public class EnemyAI : ComponentSystem<Pong>
    {
        private float speed = 8.0f;

        public override void Execute(Canvas canvas, float deltaTime)
        {
            var maxHeight = Game.ScreenHeight;

            // We get the Quad from the ball and the player.
            var playerQuad = GetComponent<Quad>(Game.player);
            var ballQuad = GetComponent<Quad>(Game.ball);

            if (playerQuad == null || ballQuad == null) return;

            // We extract the position from both
            var playerPos = playerQuad.Value.Position;
            var ballPos = ballQuad.Value.Position;

            Entities
                .Query()
                .WithTag<EnemyTag>()
                .ForEach((ref Quad quad) =>
            {
                // We linear interpolate the enemy's position to the average of
                // the player and ball's position.
                quad.Position = new Vec2()
                {
                    X = quad.Position.X,
                    Y = Mathf.Lerp(
                            quad.Position.Y,
                            (playerPos.Y + ballPos.Y) * 0.5f,
                            speed * deltaTime
                        ),
                };
            });
        }
    }

    /// <summary>
    /// This checks whether the ball has collided with something.
    /// If so, then the ball will change trajectory.
    /// </summary>
    public class ColliderSystem : ComponentSystem<Pong>
    {
        public override void Execute(Canvas canvas, float deltaTime)
        {
            Entities.Query().ForEach((ref Velocity vel, ref Quad quad) =>
            {
                var (X, Y) = quad.Position;
                var width = quad.Size.X;
                var height = quad.Size.Y;
                var newVel = vel.Value;

                // Bounce off ceiling
                if (Y < 0 || Y + height > Game.ScreenHeight)
                    newVel = new Vec2(vel.Value.X, -vel.Value.Y);

                // Reset ball if it goes past goals
                if (X + width > Game.ScreenWidth || X < 0)
                {
                    quad.Position = new(Game.ScreenWidth * 0.5f,
                        Game.ScreenHeight * 0.5f);
                    newVel *= -1;
                }

                // Check for collision with Paddles
                Entities.Query().ForEach((ref Quad other) =>
                {
                    if (X != other.Position.X && Y != other.Position.Y)
                    {
                        if (X + width >= other.Position.X &&
                            X < other.Position.X + other.Size.X &&
                            Y + height >= other.Position.Y &&
                            Y < other.Position.Y + other.Size.Y)
                        {
                            newVel.X *= -1;
                        }
                    }

                });

                vel.Value = newVel;
            });
        }
    }

    /// <summary>
    /// This is in charge of simply moving the ball with a velocity.
    /// </summary>
    public class BallMover : ComponentSystem<Pong>
    {
        public override void Execute(Canvas canvas, float deltaTime)
        {
            Entities.Query().ForEach((ref Quad quad, ref Velocity vel) =>
            {
                quad.Position += vel.Value;
            });
        }
    }

    public class Pong : Game
    {
        public Pong() : base("Pong ECS", 1280, 720, 4, false) { }

        public Entity player, enemy, ball;

        protected override void OnCreated()
        {
            float centerY = ScreenHeight * 0.5f;

            // Setup the Player Paddle
            {
                player = EntityManager.CreateEntity();
                EntityManager.AddTag(player, new PlayerTag());

                EntityManager.AddComponent(player, new Quad()
                {
                    Position = new Vec2(8, centerY - 16),
                    Size = new Vec2(8, 32),
                });
            }

            // Setup the Enemy Paddle
            {
                enemy = EntityManager.CreateEntity();
                EntityManager.AddTag(enemy, new EnemyTag());

                EntityManager.AddComponent(enemy, new Quad()
                {
                    Position = new Vec2(ScreenWidth - 16, centerY - 16),
                    Size = new Vec2(8, 32),
                });
            }

            // Setup the Ball
            {
                ball = EntityManager.CreateEntity();

                EntityManager.AddComponent(ball, new Velocity()
                {
                    Value = new Vec2(-1, 1)
                });

                EntityManager.AddComponent(ball, new Quad()
                {
                    Position = new Vec2((ScreenWidth / 2f) - 4, centerY - 4),
                    Size = new Vec2(8, 8),
                });
            }
        }

        protected override void OnRender(Canvas canvas, float deltaTime)
        {
            // Clear the Screen to Black
            canvas.FillRectangle(0, 0, ScreenWidth, ScreenHeight, Color.Black);
        }
    }
}