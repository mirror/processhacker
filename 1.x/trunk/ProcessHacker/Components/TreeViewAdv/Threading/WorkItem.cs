using System.Threading;

namespace Aga.Controls.Threading
{
	public sealed class WorkItem
	{
		private readonly WaitCallback _callback;
		private readonly object _state;
		private readonly ExecutionContext _ctx;

		internal WorkItem(WaitCallback wc, object state, ExecutionContext ctx)
		{
			_callback = wc; 
			_state = state; 
			_ctx = ctx;
		}

		internal WaitCallback Callback
		{
			get
			{
				return _callback;
			}
		}

		internal object State
		{
			get
			{
				return _state;
			}
		}

		internal ExecutionContext Context
		{
			get
			{
				return _ctx;
			}
		}
	}
}
