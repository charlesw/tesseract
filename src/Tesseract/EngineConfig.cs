
using System;

namespace Tesseract
{
	public class EngineConfig
	{
		public EngineConfig()
		{
		}
		
		string _dataPath;
		string _language;
				
		public string DataPath {
			get { return _dataPath; }
			set { _dataPath = value; }
		}
		
		public string Language {
			get { return _language; }
			set { _language = value; }
		}
	}
}
