using System;
using System.Collections.Generic;
using System.Text;

namespace PhotoSpaceSaver.Core.Models
{
    class MainModel
    {
        void StartScan()
        {
        }

        private List<string> directories = new List<string>();
        private Logic.PhotoSpaceSaver spaceSaver = new Logic.PhotoSpaceSaver();
    }
}
