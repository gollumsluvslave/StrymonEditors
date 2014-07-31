using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace RITS.StrymonEditor.Tests
{
    public sealed class MockKeyboardDevice : KeyboardDevice
    {
        private sealed class MockPresentationSource : PresentationSource
        {
            Visual _rootVisual;

            protected override CompositionTarget GetCompositionTargetCore()
            {
                throw new NotImplementedException();
            }

            public override bool IsDisposed
            {
                get { return false; }
            }

            public override Visual RootVisual
            {
                get { return _rootVisual; }
                set { _rootVisual = value; }
            }
        }

        private static RoutedEvent s_testEvent = EventManager.RegisterRoutedEvent(
                "Test Event",
                RoutingStrategy.Bubble,
                typeof(MockKeyboardDevice),
                typeof(MockKeyboardDevice));

        private ModifierKeys modifierKeysImpl;

        public new ModifierKeys Modifiers
        {
            get { return modifierKeysImpl; }
        }

        public MockKeyboardDevice()
            : this(InputManager.Current)
        {

        }

        public MockKeyboardDevice(InputManager manager)
            : base(manager)
        {

        }

        protected override KeyStates GetKeyStatesFromSystem(Key key)
        {
            var hasMod = false;
            switch (key)
            {
                case Key.LeftAlt:
                case Key.RightAlt:
                    hasMod = HasModifierKey(ModifierKeys.Alt);
                    break;
                case Key.LeftCtrl:
                case Key.RightCtrl:
                    hasMod = HasModifierKey(ModifierKeys.Control);
                    break;
                case Key.LeftShift:
                case Key.RightShift:
                    hasMod = HasModifierKey(ModifierKeys.Shift);
                    break;
            }

            return hasMod ? KeyStates.Down : KeyStates.None;
        }

        public KeyEventArgs CreateKeyEventArgs(
            Key key,
            ModifierKeys modKeys = ModifierKeys.None)
        {
            var arg = new KeyEventArgs(
                this,
                new MockPresentationSource(),
                0,
                key);
            modifierKeysImpl = modKeys;
            arg.RoutedEvent = s_testEvent;
            return arg;
        }


        private bool HasModifierKey(ModifierKeys modKey)
        {
            return 0 != (modifierKeysImpl & modKey);
        }

    }
}
