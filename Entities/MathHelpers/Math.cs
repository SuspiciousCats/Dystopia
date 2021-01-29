using Godot;

namespace Dystopia.Entities.MathHelpers
{
    public class Math
    {

        public static float FInterpTo(float Current, float Target, float DeltaTime, float InterpSpeed)
        {
            if (InterpSpeed <= 0)
            {
                return Target;
            }

            // Distance to reach
            float Dist = Target - Current;

            // If distance is too small, just set the desired location
            if ((Dist * Dist) < 0.1)
            {
                return Target;
            }

            // Delta Move, Clamp so we do not over shoot.

            float DeltaMove = Dist * Mathf.Clamp(DeltaTime * InterpSpeed, 0, 1);

            return Current + DeltaMove;
        }


        public static float FInterpConstantTo(float Current, float Target, float DeltaTime, float InterpSpeed)
        {
            float Dist = Target - Current;

            // If distance is too small, just set the desired location
            if ((Dist * Dist) < 0.1)
            {
                return Target;
            }

            float Step = InterpSpeed * DeltaTime;
            return Current + Mathf.Clamp(Dist, -Step, Step);
        }
    }

}