using UnityEngine;

public static class HitAnimationMapper
{
    // Maps a world-space hit direction angle to an animation name
    public static string GetHitAnimation(float angle)
    {
        // Normalize angle to 0-360 range
        angle = (angle + 360) % 360;

        // Front hit (315-45 degrees)
        if (angle > 315 || angle <= 45)
            return "Damage_Front";

        // Right hit (45-135 degrees)
        else if (angle > 45 && angle <= 135)
            return "Damage_Right";

        // Back hit (135-225 degrees)
        else if (angle > 135 && angle <= 225)
            return "Damage_Back";

        // Left hit (225-315 degrees)
        else
            return "Damage_Left";
    }

    // Get the direction vector based on hit animation name (for use in AI)
    public static Vector3 GetDirectionFromAnimation(string animationName)
    {
        switch (animationName)
        {
            case "Damage_Front":
                return Vector3.forward;
            case "Damage_Right":
                return Vector3.right;
            case "Damage_Back":
                return Vector3.back;
            case "Damage_Left":
                return Vector3.left;
            default:
                return Vector3.forward;
        }
    }
}