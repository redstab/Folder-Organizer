using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Folder_Organizer
{
	public partial class FolderOrganizerService : ServiceBase
	{
		public FolderOrganizerService()
		{
			InitializeComponent();
		}

		protected override void OnStart(string[] args)
		{
		}

		protected override void OnStop()
		{
		}
	}
}
