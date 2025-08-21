using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWEN2_TourPlannerGroupProject.Logging
{
    public interface ILoggerWrapper
    {
        void Debug(string message);
        void Info(string message);
        void Error(string message);
        void Fatal(string message);
        void Warn(string message);
    }
}

