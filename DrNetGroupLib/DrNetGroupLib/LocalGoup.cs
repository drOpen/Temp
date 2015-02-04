/*
  LocalGoup.cs - January 31, 2015
  Copyright (c) 2013-2015 Kudryashov Andrey aka Dr    
 
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

        [DllImport("netapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        extern static int NetLocalGroupAddMembers(string serverName, string groupName,
            uint level,  LOCALGROUP_MEMBERS_INFO_3[] memberInfo, uint totalEntries);

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
                if ((userNames == null) || (userNames.Length  == 0))
                    throw new ApplicationException(string.Format( Msg.CANNOT_ADD_NULL_USERS_TO_GROUP,groupName));

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
                throw new ApplicationException(string.Format( Msg.CANNOT_ADD_USER_TO_LOCAL_GROUP,  groupName), e);
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
    }
}
