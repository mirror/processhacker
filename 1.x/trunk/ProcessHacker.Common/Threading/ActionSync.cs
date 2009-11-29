/*
 * Process Hacker - 
 *   run once structure
 * 
 * Copyright (C) 2009 wj32
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

namespace ProcessHacker.Common.Threading
{
    /// <summary>
    /// Provides methods for synchronizing the execution of a 
    /// single action.
    /// </summary>
    public struct ActionSync
    {
        private int _value;
        private int _target;
        private Action _action;

        /// <summary>
        /// Initializes an action-sync structure.
        /// </summary>
        /// <param name="action">The action to be executed.</param>
        /// <param name="target">
        /// The target value required in order to execute the action.
        /// </param>
        public ActionSync(Action action, int target)
        {
            _action = action;
            _value = 0;
            _target = target;
        }

        /// <summary>
        /// Gets the current value of the action-sync structure.
        /// </summary>
        public int Value
        {
            get { return _value; }
        }

        /// <summary>
        /// Increments the current value and executes the action 
        /// if the target value has been reached due to this call.
        /// </summary>
        /// <remarks>
        /// The action is guaranteed to execute exactly once, 
        /// even if two threads increment the value at the same 
        /// time.
        /// </remarks>
        public void Increment()
        {
            if (Interlocked.Increment(ref _value) == _target)
                _action();
        }

        /// <summary>
        /// Increments the current value and executes the action 
        /// if the target value has been reached.
        /// </summary>
        /// <remarks>
        /// The action may be executed multiple times, as long 
        /// as the target value has been reached.
        /// </remarks>
        public void IncrementMultiple()
        {
            if (Interlocked.Increment(ref _value) >= _target)
                _action();
        }
    }
}
