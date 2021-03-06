﻿using Inscribe.Configuration;
using Livet;

namespace Inscribe.ViewModels.Dialogs.SettingSub
{
    public class InputConfigViewModel : ViewModel, IApplyable
    {
        public bool IsTranscender { get { return Setting.Instance.ExperienceProperty.IsTranscender; } }

        public InputConfigViewModel()
        {
            this.UseInputSuggesting = Setting.Instance.InputExperienceProperty.UseInputSuggesting;
            this.OfficialRetweetInReplyToRetweeter = Setting.Instance.InputExperienceProperty.OfficialRetweetInReplyToRetweeter;
            this.UseActiveFallback = Setting.Instance.InputExperienceProperty.UseActiveFallback;
            this.EnableTemporarilyUserSelection = Setting.Instance.InputExperienceProperty.IsEnabledTemporarilyUserSelection;
            this.FallbackBackTracking = Setting.Instance.InputExperienceProperty.FallbackBackTracking;
            this.UseOfficialRetweetFallback = Setting.Instance.InputExperienceProperty.OfficialRetweetFallback;
            this.TrimExceedChars = Setting.Instance.InputExperienceProperty.TrimExceedChars;
            this.AutoRetryOnError = Setting.Instance.InputExperienceProperty.AutoRetryOnError;
            this.SuspendAutoBindInReply = Setting.Instance.InputExperienceProperty.SuspendAutoBindInReply;
            this.AutoUniquify = Setting.Instance.InputExperienceProperty.AutoUniquify;
            this.EnableFavoriteFallback = Setting.Instance.InputExperienceProperty.EnableFavoriteFallback;
        }

        public bool OfficialRetweetInReplyToRetweeter { get; set; }

        public bool UseInputSuggesting { get; set; }

        public bool UseActiveFallback { get; set; }

        public bool EnableTemporarilyUserSelection { get; set; }

        public bool FallbackBackTracking { get; set; }

        public bool UseOfficialRetweetFallback { get; set; }

        public bool TrimExceedChars { get; set; }

        public bool AutoRetryOnError { get; set; }

        public bool SuspendAutoBindInReply { get; set; }

        public bool AutoUniquify { get; set; }

        public bool EnableFavoriteFallback { get; set; }

        public void Apply()
        {
            Setting.Instance.InputExperienceProperty.OfficialRetweetInReplyToRetweeter = this.OfficialRetweetInReplyToRetweeter;
            Setting.Instance.InputExperienceProperty.UseInputSuggesting = this.UseInputSuggesting;
            Setting.Instance.InputExperienceProperty.UseActiveFallback = this.UseActiveFallback;
            Setting.Instance.InputExperienceProperty.IsEnabledTemporarilyUserSelection = this.EnableTemporarilyUserSelection;
            Setting.Instance.InputExperienceProperty.FallbackBackTracking = this.FallbackBackTracking;
            Setting.Instance.InputExperienceProperty.OfficialRetweetFallback = this.UseOfficialRetweetFallback;
            Setting.Instance.InputExperienceProperty.TrimExceedChars = this.TrimExceedChars;
            Setting.Instance.InputExperienceProperty.AutoRetryOnError = this.AutoRetryOnError;
            Setting.Instance.InputExperienceProperty.SuspendAutoBindInReply = this.SuspendAutoBindInReply;
            Setting.Instance.InputExperienceProperty.AutoUniquify = this.AutoUniquify;
            Setting.Instance.InputExperienceProperty.EnableFavoriteFallback = this.EnableFavoriteFallback;
        }
    }
}
