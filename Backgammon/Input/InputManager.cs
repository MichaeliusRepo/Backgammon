using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Backgammon.Screen;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Backgammon.Input
{
    public class InputManager
    {
        KeyboardState currentKeyState, prevKeyState;

        private static InputManager instance;

        public static InputManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new InputManager();
                return instance;
            }
        }

        public void Update()
        {
            prevKeyState = currentKeyState;
            if (!ScreenManager.Instance.IsTransitioning)
                currentKeyState = Keyboard.GetState();
        }

        public bool IsWithinBounds(Rectangle bounds)
        {
            return bounds.Contains(GetMousePosition());
        }

        public Vector2 GetMousePosition()
        {
            var currentMouseState = Mouse.GetState();
            return new Vector2(currentMouseState.X, currentMouseState.Y);
        }

        public bool KeyPressed(params Keys[] keys)
        {
            foreach (Keys key in keys)
                if (currentKeyState.IsKeyDown(key) && prevKeyState.IsKeyUp(key))
                    return true;
            return false;
        }

        public bool KeyUp(params Keys[] keys)
        {
            foreach (Keys key in keys)
                if (currentKeyState.IsKeyUp(key) && prevKeyState.IsKeyDown(key))
                    return true;
            return false;
        }

        public bool KeyDown(params Keys[] keys)
        {
            foreach (Keys key in keys)
                if (currentKeyState.IsKeyDown(key))
                    return true;
            return false;
        }

    }
}
