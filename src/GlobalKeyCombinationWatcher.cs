using Gma.System.MouseKeyHook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace pWindowJax
{
    internal class GlobalKeyCombinationWatcher<T>
    {
        private readonly IKeyboardEvents keyboardEvents = Hook.GlobalEvents();
        private readonly Dictionary<Keys, Keys> pressedKeys = new Dictionary<Keys, Keys>();

        public Dictionary<HashSet<Keys>, T> KeyCombinations = new Dictionary<HashSet<Keys>, T>();

        private HashSet<Keys> currentActiveCombination;
        public T CurrentAction { get; private set; }
        public Action ActionChanged;

        public GlobalKeyCombinationWatcher()
        {
            keyboardEvents.KeyDown += keyDown;
            keyboardEvents.KeyUp += keyUp;
        }

        private void keyUp(object sender, KeyEventArgs e)
        {
            Keys key = (Keys)e.KeyValue;

            if (!pressedKeys.Remove(key) || currentActiveCombination == null)
                return;

            if (currentActiveCombination.Contains(key))
            {
                currentActiveCombination = null;
                CurrentAction = default;
                ActionChanged?.Invoke();
            }
        }

        private void keyDown(object sender, KeyEventArgs e)
        {
            Keys key = (Keys)e.KeyValue;

            if (pressedKeys.ContainsKey(key))
                return;

            pressedKeys.Add(key, e.KeyCode);

            validatePressedKeys();

            foreach (var pair in KeyCombinations)
            {
                if (pair.Key.IsSubsetOf(pressedKeys.Select(p => p.Key)))
                {
                    currentActiveCombination = pair.Key;
                    CurrentAction = pair.Value;
                    ActionChanged?.Invoke();
                    break;
                }
            }
        }

        private void validatePressedKeys()
        {
            var oldKeys = new List<Keys>();

            foreach (var key in pressedKeys)
            {
                if (PInvoke.User32.GetKeyState((int)key.Value) == 0)
                    oldKeys.Add(key.Key);
            }

            foreach (var key in oldKeys)
                pressedKeys.Remove(key);
        }
    }
}
