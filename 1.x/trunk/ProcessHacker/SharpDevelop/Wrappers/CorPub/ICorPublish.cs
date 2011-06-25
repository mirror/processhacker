// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision: 3165 $</version>
// </file>

#pragma warning disable 1591

namespace Debugger.Core.Wrappers.CorPub
{
    public class ICorPublish
	{
		private readonly Debugger.Interop.CorPub.CorpubPublishClass corpubPublishClass;

		public ICorPublish()
		{
			corpubPublishClass = new Debugger.Interop.CorPub.CorpubPublishClass();
		}
		
		public ICorPublishProcess GetProcess(int id) 
		{
			Debugger.Interop.CorPub.ICorPublishProcess process;
			this.corpubPublishClass.GetProcess((uint)id, out  process);
			return ICorPublishProcess.Wrap(process);
		}
	}
}

#pragma warning restore 1591
