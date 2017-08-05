/*
  LocalGoup.cs - January 31, 2015
  Copyright (c) 2013-2017 Kudryashov Andrey aka Dr    
 
  This software is provided 'as-is', without any express or implied
  warranty. In no event will the authors be held liable for any damages
  arising from the use of this software.

  Permission is granted to anyone to use this software for any purpose,
  including commercial applications, and to alter it and redistribute it
  freely, subject to the following restrictions:

      1. The origin of this software must not be misrepresented; you must not
      claim that you wrote the original software. If you use this software
      in a product, an acknowledgment in the product documentation would be
      appreciated but is not required.

      2. Altered source versions must be plainly marked as such, and must not be
      misrepresented as being the original software.

      3. This notice may not be removed or altered from any source distribution.

      Kudryashov Andrey <kudryashov.andrey at gmail.com>

 */

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace DrNetGroupLib
{
    public static class LocalGoup
    {
        [DllImport("Netapi32.dll")]
        extern static int NetLocalGroupGetInfo([MarshalAs(UnmanagedType.LPWStr)]  string serverName,
                                               [MarshalAs(UnmanagedType.LPWStr)]  string groupName,
                                               int level,
                                               out IntPtr bufptr);

        [DllImport("Netapi32.dll", SetLastError = true)]
        static extern int NetApiBufferFree(IntPtr Buffer);

        [DllImport("Netapi32.dll")]
        extern static int NetLocalGroupAdd([MarshalAs(UnmanagedType.LPWStr)] string serverName,
                                           int level,
                                           ref LOCALGROUP_INFO_1 buf,
                                           int parm_err);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct LOCALGROUP_INFO_1
        {
            [MarshalAs(UnmanagedType.LPWStr)]
            public string lgrpi1_name;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string lgrpi1_comment;
        }
        /// <summary>
        /// The LOCALGROUP_MEMBERS_INFO_1 structure contains the security identifier (SID) and account information associated with the member of a local group.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct LOCALGROUP_MEMBERS_INFO_1
        {
            /// <summary>
            /// A pointer to a SID structure that contains the security identifier (SID) of an account that is a member of this local group member. The account can be a user account or a global group account.
            /// </summary>
            public IntPtr lgrmi1_sid;
            /// <summary>
            /// The account type associated with the security identifier specified in the lgrmi1_sid member. The following values are valid:
            /// SidTypeUser - The account is a user account, SidTypeGroup - The account is a global group account, SidTypeWellKnownGroup - The account is a well-known group account (such as Everyone). For more information, see Well-Known SIDs, SidTypeDeletedAccount - The account has been deleted, SidTypeUnknown - The account type cannot be determined, 
            /// </summary>
            public int lgrmi1_sidusage;
            /// <summary>
            /// A pointer to the account name of the local group member identified by the lgrmi1_sid member. The lgrmi1_name member does not include the domain name. For more information, see the following Remarks section.
            /// </summary>
            public string lgrmi1_name;
            /// <summary>
            /// returns size of current structure
            /// </summary>
            public static readonly int SizeOf = Marshal.SizeOf(typeof(LOCALGROUP_MEMBERS_INFO_1));
        }

        /// <summary>
        /// The NetLocalGroupGetMembers function retrieves a list of the members of a particular local group in the security database, which is the security accounts manager (SAM) database or, in the case of domain controllers, the Active Directory. Local group members can be users or global groups.
        /// </summary>
        /// <param name="servername">Pointer to a constant string that specifies the DNS or NetBIOS name of the remote server on which the function is to execute. If this parameter is NULL, the local computer is used.</param>
        /// <param name="localgroupname">Pointer to a constant string that specifies the name of the local group whose members are to be listed. For more information, see the following Remarks section.</param>
        /// <param name="level">Specifies the information level of the data. This parameter can be one of the following values 0-3.</param>
        /// <param name="bufptr">Pointer to the address that receives the return information structure. The format of this data depends on the value of the level parameter. This buffer is allocated by the system and must be freed using the NetApiBufferFree function. Note that you must free the buffer even if the function fails with ERROR_MORE_DATA.</param>
        /// <param name="prefmaxlen">Specifies the preferred maximum length of returned data, in bytes. If you specify MAX_PREFERRED_LENGTH, the function allocates the amount of memory required for the data. If you specify another value in this parameter, it can restrict the number of bytes that the function returns. If the buffer size is insufficient to hold all entries, the function returns ERROR_MORE_DATA. For more information, see Network Management Function Buffers and Network Management Function Buffer Lengths.</param>
        /// <param name="entriesread">Pointer to a value that receives the count of elements actually enumerated.</param>
        /// <param name="totalentries">Pointer to a value that receives the total number of entries that could have been enumerated from the current resume position.</param>
        /// <param name="resumehandle">Pointer to a value that contains a resume handle which is used to continue an existing group member search. The handle should be zero on the first call and left unchanged for subsequent calls. If this parameter is NULL, then no resume handle is stored.</param>
        /// <returns></returns>
        [DllImport("Netapi32.dll")]
        public extern static int NetLocalGroupGetMembers(
            [MarshalAs(UnmanagedType.LPWStr)] string servername,
            [MarshalAs(UnmanagedType.LPWStr)] string localgroupname,
            int level, out IntPtr bufptr, int prefmaxlen, out int entriesread, out int totalentries, out IntPtr resumehandle);


        [DllImport("netapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        extern static int NetLocalGroupAddMembers(string serverName, string groupName,
            uint level, LOCALGROUP_MEMBERS_INFO_3[] memberInfo, uint totalEntries);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct LOCALGROUP_MEMBERS_INFO_3
        {
            [MarshalAs(UnmanagedType.LPWStr)]
            public string Domain;
        }

        enum SID_NAME_USE
        {
            SidTypeUser = 1,
            SidTypeGroup,
            SidTypeDomain,
            SidTypeAlias,
            SidTypeWellKnownGroup,
            SidTypeDeletedAccount,
            SidTypeInvalid,
            SidTypeUnknown,
            SidTypeComputer
        }


        #region Const
        /// <summary>
        /// Success
        /// </summary>
        public const int NERR_Success = 0;
        /// <summary>
        /// NERR_BASE is the base of error codes from network utilities, chosen to avoid conflict with system and redirector error codes. 2100 is a value that has been assigned to us by system.
        /// </summary>
        public const int NERR_BASE = 2100;
        /// <summary>
        /// The group name could not be found.
        /// </summary>
        public const int NERR_GroupNotFound = (NERR_BASE + 120);
        /// <summary>
        /// allocates the amount of memory required for the data
        /// </summary>
        public const int MAX_PREFERRED_LENGTH = -1;
        public const int ERROR_MORE_DATA = 234;
        /// <summary>
        /// The operation was canceled by the user.
        /// </summary>
        public const int ERROR_CANCELLED = 1223;
        #endregion Const

        #region AddUsersToLocalGroupByName


        /// <summary>
        /// Function adds membership of one or more existing user accounts or global group accounts to an existing local group. The function does not change the membership status of users or global groups that are currently members of the local group.
        /// If the function succeeds, the return value is true. If the function fails, - throw new application exception
        /// </summary>
        /// <param name="serverName">string that specifies the DNS or NetBIOS name of the remote server on which the function is to execute. If this parameter is NULL, the local computer is used.</param>
        /// <param name="groupName">string that specifies the name of the local group to which the specified users or global groups will be added</param>
        /// <param name="userNames">string array that contains the data for the new local group members.</param>
        /// <returns>If the function succeeds, the return value is true. If the function fails, - throw new application exception</returns>
        /// <exception cref="ApplicationException">If the function fails, - throw new application exception</exception>
        public static bool AddUsersToLocalGroupByName(string serverName, string groupName, params string[] userNames)
        {
            try
            {
                if ((userNames == null) || (userNames.Length == 0))
                    throw new ApplicationException(string.Format(Msg.CANNOT_ADD_NULL_USERS_TO_GROUP, groupName));

                var users = new LOCALGROUP_MEMBERS_INFO_3[userNames.Length];
                for (int i = 0; i < userNames.Length; i++)
                {
                    users[i].Domain = userNames[i];
                }
                var result = NetLocalGroupAddMembers(serverName, groupName, 3, users, (uint)userNames.Length);
                if (result != NERR_Success) throw new Win32Exception(result);
                return true;
            }
            catch (Exception e)
            {
                throw new ApplicationException(string.Format(Msg.CANNOT_ADD_USER_TO_LOCAL_GROUP, groupName), e);
            }
        }

        #endregion AddUsersToLocalGroupByName

        # region IsThereLocalGroup
        /// <summary>
        /// The function try retrieves information about a particular local group account on the local server and return true if group already exists, false if group name could not be found, otherwise throw exception
        /// </summary>
        /// <param name="groupName">constant string that specifies the name of the local group account for which the information will be retrieved.</param>
        /// <returns>Return true if group already exists, false if group name could not be found, otherwise throw exception.</returns>
        public static bool IsThereLocalGroup(string groupName)
        {
            return IsThereLocalGroup(null, groupName);
        }

        /// <summary>
        /// The function try retrieves information about a particular local group account on a server and return true if group already exists, false if group name could not be found, otherwise throw exception
        /// </summary>
        /// <param name="serverName">constant string that specifies the DNS or NetBIOS name of the remote server on which the function is to execute. If this parameter is NULL, the local computer is used.</param>
        /// <param name="groupName">constant string that specifies the name of the local group account for which the information will be retrieved.</param>
        /// <returns>Return true if group already exists, false if group name could not be found, otherwise throw exception.</returns>
        public static bool IsThereLocalGroup(string serverName, string groupName)
        {
            try
            {
                IntPtr ptr;
                int result = NetLocalGroupGetInfo(serverName, groupName, 1, out ptr);
                if (result != NERR_Success)
                {
                    if (result == NERR_GroupNotFound) return false; //The group name could not be found.
                    throw new Win32Exception(result);
                }
                NetApiBufferFree(ptr);
                return true;
            }
            catch (Exception e)
            {
                if (serverName == null) serverName = "";
                throw new ApplicationException(String.Format(Msg.CANNOT_GET_LOCAL_GROUP, groupName, serverName), e);
            }
        }
        #endregion IsThereLocalGroup

        #region CreateLocalGroup
        /// <summary>
        /// The function creates a local group in the security database, which is the security accounts manager (SAM) database or, in the case of domain controllers, the Active Directory on the local server
        /// If the function succeeds, the return value is true. If the function fails, throw exception
        /// </summary>
        /// <param name="groupName">string that specifies a local group name.</param>
        /// <param name="groupDescription">string that contains a remark associated with the local group. This member can be a null string. The comment can have as many as MAXCOMMENTSZ characters.</param>
        /// <returns>If the function succeeds, the return value is true. If the function fails, throw exception</returns>
        public static bool CreateLocalGroup(string groupName, string groupDescription)
        {
            return CreateLocalGroup(null, groupName, groupDescription);
        }
        /// <summary>
        /// The function creates a local group in the security database, which is the security accounts manager (SAM) database or, in the case of domain controllers, the Active Directory.
        /// If the function succeeds, the return value is true. If the function fails, throw exception
        /// </summary>
        /// <param name="serverName">A pointer to a string that specifies the DNS or NetBIOS name of the remote server on which the function is to execute. If this parameter is NULL, the local computer is used.</param>
        /// <param name="groupName">string that specifies a local group name.</param>
        /// <param name="groupDescription">string that contains a remark associated with the local group. This member can be a null string. The comment can have as many as MAXCOMMENTSZ characters.</param>
        /// <returns>If the function succeeds, the return value is true. If the function fails, throw exception</returns>
        public static bool CreateLocalGroup(string serverName, string groupName, string groupDescription)
        {
            try
            {
                var lgInf = new LOCALGROUP_INFO_1();
                lgInf.lgrpi1_name = groupName;
                lgInf.lgrpi1_comment = groupDescription;
                var res = NetLocalGroupAdd(serverName, 1, ref lgInf, 0);
                if (res != NERR_Success) throw new Win32Exception(res);
                return true;
            }
            catch (Exception e)
            {
                if (serverName == null) serverName = "";
                throw new ApplicationException(String.Format(Msg.CANNOT_ADD_LOCAL_GROUP, groupName, serverName), e);
            }
        }


        #endregion CreateLocalGroup

        #region GetLocalGroupGetMembers

        public class DrLocalGroupEventArgsEnumeratedPartialyOfLocalGroupMemberInfo1 : EventArgs
        {
            public DrLocalGroupEventArgsEnumeratedPartialyOfLocalGroupMemberInfo1(int entriesRead,  int entriesTotal, LOCALGROUP_MEMBERS_INFO_1[] memberInfo)
            {
                this.MemberInfo = memberInfo;
                this.EntriesRead = entriesRead;
                this.EntriesTotal = entriesTotal;
                this.Cancel = false;
            }

            public LOCALGROUP_MEMBERS_INFO_1[] MemberInfo { get; private set; }
            public int EntriesRead { get; private set; }
            public int EntriesTotal { get; private set; }
            public bool Cancel { get; set; }
        }

        public static event EventHandler<DrLocalGroupEventArgsEnumeratedPartialyOfLocalGroupMemberInfo1> EventEnumeratePartiallyOfLocalGroupMemberInfo1;
        static void OnEnumeratedPartialyOfLocalGroupMemberInfo1(DrLocalGroupEventArgsEnumeratedPartialyOfLocalGroupMemberInfo1 e)
        {
            EventHandler<DrLocalGroupEventArgsEnumeratedPartialyOfLocalGroupMemberInfo1> handler = EventEnumeratePartiallyOfLocalGroupMemberInfo1;
            if (handler != null) handler(null, e);
        }
        /// <summary>
        /// Returns a list of the names of members of a particular local group in the security database, which is the security accounts manager (SAM) database or, in the case of domain controllers, the Active Directory. Local group members can be users or global groups. This function uses value of buffer size = 32768 bytes
        /// </summary>
        /// <param name="serverName">string that specifies the DNS or NetBIOS name of the remote server on which the function is to execute. If this parameter is empty, the local computer is used.</param>
        /// <param name="groupName">string that specifies the name of the local group whose members are to be listed. For more information, see the following Remarks section.</param>

        /// <returns></returns>
        public static LOCALGROUP_MEMBERS_INFO_1[] GetLocalGroupGetMembers1(string serverName, string groupName)
        {
            return GetLocalGroupGetMembers1(serverName, groupName, 32768);
        }
        /// <summary>
        /// Returns a list of the names of members of a particular local group in the security database, which is the security accounts manager (SAM) database or, in the case of domain controllers, the Active Directory. Local group members can be users or global groups.
        /// </summary>
        /// <param name="serverName">string that specifies the DNS or NetBIOS name of the remote server on which the function is to execute. If this parameter is empty, the local computer is used.</param>
        /// <param name="groupName">string that specifies the name of the local group whose members are to be listed. For more information, see the following Remarks section.</param>
        /// <param name="pageBufferSize">value of buffer size in bytes for paging information about members of local group. Specify MAX_PREFERRED_LENGTH for work without paging. For example, approximately 128 bytres need to a single group information.</param>
        /// <returns></returns>
        public static LOCALGROUP_MEMBERS_INFO_1[] GetLocalGroupGetMembers1(string serverName, string groupName, int pageBufferSize)
        {
            LOCALGROUP_MEMBERS_INFO_1[] membersInfo = { };
            IntPtr ptr, hResume;
            int entriesRead, entriesTotal, res;
            do
            {
                res = NetLocalGroupGetMembers(serverName, groupName, 1, out ptr, 128, out entriesRead, out entriesTotal, out hResume);
                if ((res == NERR_Success) || (res == ERROR_MORE_DATA))
                {
                    if ((entriesRead == 0) && (res == NERR_Success)) return membersInfo; // there are nobody
                    int iStart = membersInfo.Length;
                    Array.Resize<LOCALGROUP_MEMBERS_INFO_1>(ref membersInfo, entriesRead + membersInfo.Length);
                    IntPtr ptrMemberInfo = ptr;
                    for (int i = 0; i < entriesRead; i++)
                    {
                        membersInfo[i + iStart] = (LOCALGROUP_MEMBERS_INFO_1)Marshal.PtrToStructure(ptrMemberInfo, typeof(LOCALGROUP_MEMBERS_INFO_1));
                        ptrMemberInfo = (IntPtr)(ptrMemberInfo.ToInt32() + LOCALGROUP_MEMBERS_INFO_1.SizeOf);
                    }
                    NetApiBufferFree(ptr);
                    var eArgs = new DrLocalGroupEventArgsEnumeratedPartialyOfLocalGroupMemberInfo1(entriesRead, entriesTotal, membersInfo);
                    OnEnumeratedPartialyOfLocalGroupMemberInfo1(eArgs);
                    if (eArgs.Cancel == false) throw new Win32Exception(ERROR_CANCELLED);
                }
            } while (res == ERROR_MORE_DATA);
            if (res == NERR_Success) return membersInfo;
            throw new Win32Exception(res);
        }

        #endregion GetLocalGroupGetMembers
    }
}
