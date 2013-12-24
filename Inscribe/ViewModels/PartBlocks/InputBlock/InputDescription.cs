﻿using Inscribe.Authentication;
using Inscribe.Helpers;
using Inscribe.Storage;
using Inscribe.ViewModels.Common;
using Livet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Inscribe.ViewModels.PartBlocks.InputBlock
{
    public class InputDescription : ViewModel
    {
        private long _inReplyToId = 0;
        public long InReplyToId
        {
            get { return this._inReplyToId; }
            set
            {
                this._inReplyToId = value;
                RaisePropertyChanged(() => InReplyToId);
                RaisePropertyChanged(() => InReplyToIdStatus);
                RaisePropertyChanged(() => IsInReplyToEnabled);
            }
        }

        public bool IsInReplyToEnabled
        {
            get { return this._inReplyToId != 0; }
        }

        public string InReplyToIdStatus
        {
            get
            {
                var tvm = TweetStorage.Get(this._inReplyToId);
                if (tvm == null)
                    return "[undefined]";
                else
                    return "@" + tvm.Status.User.ScreenName + ": " + tvm.Status.Text;
            }
        }

        private string _inputText = String.Empty;
        public string InputText
        {
            get { return this._inputText; }
            set
            {
                if (this._inputText == value) return;
                this._inputText = value;
                this.isTextBoxVM.TextBoxText = value;
                RaisePropertyChanged(() => InputText);
            }
        }

        private string _attachedImage = null;

        private IntelliSenseTextBoxViewModel isTextBoxVM;

        public InputDescription(IntelliSenseTextBoxViewModel intelliSenseTextBoxViewModel)
        {
            this.isTextBoxVM = intelliSenseTextBoxViewModel;
            ViewModelHelper.BindNotification(intelliSenseTextBoxViewModel.TextChangedEvent, this, (o, e) =>
            {
                this.InputText = this.isTextBoxVM.TextBoxText;
            });
            // Initialize
            this.isTextBoxVM.TextBoxText = String.Empty;
        }

        public string AttachedImage
        {
            get { return this._attachedImage; }
            set
            {
                this._attachedImage = value;
                RaisePropertyChanged(()=>AttachedImage);
                RaisePropertyChanged(() => IsAttachedImage);
            }
        }

        public bool IsAttachedImage
        {
            get { return this._attachedImage != null; }
        }

        public IEnumerable<TweetWorker> ReadyUpdate(InputBlockViewModel ibvm, IEnumerable<string> bindTags, IEnumerable<AccountInfo> infos)
        {
            var containsTags = TwitterRegexPatterns.ValidHashtag.Matches(InputText)
                .Cast<Match>().Select(m => m.Value).ToArray();
            var excepteds = bindTags.Except(containsTags).Distinct();
            return infos
                .Where(i => i != null)
                .Select(i => new TweetWorker(ibvm, i, InputText, InReplyToId, AttachedImage, excepteds.ToArray()));
        }

    }
}
