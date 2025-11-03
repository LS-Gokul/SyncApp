namespace LSSyncApp
{
	public class AuditLogVar
	{
		//Audit Log Table
		public string LogId { get; set; }
		public string Process { get; set; }
		public string Status { get; set; }
		public string LogDetails { get; set; }
		public string Param { get; set; }
		public string NextSyncTime { get; set; }

		//Audit Log Details Table
		public string Object { get; set; }
		public string ChildObject { get; set; }
		public int Sequence { get; set; }
		public string ObjectFromTime { get; set; }
		public string StartTime { get; set; }
		public string EndTime { get; set; }
	}
}
