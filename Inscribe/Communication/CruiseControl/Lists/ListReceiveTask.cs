﻿using System.Collections.Generic;
using Inscribe.Authentication;

namespace Inscribe.Communication.CruiseControl.Lists
{
    public class ListReceiveTask : ReceiveTaskBase
    {
        private AccountInfo _accountInfo;
        public AccountInfo AccountInfo
        {
            get { return this._accountInfo; }
        }

        private string _listUserScreenName;
        public string ListUserScreenName
        {
            get { return this._listUserScreenName; }
        }

        private string _listName;
        public string ListName
        {
            get { return this._listName; }
        }

        public ListReceiveTask(AccountInfo info, string user, string listName)
        {
            this._accountInfo = info;
            this._listUserScreenName = user;
            this._listName = listName;
        }

        protected override int ReceiveCount
        {
            get { return TwitterDefine.ListReceiveCount; }
        }

        protected override IEnumerable<Dulcet.Twitter.TwitterStatusBase> GetTweets()
        {
            return InjectionPoint._GetListTimelineInjection.Execute(new System.Tuple<AccountInfo, string, string>(
                AccountInfo, ListUserScreenName, ListName));
        }

        public void EndReceive()
        {
            this.RemoveSchedule();
        }
    }
}
