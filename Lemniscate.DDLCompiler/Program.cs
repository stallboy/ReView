using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lemniscate;

namespace Lemniscate
{
	class Program
	{
		static void Main(string[] args)
		{
			var ddl_compiler = new DDL_Compiler();
			ddl_compiler.Mark_Generated_Files_As_Readonly = false;

			if (args[0].ToLower().StartsWith("-immediate"))
				ddl_compiler.Process_Immediately(args[1], args[2]);
			else
				ddl_compiler.Process_And_Monitor_Directory(args[0],args[1]);
			
			Console.ReadLine();
		}
	}
}
