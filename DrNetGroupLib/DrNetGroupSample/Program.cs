/*
  Program.cs - January 31, 2015
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DrNetGroupLib;

namespace DrNetGroupSample
{
    class Program
    {
        static void Main(string[] args)
        {
            var grName = "Test";
            var grDescription = "Test Description";
            try
            {
                Console.WriteLine("Check group '{0}'", grName);
                bool res = LocalGoup.IsThereLocalGroup(grName);
                if (res)
                    Console.WriteLine("The group '{0}' already exists.", grName);
                else
                {
                    Console.WriteLine("The group '{0}' doesn't exists.", grName);
                    LocalGoup.CreateLocalGroup(grName, grDescription);
                    Console.WriteLine("The group '{0}' successfully created.", grName);
                }

                LocalGoup.AddUsersToLocalGroupByName(null, grName, "Administrator");

            }
            catch (Exception e)
            {
                Console.WriteLine(GetExceptionMessage(e));
            }

        }


        static private string GetExceptionMessage(Exception e)
        {
            var msg = e.Message;
            if (e.InnerException != null) msg = msg + " (" + GetExceptionMessage(e.InnerException) + ")";
            return msg;
        }

    }
}
