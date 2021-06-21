using System;
using System.Collections.Generic;
using System.Text;
using PokerPlatformCommon;
using PokerPlatformCommon.Proto;

namespace PokerPlatformClient
{
    public abstract class AbstractStrategy
    {
        public void InitTable(PokerPlatformCommon.TableView tableView)
        {
            TableView = tableView;
            OnInitTable();
        }

        public abstract void OnInitTable();

        protected PokerPlatformCommon.TableView TableView { get; private set; }
    }
}
