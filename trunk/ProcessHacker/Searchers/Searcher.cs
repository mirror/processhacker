/*
 * Process Hacker
 * 
 * Copyright (C) 2008 wj32
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ProcessHacker
{
    public delegate void SearchFinished();
    public delegate void SearchProgressChanged(string progress);
    public delegate void SearchError(string message);

    public interface ISearcher
    {
        event SearchFinished SearchFinished;
        event SearchProgressChanged SearchProgressChanged;
        event SearchError SearchError;
        int PID { get; }
        Dictionary<string, object> Params { get; }
        List<string[]> Results { get; }
        void Search();
    }

    public class Searcher : ISearcher
    {
        private int _pid;
        private Dictionary<string, object> _params;
        private List<string[]> _results;

        public virtual event SearchFinished SearchFinished;
        public virtual event SearchProgressChanged SearchProgressChanged;
        public virtual event SearchError SearchError;

        public Searcher(int PID)
        {
            _pid = PID;
            _params = new Dictionary<string, object>();
            _results = new List<string[]>();
        }

        public int PID
        {
            get { return _pid; }
        }

        public Dictionary<string, object> Params
        {
            get { return _params; }
        }

        public List<string[]> Results
        {
            get { return _results; }
            set { _results = value; }
        }

        public virtual void Search()
        {
        }
    }
}
