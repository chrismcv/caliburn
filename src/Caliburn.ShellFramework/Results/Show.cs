﻿namespace Caliburn.ShellFramework.Results
{
    using System;
    using Microsoft.Practices.ServiceLocation;
    using Microsoft.Win32;
    using PresentationFramework.Screens;
    using PresentationFramework.ViewModels;
    using Questions;

#if SILVERLIGHT
    using System.Windows.Controls;
#endif

    public static class Show
    {
        public static OpenScreenSubjectResult ChildFor(IScreenSubject screenSubject)
        {
            return new OpenScreenSubjectResult(screenSubject);
        }

        public static OpenScreenSubjectResult ChildFor<T>(T subject)
        {
            var screenSubject = new ScreenSubject<T>(subject);
            return new OpenScreenSubjectResult(screenSubject);
        }

        public static OpenChildResult<TChild> Child<TChild>()
            where TChild : IScreen
        {
            return new OpenChildResult<TChild>();
        }

        public static OpenChildResult<TChild> Child<TChild>(TChild child)
            where TChild : IScreen
        {
            return new OpenChildResult<TChild>(child);
        }

        public static OpenDialogResult<IScreen<T>> DialogFor<T>(T subject)
        {
            var factory = ServiceLocator.Current.GetInstance<IViewModelFactory>();
            var screen = factory.CreateFor(subject);

            return new OpenDialogResult<IScreen<T>>(screen);
        }

        public static OpenDialogResult<TModal> Dialog<TModal>()
            where TModal : IScreen
        {
            return new OpenDialogResult<TModal>();
        }

        public static OpenDialogResult<TModal> Dialog<TModal>(TModal modal)
            where TModal : IScreen
        {
            return new OpenDialogResult<TModal>(modal);
        }

#if SILVERLIGHT

        public static SaveFileDialogResult Dialog(SaveFileDialog dialog)
        {
            return new SaveFileDialogResult(dialog);
        }

        public static OpenFileDialogResult Dialog(OpenFileDialog dialog)
        {
            return new OpenFileDialogResult(dialog);
        }

#else

        public static ShowCommonDialogResult Dialog(CommonDialog dialog)
        {
            return new ShowCommonDialogResult(dialog);
        }

#endif

        public static PlayAnimationResult Animation(string animationKey)
        {
            return new PlayAnimationResult(animationKey);
        }

        public static MessageBoxResult MessageBox(string text)
        {
            return new MessageBoxResult(text);
        }

        public static MessageBoxResult MessageBox(string text, string caption)
        {
            return new MessageBoxResult(text, caption);
        }

        public static MessageBoxResult MessageBox(string text, string caption, Action<Answer> handleResult)
        {
            return new MessageBoxResult(text, caption, handleResult);
        }

        public static MessageBoxResult MessageBox(string text, string caption, Action<Answer> handleResult, params Answer[] possibleAnswers)
        {
            return new MessageBoxResult(text, caption, handleResult, possibleAnswers);
        }
    }
}