using System;
using System.Net.Mime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Pong;

public class Colligion
{
    public static float HorrizontalGetDirection(KeyboardState keyboard, Keys left, Keys right)
    {
        if (keyboard.IsKeyDown(left))
        {
            return -1;
        }

        else if (keyboard.IsKeyDown(right))
        {
            return 1;
        }

        return 0;
    }
}