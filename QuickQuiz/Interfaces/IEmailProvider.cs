﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuickQuiz.Interfaces
{
	public interface IEmailProvider
	{
		public void SendEmail(string to, string subject, string content);
	}
}
