using Gma.System.MouseKeyHook;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace pWindowJax
{
    internal class GlobalKeyCombinationWatcher<T>
    {
        private readonly IKeyboardEvents keyboardEvents = Hook.GlobalEvents();
        private readonly HashSet<Keys> pressedKeys = new HashSet<Keys>();

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
                CurrentAction = default(T);
                ActionChanged?.Invoke();
            }
        }

        private void keyDown(object sender, KeyEventArgs e)
        {
            Keys key = (Keys)e.KeyValue;

            if (!pressedKeys.Add(key))
                return;

            foreach(var pair in KeyCombinations)
            {
                if (pair.Key.IsSubsetOf(pressedKeys))
                {
                    currentActiveCombination = pair.Key;
                    CurrentAction = pair.Value;
                    ActionChanged?.Invoke();
                    break;
                }
            }
        }
    }
}
