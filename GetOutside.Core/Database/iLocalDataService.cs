using GetOutside.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace GetOutside.Database
{
    public interface IILocalDataService
    {
        void Initialize();
        void CreateOutsideActivity(OutsideActivity outsideActivity);
        System.Collections.Generic.List<OutsideActivity> GetOutsideActivity();
        void DeleteOutsideActivity(OutsideActivity outsideActivity);
        void UpdateOutsideActivity(OutsideActivity outsideActivity);

    }
}