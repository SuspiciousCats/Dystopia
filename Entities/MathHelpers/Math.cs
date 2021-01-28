using Godot;

namespace Dystopia.Entities.MathHelpers
{
    public class Math
    {

        static public float FInterpTo(float Current, float Target, float DeltaTime, float InterpSpeed)
        {
            if (InterpSpeed <= 0)
            {
                return Target;
            }

            // Distance to reach
            float Dist = Target - Current;

            // If distance is too small, just set the desired location
            if ((Dist * Dist) < 0.01)
            {
                return Target;
            }

            // Delta Move, Clamp so we do not over shoot.

            float DeltaMove = Dist * Mathf.Clamp(DeltaTime * InterpSpeed, 0, 1);

            return Current + DeltaMove;
        }
    }
}