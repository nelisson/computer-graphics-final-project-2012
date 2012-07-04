namespace FinalProjectCG.Utilities
{
    using System.Collections.Generic;
    using System.Reflection;
    using Microsoft.Xna.Framework.Input;

    public static class GamePadUtilities
    {
        public static List<Buttons> GetPressed(GamePadState state)
        {
            var pressed = new List<Buttons>();

            #region DPad

            var dpad = state.DPad;
            if (dpad.Up == ButtonState.Pressed)
                pressed.Add(Buttons.DPadUp);
            if (dpad.Down == ButtonState.Pressed)
                pressed.Add(Buttons.DPadDown);
            if (dpad.Left == ButtonState.Pressed)
                pressed.Add(Buttons.DPadLeft);
            if (dpad.Right == ButtonState.Pressed)
                pressed.Add(Buttons.DPadRight);

            #endregion

            #region Buttons

            var buttons = state.Buttons;
            if (buttons.A == ButtonState.Pressed)
                pressed.Add(Buttons.A);
            if (buttons.B == ButtonState.Pressed)
                pressed.Add(Buttons.B);
            if (buttons.X == ButtonState.Pressed)
                pressed.Add(Buttons.X);
            if (buttons.Y == ButtonState.Pressed)
                pressed.Add(Buttons.Y);
            if (buttons.Back == ButtonState.Pressed)
                pressed.Add(Buttons.Back);
            if (buttons.BigButton == ButtonState.Pressed)
                pressed.Add(Buttons.BigButton);
            if (buttons.Start == ButtonState.Pressed)
                pressed.Add(Buttons.Start);
            if (buttons.LeftShoulder == ButtonState.Pressed)
                pressed.Add(Buttons.LeftShoulder);
            if (buttons.RightShoulder == ButtonState.Pressed)
                pressed.Add(Buttons.RightShoulder);

            #endregion

            return pressed;
        }
    }
}