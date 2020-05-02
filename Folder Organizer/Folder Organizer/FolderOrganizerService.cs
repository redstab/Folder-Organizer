using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Folder_Organizer
{
	public partial class FolderOrganizerService : ServiceBase
	{

		public enum ServiceState
		{
			SERVICE_STOPPED = 0x00000001,
			SERVICE_START_PENDING = 0x00000002,
			SERVICE_STOP_PENDING = 0x00000003,
			SERVICE_RUNNING = 0x00000004,
			SERVICE_CONTINUE_PENDING = 0x00000005,
			SERVICE_PAUSE_PENDING = 0x00000006,
			SERVICE_PAUSED = 0x00000007,
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct ServiceStatus
		{
			public int dwServiceType;
			public ServiceState dwCurrentState;
			public int dwControlsAccepted;
			public int dwWin32ExitCode;
			public int dwServiceSpecificExitCode;
			public int dwCheckPoint;
			public int dwWaitHint;
		};

		StringDictionary ExtensionMapper = new StringDictionary();

		[DllImport("advapi32.dll", SetLastError = true)]
		private static extern bool SetServiceStatus(System.IntPtr handle, ref ServiceStatus serviceStatus);

		public FolderOrganizerService()
		{
			InitializeComponent();
			Logger = new EventLog();
			if (!EventLog.SourceExists("FolderOrg"))
			{
				EventLog.CreateEventSource("FolderOrg", "Folder Organizer");
			}
			Logger.Source = "FolderOrg";
			Logger.Log = "Folder Organizer";
		}

		protected override void OnStart(string[] args)
		{
			Logger.WriteEntry("Started Folder Organizer with " + ConfigurationManager.AppSettings["WatchPath"] + " as its path");
			ServiceStatus serviceStatus = new ServiceStatus();
			serviceStatus.dwCurrentState = ServiceState.SERVICE_START_PENDING;
			serviceStatus.dwWaitHint = 100000;
			SetServiceStatus(this.ServiceHandle, ref serviceStatus);

			using (TextFieldParser csvParser = new TextFieldParser(ConfigurationManager.AppSettings["ExtensionDatabaseFile"]))
			{
				csvParser.CommentTokens = new string[] { "#" };
				csvParser.SetDelimiters(new string[] { "," });
				csvParser.HasFieldsEnclosedInQuotes = false;

				while (!csvParser.EndOfData)
				{
					string[] fields = csvParser.ReadFields();
					fields.Skip(1).ToList().ForEach(extension =>
					{
						if (!ExtensionMapper.ContainsKey(extension))
						{
							ExtensionMapper.Add(extension, fields[0]);
						}
					});
				}
			}

			FileSystemWatcher Watcher = new FileSystemWatcher();
			Watcher.Path = ConfigurationManager.AppSettings["WatchPath"];
			Watcher.EnableRaisingEvents = true;
			Watcher.Created += new FileSystemEventHandler(HandleFile);
			Watcher.Changed += new FileSystemEventHandler(HandleFile);

			serviceStatus.dwCurrentState = ServiceState.SERVICE_RUNNING;
			SetServiceStatus(this.ServiceHandle, ref serviceStatus);
		}

		private void HandleFile(object sender, FileSystemEventArgs e)
		{
			string path = e.FullPath;
			
		}

		protected override void OnStop()
		{
			Logger.WriteEntry("Folder Organizer Stopped");
		}
	}
}
