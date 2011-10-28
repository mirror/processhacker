using ProcessHacker.Native.Api;

namespace ProcessHacker.Native.SsLogging
{
    public class SsClientId : SsData
    {
        public SsClientId(MemoryRegion data)
        {
            this.Original = data.ReadStruct<ClientId>(0, ClientId.SizeOf, 0);
        }

        public ClientId Original
        {
            get;
            internal set;
        }
    }
}
