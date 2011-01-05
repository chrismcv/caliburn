﻿namespace Caliburn.ScreenComposition.Settings
{
    using System.ComponentModel.Composition;
    using Framework;
    using PresentationFramework.Screens;

    [Export(typeof(IWorkspace))]
    public class SettingsViewModel : Screen, IWorkspace {
        public SettingsViewModel() {
            DisplayName = IconName;
        }

        public string Icon {
            get { return "../Settings/Resources/Images/report48.png"; }
        }

        public string IconName {
            get { return "Settings"; }
        }

        public string Status {
            get { return string.Empty; }
        }

        public void Show() {
            Parent.ActivateItem(this);
        }
    }
}