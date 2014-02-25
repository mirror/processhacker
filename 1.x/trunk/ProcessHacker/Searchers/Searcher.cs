/*
 * Process Hacker - 
 *   searcher base class
 * 
 * Copyright (C) 2008 wj32
 * 
 * This file is part of Process Hacker.
 * 
 * Process Hacker is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * Process Hacker is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with Process Hacker.  If not, see <http://www.gnu.org/licenses/>.
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

    /// <summary>
    /// Defines a generic process memory searcher with status events. 
    /// </summary>
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

    /// <summary>
    /// A base process memory searcher. All searchers should inherit from this class.
    /// </summary>
    public class Searcher : ISearcher
    {
        private int _pid;
        private Dictionary<string, object> _params;
        private List<string[]> _results;

        public event SearchFinished SearchFinished;
        public event SearchProgressChanged SearchProgressChanged;
        public event SearchError SearchError;

        /// <summary>
        /// Creates a dummy searcher which does nothing.
        /// </summary>
        /// <param name="PID">This parameter has no effect.</param>
        public Searcher(int PID)
        {
            _pid = PID;
            _params = new Dictionary<string, object>();
            _results = new List<string[]>();
        }

        /// <summary>
        /// The PID of the process to be searched.
        /// </summary>
        public int PID
        {
            get { return _pid; }
        }

        /// <summary>
        /// The parameters of the search.
        /// </summary>
        public Dictionary<string, object> Params
        {
            get { return _params; }
        }

        /// <summary>
        /// A <see cref="List"/> containing the search results.
        /// </summary>
        public List<string[]> Results
        {
            get { return _results; }
            set { _results = value; }
        }

        /// <summary>
        /// This is a dummy function, and should be overridden.
        /// </summary>
        public virtual void Search()
        {
        }

        protected void CallSearchFinished()
        {
            if (SearchFinished != null)
                SearchFinished();
        }

        protected void CallSearchProgressChanged(string progress)
        {
            if (SearchProgressChanged != null)
                SearchProgressChanged(progress);
        }

        protected void CallSearchError(string message)
        {
            if (SearchError != null)
                SearchError(message);
        }
    }
}
