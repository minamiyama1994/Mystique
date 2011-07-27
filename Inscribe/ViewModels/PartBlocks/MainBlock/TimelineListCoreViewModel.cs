﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Livet;
using Inscribe.Filter;
using System.Threading.Tasks;
using Livet.Commands;
using System.Windows.Data;
using Inscribe.Data;
using Inscribe.ViewModels.PartBlocks.MainBlock.TimelineChild;
using Inscribe.Storage;
using Inscribe.Configuration;
using System.ComponentModel;
using Inscribe.ViewModels.Behaviors.Messaging;

namespace Inscribe.ViewModels.PartBlocks.MainBlock
{
    public class TimelineListCoreViewModel : ViewModel
    {
        public TabViewModel Parent { get; private set; }

        public event EventHandler NewTweetReceived;
        private void OnNewTweetReceived()
        {
            var handler = NewTweetReceived;
            if (handler != null)
                NewTweetReceived(this, EventArgs.Empty);
        }

        private IEnumerable<IFilter> sources;
        public IEnumerable<IFilter> Sources
        {
            get { return this.sources; }
            set
            {
                if (this.sources == value) return;
                var prev = this.sources;
                this.sources = (value ?? new IFilter[0]).ToArray();
                UpdateReacceptionChain(prev, false);
                UpdateReacceptionChain(value, true);
                RaisePropertyChanged(() => Sources);
            }
        }

        private void UpdateReacceptionChain(IEnumerable<IFilter> filter, bool join)
        {
            if (filter != null)
            {
                if (join)
                    filter.ForEach(f => f.RequireReaccept += f_RequireReaccept);
                else
                    filter.ForEach(f => f.RequireReaccept -= f_RequireReaccept);
            }
        }

        void f_RequireReaccept()
        {
            System.Diagnostics.Debug.WriteLine("received reacception");
            Task.Factory.StartNew(() => this.RefreshCache()).ContinueWith(_ => this.Commit(true));
        }

        public TimelineListCoreViewModel(TabViewModel parent, IEnumerable<IFilter> sources = null)
        {
            this.Parent = parent;
            this.sources = sources;
            UpdateReacceptionChain(sources, true);
            // binding nortifications
            ViewModelHelper.BindNotification(TweetStorage.Notificator, this, TweetStorageChanged);
            ViewModelHelper.BindNotification(Setting.SettingValueChangedEvent, this, SettingValueChanged);
            // Initialize binding timeline
            this._tweetsSource = new CachedConcurrentObservableCollection<TabDependentTweetViewModel>();
            this._tweetCollectionView = new CollectionViewSource();
            this._tweetCollectionView.Source = this._tweetsSource;
            // Generate timeline
            Task.Factory.StartNew(() => RefreshCache())
                .ContinueWith(_ => DispatcherHelper.BeginInvoke(() => UpdateSortDescription()));
        }

        public void Commit(bool reinvalidate)
        {
            DispatcherHelper.BeginInvoke(() =>
            {
                using (var disp = this._tweetCollectionView.DeferRefresh())
                {
                    this._tweetsSource.Commit(reinvalidate);
                }
            });
        }

        private void TweetStorageChanged(object o, TweetStorageChangedEventArgs e)
        {
            switch (e.ActionKind)
            {
                case TweetActionKind.Added:
                    if (CheckFilters(e.Tweet))
                    {
                        this._tweetsSource.Add(new TabDependentTweetViewModel(e.Tweet, this.Parent));
                        OnNewTweetReceived();
                    }
                    break;
                case TweetActionKind.Refresh:
                    Task.Factory.StartNew(RefreshCache);
                    break;
                case TweetActionKind.Changed:
                    var tdtvm = new TabDependentTweetViewModel(e.Tweet, this.Parent);
                    var contains = this._tweetsSource.Contains(tdtvm);
                    if (CheckFilters(e.Tweet) != contains)
                    {
                        if (contains)
                            this._tweetsSource.Remove(tdtvm);
                        else
                            this._tweetsSource.Add(tdtvm);
                    }
                    break;
                case TweetActionKind.Removed:
                    this._tweetsSource.Remove(new TabDependentTweetViewModel(e.Tweet, this.Parent));
                    break;
            }
        }

        private void SettingValueChanged(object o, EventArgs e)
        {
            UpdateSortDescription();
        }

        private void UpdateSortDescription()
        {
            using (var disp = this._tweetCollectionView.DeferRefresh())
            {
                this._tweetsSource.Commit();
                this._tweetCollectionView.SortDescriptions.Clear();
                if (Setting.Instance.TimelineExperienceProperty.UseAscendingSort)
                    this._tweetCollectionView.SortDescriptions.Add(
                        new SortDescription("Tweet.CreatedAt", ListSortDirection.Ascending));
                else
                    this._tweetCollectionView.SortDescriptions.Add(
                        new SortDescription("Tweet.CreatedAt", ListSortDirection.Descending));
            }
        }

        public bool CheckFilters(TweetViewModel viewModel)
        {
            if (!viewModel.IsStatusInfoContains) return false;
            return (this.sources ?? this.Parent.TabProperty.TweetSources ?? new IFilter[0])
                .Any(f => f.Filter(viewModel.Status));
        }

        public void RefreshCache()
        {
            this._tweetsSource.Clear();
            TweetStorage.GetAll(vm => CheckFilters(vm))
                .Select(tvm => new TabDependentTweetViewModel(tvm, this.Parent))
                .Where(tvm => !this._tweetsSource.Contains(tvm))
                .ForEach(this._tweetsSource.Add);
        }

        private CachedConcurrentObservableCollection<TabDependentTweetViewModel> _tweetsSource;
        public ICollection<TabDependentTweetViewModel> TweetsSource { get { return this._tweetsSource; } }

        private CollectionViewSource _tweetCollectionView;
        public CollectionViewSource TweetCollectionView { get { return this._tweetCollectionView; } }

        private TabDependentTweetViewModel _selectedTweetViewModel = null;
        public TabDependentTweetViewModel SelectedTweetViewModel
        {
            get { return _selectedTweetViewModel; }
            set
            {
                this._selectedTweetViewModel = value;
                RaisePropertyChanged(() => SelectedTweetViewModel);
                Task.Factory.StartNew(() => this.Commit(false))
                    .ContinueWith(_ => Parallel.ForEach(this._tweetsSource, vm => vm.PendingColorChanged()));
            }
        }

        public void SetSelect(ListSelectionKind kind)
        {
            Messenger.Raise(new SetListSelectionMessage("SetListSelection", kind));
        }
    }
}