using System;
using System.Collections.Generic;
using System.Text;
using DrOpen.DrCommon.DrData;
using DrOpen.DrCommon.DrCmd;
using System.IO;

namespace SmartSnailRecConveter
{
    class Program
    {

        enum CONVERTER
        {
            TEXT,
            BIN
        }

        static void Main(string[] args)
        {

            var ddNode = new DDNode(DrCmdConst.TypeSettings, new DDType(DrCmdConst.TypeSettings));
            ddNode.Attributes.Add(DrCmdSettings.ApplicationDescription, "Converters binary file from smart snail project.");
            ddNode.Add(GetCommandTXT());
            ddNode.Add(GetCommandBIN());
            ddNode.Attributes.Add(DrCmdSettings.Arguments, args, ResolveConflict.OVERWRITE);
            var cmd = new DrCmdParser(ddNode);

            try
            {
                var res = cmd.Parse();
            }
            catch (Exception e)
            {

                if (cmd.ActiveCommnand == null)
                {
                    if ((args != null) && (args.Length > 0))
                    {
                        Console.WriteLine(GetExceptionAsString(e)); // if command line is empty
                        Console.WriteLine();
                    }
                    Console.Write(cmd.GetHelp(false)); // show command help only
                }
                else
                {
                    Console.WriteLine(GetExceptionAsString(e));
                    Console.WriteLine();
                    Console.Write(cmd.GetHelpForActiveCommand()); // show active command help only

                }
            }

                if (cmd.ActiveCommnand.Name == "BIN") ConvertFromBin(cmd.ActiveCommnand["s"].Attributes["Value"], cmd.ActiveCommnand["d"].Attributes["Value"]);
                if (cmd.ActiveCommnand.Name == "TXT") ConvertFromTxt(cmd.ActiveCommnand["s"].Attributes["Value"], cmd.ActiveCommnand["d"].Attributes["Value"]);
        }

        private static void ConvertFromTxt(string s, string d)
        {
            if (File.Exists(d)) File.Delete(d);

            using (FileStream fd = File.OpenWrite(d))
            {
                using (BinaryWriter w = new BinaryWriter(fd))
                {
                    using (FileStream fs = File.OpenRead(s))
                    {
                        using (StreamReader r = new StreamReader(fs))
                        {
                            string[] line;
                            // header
                            line = r.ReadLine().Split('\t');
                            foreach (var b in line)
                            {
                                w.Write(Convert.ToByte(b));
                            }
                            // end header
                            while (r.EndOfStream == false)
                            {
                                line = r.ReadLine().Split('\t');
                                w.Write(Convert.ToByte(line[0]));
                                w.Write(Convert.ToByte(line[1]));
                                if ((line.Length > 2) && (line[2].Length > 0)) w.Write(Convert.ToByte(line[2]));
                            }
                        }
                    }
                }
            }
        }
        private static void ConvertFromBin(string s, string d)
        {
            if (File.Exists(d)) File.Delete(d);

            using (FileStream fd = File.OpenWrite(d))
            {
                using (TextWriter w = new StreamWriter(fd))
                {
                    using (FileStream fs = File.OpenRead(s))
                    {
                        using (BinaryReader r = new BinaryReader(fs))
                        {
                            // header
                            w.WriteLine( r.ReadByte() );
                            // end header
                            byte b = 0;
                            float time = 0;
                            while (r.BaseStream.Position <= r.BaseStream.Length - 3 )
                            {
                                w.Write(r.ReadByte().ToString());
                                w.Write("\t");
                                w.Write(r.ReadByte().ToString());
                                w.Write("\t");
                                b = r.ReadByte();
                                time += b;
                                w.Write(b.ToString());
                                w.Write("\t");
                                w.Write((time / 1000).ToString("0.000"));
                                w.Write("\r\n");
                            }
                            w.Write(r.ReadByte().ToString());
                            w.Write("\t");
                            w.Write(r.ReadByte().ToString());
                            w.Write("\t");
                        }
                    }
                }
            }
        }


        private static string GetExceptionAsString(Exception e)
        {
            var text = e.Message;
            if (e.InnerException != null) text += " --> (" + GetExceptionAsString(e.InnerException) + ")";
            return text;
        }


        public static DDNode GetCommandBIN()
        {
            var name = "BIN";
            var cmd = new DDNode(name, DrCmdConst.TypeCommand);
            cmd.Attributes.Add(DrCmdCommandSettings.Name, name);
            cmd.Attributes.Add(DrCmdCommandSettings.Enabled, true);
            cmd.Attributes.Add(DrCmdCommandSettings.Description, "This command converts recorded binary file from smart snail project to text format. ");
            cmd.Attributes.Add(DrCmdCommandSettings.Example, new[] { "{0} {1} -s rec.bin -d rec.txt" });

            cmd.Add(GetOptionSourceFile());
            cmd.Add(GetOptionTargetFile());

            return cmd;
        }
        public static DDNode GetCommandTXT()
        {
            var name = "TXT";
            var cmd = new DDNode(name, DrCmdConst.TypeCommand);
            cmd.Attributes.Add(DrCmdCommandSettings.Name, name);
            cmd.Attributes.Add(DrCmdCommandSettings.Enabled, true);
            cmd.Attributes.Add(DrCmdCommandSettings.Description, "This command converts recorded text file to the binary format for smart snail project. ");
            cmd.Attributes.Add(DrCmdCommandSettings.Example, new[] { "{0} {1} -s rec.txt -d rec.bin" });

            cmd.Add(GetOptionSourceFile());
            cmd.Add(GetOptionTargetFile());

            return cmd;
        }

        public static DDNode GetOptionTargetFile()
        {
            var name = "d";
            var opt = new DDNode(name, DrCmdConst.TypeOption);
            opt.Attributes.Add(DrCmdOptionSettings.Name, name);
            opt.Attributes.Add(DrCmdOptionSettings.Enabled, true);
            opt.Attributes.Add(DrCmdOptionSettings.Aliases, new[] { "destination" });
            opt.Attributes.Add(DrCmdOptionSettings.Description, "specify destination file name.");
            opt.Attributes.Add(DrCmdOptionSettings.Type, new[] { DrCmdOptionType.Required.ToString() });
            opt.Attributes.Add(DrCmdOptionSettings.ValueFlags, new[] { DrCmdValueFlags.Required.ToString(), DrCmdValueFlags.Single.ToString() });
            opt.Attributes.Add(DrCmdOptionSettings.SynopsisValue, "rec.txt");
            opt.Attributes.Add(DrCmdOptionSettings.Synopsis, "Synopsis");
            return opt;
        }

        public static DDNode GetOptionSourceFile()
        {
            var name = "s";
            var opt = new DDNode(name, DrCmdConst.TypeOption);
            opt.Attributes.Add(DrCmdOptionSettings.Name, name);
            opt.Attributes.Add(DrCmdOptionSettings.Enabled, true);
            opt.Attributes.Add(DrCmdOptionSettings.Aliases, new[] { "source" });
            opt.Attributes.Add(DrCmdOptionSettings.Description, "specify source binary file.");
            opt.Attributes.Add(DrCmdOptionSettings.Type, new[] { DrCmdOptionType.Required.ToString() });
            opt.Attributes.Add(DrCmdOptionSettings.ValueFlags, new[] { DrCmdValueFlags.Required.ToString(), DrCmdValueFlags.Single.ToString() });
            opt.Attributes.Add(DrCmdOptionSettings.SynopsisValue, "rec.bin");
            opt.Attributes.Add(DrCmdOptionSettings.Synopsis, "Synopsis");
            return opt;
        }

    }
}
