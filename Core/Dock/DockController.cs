using GameBoost.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace GameBoost.Core.Dock
{
    public class DockController(FrameworkElement dockRoot) : IDockController
    {
        private readonly FrameworkElement _dockRoot = dockRoot;

        public void SetState(DockState state)
        {
            VisualStateManager.GoToElementState(
                _dockRoot,
                state == DockState.Full ? "FullDock" : "CompactDock",
                true);
        }
    }
}
