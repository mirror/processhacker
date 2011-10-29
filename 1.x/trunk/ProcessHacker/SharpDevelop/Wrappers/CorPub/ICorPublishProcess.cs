// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision: 3165 $</version>
// </file>

#pragma warning disable 1591

namespace Debugger.Core.Wrappers.CorPub
{
    using Debugger.Wrappers;
	
	public class ICorPublishProcess
	{
		private readonly Interop.CorPub.ICorPublishProcess wrappedObject;
		
		internal Interop.CorPub.ICorPublishProcess WrappedObject
		{
			get
			{
				return this.wrappedObject;
			}
		}
		
		public ICorPublishProcess(Interop.CorPub.ICorPublishProcess wrappedObject)
		{
			this.wrappedObject = wrappedObject;
			ResourceManager.TrackCOMObject(wrappedObject, typeof(ICorPublishProcess));
		}
		
		public static ICorPublishProcess Wrap(Interop.CorPub.ICorPublishProcess objectToWrap)
		{
		    if (objectToWrap != null)
			{
				return new ICorPublishProcess(objectToWrap);
			}

		    return null;
		}

	    public int ProcessId 
		{
			get 
			{
				uint id;
				wrappedObject.GetProcessID(out id);
				return (int)id;
			}
		}
		
		public bool IsManaged
		{
			get
			{
				int managed;
				wrappedObject.IsManaged(out managed);
				return managed != 0;
			}
		}
	}
}

#pragma warning restore 1591
