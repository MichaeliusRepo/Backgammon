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
    internal class InputManager
    {
        KeyboardState currentKeyState, prevKeyState;
        MouseState currentMouseState, prevMouseState;

        private static InputManager instance;

        internal static InputManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new InputManager();
                return instance;
            }
        }

        private InputManager() { } // Make constructor private.

        public void Update()
        {
            prevKeyState = currentKeyState;
            prevMouseState = currentMouseState;
            if (!ScreenManager.Instance.IsTransitioning)
            {
                currentKeyState = Keyboard.GetState();
                currentMouseState = Mouse.GetState();
            }
        }

        internal bool IsWithinBounds(Rectangle bounds)
        {
            return bounds.Contains(GetMousePosition());
        }

        internal bool WasClicked(Rectangle bounds)
        {
            return MouseLeftPressed() && IsWithinBounds(bounds);
        }

        private Point GetMousePosition()
        {
            return currentMouseState.Position;
        }

        internal bool MouseLeftPressed()
        {
            return (currentMouseState.LeftButton == ButtonState.Released && prevMouseState.LeftButton == ButtonState.Pressed);
        }

        internal bool KeyPressed(params Keys[] keys)
        {
            foreach (Keys key in keys)
                if (currentKeyState.IsKeyDown(key) && prevKeyState.IsKeyUp(key))
                    return true;
            return false;
        }

        internal bool KeyUp(params Keys[] keys)
        {
            foreach (Keys key in keys)
                if (currentKeyState.IsKeyUp(key) && prevKeyState.IsKeyDown(key))
                    return true;
            return false;
        }

        internal bool KeyDown(params Keys[] keys)
        {
            foreach (Keys key in keys)
                if (currentKeyState.IsKeyDown(key))
                    return true;
            return false;
        }

    }
}
