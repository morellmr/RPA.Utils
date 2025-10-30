using System.Runtime.InteropServices;

namespace RPA.Utils.File
{
    public class NetworkDrive
    {
        public enum ResourceScope
        {
            RESOURCE_CONNECTED = 1,
            RESOURCE_GLOBALNET,
            RESOURCE_REMEMBERED,
            RESOURCE_RECENT,
            RESOURCE_CONTEXT
        }

        public enum ResourceType
        {
            RESOURCETYPE_ANY,
            RESOURCETYPE_DISK,
            RESOURCETYPE_PRINT,
            RESOURCETYPE_RESERVED
        }

        public enum ResourceUsage
        {
            RESOURCEUSAGE_CONNECTABLE = 0x00000001,
            RESOURCEUSAGE_CONTAINER = 0x00000002,
            RESOURCEUSAGE_NOLOCALDEVICE = 0x00000004,
            RESOURCEUSAGE_SIBLING = 0x00000008,
            RESOURCEUSAGE_ATTACHED = 0x00000010,
            RESOURCEUSAGE_ALL = (RESOURCEUSAGE_CONNECTABLE | RESOURCEUSAGE_CONTAINER | RESOURCEUSAGE_ATTACHED),
        }

        public enum ResourceDisplayType
        {
            RESOURCEDISPLAYTYPE_GENERIC,
            RESOURCEDISPLAYTYPE_DOMAIN,
            RESOURCEDISPLAYTYPE_SERVER,
            RESOURCEDISPLAYTYPE_SHARE,
            RESOURCEDISPLAYTYPE_FILE,
            RESOURCEDISPLAYTYPE_GROUP,
            RESOURCEDISPLAYTYPE_NETWORK,
            RESOURCEDISPLAYTYPE_ROOT,
            RESOURCEDISPLAYTYPE_SHAREADMIN,
            RESOURCEDISPLAYTYPE_DIRECTORY,
            RESOURCEDISPLAYTYPE_TREE,
            RESOURCEDISPLAYTYPE_NDSCONTAINER
        }

        [System.Flags]
        public enum AddConnectionOptions
        {
            CONNECT_UPDATE_PROFILE = 0x00000001,
            CONNECT_UPDATE_RECENT = 0x00000002,
            CONNECT_TEMPORARY = 0x00000004,
            CONNECT_INTERACTIVE = 0x00000008,
            CONNECT_PROMPT = 0x00000010,
            CONNECT_NEED_DRIVE = 0x00000020,
            CONNECT_REFCOUNT = 0x00000040,
            CONNECT_REDIRECT = 0x00000080,
            CONNECT_LOCALDRIVE = 0x00000100,
            CONNECT_CURRENT_MEDIA = 0x00000200,
            CONNECT_DEFERRED = 0x00000400,
            CONNECT_RESERVED = unchecked((int)0xFF000000),
            CONNECT_COMMANDLINE = 0x00000800,
            CONNECT_CMD_SAVECRED = 0x00001000,
            CONNECT_CRED_RESET = 0x00002000
        }

        [StructLayout(LayoutKind.Sequential)]
        private class NETRESOURCE
        {
            public ResourceScope dwScope = 0;
            public ResourceType dwType = ResourceType.RESOURCETYPE_DISK;
            public ResourceDisplayType dwDisplayType = 0;
            public ResourceUsage dwUsage = 0;
            public string lpLocalName = null;
            public string lpRemoteName = null;
            public string lpComment = null;
            public string lpProvider = null;
        }

        [DllImport("mpr.dll")]
        private static extern int WNetAddConnection2(NETRESOURCE lpNetResource, string lpPassword, string lpUsername, int dwFlags);

        [DllImport("mpr.dll")]
        private static extern int WNetCancelConnection2(string sLocalName, uint iFlags, int iForce);
        
        public static int MapNetworkDrive(string directory, string driveLetter, string user, string password)
        {
            NETRESOURCE myNetResource = new NETRESOURCE();
            myNetResource.lpLocalName = driveLetter + ":";
            myNetResource.lpRemoteName = directory;
            myNetResource.lpProvider = null;
            int result = WNetAddConnection2(myNetResource, password, user, 
                                            (int)AddConnectionOptions.CONNECT_TEMPORARY);
            return result;
        }
        
        public static int DisconnectNetworkDrive(string driveLetter, bool forceDisconnect)
        {
            if (forceDisconnect)
            {
                return WNetCancelConnection2(driveLetter + ":", 0, 1);
            }
            else
            {
                return WNetCancelConnection2(driveLetter + ":", 0, 0);
            }
        }
    }
}