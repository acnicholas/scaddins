// (C) Copyright 2018-2020 by Andrew Nicholas andrewnicholas@iinet.net.au
//
// This file is part of SCaddins.
//
// SCaddins is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// SCaddins is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with SCaddins.  If not, see <http://www.gnu.org/licenses/>.

namespace SCaddins.DestructivePurge.ViewModels
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    public class CheckableItem : Caliburn.Micro.PropertyChangedBase
    {
        private ObservableCollection<CheckableItem> children;
        private bool? isChecked;
        private CheckableItem parent;

        public CheckableItem(DeletableItem deletable, CheckableItem parent)
        {
            Deletable = deletable;
            children = new ObservableCollection<CheckableItem>();
            IsChecked = false;
            this.parent = parent;
        }

        public ObservableCollection<CheckableItem> Children
        {
            get { return children; }
        }

        public DeletableItem Deletable
        {
            get; set;
        }

        public bool IsYesOrMaybe
        {
            get
            {
                return IsYes || IsMaybe;
            }
        }

        public bool IsYes
        {
            get
            {
                return IsChecked.HasValue && IsChecked.Value;
            }
        }

        public bool IsNo
        {
            get
            {
                return IsChecked.HasValue && IsChecked.Value == false;
            }
        }

        public bool IsMaybe
        {
            get
            {
                return IsChecked == null;
            }
        }

        public int CheckedCount
        {
            get
            {
                int n = 0;
                if (IsYes && Deletable.Id != null)
                {
                    n++;
                }
                if (IsYesOrMaybe)
                {
                    foreach (var child in Children)
                    {
                        n += child.CheckedCount;
                    }
                }
                return n;
            }
        }

        public bool? IsChecked
        {
            get
            {
                return isChecked;
            }

            set
            {
                if (value != isChecked)
                {
                    isChecked = value;
                    if (isChecked.HasValue)
                    {
                        foreach (var i in Children)
                        {
                            i.IsChecked = value;
                        }
                    }
                    NotifyOfPropertyChange(() => IsChecked);
                    NotifyOfPropertyChange(() => CheckedCount);

                    if (parent != null)
                    {
                        int n = 0;
                        foreach (var i in parent.Children)
                        {
                            if (i.IsChecked.HasValue && i.IsChecked.Value)
                            {
                                n++;
                            }
                            if (i.IsChecked.HasValue && i.IsChecked.Value == false)
                            {
                                n--;
                            }
                        }
                        bool? nb = null;
                        parent.IsChecked = System.Math.Abs(n) == parent.Children.Count ? n > 0 : nb;
                    }
                }
            }
        }

        public string Name
        {
            get { return Deletable.Name; }
        }

        public void AddChild(CheckableItem deletable)
        {
            if (deletable == null)
            {
                return;
            }
            Children.Add(deletable);
        }

        public void AddChildren(List<DeletableItem> deletables)
        {
            if (deletables == null || deletables.Count < 1)
            {
                return;
            }
            foreach (var deletable in deletables)
            {
                Children.Add(new CheckableItem(deletable, this));
            }
        }
    }
}

/* vim: set ts=4 sw=4 nu expandtab: */
